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
    /// Class containing methods for performing FaultsClient operations.
    /// </summary>
    internal partial class FaultsClient : IFaultsClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the FaultsClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public FaultsClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task StartDataLossAsync(
            string serviceId,
            PartitionId partitionId,
            Guid? operationId,
            DataLossMode? dataLossMode,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            partitionId.ThrowIfNull(nameof(partitionId));
            operationId.ThrowIfNull(nameof(operationId));
            dataLossMode.ThrowIfNull(nameof(dataLossMode));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Faults/Services/{serviceId}/$/GetPartitions/{partitionId}/$/StartDataLoss";
            url = url.Replace("{serviceId}", serviceId);
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            operationId?.AddToQueryParameters(queryParams, $"OperationId={operationId.ToString()}");
            dataLossMode?.AddToQueryParameters(queryParams, $"DataLossMode={dataLossMode.ToString()}");
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
        public Task<PartitionDataLossProgress> GetDataLossProgressAsync(
            string serviceId,
            PartitionId partitionId,
            Guid? operationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            partitionId.ThrowIfNull(nameof(partitionId));
            operationId.ThrowIfNull(nameof(operationId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Faults/Services/{serviceId}/$/GetPartitions/{partitionId}/$/GetDataLossProgress";
            url = url.Replace("{serviceId}", serviceId);
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            operationId?.AddToQueryParameters(queryParams, $"OperationId={operationId.ToString()}");
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, PartitionDataLossProgressConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task StartQuorumLossAsync(
            string serviceId,
            PartitionId partitionId,
            Guid? operationId,
            QuorumLossMode? quorumLossMode,
            int? quorumLossDuration,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            partitionId.ThrowIfNull(nameof(partitionId));
            operationId.ThrowIfNull(nameof(operationId));
            quorumLossMode.ThrowIfNull(nameof(quorumLossMode));
            quorumLossDuration.ThrowIfNull(nameof(quorumLossDuration));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Faults/Services/{serviceId}/$/GetPartitions/{partitionId}/$/StartQuorumLoss";
            url = url.Replace("{serviceId}", serviceId);
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            operationId?.AddToQueryParameters(queryParams, $"OperationId={operationId.ToString()}");
            quorumLossMode?.AddToQueryParameters(queryParams, $"QuorumLossMode={quorumLossMode.ToString()}");
            quorumLossDuration?.AddToQueryParameters(queryParams, $"QuorumLossDuration={quorumLossDuration}");
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
        public Task<PartitionQuorumLossProgress> GetQuorumLossProgressAsync(
            string serviceId,
            PartitionId partitionId,
            Guid? operationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            partitionId.ThrowIfNull(nameof(partitionId));
            operationId.ThrowIfNull(nameof(operationId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Faults/Services/{serviceId}/$/GetPartitions/{partitionId}/$/GetQuorumLossProgress";
            url = url.Replace("{serviceId}", serviceId);
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            operationId?.AddToQueryParameters(queryParams, $"OperationId={operationId.ToString()}");
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, PartitionQuorumLossProgressConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task StartPartitionRestartAsync(
            string serviceId,
            PartitionId partitionId,
            Guid? operationId,
            RestartPartitionMode? restartPartitionMode,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            partitionId.ThrowIfNull(nameof(partitionId));
            operationId.ThrowIfNull(nameof(operationId));
            restartPartitionMode.ThrowIfNull(nameof(restartPartitionMode));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Faults/Services/{serviceId}/$/GetPartitions/{partitionId}/$/StartRestart";
            url = url.Replace("{serviceId}", serviceId);
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            operationId?.AddToQueryParameters(queryParams, $"OperationId={operationId.ToString()}");
            restartPartitionMode?.AddToQueryParameters(queryParams, $"RestartPartitionMode={restartPartitionMode.ToString()}");
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
        public Task<PartitionRestartProgress> GetPartitionRestartProgressAsync(
            string serviceId,
            PartitionId partitionId,
            Guid? operationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            partitionId.ThrowIfNull(nameof(partitionId));
            operationId.ThrowIfNull(nameof(operationId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Faults/Services/{serviceId}/$/GetPartitions/{partitionId}/$/GetRestartProgress";
            url = url.Replace("{serviceId}", serviceId);
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            operationId?.AddToQueryParameters(queryParams, $"OperationId={operationId.ToString()}");
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, PartitionRestartProgressConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task StartNodeTransitionAsync(
            NodeName nodeName,
            Guid? operationId,
            NodeTransitionType? nodeTransitionType,
            string nodeInstanceId,
            int? stopDurationInSeconds,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            operationId.ThrowIfNull(nameof(operationId));
            nodeTransitionType.ThrowIfNull(nameof(nodeTransitionType));
            nodeInstanceId.ThrowIfNull(nameof(nodeInstanceId));
            stopDurationInSeconds.ThrowIfNull(nameof(stopDurationInSeconds));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Faults/Nodes/{nodeName}/$/StartTransition/";
            url = url.Replace("{nodeName}", Uri.EscapeDataString(nodeName.ToString()));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            operationId?.AddToQueryParameters(queryParams, $"OperationId={operationId.ToString()}");
            nodeTransitionType?.AddToQueryParameters(queryParams, $"NodeTransitionType={nodeTransitionType.ToString()}");
            nodeInstanceId?.AddToQueryParameters(queryParams, $"NodeInstanceId={nodeInstanceId}");
            stopDurationInSeconds?.AddToQueryParameters(queryParams, $"StopDurationInSeconds={stopDurationInSeconds}");
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
        public Task<NodeTransitionProgress> GetNodeTransitionProgressAsync(
            NodeName nodeName,
            Guid? operationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            operationId.ThrowIfNull(nameof(operationId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Faults/Nodes/{nodeName}/$/GetTransitionProgress";
            url = url.Replace("{nodeName}", Uri.EscapeDataString(nodeName.ToString()));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            operationId?.AddToQueryParameters(queryParams, $"OperationId={operationId.ToString()}");
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, NodeTransitionProgressConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<OperationStatus>> GetFaultOperationListAsync(
            int? typeFilter,
            int? stateFilter,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            typeFilter.ThrowIfNull(nameof(typeFilter));
            stateFilter.ThrowIfNull(nameof(stateFilter));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Faults/";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            typeFilter?.AddToQueryParameters(queryParams, $"TypeFilter={typeFilter}");
            stateFilter?.AddToQueryParameters(queryParams, $"StateFilter={stateFilter}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, OperationStatusConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task CancelOperationAsync(
            Guid? operationId,
            bool? force,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            operationId.ThrowIfNull(nameof(operationId));
            force.ThrowIfNull(nameof(force));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Faults/$/Cancel";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            operationId?.AddToQueryParameters(queryParams, $"OperationId={operationId.ToString()}");
            force?.AddToQueryParameters(queryParams, $"Force={force}");
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
    }
}
