// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    internal class WebHostCommunicationListener : ICommunicationListener
    {
        private readonly Func<string, AspNetCoreCommunicationListener, IWebHost> build;
        private readonly ServiceContext serviceContext;
        private readonly AspNetCoreCommunicationListener listener;
        private IWebHost host;

        public WebHostCommunicationListener(Func<string, AspNetCoreCommunicationListener, IWebHost> build, AspNetCoreCommunicationListener listener)
        {
            this.serviceContext = listener.ServiceContext;
            this.build = build;
            this.listener = listener;
        }

        public void Abort()
        {
            if (this.host != null)
            {
                this.host.Dispose();
            }
        }

        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            if (this.host != null)
            {
                await this.host.StopAsync(cancellationToken);
                this.host.Dispose();
            }
        }

        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            this.host = this.build(this.listener.GetListenerUrl(), this.listener);
            if (this.host == null)
            {
                throw new InvalidOperationException("IWebHost returned from build delegate is null.");
            }

            await this.host.StartAsync(cancellationToken);

            var url = this.host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.FirstOrDefault();

            if (url == null)
            {
                throw new InvalidOperationException("No Url returned from AspNetCore IServerAddressesFeature.");
            }

            var publishAddress = this.serviceContext.PublishAddress;

            if (url.Contains("://+:"))
            {
                url = url.Replace("://+:", $"://{publishAddress}:");
            }
            else if (url.Contains("://[::]:"))
            {
                url = url.Replace("://[::]:", $"://{publishAddress}:");
            }

            // When returning url to naming service, add UrlSuffix to it.
            // This UrlSuffix will be used by middleware to:
            //    - drop calls not intended for the service and return 410.
            //    - modify Path and PathBase in Microsoft.AspNetCore.Http.HttpRequest to be sent correctly to the service code.
            url = url.TrimEnd(new[] { '/' }) + this.listener.UrlSuffix;

            return url;
        }
    }
}
