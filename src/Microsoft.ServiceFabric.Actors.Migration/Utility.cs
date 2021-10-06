// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System.Fabric;
    using Microsoft.ServiceFabric.Actors.Runtime;
#if DotNetCoreClr
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
#endif

    internal class Utility
    {
        ////public GrpcCommunicationListener GetKVSGrpcCommunicationListener(StatefulServiceContext serviceContext, ActorTypeInformation actorTypeInformation, KvsActorStateProvider stateProvider)
        ////{
        ////    return new GrpcCommunicationListener(serviceContext, actorTypeInformation, new[] { KvsMigration.BindService(new KvsMigrationService(stateProvider)) });
        ////}

        ////public OwinCommunicationListener GetKVSOwinCommunicationListener(StatefulServiceContext serviceContext, ActorTypeInformation actorTypeInformation, KvsActorStateProvider stateProvider)
        ////{
        ////    return null; // new OwinCommunicationListener(serviceContext, actorTypeInformation, new[] { KvsMigration.BindService(new KvsMigrationService(stateProvider)) });
        ////}

#if DotNetCoreClr
        public KestrelCommunicationListener GetKVSKestrelCommunicationListener(StatefulServiceContext serviceContext, ActorTypeInformation actorTypeInformation, KvsActorStateProvider stateProvider)
        {
            return new KestrelCommunicationListener(serviceContext, (url, listener) =>
                     new WebHostBuilder()
                        .UseKestrel()
                        .ConfigureServices(
                             services => services
                                 .AddSingleton<StatefulServiceContext>(serviceContext)
                                 .AddSingleton<ActorTypeInformation>(actorTypeInformation)
                                 .AddSingleton<KvsActorStateProvider>(stateProvider))
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .ConfigureLogging(logging =>
                        {
                            logging.ClearProviders();
                            logging.AddEventLog();
                        })
                        .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                        .UseStartup<Startup>()
                        .UseUrls(url)
                        .Build());
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
