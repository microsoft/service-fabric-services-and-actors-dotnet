// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client.Http.Serialization;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Class containing methods for performing MeshSecretValuesClient operations.
    /// </summary>
    internal partial class MeshSecretValuesClient : IMeshSecretValuesClient
    {
        /// <inheritdoc />
        public Task<SecretValueResourceDescription> AddValueAsync(
            string secretResourceName,
            string secretValueResourceName,
            string jsonDescription,
            string apiVersion = Constants.DefaultApiVersionForResources,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            secretResourceName.ThrowIfNull(nameof(secretResourceName));
            secretValueResourceName.ThrowIfNull(nameof(secretValueResourceName));
            jsonDescription.ThrowIfNull(nameof(jsonDescription));
            var requestId = Guid.NewGuid().ToString();
            var url = $"Resources/Secrets/{secretResourceName}/values/{secretValueResourceName}?api-version={apiVersion}";

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Put,
                    Content = new StringContent(jsonDescription, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, SecretValueResourceDescriptionConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
