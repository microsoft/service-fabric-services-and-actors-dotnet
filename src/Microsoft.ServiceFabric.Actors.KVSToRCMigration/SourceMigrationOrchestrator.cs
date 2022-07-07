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
    using Microsoft.ServiceFabric.Actors.Migration.Exceptions;
    using Microsoft.ServiceFabric.Actors.Runtime;

    /// <summary>
    /// Migration orchestrator for source(KVS based) service.
    /// </summary>
    internal class SourceMigrationOrchestrator : MigrationOrchestratorBase
    {
        private static readonly string TraceType = typeof(SourceMigrationOrchestrator).Name;
        private KvsActorStateProvider migrationActorStateProvider;
        private bool actorCallsAllowed;
        private bool forwardRequest;
        private ActorStateProviderHelper stateProviderHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceMigrationOrchestrator"/> class.
        /// </summary>
        /// <param name="stateProvider">KVS actor state provider.</param>
        /// <param name="actorTypeInfo">The type information of the Actor.</param>
        /// <param name="serviceContext">Service context the actor service is operating under.</param>
        /// <param name="migrationSettings">Migration settings.</param>
        public SourceMigrationOrchestrator(IActorStateProvider stateProvider, ActorTypeInformation actorTypeInfo, StatefulServiceContext serviceContext, Actors.Runtime.Migration.MigrationSettings migrationSettings)
            : base(serviceContext, actorTypeInfo, migrationSettings)
        {
            if (stateProvider.GetType() != typeof(KvsActorStateProvider))
            {
                var errorMsg = $"{stateProvider.GetType()} not a valid state provider type for source of migration. {typeof(KvsActorStateProvider)} is the valid type.";
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.TraceId,
                    errorMsg);

                throw new InvalidMigrationStateProviderException(errorMsg);
            }

            this.actorCallsAllowed = false;
            this.forwardRequest = false;
            this.migrationActorStateProvider = stateProvider as KvsActorStateProvider;
            this.stateProviderHelper = new ActorStateProviderHelper(this.migrationActorStateProvider);
        }

        /// <inheritdoc/>
        public override async Task AbortMigrationAsync(bool userTriggered, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                "Aborting Migration");

            await this.migrationActorStateProvider.ResumeWritesAsync(this.TraceId, cancellationToken);
            this.actorCallsAllowed = true;
            this.forwardRequest = false;
            await this.InvokeCompletionCallback(this.actorCallsAllowed, cancellationToken);
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
        public override async Task StartDowntimeAsync(bool userTriggered, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                "Starting Downtime");

            await this.migrationActorStateProvider.RejectWritesAsync(this.TraceId, cancellationToken);
            this.actorCallsAllowed = false;
            this.forwardRequest = true;

            await this.InvokeCompletionCallback(this.actorCallsAllowed, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task StartMigrationAsync(bool userTriggered, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                "Starting Migration");

            await this.StartOrResumeMigrationAsync(cancellationToken);
        }

        public override bool IsActorCallToBeForwarded()
        {
            return this.forwardRequest && this.MigrationSettings.TargetServiceUri != null;
        }

        public override void ThrowIfActorCallsDisallowed()
        {
            if (this.actorCallsAllowed)
            {
                return;
            }

            var errorMsg = $"Actor calls are not allowed on the service.";
            if (this.MigrationSettings.TargetServiceUri == null)
            {
                errorMsg += $" Configure TargetServiceUri in {this.MigrationSettings.MigrationConfigSectionName} section of settings file to forward the request.";
            }

            throw new ActorCallsDisallowedException(errorMsg);
        }

        public override bool IsAutoStartMigration()
        {
            return true;
        }

        protected override Uri GetForwardServiceUri()
        {
            return this.MigrationSettings.TargetServiceUri;
        }

        protected override Int64RangePartitionInformation GetInt64RangePartitionInformation()
        {
            var servicePartition = this.migrationActorStateProvider.StatefulServicePartition;
            return servicePartition.PartitionInfo as Int64RangePartitionInformation;
        }

        /// <inheritdoc/>
        protected override string GetMigrationEndpointName()
        {
            // TODO: Validate migration EP in service manifest
            return ActorNameFormat.GetMigrationSourceEndpointName(this.ActorTypeInformation.ImplementationType);
        }

        private async Task<bool> AreActorCallsAllowedInternalAsync(CancellationToken cancellationToken)
        {
            return await this.stateProviderHelper.ExecuteWithRetriesAsync(
                async () => !(await this.migrationActorStateProvider.GetRejectWriteStateAsync(this.TraceId, cancellationToken)),
                "KVSActorStateProvider.GetRejectWriteState",
                CancellationToken.None);
        }

        private async Task StartOrResumeMigrationAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                "Starting or resuming Migration");

            this.actorCallsAllowed = await this.AreActorCallsAllowedInternalAsync(cancellationToken);
            this.forwardRequest = !this.actorCallsAllowed;
            if (!this.migrationActorStateProvider.IsTombstoneCleanupDisabled())
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.TraceId,
                    "Tombstone cleanup is not enabled.");

                var healthInfo = new HealthInformation("ActorStateMigration", "ActorStateMigrationChecks", HealthState.Warning)
                {
                    TimeToLive = TimeSpan.MaxValue,
                    RemoveWhenExpired = false,
                    Description = KvsActorStateProviderExtensions.TombstoneCleanupMessage,
                };

                this.migrationActorStateProvider.ReportPartitionHealth(healthInfo);
            }

            await this.InvokeCompletionCallback(this.actorCallsAllowed, cancellationToken);
        }
    }
}
