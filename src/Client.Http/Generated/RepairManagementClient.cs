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
    /// Class containing methods for performing RepairManagementClient operations.
    /// </summary>
    internal partial class RepairManagementClient : IRepairManagementClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the RepairManagementClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public RepairManagementClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<RepairTaskUpdateInfo> CreateRepairTaskAsync(
            RepairTask repairTask,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            repairTask.ThrowIfNull(nameof(repairTask));
            var requestId = Guid.NewGuid().ToString();
            var url = "$/CreateRepairTask";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                RepairTaskConverter.Serialize(new JsonTextWriter(sw), repairTask);
                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, RepairTaskUpdateInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<RepairTaskUpdateInfo> CancelRepairTaskAsync(
            RepairTaskCancelDescription repairTaskCancelDescription,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            repairTaskCancelDescription.ThrowIfNull(nameof(repairTaskCancelDescription));
            var requestId = Guid.NewGuid().ToString();
            var url = "$/CancelRepairTask";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                RepairTaskCancelDescriptionConverter.Serialize(new JsonTextWriter(sw), repairTaskCancelDescription);
                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, RepairTaskUpdateInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteRepairTaskAsync(
            RepairTaskDeleteDescription repairTaskDeleteDescription,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            repairTaskDeleteDescription.ThrowIfNull(nameof(repairTaskDeleteDescription));
            var requestId = Guid.NewGuid().ToString();
            var url = "$/DeleteRepairTask";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                RepairTaskDeleteDescriptionConverter.Serialize(new JsonTextWriter(sw), repairTaskDeleteDescription);
                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<RepairTask>> GetRepairTaskListAsync(
            string taskIdFilter = default(string),
            int? stateFilter = default(int?),
            string executorFilter = default(string),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetRepairTaskList";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            taskIdFilter?.AddToQueryParameters(queryParams, $"TaskIdFilter={taskIdFilter}");
            stateFilter?.AddToQueryParameters(queryParams, $"StateFilter={stateFilter}");
            executorFilter?.AddToQueryParameters(queryParams, $"ExecutorFilter={executorFilter}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, RepairTaskConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<RepairTaskUpdateInfo> ForceApproveRepairTaskAsync(
            RepairTaskApproveDescription repairTaskApproveDescription,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            repairTaskApproveDescription.ThrowIfNull(nameof(repairTaskApproveDescription));
            var requestId = Guid.NewGuid().ToString();
            var url = "$/ForceApproveRepairTask";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                RepairTaskApproveDescriptionConverter.Serialize(new JsonTextWriter(sw), repairTaskApproveDescription);
                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, RepairTaskUpdateInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<RepairTaskUpdateInfo> UpdateRepairTaskHealthPolicyAsync(
            RepairTaskUpdateHealthPolicyDescription repairTaskUpdateHealthPolicyDescription,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            repairTaskUpdateHealthPolicyDescription.ThrowIfNull(nameof(repairTaskUpdateHealthPolicyDescription));
            var requestId = Guid.NewGuid().ToString();
            var url = "$/UpdateRepairTaskHealthPolicy";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                RepairTaskUpdateHealthPolicyDescriptionConverter.Serialize(new JsonTextWriter(sw), repairTaskUpdateHealthPolicyDescription);
                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, RepairTaskUpdateInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<RepairTaskUpdateInfo> UpdateRepairExecutionStateAsync(
            RepairTask repairTask,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            repairTask.ThrowIfNull(nameof(repairTask));
            var requestId = Guid.NewGuid().ToString();
            var url = "$/UpdateRepairExecutionState";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                RepairTaskConverter.Serialize(new JsonTextWriter(sw), repairTask);
                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, RepairTaskUpdateInfoConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
