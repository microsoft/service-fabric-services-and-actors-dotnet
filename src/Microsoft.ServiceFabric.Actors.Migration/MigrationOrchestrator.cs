// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.DataProtection.Repositories;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal class MigrationOrchestrator : IMigrationOrchestrator
    {
        private KVStoRCMigrationActorStateProvider stateProvider;
        private ActorTypeInformation actorTypeInfo;
        private IReliableDictionary2<string, byte[]> metadataDict;
        private int workerCount;
        private long downtimeThreshold;
        private Uri kvsServiceUri;
        private StatefulServiceInitializationParameters initParams;
        private ServicePartitionClient<HttpCommunicationClient> servicePartitionClient;
        private StatefulServiceContext serviceContext;

        public MigrationOrchestrator(KVStoRCMigrationActorStateProvider stateProvider, ActorTypeInformation actorTypeInfo, StatefulServiceContext serviceContext)
        {
            this.stateProvider = stateProvider;
            this.actorTypeInfo = actorTypeInfo;
            this.initParams = this.stateProvider.GetInitParams();
            this.GetUserSettingsOrDefault();
            this.serviceContext = serviceContext;
        }

        public async Task StartMigration(CancellationToken cancellationToken)
        {
            try
            {
                var partitionInformation = this.stateProvider.StatefulServicePartition.PartitionInfo as Int64RangePartitionInformation;
                this.servicePartitionClient = new ServicePartitionClient<HttpCommunicationClient>(
                    new HttpCommunicationClientFactory(null, new List<IExceptionHandler>() { new HttpExceptionHandler() }),
                    this.kvsServiceUri,
                    new ServicePartitionKey(partitionInformation.LowKey),
                    TargetReplicaSelector.PrimaryReplica,
                    MigrationConstants.KVSMigrationListenerName);
                this.metadataDict = await this.stateProvider.GetMetadataDictionaryAsync();

                ConditionalValue<byte[]> migrationPhaseValue;
                cancellationToken.ThrowIfCancellationRequested();
                using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                {
                    migrationPhaseValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.MigrationPhaseKey);
                }

                if (migrationPhaseValue.HasValue)
                {
                    MigrationPhase.TryParse(Encoding.ASCII.GetString(migrationPhaseValue.Value), out MigrationPhase phase);
                    if (phase == MigrationPhase.Copy)
                    {
                        var lastAppliedSN = await this.ResumeCopyPhaseFromFailover(cancellationToken);
                        lastAppliedSN = await this.StartCatchupPhaseWorkload(lastAppliedSN, cancellationToken);
                        await this.StartDowntimePhaseWorkload(lastAppliedSN, cancellationToken);
                    }
                    else if (phase == MigrationPhase.Catchup)
                    {
                        var lastAppliedSNInCatchupPhase = await this.ResumeCatchupPhaseFromFailover(cancellationToken);
                        cancellationToken.ThrowIfCancellationRequested();
                        await this.StartDowntimePhaseWorkload(lastAppliedSNInCatchupPhase, cancellationToken);
                    }
                    else if (phase == MigrationPhase.Downtime)
                    {
                        await this.ResumeDowntimePhaseFromFailover(cancellationToken);
                    }
                    else if (phase == MigrationPhase.Uninitialized)
                    {
                        await this.StartCompleteMigration(cancellationToken);
                    }
                }
                else
                {
                    await this.StartCompleteMigration(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                DateTime startTime = DateTime.UtcNow;
                try
                {
                    using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                    {
                        startTime = DateTime.Parse(Encoding.ASCII.GetString(await this.metadataDict.GetValueAsync(tx, MigrationConstants.MigrationStartTimeUtcKey)));
                    }
                }
                catch (Exception ex2)
                {
                    ActorTrace.Source.WriteError("MigrationOrchestrator", ex2.Message);
                }

                ActorTelemetry.KVSToRCMigrationCompletionWithFailureEvent(this.serviceContext, this.kvsServiceUri.OriginalString, DateTime.UtcNow - startTime, ex.Message);
                throw ex;
            }
        }

        private async Task<long> ResumeCopyPhaseFromFailover(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfo("MigrationOrchestrator", "Resuming migration from copy phase failover");
            ConditionalValue<byte[]> copyPhaseWorkerCountValue, copyPhaseEndSNValue;
            cancellationToken.ThrowIfCancellationRequested();
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                copyPhaseWorkerCountValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.CopyWorkerCountKey);
                copyPhaseEndSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.CopyPhaseEndSNKey);
            }

            var workerCountForCopyPhase = int.Parse(Encoding.ASCII.GetString(copyPhaseWorkerCountValue.Value));
            var endSNForCopyPhase = long.Parse(Encoding.ASCII.GetString(copyPhaseEndSNValue.Value));
            var isWorkerTaskIncomplete = await this.GetWorkersWithIncompleteTasksAsync(workerCountForCopyPhase, cancellationToken);

            if (isWorkerTaskIncomplete.Contains(true))
            {
                // Ensure copy workers complete tasks.
                var tasks = new List<Task>();

                Parallel.For(0, workerCountForCopyPhase, async i =>
                {
                    if (isWorkerTaskIncomplete[i])
                    {
                        var worker = new MigrationWorker(this.stateProvider, this.actorTypeInfo, this.servicePartitionClient);
                        ConditionalValue<byte[]> startSNMetadataValue, endSNMetadataValue, lastAppliedSNMetadataValue;
                        using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                        {
                            startSNMetadataValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCopyWorkerStartSNKey(i));
                            endSNMetadataValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCopyWorkerEndSNKey(i));
                            lastAppliedSNMetadataValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCopyWorkerLastAppliedSNKey(i));
                        }

                        long startSNForCopyWorker = long.Parse(Encoding.ASCII.GetString(startSNMetadataValue.Value));
                        long endSNForCopyWorker = long.Parse(Encoding.ASCII.GetString(endSNMetadataValue.Value));
                        long lastAppliedSNForCopyWorker = long.Parse(Encoding.ASCII.GetString(lastAppliedSNMetadataValue.Value));

                        if (lastAppliedSNForCopyWorker != -1)
                        {
                            tasks.Add(worker.StartMigrationWorker(MigrationPhase.Copy, lastAppliedSNForCopyWorker + 1, endSNForCopyWorker, i, cancellationToken));
                        }
                        else
                        {
                            tasks.Add(worker.StartMigrationWorker(MigrationPhase.Copy, startSNForCopyWorker, endSNForCopyWorker, i, cancellationToken));
                        }
                    }
                });
                await Task.WhenAll(tasks);
            }

            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                var startTimebytes = await this.metadataDict.GetValueAsync(tx, MigrationConstants.CurrentMigrationPhaseStartTimeUtcKey);
                var startTime = DateTime.Parse(Encoding.ASCII.GetString(startTimebytes));
                await this.UpdateKeysMigratedForCopyPhaseAsync(startTime, DateTime.UtcNow);
            }

            return endSNForCopyPhase;
        }

        private async Task<List<bool>> GetWorkersWithIncompleteTasksAsync(int workerCount, CancellationToken cancellationToken)
        {
            var isWorkerTaskIncomplete = new List<bool>(workerCount);
            cancellationToken.ThrowIfCancellationRequested();
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                for (int i = 0; i < workerCount; i++)
                {
                    var workerStatusValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCopyWorkerStatusKey(i));
                    MigrationState.TryParse(Encoding.ASCII.GetString(workerStatusValue.Value), out MigrationState status);
                    isWorkerTaskIncomplete[i] = status != MigrationState.Completed;
                }
            }

            return isWorkerTaskIncomplete;
        }

        private async Task<long> ResumeCatchupPhaseFromFailover(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfo("MigrationOrchestrator", "Resuming migration from catchup phase failover");
            ConditionalValue<byte[]> iterationValue, startSNValue;
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                iterationValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.CatchupIterationKey);
                startSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.CatchupStartSNKey);
            }

            long lastAppliedSNInCatchupPhase;
            if (!iterationValue.HasValue)
            {
                // No catchup iterations started before failover.
                long startSN = long.Parse(Encoding.ASCII.GetString(startSNValue.Value));
                lastAppliedSNInCatchupPhase = await this.StartCatchupPhaseWorkload(startSN - 1, cancellationToken);
            }
            else
            {
                int catchupIterationCount = int.Parse(Encoding.ASCII.GetString(iterationValue.Value));
                ConditionalValue<byte[]> lastSNValue, lastAppliedSNValue;
                using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                {
                    lastSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCatchupWorkerEndSNKey(catchupIterationCount));
                    lastAppliedSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCatchupWorkerLastAppliedSNKey(catchupIterationCount));
                }

                if (lastAppliedSNValue.HasValue)
                {
                    long lastAppliedSNBeforeFailover = long.Parse(Encoding.ASCII.GetString(lastAppliedSNValue.Value));
                    long lastSN = long.Parse(Encoding.ASCII.GetString(lastSNValue.Value));
                    var endSequenceNumber = await this.GetEndSequenceNumber();

                    if (lastAppliedSNBeforeFailover != lastSN || (endSequenceNumber - lastAppliedSNBeforeFailover > this.downtimeThreshold))
                    {
                        // More catchup iterations are needed before moving to downtime phase
                        lastAppliedSNInCatchupPhase = await this.StartCatchupIteration(catchupIterationCount, lastAppliedSNBeforeFailover, cancellationToken);
                    }
                    else
                    {
                        lastAppliedSNInCatchupPhase = lastAppliedSNBeforeFailover;
                    }
                }
                else
                {
                    // Last catchup iteration didn't start when failover occurred.
                    ConditionalValue<byte[]> prevIterationLastAppliedSNValue;
                    using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                    {
                        prevIterationLastAppliedSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCatchupWorkerLastAppliedSNKey(catchupIterationCount - 1));
                    }

                    long prevIterationLastAppliedSN = long.Parse(Encoding.ASCII.GetString(prevIterationLastAppliedSNValue.Value));
                    lastAppliedSNInCatchupPhase = await this.StartCatchupIteration(catchupIterationCount - 1, prevIterationLastAppliedSN, cancellationToken);
                }
            }

            return lastAppliedSNInCatchupPhase;
        }

        private async Task ResumeDowntimePhaseFromFailover(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfo("MigrationOrchestrator", "Resuming migration from downtime phase failover");
            ConditionalValue<byte[]> lastAppliedSNValueForDowntime, startSNValueForDowntime, lastSNValueForDowntime;
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                lastAppliedSNValueForDowntime = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.DowntimeWorkerLastAppliedSNKey);
                startSNValueForDowntime = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.DowntimeStartSNKey);
                lastSNValueForDowntime = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.DowntimeEndSNKey);
            }

            long lastSNForDowntime = long.Parse(Encoding.ASCII.GetString(lastSNValueForDowntime.Value));

            if (lastAppliedSNValueForDowntime.HasValue)
            {
                long lastAppliedSNBeforeFailure = long.Parse(Encoding.ASCII.GetString(lastAppliedSNValueForDowntime.Value));
                if (lastAppliedSNBeforeFailure != lastSNForDowntime)
                {
                    var worker = new MigrationWorker(this.stateProvider, this.actorTypeInfo, this.servicePartitionClient);
                    await worker.StartMigrationWorker(MigrationPhase.Downtime, lastAppliedSNBeforeFailure + 1, lastSNForDowntime, -1, cancellationToken);
                }

                DateTime startDateTime = DateTime.UtcNow;
                using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                {
                    var startDateTimeBytes = await this.metadataDict.GetValueAsync(tx, MigrationConstants.CurrentMigrationPhaseStartTimeUtcKey);
                    var startDateTimeString = Encoding.ASCII.GetString(startDateTimeBytes);
                    startDateTime = DateTime.Parse(startDateTimeString);
                }

                await this.UpdateKeysMigratedForDowntimePhaseAsync(startDateTime, DateTime.UtcNow);

                await this.MarkMigrationCompleted(cancellationToken);
            }
            else
            {
                long startSN = long.Parse(Encoding.ASCII.GetString(startSNValueForDowntime.Value));
                await this.StartDowntimePhaseWorkload(startSN - 1, cancellationToken);
            }
        }

        private async Task StartCompleteMigration(CancellationToken cancellationToken)
        {
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationPhaseKey, MigrationPhase.Uninitialized.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationStateKey, MigrationState.InProgress.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationStartTimeUtcKey, DateTime.UtcNow.ToString());
                await tx.CommitAsync();
            }

            ActorTrace.Source.WriteInfo("MigrationOrchestrator", "Inititating complete migration");

            cancellationToken.ThrowIfCancellationRequested();
            long lastUpdatedRecord = await this.StartCopyPhaseWorkload(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            lastUpdatedRecord = await this.StartCatchupPhaseWorkload(lastUpdatedRecord, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            await this.StartDowntimePhaseWorkload(lastUpdatedRecord, cancellationToken);
        }

        private async Task<long> StartCopyPhaseWorkload(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfo("MigrationOrchestrator", "Inititating copy phase of migration");
            var startSequenceNumber = await this.GetStartSequenceNumber();
            var endSequenceNumber = await this.GetEndSequenceNumber();
            var startTimeUtc = DateTime.UtcNow;

            ActorTelemetry.KVSToRCMigrationStartEvent(
                this.serviceContext,
                this.kvsServiceUri.OriginalString,
                startTimeUtc,
                endSequenceNumber - startSequenceNumber,
                this.workerCount,
                this.downtimeThreshold);

            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationPhaseKey, MigrationPhase.Copy.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.CopyWorkerCountKey, this.workerCount.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.CopyPhaseStartSNKey, startSequenceNumber.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.CopyPhaseEndSNKey, endSequenceNumber.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.CurrentMigrationPhaseStartTimeUtcKey, startTimeUtc.ToString());
                await tx.CommitAsync();
            }

            List<MigrationWorker> workers = new List<MigrationWorker>(this.workerCount);
            for (int i = 0; i < this.workerCount; i++)
            {
                workers.Add(new MigrationWorker(this.stateProvider, this.actorTypeInfo, this.servicePartitionClient));
            }

            var workerLoad = this.GetEndSequenceNumberForEachWorker(startSequenceNumber, endSequenceNumber);
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                for (int i = 0; i < this.workerCount; i++)
                {
                    this.AddOrUpdateMetadata(tx, MigrationConstants.GetCopyWorkerStatusKey(i), MigrationState.InProgress.ToString());
                    if (i == 0)
                    {
                        this.AddOrUpdateMetadata(tx, MigrationConstants.GetCopyWorkerStartSNKey(i), startSequenceNumber.ToString());
                    }
                    else
                    {
                        this.AddOrUpdateMetadata(tx, MigrationConstants.GetCopyWorkerStartSNKey(i), workerLoad[i - 1] + 1.ToString());
                    }

                    this.AddOrUpdateMetadata(tx, MigrationConstants.GetCopyWorkerEndSNKey(i), workerLoad[i].ToString());
                    this.AddOrUpdateMetadata(tx, MigrationConstants.GetCopyWorkerLastAppliedSNKey(i), "-1");
                }

                await tx.CommitAsync();
            }

            var tasks = new List<Task>();
            Parallel.For(0, this.workerCount, index =>
            {
                if (index == 0)
                {
                    tasks.Add(workers[index].StartMigrationWorker(MigrationPhase.Copy, startSequenceNumber, workerLoad[index], index, cancellationToken));
                }
                else
                {
                    tasks.Add(workers[index].StartMigrationWorker(MigrationPhase.Copy, workerLoad[index - 1] + 1, workerLoad[index], index, cancellationToken));
                }
            });

            cancellationToken.ThrowIfCancellationRequested();
            await Task.WhenAll(tasks);

            await this.UpdateKeysMigratedForCopyPhaseAsync(startTimeUtc, DateTime.UtcNow);

            ActorTrace.Source.WriteInfo("MigrationOrchestrator", "Completed copy phase of migration");

            return endSequenceNumber;
        }

        private async Task UpdateKeysMigratedForCopyPhaseAsync(DateTime startTime, DateTime endTime)
        {
            int totalKeysMigrated = 0;
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                for (int i = 0; i < this.workerCount; i++)
                {
                    var result = await this.metadataDict.GetValueAsync(tx, MigrationWorker.GetKeyForKeysMigrated(MigrationPhase.Copy, i));
                    int currentKeyCount;
                    if (int.TryParse(Encoding.ASCII.GetString(result), out currentKeyCount))
                    {
                        totalKeysMigrated += currentKeyCount;
                    }
                }

                var keyMigratedVal = Encoding.ASCII.GetBytes(totalKeysMigrated.ToString());
                await this.metadataDict.AddOrUpdateAsync(tx, MigrationConstants.CopyPhaseKeysMigrated, keyMigratedVal, (k, v) => keyMigratedVal);
            }

            ActorTelemetry.KVSToRCMigrationCopyPhaseEvent(this.serviceContext, this.kvsServiceUri.OriginalString, endTime - startTime, totalKeysMigrated);
        }

        private async Task UpdateKeysMigratedForDowntimePhaseAsync(DateTime startTime, DateTime endTime)
        {
            int totalKeysMigrated = 0;
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                var result = await this.metadataDict.GetValueAsync(tx, MigrationWorker.GetKeyForKeysMigrated(MigrationPhase.Downtime, -1));
                int currentKeyCount;
                if (int.TryParse(Encoding.ASCII.GetString(result), out currentKeyCount))
                {
                    totalKeysMigrated = currentKeyCount;
                }
            }

            ActorTelemetry.KVSToRCMigrationDowntimePhaseEvent(this.serviceContext, this.kvsServiceUri.OriginalString, endTime - startTime, totalKeysMigrated);
        }

        private async Task<long> StartCatchupPhaseWorkload(long lastUpdatedRecord, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfo("MigrationOrchestrator", "Inititating catchup phase of migration");
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationPhaseKey, MigrationPhase.Catchup.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.CatchupStartSNKey, lastUpdatedRecord + 1.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.CurrentMigrationPhaseStartTimeUtcKey, DateTime.UtcNow.ToString());
                await tx.CommitAsync();
            }

            return await this.StartCatchupIteration(0, lastUpdatedRecord, cancellationToken);
        }

        private async Task<long> StartCatchupIteration(int catchupCount, long lastUpdatedRecord, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfo("MigrationOrchestrator", "Starting catchup iterations");
            var endSequenceNumber = await this.GetEndSequenceNumber();

            if (endSequenceNumber <= lastUpdatedRecord)
            {
                return lastUpdatedRecord;
            }

            var worker = new MigrationWorker(this.stateProvider, this.actorTypeInfo, this.servicePartitionClient);
            cancellationToken.ThrowIfCancellationRequested();
            while (endSequenceNumber - lastUpdatedRecord > this.downtimeThreshold)
            {
                catchupCount++;
                cancellationToken.ThrowIfCancellationRequested();
                using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                {
                    this.AddOrUpdateMetadata(tx, MigrationConstants.CatchupIterationKey, catchupCount.ToString());
                    this.AddOrUpdateMetadata(tx, MigrationConstants.GetCatchupWorkerStartSNKey(catchupCount), lastUpdatedRecord + 1.ToString());
                    this.AddOrUpdateMetadata(tx, MigrationConstants.GetCatchupWorkerEndSNKey(catchupCount), endSequenceNumber.ToString());
                    await tx.CommitAsync();
                }

                await worker.StartMigrationWorker(MigrationPhase.Catchup, lastUpdatedRecord + 1, endSequenceNumber, catchupCount, cancellationToken);
                lastUpdatedRecord = endSequenceNumber;
                endSequenceNumber = await this.GetEndSequenceNumber();
            }

            DateTime startDateTime = DateTime.UtcNow;
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                var startDateTimeBytes = await this.metadataDict.GetValueAsync(tx, MigrationConstants.CurrentMigrationPhaseStartTimeUtcKey);
                var startDateTimeString = Encoding.ASCII.GetString(startDateTimeBytes);
                startDateTime = DateTime.Parse(startDateTimeString);
            }

            await this.UpdateKeyMigratedForCatchupPhaseAsync(startDateTime, DateTime.UtcNow, catchupCount);
            ActorTrace.Source.WriteInfo("MigrationOrchestrator", "Completed catchup phase of migration");
            return lastUpdatedRecord;
        }

        private async Task UpdateKeyMigratedForCatchupPhaseAsync(DateTime startTime, DateTime endTime, int catchupCount)
        {
            int totalCount = 0;
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                for (int i = 1; i <= catchupCount; i++)
                {
                    var currentCountBytes = await this.metadataDict.GetValueAsync(tx, MigrationWorker.GetKeyForKeysMigrated(MigrationPhase.Catchup, i));
                    var curretnCountString = Encoding.ASCII.GetString(currentCountBytes);
                    var currentCount = int.Parse(curretnCountString);
                    totalCount += currentCount;
                    await this.metadataDict.AddAsync(tx, MigrationConstants.CatchupPhaseKeysMigrated, Encoding.ASCII.GetBytes(totalCount.ToString()));
                }
            }

            ActorTelemetry.KVSToRCMigrationCatchupPhaseEvent(this.serviceContext, this.kvsServiceUri.OriginalString, endTime - startTime, catchupCount, totalCount);
        }

        private async Task StartDowntimePhaseWorkload(long lastUpdatedRecord, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfo("MigrationOrchestrator", "Inititating downtime phase of migration");
            await this.servicePartitionClient.InvokeWithRetryAsync(async client =>
            {
                return await client.HttpClient.PutAsync($"{MigrationConstants.KVSMigrationControllerName}/{MigrationConstants.RejectWritesAPIEndpoint}", null);
            });

            var endSequenceNumber = await this.GetEndSequenceNumber();
            if (endSequenceNumber <= lastUpdatedRecord)
            {
                return;
            }

            var worker = new MigrationWorker(this.stateProvider, this.actorTypeInfo, this.servicePartitionClient);

            cancellationToken.ThrowIfCancellationRequested();
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationPhaseKey, MigrationPhase.Downtime.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.DowntimeStartSNKey, lastUpdatedRecord + 1.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.DowntimeEndSNKey, endSequenceNumber.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.CurrentMigrationPhaseStartTimeUtcKey, DateTime.UtcNow.ToString());
                await tx.CommitAsync();
            }

            await worker.StartMigrationWorker(MigrationPhase.Downtime, lastUpdatedRecord + 1, endSequenceNumber, -1, cancellationToken);
            DateTime startDateTime = DateTime.UtcNow;
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                var startDateTimeBytes = await this.metadataDict.GetValueAsync(tx, MigrationConstants.CurrentMigrationPhaseStartTimeUtcKey);
                var startDateTimeString = Encoding.ASCII.GetString(startDateTimeBytes);
                startDateTime = DateTime.Parse(startDateTimeString);
            }

            await this.UpdateKeysMigratedForDowntimePhaseAsync(startDateTime, DateTime.UtcNow);
            ActorTrace.Source.WriteInfo("MigrationOrchestrator", "Completed downtime phase of migration");
            await this.MarkMigrationCompleted(cancellationToken);
        }

        private async Task MarkMigrationCompleted(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            long totalKeyMigrated = 0L;
            DateTime startTime = DateTime.UtcNow;
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                totalKeyMigrated += int.Parse(Encoding.ASCII.GetString(await this.metadataDict.GetValueAsync(tx, MigrationConstants.CopyPhaseKeysMigrated)));
                startTime = DateTime.Parse(Encoding.ASCII.GetString(await this.metadataDict.GetValueAsync(tx, MigrationConstants.MigrationStartTimeUtcKey)));
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationPhaseKey, MigrationPhase.Completed.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationStateKey, MigrationState.Completed.ToString());
                await tx.CommitAsync();
            }

            ActorTelemetry.KVSToRCMigrationCompletionWithSuccessEvent(this.serviceContext, this.kvsServiceUri.OriginalString, DateTime.UtcNow - startTime, totalKeyMigrated);
        }

        private async Task<long> GetEndSequenceNumber()
        {
            var endSNString = await this.servicePartitionClient.InvokeWithRetryAsync<string>(async client =>
            {
                return await client.HttpClient.GetStringAsync($"{MigrationConstants.KVSMigrationControllerName}/{MigrationConstants.GetEndSNEndpoint}");
            });

            return long.Parse(endSNString);
        }

        private async Task<long> GetStartSequenceNumber()
        {
            var startSNString = await this.servicePartitionClient.InvokeWithRetryAsync<string>(async client =>
            {
                return await client.HttpClient.GetStringAsync($"{MigrationConstants.KVSMigrationControllerName}/{MigrationConstants.GetStartSNEndpoint}");
            });

            return long.Parse(startSNString);
        }

        private List<long> GetEndSequenceNumberForEachWorker(long startLSN, long endLSN)
        {
            var recordCount = endLSN - startLSN + 1;
            var n = recordCount / this.workerCount;

            var list = new List<long>(this.workerCount);
            for (int i = 0; i < this.workerCount; ++i)
            {
                list.Add(n);
            }

            if (recordCount % this.workerCount != 0)
            {
                var q = recordCount % this.workerCount;
                for (int i = 0; i < q; ++i)
                {
                    list[i] += 1;
                }
            }

            var lastEndLSN = startLSN - 1;
            for (int i = 0; i < this.workerCount; i++)
            {
                list[i] += lastEndLSN;
                lastEndLSN = list[i];
            }

            if (lastEndLSN != endLSN)
            {
                var message = "Error while dividing sequence numbers among workers during migration.";
                ActorTrace.Source.WriteError("MigrationOrchestrator", message);
            }

            return list;
        }

        private async void AddOrUpdateMetadata(ITransaction tx, string key, string value)
        {
            byte[] metadataEntry = Encoding.ASCII.GetBytes(value);
            await this.metadataDict.AddOrUpdateAsync(tx, key, metadataEntry, (k, v) => metadataEntry);
        }

        private void GetUserSettingsOrDefault()
        {
            this.workerCount = Environment.ProcessorCount;
            this.downtimeThreshold = 1024;

            var configPackageName = ActorNameFormat.GetConfigPackageName();
            try
            {
                var configPackageObj = this.initParams.CodePackageActivationContext.GetConfigurationPackageObject(configPackageName);
                var migrationConfigLabel = ActorNameFormat.GetMigrationConfigSectionName(this.actorTypeInfo.ImplementationType);
                if (configPackageObj.Settings.Sections.Contains(migrationConfigLabel))
                {
                    var migrationSettings = configPackageObj.Settings.Sections[migrationConfigLabel];
                    if (migrationSettings.Parameters.Contains("CopyPhaseParallelism"))
                    {
                        this.workerCount = int.Parse(migrationSettings.Parameters["CopyPhaseParallelism"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("KVSActorServiceUri"))
                    {
                        this.kvsServiceUri = new Uri(migrationSettings.Parameters["KVSActorServiceUri"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("DowntimeThreshold"))
                    {
                        this.downtimeThreshold = int.Parse(migrationSettings.Parameters["DowntimeThreshold"].Value);
                    }
                }
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteError("MigrationOrchestrator", e.Message);
            }
        }
    }
}
