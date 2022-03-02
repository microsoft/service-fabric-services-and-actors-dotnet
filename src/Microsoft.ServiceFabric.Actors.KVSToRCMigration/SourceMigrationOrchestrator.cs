// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Fabric;
    using System.Fabric.Health;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Runtime;

    /// <summary>
    /// Migration orchestrator for source(KVS based) service.
    /// </summary>
    internal class SourceMigrationOrchestrator : MigrationOrchestratorBase
    {
        private static readonly string TraceType = typeof(SourceMigrationOrchestrator).Name;
        private static readonly string TombstoneCleanupIsNotDisabledForMigrationHealthProperty = "TombstoneCleanupIsNotDisabledForMigration";
        private KvsActorStateProvider migrationActorStateProvider;
        private bool actorCallsAllowed;
        private bool forwardRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceMigrationOrchestrator"/> class.
        /// </summary>
        /// <param name="stateProvider">KVS actor state provider.</param>
        /// <param name="actorTypeInfo">The type information of the Actor.</param>
        /// <param name="serviceContext">Service context the actor service is operating under.</param>
        public SourceMigrationOrchestrator(IActorStateProvider stateProvider, ActorTypeInformation actorTypeInfo, StatefulServiceContext serviceContext)
            : base(serviceContext, actorTypeInfo)
        {
            if (stateProvider.GetType() != typeof(KvsActorStateProvider))
            {
                // TODO throw
            }

            this.actorCallsAllowed = false;
            this.forwardRequest = false;
            this.migrationActorStateProvider = stateProvider as KvsActorStateProvider;
        }

        /// <inheritdoc/>
        public override async Task AbortMigrationAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                "Aborting Migration");

            await this.migrationActorStateProvider.ResumeWritesAsync();
            this.actorCallsAllowed = true;
            this.forwardRequest = false;
        }

        /// <inheritdoc/>
        public override bool AreActorCallsAllowed()
        {
            return this.actorCallsAllowed;
        }

        /// <inheritdoc/>
        public override IActorStateProvider GetMigrationActorStateProvider()
        {
           return this.migrationActorStateProvider;
        }

        /// <inheritdoc/>
        public override async Task StartDowntimeAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                "Starting Downtime");

            await this.migrationActorStateProvider.RejectWritesAsync();
            this.actorCallsAllowed = false;
            this.forwardRequest = true;
        }

        /// <inheritdoc/>
        public override async Task StartMigrationAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                "Starting Migration");

            this.actorCallsAllowed = this.AreActorCallsAllowedInternal();
            this.forwardRequest = !this.actorCallsAllowed;
            await Task.Run(() =>
            {
                if (!this.migrationActorStateProvider.GetStoreReplica().KeyValueStoreReplicaSettings.DisableTombstoneCleanup)
                {
                    ActorTrace.Source.WriteWarningWithId(
                        TraceType,
                        this.TraceId,
                        "Tombstone cleanup is not enabled.");

                    var healthInfo = new HealthInformation("KvsActorStateProvider", TombstoneCleanupIsNotDisabledForMigrationHealthProperty, HealthState.Warning)
                    {
                        TimeToLive = TimeSpan.MaxValue,
                        RemoveWhenExpired = false,
                        Description = "Tombstone cleanup(KeyValueStoreReplicaSettings.DisableTombstoneCleanup) must be disabled during the migration so that deletes can be tracked and copied from KVS to Reliable Collections.",
                    };

                    this.migrationActorStateProvider.ReportPartitionHealth(healthInfo);
                }
            });
        }

        public override bool IsActorCallToBeForwarded()
        {
            return this.forwardRequest && this.MigrationSettings.TargetServiceUri != null;
        }

        protected override Uri GetForwardServiceUri()
        {
            return this.MigrationSettings.TargetServiceUri;
        }

        protected override Int64RangePartitionInformation GetInt64RangePartitionInformation()
        {
            var servicePartition = this.migrationActorStateProvider.StatefulServicePartition;
            if (servicePartition == null)
            {
                // TODO throw
            }

            return servicePartition.PartitionInfo as Int64RangePartitionInformation;
        }

        /// <inheritdoc/>
        protected override string GetMigrationEndpointName()
        {
            // TODO: Validate migration EP in service manifest
            return ActorNameFormat.GetMigrationSourceEndpointName(this.ActorTypeInformation.ImplementationType);
        }

        private bool AreActorCallsAllowedInternal()
        {
            return !this.migrationActorStateProvider.GetRejectWriteState();
        }
    }
}
