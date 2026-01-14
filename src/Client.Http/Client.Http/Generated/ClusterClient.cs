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
    /// Class containing methods for performing ClusterClient operations.
    /// </summary>
    internal partial class ClusterClient : IClusterClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the ClusterClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public ClusterClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<ClusterManifest> GetClusterManifestAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetClusterManifest";
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ClusterManifestConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ClusterHealth> GetClusterHealthAsync(
            int? nodesHealthStateFilter = 0,
            int? applicationsHealthStateFilter = 0,
            int? eventsHealthStateFilter = 0,
            bool? excludeHealthStatistics = false,
            bool? includeSystemApplicationHealthStatistics = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetClusterHealth";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            nodesHealthStateFilter?.AddToQueryParameters(queryParams, $"NodesHealthStateFilter={nodesHealthStateFilter}");
            applicationsHealthStateFilter?.AddToQueryParameters(queryParams, $"ApplicationsHealthStateFilter={applicationsHealthStateFilter}");
            eventsHealthStateFilter?.AddToQueryParameters(queryParams, $"EventsHealthStateFilter={eventsHealthStateFilter}");
            excludeHealthStatistics?.AddToQueryParameters(queryParams, $"ExcludeHealthStatistics={excludeHealthStatistics}");
            includeSystemApplicationHealthStatistics?.AddToQueryParameters(queryParams, $"IncludeSystemApplicationHealthStatistics={includeSystemApplicationHealthStatistics}");
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ClusterHealthConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ClusterHealth> GetClusterHealthUsingPolicyAsync(
            int? nodesHealthStateFilter = 0,
            int? applicationsHealthStateFilter = 0,
            int? eventsHealthStateFilter = 0,
            bool? excludeHealthStatistics = false,
            bool? includeSystemApplicationHealthStatistics = false,
            ClusterHealthPolicies clusterHealthPolicies = default(ClusterHealthPolicies),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetClusterHealth";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            nodesHealthStateFilter?.AddToQueryParameters(queryParams, $"NodesHealthStateFilter={nodesHealthStateFilter}");
            applicationsHealthStateFilter?.AddToQueryParameters(queryParams, $"ApplicationsHealthStateFilter={applicationsHealthStateFilter}");
            eventsHealthStateFilter?.AddToQueryParameters(queryParams, $"EventsHealthStateFilter={eventsHealthStateFilter}");
            excludeHealthStatistics?.AddToQueryParameters(queryParams, $"ExcludeHealthStatistics={excludeHealthStatistics}");
            includeSystemApplicationHealthStatistics?.AddToQueryParameters(queryParams, $"IncludeSystemApplicationHealthStatistics={includeSystemApplicationHealthStatistics}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                if (clusterHealthPolicies != default(ClusterHealthPolicies))
                {
                    ClusterHealthPoliciesConverter.Serialize(new JsonTextWriter(sw), clusterHealthPolicies);
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ClusterHealthConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ClusterHealthChunk> GetClusterHealthChunkAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetClusterHealthChunk";
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ClusterHealthChunkConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ClusterHealthChunk> GetClusterHealthChunkUsingPolicyAndAdvancedFiltersAsync(
            ClusterHealthChunkQueryDescription clusterHealthChunkQueryDescription = default(ClusterHealthChunkQueryDescription),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetClusterHealthChunk";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                if (clusterHealthChunkQueryDescription != default(ClusterHealthChunkQueryDescription))
                {
                    ClusterHealthChunkQueryDescriptionConverter.Serialize(new JsonTextWriter(sw), clusterHealthChunkQueryDescription);
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ClusterHealthChunkConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task ReportClusterHealthAsync(
            HealthInformation healthInformation,
            bool? immediate = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            healthInformation.ThrowIfNull(nameof(healthInformation));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/ReportClusterHealth";
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
        public Task<IEnumerable<FabricCodeVersionInfo>> GetProvisionedFabricCodeVersionInfoListAsync(
            string codeVersion = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetProvisionedCodeVersions";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            codeVersion?.AddToQueryParameters(queryParams, $"CodeVersion={codeVersion}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, FabricCodeVersionInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<FabricConfigVersionInfo>> GetProvisionedFabricConfigVersionInfoListAsync(
            string configVersion = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetProvisionedConfigVersions";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            configVersion?.AddToQueryParameters(queryParams, $"ConfigVersion={configVersion}");
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

            return this.httpClient.SendAsyncGetResponseAsList(RequestFunc, url, FabricConfigVersionInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ClusterUpgradeProgressObject> GetClusterUpgradeProgressAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetUpgradeProgress";
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ClusterUpgradeProgressObjectConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ClusterConfiguration> GetClusterConfigurationAsync(
            string configurationApiVersion,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            configurationApiVersion.ThrowIfNull(nameof(configurationApiVersion));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetClusterConfiguration";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            configurationApiVersion?.AddToQueryParameters(queryParams, $"ConfigurationApiVersion={configurationApiVersion}");
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ClusterConfigurationConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ClusterConfigurationUpgradeStatusInfo> GetClusterConfigurationUpgradeStatusAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetClusterConfigurationUpgradeStatus";
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ClusterConfigurationUpgradeStatusInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<UpgradeOrchestrationServiceState> GetUpgradeOrchestrationServiceStateAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetUpgradeOrchestrationServiceState";
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, UpgradeOrchestrationServiceStateConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<UpgradeOrchestrationServiceStateSummary> SetUpgradeOrchestrationServiceStateAsync(
            UpgradeOrchestrationServiceState upgradeOrchestrationServiceState,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            upgradeOrchestrationServiceState.ThrowIfNull(nameof(upgradeOrchestrationServiceState));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/SetUpgradeOrchestrationServiceState";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                UpgradeOrchestrationServiceStateConverter.Serialize(new JsonTextWriter(sw), upgradeOrchestrationServiceState);
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, UpgradeOrchestrationServiceStateSummaryConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task ProvisionClusterAsync(
            ProvisionFabricDescription provisionFabricDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            provisionFabricDescription.ThrowIfNull(nameof(provisionFabricDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/Provision";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                ProvisionFabricDescriptionConverter.Serialize(new JsonTextWriter(sw), provisionFabricDescription);
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
        public Task UnprovisionClusterAsync(
            UnprovisionFabricDescription unprovisionFabricDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            unprovisionFabricDescription.ThrowIfNull(nameof(unprovisionFabricDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/Unprovision";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                UnprovisionFabricDescriptionConverter.Serialize(new JsonTextWriter(sw), unprovisionFabricDescription);
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
        public Task RollbackClusterUpgradeAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/RollbackUpgrade";
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
        public Task ResumeClusterUpgradeAsync(
            ResumeClusterUpgradeDescription resumeClusterUpgradeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            resumeClusterUpgradeDescription.ThrowIfNull(nameof(resumeClusterUpgradeDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/MoveToNextUpgradeDomain";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                ResumeClusterUpgradeDescriptionConverter.Serialize(new JsonTextWriter(sw), resumeClusterUpgradeDescription);
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
        public Task StartClusterUpgradeAsync(
            StartClusterUpgradeDescription startClusterUpgradeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            startClusterUpgradeDescription.ThrowIfNull(nameof(startClusterUpgradeDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/Upgrade";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                StartClusterUpgradeDescriptionConverter.Serialize(new JsonTextWriter(sw), startClusterUpgradeDescription);
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
        public Task StartClusterConfigurationUpgradeAsync(
            ClusterConfigurationUpgradeDescription clusterConfigurationUpgradeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            clusterConfigurationUpgradeDescription.ThrowIfNull(nameof(clusterConfigurationUpgradeDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/StartClusterConfigurationUpgrade";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                ClusterConfigurationUpgradeDescriptionConverter.Serialize(new JsonTextWriter(sw), clusterConfigurationUpgradeDescription);
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
        public Task UpdateClusterUpgradeAsync(
            UpdateClusterUpgradeDescription updateClusterUpgradeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            updateClusterUpgradeDescription.ThrowIfNull(nameof(updateClusterUpgradeDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/UpdateUpgrade";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                UpdateClusterUpgradeDescriptionConverter.Serialize(new JsonTextWriter(sw), updateClusterUpgradeDescription);
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
        public Task<AadMetadataObject> GetAadMetadataAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetAadMetadata";
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, AadMetadataObjectConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ClusterVersion> GetClusterVersionAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetClusterVersion";
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ClusterVersionConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ClusterLoadInfo> GetClusterLoadAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetLoadInformation";
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ClusterLoadInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task ToggleVerboseServicePlacementHealthReportingAsync(
            bool? enabled,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            enabled.ThrowIfNull(nameof(enabled));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/ToggleVerboseServicePlacementHealthReporting";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            enabled?.AddToQueryParameters(queryParams, $"Enabled={enabled}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
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
        public Task<ValidateClusterUpgradeResult> ValidateClusterUpgradeAsync(
            StartClusterUpgradeDescription startClusterUpgradeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            startClusterUpgradeDescription.ThrowIfNull(nameof(startClusterUpgradeDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "$/ValidateUpgrade";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=8.2");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                StartClusterUpgradeDescriptionConverter.Serialize(new JsonTextWriter(sw), startClusterUpgradeDescription);
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ValidateClusterUpgradeResultConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
