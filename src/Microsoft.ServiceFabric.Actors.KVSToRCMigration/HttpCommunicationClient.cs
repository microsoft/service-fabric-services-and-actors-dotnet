// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Fabric;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal class HttpCommunicationClient : ICommunicationClient
    {
        private Uri endpointUri;
        private HttpClient httpClient;

        public HttpCommunicationClient(string address, MigrationSecuritySettings securitySettings)
        {
            this.endpointUri = new Uri(address.EndsWith("/") ? address : $"{address}/");
            this.httpClient = this.GetHttpClient(this.endpointUri, securitySettings);
        }

        public Uri EndpointUri { get => this.endpointUri; }

        public HttpClient HttpClient { get => this.httpClient; }

        public ResolvedServicePartition ResolvedServicePartition { get; set; }

        public string ListenerName { get; set; }

        public ResolvedServiceEndpoint Endpoint { get; set; }

        private HttpClient GetHttpClient(Uri endpointUri, MigrationSecuritySettings securitySettings)
        {
            var certs = CertificateHelper.GetCertificates(securitySettings);
            var handler = this.CreateRequestHandler(certs.ToArray(), securitySettings);

            // CommunicationClientBase needs the BaseAddress to be set
            var httpClient = new HttpClient(handler);
            httpClient.BaseAddress = endpointUri;

            return httpClient;
        }

#if DotNetCoreClr
        private HttpClientHandler CreateRequestHandler(X509Certificate2[] clientCertificates, MigrationSecuritySettings securitySettings)
        {
            var handler = new HttpClientHandler();

            if (clientCertificates != null)
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ClientCertificates.AddRange(clientCertificates);
                handler.ServerCertificateCustomValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return CertificateHelper.IsValidRemoteCert(certificate, chain, securitySettings);
                };
            }

            return handler;
        }
#else
        private WinHttpHandler CreateRequestHandler(X509Certificate2[] clientCertificates, MigrationSecuritySettings securitySettings)
        {
            var handler = new WinHttpHandler();

            if (clientCertificates != null)
            {
                handler.ClientCertificateOption = ClientCertificateOption.Manual;
                handler.ClientCertificates.AddRange(clientCertificates);
                handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return CertificateHelper.IsValidRemoteCert(certificate, chain, securitySettings);
                };
            }

            return handler;
        }
#endif
    }
}
