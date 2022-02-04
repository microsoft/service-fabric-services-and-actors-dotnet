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
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using static Microsoft.ServiceFabric.Actors.Migration.MigrationConstants;
    using static Microsoft.ServiceFabric.Actors.Migration.MigrationUtility;

    internal class MigrationOrchestrator : IMigrationOrchestrator
    {
        private KVStoRCMigrationActorStateProvider stateProvider;
        private ActorTypeInformation actorTypeInfo;
        private IReliableDictionary2<string, string> metadataDict;
        private StatefulServiceInitializationParameters initParams;
        private ServicePartitionClient<HttpCommunicationClient> servicePartitionClient;
        private StatefulServiceContext serviceContext;
        private MigrationSettings migrationSettings;
        private string traceId;

        public MigrationOrchestrator(KVStoRCMigrationActorStateProvider stateProvider, ActorTypeInformation actorTypeInfo, StatefulServiceContext serviceContext)
        {
            this.stateProvider = stateProvider;
            this.actorTypeInfo = actorTypeInfo;
            this.initParams = this.stateProvider.GetInitParams();
            this.migrationSettings = MigrationSettings.LoadFrom(
                this.initParams.CodePackageActivationContext,
                ActorNameFormat.GetMigrationConfigSectionName(this.actorTypeInfo.ImplementationType));
            this.serviceContext = serviceContext;
            this.traceId = ActorTrace.GetTraceIdForReplica(this.initParams.PartitionId, this.initParams.ReplicaId);

            var partitionInformation = this.stateProvider.StatefulServicePartition.PartitionInfo as Int64RangePartitionInformation;
            this.servicePartitionClient = new ServicePartitionClient<HttpCommunicationClient>(
                    new HttpCommunicationClientFactory(null, new List<IExceptionHandler>() { new HttpExceptionHandler() }),
                    this.migrationSettings.KVSActorServiceUri,
                    new ServicePartitionKey(partitionInformation.LowKey),
                    TargetReplicaSelector.PrimaryReplica,
                    KVSMigrationListenerName);
        }

        public Data.ITransaction Transaction { get => this.stateProvider.GetStateManager().CreateTransaction(); }

        public IReliableDictionary2<string, string> MetaDataDictionary { get => this.metadataDict; }

        public ServicePartitionClient<HttpCommunicationClient> ServicePartitionClient { get => this.servicePartitionClient; }

        public StatefulServiceContext StatefulServiceContext { get => this.serviceContext; }

        public MigrationSettings MigrationSettings { get => this.migrationSettings; }

        public StatefulServiceInitializationParameters StatefulServiceInitializationParameters { get => this.initParams; }

        public KVStoRCMigrationActorStateProvider StateProvider { get => this.stateProvider; }

        public ActorTypeInformation ActorTypeInformation { get => this.actorTypeInfo; }

        public string TraceId { get => this.traceId; }

        public async Task StartMigrationAsync(CancellationToken cancellationToken)
        {
            await this.InitializeIfRequiredAsync(cancellationToken);
            var workloadRunner = await this.NextWorkloadRunnerAsync(MigrationPhase.None, cancellationToken);

            MigrationResult currentResult = null;
            while (workloadRunner != null)
            {
                await this.UpdateStartMetadataAsync(workloadRunner, cancellationToken);
                currentResult = await workloadRunner.StartOrResumeMigrationAsync(cancellationToken);
                await this.UpdateEndMetadataAsync(currentResult, cancellationToken);
                workloadRunner = await this.NextWorkloadRunnerAsync(currentResult, cancellationToken);
            }

            if (currentResult != null)
            {
                await this.CompleteMigrationAsync(currentResult, cancellationToken);
            }
        }

        private async Task CompleteMigrationAsync(MigrationResult result, CancellationToken cancellationToken)
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

                await tx.CommitAsync();
            }
        }

        private async Task UpdateEndMetadataAsync(MigrationResult result, CancellationToken cancellationToken)
        {
            using (var tx = this.Transaction)
            {
                await this.metadataDict.AddOrUpdateAsync(
                    tx,
                    MigrationLastAppliedSeqNum,
                    result.LastAppliedSeqNum.ToString(),
                    (_, __) => result.LastAppliedSeqNum.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await this.metadataDict.AddOrUpdateAsync(
                    tx,
                    MigrationNoOfKeysMigrated,
                    result.NoOfKeysMigrated.ToString(),
                    (k, v) =>
                    {
                        var currentVal = ParseLong(v, this.TraceId);
                        var newVal = currentVal + result.NoOfKeysMigrated;

                        return newVal.ToString();
                    },
                    DefaultRCTimeout,
                    cancellationToken);

                await tx.CommitAsync();
            }
        }

        private async Task UpdateStartMetadataAsync(IMigrationPhaseWorkload runner, CancellationToken cancellationToken)
        {
            using (var tx = this.Transaction)
            {
                await this.metadataDict.AddOrUpdateAsync(
                    tx,
                    MigrationCurrentPhase,
                    runner.Phase.ToString(),
                    (_, __) => runner.Phase.ToString(),
                    DefaultRCTimeout,
                    cancellationToken);

                await tx.CommitAsync();
            }
        }

        private async Task<long> GetEndSequenceNumberAsync(CancellationToken cancellationToken)
        {
            return await ParseLongAsync(
                () => this.ServicePartitionClient.InvokeWithRetryAsync<string>(
                async client =>
                {
                    return await client.HttpClient.GetStringAsync($"{KVSMigrationControllerName}/{GetEndSNEndpoint}");
                },
                cancellationToken),
                this.TraceId);
        }

        private async Task InvokeRejectWritesAsync(CancellationToken cancellationToken)
        {
            await this.ServicePartitionClient.InvokeWithRetryAsync(
                async client =>
                {
                    return await client.HttpClient.PutAsync($"{MigrationConstants.KVSMigrationControllerName}/{MigrationConstants.RejectWritesAPIEndpoint}", null);
                },
                cancellationToken);
        }

        private async Task<IMigrationPhaseWorkload> NextWorkloadRunnerAsync(MigrationResult currentResult, CancellationToken cancellationToken)
        {
            var endSN = await this.GetEndSequenceNumberAsync(cancellationToken);
            if ((currentResult.Phase == MigrationPhase.Catchup && endSN - currentResult.EndSeqNum < this.MigrationSettings.DowntimeThreshold))
            {
                await this.InvokeRejectWritesAsync(cancellationToken);
                return await this.NextWorkloadRunnerAsync(MigrationPhase.Downtime, cancellationToken);
            }

            return await this.NextWorkloadRunnerAsync(currentResult.Phase + 1, cancellationToken);
        }

        private async Task<IMigrationPhaseWorkload> NextWorkloadRunnerAsync(MigrationPhase currentPhase, CancellationToken cancellationToken)
        {
            IMigrationPhaseWorkload migrationWorkload = null;
            using (var tx = this.Transaction)
            {
                var currentPhase1 = await ParseMigrationPhaseAsync(
                    () =>
                    {
                        return this.MetaDataDictionary.GetAsync(
                            tx,
                            MigrationCurrentPhase,
                            DefaultRCTimeout,
                            cancellationToken);
                    },
                    this.TraceId);

                if (currentPhase == MigrationPhase.None || currentPhase == MigrationPhase.Copy)
                {
                    migrationWorkload = new CopyPhaseWorkload(
                        this.StateProvider,
                        this.servicePartitionClient,
                        this.StatefulServiceContext,
                        this.MigrationSettings,
                        this.StatefulServiceInitializationParameters,
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

                    migrationWorkload = new CatchupPhaseWorkload(
                        currentIteration,
                        this.StateProvider,
                        this.servicePartitionClient,
                        this.StatefulServiceContext,
                        this.MigrationSettings,
                        this.StatefulServiceInitializationParameters,
                        this.ActorTypeInformation,
                        this.TraceId);
                }
                else if (currentPhase == MigrationPhase.Downtime)
                {
                    migrationWorkload = new DowntimeWorkload(
                        this.StateProvider,
                        this.servicePartitionClient,
                        this.StatefulServiceContext,
                        this.MigrationSettings,
                        this.StatefulServiceInitializationParameters,
                        this.ActorTypeInformation,
                        this.TraceId);
                }

                return migrationWorkload;
            }
        }

        private async Task InitializeIfRequiredAsync(CancellationToken cancellationToken)
        {
            this.metadataDict = await this.stateProvider.GetMetadataDictionaryAsync();
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
    }
}
