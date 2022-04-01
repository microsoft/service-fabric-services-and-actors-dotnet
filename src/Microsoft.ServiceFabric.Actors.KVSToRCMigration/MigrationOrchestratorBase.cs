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
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime.Migration;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    /// <summary>
    /// Base class for migration orchestration.
    /// </summary>
    internal abstract class MigrationOrchestratorBase : IMigrationOrchestrator
    {
        private static readonly string TraceType = typeof(MigrationOrchestratorBase).Name;

        private StatefulServiceContext serviceContext;
        private ActorTypeInformation actorTypeInformation;
        private string traceId;
        private MigrationSettings migrationSettings;
        private Func<bool, CancellationToken, Task> completionCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationOrchestratorBase"/> class.
        /// </summary>
        /// <param name="actorTypeInformation">The type information of the Actor.</param>
        /// <param name="serviceContext">Service context the actor service is operating under.</param>
        /// <param name="migrationSettings">Migration settings.</param>
        public MigrationOrchestratorBase(StatefulServiceContext serviceContext, ActorTypeInformation actorTypeInformation, Actors.Runtime.Migration.MigrationSettings migrationSettings)
        {
            this.actorTypeInformation = actorTypeInformation;
            this.serviceContext = serviceContext;
            this.traceId = this.serviceContext.TraceId;
            if (migrationSettings != null && migrationSettings is MigrationSettings)
            {
                this.migrationSettings = (MigrationSettings)migrationSettings;
            }
            else
            {
                this.migrationSettings = new MigrationSettings();
                this.migrationSettings.LoadFrom(
                     this.StatefulServiceContext.CodePackageActivationContext,
                     ActorNameFormat.GetMigrationConfigSectionName(this.actorTypeInformation.ImplementationType));
            }
        }

        internal ActorTypeInformation ActorTypeInformation { get => this.actorTypeInformation; }

        internal StatefulServiceContext StatefulServiceContext { get => this.serviceContext; }

        internal string TraceId { get => this.traceId; }

        internal MigrationSettings MigrationSettings { get => this.migrationSettings; }

        internal Func<bool, CancellationToken, Task> CompletionCallback { get => this.completionCallback; }

        /// <inheritdoc/>
        public abstract bool AreActorCallsAllowed();

        /// <inheritdoc/>
        public abstract IActorStateProvider GetMigrationActorStateProvider();

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public IServiceRemotingMessageHandler GetMessageHandler(ActorService actorService, IServiceRemotingMessageHandler messageHandler, Func<RequestForwarderContext, IRequestForwarder> requestForwarderFactory)
        {
            var forwardServiceUri = this.GetForwardServiceUri();
            if (forwardServiceUri == null)
            {
                // Service is not configured to forward requests.
                return messageHandler;
            }

            var partitionInformation = this.GetInt64RangePartitionInformation();
            var lowKey = partitionInformation.LowKey;

            return new RequestForwardableRemotingDispatcher(
                actorService,
                messageHandler,
                requestForwarderFactory.Invoke(new RequestForwarderContext
                {
                    ServiceUri = forwardServiceUri,
                    ServicePartitionKey = new ServicePartitionKey(lowKey),
                    ReplicaSelector = TargetReplicaSelector.PrimaryReplica,
                    TraceId = this.StatefulServiceContext.TraceId,
                }));
        }

        /// <summary>
        /// Gets the migration start mode.
        /// </summary>
        /// <returns>Return true if the MigrationMode is Auto, false otherwise.</returns>
        public virtual bool IsAutoStartMigration()
        {
            return this.MigrationSettings.MigrationMode == MigrationMode.Auto;
        }

        public void RegisterCompletionCallback(Func<bool, CancellationToken, Task> completionCallback)
        {
            this.completionCallback = completionCallback;
        }

        /// <inheritdoc/>
        public abstract Task StartDowntimeAsync(CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task StartMigrationAsync(CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task<bool> TryResumeMigrationAsync(CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task AbortMigrationAsync(CancellationToken cancellationToken);

        public abstract bool IsActorCallToBeForwarded();

        public abstract void ThrowIfActorCallsDisallowed();

        protected Task InvokeCompletionCallback(bool actorCallsAllowed, CancellationToken cancellationToken)
        {
            if (this.completionCallback != null)
            {
                return this.completionCallback.Invoke(actorCallsAllowed, cancellationToken);
            }

            ActorTrace.Source.WriteWarningWithId(
                TraceType,
                this.TraceId,
                "Completion callback not registered. Ignoriing callback invocation.");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the migration endpoint name.
        /// </summary>
        /// <returns>Migration endpoint name.</returns>
        protected abstract string GetMigrationEndpointName();

        protected abstract Int64RangePartitionInformation GetInt64RangePartitionInformation();

        protected abstract Uri GetForwardServiceUri();
    }
}
