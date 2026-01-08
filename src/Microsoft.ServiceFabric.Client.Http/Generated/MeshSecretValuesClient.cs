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
    /// Class containing methods for performing MeshSecretValuesClient operations.
    /// </summary>
    internal partial class MeshSecretValuesClient : IMeshSecretValuesClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the MeshSecretValuesClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public MeshSecretValuesClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task<SecretValueResourceDescription> AddValueAsync(
            string secretResourceName,
            string secretValueResourceName,
            SecretValueResourceDescription secretValueResourceDescription,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            secretResourceName.ThrowIfNull(nameof(secretResourceName));
            secretValueResourceName.ThrowIfNull(nameof(secretValueResourceName));
            secretValueResourceDescription.ThrowIfNull(nameof(secretValueResourceDescription));
            var requestId = Guid.NewGuid().ToString();
            var url = "Resources/Secrets/{secretResourceName}/values/{secretValueResourceName}";
            url = url.Replace("{secretResourceName}", secretResourceName);
            url = url.Replace("{secretValueResourceName}", secretValueResourceName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.4-preview");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                SecretValueResourceDescriptionConverter.Serialize(new JsonTextWriter(sw), secretValueResourceDescription);
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, SecretValueResourceDescriptionConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<SecretValueResourceDescription> GetAsync(
            string secretResourceName,
            string secretValueResourceName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            secretResourceName.ThrowIfNull(nameof(secretResourceName));
            secretValueResourceName.ThrowIfNull(nameof(secretValueResourceName));
            var requestId = Guid.NewGuid().ToString();
            var url = "Resources/Secrets/{secretResourceName}/values/{secretValueResourceName}";
            url = url.Replace("{secretResourceName}", secretResourceName);
            url = url.Replace("{secretValueResourceName}", secretValueResourceName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.4-preview");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, SecretValueResourceDescriptionConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteAsync(
            string secretResourceName,
            string secretValueResourceName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            secretResourceName.ThrowIfNull(nameof(secretResourceName));
            secretValueResourceName.ThrowIfNull(nameof(secretValueResourceName));
            var requestId = Guid.NewGuid().ToString();
            var url = "Resources/Secrets/{secretResourceName}/values/{secretValueResourceName}";
            url = url.Replace("{secretResourceName}", secretResourceName);
            url = url.Replace("{secretValueResourceName}", secretValueResourceName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.4-preview");
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
        public Task<PagedData<SecretValueResourceDescription>> ListAsync(
            string secretResourceName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            secretResourceName.ThrowIfNull(nameof(secretResourceName));
            var requestId = Guid.NewGuid().ToString();
            var url = "Resources/Secrets/{secretResourceName}/values";
            url = url.Replace("{secretResourceName}", secretResourceName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            queryParams.Add("api-version=6.4-preview");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, SecretValueResourceDescriptionConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<SecretValue> ShowAsync(
            string secretResourceName,
            string secretValueResourceName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            secretResourceName.ThrowIfNull(nameof(secretResourceName));
            secretValueResourceName.ThrowIfNull(nameof(secretValueResourceName));
            var requestId = Guid.NewGuid().ToString();
            var url = "Resources/Secrets/{secretResourceName}/values/{secretValueResourceName}/list_value";
            url = url.Replace("{secretResourceName}", secretResourceName);
            url = url.Replace("{secretValueResourceName}", secretValueResourceName);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, SecretValueConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
