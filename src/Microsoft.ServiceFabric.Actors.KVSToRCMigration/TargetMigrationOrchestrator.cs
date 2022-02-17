// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Description;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationConstants;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationUtility;

    /// <summary>
    /// Orchestrator for Target(RC based) service.
    /// </summary>
    internal class TargetMigrationOrchestrator : MigrationOrchestratorBase
    {
        private static readonly string TraceType = typeof(TargetMigrationOrchestrator).Name;
        private static volatile int isMigrationWorkflowRunning = 0;
        private KVStoRCMigrationActorStateProvider migrationActorStateProvider;
        private IReliableDictionary2<string, string> metadataDict;
        private ServicePartitionClient<HttpCommunicationClient> partitionClient;
        private volatile bool isMigrationCompleted = false;
        private CancellationTokenSource childCancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetMigrationOrchestrator"/> class.
        /// </summary>
        /// <param name="stateProvider">KVS actor state provider.</param>
        /// <param name="actorTypeInfo">The type information of the Actor.</param>
        /// <param name="serviceContext">Service context the actor service is operating under.</param>
        public TargetMigrationOrchestrator(IActorStateProvider stateProvider, ActorTypeInformation actorTypeInfo, StatefulServiceContext serviceContext)
            : base(serviceContext, actorTypeInfo)
        {
            if (stateProvider.GetType() != typeof(ReliableCollectionsActorStateProvider))
            {
                // TODO throw
            }

            this.migrationActorStateProvider = new KVStoRCMigrationActorStateProvider(stateProvider as ReliableCollectionsActorStateProvider);
        }

        internal Data.ITransaction Transaction { get => this.migrationActorStateProvider.GetStateManager().CreateTransaction(); }

        internal IReliableDictionary2<string, string> MetaDataDictionary { get => this.metadataDict; }

        internal ServicePartitionClient<HttpCommunicationClient> ServicePartitionClient
        {
            get
            {
                if (this.partitionClient == null)
                {
                    var partitionInformation = this.GetInt64RangePartitionInformation();
                    this.partitionClient = new ServicePartitionClient<HttpCommunicationClient>(
                            new HttpCommunicationClientFactory(null, new List<IExceptionHandler>() { new HttpExceptionHandler() }),
                            this.MigrationSettings.SourceServiceUri,
                            new ServicePartitionKey(partitionInformation.LowKey),
                            TargetReplicaSelector.PrimaryReplica,
                            Runtime.Migration.Constants.MigrationListenerName);
                }

                return this.partitionClient;
            }
        }

        internal KVStoRCMigrationActorStateProvider StateProvider { get => this.migrationActorStateProvider; }

        /// <inheritdoc/>
        public async override Task StartMigrationAsync(CancellationToken cancellationToken)
        {
            int i = 0;
            while (i == 0)
            {
                Thread.Sleep(5000);
            }

            if (Interlocked.CompareExchange(ref isMigrationWorkflowRunning, 0, 1) != 0)
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.TraceId,
                    "Migration workflow already running. Ignoring the request.");
            }

            this.childCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var childToken = this.childCancellationTokenSource.Token;

            await this.ThrowIfInvalidConfigForMigrationAsync(childToken);
            await this.InitializeIfRequiredAsync(childToken);
            this.isMigrationCompleted = await this.IsMigrationCompleted(childToken);
            this.StateProviderStateChangeCallback(this.isMigrationCompleted);
            if (this.isMigrationCompleted)
            {
                return;
            }

            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                "Starting Migration.");
            IMigrationPhaseWorkload workloadRunner = null;

            try
            {
                workloadRunner = await this.NextWorkloadRunnerAsync(MigrationPhase.None, childToken);

                PhaseResult currentResult = null;
                while (workloadRunner != null)
                {
                    currentResult = await workloadRunner.StartOrResumeMigrationAsync(childToken);
                    workloadRunner = await this.NextWorkloadRunnerAsync(currentResult, childToken);
                }

                if (currentResult != null)
                {
                    await this.CompleteMigrationAsync(currentResult, childToken);

                    ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.TraceId,
                        $"Migration successfully completed - {currentResult.ToString()}");
                }
            }
            catch (Exception e)
            {
                var currentPhase = workloadRunner != null ? workloadRunner.Phase : MigrationPhase.None;
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.TraceId,
                    $"Migration {currentPhase} Phase failed with error: {e}");

                throw e;
            }
        }

        /// <inheritdoc/>
        public override async Task AbortMigrationAsync(CancellationToken cancellationToken)
        {
            if (this.childCancellationTokenSource != null)
            {
                this.childCancellationTokenSource.Cancel();
            }

            await this.ServicePartitionClient.InvokeWithRetryAsync(
               async client =>
               {
                   return await client.HttpClient.PutAsync($"{KVSMigrationControllerName}/{ResumeWritesAPIEndpoint}", null);
               },
               cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<bool> AreActorCallsAllowedAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.isMigrationCompleted);
        }

        /// <inheritdoc/>
        public override IActorStateProvider GetMigrationActorStateProvider()
        {
            return this.migrationActorStateProvider;
        }

        /// <inheritdoc/>
        public override Task StartDowntimeAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        internal async Task<MigrationResult> GetResultAsync(CancellationToken cancellationToken)
        {
            var startSN = await this.GetStartSequenceNumberAsync(cancellationToken);
            var endSN = await this.GetStartSequenceNumberAsync(cancellationToken);
            using (var tx = this.Transaction)
            {
                var status = await ParseMigrationStateAsync(
                    () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                    tx,
                    MigrationCurrentStatus,
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);
                if (status == MigrationState.None)
                {
                    return new MigrationResult
                    {
                        CurrentPhase = MigrationPhase.None,
                        Status = MigrationState.None,
                        StartSeqNum = startSN,
                        EndSeqNum = endSN,
                    };
                }

                var result = new MigrationResult
                {
                    Status = status,
                    EndSeqNum = endSN,
                };

                result.CurrentPhase = await ParseMigrationPhaseAsync(
                    () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                    tx,
                    MigrationCurrentPhase,
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                result.StartDateTimeUTC = (await ParseDateTimeAsync(
                    () => this.MetaDataDictionary.GetAsync(
                    tx,
                    MigrationStartDateTimeUTC,
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId)).Value;

                result.EndDateTimeUTC = await ParseDateTimeAsync(
                    () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                    tx,
                    MigrationEndDateTimeUTC,
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                result.StartSeqNum = (await ParseLongAsync(
                    () => this.MetaDataDictionary.GetAsync(
                    tx,
                    MigrationStartSeqNum,
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId)).Value;

                result.LastAppliedSeqNum = await ParseLongAsync(
                    () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                    tx,
                    MigrationLastAppliedSeqNum,
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                result.NoOfKeysMigrated = await ParseLongAsync(
                    () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                    tx,
                    MigrationNoOfKeysMigrated,
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                var currentPhase = MigrationPhase.Copy;
                var phaseResults = new List<PhaseResult>();
                while (currentPhase <= result.CurrentPhase)
                {
                    var currentIteration = await ParseIntAsync(
                    () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                        tx,
                        Key(PhaseIterationCount, MigrationPhase.Catchup),
                        DefaultRCTimeout,
                        cancellationToken),
                    1,
                    this.TraceId);
                    for (int i = 1; i <= currentIteration; i++)
                    {
                        phaseResults.Add(await MigrationPhaseWorkloadBase.GetResultAsync(this.MetaDataDictionary, tx, currentPhase, i, this.TraceId, cancellationToken));
                    }

                    currentPhase++;
                }

                await tx.CommitAsync();
                result.PhaseResults = phaseResults.ToArray();

                return result;
            }
        }

        protected override Uri GetForwardServiceUri()
        {
            return this.MigrationSettings.SourceServiceUri;
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
            return ActorNameFormat.GetMigrationTargetEndpointName(this.ActorTypeInformation.ImplementationType);
        }

        private async Task<bool> IsMigrationCompleted(CancellationToken cancellationToken)
        {
            using (var tx = this.Transaction)
            {
                var status = await ParseMigrationStateAsync(
                    () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                    tx,
                    MigrationCurrentStatus,
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                await tx.CommitAsync();

                return status == MigrationState.Completed;
            }
        }

        private async Task CompleteMigrationAsync(PhaseResult result, CancellationToken cancellationToken)
        {
            using (var tx = this.Transaction)
            {
                await this.metadataDict.TryAddAsync(
                    tx,
                    MigrationEndDateTimeUTC,
                    DateTime.UtcNow.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await this.metadataDict.TryAddAsync(
                    tx,
                    MigrationEndSeqNum,
                    result.EndSeqNum.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await this.metadataDict.AddOrUpdateAsync(
                    tx,
                    MigrationCurrentStatus,
                    MigrationState.Completed.ToString(),
                    (_, __) => MigrationState.Completed.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await tx.CommitAsync();

                this.isMigrationCompleted = true;
                this.StateProviderStateChangeCallback(true);
            }
        }

        private async Task<long> GetEndSequenceNumberAsync(CancellationToken cancellationToken)
        {
            return (await ParseLongAsync(
                () => this.ServicePartitionClient.InvokeWithRetryAsync<string>(
                async client =>
                {
                    return await client.HttpClient.GetStringAsync($"{KVSMigrationControllerName}/{GetEndSNEndpoint}");
                },
                cancellationToken),
                this.TraceId)).Value;
        }

        private async Task<long> GetStartSequenceNumberAsync(CancellationToken cancellationToken)
        {
            return (await ParseLongAsync(
                () => this.ServicePartitionClient.InvokeWithRetryAsync<string>(
                async client =>
                {
                    return await client.HttpClient.GetStringAsync($"{KVSMigrationControllerName}/{GetStartSNEndpoint}");
                },
                cancellationToken),
                this.TraceId)).Value;
        }

        private async Task InvokeRejectWritesAsync(CancellationToken cancellationToken)
        {
            await this.ServicePartitionClient.InvokeWithRetryAsync(
                async client =>
                {
                    return await client.HttpClient.PutAsync($"{KVSMigrationControllerName}/{RejectWritesAPIEndpoint}", null);
                },
                cancellationToken);
        }

        private async Task<IMigrationPhaseWorkload> NextWorkloadRunnerAsync(PhaseResult currentResult, CancellationToken cancellationToken)
        {
            var endSN = await this.GetEndSequenceNumberAsync(cancellationToken);
            var delta = endSN - currentResult.EndSeqNum;
            if (currentResult.Phase == MigrationPhase.Catchup)
            {
                if (delta > this.MigrationSettings.DowntimeThreshold)
                {
                    return await this.NextWorkloadRunnerAsync(MigrationPhase.Catchup, cancellationToken);
                }

                await this.InvokeRejectWritesAsync(cancellationToken);
            }

            return await this.NextWorkloadRunnerAsync(currentResult.Phase + 1, cancellationToken);
        }

        private async Task<IMigrationPhaseWorkload> NextWorkloadRunnerAsync(MigrationPhase currentPhase, CancellationToken cancellationToken)
        {
            IMigrationPhaseWorkload migrationWorkload = null;
            using (var tx = this.Transaction)
            {
                if (currentPhase == MigrationPhase.None || currentPhase == MigrationPhase.Copy)
                {
                    migrationWorkload = new CopyPhaseWorkload(
                        this.StateProvider,
                        this.ServicePartitionClient,
                        this.StatefulServiceContext,
                        this.MigrationSettings,
                        null,
                        this.ActorTypeInformation,
                        this.TraceId);
                }
                else if (currentPhase == MigrationPhase.Catchup)
                {
                    var currentIteration = await ParseIntAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                        tx,
                        Key(PhaseIterationCount, MigrationPhase.Catchup),
                        "1",
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId);

                    var status = await ParseMigrationStateAsync(
                        () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                        tx,
                        Key(PhaseCurrentStatus, MigrationPhase.Catchup, currentIteration),
                        DefaultRCTimeout,
                        cancellationToken),
                        this.TraceId);
                    if (status == MigrationState.Completed)
                    {
                        currentIteration++;
                    }

                    migrationWorkload = new CatchupPhaseWorkload(
                        currentIteration,
                        this.StateProvider,
                        this.ServicePartitionClient,
                        this.StatefulServiceContext,
                        this.MigrationSettings,
                        null,
                        this.ActorTypeInformation,
                        this.TraceId);
                }
                else if (currentPhase == MigrationPhase.Downtime)
                {
                    migrationWorkload = new DowntimeWorkload(
                        this.StateProvider,
                        this.ServicePartitionClient,
                        this.StatefulServiceContext,
                        this.MigrationSettings,
                        null,
                        this.ActorTypeInformation,
                        this.TraceId);
                }

                await tx.CommitAsync();

                return migrationWorkload;
            }
        }

        private async Task InitializeIfRequiredAsync(CancellationToken cancellationToken)
        {
            this.metadataDict = await this.migrationActorStateProvider.GetMetadataDictionaryAsync();
            using (var tx = this.Transaction)
            {
                await this.MetaDataDictionary.TryAddAsync(
                    tx,
                    MigrationStartDateTimeUTC,
                    DateTime.UtcNow.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await this.MetaDataDictionary.TryAddAsync(
                    tx,
                    MigrationCurrentStatus,
                    MigrationState.InProgress.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await this.MetaDataDictionary.TryAddAsync(
                    tx,
                    MigrationCurrentPhase,
                    MigrationPhase.None.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await tx.CommitAsync();
            }
        }

        private async Task ThrowIfInvalidConfigForMigrationAsync(CancellationToken cancellationToken)
        {
            if (this.MigrationSettings.SourceServiceUri == null)
            {
                throw new ActorStateInvalidMigrationConfigException("KVSServiceName is not configured in Settings.xml");
            }

            var fabricClient = new FabricClient();
            var kvsServiceDescription = await fabricClient.ServiceManager.GetServiceDescriptionAsync(this.MigrationSettings.SourceServiceUri);
            if (kvsServiceDescription == null)
            {
                throw new ActorStateInvalidMigrationConfigException($"Could not find Service Description for {this.MigrationSettings.SourceServiceUri}.");
            }

            var kvsServicePartitionCount = this.GetServicePartitionCount(kvsServiceDescription);
            var rcServiceDescription = await fabricClient.ServiceManager.GetServiceDescriptionAsync(this.StatefulServiceContext.ServiceName);
            var rcServicePartitionCount = this.GetServicePartitionCount(rcServiceDescription);
            var isDisableTombstoneCleanup = await this.GetKVSDisableTombstoneCleanupSettingAsync(cancellationToken);

            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                "kvsServiceDescription.PartitionSchemeDescription.Scheme = {0}; kvsServicePartitionCount = {1}; rcServiceDescription.PartitionSchemeDescription.Scheme = {2}; rcServicePartitionCount = {3}; isDisableTombstoneCleanup = {4}",
                kvsServiceDescription.PartitionSchemeDescription.Scheme,
                kvsServicePartitionCount,
                rcServiceDescription.PartitionSchemeDescription.Scheme,
                rcServicePartitionCount,
                isDisableTombstoneCleanup);

            if (kvsServiceDescription.PartitionSchemeDescription.Scheme != rcServiceDescription.PartitionSchemeDescription.Scheme
                || kvsServicePartitionCount != rcServicePartitionCount
                || !isDisableTombstoneCleanup)
            {
                //// TODO: Emit telemetry

                throw new ActorStateInvalidMigrationConfigException($"kvsServiceDescription.PartitionSchemeDescription.Scheme = {kvsServiceDescription.PartitionSchemeDescription.Scheme}; " +
                    $"kvsServicePartitionCount = {kvsServicePartitionCount}; rcServiceDescription.PartitionSchemeDescription.Scheme = {rcServiceDescription.PartitionSchemeDescription.Scheme}; " +
                    $"rcServicePartitionCount = {rcServicePartitionCount}; isDisableTombstoneCleanup = {isDisableTombstoneCleanup}");
            }
        }

        private int GetServicePartitionCount(ServiceDescription serviceDescription)
        {
            switch (serviceDescription.PartitionSchemeDescription.Scheme)
            {
                case PartitionScheme.Singleton:
                    return 1;
                case PartitionScheme.UniformInt64Range:
                    return (serviceDescription.PartitionSchemeDescription as UniformInt64RangePartitionSchemeDescription).PartitionCount;
                case PartitionScheme.Named:
                    return (serviceDescription.PartitionSchemeDescription as NamedPartitionSchemeDescription).PartitionNames.Count;
                case PartitionScheme.Invalid:
                default:
                    return 0;
            }
        }

        private async Task<bool> GetKVSDisableTombstoneCleanupSettingAsync(CancellationToken cancellationToken)
        {
            var kvsDisableTCSString = await this.ServicePartitionClient.InvokeWithRetryAsync<string>(
                async client =>
                {
                    return await client.HttpClient.GetStringAsync($"{MigrationConstants.KVSMigrationControllerName}/{MigrationConstants.GetDisableTCSEndpoint}");
                },
                cancellationToken);

            return bool.Parse(kvsDisableTCSString);
        }
    }
}
