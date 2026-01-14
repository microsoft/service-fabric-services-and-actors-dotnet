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
    /// Class containing methods for performing EventsStoreClient operations.
    /// </summary>
    internal partial class EventsStoreClient : IEventsStoreClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the EventsStoreClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public EventsStoreClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<IEnumerable<ClusterEvent>> GetClusterEventListAsync(
            string startTimeUtc,
            string endTimeUtc,
            long? serverTimeout = 60,
            string eventsTypesFilter = default(string),
            bool? excludeAnalysisEvents = default(bool?),
            bool? skipCorrelationLookup = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            startTimeUtc.ThrowIfNull(nameof(startTimeUtc));
            endTimeUtc.ThrowIfNull(nameof(endTimeUtc));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "EventsStore/Cluster/Events";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            startTimeUtc?.AddToQueryParameters(queryParams, $"StartTimeUtc={startTimeUtc}");
            endTimeUtc?.AddToQueryParameters(queryParams, $"EndTimeUtc={endTimeUtc}");
            eventsTypesFilter?.AddToQueryParameters(queryParams, $"EventsTypesFilter={eventsTypesFilter}");
            excludeAnalysisEvents?.AddToQueryParameters(queryParams, $"ExcludeAnalysisEvents={excludeAnalysisEvents}");
            skipCorrelationLookup?.AddToQueryParameters(queryParams, $"SkipCorrelationLookup={skipCorrelationLookup}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, ClusterEventConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<ContainerInstanceEvent>> GetContainersEventListAsync(
            string startTimeUtc,
            string endTimeUtc,
            long? serverTimeout = 60,
            string eventsTypesFilter = default(string),
            bool? excludeAnalysisEvents = default(bool?),
            bool? skipCorrelationLookup = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            startTimeUtc.ThrowIfNull(nameof(startTimeUtc));
            endTimeUtc.ThrowIfNull(nameof(endTimeUtc));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "EventsStore/Containers/Events";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            startTimeUtc?.AddToQueryParameters(queryParams, $"StartTimeUtc={startTimeUtc}");
            endTimeUtc?.AddToQueryParameters(queryParams, $"EndTimeUtc={endTimeUtc}");
            eventsTypesFilter?.AddToQueryParameters(queryParams, $"EventsTypesFilter={eventsTypesFilter}");
            excludeAnalysisEvents?.AddToQueryParameters(queryParams, $"ExcludeAnalysisEvents={excludeAnalysisEvents}");
            skipCorrelationLookup?.AddToQueryParameters(queryParams, $"SkipCorrelationLookup={skipCorrelationLookup}");
            queryParams.Add("api-version=6.2-preview");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, ContainerInstanceEventConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<NodeEvent>> GetNodeEventListAsync(
            NodeName nodeName,
            string startTimeUtc,
            string endTimeUtc,
            long? serverTimeout = 60,
            string eventsTypesFilter = default(string),
            bool? excludeAnalysisEvents = default(bool?),
            bool? skipCorrelationLookup = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            startTimeUtc.ThrowIfNull(nameof(startTimeUtc));
            endTimeUtc.ThrowIfNull(nameof(endTimeUtc));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "EventsStore/Nodes/{nodeName}/$/Events";
            url = url.Replace("{nodeName}", Uri.EscapeDataString(nodeName.ToString()));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            startTimeUtc?.AddToQueryParameters(queryParams, $"StartTimeUtc={startTimeUtc}");
            endTimeUtc?.AddToQueryParameters(queryParams, $"EndTimeUtc={endTimeUtc}");
            eventsTypesFilter?.AddToQueryParameters(queryParams, $"EventsTypesFilter={eventsTypesFilter}");
            excludeAnalysisEvents?.AddToQueryParameters(queryParams, $"ExcludeAnalysisEvents={excludeAnalysisEvents}");
            skipCorrelationLookup?.AddToQueryParameters(queryParams, $"SkipCorrelationLookup={skipCorrelationLookup}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, NodeEventConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<NodeEvent>> GetNodesEventListAsync(
            string startTimeUtc,
            string endTimeUtc,
            long? serverTimeout = 60,
            string eventsTypesFilter = default(string),
            bool? excludeAnalysisEvents = default(bool?),
            bool? skipCorrelationLookup = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            startTimeUtc.ThrowIfNull(nameof(startTimeUtc));
            endTimeUtc.ThrowIfNull(nameof(endTimeUtc));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "EventsStore/Nodes/Events";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            startTimeUtc?.AddToQueryParameters(queryParams, $"StartTimeUtc={startTimeUtc}");
            endTimeUtc?.AddToQueryParameters(queryParams, $"EndTimeUtc={endTimeUtc}");
            eventsTypesFilter?.AddToQueryParameters(queryParams, $"EventsTypesFilter={eventsTypesFilter}");
            excludeAnalysisEvents?.AddToQueryParameters(queryParams, $"ExcludeAnalysisEvents={excludeAnalysisEvents}");
            skipCorrelationLookup?.AddToQueryParameters(queryParams, $"SkipCorrelationLookup={skipCorrelationLookup}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, NodeEventConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<ApplicationEvent>> GetApplicationEventListAsync(
            string applicationId,
            string startTimeUtc,
            string endTimeUtc,
            long? serverTimeout = 60,
            string eventsTypesFilter = default(string),
            bool? excludeAnalysisEvents = default(bool?),
            bool? skipCorrelationLookup = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationId.ThrowIfNull(nameof(applicationId));
            startTimeUtc.ThrowIfNull(nameof(startTimeUtc));
            endTimeUtc.ThrowIfNull(nameof(endTimeUtc));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "EventsStore/Applications/{applicationId}/$/Events";
            url = url.Replace("{applicationId}", applicationId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            startTimeUtc?.AddToQueryParameters(queryParams, $"StartTimeUtc={startTimeUtc}");
            endTimeUtc?.AddToQueryParameters(queryParams, $"EndTimeUtc={endTimeUtc}");
            eventsTypesFilter?.AddToQueryParameters(queryParams, $"EventsTypesFilter={eventsTypesFilter}");
            excludeAnalysisEvents?.AddToQueryParameters(queryParams, $"ExcludeAnalysisEvents={excludeAnalysisEvents}");
            skipCorrelationLookup?.AddToQueryParameters(queryParams, $"SkipCorrelationLookup={skipCorrelationLookup}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, ApplicationEventConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<ApplicationEvent>> GetApplicationsEventListAsync(
            string startTimeUtc,
            string endTimeUtc,
            long? serverTimeout = 60,
            string eventsTypesFilter = default(string),
            bool? excludeAnalysisEvents = default(bool?),
            bool? skipCorrelationLookup = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            startTimeUtc.ThrowIfNull(nameof(startTimeUtc));
            endTimeUtc.ThrowIfNull(nameof(endTimeUtc));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "EventsStore/Applications/Events";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            startTimeUtc?.AddToQueryParameters(queryParams, $"StartTimeUtc={startTimeUtc}");
            endTimeUtc?.AddToQueryParameters(queryParams, $"EndTimeUtc={endTimeUtc}");
            eventsTypesFilter?.AddToQueryParameters(queryParams, $"EventsTypesFilter={eventsTypesFilter}");
            excludeAnalysisEvents?.AddToQueryParameters(queryParams, $"ExcludeAnalysisEvents={excludeAnalysisEvents}");
            skipCorrelationLookup?.AddToQueryParameters(queryParams, $"SkipCorrelationLookup={skipCorrelationLookup}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, ApplicationEventConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<ServiceEvent>> GetServiceEventListAsync(
            string serviceId,
            string startTimeUtc,
            string endTimeUtc,
            long? serverTimeout = 60,
            string eventsTypesFilter = default(string),
            bool? excludeAnalysisEvents = default(bool?),
            bool? skipCorrelationLookup = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            startTimeUtc.ThrowIfNull(nameof(startTimeUtc));
            endTimeUtc.ThrowIfNull(nameof(endTimeUtc));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "EventsStore/Services/{serviceId}/$/Events";
            url = url.Replace("{serviceId}", serviceId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            startTimeUtc?.AddToQueryParameters(queryParams, $"StartTimeUtc={startTimeUtc}");
            endTimeUtc?.AddToQueryParameters(queryParams, $"EndTimeUtc={endTimeUtc}");
            eventsTypesFilter?.AddToQueryParameters(queryParams, $"EventsTypesFilter={eventsTypesFilter}");
            excludeAnalysisEvents?.AddToQueryParameters(queryParams, $"ExcludeAnalysisEvents={excludeAnalysisEvents}");
            skipCorrelationLookup?.AddToQueryParameters(queryParams, $"SkipCorrelationLookup={skipCorrelationLookup}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, ServiceEventConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<ServiceEvent>> GetServicesEventListAsync(
            string startTimeUtc,
            string endTimeUtc,
            long? serverTimeout = 60,
            string eventsTypesFilter = default(string),
            bool? excludeAnalysisEvents = default(bool?),
            bool? skipCorrelationLookup = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            startTimeUtc.ThrowIfNull(nameof(startTimeUtc));
            endTimeUtc.ThrowIfNull(nameof(endTimeUtc));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "EventsStore/Services/Events";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            startTimeUtc?.AddToQueryParameters(queryParams, $"StartTimeUtc={startTimeUtc}");
            endTimeUtc?.AddToQueryParameters(queryParams, $"EndTimeUtc={endTimeUtc}");
            eventsTypesFilter?.AddToQueryParameters(queryParams, $"EventsTypesFilter={eventsTypesFilter}");
            excludeAnalysisEvents?.AddToQueryParameters(queryParams, $"ExcludeAnalysisEvents={excludeAnalysisEvents}");
            skipCorrelationLookup?.AddToQueryParameters(queryParams, $"SkipCorrelationLookup={skipCorrelationLookup}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, ServiceEventConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<PartitionEvent>> GetPartitionEventListAsync(
            PartitionId partitionId,
            string startTimeUtc,
            string endTimeUtc,
            long? serverTimeout = 60,
            string eventsTypesFilter = default(string),
            bool? excludeAnalysisEvents = default(bool?),
            bool? skipCorrelationLookup = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            startTimeUtc.ThrowIfNull(nameof(startTimeUtc));
            endTimeUtc.ThrowIfNull(nameof(endTimeUtc));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "EventsStore/Partitions/{partitionId}/$/Events";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            startTimeUtc?.AddToQueryParameters(queryParams, $"StartTimeUtc={startTimeUtc}");
            endTimeUtc?.AddToQueryParameters(queryParams, $"EndTimeUtc={endTimeUtc}");
            eventsTypesFilter?.AddToQueryParameters(queryParams, $"EventsTypesFilter={eventsTypesFilter}");
            excludeAnalysisEvents?.AddToQueryParameters(queryParams, $"ExcludeAnalysisEvents={excludeAnalysisEvents}");
            skipCorrelationLookup?.AddToQueryParameters(queryParams, $"SkipCorrelationLookup={skipCorrelationLookup}");
            queryParams.Add("api-version=7.2");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, PartitionEventConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<PartitionEvent>> GetPartitionsEventListAsync(
            string startTimeUtc,
            string endTimeUtc,
            long? serverTimeout = 60,
            string eventsTypesFilter = default(string),
            bool? excludeAnalysisEvents = default(bool?),
            bool? skipCorrelationLookup = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            startTimeUtc.ThrowIfNull(nameof(startTimeUtc));
            endTimeUtc.ThrowIfNull(nameof(endTimeUtc));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "EventsStore/Partitions/Events";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            startTimeUtc?.AddToQueryParameters(queryParams, $"StartTimeUtc={startTimeUtc}");
            endTimeUtc?.AddToQueryParameters(queryParams, $"EndTimeUtc={endTimeUtc}");
            eventsTypesFilter?.AddToQueryParameters(queryParams, $"EventsTypesFilter={eventsTypesFilter}");
            excludeAnalysisEvents?.AddToQueryParameters(queryParams, $"ExcludeAnalysisEvents={excludeAnalysisEvents}");
            skipCorrelationLookup?.AddToQueryParameters(queryParams, $"SkipCorrelationLookup={skipCorrelationLookup}");
            queryParams.Add("api-version=7.2");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, PartitionEventConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<ReplicaEvent>> GetPartitionReplicaEventListAsync(
            PartitionId partitionId,
            ReplicaId replicaId,
            string startTimeUtc,
            string endTimeUtc,
            long? serverTimeout = 60,
            string eventsTypesFilter = default(string),
            bool? excludeAnalysisEvents = default(bool?),
            bool? skipCorrelationLookup = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            replicaId.ThrowIfNull(nameof(replicaId));
            startTimeUtc.ThrowIfNull(nameof(startTimeUtc));
            endTimeUtc.ThrowIfNull(nameof(endTimeUtc));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "EventsStore/Partitions/{partitionId}/$/Replicas/{replicaId}/$/Events";
            url = url.Replace("{partitionId}", partitionId.ToString());
            url = url.Replace("{replicaId}", replicaId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            startTimeUtc?.AddToQueryParameters(queryParams, $"StartTimeUtc={startTimeUtc}");
            endTimeUtc?.AddToQueryParameters(queryParams, $"EndTimeUtc={endTimeUtc}");
            eventsTypesFilter?.AddToQueryParameters(queryParams, $"EventsTypesFilter={eventsTypesFilter}");
            excludeAnalysisEvents?.AddToQueryParameters(queryParams, $"ExcludeAnalysisEvents={excludeAnalysisEvents}");
            skipCorrelationLookup?.AddToQueryParameters(queryParams, $"SkipCorrelationLookup={skipCorrelationLookup}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, ReplicaEventConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<ReplicaEvent>> GetPartitionReplicasEventListAsync(
            PartitionId partitionId,
            string startTimeUtc,
            string endTimeUtc,
            long? serverTimeout = 60,
            string eventsTypesFilter = default(string),
            bool? excludeAnalysisEvents = default(bool?),
            bool? skipCorrelationLookup = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            startTimeUtc.ThrowIfNull(nameof(startTimeUtc));
            endTimeUtc.ThrowIfNull(nameof(endTimeUtc));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "EventsStore/Partitions/{partitionId}/$/Replicas/Events";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            startTimeUtc?.AddToQueryParameters(queryParams, $"StartTimeUtc={startTimeUtc}");
            endTimeUtc?.AddToQueryParameters(queryParams, $"EndTimeUtc={endTimeUtc}");
            eventsTypesFilter?.AddToQueryParameters(queryParams, $"EventsTypesFilter={eventsTypesFilter}");
            excludeAnalysisEvents?.AddToQueryParameters(queryParams, $"ExcludeAnalysisEvents={excludeAnalysisEvents}");
            skipCorrelationLookup?.AddToQueryParameters(queryParams, $"SkipCorrelationLookup={skipCorrelationLookup}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, ReplicaEventConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<FabricEvent>> GetCorrelatedEventListAsync(
            string eventInstanceId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            eventInstanceId.ThrowIfNull(nameof(eventInstanceId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "EventsStore/CorrelatedEvents/{eventInstanceId}/$/Events";
            url = url.Replace("{eventInstanceId}", Uri.EscapeDataString(eventInstanceId));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, FabricEventConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
