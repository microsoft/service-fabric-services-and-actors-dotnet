// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Client.Exceptions;
using Microsoft.ServiceFabric.Client.Http.Serialization;
using Microsoft.ServiceFabric.Common;
using Microsoft.ServiceFabric.Common.Exceptions;
using Microsoft.ServiceFabric.Common.Security;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http
{
    /// <summary>
    /// Represents a Service Fabric Client to the Http management endpoint (or HttpGatewayEndpoint) of Service Fabric cluster.
    /// </summary>
    public class ServiceFabricHttpClient : ServiceFabricClient, IDisposable
    {
        const int MaxTryCount = 2;
        readonly RandomizedList<Uri> randomizedEndpoints;
        readonly SemaphoreSlim refreshSecurityLockObj = new SemaphoreSlim(1, 1);
        readonly Random rand = new Random();
        readonly TimeSpan maxRetryInterval = TimeSpan.FromSeconds(2);
        SecurityType securityType = SecurityType.None;
        HttpClient httpClient = null;
        HttpClientHandler innerHandler;
        HttpClientHandlerWrapper httpClientHandlerWrapper;
        bool disposed = false;
        SecuritySettings securitySettings = null;
        IBearerTokenHandler bearerTokenHandler;
        IReadOnlyList<DelegatingHandler> delegateHandlers;
        Func<CancellationToken, Task<SecuritySettings>> refreshSecuritySettingsFunc;
        string clientTypeHeaderValue;
        object syncObj = new object();

        ServiceFabricHttpClient(ServiceFabricClientBuilder builder)
            : base(builder.Endpoints, builder.SecuritySettings, builder.ClientSettings)
        {
            CreateManagementClients();

            // Validate when Security Settings is null, url can't be https
            if (SecuritySettingsFunc == null)
            {
                string scheme = Uri.UriSchemeHttp;
                Uri invalidClusterEndpoint = ClusterEndpoints.FirstOrDefault(url => !string.Equals(url.Scheme, scheme, StringComparison.OrdinalIgnoreCase));
                if (invalidClusterEndpoint != null)
                    throw new InvalidOperationException(string.Format(SR.ErrorUrlScheme, invalidClusterEndpoint.Scheme, scheme));
            }

            // Url validation for secured cluster will be done after SecuritySettings is invoked in Initialize, it can be https only for Claims and X509.

            var seed = (int)DateTime.Now.Ticks;
            randomizedEndpoints = new RandomizedList<Uri>(ClusterEndpoints, new Random(seed));
            ClientId = Guid.NewGuid().ToString();

            // Get information from DI container in ServiceFabricClientBuilder
            innerHandler = new HttpClientHandler();
            if (builder.Container.ContainsKey(typeof(HttpClientHandler)))
                innerHandler = (HttpClientHandler)builder.Container[typeof(HttpClientHandler)];

            if (builder.Container.ContainsKey(typeof(DelegatingHandler[])))
                delegateHandlers = (DelegatingHandler[])builder.Container[typeof(DelegatingHandler[])];

            refreshSecuritySettingsFunc = SecuritySettingsFunc;
            httpClientHandlerWrapper = new HttpClientHandlerWrapper(innerHandler);
            ClientTypeHeaderValue = Constants.ClientlibClientTypeHeaderValue;
    }

        // Gets the clientId used for tracing.
        internal string ClientId { get; }

        // Gets or sets the clientType used for telemetry in http gateway.
        internal string ClientTypeHeaderValue
        {
            get { return clientTypeHeaderValue; }

            set
            {
                // Append OS platform.
                var osPlatformAppend = "-Windows";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    osPlatformAppend = "-Linux";
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    osPlatformAppend = "-OSX";

                clientTypeHeaderValue = value + osPlatformAppend;
            }
        }

        /// <summary>
        /// Sends an HTTP get request to cluster http gateway.
        /// </summary>
        /// <param name="requestFunc">Func to create HttpRequest to send.</param>
        /// <param name="relativeUri">Relative request Uri.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The payload of the GET response.</returns>
        /// <exception cref="ServiceFabricException">When the response is not a success.</exception>
        public override async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> requestFunc, string relativeUri, CancellationToken cancellationToken) 
        {
            var endpoint = randomizedEndpoints.GetElement();
            var requestUri = new Uri(endpoint, relativeUri);
            var requestId = Guid.NewGuid().ToString();
            return await SendAsyncHandleUnsuccessfulResponse(requestFunc, requestUri, requestId, cancellationToken);
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        /// <param name="disposing">False values indicates the method is being called by the runtime, true value indicates the method is called by the user code.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }

                disposed = true;
            }
        }

        internal static async Task<IServiceFabricClient> CreateAsync(ServiceFabricClientBuilder builder, CancellationToken cancellationToken)
        {
            var obj = new ServiceFabricHttpClient(builder);
            await obj.InitializeAsync(cancellationToken);
            return obj;
        }

        internal async Task SendAsync(Func<HttpRequestMessage> requestFunc, string relativeUri, string requestId, CancellationToken cancellationToken)
        {
            Uri endpoint = randomizedEndpoints.GetElement();
            Uri requestUri = new Uri(endpoint, relativeUri);
            string clientRequestId = GetClientRequestIdWithCorrelation(requestId);
            await SendAsyncHandleUnsuccessfulResponse(requestFunc, requestUri, clientRequestId, cancellationToken);
        }

        internal async Task<string> SendAsyncGetResponseAsRawJson(Func<HttpRequestMessage> requestFunc, string relativeUri, string requestId, CancellationToken cancellationToken)
        {
            Uri endpoint = randomizedEndpoints.GetElement();
            Uri requestUri = new Uri(endpoint, relativeUri);
            string clientRequestId = GetClientRequestIdWithCorrelation(requestId);
            HttpResponseMessage response = await SendAsyncHandleUnsuccessfulResponse(requestFunc, requestUri, clientRequestId, cancellationToken);
            string retValue = default;
            if (response != null && response.Content != null)
                retValue = await response.Content.ReadAsStringAsync();
            return retValue;
        }

        internal async Task<T> SendAsyncGetResponse<T>(Func<HttpRequestMessage> requestFunc, string relativeUri,  Func<JsonReader, T> deserializeFunc, string requestId, CancellationToken cancellationToken)
            where T : class
        {
            Uri endpoint = randomizedEndpoints.GetElement();
            Uri requestUri = new Uri(endpoint, relativeUri);
            string clientRequestId = GetClientRequestIdWithCorrelation(requestId);
            HttpResponseMessage response = await SendAsyncHandleUnsuccessfulResponse(requestFunc, requestUri, clientRequestId, cancellationToken);
            T retValue = default;

            if (response != null && response.Content != null)
            {
                try
                {
                    Stream contentStream = await response.Content.ReadAsStreamAsync();
                    using var streamReader = new StreamReader(contentStream);
                    using var reader = new JsonTextReader(streamReader);
                    retValue = deserializeFunc.Invoke(reader);
                }
                catch (JsonReaderException ex)
                {
                    ServiceFabricHttpClientEventSource.Current.WarningMessage($"{clientRequestId}", $"{SR.ErrorCanNotDeserializeResponseFromServer} JsonReaderException: {ex.ToString()}");
                    throw new ServiceFabricException(string.Format(SR.ErrorCanNotDeserializeResponseFromServer, response.StatusCode), ex);
                }
            }

            return retValue;
        }

        internal async Task<IEnumerable<T>> SendAsyncGetResponseAsList<T>(Func<HttpRequestMessage> requestFunc, string relativeUri, Func<JsonReader, T> deserializeFunc, string requestId, CancellationToken cancellationToken)
        {
            Uri endpoint = randomizedEndpoints.GetElement();
            Uri requestUri = new Uri(endpoint, relativeUri);
            string clientRequestId = GetClientRequestIdWithCorrelation(requestId);
            HttpResponseMessage response = await SendAsyncHandleUnsuccessfulResponse(requestFunc, requestUri, clientRequestId, cancellationToken);
            IEnumerable<T> retValue = default;

            if (response != null && response.Content != null)
            {
                try
                {
                    Stream contentStream = await response.Content.ReadAsStreamAsync();
                    using var streamReader = new StreamReader(contentStream);
                    using var reader = new JsonTextReader(streamReader);
                    retValue = reader.ReadList(deserializeFunc);
                }
                catch (JsonReaderException ex)
                {
                    ServiceFabricHttpClientEventSource.Current.WarningMessage($"{clientRequestId}", $"{SR.ErrorCanNotDeserializeResponseFromServer} JsonReaderException: {ex.ToString()}");
                    throw new ServiceFabricException(string.Format(SR.ErrorCanNotDeserializeResponseFromServer, response.StatusCode), ex);
                }
            }

            return retValue;
        }

        internal async Task<PagedData<T>> SendAsyncGetResponseAsPagedData<T>(Func<HttpRequestMessage> requestFunc, string relativeUri, Func<JsonReader, T> deserializeFunc, string requestId, CancellationToken cancellationToken)
        {
            Uri endpoint = randomizedEndpoints.GetElement();
            Uri requestUri = new Uri(endpoint, relativeUri);
            string clientRequestId = GetClientRequestIdWithCorrelation(requestId);
            HttpResponseMessage response = await SendAsyncHandleUnsuccessfulResponse(requestFunc, requestUri, clientRequestId, cancellationToken);
            PagedData<T> retValue = default;

            if (response != null && response.Content != null)
            {
                try
                {
                    Stream contentStream = await response.Content.ReadAsStreamAsync();
                    using var streamReader = new StreamReader(contentStream);
                    using var reader = new JsonTextReader(streamReader);
                    retValue = PagedDataConverter<T>.Deserialize(reader, deserializeFunc);
                }
                catch (JsonReaderException ex)
                {
                    ServiceFabricHttpClientEventSource.Current.WarningMessage($"{clientRequestId}", $"{SR.ErrorCanNotDeserializeResponseFromServer} JsonReaderException: {ex.ToString()}");
                    throw new ServiceFabricException(string.Format(SR.ErrorCanNotDeserializeResponseFromServer, response.StatusCode), ex);
                }
            }

            return retValue;
        }

        async Task<HttpResponseMessage> SendAsyncHandleUnsuccessfulResponse(Func<HttpRequestMessage> requestFunc, Uri requestUri, string clientRequestId, CancellationToken cancellationToken)
        {
            HttpRequestMessage FinalRequestFunc()
            {
                HttpRequestMessage request = requestFunc.Invoke();
                request.RequestUri = requestUri;

                // Add claims token to request if needed.
                bearerTokenHandler.AddTokenToRequest(request);

                // Add client request id to header for corelation on server.
                request.Headers.Add(Constants.ServiceFabricHttpRequestIdHeaderName, $"{clientRequestId}");
                request.Headers.Add(Constants.ServiceFabricHttpClientTypeHeaderName, $"{ClientTypeHeaderValue}");
                return request;
            }

            HttpResponseMessage response = await SendAsyncHandleSecurityExceptions(FinalRequestFunc, clientRequestId, cancellationToken);

            if (response.IsSuccessStatusCode)
                return response;

            // Continue with handling the unsuccessful response.
            var message = string.Format(SR.ErrorHttpOperationUnsuccessfulFormatString, requestUri.ToString(), response.StatusCode, response.ReasonPhrase, response.RequestMessage);
            ServiceFabricHttpClientEventSource.Current.ErrorResponse($"{clientRequestId}", message);

            // Try to get Fabric Error Code if present in response body.
            if (response.Content != null)
            {
                FabricError error = null;

                try
                {
                    Stream contentStream = await response.Content.ReadAsStreamAsync();
                    if (contentStream.Length != 0)
                    {
                        using var streamReader = new StreamReader(contentStream);
                        using var reader = new JsonTextReader(streamReader);
                        error = FabricErrorConverter.Deserialize(reader);
                    }
                }
                catch (JsonReaderException ex)
                {
                    ServiceFabricHttpClientEventSource.Current.ErrorMessage($"{clientRequestId}", $"Request Url: {requestUri} JsonReaderException: {ex.ToString()}");
                    throw new ServiceFabricException(string.Format(SR.ServerErrorNoMeaningFulResponse, response.StatusCode));
                }

                if (error != null)
                    throw new ServiceFabricException(error.Message, error.ErrorCode ?? FabricErrorCodes.UNKNOWN, false);
                else if (response.StatusCode.Equals(HttpStatusCode.NotFound))
                    throw new ServiceFabricException(SR.ErrorMessageHTTP404, FabricErrorCodes.FABRIC_E_DOES_NOT_EXIST, false);
            }

            // Couldn't determine FabricError code, throw exception with status code.
            throw new ServiceFabricException(string.Format(SR.ServerErrorNoMeaningFulResponse, response.StatusCode));
        }

        async Task<HttpResponseMessage> SendAsyncHandleSecurityExceptions(Func<HttpRequestMessage> requestFunc, string clientRequestId, CancellationToken cancellationToken)
        {
            // Sends request, if Exception is thrown because Server Cert is not validated OR its Forbidden because of invalid client creds,
            // refreshes security settings by calling the func and retires request one more time. If it fails again Exception is thrown.
            var tryCount = 1;

            while (tryCount <= MaxTryCount)
            {
                cancellationToken.ThrowIfCancellationRequested();
                bool serverCertValid = true;

                // wait if other thread is refreshing security settings.
                object obj = await WaitToUseHttpClient(cancellationToken);
                HttpResponseMessage response = null;

                try
                {
                    // Get the request using the Func as same request cannot be resent.
                    HttpRequestMessage request = requestFunc.Invoke();
                    ServiceFabricHttpClientEventSource.Current.Send($"{clientRequestId}", $"{request.Method.Method} Request Url: {request.RequestUri}");
                    response = await httpClient.SendAsync(request, cancellationToken);
                }
                catch (AuthenticationException ex)
                {
                    // Retry on Server cert validation Security error, this check is for dotnet core as AuthenticationException is thrown from
                    // ServerCertificateValidatorHttpWrapper.ValidateServerCertificate 
                    if (tryCount == MaxTryCount)
                    {
                        ServiceFabricHttpClientEventSource.Current.ErrorResponse($"{clientRequestId}", ex.ToString());
                        throw new InvalidCredentialsException(SR.ErrorRemoteServerCertValidation);
                    }

                    serverCertValid = false;
                    ServiceFabricHttpClientEventSource.Current.RemoteCertValidationError($"{clientRequestId}", SR.ErrorRemoteCertValidationFailureRetryMessage);
                }
                catch (HttpRequestException ex)
                {
                    // Retry on Server cert validation Security error, this check is for full dotnet framework as AuthenticationException is thrown from
                    // ServerCertificateValidatorHttpWrapper.ValidateServerCertificate is wrapped inside a HttpRequestException
                    if (ex.InnerException?.InnerException is AuthenticationException)
                    {
                        if (tryCount == MaxTryCount)
                        {
                            ServiceFabricHttpClientEventSource.Current.ErrorResponse($"{clientRequestId}", ex.ToString());
                            throw new InvalidCredentialsException(SR.ErrorRemoteServerCertValidation);
                        }

                        serverCertValid = false;
                        ServiceFabricHttpClientEventSource.Current.RemoteCertValidationError($"{clientRequestId}", SR.ErrorRemoteCertValidationFailureRetryMessage);
                    }
                    else
                    {
                        ServiceFabricHttpClientEventSource.Current.ErrorResponse($"{clientRequestId}", ex.ToString());
                        throw new ServiceFabricRequestException(ex.Message, ex);
                    }
                }

                if (serverCertValid)
                {
                    // server cert was validated, check if client credentials were valid.
                    if (!response.IsSuccessStatusCode)
                    {
                        // RefreshSecurity Settings and try again,
                        if (response.StatusCode == HttpStatusCode.Forbidden && refreshSecuritySettingsFunc != null)
                        {
                            if (tryCount == MaxTryCount)
                            {
                                ServiceFabricHttpClientEventSource.Current.ErrorResponse($"{clientRequestId}", SR.ErrorClientCredentialsInvalid);
                                throw new InvalidCredentialsException(SR.ErrorClientCredentialsInvalid);
                            }

                            ServiceFabricHttpClientEventSource.Current.ClientCertInvalid($"{clientRequestId}", SR.ErrorInvalidClientCredentialsRetryMessage);
                        }
                        else
                            return response;
                    }
                    else
                        return response;
                }

                // refresh security settings.
                await RefreshSecuritySettings(obj, cancellationToken);

                // wait before retry
                await Task.Delay(new TimeSpan((long)(rand.NextDouble() * maxRetryInterval.Ticks)), cancellationToken);
                tryCount++;
            }

            return null;
        }

        string GetClientRequestIdWithCorrelation(string requestId)
        {
            string clientRequestId = $"{ClientId}:{requestId}";

            // use corelationId if available in CallContext, this can be added by consumers of this library for further corelation.
            if (ServiceFabricHttpClientCallContext.TryGetCorrelationId(out var correlationId))
                clientRequestId = $"{clientRequestId}:External:{correlationId}";

            return clientRequestId;
        }

        async Task<object> WaitToUseHttpClient(CancellationToken cancellationToken)
        {
            await refreshSecurityLockObj.WaitAsync(cancellationToken);
            try
            {
                return syncObj;
            }
            finally
            {
                refreshSecurityLockObj.Release();
            }
        }

        async Task RefreshSecuritySettings(object obj, CancellationToken cancellationToken)
        {
            await refreshSecurityLockObj.WaitAsync(cancellationToken);
            try
            {
                // only refresh if syncObj is same as the object acquired by thread from WaitToUseHttpClient().
                // If its not same, that means that some other thread has refreshed.
                if (ReferenceEquals(obj, syncObj))
                {
                    if (refreshSecuritySettingsFunc != null)
                    {
                        SecuritySettings newSecuritySettings = await refreshSecuritySettingsFunc.Invoke(cancellationToken);

                        if (newSecuritySettings.SecurityType != securityType)
                            throw new InvalidOperationException(SR.ErrorCannotChangeSecurityType);

                        // Refresh settings for HttpClientHandler.
                        securitySettings = newSecuritySettings;
                        await bearerTokenHandler.RefreshTokenAsync(securitySettings, cancellationToken);
                        httpClientHandlerWrapper.RefreshSecuritySettings(newSecuritySettings);
                    }
                    
                    syncObj = new object();
                }
            }
            finally
            {
                refreshSecurityLockObj.Release();
            }
        }

        async Task InitializeAsync(CancellationToken cancellationToken)
        {
            // setup security settings
            if (SecuritySettingsFunc != null)
                securitySettings = await SecuritySettingsFunc.Invoke(cancellationToken);

            securityType = securitySettings?.SecurityType ?? SecurityType.None;

            // Validate Url scheme.
            var scheme = Uri.UriSchemeHttp;
            if (securityType == SecurityType.X509 || securityType == SecurityType.Claims)
                scheme = Uri.UriSchemeHttps;

            Uri invalidClusterEndpoint = ClusterEndpoints.FirstOrDefault(url => !string.Equals(url.Scheme, scheme, StringComparison.OrdinalIgnoreCase));
            if (invalidClusterEndpoint != null)
                throw new InvalidOperationException(string.Format(SR.ErrorUrlScheme, invalidClusterEndpoint.Scheme, scheme));

            if (securitySettings != null)
                httpClientHandlerWrapper.ConfigureSecuritySettings(securitySettings);

            httpClient = CreateHttpClient(cancellationToken);
            await InitializeBearerTokenHandlerAsync(cancellationToken);
        }

        HttpClient CreateHttpClient(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Chain Delegating Handlers.
            HttpMessageHandler pipeline = innerHandler;
            if (delegateHandlers != null)
            {
                for (int i = delegateHandlers.Count - 1; i >= 0; i--)
                {
                    DelegatingHandler handler = delegateHandlers[i];
                    handler.InnerHandler = pipeline;
                    pipeline = handler;
                }
            }

            var httpClientInstance = new HttpClient(pipeline, true);

            httpClientInstance.DefaultRequestHeaders.ExpectContinue = true;

            if (ClientSettings?.ClientTimeout != null)
                httpClientInstance.Timeout = (TimeSpan)ClientSettings.ClientTimeout;

            return httpClientInstance;
        }

        async Task InitializeBearerTokenHandlerAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // set bearer token to default to allow call for GetAadMetadata.
            bearerTokenHandler = new DefaultTokenHandler();

            if (securityType.Equals(SecurityType.Claims))
            {
                // Build Handlers for derived types first.
                if (securitySettings is AzureActiveDirectorySecuritySettings aadSecuritySettings)
                {
                    // Get AaadMetadata for AzureActiveDirectorySecuritySettings, if user has provided the Func for getting Bearer token.
                    if (aadSecuritySettings.GetClaimsToken != null)
                    {
                        // get AadMetadata from cluster using another http client.
                        AadMetadataObject aadMetaData = await Cluster.GetAadMetadataAsync(cancellationToken: cancellationToken);
                        bearerTokenHandler = new AADTokenHandler(aadMetaData.Metadata);
                    }
                    else
                        bearerTokenHandler = new AADTokenHandler();
                }
                else if (securitySettings is DstsClaimsSecuritySettings dstsSecuritySettings)
                {
                    // Get AaadMetadata for AzureActiveDirectorySecuritySettings, if user has provided the Func for getting Bearer token.
                    if (dstsSecuritySettings.GetClaimsToken != null)
                    {
                        // get AadMetadata from cluster using another http client.
                        TokenServiceMetadata metaData = await Cluster.GetTokenServiceMetadtaAsync(cancellationToken: cancellationToken);
                        bearerTokenHandler = new DstsTokenHandler(metaData);
                    }
                    else
                        bearerTokenHandler = new DstsTokenHandler();
                }
                else
                    bearerTokenHandler = new ClaimsTokenHandler();
            }

            await bearerTokenHandler.InitializeTokenAsync(securitySettings, cancellationToken);
        }

        void CreateManagementClients()
        {
            Applications = new ApplicationClient(this);
            ApplicationTypes = new ApplicationTypeClient(this);
            BackupRestore = new BackupRestoreClient(this);
            ChaosClient = new ChaosClient(this);
            Cluster = new ClusterClient(this);
            CodePackages = new CodePackageClient(this);
            ComposeDeployments = new ComposeDeploymentClient(this);
            Faults = new FaultsClient(this);
            ImageStore = new ImageStoreClient(this);
            Infrastructure = new InfrastructureClient(this);
            Nodes = new NodeClient(this);
            Partitions = new PartitionClient(this);
            Properties = new PropertyManagementClient(this);
            Replicas = new ReplicaClient(this);
            Repairs = new RepairManagementClient(this);
            Services = new ServiceClient(this);
            ServicePackages = new ServicePackageClient(this);
            ServiceTypes = new ServiceTypeClient(this);
            EventsStore = new EventsStoreClient(this);
            MeshApplications = new MeshApplicationsClient(this);
            MeshVolumes = new MeshVolumesClient(this);
            MeshSecrets = new MeshSecretsClient(this);
            MeshSecretValues = new MeshSecretValuesClient(this);
            MeshNetworks = new MeshNetworksClient(this);
            MeshServices = new MeshServicesClient(this);
            MeshServiceReplicas = new MeshServiceReplicasClient(this);
            MeshCodePackages = new MeshCodePackagesClient(this);
        }
    }
}
