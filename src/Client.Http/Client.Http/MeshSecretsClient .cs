// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Client.Http.Serialization;
using Microsoft.ServiceFabric.Common;

namespace Microsoft.ServiceFabric.Client.Http
{
    sealed partial class MeshSecretsClient : IMeshSecretsClient
    {
        Task<SecretResourceDescription> IMeshSecretsClient.CreateOrUpdateAsync(string secretResourceName, string jsonDescription, string apiVersion, CancellationToken cancellationToken)
        {
            secretResourceName.ThrowIfNull(nameof(secretResourceName));
            jsonDescription.ThrowIfNull(nameof(jsonDescription));

            string requestId = Guid.NewGuid().ToString();
            string url = $"Resources/Secrets/{secretResourceName}?api-version={apiVersion}";

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Put,
                    Content = new StringContent(jsonDescription, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return httpClient.SendAsyncGetResponse(RequestFunc, url, SecretResourceDescriptionConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
