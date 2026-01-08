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
    /// Class containing methods for performing PartitionClient operations.
    /// </summary>
    internal partial class PartitionClient : IPartitionClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the PartitionClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public PartitionClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<PagedData<LoadedPartitionInformationResult>> GetLoadedPartitionInfoListAsync(
            string metricName,
            string serviceName = default(string),
            Ordering? ordering = Ordering.Desc,
            long? maxResults = 0,
            ContinuationToken continuationToken = default(ContinuationToken),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            metricName.ThrowIfNull(nameof(metricName));
            maxResults?.ThrowIfLessThan("maxResults", 0);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetLoadedPartitionInfoList";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            metricName?.AddToQueryParameters(queryParams, $"MetricName={metricName}");
            serviceName?.AddToQueryParameters(queryParams, $"ServiceName={serviceName}");
            ordering?.AddToQueryParameters(queryParams, $"Ordering={ordering.ToString()}");
            maxResults?.AddToQueryParameters(queryParams, $"MaxResults={maxResults}");
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
            queryParams.Add("api-version=8.0");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, LoadedPartitionInformationResultConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<ServicePartitionInfo>> GetPartitionInfoListAsync(
            string serviceId,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Services/{serviceId}/$/GetPartitions";
            url = url.Replace("{serviceId}", serviceId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, ServicePartitionInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ServicePartitionInfo> GetPartitionInfoAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}";
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ServicePartitionInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ServiceNameInfo> GetServiceNameInfoAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/GetServiceName";
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ServiceNameInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PartitionHealth> GetPartitionHealthAsync(
            PartitionId partitionId,
            int? eventsHealthStateFilter = 0,
            int? replicasHealthStateFilter = 0,
            bool? excludeHealthStatistics = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/GetHealth";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            eventsHealthStateFilter?.AddToQueryParameters(queryParams, $"EventsHealthStateFilter={eventsHealthStateFilter}");
            replicasHealthStateFilter?.AddToQueryParameters(queryParams, $"ReplicasHealthStateFilter={replicasHealthStateFilter}");
            excludeHealthStatistics?.AddToQueryParameters(queryParams, $"ExcludeHealthStatistics={excludeHealthStatistics}");
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, PartitionHealthConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PartitionHealth> GetPartitionHealthUsingPolicyAsync(
            PartitionId partitionId,
            int? eventsHealthStateFilter = 0,
            int? replicasHealthStateFilter = 0,
            ApplicationHealthPolicy applicationHealthPolicy = default(ApplicationHealthPolicy),
            bool? excludeHealthStatistics = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/GetHealth";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            eventsHealthStateFilter?.AddToQueryParameters(queryParams, $"EventsHealthStateFilter={eventsHealthStateFilter}");
            replicasHealthStateFilter?.AddToQueryParameters(queryParams, $"ReplicasHealthStateFilter={replicasHealthStateFilter}");
            excludeHealthStatistics?.AddToQueryParameters(queryParams, $"ExcludeHealthStatistics={excludeHealthStatistics}");
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, PartitionHealthConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task ReportPartitionHealthAsync(
            PartitionId partitionId,
            HealthInformation healthInformation,
            bool? immediate = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            healthInformation.ThrowIfNull(nameof(healthInformation));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/ReportHealth";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
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
        public Task<PartitionLoadInformation> GetPartitionLoadInformationAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/GetLoadInformation";
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, PartitionLoadInformationConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task ResetPartitionLoadAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/ResetLoad";
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
                    Method = HttpMethod.Post,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task RecoverPartitionAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/Recover";
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
                    Method = HttpMethod.Post,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task RecoverServicePartitionsAsync(
            string serviceId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Services/$/{serviceId}/$/GetPartitions/$/Recover";
            url = url.Replace("{serviceId}", serviceId);
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
        public Task RecoverSystemPartitionsAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/RecoverSystemPartitions";
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
        public Task RecoverAllPartitionsAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/RecoverAllPartitions";
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
        public Task MovePrimaryReplicaAsync(
            PartitionId partitionId,
            NodeName nodeName = default(NodeName),
            bool? ignoreConstraints = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/MovePrimaryReplica";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            nodeName?.AddToQueryParameters(queryParams, $"NodeName={nodeName.ToString()}");
            ignoreConstraints?.AddToQueryParameters(queryParams, $"IgnoreConstraints={ignoreConstraints}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.5");
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
        public Task MoveSecondaryReplicaAsync(
            PartitionId partitionId,
            NodeName currentNodeName,
            NodeName newNodeName = default(NodeName),
            bool? ignoreConstraints = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            currentNodeName.ThrowIfNull(nameof(currentNodeName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/MoveSecondaryReplica";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            currentNodeName?.AddToQueryParameters(queryParams, $"CurrentNodeName={currentNodeName.ToString()}");
            newNodeName?.AddToQueryParameters(queryParams, $"NewNodeName={newNodeName.ToString()}");
            ignoreConstraints?.AddToQueryParameters(queryParams, $"IgnoreConstraints={ignoreConstraints}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.5");
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
        public Task<PagedData<UpdatePartitionLoadResult>> UpdatePartitionLoadAsync(
            IEnumerable<PartitionMetricLoadDescription> partitionMetricLoadDescriptionList,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionMetricLoadDescriptionList.ThrowIfNull(nameof(partitionMetricLoadDescriptionList));
            maxResults?.ThrowIfLessThan("maxResults", 0);
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/UpdatePartitionLoad";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
            maxResults?.AddToQueryParameters(queryParams, $"MaxResults={maxResults}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=7.2");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                ListTConverter<PartitionMetricLoadDescription>.Serialize(new JsonTextWriter(sw), partitionMetricLoadDescriptionList, PartitionMetricLoadDescriptionConverter.Serialize);
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

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, UpdatePartitionLoadResultConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task MoveInstanceAsync(
            string serviceId,
            PartitionId partitionId,
            NodeName currentNodeName = default(NodeName),
            NodeName newNodeName = default(NodeName),
            bool? ignoreConstraints = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Services/{serviceId}/$/GetPartitions/{partitionId}/$/MoveInstance";
            url = url.Replace("{serviceId}", serviceId);
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            currentNodeName?.AddToQueryParameters(queryParams, $"CurrentNodeName={currentNodeName.ToString()}");
            newNodeName?.AddToQueryParameters(queryParams, $"NewNodeName={newNodeName.ToString()}");
            ignoreConstraints?.AddToQueryParameters(queryParams, $"IgnoreConstraints={ignoreConstraints}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=8.0");
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
        public Task MoveAuxiliaryReplicaAsync(
            string serviceId,
            PartitionId partitionId,
            NodeName currentNodeName = default(NodeName),
            NodeName newNodeName = default(NodeName),
            bool? ignoreConstraints = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Services/{serviceId}/$/GetPartitions/{partitionId}/$/MoveAuxiliaryReplica";
            url = url.Replace("{serviceId}", serviceId);
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            currentNodeName?.AddToQueryParameters(queryParams, $"CurrentNodeName={currentNodeName.ToString()}");
            newNodeName?.AddToQueryParameters(queryParams, $"NewNodeName={newNodeName.ToString()}");
            ignoreConstraints?.AddToQueryParameters(queryParams, $"IgnoreConstraints={ignoreConstraints}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=8.1");
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
