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
    /// Class containing methods for performing ApplicationTypeClient operations.
    /// </summary>
    internal partial class ApplicationTypeClient : IApplicationTypeClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the ApplicationTypeClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public ApplicationTypeClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<PagedData<ApplicationTypeInfo>> GetApplicationTypeInfoListAsync(
            int? applicationTypeDefinitionKindFilter = 0,
            bool? excludeApplicationParameters = false,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            maxResults?.ThrowIfLessThan("maxResults", 0);
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ApplicationTypes";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            applicationTypeDefinitionKindFilter?.AddToQueryParameters(queryParams, $"ApplicationTypeDefinitionKindFilter={applicationTypeDefinitionKindFilter}");
            excludeApplicationParameters?.AddToQueryParameters(queryParams, $"ExcludeApplicationParameters={excludeApplicationParameters}");
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
            maxResults?.AddToQueryParameters(queryParams, $"MaxResults={maxResults}");
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

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, ApplicationTypeInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<ApplicationTypeInfo>> GetApplicationTypeInfoListByNameAsync(
            string applicationTypeName,
            string applicationTypeVersion = default(string),
            bool? excludeApplicationParameters = false,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationTypeName.ThrowIfNull(nameof(applicationTypeName));
            maxResults?.ThrowIfLessThan("maxResults", 0);
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ApplicationTypes/{applicationTypeName}";
            url = url.Replace("{applicationTypeName}", Uri.EscapeDataString(applicationTypeName));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            applicationTypeVersion?.AddToQueryParameters(queryParams, $"ApplicationTypeVersion={applicationTypeVersion}");
            excludeApplicationParameters?.AddToQueryParameters(queryParams, $"ExcludeApplicationParameters={excludeApplicationParameters}");
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
            maxResults?.AddToQueryParameters(queryParams, $"MaxResults={maxResults}");
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

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, ApplicationTypeInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task ProvisionApplicationTypeAsync(
            ProvisionApplicationTypeDescriptionBase provisionApplicationTypeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            provisionApplicationTypeDescription.ThrowIfNull(nameof(provisionApplicationTypeDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ApplicationTypes/$/Provision";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.2");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                ProvisionApplicationTypeDescriptionBaseConverter.Serialize(new JsonTextWriter(sw), provisionApplicationTypeDescription);
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
        public Task UnprovisionApplicationTypeAsync(
            string applicationTypeName,
            UnprovisionApplicationTypeDescriptionInfo unprovisionApplicationTypeDescriptionInfo,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationTypeName.ThrowIfNull(nameof(applicationTypeName));
            unprovisionApplicationTypeDescriptionInfo.ThrowIfNull(nameof(unprovisionApplicationTypeDescriptionInfo));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ApplicationTypes/{applicationTypeName}/$/Unprovision";
            url = url.Replace("{applicationTypeName}", Uri.EscapeDataString(applicationTypeName));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                UnprovisionApplicationTypeDescriptionInfoConverter.Serialize(new JsonTextWriter(sw), unprovisionApplicationTypeDescriptionInfo);
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
        public Task UpdateApplicationTypeArmMetadataAsync(
            string applicationTypeName,
            string applicationTypeVersion,
            ApplicationTypeArmMetadataUpdateDescription applicationTypeArmMetadataUpdateDescription,
            long? serverTimeout = 60,
            bool? force = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationTypeName.ThrowIfNull(nameof(applicationTypeName));
            applicationTypeVersion.ThrowIfNull(nameof(applicationTypeVersion));
            applicationTypeArmMetadataUpdateDescription.ThrowIfNull(nameof(applicationTypeArmMetadataUpdateDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ApplicationTypes/{applicationTypeName}/$/UpdateArmMetadata";
            url = url.Replace("{applicationTypeName}", Uri.EscapeDataString(applicationTypeName));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            applicationTypeVersion?.AddToQueryParameters(queryParams, $"ApplicationTypeVersion={applicationTypeVersion}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            force?.AddToQueryParameters(queryParams, $"Force={force}");
            queryParams.Add("api-version=9.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                ApplicationTypeArmMetadataUpdateDescriptionConverter.Serialize(new JsonTextWriter(sw), applicationTypeArmMetadataUpdateDescription);
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
        public Task<ApplicationTypeManifest> GetApplicationManifestAsync(
            string applicationTypeName,
            string applicationTypeVersion,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationTypeName.ThrowIfNull(nameof(applicationTypeName));
            applicationTypeVersion.ThrowIfNull(nameof(applicationTypeVersion));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "ApplicationTypes/{applicationTypeName}/$/GetApplicationManifest";
            url = url.Replace("{applicationTypeName}", Uri.EscapeDataString(applicationTypeName));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            applicationTypeVersion?.AddToQueryParameters(queryParams, $"ApplicationTypeVersion={applicationTypeVersion}");
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ApplicationTypeManifestConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
