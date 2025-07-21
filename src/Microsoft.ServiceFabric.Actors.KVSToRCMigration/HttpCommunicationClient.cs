// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Fabric;
    using System.Net.Http;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal class HttpCommunicationClient : ICommunicationClient
    {
        private Uri endpointUri;
        private HttpClient httpClient;

        public HttpCommunicationClient(string address)
        {
            this.endpointUri = new Uri(address.EndsWith("/") ? address : $"{address}/");
#if !NET
            var handler = new WinHttpHandler();
            this.httpClient = new HttpClient(handler)
            {
                BaseAddress = this.endpointUri,
            };
#else
            this.httpClient = new HttpClient()
            {
                BaseAddress = this.endpointUri,
            };
#endif
        }

        public HttpCommunicationClient(HttpClient httpClient)
        {
            this.endpointUri = httpClient.BaseAddress;
            this.httpClient = httpClient;
        }

        public Uri EndpointUri { get => this.endpointUri; }

        public HttpClient HttpClient { get => this.httpClient; }

        public ResolvedServicePartition ResolvedServicePartition { get; set; }

        public string ListenerName { get; set; }

        public ResolvedServiceEndpoint Endpoint { get; set; }
    }
}
