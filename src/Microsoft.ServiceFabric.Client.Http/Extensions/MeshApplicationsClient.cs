// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client.Http.Serialization;
    using Microsoft.ServiceFabric.Common;
    using Newtonsoft.Json;

    internal partial class MeshApplicationsClient : IMeshApplicationsClient
    {
        /// <inheritdoc />
        public Task<ApplicationResourceDescription> CreateOrUpdateAsync(
            string applicationResourceName,
            string jsonDescription,
            string apiVersion = Constants.DefaultApiVersionForResources,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationResourceName.ThrowIfNull(nameof(applicationResourceName));
            jsonDescription.ThrowIfNull(nameof(jsonDescription));            

            string requestId = Guid.NewGuid().ToString();
            var url = $"Resources/Applications/{applicationResourceName}?api-version={apiVersion}";

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

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, ApplicationResourceDescriptionConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
