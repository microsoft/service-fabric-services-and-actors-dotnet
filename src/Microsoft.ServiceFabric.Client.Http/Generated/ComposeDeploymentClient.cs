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
    /// Class containing methods for performing ComposeDeploymentClient operations.
    /// </summary>
    internal partial class ComposeDeploymentClient : IComposeDeploymentClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the ComposeDeploymentClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public ComposeDeploymentClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task CreateComposeDeploymentAsync(
            CreateComposeDeploymentDescription createComposeDeploymentDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            createComposeDeploymentDescription.ThrowIfNull(nameof(createComposeDeploymentDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ComposeDeployments/$/Create";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0-preview");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                CreateComposeDeploymentDescriptionConverter.Serialize(new JsonTextWriter(sw), createComposeDeploymentDescription);
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

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ComposeDeploymentStatusInfo> GetComposeDeploymentStatusAsync(
            string deploymentName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            deploymentName.ThrowIfNull(nameof(deploymentName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ComposeDeployments/{deploymentName}";
            url = url.Replace("{deploymentName}", deploymentName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0-preview");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ComposeDeploymentStatusInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<ComposeDeploymentStatusInfo>> GetComposeDeploymentStatusListAsync(
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            maxResults?.ThrowIfLessThan("maxResults", 0);
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ComposeDeployments";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
            maxResults?.AddToQueryParameters(queryParams, $"MaxResults={maxResults}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0-preview");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, ComposeDeploymentStatusInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ComposeDeploymentUpgradeProgressInfo> GetComposeDeploymentUpgradeProgressAsync(
            string deploymentName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            deploymentName.ThrowIfNull(nameof(deploymentName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ComposeDeployments/{deploymentName}/$/GetUpgradeProgress";
            url = url.Replace("{deploymentName}", deploymentName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0-preview");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ComposeDeploymentUpgradeProgressInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task RemoveComposeDeploymentAsync(
            string deploymentName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            deploymentName.ThrowIfNull(nameof(deploymentName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ComposeDeployments/{deploymentName}/$/Delete";
            url = url.Replace("{deploymentName}", deploymentName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0-preview");
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
        public Task StartComposeDeploymentUpgradeAsync(
            string deploymentName,
            ComposeDeploymentUpgradeDescription composeDeploymentUpgradeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            deploymentName.ThrowIfNull(nameof(deploymentName));
            composeDeploymentUpgradeDescription.ThrowIfNull(nameof(composeDeploymentUpgradeDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ComposeDeployments/{deploymentName}/$/Upgrade";
            url = url.Replace("{deploymentName}", deploymentName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0-preview");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                ComposeDeploymentUpgradeDescriptionConverter.Serialize(new JsonTextWriter(sw), composeDeploymentUpgradeDescription);
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
        public Task StartRollbackComposeDeploymentUpgradeAsync(
            string deploymentName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            deploymentName.ThrowIfNull(nameof(deploymentName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ComposeDeployments/{deploymentName}/$/RollbackUpgrade";
            url = url.Replace("{deploymentName}", deploymentName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4-preview");
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
