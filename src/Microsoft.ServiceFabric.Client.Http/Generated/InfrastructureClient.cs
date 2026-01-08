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
    /// Class containing methods for performing InfrastructureClient operations.
    /// </summary>
    internal partial class InfrastructureClient : IInfrastructureClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the InfrastructureClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public InfrastructureClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<string> InvokeInfrastructureCommandAsync(
            string command,
            string serviceId = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            command.ThrowIfNull(nameof(command));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/InvokeInfrastructureCommand";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            command?.AddToQueryParameters(queryParams, $"Command={command}");
            serviceId?.AddToQueryParameters(queryParams, $"ServiceId={serviceId}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsRawJson(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<string> InvokeInfrastructureQueryAsync(
            string command,
            string serviceId = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            command.ThrowIfNull(nameof(command));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/InvokeInfrastructureQuery";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            command?.AddToQueryParameters(queryParams, $"Command={command}");
            serviceId?.AddToQueryParameters(queryParams, $"ServiceId={serviceId}");
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

            return this.httpClient.SendAsyncGetResponseAsRawJson(RequestFunc, url, requestId, cancellationToken);
        }
    }
}
