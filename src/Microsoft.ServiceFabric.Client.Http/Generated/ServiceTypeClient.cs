// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client;
    using Microsoft.ServiceFabric.Client.Http.Serialization;
    using Microsoft.ServiceFabric.Common;
    using Newtonsoft.Json;

    /// <summary>
    /// Class containing methods for performing ServiceTypeClient operations.
    /// </summary>
    internal partial class ServiceTypeClient : IServiceTypeClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the ServiceTypeClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public ServiceTypeClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<IEnumerable<ServiceTypeInfo>> GetServiceTypeInfoListAsync(
            string applicationTypeName,
            string applicationTypeVersion,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationTypeName.ThrowIfNull(nameof(applicationTypeName));
            applicationTypeVersion.ThrowIfNull(nameof(applicationTypeVersion));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ApplicationTypes/{applicationTypeName}/$/GetServiceTypes";
            url = url.Replace("{applicationTypeName}", Uri.EscapeDataString(applicationTypeName));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            applicationTypeVersion?.AddToQueryParameters(queryParams, $"ApplicationTypeVersion={applicationTypeVersion}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, ServiceTypeInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ServiceTypeInfo> GetServiceTypeInfoByNameAsync(
            string applicationTypeName,
            string applicationTypeVersion,
            string serviceTypeName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationTypeName.ThrowIfNull(nameof(applicationTypeName));
            applicationTypeVersion.ThrowIfNull(nameof(applicationTypeVersion));
            serviceTypeName.ThrowIfNull(nameof(serviceTypeName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ApplicationTypes/{applicationTypeName}/$/GetServiceTypes/{serviceTypeName}";
            url = url.Replace("{applicationTypeName}", Uri.EscapeDataString(applicationTypeName));
            url = url.Replace("{serviceTypeName}", serviceTypeName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            applicationTypeVersion?.AddToQueryParameters(queryParams, $"ApplicationTypeVersion={applicationTypeVersion}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ServiceTypeInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ServiceTypeManifest> GetServiceManifestAsync(
            string applicationTypeName,
            string applicationTypeVersion,
            string serviceManifestName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationTypeName.ThrowIfNull(nameof(applicationTypeName));
            applicationTypeVersion.ThrowIfNull(nameof(applicationTypeVersion));
            serviceManifestName.ThrowIfNull(nameof(serviceManifestName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ApplicationTypes/{applicationTypeName}/$/GetServiceManifest";
            url = url.Replace("{applicationTypeName}", Uri.EscapeDataString(applicationTypeName));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            applicationTypeVersion?.AddToQueryParameters(queryParams, $"ApplicationTypeVersion={applicationTypeVersion}");
            serviceManifestName?.AddToQueryParameters(queryParams, $"ServiceManifestName={serviceManifestName}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ServiceTypeManifestConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<DeployedServiceTypeInfo>> GetDeployedServiceTypeInfoListAsync(
            NodeName nodeName,
            string applicationId,
            string serviceManifestName = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            applicationId.ThrowIfNull(nameof(applicationId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Nodes/{nodeName}/$/GetApplications/{applicationId}/$/GetServiceTypes";
            url = url.Replace("{nodeName}", Uri.EscapeDataString(nodeName.ToString()));
            url = url.Replace("{applicationId}", applicationId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serviceManifestName?.AddToQueryParameters(queryParams, $"ServiceManifestName={serviceManifestName}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, DeployedServiceTypeInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<DeployedServiceTypeInfo>> GetDeployedServiceTypeInfoByNameAsync(
            NodeName nodeName,
            string applicationId,
            string serviceTypeName,
            string serviceManifestName = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            applicationId.ThrowIfNull(nameof(applicationId));
            serviceTypeName.ThrowIfNull(nameof(serviceTypeName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Nodes/{nodeName}/$/GetApplications/{applicationId}/$/GetServiceTypes/{serviceTypeName}";
            url = url.Replace("{nodeName}", Uri.EscapeDataString(nodeName.ToString()));
            url = url.Replace("{applicationId}", applicationId);
            url = url.Replace("{serviceTypeName}", serviceTypeName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serviceManifestName?.AddToQueryParameters(queryParams, $"ServiceManifestName={serviceManifestName}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, DeployedServiceTypeInfoConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
