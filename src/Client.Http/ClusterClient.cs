// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.ServiceFabric.Client.Http.Serialization;
using Microsoft.ServiceFabric.Common;

namespace Microsoft.ServiceFabric.Client.Http
{
    sealed partial class ClusterClient : IClusterClient
    {
        async Task<string> IClusterClient.GetImageStoreConnectionStringAsync(long? serverTimeout, CancellationToken cancellationToken)
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var cluster = XDocument.Parse((await httpClient.Cluster.GetClusterManifestAsync(serverTimeout, cancellationToken)).Manifest);
            var r = new XmlNamespaceManager(new NameTable());
            r.AddNamespace("ns", cluster.Root.Attribute("xmlns").Value);
            string imageStore = cluster.XPathSelectElement("/ns:ClusterManifest/ns:FabricSettings/ns:Section[@Name='Management']/ns:Parameter[@Name='ImageStoreConnectionString']", r).Attribute("Value").Value;
            return imageStore;
        }

        Task<TokenServiceMetadata> IClusterClient.GetTokenServiceMetadtaAsync(long? serverTimeout, CancellationToken cancellationToken)
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);

            string requestId = Guid.NewGuid().ToString();
            string url = "$/GetDstsMetadata";
            var queryParams = new List<string>();

            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=1.0");
            url += "?" + string.Join("&", queryParams);

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage() { Method = HttpMethod.Get };
                return request;
            }

            return httpClient.SendAsyncGetResponse(RequestFunc, url, TokenServiceMetadataConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
