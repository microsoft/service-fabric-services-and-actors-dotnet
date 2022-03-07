// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration.Models;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationConstants;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.PhaseInput;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.PhaseResult;

    internal class DataValidationWorker : WorkerBase
    {
        private static readonly string TraceType = typeof(DataValidationWorker).Name;
        private StatefulServiceInitializationParameters initParams;
        private ServicePartitionClient<HttpCommunicationClient> servicePartitionClient;
        private MigrationSettings migrationSettings;

        public DataValidationWorker(
            KVStoRCMigrationActorStateProvider stateProvider,
            ActorTypeInformation actorTypeInfo,
            ServicePartitionClient<HttpCommunicationClient> servicePartitionClient,
            MigrationSettings migrationSettings,
            WorkerInput workerInput,
            string traceId)
            : base(stateProvider, workerInput, traceId)
        {
            this.initParams = this.StateProvider.GetInitParams();
            this.servicePartitionClient = servicePartitionClient;
            this.migrationSettings = migrationSettings;
        }

        public override async Task<WorkerResult> StartWorkAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.TraceId,
                        $"Starting or resuming data validation worker\n Input: {this.Input.ToString()}");

            try
            {
                var startSN = this.Input.StartSeqNum;
                var endSN = this.Input.EndSeqNum;

                var migratedKeys = new List<string>();
                using (var tx = this.StateProvider.GetStateManager().CreateTransaction())
                {
                    var savedMigratedKeys = await this.MetadataDict.TryGetValueAsync(tx, MigrationConstants.MigrationKeysMigrated);
                    if (savedMigratedKeys.HasValue)
                    {
                        migratedKeys = savedMigratedKeys.Value.Split(DefaultDelimiter).ToList();
                    }
                }

                var noOfKeysToValidate = Math.Round(migratedKeys.Count * (this.migrationSettings.PercentageOfMigratedDataToValidate / 100), 0);
                var rand = new Random();
                var validatedKeysIndices = new List<int>();

                for (long index = startSN; index <= endSN; index++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (this.migrationSettings.PercentageOfMigratedDataToValidate <= 0)
                    {
                        break;
                    }

                    var tIndex = rand.Next(((int)startSN), ((int)endSN));
                    if (validatedKeysIndices.Contains(tIndex))
                    {
                        // skip index already validated
                        continue;
                    }

                    if (await this.GetDataFromKvsAndValidateWithRCAsync(migratedKeys[(int)index], this.Input.WorkerId, cancellationToken))
                    {
                        validatedKeysIndices.Add(tIndex);

                        if (validatedKeysIndices.Count == noOfKeysToValidate)
                        {
                            break;
                        }

                        continue;
                    }
                    else
                    {
                        // Exit worker if validation fails
                        using (var tx = this.StateProvider.GetStateManager().CreateTransaction())
                        {
                            await this.AbortWorkerAsync(tx, cancellationToken);
                            WorkerResult tresult = await GetResultAsync(this.MetadataDict, tx, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId, this.TraceId, cancellationToken);
                            await tx.CommitAsync();

                            return tresult;
                        }
                    }
                }

                using (var tx = this.StateProvider.GetStateManager().CreateTransaction())
                {
                    await this.CompleteWorkerAsync(tx, cancellationToken);
                    await tx.CommitAsync();
                }

                var result = await this.GetResultAsync(cancellationToken);
                ActorTrace.Source.WriteInfoWithId(
                           TraceType,
                           this.TraceId,
                           $"Completed data validation worker\n Result: {result.ToString()} ");

                return result;
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                            TraceType,
                            this.TraceId,
                            $"Data validation worker failed with error: {ex} \n Input: /*Dump input*/");

                throw ex;
            }
        }

        private async Task<bool> GetDataFromKvsAndValidateWithRCAsync(string migratedKey, int workerIdentifier, CancellationToken cancellationToken)
        {
            var keyvaluepairserializer = new DataContractSerializer(typeof(List<KeyValuePair>));

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var kvsValue = await this.servicePartitionClient.InvokeWithRetryAsync<byte[]>(async client =>
                {
                    return await client.HttpClient.GetByteArrayAsync($"{MigrationConstants.KVSMigrationControllerName}/{MigrationConstants.GetKVSValueByKey}{migratedKey}");
                });

                cancellationToken.ThrowIfCancellationRequested();
                var rcValue = await this.StateProvider.GetValueByKeyAsync(migratedKey, cancellationToken);

                if (this.StateProvider.CompareKVSandRCValue(kvsValue, rcValue, migratedKey))
                {
                    return true;
                }
                else
                {
                    ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        this.TraceId,
                        $"Migrated data validation worker failed for key {migratedKey}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.TraceId,
                    $"Exception occured while fetching and validating Kvs data with RC for worker {workerIdentifier}.\n{ex.Message}");
                throw ex;
            }
        }

        private async Task AbortWorkerAsync(ITransaction tx, CancellationToken cancellationToken)
        {
            await this.MetadataDict.AddOrUpdateAsync(
                tx,
                Key(PhaseWorkerLastAppliedSeqNum, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                this.Input.EndSeqNum.ToString(),
                (_, __) => this.Input.EndSeqNum.ToString(),
                DefaultRCTimeout,
                cancellationToken);

            await this.MetadataDict.AddOrUpdateAsync(
                tx,
                Key(PhaseWorkerCurrentStatus, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                MigrationState.Aborted.ToString(),
                (_, __) => MigrationState.Aborted.ToString(),
                DefaultRCTimeout,
                cancellationToken);

            await this.MetadataDict.AddOrUpdateAsync(
                tx,
                Key(PhaseWorkerEndDateTimeUTC, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                DateTime.UtcNow.ToString(),
                (_, v) => v,
                DefaultRCTimeout,
                cancellationToken);
        }
    }
}
