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

    internal class SourceMigrationOrchestrator : MigrationOrchestratorBase
    {
        private static readonly string TraceType = typeof(SourceMigrationOrchestrator).Name;
        private static readonly string TombstoneCleanupIsNotDisabledForMigrationHealthProperty = "TombstoneCleanupIsNotDisabledForMigration";
        private KvsActorStateProvider migrationActorStateProvider;

        public SourceMigrationOrchestrator(IActorStateProvider stateProvider, ActorTypeInformation actorTypeInfo, StatefulServiceContext serviceContext, Action<bool> stateProviderStateChangeCallback)
            : base(serviceContext, actorTypeInfo, stateProviderStateChangeCallback)
        {
            if (stateProvider.GetType() != typeof(KvsActorStateProvider))
            {
                // TODO throw
            }

            this.migrationActorStateProvider = stateProvider as KvsActorStateProvider;
            this.StateProviderStateChangeCallback(this.AreActorCallsAllowed());
        }

        public override async Task AbortMigrationAsync(CancellationToken cancellationToken)
        {
            await this.migrationActorStateProvider.RejectWritesAsync();
            this.StateProviderStateChangeCallback(true);
        }

        public override Task<bool> AreActorCallsAllowedAsync(CancellationToken cancellationToken)
        {
            var rejectWrites = this.migrationActorStateProvider.GetRejectWriteState();
            return Task.FromResult(!rejectWrites);
        }

        public override IActorStateProvider GetMigrationActorStateProvider()
        {
           return this.migrationActorStateProvider;
        }

        public override async Task StartDowntimeAsync(CancellationToken cancellationToken)
        {
            await this.migrationActorStateProvider.RejectWritesAsync();
            this.StateProviderStateChangeCallback(false);
        }

        public override async Task StartMigrationAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                if (this.migrationActorStateProvider.GetStoreReplica().KeyValueStoreReplicaSettings.DisableTombstoneCleanup)
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

        protected override string GetMigrationEndpointName()
        {
            return ActorNameFormat.GetMigrationSourceEndpointName(this.ActorTypeInformation.ImplementationType);
        }

        private bool AreActorCallsAllowed()
        {
            return !this.migrationActorStateProvider.GetRejectWriteState();
        }
    }
}
