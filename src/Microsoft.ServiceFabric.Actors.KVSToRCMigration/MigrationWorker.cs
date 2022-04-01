// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration.Models;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationConstants;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationUtility;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.PhaseInput;
    using static Microsoft.ServiceFabric.Actors.Migration.PhaseResult;

    internal class MigrationWorker
    {
        private static readonly string TraceType = typeof(MigrationWorker).Name;
        private KVStoRCMigrationActorStateProvider stateProvider;
        private IReliableDictionary2<string, string> metadataDict;
        private StatefulServiceInitializationParameters initParams;
        private ServicePartitionClient<HttpCommunicationClient> servicePartitionClient;
        private MigrationSettings migrationSettings;
        private WorkerInput workerInput;
        private string traceId;

        public MigrationWorker(
            KVStoRCMigrationActorStateProvider stateProvider,
            ActorTypeInformation actorTypeInfo,
            ServicePartitionClient<HttpCommunicationClient> servicePartitionClient,
            MigrationSettings migrationSettings,
            WorkerInput workerInput,
            string traceId)
        {
            this.stateProvider = stateProvider;
            this.initParams = this.stateProvider.GetInitParams();
            this.servicePartitionClient = servicePartitionClient;
            this.migrationSettings = migrationSettings;
            this.workerInput = workerInput;
            this.metadataDict = this.stateProvider.GetMetadataDictionaryAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            this.traceId = traceId;
        }

        public WorkerInput Input { get => this.workerInput; }

        public static async Task<WorkerResult> GetResultAsync(
           IReliableDictionary2<string, string> metadataDict,
           ITransaction tx,
           MigrationPhase migrationPhase,
           int currentIteration,
           int workerId,
           string traceId,
           CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var status = await ParseMigrationStateAsync(
                () => metadataDict.GetValueOrDefaultAsync(
                tx,
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
                tx,
                Key(PhaseWorkerStartDateTimeUTC, migrationPhase, currentIteration, workerId),
                DefaultRCTimeout,
                cancellationToken),
                traceId)).Value;

            workerResult.EndDateTimeUTC = await ParseDateTimeAsync(
                () => metadataDict.GetValueOrDefaultAsync(
                tx,
                Key(PhaseWorkerEndDateTimeUTC, migrationPhase, currentIteration, workerId),
                DefaultRCTimeout,
                cancellationToken),
                traceId);

            workerResult.StartSeqNum = (await ParseLongAsync(
                () => metadataDict.GetAsync(
                tx,
                Key(PhaseWorkerStartSeqNum, migrationPhase, currentIteration, workerId),
                DefaultRCTimeout,
                cancellationToken),
                traceId)).Value;

            workerResult.EndSeqNum = (await ParseLongAsync(
                () => metadataDict.GetAsync(
                tx,
                Key(PhaseWorkerEndSeqNum, migrationPhase, currentIteration, workerId),
                DefaultRCTimeout,
                cancellationToken),
                traceId)).Value;

            workerResult.LastAppliedSeqNum = await ParseLongAsync(
                () => metadataDict.GetValueOrDefaultAsync(
                tx,
                Key(PhaseWorkerLastAppliedSeqNum, migrationPhase, currentIteration, workerId),
                DefaultRCTimeout,
                cancellationToken),
                traceId);

            workerResult.NoOfKeysMigrated = await ParseLongAsync(
                () => metadataDict.GetValueOrDefaultAsync(
                tx,
                Key(PhaseWorkerNoOfKeysMigrated, migrationPhase, currentIteration, workerId),
                DefaultRCTimeout,
                cancellationToken),
                traceId);

            return workerResult;
        }

        public async Task<WorkerResult> StartMigrationAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.traceId,
                        $"Starting or resuming migration worker\n Input: {this.Input.ToString()}");

            try
            {
                var startSN = this.workerInput.StartSeqNum;
                if (this.workerInput.LastAppliedSeqNum.HasValue)
                {
                    startSN = this.workerInput.LastAppliedSeqNum.Value + 1;
                    if (startSN > this.workerInput.EndSeqNum)
                    {
                        using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                        {
                            await this.CompleteWorkerAsync(tx, cancellationToken);
                            WorkerResult tresult = await GetResultAsync(this.metadataDict, tx, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId, this.traceId, cancellationToken);
                            await tx.CommitAsync();

                            return tresult;
                        }
                    }
                }

                var endSN = startSN + this.migrationSettings.ItemsPerEnumeration - 1;
                long keysMigrated = 0L;

                while (startSN <= this.workerInput.EndSeqNum)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (endSN > this.workerInput.EndSeqNum)
                    {
                        endSN = this.workerInput.EndSeqNum;
                    }

                    keysMigrated += await this.FetchAndSaveAsync(startSN, this.migrationSettings.ItemsPerEnumeration, endSN, cancellationToken);
                    startSN = endSN + 1;
                    endSN += this.migrationSettings.ItemsPerEnumeration;
                }

                var result = await this.GetResultAsync(cancellationToken);
                ActorTrace.Source.WriteInfoWithId(
                           TraceType,
                           this.traceId,
                           $"Completed migration worker\n Result: {result.ToString()} ");

                return result;
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                            TraceType,
                            this.traceId,
                            $"Migration worker failed with error: {ex} \n Input: /*Dump input*/");

                throw ex;
            }
        }

        private async Task<WorkerResult> GetResultAsync(CancellationToken cancellationToken)
        {
            WorkerResult result;
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                result = await GetResultAsync(this.metadataDict, tx, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId, this.traceId, cancellationToken);
                await tx.CommitAsync();
            }

            return result;
        }

        private async Task<long> FetchAndSaveAsync(long startSN, long snCount, long endSN, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                $"Enumerating from KVS - StartSN: {startSN}, SNCount: {snCount}");
            var keyvaluepairserializer = new DataContractSerializer(typeof(List<KeyValuePair>));
            long keysMigrated = 0L;
            long laSN = -1;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var response = await this.servicePartitionClient.InvokeWithRetryAsync<HttpResponseMessage>(async client =>
                {
                    return await client.HttpClient.SendAsync(this.CreateKvsApiRequestMessage(client.EndpointUri, startSN, snCount), HttpCompletionOption.ResponseHeadersRead);
                });

                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        string responseLine = streamReader.ReadLine();
                        cancellationToken.ThrowIfCancellationRequested();
                        while (responseLine != null)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            List<KeyValuePair> kvsData = new List<KeyValuePair>();
                            using (Stream memoryStream = new MemoryStream())
                            {
                                byte[] data = Encoding.UTF8.GetBytes(responseLine);
                                memoryStream.Write(data, 0, data.Length);
                                memoryStream.Position = 0;

                                using (var reader = XmlDictionaryReader.CreateTextReader(memoryStream, XmlDictionaryReaderQuotas.Max))
                                {
                                    kvsData = (List<KeyValuePair>)keyvaluepairserializer.ReadObject(reader);
                                }
                            }

                            if (kvsData.Count > 0)
                            {
                                laSN = kvsData[kvsData.Count - 1].Version;
                                keysMigrated += await this.stateProvider.SaveStateAsync(kvsData, cancellationToken);
                                using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                                {
                                    await this.metadataDict.AddOrUpdateAsync(
                                        tx,
                                        Key(PhaseWorkerNoOfKeysMigrated, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                                        keysMigrated.ToString(),
                                        (k, v) =>
                                        {
                                            long currVal = ParseLong(v, this.traceId);
                                            return (currVal + keysMigrated).ToString();
                                        },
                                        DefaultRCTimeout,
                                        cancellationToken);

                                    await this.metadataDict.AddOrUpdateAsync(
                                        tx,
                                        Key(PhaseWorkerLastAppliedSeqNum, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                                        laSN.ToString(),
                                        (_, __) => laSN.ToString(),
                                        DefaultRCTimeout,
                                        cancellationToken);

                                    if (laSN == this.workerInput.EndSeqNum)
                                    {
                                        await this.CompleteWorkerAsync(tx, cancellationToken);
                                    }

                                    await tx.CommitAsync();
                                }

                                ActorTrace.Source.WriteInfoWithId(
                                    TraceType,
                                    this.traceId,
                                    $"Total Keys migrated - StartSN: {startSN}, SNCount: {snCount}, KeysFetched: {kvsData.Count}, KeysMigrated: {keysMigrated}");
                            }

                            responseLine = streamReader.ReadLine();
                        }
                    }
                }

                if (endSN == this.workerInput.EndSeqNum && laSN != endSN)
                {
                    // This could happen, if SN merge caused the endSN to disappear
                    using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                    {
                        await this.metadataDict.AddOrUpdateAsync(
                            tx,
                            Key(PhaseWorkerLastAppliedSeqNum, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                            endSN.ToString(),
                            (_, __) => endSN.ToString(),
                            DefaultRCTimeout,
                            cancellationToken);

                        if (endSN == this.workerInput.EndSeqNum)
                        {
                            await this.CompleteWorkerAsync(tx, cancellationToken);
                        }

                        await tx.CommitAsync();
                    }
                }

                return keysMigrated;
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.traceId,
                    "Error occured while enumerating and saving data - StartSN: {0}, SNCount: {1}, Exception: {2}",
                    startSN,
                    snCount,
                    ex);
                throw ex;
            }
        }

        private async Task CompleteWorkerAsync(ITransaction tx, CancellationToken cancellationToken)
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

        private EnumerationRequest CreateEnumerationRequestObject(long startSN, long enumerationSize)
        {
            var req = new EnumerationRequest();
            req.StartSN = startSN;
            req.ChunkSize = this.migrationSettings.ItemsPerChunk;
            req.NoOfItems = enumerationSize;
            req.IncludeDeletes = this.workerInput.Phase != MigrationPhase.Copy;

            return req;
        }

        private HttpRequestMessage CreateKvsApiRequestMessage(Uri baseUri, long startSN, long enumerationSize)
        {
            var requestserializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(EnumerationRequest));
            var enumerationRequestContent = this.CreateEnumerationRequestObject(startSN, enumerationSize);

            var stream = new MemoryStream();
            requestserializer.WriteObject(stream, enumerationRequestContent);

            var content = Encoding.UTF8.GetString(stream.ToArray());

            return new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(baseUri, $"{MigrationConstants.KVSMigrationControllerName}/{MigrationConstants.EnumeratebySNEndpoint}"),
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
            };
        }
    }
}
