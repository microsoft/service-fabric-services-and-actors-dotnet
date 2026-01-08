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
    /// Class containing methods for performing MeshGatewaysClient operations.
    /// </summary>
    internal partial class MeshGatewaysClient : IMeshGatewaysClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the MeshGatewaysClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public MeshGatewaysClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<GatewayResourceDescription> CreateOrUpdateAsync(
            string gatewayResourceName,
            GatewayResourceDescription gatewayResourceDescription,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            gatewayResourceName.ThrowIfNull(nameof(gatewayResourceName));
            gatewayResourceDescription.ThrowIfNull(nameof(gatewayResourceDescription));
            var requestId = Guid.NewGuid().ToString();
            var url = "Resources/Gateways/{gatewayResourceName}";
            url = url.Replace("{gatewayResourceName}", gatewayResourceName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.4-preview");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                GatewayResourceDescriptionConverter.Serialize(new JsonTextWriter(sw), gatewayResourceDescription);
                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Put,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, GatewayResourceDescriptionConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<GatewayResourceDescription> GetAsync(
            string gatewayResourceName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            gatewayResourceName.ThrowIfNull(nameof(gatewayResourceName));
            var requestId = Guid.NewGuid().ToString();
            var url = "Resources/Gateways/{gatewayResourceName}";
            url = url.Replace("{gatewayResourceName}", gatewayResourceName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.4-preview");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, GatewayResourceDescriptionConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteAsync(
            string gatewayResourceName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            gatewayResourceName.ThrowIfNull(nameof(gatewayResourceName));
            var requestId = Guid.NewGuid().ToString();
            var url = "Resources/Gateways/{gatewayResourceName}";
            url = url.Replace("{gatewayResourceName}", gatewayResourceName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.4-preview");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Delete,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<GatewayResourceDescription>> ListAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestId = Guid.NewGuid().ToString();
            var url = "Resources/Gateways";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.4-preview");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, GatewayResourceDescriptionConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
