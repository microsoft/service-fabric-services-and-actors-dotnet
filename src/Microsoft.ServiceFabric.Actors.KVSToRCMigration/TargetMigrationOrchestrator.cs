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
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration.Extensions;
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
        private static volatile int isReadyForMigrationOperations = 0;
        private volatile int isMigrationWorkflowRunning = 0;
        private MigrationPhase currentPhase;
        private KVStoRCMigrationActorStateProvider migrationActorStateProvider;
        private IReliableDictionary2<string, string> metadataDict;
        private ServicePartitionClient<HttpCommunicationClient> partitionClient;
        private volatile MigrationState currentMigrationState;
        private CancellationTokenSource childCancellationTokenSource;
        private PartitionHealthExceptionFilter exceptionFilter;
        private ActorStateProviderHelper stateProviderHelper;

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
            this.stateProviderHelper = (stateProvider as ReliableCollectionsActorStateProvider).GetActorStateProviderHelper();
            this.exceptionFilter = new PartitionHealthExceptionFilter(this.MigrationSettings);
        }

        internal Data.ITransaction Transaction { get => this.migrationActorStateProvider.GetStateManager().CreateTransaction(); }

        internal IReliableDictionary2<string, string> MetaDataDictionary { get => this.metadataDict; }

        internal ServicePartitionClient<HttpCommunicationClient> ServicePartitionClient
        {
            get
            {
                if (this.partitionClient == null)
                { //// TODO: Operation retry settings
                    var partitionInformation = this.GetInt64RangePartitionInformation();
                    this.partitionClient = new ServicePartitionClient<HttpCommunicationClient>(
                            new HttpCommunicationClientFactory(null, new List<Services.Communication.Client.IExceptionHandler>() { new HttpExceptionHandler() }),
                            this.MigrationSettings.SourceServiceUri,
                            new ServicePartitionKey(partitionInformation.LowKey),
                            TargetReplicaSelector.PrimaryReplica,
                            Constants.MigrationListenerName);
                }

                return this.partitionClient;
            }
        }

        internal KVStoRCMigrationActorStateProvider StateProvider { get => this.migrationActorStateProvider; }

        public async override Task StartMigrationAsync(bool isUserTriggered, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                  TraceType,
                  this.TraceId,
                  $"StartMigrationAsync isUserTriggered : {isUserTriggered}, MigrationMode : {this.MigrationSettings.MigrationMode}");

            try
            {
                if (!isUserTriggered)
                {
                    this.metadataDict = await this.migrationActorStateProvider.GetMetadataDictionaryAsync();
                    this.childCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    this.currentMigrationState = await this.GetCurrentMigrationStateAsync(this.childCancellationTokenSource.Token);
                    Interlocked.CompareExchange(ref isReadyForMigrationOperations, 1, 0);
                    this.childCancellationTokenSource.Token.Register(() =>
                    {
                        Interlocked.CompareExchange(ref isReadyForMigrationOperations, 0, 1);
                        Interlocked.CompareExchange(ref this.isMigrationWorkflowRunning, 0, 1);
                    });
                }
                else
                {
                    if (this.MigrationSettings.MigrationMode == MigrationMode.Auto)
                    {
                        throw new FabricException("MigrationMode is set to Auto. Manual migration starts are not allowed.");
                    }
                    else
                    {
                        this.ThrowIfNotReady();
                    }
                }

                var childToken = this.childCancellationTokenSource.Token;
                if (this.currentMigrationState == MigrationState.Aborted)
                {
                    await this.InvokeResumeWritesAsync(childToken);
                    return;
                }

                if (this.MigrationSettings.MigrationMode == MigrationMode.Auto)
                {
                    if (this.currentMigrationState == MigrationState.None)
                    {
                        await this.ValidateConfigForMigrationAsync(childToken);
                        await this.InitializeAsync(childToken);
                    }

                    await this.StartOrResumeMigrationAsync(childToken);
                }
                else
                {
                    if (isUserTriggered)
                    {
                        if (this.currentMigrationState == MigrationState.None)
                        {
                            await this.ValidateConfigForMigrationAsync(childToken);
                            await this.InitializeAsync(childToken);
                            await this.StartOrResumeMigrationAsync(childToken);
                        }
                        else
                        {
                            throw new FabricException("Migration is either in progress or already completed");
                        }
                    }
                    else
                    {
                        if (this.currentMigrationState == MigrationState.InProgress)
                        {
                            await this.StartOrResumeMigrationAsync(childToken);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.TraceId,
                    $"Migration {this.currentPhase} Phase failed with error: {ex}");
                MigrationTelemetry.MigrationFailureEvent(this.StatefulServiceContext, this.currentPhase.ToString(), ex.Message);
                //// TODO: Emit exception type in telemetry

                this.exceptionFilter.ReportPartitionHealthIfNeeded(ex, this.StateProvider.StatefulServicePartition, out var abortMigration, out var rethrow);
                if (abortMigration)
                {
                    await this.AbortMigrationAsync(userTriggered: false, this.GetToken());
                }

                if (rethrow)
                {
                    throw ex;
                }
            }
            finally
            {
                ActorTrace.Source.WriteInfoWithId(
                  TraceType,
                  this.TraceId,
                  $"StartMigrationAsync Completed - isUserTriggered : {isUserTriggered}, MigrationMode : {this.MigrationSettings.MigrationMode}");

                await this.InvokeCompletionCallback(this.AreActorCallsAllowed(), this.GetToken());
            }
        }

        public async override Task AbortMigrationAsync(bool userTriggered, CancellationToken cancellationToken)
        {
            this.ThrowIfNotReady();

            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Aborting Migration. UserTriggered : {userTriggered}");
            MigrationTelemetry.MigrationAbortEvent(this.StatefulServiceContext, userTriggered);

            if (userTriggered)
            {
                if (this.childCancellationTokenSource != null)
                {
                    this.childCancellationTokenSource.Cancel();
                }
            }

            await this.stateProviderHelper.ExecuteWithRetriesAsync(
                async () =>
                {
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
                    }
                },
                "TargetMigrationOrchestrator.AbortMigrationAsync",
                cancellationToken);
            this.currentMigrationState = MigrationState.Aborted;

            await this.InvokeResumeWritesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override bool AreActorCallsAllowed()
        {
            this.ThrowIfNotReady();
            return this.currentMigrationState == MigrationState.Completed;
        }

        /// <inheritdoc/>
        public override IActorStateProvider GetMigrationActorStateProvider()
        {
            return this.migrationActorStateProvider;
        }

        /// <inheritdoc/>
        public override async Task StartDowntimeAsync(bool userTriggered, CancellationToken cancellationToken)
        {
            this.ThrowIfNotReady();
            await this.stateProviderHelper.ExecuteWithRetriesAsync(
                async () =>
                {
                    using (var tx = this.Transaction)
                    {
                        await this.metadataDict.TryAddAsync(
                        tx,
                        IsDowntimeInvoked,
                        true.ToString(),
                        DefaultRCTimeout,
                        cancellationToken);

                        await tx.CommitAsync();
                    }
                },
                "TargetMigrationOrchestrator.StartDowntimeAsync",
                cancellationToken);
        }

        public override bool IsActorCallToBeForwarded()
        {
            this.ThrowIfNotReady();

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
            this.ThrowIfNotReady();

            ActorTrace.Source.WriteNoiseWithId(
                TraceType,
                this.TraceId,
                $"Getting migration result.");

            var startSN = await this.GetStartSequenceNumberAsync(cancellationToken);
            var endSN = await this.GetEndSequenceNumberAsync(cancellationToken);
            var status = await this.GetCurrentMigrationStateAsync(cancellationToken);
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

            result.CurrentPhase = await this.GetCurrentMigrationPhaseAsync(cancellationToken);

            result.StartDateTimeUTC = await ParseDateTimeAsync(
                () => GetValueOrDefaultAsync(this.stateProviderHelper, () => this.Transaction, this.MetaDataDictionary, MigrationStartDateTimeUTC, cancellationToken),
                this.TraceId);

            result.EndDateTimeUTC = await ParseDateTimeAsync(
                () => GetValueOrDefaultAsync(this.stateProviderHelper, () => this.Transaction, this.MetaDataDictionary, MigrationEndDateTimeUTC, cancellationToken),
                this.TraceId);

            result.StartSeqNum = await ParseLongAsync(
                () => GetValueOrDefaultAsync(this.stateProviderHelper, () => this.Transaction, this.MetaDataDictionary, MigrationStartSeqNum, cancellationToken),
                this.TraceId);

            result.LastAppliedSeqNum = await ParseLongAsync(
                () => GetValueOrDefaultAsync(this.stateProviderHelper, () => this.Transaction, this.MetaDataDictionary, MigrationLastAppliedSeqNum, cancellationToken),
                this.TraceId);

            result.NoOfKeysMigrated = await ParseLongAsync(
                () => GetValueOrDefaultAsync(this.stateProviderHelper, () => this.Transaction, this.MetaDataDictionary, MigrationNoOfKeysMigrated, cancellationToken),
                this.TraceId);

            var currentPhase = MigrationPhase.Copy;
            var phaseResults = new List<PhaseResult>();
            while (currentPhase <= result.CurrentPhase)
            {
                var currentIteration = await ParseIntAsync(
                    () => GetValueOrDefaultAsync(this.stateProviderHelper, () => this.Transaction, this.MetaDataDictionary, Key(PhaseIterationCount, currentPhase), cancellationToken),
                    0,
                    this.TraceId);
                for (int i = 1; i <= currentIteration; i++)
                {
                    phaseResults.Add(await MigrationPhaseWorkloadBase.GetResultAsync(
                        this.stateProviderHelper,
                        () => this.Transaction,
                        this.MetaDataDictionary,
                        currentPhase,
                        i,
                        this.TraceId,
                        cancellationToken));
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

        private CancellationToken GetToken()
        {
            var token = CancellationToken.None;
            if (this.childCancellationTokenSource != null && !this.childCancellationTokenSource.IsCancellationRequested)
            {
                var token1 = this.childCancellationTokenSource.Token;
                if (!token1.IsCancellationRequested)
                {
                    return token1;
                }
            }

            return token;
        }

        private async Task StartOrResumeMigrationAsync(CancellationToken cancellationToken)
        {
            if (Interlocked.CompareExchange(ref this.isMigrationWorkflowRunning, 1, 0) != 0)
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.TraceId,
                    "Migration workflow already running. Ignoring the request.");

                return;
            }

            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                "Starting or resuming migration Migration.");
            MigrationTelemetry.MigrationStartEvent(this.StatefulServiceContext, this.MigrationSettings.ToString());

            var migrationCurrentPhase = await this.GetCurrentMigrationPhaseAsync(cancellationToken);
            var workloadRunner = await this.NextWorkloadRunnerAsync(migrationCurrentPhase, cancellationToken);

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

        private async Task<MigrationPhase> GetCurrentMigrationPhaseAsync(CancellationToken cancellationToken)
        {
            return await ParseMigrationPhaseAsync(
                () => GetValueOrDefaultAsync(this.stateProviderHelper, () => this.Transaction, this.MetaDataDictionary, MigrationCurrentPhase, cancellationToken),
                this.TraceId);
        }

        private async Task<MigrationState> GetCurrentMigrationStateAsync(CancellationToken cancellationToken)
        {
            return await ParseMigrationStateAsync(
                () => GetValueOrDefaultAsync(this.stateProviderHelper, () => this.Transaction, this.MetaDataDictionary, MigrationCurrentStatus, cancellationToken),
                this.TraceId);
        }

        private async Task CompleteMigrationAsync(PhaseResult result, CancellationToken cancellationToken)
        {
            await this.stateProviderHelper.ExecuteWithRetriesAsync(
                async () =>
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

                        await this.metadataDict.AddOrUpdateAsync(
                            tx,
                            MigrationCurrentPhase,
                            MigrationPhase.Completed.ToString(),
                            (_, __) => MigrationPhase.Completed.ToString(),
                            DefaultRCTimeout,
                            cancellationToken);

                        await tx.CommitAsync();
                    }
                },
                "TargetMigrationOrchestrator.CompleteMigrationAsync",
                cancellationToken);

            this.currentMigrationState = MigrationState.Completed;
            Interlocked.CompareExchange(ref this.isMigrationWorkflowRunning, 0, 1);
        }

        private async Task<long> GetEndSequenceNumberAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteNoiseWithId(
                TraceType,
                this.TraceId,
                $"Getting End Seq num.");
            var response = await this.ServicePartitionClient.InvokeWebRequestWithRetryAsync(
                async client =>
                {
                    return await client.HttpClient.GetAsync($"{KVSMigrationControllerName}/{GetEndSNEndpoint}");
                },
                "GetEndSequenceNumberAsync",
                cancellationToken);
            return (await ParseLongAsync(async () => await response.Content.ReadAsStringAsync(), this.TraceId)).Value;
        }

        private async Task<long> GetStartSequenceNumberAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Gettting start seq num.");

            var response = await this.ServicePartitionClient.InvokeWebRequestWithRetryAsync(
                async client =>
                {
                    return await client.HttpClient.GetAsync($"{KVSMigrationControllerName}/{GetStartSNEndpoint}");
                },
                "GetStartSequenceNumberAsync",
                cancellationToken);
            return (await ParseLongAsync(async () => await response.Content.ReadAsStringAsync(), this.TraceId)).Value;
        }

        private async Task InvokeRejectWritesAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Invoking reject writes in source migration service.");

            var response = await this.ServicePartitionClient.InvokeWebRequestWithRetryAsync(
                async client =>
                {
                    return await client.HttpClient.PutAsync($"{KVSMigrationControllerName}/{StartDowntimeEndpoint}", null);
                },
                "InvokeRejectWritesAsync",
                cancellationToken);
        }

        private async Task InvokeResumeWritesAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Invoking resume writes in source migration service.");

            var response = await this.ServicePartitionClient.InvokeWebRequestWithRetryAsync(
               async client =>
               {
                   return await client.HttpClient.PutAsync($"{KVSMigrationControllerName}/{AbortMigrationEndpoint}", null);
               },
               "InvokeResumeWritesAsync",
               cancellationToken);
        }

        private async Task<IMigrationPhaseWorkload> NextWorkloadRunnerAsync(PhaseResult currentResult, CancellationToken cancellationToken)
        {
            var endSN = await this.GetEndSequenceNumberAsync(cancellationToken);
            var delta = endSN - currentResult.EndSeqNum;
            var isDowntimeInvoked = false;

            if (currentResult.Phase == MigrationPhase.Catchup)
            {
                if (!this.IsAutoStartMigration())
                {
                    isDowntimeInvoked = await ParseBoolAsync(
                        () => GetValueOrDefaultAsync(this.stateProviderHelper, () => this.Transaction, this.MetaDataDictionary, IsDowntimeInvoked, cancellationToken),
                        this.TraceId);
                }
                else if (delta < this.MigrationSettings.DowntimeThreshold)
                {
                    isDowntimeInvoked = true;
                }

                if (isDowntimeInvoked)
                {
                    await this.InvokeRejectWritesAsync(cancellationToken);
                    return await this.NextWorkloadRunnerAsync(MigrationPhase.Downtime, cancellationToken);
                }
                else
                {
                    return await this.NextWorkloadRunnerAsync(MigrationPhase.Catchup, cancellationToken);
                }
            }

            return await this.NextWorkloadRunnerAsync(currentResult.Phase + 1, cancellationToken);
        }

        private async Task<IMigrationPhaseWorkload> NextWorkloadRunnerAsync(MigrationPhase currentPhase, CancellationToken cancellationToken)
        {
            IMigrationPhaseWorkload migrationWorkload = null;
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
                    () => this.stateProviderHelper.ExecuteWithRetriesAsync(
                        async () =>
                        {
                            using (var tx = this.Transaction)
                            {
                                var res = await this.MetaDataDictionary.GetOrAddAsync(
                                            tx,
                                            Key(PhaseIterationCount, MigrationPhase.Catchup),
                                            "1",
                                            DefaultRCTimeout,
                                            cancellationToken);
                                await tx.CommitAsync();

                                return res;
                            }
                        },
                        $"TargetMigrationOrchestrator.NextWorkloadRunnerAsync.{Key(PhaseIterationCount, MigrationPhase.Catchup)}",
                        cancellationToken),
                    this.TraceId);

                var status = await ParseMigrationStateAsync(
                    () => GetValueOrDefaultAsync(this.stateProviderHelper, () => this.Transaction, this.MetaDataDictionary, Key(PhaseCurrentStatus, MigrationPhase.Catchup, currentIteration), cancellationToken),
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

            return migrationWorkload;
        }

        private async Task InitializeAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Initializing Migration.");

            await this.stateProviderHelper.ExecuteWithRetriesAsync(
                async () =>
                {
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
                },
                "TargetMigrationOrchestrator.InitializeAsync",
                cancellationToken);

            this.currentMigrationState = MigrationState.InProgress;
        }

        private async Task ValidateConfigForMigrationAsync(CancellationToken cancellationToken)
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

            var response = await this.ServicePartitionClient.InvokeWebRequestWithRetryAsync(
                async client =>
                {
                    return await client.HttpClient.GetAsync($"{KVSMigrationControllerName}/{GetDisableTCSEndpoint}");
                },
                "GetDisableTCSEndpoint",
                cancellationToken);
            return (await ParseBoolAsync(async () => await response.Content.ReadAsStringAsync(), this.TraceId));
        }

        private void ThrowIfNotReady()
        {
            if (isReadyForMigrationOperations != 1)
            {
                throw new FabricException("MigrationFramework not initialized. Retry the request");
            }
        }
    }
}
