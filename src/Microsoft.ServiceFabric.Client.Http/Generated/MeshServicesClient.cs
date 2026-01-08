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
    /// Class containing methods for performing MeshServicesClient operations.
    /// </summary>
    internal partial class MeshServicesClient : IMeshServicesClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the MeshServicesClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public MeshServicesClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<ServiceResourceDescription> GetAsync(
            string applicationResourceName,
            string serviceResourceName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationResourceName.ThrowIfNull(nameof(applicationResourceName));
            serviceResourceName.ThrowIfNull(nameof(serviceResourceName));
            var requestId = Guid.NewGuid().ToString();
            var url = "Resources/Applications/{applicationResourceName}/Services/{serviceResourceName}";
            url = url.Replace("{applicationResourceName}", applicationResourceName);
            url = url.Replace("{serviceResourceName}", serviceResourceName);
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ServiceResourceDescriptionConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<ServiceResourceDescription>> ListAsync(
            string applicationResourceName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationResourceName.ThrowIfNull(nameof(applicationResourceName));
            var requestId = Guid.NewGuid().ToString();
            var url = "Resources/Applications/{applicationResourceName}/Services";
            url = url.Replace("{applicationResourceName}", applicationResourceName);
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

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, ServiceResourceDescriptionConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
