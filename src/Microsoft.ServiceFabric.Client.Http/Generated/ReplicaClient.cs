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
    /// Class containing methods for performing ReplicaClient operations.
    /// </summary>
    internal partial class ReplicaClient : IReplicaClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the ReplicaClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public ReplicaClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<PagedData<ReplicaInfo>> GetReplicaInfoListAsync(
            PartitionId partitionId,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/GetReplicas";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
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

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, ReplicaInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ReplicaInfo> GetReplicaInfoAsync(
            PartitionId partitionId,
            ReplicaId replicaId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            replicaId.ThrowIfNull(nameof(replicaId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/GetReplicas/{replicaId}";
            url = url.Replace("{partitionId}", partitionId.ToString());
            url = url.Replace("{replicaId}", replicaId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ReplicaInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ReplicaHealth> GetReplicaHealthAsync(
            PartitionId partitionId,
            ReplicaId replicaId,
            int? eventsHealthStateFilter = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            replicaId.ThrowIfNull(nameof(replicaId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/GetReplicas/{replicaId}/$/GetHealth";
            url = url.Replace("{partitionId}", partitionId.ToString());
            url = url.Replace("{replicaId}", replicaId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            eventsHealthStateFilter?.AddToQueryParameters(queryParams, $"EventsHealthStateFilter={eventsHealthStateFilter}");
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ReplicaHealthConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ReplicaHealth> GetReplicaHealthUsingPolicyAsync(
            PartitionId partitionId,
            ReplicaId replicaId,
            int? eventsHealthStateFilter = 0,
            ApplicationHealthPolicy applicationHealthPolicy = default(ApplicationHealthPolicy),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            replicaId.ThrowIfNull(nameof(replicaId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/GetReplicas/{replicaId}/$/GetHealth";
            url = url.Replace("{partitionId}", partitionId.ToString());
            url = url.Replace("{replicaId}", replicaId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            eventsHealthStateFilter?.AddToQueryParameters(queryParams, $"EventsHealthStateFilter={eventsHealthStateFilter}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                if (applicationHealthPolicy != default(ApplicationHealthPolicy))
                {
                    ApplicationHealthPolicyConverter.Serialize(new JsonTextWriter(sw), applicationHealthPolicy);
                }

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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ReplicaHealthConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task ReportReplicaHealthAsync(
            PartitionId partitionId,
            ReplicaId replicaId,
            ReplicaHealthReportServiceKind? serviceKind,
            HealthInformation healthInformation,
            bool? immediate = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            replicaId.ThrowIfNull(nameof(replicaId));
            serviceKind.ThrowIfNull(nameof(serviceKind));
            healthInformation.ThrowIfNull(nameof(healthInformation));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/GetReplicas/{replicaId}/$/ReportHealth";
            url = url.Replace("{partitionId}", partitionId.ToString());
            url = url.Replace("{replicaId}", replicaId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serviceKind?.AddToQueryParameters(queryParams, $"ServiceKind={serviceKind.ToString()}");
            immediate?.AddToQueryParameters(queryParams, $"Immediate={immediate}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                HealthInformationConverter.Serialize(new JsonTextWriter(sw), healthInformation);
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
        public Task<IEnumerable<DeployedServiceReplicaInfo>> GetDeployedServiceReplicaInfoListAsync(
            NodeName nodeName,
            string applicationId,
            PartitionId partitionId = default(PartitionId),
            string serviceManifestName = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            applicationId.ThrowIfNull(nameof(applicationId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Nodes/{nodeName}/$/GetApplications/{applicationId}/$/GetReplicas";
            url = url.Replace("{nodeName}", Uri.EscapeDataString(nodeName.ToString()));
            url = url.Replace("{applicationId}", applicationId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            partitionId?.AddToQueryParameters(queryParams, $"PartitionId={partitionId.ToString()}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, DeployedServiceReplicaInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<DeployedServiceReplicaDetailInfo> GetDeployedServiceReplicaDetailInfoAsync(
            NodeName nodeName,
            PartitionId partitionId,
            ReplicaId replicaId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            partitionId.ThrowIfNull(nameof(partitionId));
            replicaId.ThrowIfNull(nameof(replicaId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Nodes/{nodeName}/$/GetPartitions/{partitionId}/$/GetReplicas/{replicaId}/$/GetDetail";
            url = url.Replace("{nodeName}", Uri.EscapeDataString(nodeName.ToString()));
            url = url.Replace("{partitionId}", partitionId.ToString());
            url = url.Replace("{replicaId}", replicaId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, DeployedServiceReplicaDetailInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<DeployedServiceReplicaDetailInfo> GetDeployedServiceReplicaDetailInfoByPartitionIdAsync(
            NodeName nodeName,
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Nodes/{nodeName}/$/GetPartitions/{partitionId}/$/GetReplicas";
            url = url.Replace("{nodeName}", Uri.EscapeDataString(nodeName.ToString()));
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, DeployedServiceReplicaDetailInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task RestartReplicaAsync(
            NodeName nodeName,
            PartitionId partitionId,
            ReplicaId replicaId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            partitionId.ThrowIfNull(nameof(partitionId));
            replicaId.ThrowIfNull(nameof(replicaId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Nodes/{nodeName}/$/GetPartitions/{partitionId}/$/GetReplicas/{replicaId}/$/Restart";
            url = url.Replace("{nodeName}", Uri.EscapeDataString(nodeName.ToString()));
            url = url.Replace("{partitionId}", partitionId.ToString());
            url = url.Replace("{replicaId}", replicaId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
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

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task RemoveReplicaAsync(
            NodeName nodeName,
            PartitionId partitionId,
            ReplicaId replicaId,
            bool? forceRemove = default(bool?),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            partitionId.ThrowIfNull(nameof(partitionId));
            replicaId.ThrowIfNull(nameof(replicaId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Nodes/{nodeName}/$/GetPartitions/{partitionId}/$/GetReplicas/{replicaId}/$/Delete";
            url = url.Replace("{nodeName}", Uri.EscapeDataString(nodeName.ToString()));
            url = url.Replace("{partitionId}", partitionId.ToString());
            url = url.Replace("{replicaId}", replicaId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            forceRemove?.AddToQueryParameters(queryParams, $"ForceRemove={forceRemove}");
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

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task RestoreReplicaAsync(
            NodeName nodeName,
            PartitionId partitionId,
            ReplicaId replicaId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            partitionId.ThrowIfNull(nameof(partitionId));
            replicaId.ThrowIfNull(nameof(replicaId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Nodes/{nodeName}/$/GetPartitions/{partitionId}/$/GetReplicas/{replicaId}/$/Restore";
            url = url.Replace("{nodeName}", Uri.EscapeDataString(nodeName.ToString()));
            url = url.Replace("{partitionId}", partitionId.ToString());
            url = url.Replace("{replicaId}", replicaId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=11.3");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }
    }
}
