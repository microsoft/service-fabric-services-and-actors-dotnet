// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using static Microsoft.ServiceFabric.Actors.Migration.MigrationConstants;
    using static Microsoft.ServiceFabric.Actors.Migration.MigrationInput;
    using static Microsoft.ServiceFabric.Actors.Migration.MigrationResult;
    using static Microsoft.ServiceFabric.Actors.Migration.MigrationUtility;

    internal abstract class MigrationPhaseWorkloadBase : IMigrationPhaseWorkload
    {
        private static readonly string TraceType = typeof(MigrationPhaseWorkloadBase).ToString();
        private MigrationPhase migrationPhase;
        private IReliableDictionary2<string, string> metadataDict;
        private ServicePartitionClient<HttpCommunicationClient> servicePartitionClient;
        private StatefulServiceContext statefulServiceContext;
        private MigrationSettings migrationSettings;
        private KVStoRCMigrationActorStateProvider stateProvider;
        private StatefulServiceInitializationParameters initParams;
        private string traceId;
        private ActorTypeInformation actorTypeInformation;
        private int currentIteration;
        private int workerCount;

        public MigrationPhaseWorkloadBase(
            MigrationPhase migrationPhase,
            int currentIteration,
            int workerCount,
            KVStoRCMigrationActorStateProvider stateProvider,
            ServicePartitionClient<HttpCommunicationClient> servicePartitionClient,
            StatefulServiceContext statefulServiceContext,
            MigrationSettings migrationSettings,
            StatefulServiceInitializationParameters initParams,
            ActorTypeInformation actorTypeInfo,
            string traceId)
        {
            this.migrationPhase = migrationPhase;
            this.servicePartitionClient = servicePartitionClient;
            this.migrationSettings = migrationSettings;
            this.statefulServiceContext = statefulServiceContext;
            this.stateProvider = stateProvider;
            this.initParams = initParams;
            this.actorTypeInformation = actorTypeInfo;
            this.currentIteration = currentIteration;
            this.workerCount = workerCount;
            this.traceId = traceId;
            this.metadataDict = this.stateProvider.GetMetadataDictionaryAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public IReliableDictionary2<string, string> MetaDataDictionary { get => this.metadataDict; }

        public ServicePartitionClient<HttpCommunicationClient> ServicePartitionClient { get => this.servicePartitionClient; }

        public StatefulServiceContext StatefulServiceContext { get => this.statefulServiceContext; }

        public MigrationSettings MigrationSettings { get => this.migrationSettings; }

        public StatefulServiceInitializationParameters StatefulServiceInitializationParameters { get => this.initParams; }

        public KVStoRCMigrationActorStateProvider StateProvider { get => this.stateProvider; }

        public ActorTypeInformation ActorTypeInformation { get => this.actorTypeInformation; }

        public string TraceId { get => this.traceId; }

        public Data.ITransaction Transaction { get => this.stateProvider.GetStateManager().CreateTransaction(); }

        public MigrationPhase Phase { get => this.migrationPhase; }

        public async Task<MigrationResult> StartOrResumeMigrationAsync(CancellationToken cancellationToken)
        {
            MigrationInput input = null;

            try
            {
                input = await this.GetOrAddInputAsync(cancellationToken);
                if (input.Status == MigrationState.Completed)
                {
                    ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.traceId,
                        $"Phase already completed: /*DUMP input*/");

                    return await this.GetResultAsync(input, cancellationToken);
                }
                else
                {
                    ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.traceId,
                        $"Starting or resuming {this.migrationPhase} - {this.currentIteration} Phase - /*DUMP input*/");
                }

                var workers = this.CreateMigrationWorkers(input, cancellationToken);
                var tasks = new List<Task<WorkerResult>>();
                foreach (var worker in workers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (worker.Input.Status != MigrationState.Completed)
                    {
                        tasks.Add(worker.StartMigrationAsync(cancellationToken));
                    }
                }

                var results = await Task.WhenAll(tasks);
                var migrationResult = await this.AddOrUpdateResultAsync(input, results, cancellationToken);

                ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.traceId,
                        $"Completed Migration phase - /*DUMP result*/");

                return migrationResult;
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        this.traceId,
                        $"Migration phase failed with error: {ex} \n Input: /*DUMP input*/");

                throw ex;
            }
        }

        protected virtual async Task<long> GetEndSequenceNumberAsync(CancellationToken cancellationToken)
        {
            return await ParseLongAsync(
                () => this.servicePartitionClient.InvokeWithRetryAsync<string>(
                async client =>
                {
                    return await client.HttpClient.GetStringAsync($"{KVSMigrationControllerName}/{GetEndSNEndpoint}");
                },
                cancellationToken),
                this.TraceId);
        }

        protected virtual async Task<long> GetStartSequenceNumberAsync(CancellationToken cancellationToken)
        {
            long startSequenceNumber;
            using (var tx = this.Transaction)
            {
                startSequenceNumber = await ParseLongAsync(
                    () => this.MetaDataDictionary.GetAsync(
                        tx,
                        MigrationLastAppliedSeqNum,
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId);

                await tx.CommitAsync();
            }

            return startSequenceNumber;
        }

        protected virtual async Task<MigrationInput> GetOrAddInputAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var input = new MigrationInput();
            using (var tx = this.Transaction)
            {
                input.Phase = this.migrationPhase;

                input.StartDateTimeUTC = await ParseDateTimeAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                        tx,
                        Key(PhaseStartDateTimeUTC, this.migrationPhase, this.currentIteration),
                        DateTime.UtcNow.ToString(),
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId);

                input.EndDateTimeUTC = await ParseDateTimeOrGetDefaultAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                        tx,
                        Key(PhaseStartDateTimeUTC, this.migrationPhase, this.currentIteration),
                        default(DateTime).ToString(),
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId);

                input.Status = await ParseMigrationStateAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                        tx,
                        Key(PhaseCurrentStatus, this.migrationPhase, this.currentIteration),
                        MigrationState.InProgress.ToString(),
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId);

                input.StartSeqNum = await ParseLongAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                        tx,
                        Key(PhaseStartSeqNum, this.migrationPhase, this.currentIteration),
                        this.GetStartSequenceNumberAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult().ToString(),
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId);

                input.EndSeqNum = await ParseLongAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                        tx,
                        Key(PhaseEndSeqNum, this.migrationPhase, this.currentIteration),
                        this.GetEndSequenceNumberAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult().ToString(),
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId);

                input.LastAppliedSeqNum = await ParseLongAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                        tx,
                        Key(PhaseLastAppliedSeqNum, this.migrationPhase, this.currentIteration),
                        "-1",
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId);

                input.IterationCount = await ParseIntAsync(
                    () => this.MetaDataDictionary.AddOrUpdateAsync(
                        tx,
                        Key(PhaseIterationCount, this.migrationPhase),
                        this.currentIteration.ToString(),
                        (_, __) => this.currentIteration.ToString(),
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId);

                input.WorkerCount = await ParseIntAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                        tx,
                        Key(PhaseWorkerCount, this.migrationPhase),
                        this.workerCount.ToString(),
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId);

                long perWorker = (input.EndSeqNum - input.StartSeqNum + 1) / this.workerCount;
                var perWorkerStartSN = input.StartSeqNum;
                var perWorkerEndSN = input.StartSeqNum + perWorker;
                input.WorkerInputs = new List<WorkerInput>();
                for (int i = 1; i <= this.workerCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (i == this.workerCount)
                    {
                        perWorkerEndSN += (input.EndSeqNum - input.StartSeqNum + 1) % this.workerCount;
                    }

                    var workerInput = new WorkerInput
                    {
                        WorkerId = i,
                        Phase = this.migrationPhase,
                        Iteration = this.currentIteration,
                    };

                    input.StartDateTimeUTC = await ParseDateTimeAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                        tx,
                        Key(PhaseWorkerStartDateTimeUTC, this.migrationPhase, this.currentIteration, i),
                        DateTime.UtcNow.ToString(),
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId);

                    input.EndDateTimeUTC = await ParseDateTimeOrGetDefaultAsync(
                        () => this.MetaDataDictionary.GetOrAddAsync(
                            tx,
                            Key(PhaseWorkerStartDateTimeUTC, this.migrationPhase, this.currentIteration, i),
                            default(DateTime).ToString(),
                            DefaultRCTimeout,
                            cancellationToken),
                        this.TraceId);

                    input.Status = await ParseMigrationStateAsync(
                        () => this.MetaDataDictionary.GetOrAddAsync(
                            tx,
                            Key(PhaseWorkerCurrentStatus, this.migrationPhase, this.currentIteration, i),
                            MigrationState.InProgress.ToString(),
                            DefaultRCTimeout,
                            cancellationToken),
                        this.TraceId);

                    input.StartSeqNum = await ParseLongAsync(
                        () => this.MetaDataDictionary.GetOrAddAsync(
                            tx,
                            Key(PhaseWorkerStartSeqNum, this.migrationPhase, this.currentIteration, i),
                            perWorkerStartSN.ToString(),
                            DefaultRCTimeout,
                            cancellationToken),
                        this.TraceId);

                    input.EndSeqNum = await ParseLongAsync(
                        () => this.MetaDataDictionary.GetOrAddAsync(
                            tx,
                            Key(PhaseWorkerEndSeqNum, this.migrationPhase, this.currentIteration, i),
                            perWorkerEndSN.ToString(),
                            DefaultRCTimeout,
                            cancellationToken),
                        this.TraceId);

                    input.LastAppliedSeqNum = await ParseLongAsync(
                        () => this.MetaDataDictionary.GetOrAddAsync(
                            tx,
                            Key(PhaseWorkerLastAppliedSeqNum, this.migrationPhase, this.currentIteration, i),
                            "-1",
                            DefaultRCTimeout,
                            cancellationToken),
                        this.TraceId);

                    perWorkerStartSN = perWorkerEndSN + 1;
                    perWorkerEndSN = perWorkerStartSN + perWorker;
                }

                await tx.CommitAsync();
            }

            return input;
        }

        protected virtual List<MigrationWorker> CreateMigrationWorkers(MigrationInput input, CancellationToken cancellationToken)
        {
            var workers = new List<MigrationWorker>();

            foreach (var workerInput in input.WorkerInputs)
            {
                cancellationToken.ThrowIfCancellationRequested();

                workers.Add(new MigrationWorker(
                    this.StateProvider,
                    this.ActorTypeInformation,
                    this.ServicePartitionClient,
                    this.MigrationSettings,
                    workerInput,
                    this.TraceId));
            }

            return workers;
        }

        protected virtual async Task<MigrationResult> GetResultAsync(MigrationInput migrationInput, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = new MigrationResult();
            using (var tx = this.Transaction)
            {
                result.Phase = migrationInput.Phase;

                result.StartDateTimeUTC = await ParseDateTimeAsync(
                    () => this.MetaDataDictionary.GetAsync(
                    tx,
                    Key(PhaseStartDateTimeUTC, this.migrationPhase, this.currentIteration),
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                result.EndDateTimeUTC = await ParseDateTimeAsync(
                    () => this.MetaDataDictionary.GetAsync(
                    tx,
                    Key(PhaseEndDateTimeUTC, this.migrationPhase, this.currentIteration),
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                result.StartSeqNum = await ParseLongAsync(
                    () => this.MetaDataDictionary.GetAsync(
                    tx,
                    Key(PhaseStartSeqNum, this.migrationPhase, this.currentIteration),
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                result.EndSeqNum = await ParseLongAsync(
                    () => this.MetaDataDictionary.GetAsync(
                    tx,
                    Key(PhaseEndSeqNum, this.migrationPhase, this.currentIteration),
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                result.LastAppliedSeqNum = await ParseLongAsync(
                    () => this.MetaDataDictionary.GetAsync(
                    tx,
                    Key(PhaseLastAppliedSeqNum, this.migrationPhase, this.currentIteration),
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                result.NoOfKeysMigrated = await ParseLongAsync(
                    () => this.MetaDataDictionary.GetAsync(
                    tx,
                    Key(PhaseNoOfKeysMigrated, this.migrationPhase, this.currentIteration),
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                result.WorkerCount = await ParseIntAsync(
                    () => this.MetaDataDictionary.GetAsync(
                    tx,
                    Key(PhaseWorkerCount, this.migrationPhase, this.currentIteration),
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                result.IterationCount = await ParseIntAsync(
                    () => this.MetaDataDictionary.GetAsync(
                    tx,
                    Key(PhaseIterationCount, this.migrationPhase, this.currentIteration),
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                result.Status = await ParseMigrationStateAsync(
                    () => this.MetaDataDictionary.GetAsync(
                    tx,
                    Key(PhaseCurrentStatus, this.migrationPhase, this.currentIteration),
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                result.WorkerResults = new List<WorkerResult>();

                for (int i = 1; i <= result.WorkerCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var workerResult = new WorkerResult();
                    workerResult.StartDateTimeUTC = await ParseDateTimeAsync(
                        () => this.MetaDataDictionary.GetAsync(
                        tx,
                        Key(PhaseWorkerStartDateTimeUTC, this.migrationPhase, this.currentIteration, i),
                        DefaultRCTimeout,
                        cancellationToken),
                        this.TraceId);

                    workerResult.EndDateTimeUTC = await ParseDateTimeAsync(
                        () => this.MetaDataDictionary.GetAsync(
                        tx,
                        Key(PhaseWorkerEndDateTimeUTC, this.migrationPhase, this.currentIteration, i),
                        DefaultRCTimeout,
                        cancellationToken),
                        this.TraceId);

                    workerResult.StartSeqNum = await ParseLongAsync(
                        () => this.MetaDataDictionary.GetAsync(
                        tx,
                        Key(PhaseWorkerStartSeqNum, this.migrationPhase, this.currentIteration, i),
                        DefaultRCTimeout,
                        cancellationToken),
                        this.TraceId);

                    workerResult.EndSeqNum = await ParseLongAsync(
                        () => this.MetaDataDictionary.GetAsync(
                        tx,
                        Key(PhaseWorkerEndSeqNum, this.migrationPhase, this.currentIteration, i),
                        DefaultRCTimeout,
                        cancellationToken),
                        this.TraceId);

                    workerResult.LastAppliedSeqNum = await ParseLongAsync(
                        () => this.MetaDataDictionary.GetAsync(
                        tx,
                        Key(PhaseWorkerLastAppliedSeqNum, this.migrationPhase, this.currentIteration, i),
                        DefaultRCTimeout,
                        cancellationToken),
                        this.TraceId);

                    workerResult.NoOfKeysMigrated = await ParseLongAsync(
                        () => this.MetaDataDictionary.GetAsync(
                        tx,
                        Key(PhaseWorkerNoOfKeysMigrated, this.migrationPhase, this.currentIteration, i),
                        DefaultRCTimeout,
                        cancellationToken),
                        this.TraceId);

                    workerResult.Status = await ParseMigrationStateAsync(
                        () => this.MetaDataDictionary.GetAsync(
                        tx,
                        Key(PhaseWorkerCurrentStatus, this.migrationPhase, this.currentIteration, i),
                        DefaultRCTimeout,
                        cancellationToken),
                        this.TraceId);

                    workerResult.Phase = this.migrationPhase;
                    workerResult.Iteration = this.currentIteration;
                    workerResult.WorkerId = i;

                    result.WorkerResults.Add(workerResult);
                }

                await tx.CommitAsync();
            }

            return result;
        }

        protected virtual async Task<MigrationResult> AddOrUpdateResultAsync(MigrationInput migrationInput, WorkerResult[] workerResults, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            long keysMigrated = 0;
            var endTime = DateTime.UtcNow;
            using (var tx = this.Transaction)
            {
                endTime = await ParseDateTimeAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                    tx,
                    Key(PhaseEndDateTimeUTC, this.migrationPhase, this.currentIteration),
                    endTime.ToString(),
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                await this.MetaDataDictionary.AddOrUpdateAsync(
                    tx,
                    Key(PhaseLastAppliedSeqNum, this.migrationPhase, this.currentIteration),
                    migrationInput.EndSeqNum.ToString(),
                    (_, __) => migrationInput.EndSeqNum.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                keysMigrated = await ParseLongAsync(
                    () => this.MetaDataDictionary.AddOrUpdateAsync(
                    tx,
                    Key(PhaseNoOfKeysMigrated, this.migrationPhase, this.currentIteration),
                    _ =>
                    {
                        long newVal = 0L;
                        foreach (var workerResult in workerResults)
                        {
                            newVal += workerResult.NoOfKeysMigrated;
                        }

                        return newVal.ToString();
                    },
                    (k, v) =>
                    {
                        long newVal = ParseLong(v, this.TraceId);
                        foreach (var workerResult in workerResults)
                        {
                            newVal += workerResult.NoOfKeysMigrated;
                        }

                        return newVal.ToString();
                    },
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                await this.MetaDataDictionary.AddOrUpdateAsync(
                    tx,
                    Key(PhaseCurrentStatus, this.migrationPhase, this.currentIteration),
                    MigrationState.Completed.ToString(),
                    (_, __) => MigrationState.Completed.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await this.MetaDataDictionary.AddOrUpdateAsync(
                    tx,
                    MigrationLastAppliedSeqNum,
                    migrationInput.EndSeqNum.ToString(),
                    (_, __) => migrationInput.EndSeqNum.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await this.MetaDataDictionary.AddOrUpdateAsync(
                    tx,
                    MigrationNoOfKeysMigrated,
                    keysMigrated.ToString(),
                    (k, v) =>
                    {
                        var currentVal = ParseLong(v, this.TraceId);
                        var newVal = currentVal + keysMigrated;

                        return newVal.ToString();
                    },
                    DefaultRCTimeout,
                    cancellationToken);

                await tx.CommitAsync();
            }

            return new MigrationResult
            {
                EndDateTimeUTC = endTime,
                EndSeqNum = migrationInput.EndSeqNum,
                StartSeqNum = migrationInput.StartSeqNum,
                IterationCount = migrationInput.IterationCount,
                LastAppliedSeqNum = migrationInput.EndSeqNum,
                StartDateTimeUTC = migrationInput.StartDateTimeUTC,
                Status = MigrationState.Completed,
                WorkerCount = migrationInput.WorkerCount,
                Phase = migrationInput.Phase,
                WorkerResults = new List<WorkerResult>(workerResults),
            };
        }

        private void EmitTelemetryEvent(TimeSpan timeSpent, long keysMigrated, int workerCount, int iterationCount)
        {
            ActorTelemetry.KVSToRCMigrationPhaseEndEvent(
                MigrationUtility.GetPhaseEndTelemetryKey(this.migrationPhase),
                this.statefulServiceContext,
                this.migrationSettings.KVSActorServiceUri.OriginalString,
                timeSpent,
                keysMigrated,
                workerCount,
                iterationCount);
        }
    }
}
