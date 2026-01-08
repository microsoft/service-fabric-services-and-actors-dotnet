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
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Microsoft.ServiceFabric.Client;
    using Microsoft.ServiceFabric.Client.Http.Serialization;
    using Microsoft.ServiceFabric.Common;
    using Newtonsoft.Json;

    /// <summary>
    /// Class containing methods for performing ClusterClient operataions.
    /// </summary>
    internal partial class ClusterClient : IClusterClient
    {        
        /// <inheritdoc />
        public async Task<string> GetImageStoreConnectionStringAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var cluster = XDocument.Parse((await this.httpClient.Cluster.GetClusterManifestAsync(serverTimeout, cancellationToken)).Manifest);
            XmlNamespaceManager r = new XmlNamespaceManager(new NameTable());
            r.AddNamespace("ns", cluster.Root.Attribute("xmlns").Value);
            var imageStore = cluster.XPathSelectElement("/ns:ClusterManifest/ns:FabricSettings/ns:Section[@Name='Management']/ns:Parameter[@Name='ImageStoreConnectionString']", r).Attribute("Value").Value;
            return imageStore;
        }

        /// <inheritdoc />
        public Task<TokenServiceMetadata> GetTokenServiceMetadtaAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);

            var requestId = Guid.NewGuid().ToString();
            var url = "$/GetDstsMetadata";
            var queryParams = new List<string>();

            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=1.0");
            url += "?" + string.Join("&", queryParams);

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, TokenServiceMetadataConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
