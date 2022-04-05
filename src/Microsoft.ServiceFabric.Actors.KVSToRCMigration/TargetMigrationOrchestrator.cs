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
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Migration.Exceptions;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime.Migration;
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
        private MigrationPhase currentPhase;
        private KVStoRCMigrationActorStateProvider migrationActorStateProvider;
        private IReliableDictionary2<string, string> metadataDict;
        private ServicePartitionClient<HttpCommunicationClient> partitionClient;
        private volatile MigrationState currentMigrationState;
        private CancellationTokenSource childCancellationTokenSource;
        private Task migrationWorkflowTask;
        private volatile bool downtimeAllowed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetMigrationOrchestrator"/> class.
        /// </summary>
        /// <param name="stateProvider">KVS actor state provider.</param>
        /// <param name="actorTypeInfo">The type information of the Actor.</param>
        /// <param name="serviceContext">Service context the actor service is operating under.</param>
        /// <param name="migrationSettings">Migration settings.</param>
        public TargetMigrationOrchestrator(IActorStateProvider stateProvider, ActorTypeInformation actorTypeInfo, StatefulServiceContext serviceContext, Actors.Runtime.Migration.MigrationSettings migrationSettings)
            : base(serviceContext, actorTypeInfo, migrationSettings)
        {
            if (stateProvider.GetType() != typeof(ReliableCollectionsActorStateProvider))
            {
                var errorMsg = $"{stateProvider.GetType()} not a valid state provider type for source of migration. {typeof(ReliableCollectionsActorStateProvider)} is the valid type.";
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.TraceId,
                    errorMsg);

                throw new InvalidMigrationStateProviderException(errorMsg);
            }

            this.currentPhase = MigrationPhase.None;
            this.currentMigrationState = MigrationState.None;
            this.migrationActorStateProvider = new KVStoRCMigrationActorStateProvider(stateProvider as ReliableCollectionsActorStateProvider);
            this.downtimeAllowed = this.MigrationSettings.MigrationMode == Runtime.Migration.MigrationMode.Auto;
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
            if (Interlocked.CompareExchange(ref isMigrationWorkflowRunning, 1, 0) != 0)
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.TraceId,
                    "Migration workflow already running. Ignoring the request.");

                return;
            }

            this.childCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var childToken = this.childCancellationTokenSource.Token;

            await this.ThrowIfInvalidConfigForMigrationAsync(childToken);
            this.metadataDict = await this.migrationActorStateProvider.GetMetadataDictionaryAsync();
            await this.InitializeIfRequiredAsync(childToken);
            this.currentMigrationState = await this.GetCurrentMigrationStateAsync(childToken);
            if (this.currentMigrationState == MigrationState.Completed || this.currentMigrationState == MigrationState.Aborted)
            {
                ActorTrace.Source.WriteInfoWithId(
                    TraceType,
                    this.TraceId,
                    "Migration workflow already completed or aborted.");
                this.currentPhase = MigrationPhase.Completed;

                if (this.currentMigrationState == MigrationState.Aborted)
                {
                    // If Invoke resume writes failed before replica failed over, then we need to try resuming writes again.
                    await this.InvokeResumeWritesAsync(childToken);
                }

                return;
            }

            this.migrationWorkflowTask = Task.Run(() => this.StartOrResumeMigrationAsync(childToken), CancellationToken.None);

            return;
        }

        public override async Task<bool> TryResumeMigrationAsync(CancellationToken cancellationToken)
        {
            this.metadataDict = await this.migrationActorStateProvider.GetMetadataDictionaryAsync();
            var currentState = await this.GetCurrentMigrationStateAsync(cancellationToken);
            if (currentState == MigrationState.InProgress)
            {
                ActorTrace.Source.WriteInfoWithId(
                    TraceType,
                    this.TraceId,
                    "Resuming migration.");

                if (Interlocked.CompareExchange(ref isMigrationWorkflowRunning, 0, 1) != 0)
                {
                    ActorTrace.Source.WriteWarningWithId(
                        TraceType,
                        this.TraceId,
                        "Migration workflow already running. Ignoring the request.");

                    return false;
                }

                this.currentMigrationState = currentState;
                this.childCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var childToken = this.childCancellationTokenSource.Token;

                this.migrationWorkflowTask = Task.Run(() => this.StartOrResumeMigrationAsync(childToken), CancellationToken.None);

                return true;
            }

            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                "Migration workflow has not previously started, hence ignoring Resume migration call.");

            return false;
        }

        /// <inheritdoc/>
        public override async Task AbortMigrationAsync(CancellationToken cancellationToken)
        {
            await this.AbortMigrationAsync(true, cancellationToken);

            // If user triggered abort, then we need to cancel the migration workflow.
            if (this.childCancellationTokenSource != null)
            {
                this.childCancellationTokenSource.Cancel();
            }

            try
            {
                await this.migrationWorkflowTask;
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.TraceId,
                    $"Migration workflow Cancellation encountered exception. {ex}");
            }
        }

        /// <inheritdoc/>
        public override bool AreActorCallsAllowed()
        {
            return this.currentMigrationState == MigrationState.Completed;
        }

        /// <inheritdoc/>
        public override IActorStateProvider GetMigrationActorStateProvider()
        {
            return this.migrationActorStateProvider;
        }

        /// <inheritdoc/>
        public override Task StartDowntimeAsync(CancellationToken cancellationToken)
        {
            this.downtimeAllowed = true;

            return Task.CompletedTask;
        }

        public override bool IsActorCallToBeForwarded()
        {
            // No reason to forward the call in downtime phase as the KVS service also cannot service the request.
            return (this.currentPhase < MigrationPhase.Downtime || this.currentMigrationState == MigrationState.Aborted) && this.MigrationSettings.SourceServiceUri != null;
        }

        public override void ThrowIfActorCallsDisallowed()
        {
            if (this.AreActorCallsAllowed())
            {
                return;
            }

            var errorMsg = $"Actor calls are not allowed on the service.";
            if (this.MigrationSettings.SourceServiceUri == null)
            {
                errorMsg += $" Configure SourceServiceUri in {this.MigrationSettings.MigrationConfigSectionName} section of settings file to forward the request.";
            }

            throw new ActorCallsDisallowedException(errorMsg);
        }

        internal async Task<MigrationResult> GetResultAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Getting migration result.");

            var startSN = await this.GetStartSequenceNumberAsync(cancellationToken);
            var endSN = await this.GetEndSequenceNumberAsync(cancellationToken);
            var status = await ParseMigrationStateAsync(
                () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                () => this.Transaction,
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
                () => this.Transaction,
                MigrationCurrentPhase,
                DefaultRCTimeout,
                cancellationToken),
                this.TraceId);

            result.StartDateTimeUTC = (await ParseDateTimeAsync(
                () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                () => this.Transaction,
                MigrationStartDateTimeUTC,
                DefaultRCTimeout,
                cancellationToken),
                this.TraceId));

            result.EndDateTimeUTC = await ParseDateTimeAsync(
                () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                () => this.Transaction,
                MigrationEndDateTimeUTC,
                DefaultRCTimeout,
                cancellationToken),
                this.TraceId);

            result.StartSeqNum = (await ParseLongAsync(
                () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                () => this.Transaction,
                MigrationStartSeqNum,
                DefaultRCTimeout,
                cancellationToken),
                this.TraceId));

            result.LastAppliedSeqNum = await ParseLongAsync(
                () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                () => this.Transaction,
                MigrationLastAppliedSeqNum,
                DefaultRCTimeout,
                cancellationToken),
                this.TraceId);

            result.NoOfKeysMigrated = await ParseLongAsync(
                () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                () => this.Transaction,
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
                    () => this.Transaction,
                    Key(PhaseIterationCount, currentPhase),
                    DefaultRCTimeout,
                    cancellationToken),
                0,
                this.TraceId);
                for (int i = 1; i <= currentIteration; i++)
                {
                    phaseResults.Add(await MigrationPhaseWorkloadBase.GetResultAsync(this.MetaDataDictionary, () => this.Transaction, currentPhase, i, this.TraceId, cancellationToken));
                }

                currentPhase++;
            }

            result.PhaseResults = phaseResults.ToArray();

            return result;
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

        private async Task StartOrResumeMigrationAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                "Starting or resuming migration Migration.");
            MigrationTelemetry.MigrationStartEvent(this.StatefulServiceContext, this.MigrationSettings.ToString());

            IMigrationPhaseWorkload workloadRunner = null;

            try
            {
                workloadRunner = await this.NextWorkloadRunnerAsync(MigrationPhase.None, cancellationToken);

                PhaseResult currentResult = null;
                while (workloadRunner != null)
                {
                    this.currentPhase = workloadRunner.Phase;
                    currentResult = await workloadRunner.StartOrResumeMigrationAsync(cancellationToken);
                    workloadRunner = await this.NextWorkloadRunnerAsync(currentResult, cancellationToken);
                }

                if (currentResult != null)
                {
                    await this.CompleteMigrationAsync(currentResult, cancellationToken);
                    this.currentPhase = MigrationPhase.Completed;

                    ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.TraceId,
                        $"Migration successfully completed - {currentResult.ToString()}");
                    MigrationTelemetry.MigrationEndEvent(this.StatefulServiceContext, currentResult.ToString());
                }
            }
            catch (Exception e)
            {
                var currentPhase = workloadRunner != null ? workloadRunner.Phase : MigrationPhase.None;
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.TraceId,
                    $"Migration {currentPhase} Phase failed with error: {e}");
                MigrationTelemetry.MigrationFailureEvent(this.StatefulServiceContext, currentPhase.ToString(), e.Message);

                //// TODO: set partition health with permanent health message
                await this.AbortMigrationAsync(false, cancellationToken);

                throw e;
            }

            await this.InvokeCompletionCallback(this.AreActorCallsAllowed(), cancellationToken);
        }

        private async Task<MigrationState> GetCurrentMigrationStateAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Getting current migration state.");

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

                return status;
            }
        }

        private async Task CompleteMigrationAsync(PhaseResult result, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Completing Migration.");

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

                await this.metadataDict.AddOrUpdateAsync(
                    tx,
                    MigrationCurrentPhase,
                    MigrationPhase.Completed.ToString(),
                    (_, __) => MigrationPhase.Completed.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await tx.CommitAsync();

                this.currentMigrationState = MigrationState.Completed;
            }
        }

        private async Task<long> GetEndSequenceNumberAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Getting End Seq num.");

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
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Gettting start seq num.");

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
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Invoking reject writes in source migration service.");

            await this.ServicePartitionClient.InvokeWithRetryAsync(
                async client =>
                {
                    return await client.HttpClient.PutAsync($"{KVSMigrationControllerName}/{StartDowntimeEndpoint}", null);
                },
                cancellationToken);
        }

        private async Task InvokeResumeWritesAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Invoking resume writes in source migration service.");

            await this.ServicePartitionClient.InvokeWithRetryAsync(
               async client =>
               {
                   return await client.HttpClient.PutAsync($"{KVSMigrationControllerName}/{AbortMigrationEndpoint}", null);
               },
               cancellationToken);
        }

        private async Task<IMigrationPhaseWorkload> NextWorkloadRunnerAsync(PhaseResult currentResult, CancellationToken cancellationToken)
        {
            var endSN = await this.GetEndSequenceNumberAsync(cancellationToken);
            var delta = endSN - currentResult.EndSeqNum;
            if (currentResult.Phase == MigrationPhase.Catchup)
            {
                if (delta < this.MigrationSettings.DowntimeThreshold
                    && this.downtimeAllowed == true)
                {
                    await this.InvokeRejectWritesAsync(cancellationToken);
                }
                else
                {
                    // Manual downtime. Invoke Catchup iteration.
                    return await this.NextWorkloadRunnerAsync(MigrationPhase.Catchup, cancellationToken);
                }
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
                        this.ActorTypeInformation,
                        this.TraceId);
                }
                else if (currentPhase == MigrationPhase.DataValidation)
                {
                    migrationWorkload = new DataValidationPhaseWorkload(
                        this.StateProvider,
                        this.ServicePartitionClient,
                        this.StatefulServiceContext,
                        this.MigrationSettings,
                        this.ActorTypeInformation,
                        this.TraceId);
                }

                await tx.CommitAsync();

                return migrationWorkload;
            }
        }

        private async Task InitializeIfRequiredAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Initializing Migration.");

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
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Validating Migration config.");

            var migrationConfigSectionName = this.MigrationSettings.MigrationConfigSectionName;

            this.MigrationSettings.Validate();
            var fabricClient = new FabricClient();
            var kvsServiceDescription = await fabricClient.ServiceManager.GetServiceDescriptionAsync(this.MigrationSettings.SourceServiceUri);
            if (kvsServiceDescription == null)
            {
                var errorMsg = $"Unable to load service description for migration service name - {this.MigrationSettings.SourceServiceUri}.";
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.TraceId,
                    errorMsg);

                throw new InvalidMigrationConfigException(errorMsg);
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

            if (kvsServiceDescription.PartitionSchemeDescription.Scheme != rcServiceDescription.PartitionSchemeDescription.Scheme)
            {
                var errorMsg = $"Source migration service({this.MigrationSettings.SourceServiceUri}) partition scheme({kvsServiceDescription.PartitionSchemeDescription.Scheme}) does not match with target migration service({this.MigrationSettings.TargetServiceUri}) partition scheme({rcServiceDescription.PartitionSchemeDescription.Scheme})";
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.TraceId,
                    errorMsg);

                throw new InvalidMigrationConfigException(errorMsg);
            }

            if (kvsServicePartitionCount != rcServicePartitionCount)
            {
                var errorMsg = $"Source migration service({this.MigrationSettings.SourceServiceUri}) partition count({kvsServicePartitionCount}) does not match with target migration service({this.MigrationSettings.TargetServiceUri}) partition count({rcServicePartitionCount})";
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.TraceId,
                    errorMsg);

                throw new InvalidMigrationConfigException(errorMsg);
            }

            if (!isDisableTombstoneCleanup)
            {
                var errorMsg = $"DisableTombstoneCleanup is not disabled in source migration service({this.MigrationSettings.SourceServiceUri})";
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.TraceId,
                    errorMsg);

                throw new InvalidMigrationConfigException(errorMsg);
            }
            //// TODO: Emit telemetry
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
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Getting tombstone cleanup setting");

            var kvsDisableTCSString = await this.ServicePartitionClient.InvokeWithRetryAsync<string>(
                async client =>
                {
                    return await client.HttpClient.GetStringAsync($"{MigrationConstants.KVSMigrationControllerName}/{MigrationConstants.GetDisableTCSEndpoint}");
                },
                cancellationToken);

            return bool.Parse(kvsDisableTCSString);
        }

        private async Task AbortMigrationAsync(bool userTriggered, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Aborting Migration. UserTriggered : {userTriggered}");
            MigrationTelemetry.MigrationAbortEvent(this.StatefulServiceContext, userTriggered);

            using (var tx = this.Transaction)
            {
                await this.metadataDict.TryAddAsync(
                    tx,
                    MigrationEndDateTimeUTC,
                    DateTime.UtcNow.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await this.metadataDict.AddOrUpdateAsync(
                    tx,
                    MigrationCurrentStatus,
                    MigrationState.Aborted.ToString(),
                    (_, __) => MigrationState.Aborted.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await tx.CommitAsync();

                this.currentMigrationState = MigrationState.Aborted;
            }

            await this.InvokeResumeWritesAsync(cancellationToken);

            await this.InvokeCompletionCallback(this.AreActorCallsAllowed(), cancellationToken);
        }
    }
}
