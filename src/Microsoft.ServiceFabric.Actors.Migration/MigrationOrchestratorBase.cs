// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    internal abstract class MigrationOrchestratorBase : IMigrationOrchestrator
    {
        private static readonly string TraceType = typeof(MigrationOrchestratorBase).Name;

        private StatefulServiceContext serviceContext;
        private ActorTypeInformation actorTypeInformation;
        private string traceId;
        private Action<bool> stateProviderStateChangeCallback;
        private MigrationSettings migrationSettings;

        public MigrationOrchestratorBase(StatefulServiceContext serviceContext, ActorTypeInformation actorTypeInformation, Action<bool> stateProviderStateChangeCallback)
        {
            this.actorTypeInformation = actorTypeInformation;
            this.serviceContext = serviceContext;
            this.traceId = this.serviceContext.TraceId;
            this.stateProviderStateChangeCallback = stateProviderStateChangeCallback;
            this.migrationSettings = new MigrationSettings();
            this.migrationSettings.LoadFrom(
                 this.StatefulServiceContext.CodePackageActivationContext,
                 ActorNameFormat.GetMigrationConfigSectionName(this.actorTypeInformation.ImplementationType));
        }

        public ActorTypeInformation ActorTypeInformation { get => this.actorTypeInformation; }

        public StatefulServiceContext StatefulServiceContext { get => this.serviceContext; }

        public string TraceId { get => this.traceId; }

        public Action<bool> StateProviderStateChangeCallback { get => this.stateProviderStateChangeCallback; }

        public MigrationSettings MigrationSettings { get => this.migrationSettings; }

        public abstract Task<bool> AreActorCallsAllowedAsync(CancellationToken cancellationToken);

        public abstract IActorStateProvider GetMigrationActorStateProvider();

        public ICommunicationListener GetMigrationCommunicationListener()
        {
            var endpointName = this.GetMigrationEndpointName();

            return new KestrelCommunicationListener(this.serviceContext, endpointName, (url, listener) =>
            {
                try
                {
                    var endpoint = this.serviceContext.CodePackageActivationContext.GetEndpoint(endpointName);

                    ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.TraceId,
                        $"Starting Kestrel on url: {url} host: {FabricRuntime.GetNodeContext().IPAddressOrFQDN} endpointPort: {endpoint.Port}");

                    var webHostBuilder =
                        new WebHostBuilder()
                            .UseKestrel()
                            .ConfigureServices(
                                services => services
                                    .AddSingleton<IMigrationOrchestrator>(this))
                            .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                            .UseStartup<Startup>()
                            .UseUrls(url)
                            .Build();

                    return webHostBuilder;
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        this.TraceId,
                        "Error encountered while creating WebHostBuilder: " + ex);

                    throw;
                }
            });
        }

        public abstract Task StartDowntimeAsync(CancellationToken cancellationToken);

        public abstract Task StartMigrationAsync(CancellationToken cancellationToken);

        public abstract Task AbortMigrationAsync(CancellationToken cancellationToken);

        protected abstract string GetMigrationEndpointName();
    }
}
