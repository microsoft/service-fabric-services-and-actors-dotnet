// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationConstants;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationUtility;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.PhaseInput;
    using static Microsoft.ServiceFabric.Actors.Migration.PhaseResult;

    internal class WorkerBase : IWorker
    {
        private static readonly string TraceType = typeof(WorkerBase).Name;
        private KVStoRCMigrationActorStateProvider stateProvider;
        private IReliableDictionary2<string, string> metadataDict;
        private WorkerInput workerInput;
        private string traceId;

        public WorkerBase(
            KVStoRCMigrationActorStateProvider stateProvider,
            WorkerInput workerInput,
            string traceId)
        {
            this.stateProvider = stateProvider;
            this.workerInput = workerInput;
            this.metadataDict = this.stateProvider.GetMetadataDictionaryAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            this.traceId = traceId;
        }

        public IReliableDictionary2<string, string> MetadataDict { get => this.metadataDict; }

        public KVStoRCMigrationActorStateProvider StateProvider { get => this.stateProvider; }

        public WorkerInput Input { get => this.workerInput; }

        public string TraceId { get => this.traceId; }

        public static async Task<WorkerResult> GetResultAsync(
           IReliableDictionary2<string, string> metadataDict,
           Func<ITransaction> txFactory,
           MigrationPhase migrationPhase,
           int currentIteration,
           int workerId,
           string traceId,
           CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var status = await ParseMigrationStateAsync(
                () => metadataDict.GetValueOrDefaultAsync(
                txFactory,
                Key(PhaseWorkerCurrentStatus, migrationPhase, currentIteration, workerId),
                DefaultRCTimeout,
                cancellationToken),
                traceId);

            if (status == MigrationState.None)
            {
                return new WorkerResult
                {
                    Phase = migrationPhase,
                    Iteration = currentIteration,
                    WorkerId = workerId,
                    Status = MigrationState.None,
                };
            }

            var workerResult = new WorkerResult
            {
                Status = status,
                Phase = migrationPhase,
                Iteration = currentIteration,
                WorkerId = workerId,
            };

            workerResult.StartDateTimeUTC = (await ParseDateTimeAsync(
                () => metadataDict.GetAsync(
                txFactory,
                Key(PhaseWorkerStartDateTimeUTC, migrationPhase, currentIteration, workerId),
                DefaultRCTimeout,
                cancellationToken),
                traceId)).Value;

            workerResult.EndDateTimeUTC = await ParseDateTimeAsync(
                () => metadataDict.GetValueOrDefaultAsync(
                txFactory,
                Key(PhaseWorkerEndDateTimeUTC, migrationPhase, currentIteration, workerId),
                DefaultRCTimeout,
                cancellationToken),
                traceId);

            workerResult.StartSeqNum = (await ParseLongAsync(
                () => metadataDict.GetAsync(
                txFactory,
                Key(PhaseWorkerStartSeqNum, migrationPhase, currentIteration, workerId),
                DefaultRCTimeout,
                cancellationToken),
                traceId)).Value;

            workerResult.EndSeqNum = (await ParseLongAsync(
                () => metadataDict.GetAsync(
                txFactory,
                Key(PhaseWorkerEndSeqNum, migrationPhase, currentIteration, workerId),
                DefaultRCTimeout,
                cancellationToken),
                traceId)).Value;

            workerResult.LastAppliedSeqNum = await ParseLongAsync(
                () => metadataDict.GetValueOrDefaultAsync(
                txFactory,
                Key(PhaseWorkerLastAppliedSeqNum, migrationPhase, currentIteration, workerId),
                DefaultRCTimeout,
                cancellationToken),
                traceId);

            workerResult.NoOfKeysMigrated = await ParseLongAsync(
                () => metadataDict.GetValueOrDefaultAsync(
                txFactory,
                Key(PhaseWorkerNoOfKeysMigrated, migrationPhase, currentIteration, workerId),
                DefaultRCTimeout,
                cancellationToken),
                traceId);

            return workerResult;
        }

        public virtual Task<WorkerResult> StartWorkAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected async Task CompleteWorkerAsync(ITransaction tx, CancellationToken cancellationToken)
        {
            await this.metadataDict.AddOrUpdateAsync(
                tx,
                Key(PhaseWorkerLastAppliedSeqNum, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                this.workerInput.EndSeqNum.ToString(),
                (_, __) => this.workerInput.EndSeqNum.ToString(),
                DefaultRCTimeout,
                cancellationToken);

            await this.metadataDict.AddOrUpdateAsync(
                tx,
                Key(PhaseWorkerCurrentStatus, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                MigrationState.Completed.ToString(),
                (_, __) => MigrationState.Completed.ToString(),
                DefaultRCTimeout,
                cancellationToken);

            await this.metadataDict.AddOrUpdateAsync(
                tx,
                Key(PhaseWorkerEndDateTimeUTC, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                DateTime.UtcNow.ToString(),
                (_, v) => v,
                DefaultRCTimeout,
                cancellationToken);
        }
    }
}
