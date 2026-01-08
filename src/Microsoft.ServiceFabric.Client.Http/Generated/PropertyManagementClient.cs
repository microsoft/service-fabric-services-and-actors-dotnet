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
    /// Class containing methods for performing PropertyManagementClient operations.
    /// </summary>
    internal partial class PropertyManagementClient : IPropertyManagementClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the PropertyManagementClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public PropertyManagementClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task CreateNameAsync(
            FabricName fabricName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            fabricName.ThrowIfNull(nameof(fabricName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Names/$/Create";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                NameDescriptionConverter.Serialize(new JsonTextWriter(sw), fabricName);
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
        public Task GetNameExistsInfoAsync(
            string nameId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nameId.ThrowIfNull(nameof(nameId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Names/{nameId}";
            url = url.Replace("{nameId}", nameId);
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

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteNameAsync(
            string nameId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nameId.ThrowIfNull(nameof(nameId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Names/{nameId}";
            url = url.Replace("{nameId}", nameId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Delete,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<string>> GetSubNameInfoListAsync(
            string nameId,
            bool? recursive = false,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nameId.ThrowIfNull(nameof(nameId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Names/{nameId}/$/GetSubNames";
            url = url.Replace("{nameId}", nameId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            recursive?.AddToQueryParameters(queryParams, $"Recursive={recursive}");
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

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, JsonReaderExtensions.ReadValueAsString, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<PropertyInfo>> GetPropertyInfoListAsync(
            string nameId,
            bool? includeValues = false,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nameId.ThrowIfNull(nameof(nameId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Names/{nameId}/$/GetProperties";
            url = url.Replace("{nameId}", nameId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            includeValues?.AddToQueryParameters(queryParams, $"IncludeValues={includeValues}");
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

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, PropertyInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task PutPropertyAsync(
            string nameId,
            PropertyDescription propertyDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nameId.ThrowIfNull(nameof(nameId));
            propertyDescription.ThrowIfNull(nameof(propertyDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Names/{nameId}/$/GetProperty";
            url = url.Replace("{nameId}", nameId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                PropertyDescriptionConverter.Serialize(new JsonTextWriter(sw), propertyDescription);
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
        public Task<PropertyInfo> GetPropertyInfoAsync(
            string nameId,
            string propertyName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nameId.ThrowIfNull(nameof(nameId));
            propertyName.ThrowIfNull(nameof(propertyName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Names/{nameId}/$/GetProperty";
            url = url.Replace("{nameId}", nameId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            propertyName?.AddToQueryParameters(queryParams, $"PropertyName={propertyName}");
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, PropertyInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeletePropertyAsync(
            string nameId,
            string propertyName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nameId.ThrowIfNull(nameof(nameId));
            propertyName.ThrowIfNull(nameof(propertyName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Names/{nameId}/$/GetProperty";
            url = url.Replace("{nameId}", nameId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            propertyName?.AddToQueryParameters(queryParams, $"PropertyName={propertyName}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Delete,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PropertyBatchInfo> SubmitPropertyBatchAsync(
            string nameId,
            PropertyBatchDescriptionList propertyBatchDescriptionList,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nameId.ThrowIfNull(nameof(nameId));
            propertyBatchDescriptionList.ThrowIfNull(nameof(propertyBatchDescriptionList));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Names/{nameId}/$/GetProperties/$/SubmitBatch";
            url = url.Replace("{nameId}", nameId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                PropertyBatchDescriptionListConverter.Serialize(new JsonTextWriter(sw), propertyBatchDescriptionList);
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, PropertyBatchInfoConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
