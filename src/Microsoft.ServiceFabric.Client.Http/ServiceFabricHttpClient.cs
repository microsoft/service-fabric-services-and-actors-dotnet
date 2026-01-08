// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
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
    using Microsoft.ServiceFabric.Client;
    using Microsoft.ServiceFabric.Client.Exceptions;
    using Microsoft.ServiceFabric.Client.Http.Resources;
    using Microsoft.ServiceFabric.Client.Http.Serialization;
    using Microsoft.ServiceFabric.Common;
    using Microsoft.ServiceFabric.Common.Exceptions;
    using Microsoft.ServiceFabric.Common.Security;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a Service Fabric Client to the Http management endpoint (or HttpGatewayEndpoint) of Service Fabric cluster.
    /// </summary>
    public class ServiceFabricHttpClient : ServiceFabricClient, IDisposable
    {   
        private const int MaxTryCount = 2;
        private readonly RandomizedList<Uri> randomizedEndpoints;        
        private readonly SemaphoreSlim refresSecurityLockObj = new SemaphoreSlim(1, 1);
        private readonly Random rand = new Random();
        private readonly TimeSpan maxRetryInterval = TimeSpan.FromSeconds(2);        
        private SecurityType securityType = SecurityType.None;
        private HttpClient httpClient = null;
        private HttpClientHandler innerHandler;
        private HttpClientHandlerWrapper httpClientHandlerWrapper;
        private bool disposed = false;
        private SecuritySettings securitySettings = null;
        private IBearerTokenHandler bearerTokenHandler;
        private IReadOnlyList<DelegatingHandler> delegateHandlers;
        private Func<CancellationToken, Task<SecuritySettings>> refreshSecuritySettingsFunc;

        /// <summary>
        /// ClientType used for telemetry in http gateway.
        /// </summary>
        private string clientTypeHeaderValue;

        /// <summary>
        /// Used to synchronize refresh of security settings.
        /// </summary>
        private object syncObj = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricHttpClient"/> class.
        /// </summary>
        /// <param name="builder">Builder isntance to create ServiceFabricHttpClient from.</param>
        private ServiceFabricHttpClient(ServiceFabricClientBuilder builder)
            : base(builder.Endpoints, builder.SecuritySettings, builder.ClientSettings)
        {
            this.CreateManagementClients();            

            // Validate when Security Settings is null, url can't be https
            if (this.SecuritySettingsFunc == null)
            {
                var scheme = Uri.UriSchemeHttp;
                var invalidClusterEndpoint = this.ClusterEndpoints.FirstOrDefault(url => !string.Equals(url.Scheme, scheme, StringComparison.OrdinalIgnoreCase));

                if (invalidClusterEndpoint != null)
                {
                    throw new InvalidOperationException(string.Format(SR.ErrorUrlScheme, invalidClusterEndpoint.Scheme, scheme));
                }
            }
            else
            {
                // Url validation for secured cluster will be done after SecuritySettings is invoked in Initialize, it can be https only for Claims and X509.
            }

            var seed = (int)DateTime.Now.Ticks;
            this.randomizedEndpoints = new RandomizedList<Uri>(this.ClusterEndpoints, new Random(seed));
            this.ClientId = Guid.NewGuid().ToString();

            // Get information from DI container in ServiceFabricClientBuilder
            this.innerHandler = new HttpClientHandler();
            if (builder.Container.ContainsKey(typeof(HttpClientHandler)))
            {
                this.innerHandler = (HttpClientHandler)builder.Container[typeof(HttpClientHandler)];
            }

            if (builder.Container.ContainsKey(typeof(DelegatingHandler[])))
            {
                this.delegateHandlers = (DelegatingHandler[])builder.Container[typeof(DelegatingHandler[])];
            }

            this.refreshSecuritySettingsFunc = this.SecuritySettingsFunc;
            this.httpClientHandlerWrapper = new HttpClientHandlerWrapper(this.innerHandler);
            this.ClientTypeHeaderValue = Constants.ClientlibClientTypeHeaderValue;
    }

        /// <summary>
        /// Gets the clientId used for tracing.
        /// </summary>
        internal string ClientId { get; }

        /// <summary>
        /// Gets or sets the clientType used for telemetry in http gateway.
        /// </summary>
        internal string ClientTypeHeaderValue
        {
            get
            {
                return this.clientTypeHeaderValue;
            }

            set
            {
                // Append OS platform.
                var osPlatformAppend = "-Windows";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    osPlatformAppend = "-Linux";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    osPlatformAppend = "-OSX";
                }

                this.clientTypeHeaderValue = value + osPlatformAppend;
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
        public override async Task<HttpResponseMessage> SendAsync(
            Func<HttpRequestMessage> requestFunc,
            string relativeUri,
            CancellationToken cancellationToken) 
        {
            var endpoint = this.randomizedEndpoints.GetElement();
            var requestUri = new Uri(endpoint, relativeUri);
            var requestId = Guid.NewGuid().ToString();
            return await this.SendAsyncHandleUnsuccessfulResponse(requestFunc, requestUri, requestId, cancellationToken);            
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        public override void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Creates and initializes a new instance of the <see cref="ServiceFabricHttpClient"/> class.
        /// </summary>
        /// <param name="builder">Builder isntance to create ServiceFabricHttpClient from.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the async operation.</param>
        /// <returns><see cref="ServiceFabricHttpClient"/> instance.</returns>
        internal static async Task<IServiceFabricClient> CreateAsync(
            ServiceFabricClientBuilder builder,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var obj = new ServiceFabricHttpClient(builder);
            await obj.InitializeAsync(cancellationToken);
            return obj;
        }

        /// <summary>
        /// Sends an HTTP get request to cluster http gateway and deserializes the result.
        /// </summary>
        /// <param name="requestFunc">Func to create HttpRequest to send.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="requestId">Request Id for corelation</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The payload of the GET response.</returns>
        /// <exception cref="ServiceFabricException">When the response is not a success.</exception>
        internal async Task SendAsync(
            Func<HttpRequestMessage> requestFunc,
            string relativeUri,
            string requestId,
            CancellationToken cancellationToken)
        {
            // pick a random Uri from endpoints(if more than 1) to send request to.
            var endpoint = this.randomizedEndpoints.GetElement();
            var requestUri = new Uri(endpoint, relativeUri);
            var clientRequestId = this.GetClientRequestIdWithCorrelation(requestId);
            await this.SendAsyncHandleUnsuccessfulResponse(requestFunc, requestUri, clientRequestId, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP get request to cluster http gateway and returns the result as raw json.
        /// </summary>
        /// <param name="requestFunc">Func to create HttpRequest to send.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="requestId">Request Id for corelation</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The payload of the GET response as string.</returns>
        /// <exception cref="ServiceFabricException">When the response is not a success.</exception>
        internal async Task<string> SendAsyncGetResponseAsRawJson(
            Func<HttpRequestMessage> requestFunc,
            string relativeUri,
            string requestId,
            CancellationToken cancellationToken)
        {
            // pick a random Uri from endpoints(if more than 1) to send request to.
            var endpoint = this.randomizedEndpoints.GetElement();
            var requestUri = new Uri(endpoint, relativeUri);
            var clientRequestId = this.GetClientRequestIdWithCorrelation(requestId);
            var response = await this.SendAsyncHandleUnsuccessfulResponse(requestFunc, requestUri, clientRequestId, cancellationToken);
            var retValue = default(string);

            if (response != null && response.Content != null)
            {
                retValue = await response.Content.ReadAsStringAsync();
            }

            return retValue;
        }

        /// <summary>
        /// Sends an HTTP get request to cluster http gateway and deserializes the result.
        /// </summary>
        /// <typeparam name="T">The type of the response payload.</typeparam>
        /// <param name="requestFunc">Func to create HttpRequest to send.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="deserializeFunc">Func to deserialize T.</param>
        /// <param name="requestId">Request Id for corelation</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The payload of the GET response as T.</returns>
        /// <exception cref="ServiceFabricException">When the response is not a success.</exception>
        internal async Task<T> SendAsyncGetResponse<T>(
            Func<HttpRequestMessage> requestFunc,
            string relativeUri, 
            Func<JsonReader, T> deserializeFunc,
            string requestId,
            CancellationToken cancellationToken)
            where T : class
        {
            // pick a random Uri from endpoints(if more than 1) to send request to.
            var endpoint = this.randomizedEndpoints.GetElement();
            var requestUri = new Uri(endpoint, relativeUri);
            var clientRequestId = this.GetClientRequestIdWithCorrelation(requestId);
            var response = await this.SendAsyncHandleUnsuccessfulResponse(requestFunc, requestUri, clientRequestId, cancellationToken);
            var retValue = default(T);

            if (response != null && response.Content != null)
            {
                try
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();
                    using (var streamReader = new StreamReader(contentStream))
                    {
                        using (var reader = new JsonTextReader(streamReader))
                        {
                            retValue = deserializeFunc.Invoke(reader);
                        }
                    }
                }
                catch (JsonReaderException ex)
                {
                    ServiceFabricHttpClientEventSource.Current.WarningMessage($"{clientRequestId}", $"{SR.ErrorCanNotDeserializeResponseFromServer} JsonReaderException: {ex.ToString()}");
                    throw new ServiceFabricException(string.Format(SR.ErrorCanNotDeserializeResponseFromServer, response.StatusCode), ex);
                }
            }

            return retValue;
        }

        /// <summary>
        /// Sends an HTTP get request to cluster http gateway and deserializes the result.
        /// </summary>
        /// <typeparam name="T">The type of the object in List returned as response payload.</typeparam>
        /// <param name="requestFunc">Func to create HttpRequest to send.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="deserializeFunc">Func to deserialize T.</param>
        /// <param name="requestId">Request Id for corelation</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The payload of the GET response as List of T.</returns>
        /// <exception cref="ServiceFabricException">When the response is not a success.</exception>
        internal async Task<IEnumerable<T>> SendAsyncGetResponseAsList<T>(
            Func<HttpRequestMessage> requestFunc,
            string relativeUri,
            Func<JsonReader, T> deserializeFunc,
            string requestId,
            CancellationToken cancellationToken)
        {
            // pick a random Uri from endpoints(if more than 1) to send request to.
            var endpoint = this.randomizedEndpoints.GetElement();
            var requestUri = new Uri(endpoint, relativeUri);
            var clientRequestId = this.GetClientRequestIdWithCorrelation(requestId);
            var response = await this.SendAsyncHandleUnsuccessfulResponse(requestFunc, requestUri, clientRequestId, cancellationToken);
            var retValue = default(IEnumerable<T>);

            if (response != null && response.Content != null)
            {
                try
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();
                    using (var streamReader = new StreamReader(contentStream))
                    {
                        using (var reader = new JsonTextReader(streamReader))
                        {
                            retValue = reader.ReadList(deserializeFunc);
                        }
                    }
                }
                catch (JsonReaderException ex)
                {
                    ServiceFabricHttpClientEventSource.Current.WarningMessage($"{clientRequestId}", $"{SR.ErrorCanNotDeserializeResponseFromServer} JsonReaderException: {ex.ToString()}");
                    throw new ServiceFabricException(string.Format(SR.ErrorCanNotDeserializeResponseFromServer, response.StatusCode), ex);
                }
            }

            return retValue;
        }

        /// <summary>
        /// Sends an HTTP get request to cluster http gateway and deserializes the result.
        /// </summary>
        /// <typeparam name="T">The type of the object in List returned as response payload.</typeparam>
        /// <param name="requestFunc">Func to create HttpRequest to send.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="deserializeFunc">Func to deserialize T.</param>
        /// <param name="requestId">Request Id for corelation</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The payload of the GET response as List of T.</returns>
        /// <exception cref="ServiceFabricException">When the response is not a success.</exception>
        internal async Task<PagedData<T>> SendAsyncGetResponseAsPagedData<T>(
            Func<HttpRequestMessage> requestFunc,
            string relativeUri,
            Func<JsonReader, T> deserializeFunc,
            string requestId,
            CancellationToken cancellationToken)
        {
            // pick a random Uri from endpoints(if more than 1) to send request to.
            var endpoint = this.randomizedEndpoints.GetElement();
            var requestUri = new Uri(endpoint, relativeUri);
            var clientRequestId = this.GetClientRequestIdWithCorrelation(requestId);
            var response = await this.SendAsyncHandleUnsuccessfulResponse(requestFunc, requestUri, clientRequestId, cancellationToken);
            var retValue = default(PagedData<T>);

            if (response != null && response.Content != null)
            {
                try
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();
                    using (var streamReader = new StreamReader(contentStream))
                    {
                        using (var reader = new JsonTextReader(streamReader))
                        {
                            retValue = PagedDataConverter<T>.Deserialize(reader, deserializeFunc);
                        }
                    }
                }
                catch (JsonReaderException ex)
                {
                    ServiceFabricHttpClientEventSource.Current.WarningMessage($"{clientRequestId}", $"{SR.ErrorCanNotDeserializeResponseFromServer} JsonReaderException: {ex.ToString()}");
                    throw new ServiceFabricException(string.Format(SR.ErrorCanNotDeserializeResponseFromServer, response.StatusCode), ex);
                }
            }

            return retValue;
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        /// <param name="disposing">False values indicates the method is being called by the runtime, true value indicates the method is called by the user code.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.httpClient.Dispose();
                    this.httpClient = null;
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Sends an HTTP get request to cluster http gateway.
        /// </summary>
        /// <param name="requestFunc">Func to create HttpRequest to send.</param>
        /// <param name="requestUri">Request Uri.</param>
        /// <param name="clientRequestId">Request Id for corelation</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The payload of the GET response.</returns>
        /// <exception cref="ServiceFabricException">When the response is not a success.</exception>
        private async Task<HttpResponseMessage> SendAsyncHandleUnsuccessfulResponse(
            Func<HttpRequestMessage> requestFunc,
            Uri requestUri,
            string clientRequestId,
            CancellationToken cancellationToken)
        {
            HttpRequestMessage FinalRequestFunc()
            {
                var request = requestFunc.Invoke();
                request.RequestUri = requestUri;

                // Add claims token to request if needed.
                this.bearerTokenHandler.AddTokenToRequest(request);

                // Add client request id to header for corelation on server.
                request.Headers.Add(Constants.ServiceFabricHttpRequestIdHeaderName, $"{clientRequestId}");
                request.Headers.Add(Constants.ServiceFabricHttpClientTypeHeaderName, $"{this.ClientTypeHeaderValue}");
                return request;
            }

            var response = await this.SendAsyncHandleSecurityExceptions(FinalRequestFunc, clientRequestId, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            // Continue with handling the unsuccessful response.
            var message = string.Format(
                SR.ErrorHttpOperationUnsuccessfulFormatString,
                requestUri.ToString(),
                response.StatusCode,
                response.ReasonPhrase,
                response.RequestMessage);

            ServiceFabricHttpClientEventSource.Current.ErrorResponse($"{clientRequestId}", message);

            // Try to get Fabric Error Code if present in response body.
            if (response.Content != null)
            {
                FabricError error = null;

                try
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();
                    if (contentStream.Length != 0)
                    {
                        using (var streamReader = new StreamReader(contentStream))
                        {
                            using (var reader = new JsonTextReader(streamReader))
                            {
                                error = FabricErrorConverter.Deserialize(reader);
                            }
                        }
                    }
                }
                catch (JsonReaderException ex)
                {
                    ServiceFabricHttpClientEventSource.Current.ErrorMessage($"{clientRequestId}", $"Request Url: {requestUri} JsonReaderException: {ex.ToString()}");
                    throw new ServiceFabricException(string.Format(SR.ServerErrorNoMeaningFulResponse, response.StatusCode));
                }

                if (error != null)
                {
                    throw new ServiceFabricException(error.Message, error.ErrorCode ?? FabricErrorCodes.UNKNOWN, false);
                }
                else
                {
                    // Handle NotFound 404, without any ErrorCode.
                    if (response.StatusCode.Equals(HttpStatusCode.NotFound))
                    {
                        throw new ServiceFabricException(SR.ErrorMessageHTTP404, FabricErrorCodes.FABRIC_E_DOES_NOT_EXIST, false);
                    }
                }
            }

            // Couldn't determine FabricError code, throw exception with status code.
            throw new ServiceFabricException(string.Format(SR.ServerErrorNoMeaningFulResponse, response.StatusCode));
        }

        /// <summary>
        /// Send an HTTP request as an asynchronous operation using HttpClient and refreshes security settings if needed.
        /// </summary>
        /// <param name="requestFunc">Delegate to get HTTP request message to send.</param>
        /// <param name="clientRequestId">Request Id.</param>        
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        private async Task<HttpResponseMessage> SendAsyncHandleSecurityExceptions(Func<HttpRequestMessage> requestFunc, string clientRequestId, CancellationToken cancellationToken)
        {
            // Sends request, if Exception is thrown because Server Cert is not validated OR its Forbidden because of invalid client creds,
            // refreshes security settings by calling the func and retires request one more time. If it faisl again Excpetion is thrown.
            var tryCount = 1;

            while (tryCount <= MaxTryCount)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var serverCertValid = true;

                // wait if other thread is refreshing security settings.
                var obj = await this.WaitToUseHttpClient(cancellationToken);
                HttpResponseMessage response = null;

                try
                {
                    // Get the request using the Func as same request cannot be resent.
                    var request = requestFunc.Invoke();
                    ServiceFabricHttpClientEventSource.Current.Send($"{clientRequestId}", $"{request.Method.Method} Request Url: {request.RequestUri}");
                    response = await this.httpClient.SendAsync(request, cancellationToken);                    
                }
                catch (AuthenticationException ex)
                {
                    // Retry on Server cert validation Security error, this check is for dotnet core as AuthentocationException is thrown from
                    // ServerCertificateValidatorHttpWrapper.ValidateServerCertificate 
                    if (tryCount == MaxTryCount)
                    {
                        ServiceFabricHttpClientEventSource.Current.ErrorResponse(
                            $"{clientRequestId}",
                            ex.ToString());
                        throw new InvalidCredentialsException(SR.ErrorRemoteServerCertValidation);
                    }

                    serverCertValid = false;
                    ServiceFabricHttpClientEventSource.Current.RemoteCertValidationError(
                        $"{clientRequestId}",
                        SR.ErrorRemoteCertValidationFailureRetryMessage);
                }
                catch (HttpRequestException ex)
                {        
                    // Retry on Server cert validation Security error, this check is for full dotnet framework as AuthentocationException is thrown from
                    // ServerCertificateValidatorHttpWrapper.ValidateServerCertificate is wrapped inside a HttpRequestException
                    if (ex.InnerException?.InnerException is AuthenticationException)                        
                    {
                        if (tryCount == MaxTryCount)
                        {
                            ServiceFabricHttpClientEventSource.Current.ErrorResponse(
                                $"{clientRequestId}",
                                ex.ToString());
                            throw new InvalidCredentialsException(SR.ErrorRemoteServerCertValidation);
                        }

                        serverCertValid = false;
                        ServiceFabricHttpClientEventSource.Current.RemoteCertValidationError(
                            $"{clientRequestId}",
                            SR.ErrorRemoteCertValidationFailureRetryMessage);
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
                        if (response.StatusCode == HttpStatusCode.Forbidden && this.refreshSecuritySettingsFunc != null)
                        {
                            if (tryCount == MaxTryCount)
                            {
                                ServiceFabricHttpClientEventSource.Current.ErrorResponse(
                                    $"{clientRequestId}",
                                    SR.ErrorClientCredentialsInvalid);
                                throw new InvalidCredentialsException(SR.ErrorClientCredentialsInvalid);
                            }

                            ServiceFabricHttpClientEventSource.Current.ClientCertInvalid(
                                $"{clientRequestId}",
                                SR.ErrorInvalidClientCredentialsRetryMessage);
                        }
                        else
                        {
                            return response;
                        }
                    }
                    else
                    {
                        return response;
                    }
                }

                // refresh security settings.
                await this.RefreshSecuritySettings(obj, cancellationToken);

                // wait before retry
                await Task.Delay(new TimeSpan((long)(this.rand.NextDouble() * this.maxRetryInterval.Ticks)), cancellationToken);
                tryCount++;
            }

            return null;
        }

        private string GetClientRequestIdWithCorrelation(string requestId)
        {
            var clientRequestId = $"{this.ClientId}:{requestId}";

            // use corelationId if available in CallContext, this can be added by consumers of this library for further corelation.
            if (ServiceFabricHttpClientCallContext.TryGetCorrelationId(out var correlationId))
            {
                clientRequestId = $"{clientRequestId}:External:{correlationId}";
            }

            return clientRequestId;
        }

        private async Task<object> WaitToUseHttpClient(CancellationToken cancellationToken)
        {
            await this.refresSecurityLockObj.WaitAsync(cancellationToken);
            try
            {
                return this.syncObj;
            }
            finally
            {
                this.refresSecurityLockObj.Release();
            }
        }

        private async Task RefreshSecuritySettings(object obj, CancellationToken cancellationToken)
        {
            await this.refresSecurityLockObj.WaitAsync(cancellationToken);
            try
            {
                // only refresh if syncObj is same as the object acquired by thread from WaitToUseHttpClient().
                // If its not same, that means that some other thread has refreshed.
                if (object.ReferenceEquals(obj, this.syncObj))
                {
                    if (this.refreshSecuritySettingsFunc != null)
                    {
                        var newSecuritySettings = await this.refreshSecuritySettingsFunc.Invoke(cancellationToken);

                        if (newSecuritySettings.SecurityType != this.securityType)
                        {
                            throw new InvalidOperationException(SR.ErrorCannotChangeSecurityType);
                        }

                        // Refresh settings for HttpClientHandler.
                        this.securitySettings = newSecuritySettings;
                        await this.bearerTokenHandler.RefreshTokenAsync(this.securitySettings, cancellationToken);
                        this.httpClientHandlerWrapper.RefreshSecuritySettings(newSecuritySettings);
                    }
                    
                    this.syncObj = new object();
                }
            }
            finally
            {
                this.refresSecurityLockObj.Release();
            }
        }

        private async Task InitializeAsync(CancellationToken cancellationToken)
        {
            // setup security settings
            if (this.SecuritySettingsFunc != null)
            {
                this.securitySettings = await this.SecuritySettingsFunc.Invoke(cancellationToken);
            }

            this.securityType = this.securitySettings?.SecurityType ?? SecurityType.None;

            // Validate Url scheme.
            var scheme = Uri.UriSchemeHttp;
            if (this.securityType == SecurityType.X509 ||
                this.securityType == SecurityType.Claims)
            {
                scheme = Uri.UriSchemeHttps;
            }

            var invalidClusterEndpoint = this.ClusterEndpoints.FirstOrDefault(url => !string.Equals(url.Scheme, scheme, StringComparison.OrdinalIgnoreCase));
            if (invalidClusterEndpoint != null)
            {
                throw new InvalidOperationException(string.Format(SR.ErrorUrlScheme, invalidClusterEndpoint.Scheme, scheme));
            }

            if (this.securitySettings != null)
            {
                this.httpClientHandlerWrapper.ConfigureSecuritySettings(this.securitySettings);
            }

            this.httpClient = this.CreateHttpClient(cancellationToken);
            await this.InitializeBearerTokenHandlerAsync(cancellationToken);
        }

        private HttpClient CreateHttpClient(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Chain Delegating Handlers.
            HttpMessageHandler pipeline = this.innerHandler;
            if (this.delegateHandlers != null)
            {
                for (var i = this.delegateHandlers.Count - 1; i >= 0; i--)
                {
                    var handler = this.delegateHandlers[i];
                    handler.InnerHandler = pipeline;
                    pipeline = handler;
                }
            }

            var httpClientInstance = new HttpClient(pipeline, true);

            httpClientInstance.DefaultRequestHeaders.ExpectContinue = true;

            if (this.ClientSettings?.ClientTimeout != null)
            {
                httpClientInstance.Timeout = (TimeSpan)this.ClientSettings.ClientTimeout;
            }

            return httpClientInstance;
        }

        private async Task InitializeBearerTokenHandlerAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // set bearer token to default to allow call for GetAadMetadata.
            this.bearerTokenHandler = new DefaultTokenHandler();

            if (this.securityType.Equals(SecurityType.Claims))
            {
                // Build Handlers for derived types first.
                if (this.securitySettings is AzureActiveDirectorySecuritySettings aadSecuritySettings)
                {
                    // Get AaadMetadata for AzureActiveDirectorySecuritySettings, if user has provided the Func for getting Bearer token.
                    if (aadSecuritySettings.GetClaimsToken != null)
                    {
                        // get AadMetadata from cluster using another http client.
                        var aadMetaData = await this.Cluster.GetAadMetadataAsync(cancellationToken: cancellationToken);
                        this.bearerTokenHandler = new AADTokenHandler(aadMetaData.Metadata);
                    }
                    else
                    {
                        this.bearerTokenHandler = new AADTokenHandler();
                    }
                }
                else if (this.securitySettings is DstsClaimsSecuritySettings dstsSecuritySettings)
                {
                    // Get AaadMetadata for AzureActiveDirectorySecuritySettings, if user has provided the Func for getting Bearer token.
                    if (dstsSecuritySettings.GetClaimsToken != null)
                    {
                        // get AadMetadata from cluster using another http client.
                        var metaData = await this.Cluster.GetTokenServiceMetadtaAsync(cancellationToken: cancellationToken);
                        this.bearerTokenHandler = new DstsTokenHandler(metaData);
                    }
                    else
                    {
                        this.bearerTokenHandler = new DstsTokenHandler();
                    }
                }
                else
                {
                    this.bearerTokenHandler = new ClaimsTokenHandler();
                }
            }

            await this.bearerTokenHandler.InitializeTokenAsync(this.securitySettings, cancellationToken);
        }

        private void ChainDelegatingHandlers(HttpMessageHandler pipeline)
        {
            // chain delegating handlers if available.
            for (var i = this.delegateHandlers.Count - 1; i >= 0; i--)
            {
                var handler = this.delegateHandlers[i];
                handler.InnerHandler = pipeline;
                pipeline = handler;
            }
        }

        private void CreateManagementClients()
        {
            this.Applications = new ApplicationClient(this);
            this.ApplicationTypes = new ApplicationTypeClient(this);
            this.BackupRestore = new BackupRestoreClient(this);
            this.ChaosClient = new ChaosClient(this);
            this.Cluster = new ClusterClient(this);
            this.CodePackages = new CodePackageClient(this);
            this.ComposeDeployments = new ComposeDeploymentClient(this);
            this.Faults = new FaultsClient(this);
            this.ImageStore = new ImageStoreClient(this);
            this.Infrastructure = new InfrastructureClient(this);
            this.Nodes = new NodeClient(this);
            this.Partitions = new PartitionClient(this);
            this.Properties = new PropertyManagementClient(this);
            this.Replicas = new ReplicaClient(this);
            this.Repairs = new RepairManagementClient(this);
            this.Services = new ServiceClient(this);
            this.ServicePackages = new ServicePackageClient(this);
            this.ServiceTypes = new ServiceTypeClient(this);
            this.EventsStore = new EventsStoreClient(this);
            this.MeshApplications = new MeshApplicationsClient(this);
            this.MeshVolumes = new MeshVolumesClient(this);
            this.MeshSecrets = new MeshSecretsClient(this);
            this.MeshSecretValues = new MeshSecretValuesClient(this);
            this.MeshNetworks = new MeshNetworksClient(this);
            this.MeshGateways = new MeshGatewaysClient(this);
            this.MeshServices = new MeshServicesClient(this);
            this.MeshServiceReplicas = new MeshServiceReplicasClient(this);
            this.MeshCodePackages = new MeshCodePackagesClient(this);
        }
    }
}
