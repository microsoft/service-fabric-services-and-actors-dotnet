// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System.Fabric;
    using Microsoft.ServiceFabric.Actors.Runtime;
#if DotNetCoreClr
    using System;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
#endif

    internal class Utility
    {
        ////public OwinCommunicationListener GetKVSOwinCommunicationListener(StatefulServiceContext serviceContext, ActorTypeInformation actorTypeInformation, KvsActorStateProvider stateProvider)
        ////{
        ////    return null; // new OwinCommunicationListener(serviceContext, actorTypeInformation, new[] { KvsMigration.BindService(new KvsMigrationService(stateProvider)) });
        ////}

#if DotNetCoreClr
        public KestrelCommunicationListener GetKVSKestrelCommunicationListener(StatefulServiceContext serviceContext, ActorTypeInformation actorTypeInformation, KvsActorStateProvider stateProvider)
        {
            var endpointName = ActorNameFormat.GetActorKvsMigrationEndpointName(actorTypeInformation.ImplementationType);

            return new KestrelCommunicationListener(serviceContext, endpointName, (url, listener) =>
                {
                    try
                    {
                        var endpoint = serviceContext.CodePackageActivationContext.GetEndpoint(endpointName);

                        ActorTrace.Source.WriteInfo("Migration.Utility", $"Starting Kestrel on url: {url} host: {FabricRuntime.GetNodeContext().IPAddressOrFQDN} endpointPort: {endpoint.Port}");

                        var webHostBuilder =
                            new WebHostBuilder()
                                .UseKestrel()
                                .ConfigureServices(
                                    services => services
                                        .AddSingleton<StatefulServiceContext>(serviceContext)
                                        .AddSingleton<ActorTypeInformation>(actorTypeInformation)
                                        .AddSingleton<KvsActorStateProvider>(stateProvider))
                                .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                                .UseStartup<Startup>()
                                .UseUrls(url)
                                .Build();

                        ActorTrace.Source.WriteInfo("Migration.Utility", "Successfully created webhostbuilder");

                        return webHostBuilder;
                    }
                    catch (Exception ex)
                    {
                        ActorTrace.Source.WriteInfo("Migration.Utility", "Got exception in creating WebHostBuilder: " + ex);
                        throw;
                    }
                });
        }
#endif

        public bool RejectWrites(KvsActorStateProvider stateProvider)
        {
            if (stateProvider.GetKvsRejectWriteStatusAsync())
            {
                return stateProvider.TryAbortExistingTransactionsAndRejectWrites();
            }

            return false;
        }
    }
}