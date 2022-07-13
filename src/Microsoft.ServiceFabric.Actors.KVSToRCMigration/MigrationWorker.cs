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
    using System.Net.Http.Headers;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration.Extensions;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration.Models;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationConstants;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationUtility;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.PhaseInput;
    using static Microsoft.ServiceFabric.Actors.Migration.PhaseResult;

    internal class MigrationWorker : WorkerBase
    {
        private static readonly string TraceType = typeof(MigrationWorker).Name;
        private static readonly DataContractJsonSerializer ResponseSerializer = new DataContractJsonSerializer(typeof(EnumerationResponse), new[] { typeof(List<KeyValuePair>) });
        private static readonly DataContractJsonSerializer Requestserializer = new DataContractJsonSerializer(typeof(EnumerationRequest));
        private ServicePartitionClient<HttpCommunicationClient> servicePartitionClient;
        private MigrationSettings migrationSettings;
        private ActorStateProviderHelper stateProviderHelper;

        public MigrationWorker(
            KVStoRCMigrationActorStateProvider stateProvider,
            ActorTypeInformation actorTypeInfo,
            ServicePartitionClient<HttpCommunicationClient> servicePartitionClient,
            MigrationSettings migrationSettings,
            WorkerInput workerInput,
            string traceId)
            : base(stateProvider, workerInput, traceId)
        {
            this.servicePartitionClient = servicePartitionClient;
            this.migrationSettings = migrationSettings;
            this.stateProviderHelper = this.StateProvider.GetInternalStateProvider().GetActorStateProviderHelper();
        }

        public override async Task<WorkerResult> StartWorkAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Starting or resuming migration worker\n Input: {this.Input.ToString()}");

            try
            {
                var startSN = this.Input.StartSeqNum;
                if (this.Input.LastAppliedSeqNum.HasValue)
                {
                    startSN = this.Input.LastAppliedSeqNum.Value + 1;
                    if (startSN > this.Input.EndSeqNum)
                    {
                        await this.stateProviderHelper.ExecuteWithRetriesAsync(
                            async () =>
                            {
                                using (var tx = this.StateProvider.GetStateManager().CreateTransaction())
                                {
                                    await this.CompleteWorkerAsync(tx, cancellationToken);
                                    await tx.CommitAsync();
                                }
                            },
                            "MigrationWorker.StartWorkAsync",
                            cancellationToken);

                        return await GetResultAsync(
                                this.stateProviderHelper,
                                () => this.StateProvider.GetStateManager().CreateTransaction(),
                                this.MetadataDict,
                                this.Input.Phase,
                                this.Input.Iteration,
                                this.Input.WorkerId,
                                this.TraceId,
                                cancellationToken);
                    }
                }

                long keysMigrated = 0L;
                while (startSN <= this.Input.EndSeqNum)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var fetchAndSaveResponse = await this.FetchAndSaveAsync(startSN, cancellationToken);
                    startSN = fetchAndSaveResponse.LastAppliedSequenceNumber + 1;
                    keysMigrated += fetchAndSaveResponse.NumberOfKeysApplied;
                }

                var result = await GetResultAsync(
                    this.stateProviderHelper,
                    () => this.StateProvider.GetStateManager().CreateTransaction(),
                    this.MetadataDict,
                    this.Input.Phase,
                    this.Input.Iteration,
                    this.Input.WorkerId,
                    this.TraceId,
                    cancellationToken);
                ActorTrace.Source.WriteInfoWithId(
                    TraceType,
                    this.TraceId,
                    $"Completed migration worker\n Result: {result.ToString()} ");

                return result;
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.TraceId,
                    $"Migration worker failed with error: {ex} \n Input: /*Dump input*/");

                throw ex;
            }
        }

        private async Task<FetchAndSaveResponse> FetchAndSaveAsync(long startSN, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Enumerating from KVS - StartSN: {startSN}");
            long keysMigrated = 0L;
            long laSN = -1;
            EnumerationResponse enumerationResponse = null;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var response = await this.servicePartitionClient.InvokeWebRequestWithRetryAsync(
                    async client =>
                    {
                        var response = await client.HttpClient.SendAsync(
                            this.CreateKvsApiRequestMessage(client.EndpointUri, startSN),
                            HttpCompletionOption.ResponseHeadersRead,
                            cancellationToken);

                        return response;
                    },
                    "FetchAndSaveAsync",
                    cancellationToken);

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        string responseLine = await streamReader.ReadLineAsync();
                        cancellationToken.ThrowIfCancellationRequested();
                        while (responseLine != null)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            enumerationResponse = SerializationUtility.Deserialize<EnumerationResponse>(ResponseSerializer, Encoding.UTF8.GetBytes(responseLine));
                            var kvsData = enumerationResponse.KeyValuePairs;
                            if (kvsData.Count > 0)
                            {
                                laSN = kvsData[kvsData.Count - 1].Version;
                                keysMigrated += await this.StateProvider.SaveStateAsync(kvsData, cancellationToken, this.Input.Phase == MigrationPhase.Copy);
                                await this.stateProviderHelper.ExecuteWithRetriesAsync(
                                    async () =>
                                    {
                                        using (var tx = this.StateProvider.GetStateManager().CreateTransaction())
                                        {
                                            await this.MetadataDict.AddOrUpdateAsync(
                                                tx,
                                                Key(PhaseWorkerNoOfKeysMigrated, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                                                keysMigrated.ToString(),
                                                (k, v) =>
                                                {
                                                    long currVal = ParseLong(v, this.TraceId);
                                                    return (currVal + keysMigrated).ToString();
                                                },
                                                DefaultRCTimeout,
                                                cancellationToken);

                                            await this.MetadataDict.AddOrUpdateAsync(
                                                tx,
                                                Key(PhaseWorkerLastAppliedSeqNum, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                                                laSN.ToString(),
                                                (_, __) => laSN.ToString(),
                                                DefaultRCTimeout,
                                                cancellationToken);

                                            if (laSN == this.Input.EndSeqNum)
                                            {
                                                await this.CompleteWorkerAsync(tx, cancellationToken);
                                            }

                                            await tx.CommitAsync();
                                        }
                                    },
                                    "MigrationWorker.FetchAndSaveAsync",
                                    cancellationToken);

                                ActorTrace.Source.WriteInfoWithId(
                                    TraceType,
                                    this.TraceId,
                                    $"Total Keys migrated - StartSN: {startSN}, KeysFetched: {kvsData.Count}, KeysMigrated: {keysMigrated}");
                            }

                            responseLine = streamReader.ReadLine();
                        }
                    }
                }

                if (enumerationResponse != null && enumerationResponse.EndSequenceNumberReached)
                {
                    await this.stateProviderHelper.ExecuteWithRetriesAsync(
                        async () =>
                        {
                            using (var tx = this.StateProvider.GetStateManager().CreateTransaction())
                            {
                                await this.MetadataDict.AddOrUpdateAsync(
                                    tx,
                                    Key(PhaseWorkerLastAppliedSeqNum, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                                    this.Input.EndSeqNum.ToString(),
                                    (_, __) => this.Input.EndSeqNum.ToString(),
                                    DefaultRCTimeout,
                                    cancellationToken);

                                await this.CompleteWorkerAsync(tx, cancellationToken);
                                await tx.CommitAsync();
                            }
                        },
                        "MigrationWorker.FetchAndSaveAsync",
                        cancellationToken);
                }

                return new FetchAndSaveResponse
                {
                    LastAppliedSequenceNumber = enumerationResponse == null || enumerationResponse.EndSequenceNumberReached ? this.Input.EndSeqNum : laSN,
                    NumberOfKeysApplied = keysMigrated,
                };
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.TraceId,
                    "Error occured while enumerating and saving data - StartSN: {0}, Exception: {1}",
                    startSN,
                    ex);
                throw ex;
            }
        }

        private EnumerationRequest CreateEnumerationRequestObject(long startSN)
        {
            var req = new EnumerationRequest
            {
                StartSequenceNumber = startSN,
                EndSequenceNumber = this.Input.EndSeqNum,
                ChunkSize = this.migrationSettings.KeyValuePairsPerChunk,
                NumberOfChunksPerEnumeration = this.migrationSettings.ChunksPerEnumeration,
                IncludeDeletes = this.Input.Phase != MigrationPhase.Copy,
                ResolveActorIdsForStateKVPairs = this.Input.Phase == MigrationPhase.Copy,
            };

            return req;
        }

        private HttpRequestMessage CreateKvsApiRequestMessage(Uri baseUri, long startSN)
        {
            var enumerationRequestContent = this.CreateEnumerationRequestObject(startSN);

            var requestBuffer = new ByteArrayContent(SerializationUtility.Serialize(Requestserializer, enumerationRequestContent));
            requestBuffer.Headers.ContentType = new MediaTypeHeaderValue("application/json")
            {
                CharSet = Encoding.UTF8.WebName,
            };

            return new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(baseUri, $"{MigrationConstants.KVSMigrationControllerName}/{MigrationConstants.EnumeratebySNEndpoint}"),
                Content = requestBuffer,
            };
        }

        internal class FetchAndSaveResponse
        {
            public long LastAppliedSequenceNumber { get; set; }

            public long NumberOfKeysApplied { get; set; }
        }
    }
}
