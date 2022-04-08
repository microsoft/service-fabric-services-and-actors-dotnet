// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime.Migration;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationConstants;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationUtility;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.PhaseInput;
    using static Microsoft.ServiceFabric.Actors.Migration.PhaseResult;

    internal abstract class MigrationPhaseWorkloadBase : IMigrationPhaseWorkload
    {
        private static readonly string TraceType = typeof(MigrationPhaseWorkloadBase).Name;
        private MigrationPhase migrationPhase;
        private IReliableDictionary2<string, string> metadataDict;
        private ServicePartitionClient<HttpCommunicationClient> servicePartitionClient;
        private StatefulServiceContext statefulServiceContext;
        private MigrationSettings migrationSettings;
        private KVStoRCMigrationActorStateProvider stateProvider;
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
            ActorTypeInformation actorTypeInfo,
            string traceId)
        {
            this.migrationPhase = migrationPhase;
            this.servicePartitionClient = servicePartitionClient;
            this.migrationSettings = migrationSettings;
            this.statefulServiceContext = statefulServiceContext;
            this.stateProvider = stateProvider;
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

        public KVStoRCMigrationActorStateProvider StateProvider { get => this.stateProvider; }

        public ActorTypeInformation ActorTypeInformation { get => this.actorTypeInformation; }

        public string TraceId { get => this.traceId; }

        public Data.ITransaction Transaction { get => this.stateProvider.GetStateManager().CreateTransaction(); }

        public MigrationPhase Phase { get => this.migrationPhase; }

        public static async Task<PhaseResult> GetResultAsync(
            IReliableDictionary2<string, string> metadataDict,
            Func<Data.ITransaction> txFactory,
            MigrationPhase migrationPhase,
            int currentIteration,
            string traceId,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var status = await ParseMigrationStateAsync(
                () => metadataDict.GetValueOrDefaultAsync(
                txFactory,
                Key(PhaseCurrentStatus, migrationPhase, currentIteration),
                DefaultRCTimeout,
                cancellationToken),
                traceId);

            if (status == MigrationState.None)
            {
                return new PhaseResult
                {
                    Phase = migrationPhase,
                    Iteration = currentIteration,
                    Status = MigrationState.None,
                };
            }

            var result = new PhaseResult();
            result.Phase = migrationPhase;
            result.Iteration = currentIteration;
            result.Status = status;

            result.StartDateTimeUTC = (await ParseDateTimeAsync(
                () => metadataDict.GetAsync(
                txFactory,
                Key(PhaseStartDateTimeUTC, migrationPhase, currentIteration),
                DefaultRCTimeout,
                cancellationToken),
                traceId)).Value;

            result.EndDateTimeUTC = await ParseDateTimeAsync(
                () => metadataDict.GetValueOrDefaultAsync(
                txFactory,
                Key(PhaseEndDateTimeUTC, migrationPhase, currentIteration),
                DefaultRCTimeout,
                cancellationToken),
                traceId);

            result.StartSeqNum = (await ParseLongAsync(
                () => metadataDict.GetAsync(
                txFactory,
                Key(PhaseStartSeqNum, migrationPhase, currentIteration),
                DefaultRCTimeout,
                cancellationToken),
                traceId)).Value;

            result.EndSeqNum = (await ParseLongAsync(
                () => metadataDict.GetAsync(
                txFactory,
                Key(PhaseEndSeqNum, migrationPhase, currentIteration),
                DefaultRCTimeout,
                cancellationToken),
                traceId)).Value;

            result.LastAppliedSeqNum = await ParseLongAsync(
                () => metadataDict.GetValueOrDefaultAsync(
                txFactory,
                Key(PhaseLastAppliedSeqNum, migrationPhase, currentIteration),
                DefaultRCTimeout,
                cancellationToken),
                traceId);

            result.NoOfKeysMigrated = await ParseLongAsync(
                () => metadataDict.GetValueOrDefaultAsync(
                txFactory,
                Key(PhaseNoOfKeysMigrated, migrationPhase, currentIteration),
                DefaultRCTimeout,
                cancellationToken),
                traceId);

            result.WorkerCount = await ParseIntAsync(
                () => metadataDict.GetAsync(
                txFactory,
                Key(PhaseWorkerCount, migrationPhase, currentIteration),
                DefaultRCTimeout,
                cancellationToken),
                traceId);

            var workerResults = new List<WorkerResult>();

            for (int i = 1; i <= result.WorkerCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var workerResult = await MigrationWorker.GetResultAsync(metadataDict, txFactory, migrationPhase, currentIteration, i, traceId, cancellationToken);
                if (workerResult.Status != MigrationState.None)
                {
                    workerResults.Add(workerResult);
                }
            }

            result.WorkerResults = workerResults.ToArray();

            return result;
        }

        public async Task<PhaseResult> StartOrResumeMigrationAsync(CancellationToken cancellationToken)
        {
            PhaseInput input = null;

            try
            {
                input = await this.GetOrAddInputAsync(cancellationToken);
                if (input.Status == MigrationState.Completed)
                {
                    ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.traceId,
                        $"Phase already completed \n Input: {input.ToString()}");

                    return await GetResultAsync(
                        this.MetaDataDictionary,
                        () => this.Transaction,
                        this.migrationPhase,
                        this.currentIteration,
                        this.TraceId,
                        cancellationToken);
                }

                ActorTrace.Source.WriteInfoWithId(
                    TraceType,
                    this.traceId,
                    $"Starting or resuming {this.migrationPhase} - {this.currentIteration} Phase\n Input: {input.ToString()}");
                MigrationTelemetry.MigrationPhaseStartEvent(this.StatefulServiceContext, input.ToString());

                var workers = this.CreateMigrationWorkers(input, cancellationToken);
                var tasks = new List<Task<WorkerResult>>();
                foreach (var worker in workers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (worker.Input.Status != MigrationState.Completed)
                    {
                        tasks.Add(worker.StartWorkAsync(cancellationToken));
                    }
                }

                var results = await Task.WhenAll(tasks);
                await this.AddOrUpdateResultAsync(input, results, cancellationToken);
                PhaseResult phaseResult = await GetResultAsync(
                    this.MetaDataDictionary,
                    () => this.Transaction,
                    this.migrationPhase,
                    this.currentIteration,
                    this.TraceId,
                    cancellationToken);

                ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.traceId,
                        $"Completed Migration phase\n Result: {phaseResult.ToString()}");
                MigrationTelemetry.MigrationPhaseEndEvent(this.StatefulServiceContext, phaseResult.ToString());

                return phaseResult;
            }
            catch (Exception ex)
            {
                var inputString = input == null ? null : input.ToString();
                ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        this.traceId,
                        $"Migration phase failed with error: {ex} \n Input: {inputString}");

                throw ex;
            }
        }

        protected virtual async Task<long> GetEndSequenceNumberAsync(CancellationToken cancellationToken)
        {
            return (await ParseLongAsync(
                () => this.servicePartitionClient.InvokeWithRetryAsync<string>(
                async client =>
                {
                    return await client.HttpClient.GetStringAsync($"{KVSMigrationControllerName}/{GetEndSNEndpoint}");
                },
                cancellationToken),
                this.TraceId)).Value;
        }

        protected virtual async Task<long> GetStartSequenceNumberAsync(CancellationToken cancellationToken)
        {
            long startSequenceNumber;
            using (var tx = this.Transaction)
            {
                startSequenceNumber = (await ParseLongAsync(
                    () => this.MetaDataDictionary.GetAsync(
                        tx,
                        MigrationLastAppliedSeqNum,
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId)).Value;

                await tx.CommitAsync();
            }

            return ++startSequenceNumber;
        }

        protected virtual async Task<PhaseInput> GetOrAddInputAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var input = new PhaseInput();
            using (var tx = this.Transaction)
            {
                input.Phase = this.migrationPhase;

                await this.MetaDataDictionary.AddOrUpdateAsync(
                        tx,
                        MigrationCurrentPhase,
                        this.migrationPhase.ToString(),
                        (_, __) => this.migrationPhase.ToString(),
                        DefaultRCTimeout,
                        cancellationToken);

                input.StartDateTimeUTC = (await ParseDateTimeAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                        tx,
                        Key(PhaseStartDateTimeUTC, this.migrationPhase, this.currentIteration),
                        DateTime.UtcNow.ToString(),
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId)).Value;

                input.EndDateTimeUTC = await ParseDateTimeAsync(
                    () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                        tx,
                        Key(PhaseStartDateTimeUTC, this.migrationPhase, this.currentIteration),
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

                input.StartSeqNum = (await ParseLongAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                        tx,
                        Key(PhaseStartSeqNum, this.migrationPhase, this.currentIteration),
                        this.GetStartSequenceNumberAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult().ToString(),
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId)).Value;

                // Update Migration seq num if it not present(first time)
                await this.MetaDataDictionary.GetOrAddAsync(
                    tx,
                    MigrationStartSeqNum,
                    input.StartSeqNum.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                input.EndSeqNum = (await ParseLongAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                        tx,
                        Key(PhaseEndSeqNum, this.migrationPhase, this.currentIteration),
                        this.GetEndSequenceNumberAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult().ToString(),
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId)).Value;

                input.LastAppliedSeqNum = await ParseLongAsync(
                    () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                        tx,
                        Key(PhaseLastAppliedSeqNum, this.migrationPhase, this.currentIteration),
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
                        Key(PhaseWorkerCount, this.migrationPhase, this.currentIteration),
                        this.workerCount.ToString(),
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId);

                long perWorker = (input.EndSeqNum - input.StartSeqNum) / this.workerCount;
                var perWorkerStartSN = input.StartSeqNum;
                var perWorkerEndSN = input.StartSeqNum + perWorker;
                var workerInputs = new List<WorkerInput>();
                for (int i = 1; i <= this.workerCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var workerInput = new WorkerInput
                    {
                        WorkerId = i,
                        Phase = this.migrationPhase,
                        Iteration = this.currentIteration,
                    };

                    workerInput.StartDateTimeUTC = (await ParseDateTimeAsync(
                    () => this.MetaDataDictionary.GetOrAddAsync(
                        tx,
                        Key(PhaseWorkerStartDateTimeUTC, this.migrationPhase, this.currentIteration, i),
                        DateTime.UtcNow.ToString(),
                        DefaultRCTimeout,
                        cancellationToken),
                    this.TraceId)).Value;

                    workerInput.EndDateTimeUTC = await ParseDateTimeAsync(
                        () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                            tx,
                            Key(PhaseWorkerEndDateTimeUTC, this.migrationPhase, this.currentIteration, i),
                            DefaultRCTimeout,
                            cancellationToken),
                        this.TraceId);

                    workerInput.Status = await ParseMigrationStateAsync(
                        () => this.MetaDataDictionary.GetOrAddAsync(
                            tx,
                            Key(PhaseWorkerCurrentStatus, this.migrationPhase, this.currentIteration, i),
                            MigrationState.InProgress.ToString(),
                            DefaultRCTimeout,
                            cancellationToken),
                        this.TraceId);

                    workerInput.StartSeqNum = (await ParseLongAsync(
                        () => this.MetaDataDictionary.GetOrAddAsync(
                            tx,
                            Key(PhaseWorkerStartSeqNum, this.migrationPhase, this.currentIteration, i),
                            perWorkerStartSN.ToString(),
                            DefaultRCTimeout,
                            cancellationToken),
                        this.TraceId)).Value;

                    workerInput.EndSeqNum = (await ParseLongAsync(
                        () => this.MetaDataDictionary.GetOrAddAsync(
                            tx,
                            Key(PhaseWorkerEndSeqNum, this.migrationPhase, this.currentIteration, i),
                            perWorkerEndSN.ToString(),
                            DefaultRCTimeout,
                            cancellationToken),
                        this.TraceId)).Value;

                    workerInput.LastAppliedSeqNum = await ParseLongAsync(
                        () => this.MetaDataDictionary.GetValueOrDefaultAsync(
                            tx,
                            Key(PhaseWorkerLastAppliedSeqNum, this.migrationPhase, this.currentIteration, i),
                            DefaultRCTimeout,
                            cancellationToken),
                        this.TraceId);

                    workerInputs.Add(workerInput);

                    if (perWorkerEndSN == input.EndSeqNum)
                    {
                        break;
                    }

                    perWorkerStartSN = perWorkerEndSN + 1;
                    perWorkerEndSN = (perWorkerStartSN + perWorker) < input.EndSeqNum ? (perWorkerStartSN + perWorker) : input.EndSeqNum;
                }

                input.WorkerInputs = workerInputs.ToArray();

                await tx.CommitAsync();
            }

            return input;
        }

        protected virtual List<IWorker> CreateMigrationWorkers(PhaseInput input, CancellationToken cancellationToken)
        {
            var workers = new List<IWorker>();

            foreach (var workerInput in input.WorkerInputs)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (input.Phase == MigrationPhase.DataValidation)
                {
                    workers.Add(new DataValidationWorker(
                        this.StateProvider,
                        this.ActorTypeInformation,
                        this.ServicePartitionClient,
                        this.MigrationSettings,
                        workerInput,
                        this.TraceId));
                }
                else
                {
                    workers.Add(new MigrationWorker(
                        this.StateProvider,
                        this.ActorTypeInformation,
                        this.ServicePartitionClient,
                        this.MigrationSettings,
                        workerInput,
                        this.TraceId));
                }
            }

            return workers;
        }

        protected virtual async Task AddOrUpdateResultAsync(PhaseInput phaseInput, WorkerResult[] workerResults, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            long? keysMigratedByPhase = null;
            DateTime? endTime = DateTime.UtcNow;
            long keysMigratedByWorkers = 0L;
            foreach (var workerResult in workerResults)
            {
                keysMigratedByWorkers += workerResult.NoOfKeysMigrated.HasValue ? workerResult.NoOfKeysMigrated.Value : 0L;
            }

            using (var tx = this.Transaction)
            {
                endTime = await ParseDateTimeAsync(
                    () => this.MetaDataDictionary.AddOrUpdateAsync(
                    tx,
                    Key(PhaseEndDateTimeUTC, this.migrationPhase, this.currentIteration),
                    endTime.ToString(),
                    (_, __) => endTime.ToString(),
                    DefaultRCTimeout,
                    cancellationToken),
                    this.TraceId);

                await this.MetaDataDictionary.AddOrUpdateAsync(
                    tx,
                    Key(PhaseLastAppliedSeqNum, this.migrationPhase, this.currentIteration),
                    phaseInput.EndSeqNum.ToString(),
                    (_, __) => phaseInput.EndSeqNum.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                keysMigratedByPhase = await ParseLongAsync(
                    () => this.MetaDataDictionary.AddOrUpdateAsync(
                    tx,
                    Key(PhaseNoOfKeysMigrated, this.migrationPhase, this.currentIteration),
                    keysMigratedByWorkers.ToString(),
                    (k, v) =>
                    {
                        long currVal = ParseLong(v, this.TraceId);
                        long newVal = currVal + keysMigratedByWorkers;

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
                    phaseInput.EndSeqNum.ToString(),
                    (_, __) => phaseInput.EndSeqNum.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await this.MetaDataDictionary.AddOrUpdateAsync(
                    tx,
                    MigrationNoOfKeysMigrated,
                    keysMigratedByWorkers.ToString(),
                    (k, v) =>
                    {
                        var currentVal = ParseLong(v, this.TraceId);
                        var newVal = currentVal + keysMigratedByPhase;

                        return newVal.ToString();
                    },
                    DefaultRCTimeout,
                    cancellationToken);

                await tx.CommitAsync();
            }
         }
    }
}
