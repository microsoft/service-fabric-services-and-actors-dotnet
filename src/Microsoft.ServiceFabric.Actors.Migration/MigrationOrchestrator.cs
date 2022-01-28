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

        public MigrationOrchestrator(KVStoRCMigrationActorStateProvider stateProvider, ActorTypeInformation actorTypeInfo)
        {
            this.stateProvider = stateProvider;
            this.actorTypeInfo = actorTypeInfo;
            this.initParams = this.stateProvider.GetInitParams();
        }

        public async Task StartMigration(CancellationToken cancellationToken)
        {
            this.GetUserSettingsOrDefault();
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
                    ConditionalValue<byte[]> workerCountValue, endSNValue;
                    cancellationToken.ThrowIfCancellationRequested();
                    using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                    {
                        workerCountValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.CopyWorkerCountKey);
                        endSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.CopyPhaseEndSNKey);
                    }

                    int.TryParse(Encoding.ASCII.GetString(workerCountValue.Value), out var workerCount);
                    long.TryParse(Encoding.ASCII.GetString(endSNValue.Value), out var endSN);
                    var isWorkerTaskIncomplete = new List<bool>(workerCount);

                    cancellationToken.ThrowIfCancellationRequested();
                    using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                    {
                        for (int i = 0; i < workerCount; i++)
                        {
                            var workerStatusValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCopyWorkerStatusKey(i));
                            MigrationStatus.TryParse(Encoding.ASCII.GetString(workerStatusValue.Value), out MigrationStatus status);
                            isWorkerTaskIncomplete[i] = status != MigrationStatus.Completed;
                        }
                    }

                    if (isWorkerTaskIncomplete.Contains(true))
                    {
                        var tasks = new List<Task>();

                        Parallel.For(0, workerCount, async i =>
                        {
                            if (isWorkerTaskIncomplete[i])
                            {
                                var worker = new MigrationWorker(this.stateProvider, this.actorTypeInfo, this.servicePartitionClient);
                                ConditionalValue<byte[]> startSNValue, endSNValue, lastAppliedSNValue;
                                using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                                {
                                    startSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCopyWorkerStartSNKey(i));
                                    endSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCopyWorkerEndSNKey(i));
                                    lastAppliedSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCopyWorkerLastAppliedSNKey(i));
                                }

                                long.TryParse(Encoding.ASCII.GetString(startSNValue.Value), out long startSN);
                                long.TryParse(Encoding.ASCII.GetString(endSNValue.Value), out long endSN);
                                long.TryParse(Encoding.ASCII.GetString(lastAppliedSNValue.Value), out long lastAppliedSN);

                                if (lastAppliedSN != -1)
                                {
                                    tasks.Add(worker.StartMigrationWorker(MigrationPhase.Copy, lastAppliedSN, endSN, i, cancellationToken));
                                }
                                else
                                {
                                    tasks.Add(worker.StartMigrationWorker(MigrationPhase.Copy, startSN, endSN, i, cancellationToken));
                                }
                            }
                        });
                        await Task.WhenAll(tasks);

                        var lastUpdatedRecord = await this.StartCatchupPhaseWorkload(endSN, cancellationToken);
                        await this.StartDowntimePhaseWorkload(lastUpdatedRecord, cancellationToken);
                    }
                    else
                    {
                        var lastUpdatedRecord = await this.StartCatchupPhaseWorkload(endSN, cancellationToken);
                        await this.StartDowntimePhaseWorkload(lastUpdatedRecord, cancellationToken);
                    }
                }
                else if (phase == MigrationPhase.Catchup)
                {
                    ConditionalValue<byte[]> iterationValue, startSNValue;
                    using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                    {
                        iterationValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.CatchupIterationKey);
                        startSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.CatchupStartSNKey);
                    }

                    if (!iterationValue.HasValue)
                    {
                        long.TryParse(Encoding.ASCII.GetString(startSNValue.Value), out long startSN);
                        var lastAppliedSN = await this.StartCatchupPhaseWorkload(startSN - 1, cancellationToken);
                        cancellationToken.ThrowIfCancellationRequested();
                        await this.StartDowntimePhaseWorkload(lastAppliedSN, cancellationToken);
                    }
                    else
                    {
                        int.TryParse(Encoding.ASCII.GetString(iterationValue.Value), out int iterationCount);
                        ConditionalValue<byte[]> lastSNValue, lastAppliedSNValue;
                        using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                        {
                            lastSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCatchupWorkerEndSNKey(iterationCount));
                            lastAppliedSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCatchupWorkerLastAppliedSNKey(iterationCount));
                        }

                        if (lastAppliedSNValue.HasValue)
                        {
                            long.TryParse(Encoding.ASCII.GetString(lastAppliedSNValue.Value), out long lastAppliedSN);
                            long.TryParse(Encoding.ASCII.GetString(lastSNValue.Value), out long lastSN);
                            var endSequenceNumber = await this.GetEndSequenceNumber();
                            if (lastAppliedSN != lastSN || (endSequenceNumber - lastAppliedSN > this.downtimeThreshold))
                            {
                                lastAppliedSN = await this.StartCatchupIteration(iterationCount, lastAppliedSN, cancellationToken);
                                await this.StartDowntimePhaseWorkload(lastAppliedSN, cancellationToken);
                            }
                            else
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                await this.StartDowntimePhaseWorkload(lastAppliedSN, cancellationToken);
                            }
                        }
                        else
                        {
                            ConditionalValue<byte[]> prevIterationLastAppliedSNValue;
                            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                            {
                                prevIterationLastAppliedSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.GetCatchupWorkerLastAppliedSNKey(iterationCount - 1));
                            }

                            long.TryParse(Encoding.ASCII.GetString(prevIterationLastAppliedSNValue.Value), out long prevIterationLastAppliedSN);
                            var lastAppliedSN = await this.StartCatchupIteration(iterationCount - 1, prevIterationLastAppliedSN, cancellationToken);
                            await this.StartDowntimePhaseWorkload(lastAppliedSN, cancellationToken);
                        }
                    }
                }
                else if (phase == MigrationPhase.Downtime)
                {
                    ConditionalValue<byte[]> lastAppliedSNValue, startSNValue, lastSNValue;
                    using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                    {
                        lastAppliedSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.DowntimeWorkerLastAppliedSNKey);
                        startSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.DowntimeStartSNKey);
                        lastSNValue = await this.metadataDict.TryGetValueAsync(tx, MigrationConstants.DowntimeEndSNKey);
                    }

                    long.TryParse(Encoding.ASCII.GetString(lastSNValue.Value), out long lastSN);

                    if (lastAppliedSNValue.HasValue)
                    {
                        long.TryParse(Encoding.ASCII.GetString(lastAppliedSNValue.Value), out long lastAppliedSN);
                        if (lastAppliedSN != lastSN)
                        {
                            var worker = new MigrationWorker(this.stateProvider, this.actorTypeInfo, this.servicePartitionClient);
                            await worker.StartMigrationWorker(MigrationPhase.Downtime, lastAppliedSN + 1, lastSN, -1, cancellationToken);
                            await this.ResumeWritesAndMarkeMigrationCompleted(cancellationToken);
                        }
                        else
                        {
                            await this.ResumeWritesAndMarkeMigrationCompleted(cancellationToken);
                        }
                    }
                    else
                    {
                        long.TryParse(Encoding.ASCII.GetString(startSNValue.Value), out long startSN);
                        await this.StartDowntimePhaseWorkload(startSN, cancellationToken);
                    }
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

        private async Task StartCompleteMigration(CancellationToken cancellationToken)
        {
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationPhaseKey, MigrationPhase.Uninitialized.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationStatusKey, MigrationStatus.InProgress.ToString());
                await tx.CommitAsync();
            }

            cancellationToken.ThrowIfCancellationRequested();
            long lastUpdatedRecord = await this.StartCopyPhaseWorkload(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            lastUpdatedRecord = await this.StartCatchupPhaseWorkload(lastUpdatedRecord, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            await this.StartDowntimePhaseWorkload(lastUpdatedRecord, cancellationToken);
        }

        private async Task<long> StartCopyPhaseWorkload(CancellationToken cancellationToken)
        {
            var startSequenceNumber = await this.GetStartSequenceNumber();
            var endSequenceNumber = await this.GetEndSequenceNumber();

            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationPhaseKey, MigrationPhase.Copy.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.CopyWorkerCountKey, this.workerCount.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.CopyPhaseStartSNKey, startSequenceNumber.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.CopyPhaseEndSNKey, endSequenceNumber.ToString());
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
                    this.AddOrUpdateMetadata(tx, MigrationConstants.GetCopyWorkerStatusKey(i), MigrationStatus.InProgress.ToString());
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
            return endSequenceNumber;
        }

        private async Task<long> StartCatchupPhaseWorkload(long lastUpdatedRecord, CancellationToken cancellationToken)
        {
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationPhaseKey, MigrationPhase.Catchup.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.CatchupStartSNKey, lastUpdatedRecord + 1.ToString());
                await tx.CommitAsync();
            }

            return await this.StartCatchupIteration(0, lastUpdatedRecord, cancellationToken);
        }

        private async Task<long> StartCatchupIteration(int catchupCount, long lastUpdatedRecord, CancellationToken cancellationToken)
        {
            var endSNString = await this.servicePartitionClient.InvokeWithRetryAsync<string>(async client =>
            {
                return await client.HttpClient.GetStringAsync(new Uri(client.EndpointUri, MigrationConstants.GetEndSNEndpoint));
            });
            var endSequenceNumber = long.Parse(endSNString);

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

            return lastUpdatedRecord;
        }

        private async Task StartDowntimePhaseWorkload(long lastUpdatedRecord, CancellationToken cancellationToken)
        {
            await this.servicePartitionClient.InvokeWithRetryAsync(async client =>
            {
                return await client.HttpClient.PutAsync(new Uri(client.EndpointUri, MigrationConstants.RejectWritesAPIEndpoint), null);
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
                await tx.CommitAsync();
            }

            await worker.StartMigrationWorker(MigrationPhase.Downtime, lastUpdatedRecord + 1, endSequenceNumber, -1, cancellationToken);
            await this.ResumeWritesAndMarkeMigrationCompleted(cancellationToken);
        }

        private async Task ResumeWritesAndMarkeMigrationCompleted(CancellationToken cancellationToken)
        {
            await this.servicePartitionClient.InvokeWithRetryAsync(async client =>
            {
                return await client.HttpClient.PutAsync(new Uri(client.EndpointUri, MigrationConstants.ResumeWritesAPIEndpoint), null);
            });

            cancellationToken.ThrowIfCancellationRequested();
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationPhaseKey, MigrationPhase.Completed.ToString());
                this.AddOrUpdateMetadata(tx, MigrationConstants.MigrationStatusKey, MigrationStatus.Completed.ToString());
                await tx.CommitAsync();
            }
        }

        private async Task<long> GetEndSequenceNumber()
        {
            var endSNString = await this.servicePartitionClient.InvokeWithRetryAsync<string>(async client =>
            {
                return await client.HttpClient.GetStringAsync(new Uri(client.EndpointUri, MigrationConstants.GetEndSNEndpoint));
            });

            return long.Parse(endSNString);
        }

        private async Task<long> GetStartSequenceNumber()
        {
            var startSNString = await this.servicePartitionClient.InvokeWithRetryAsync<string>(async client =>
            {
                return await client.HttpClient.GetStringAsync(new Uri(client.EndpointUri, MigrationConstants.GetStartSNEndpoint));
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
            this.workerCount = 8;
            this.downtimeThreshold = 1000;

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
