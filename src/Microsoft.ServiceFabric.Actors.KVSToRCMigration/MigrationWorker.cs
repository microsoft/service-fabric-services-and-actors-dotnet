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
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationConstants;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationUtility;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.PhaseInput;
    using static Microsoft.ServiceFabric.Actors.Migration.PhaseResult;

    internal class MigrationWorker : WorkerBase
    {
        private static readonly string TraceType = typeof(MigrationWorker).Name;
        private static DataContractSerializer keyvaluepairserializer = new DataContractSerializer(typeof(EnumerationResponse), new[] { typeof(List<KeyValuePair>) });
        private StatefulServiceInitializationParameters initParams;
        private ServicePartitionClient<HttpCommunicationClient> servicePartitionClient;
        private MigrationSettings migrationSettings;

        public MigrationWorker(
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
                        $"Starting or resuming migration worker\n Input: {this.Input.ToString()}");

            try
            {
                var startSN = this.Input.StartSeqNum;
                if (this.Input.LastAppliedSeqNum.HasValue)
                {
                    startSN = this.Input.LastAppliedSeqNum.Value + 1;
                    if (startSN > this.Input.EndSeqNum)
                    {
                        using (var tx = this.StateProvider.GetStateManager().CreateTransaction())
                        {
                            await this.CompleteWorkerAsync(tx, cancellationToken);
                            await tx.CommitAsync();
                        }

                        return await GetResultAsync(
                                this.MetadataDict,
                                () => this.StateProvider.GetStateManager().CreateTransaction(),
                                this.Input.Phase,
                                this.Input.Iteration,
                                this.Input.WorkerId,
                                this.TraceId,
                                cancellationToken);
                    }
                }

                var endSN = startSN + this.migrationSettings.ItemsPerEnumeration - 1;
                long keysMigrated = 0L;

                while (startSN <= this.Input.EndSeqNum)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (endSN > this.Input.EndSeqNum)
                    {
                        endSN = this.Input.EndSeqNum;
                    }

                    keysMigrated += await this.FetchAndSaveAsync(startSN, this.migrationSettings.ItemsPerEnumeration, endSN, cancellationToken);
                    startSN = endSN + 1;
                    endSN += this.migrationSettings.ItemsPerEnumeration;
                }

                var result = await GetResultAsync(
                    this.MetadataDict,
                    () => this.StateProvider.GetStateManager().CreateTransaction(),
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

        private async Task<long> FetchAndSaveAsync(long startSN, long snCount, long endSN, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.TraceId,
                $"Enumerating from KVS - StartSN: {startSN}, SNCount: {snCount}");
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

                            EnumerationResponse enumerationResponse;
                            using (Stream memoryStream = new MemoryStream())
                            {
                                byte[] data = Encoding.UTF8.GetBytes(responseLine);
                                memoryStream.Write(data, 0, data.Length);
                                memoryStream.Position = 0;

                                using (var reader = XmlDictionaryReader.CreateTextReader(memoryStream, XmlDictionaryReaderQuotas.Max))
                                {
                                    enumerationResponse = (EnumerationResponse)keyvaluepairserializer.ReadObject(reader);
                                }
                            }

                            if (enumerationResponse != null
                                && enumerationResponse.KeyValuePairs != null
                                && enumerationResponse.KeyValuePairs.Count > 0)
                            {
                                laSN = enumerationResponse.KeyValuePairs[enumerationResponse.KeyValuePairs.Count - 1].Version;
                                keysMigrated += await this.StateProvider.SaveStateAsync(enumerationResponse.KeyValuePairs, cancellationToken);

                                // Data validation
                                await this.PostHydrationValidationAsync(enumerationResponse);

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

                                ActorTrace.Source.WriteInfoWithId(
                                    TraceType,
                                    this.TraceId,
                                    $"Total Keys migrated - StartSN: {startSN}, SNCount: {snCount}, KeysFetched: {enumerationResponse.KeyValuePairs.Count}, KeysMigrated: {keysMigrated}");
                            }

                            responseLine = streamReader.ReadLine();
                        }
                    }
                }

                if (endSN == this.Input.EndSeqNum && laSN != endSN)
                {
                    // This could happen, if SN merge caused the endSN to disappear
                    using (var tx = this.StateProvider.GetStateManager().CreateTransaction())
                    {
                        await this.MetadataDict.AddOrUpdateAsync(
                            tx,
                            Key(PhaseWorkerLastAppliedSeqNum, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId),
                            endSN.ToString(),
                            (_, __) => endSN.ToString(),
                            DefaultRCTimeout,
                            cancellationToken);

                        if (endSN == this.Input.EndSeqNum)
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
                    this.TraceId,
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
            req.IncludeDeletes = this.Input.Phase != MigrationPhase.Copy;
            req.ComputeHash = this.migrationSettings.EnableDataIntegrityChecks;
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

        private async Task PostHydrationValidationAsync(EnumerationResponse enumerationResponse)
        {
            if (this.migrationSettings.EnableDataIntegrityChecks)
            {
                await this.StateProvider.ValidateDataPostMigrationAsync(enumerationResponse.KeyValuePairs, enumerationResponse.KeyHash, enumerationResponse.ValueHash);
            }
        }
    }
}
