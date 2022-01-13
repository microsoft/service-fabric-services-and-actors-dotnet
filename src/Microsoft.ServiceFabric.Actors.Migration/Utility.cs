// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Fabric;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Runtime;

    internal class Utility
    {
        public KestrelCommunicationListener GetKVSKestrelCommunicationListener(StatefulServiceContext serviceContext, ActorTypeInformation actorTypeInformation, KvsActorStateProvider stateProvider)
        {
            var endpointName = ActorNameFormat.GetActorKvsMigrationEndpointName(actorTypeInformation.ImplementationType);

            return new KestrelCommunicationListener(serviceContext, endpointName, (url, listener) =>
                {
                    try
                    {
                        var endpoint = serviceContext.CodePackageActivationContext.GetEndpoint(endpointName);

                        ActorTrace.Source.WriteInfo("Migration.Utility.GetKVSKestrelCommunicationListener", $"Starting Kestrel on url: {url} host: {FabricRuntime.GetNodeContext().IPAddressOrFQDN} endpointPort: {endpoint.Port}");

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

                        ActorTrace.Source.WriteInfo("Migration.Utility.GetKVSKestrelCommunicationListener", "Successfully created webhostbuilder");

                        return webHostBuilder;
                    }
                    catch (Exception ex)
                    {
                        ActorTrace.Source.WriteInfo("Migration.Utility.GetKVSKestrelCommunicationListener", "Got exception in creating WebHostBuilder: " + ex);
                        throw;
                    }
                });
        }

        public KestrelCommunicationListener GetRCKestrelCommunicationListener(StatefulServiceContext serviceContext, ActorTypeInformation actorTypeInformation, KVStoRCMigrationActorStateProvider stateProvider)
        {
            var endpointName = ActorNameFormat.GetActorRcMigrationEndpointName(actorTypeInformation.ImplementationType);

            return new KestrelCommunicationListener(serviceContext, endpointName, (url, listener) =>
            {
                try
                {
                    var endpoint = serviceContext.CodePackageActivationContext.GetEndpoint(endpointName);

                    ActorTrace.Source.WriteInfo("Migration.Utility.GetRCKestrelCommunicationListener", $"Starting Kestrel on url: {url} host: {FabricRuntime.GetNodeContext().IPAddressOrFQDN} endpointPort: {endpoint.Port}");

                    var webHostBuilder =
                        new WebHostBuilder()
                            .UseKestrel()
                            .ConfigureServices(
                                services => services
                                    .AddSingleton<StatefulServiceContext>(serviceContext)
                                    .AddSingleton<ActorTypeInformation>(actorTypeInformation)
                                    .AddSingleton<KVStoRCMigrationActorStateProvider>(stateProvider))
                            .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                            .UseStartup<Startup>()
                            .UseUrls(url)
                            .Build();

                    ActorTrace.Source.WriteInfo("Migration.Utility.GetRCKestrelCommunicationListener", "Successfully created webhostbuilder");

                    return webHostBuilder;
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteInfo("Migration.Utility.GetRCKestrelCommunicationListener", "Got exception in creating WebHostBuilder: " + ex);
                    throw;
                }
            });
        }
    }
}
