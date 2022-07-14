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
    using Microsoft.AspNetCore.Hosting.Server;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    internal class GenericHostCommunicationListener : ICommunicationListener
    {
        private readonly Func<string, AspNetCoreCommunicationListener, IHost> build;
        private readonly ServiceContext serviceContext;
        private readonly AspNetCoreCommunicationListener listener;
        private IHost host;

        public GenericHostCommunicationListener(Func<string, AspNetCoreCommunicationListener, IHost> build, AspNetCoreCommunicationListener listener)
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
                throw new InvalidOperationException("IHost returned from build delegate is null.");
            }

            await this.host.StartAsync(cancellationToken);

            var server = this.host.Services.GetService<IServer>();
            if (server == null)
            {
                throw new InvalidOperationException("No web server found over IHost");
            }

            var url = server.Features.Get<IServerAddressesFeature>().Addresses.FirstOrDefault();
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

            url = url.TrimEnd(new[] { '/' });

            return url;
        }
    }
}
