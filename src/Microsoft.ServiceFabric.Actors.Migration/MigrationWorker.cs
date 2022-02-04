// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
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
    using Microsoft.ServiceFabric.Actors.Migration.Models;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using static Microsoft.ServiceFabric.Actors.Migration.KVStoRCMigrationActorStateProvider;
    using static Microsoft.ServiceFabric.Actors.Migration.MigrationConstants;
    using static Microsoft.ServiceFabric.Actors.Migration.MigrationInput;
    using static Microsoft.ServiceFabric.Actors.Migration.MigrationResult;

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

        public async Task<WorkerResult> StartMigrationAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.traceId,
                        $"Starting or resuming migration worker for {this.Input.Phase} - {this.Input.Iteration} Phase - /*DUMP input*/");

            var startSN = this.workerInput.StartSeqNum;
            var endSN = startSN + this.migrationSettings.ItemsPerEnumeration;
            long keysMigrated = -1L;

            while (startSN < this.workerInput.EndSeqNum)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (endSN < this.workerInput.EndSeqNum)
                {
                    endSN = this.workerInput.EndSeqNum;
                }

                keysMigrated = await this.FetchAndSaveAsync(startSN, endSN, cancellationToken);
                startSN = endSN + 1;
                endSN += this.migrationSettings.ItemsPerEnumeration;
            }

            ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.traceId,
                        $"Completed migration worker for {this.Input.Phase} - {this.Input.Iteration} Phase - /*DUMP result*/");

            return new WorkerResult
            {
                EndDateTimeUTC = DateTime.UtcNow,
                EndSeqNum = this.Input.EndSeqNum,
                Iteration = this.Input.Iteration,
                LastAppliedSeqNum = this.Input.LastAppliedSeqNum,
                NoOfKeysMigrated = keysMigrated,
                Phase = this.Input.Phase,
                StartDateTimeUTC = this.Input.StartDateTimeUTC,
                StartSeqNum = this.Input.StartSeqNum,
                Status = MigrationState.Completed,
                WorkerId = this.Input.WorkerId,
            };
        }

        private async Task<long> FetchAndSaveAsync(long startSN, long snCount, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                $"Enumerating from KVS - StartSN: {startSN}, SNCount: {snCount}");
            var keyvaluepairserializer = new DataContractSerializer(typeof(List<KeyValuePair>));
            long keysMigrated = -1L;

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
                                keysMigrated = await this.stateProvider.SaveStateAsync(kvsData, cancellationToken);
                                using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                                {
                                    await this.metadataDict.AddOrUpdateAsync(
                                        tx,
                                        Key(PhaseWorkerLastAppliedSeqNum, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                                        keysMigrated.ToString(),
                                        (_, __) => keysMigrated.ToString(),
                                        DefaultRCTimeout,
                                        cancellationToken);

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
