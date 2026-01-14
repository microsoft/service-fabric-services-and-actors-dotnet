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
    /// Class containing methods for performing MeshCodePackagesClient operations.
    /// </summary>
    internal partial class MeshCodePackagesClient : IMeshCodePackagesClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the MeshCodePackagesClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public MeshCodePackagesClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<ContainerLogs> GetContainerLogsAsync(
            string applicationResourceName,
            string serviceResourceName,
            string replicaName,
            string codePackageName,
            string tail = default(string),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationResourceName.ThrowIfNull(nameof(applicationResourceName));
            serviceResourceName.ThrowIfNull(nameof(serviceResourceName));
            replicaName.ThrowIfNull(nameof(replicaName));
            codePackageName.ThrowIfNull(nameof(codePackageName));
            var requestId = Guid.NewGuid().ToString();
            var url = "Resources/Applications/{applicationResourceName}/Services/{serviceResourceName}/Replicas/{replicaName}/CodePackages/{codePackageName}/Logs";
            url = url.Replace("{applicationResourceName}", applicationResourceName);
            url = url.Replace("{serviceResourceName}", serviceResourceName);
            url = url.Replace("{replicaName}", replicaName);
            url = url.Replace("{codePackageName}", Uri.EscapeDataString(codePackageName));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            tail?.AddToQueryParameters(queryParams, $"Tail={tail}");
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ContainerLogsConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
