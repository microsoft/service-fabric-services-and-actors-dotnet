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
    using System.Linq;
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
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationConstants;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.PhaseInput;
    using static Microsoft.ServiceFabric.Actors.Migration.PhaseResult;

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

        public ITransaction Transaction { get => this.StateProvider.GetStateManager().CreateTransaction(); }

        public override async Task<WorkerResult> StartWorkAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.TraceId,
                        $"#{this.Input.WorkerId}: Starting or resuming data validation worker\n Input: {this.Input.ToString()}");

            try
            {
                if (this.migrationSettings.PercentageOfMigratedDataToValidate > 0 && this.Input.EndSeqNum > 0)
                {
                    var startSN = this.Input.StartSeqNum;
                    var endSN = this.Input.EndSeqNum;
                    var allKeysToValidateCount = endSN - startSN + 1;

                    ActorTrace.Source.WriteNoiseWithId(
                        TraceType,
                        this.TraceId,
                        $"#{this.Input.WorkerId}: StartSeqNum: {startSN} EndSeqNum: {endSN}");

                    // Fetch saved migrated keys
                    var migratedKeys = new List<string>();
                    using (var tx = this.Transaction)
                    {
                        var migrationKeysMigratedChunksCount = await this.MetadataDict.TryGetValueAsync(tx, MigrationConstants.MigrationKeysMigratedChunksCount);
                        long chunksCount = 0L;
                        if (migrationKeysMigratedChunksCount.HasValue)
                        {
                            chunksCount = MigrationUtility.ParseLong(migrationKeysMigratedChunksCount.Value, this.TraceId);

                            ActorTrace.Source.WriteNoiseWithId(
                                TraceType,
                                this.TraceId,
                                $"#{this.Input.WorkerId}: chunksCount: {chunksCount}");

                            for (int i = 1; i <= chunksCount; i++)
                            {
                                var savedMigratedKeys = await this.MetadataDict.TryGetValueAsync(tx, Key(MigrationConstants.MigrationKeysMigrated, i));
                                if (savedMigratedKeys.HasValue)
                                {
                                    migratedKeys.AddRange(savedMigratedKeys.Value.Split(DefaultDelimiter).ToList());
                                }
                            }

                            ActorTrace.Source.WriteNoiseWithId(
                                TraceType,
                                this.TraceId,
                                $"#{this.Input.WorkerId}: migratedKeys: {string.Join(",", migratedKeys)}");
                        }

                        await tx.CommitAsync();
                    }

                    // Calculate and list keys to validate
                    var keysIndicesToValidate = new HashSet<int>();
                    double noOfKeysToValidate;
                    if (this.migrationSettings.PercentageOfMigratedDataToValidate < 100)
                    {
                        noOfKeysToValidate = Math.Round(migratedKeys.Count * (this.migrationSettings.PercentageOfMigratedDataToValidate / 100), 0);
                        var rand = new Random();
                        do
                        {
                            var tIndex = rand.Next(((int)startSN), ((int)endSN) + 1);
                            keysIndicesToValidate.Add(tIndex);
                        }
                        while (keysIndicesToValidate.Count < noOfKeysToValidate);
                    }
                    else
                    {
                        noOfKeysToValidate = allKeysToValidateCount;
                        for (int tIndex = (int)startSN; tIndex <= endSN; tIndex++)
                        {
                            keysIndicesToValidate.Add(tIndex);
                        }
                    }

                    keysIndicesToValidate.Remove(migratedKeys.Count);

                    ActorTrace.Source.WriteNoiseWithId(
                                TraceType,
                                this.TraceId,
                                $"#{this.Input.WorkerId}: noOfKeysToValidate: {noOfKeysToValidate} keysIndicesToValidate: {string.Join(",", keysIndicesToValidate)}");

                    // Validate value for key in KVS and RC
                    var migratedKeysChunk = new List<string>();
                    foreach (var keyIndex in keysIndicesToValidate)
                    {
                        if (!string.IsNullOrEmpty(migratedKeys[keyIndex]))
                        {
                            migratedKeysChunk.Add(migratedKeys[keyIndex]);
                        }

                        // Start validation on reaching ItemsPerEnumeration or reaching end of keysIndicesToValidate
                        if (migratedKeysChunk.Count == this.migrationSettings.ItemsPerEnumeration
                            || keysIndicesToValidate.Last() == keyIndex)
                        {
                            if (await this.GetDataFromKvsAndValidateWithRcAsync(migratedKeysChunk, this.Input.WorkerId, cancellationToken))
                            {
                                // clear and continue for next chunk
                                migratedKeysChunk.Clear();
                                continue;
                            }
                            else
                            {
                                // Exit worker if validation fails
                                using (var tx = this.Transaction)
                                {
                                    await this.AbortWorkerAsync(tx, cancellationToken);
                                    WorkerResult tresult = await GetResultAsync(this.MetadataDict, tx, this.Input.Phase, this.Input.Iteration, this.Input.WorkerId, this.TraceId, cancellationToken);
                                    await tx.CommitAsync();

                                    return tresult;
                                }
                            }
                        }
                    }
                }
                else
                {
#pragma warning disable SA1118 // Parameter should not span multiple lines
                    ActorTrace.Source.WriteInfoWithId(
                           TraceType,
                           this.TraceId,
                           $"#{this.Input.WorkerId}: No migrated data was validated. " +
                           $"PercentageOfMigratedDataToValidate: {this.migrationSettings.PercentageOfMigratedDataToValidate} " +
                           $"StartSeqNum: {this.Input.StartSeqNum} EndSeqNum: {this.Input.EndSeqNum}");
#pragma warning restore SA1118 // Parameter should not span multiple lines
                }

                using (var tx = this.Transaction)
                {
                    await this.CompleteWorkerAsync(tx, cancellationToken);
                    await tx.CommitAsync();
                }

                var result = await this.GetResultAsync(cancellationToken);
                ActorTrace.Source.WriteInfoWithId(
                           TraceType,
                           this.TraceId,
                           $"#{this.Input.WorkerId}: Completed data validation worker\n Result: {result} ");

                return result;
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                            TraceType,
                            this.TraceId,
                            $"#{this.Input.WorkerId}: Data validation worker failed with error: {ex} \n Input: /*Dump input*/");

                using (var tx = this.Transaction)
                {
                    await tx.CommitAsync();
                }

                throw ex;
            }
        }

        private async Task<bool> GetDataFromKvsAndValidateWithRcAsync(List<string> migratedKeysChunk, int workerIdentifier, CancellationToken cancellationToken)
        {
            var keyvaluepairserializer = new DataContractSerializer(typeof(List<KeyValuePair>));

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var response = await this.servicePartitionClient.InvokeWithRetryAsync<HttpResponseMessage>(async client =>
                {
                    ActorTrace.Source.WriteNoiseWithId(
                       TraceType,
                       this.TraceId,
                       $"#{this.Input.WorkerId}: migratedKeysChunk = {string.Join(DefaultDelimiter.ToString(), migratedKeysChunk)}");

                    return await client.HttpClient.SendAsync(this.CreateKvsApiRequestMessage(client.EndpointUri, migratedKeysChunk), HttpCompletionOption.ResponseHeadersRead);
                });

                response.EnsureSuccessStatusCode();
                bool result = false;
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
                                ActorTrace.Source.WriteNoiseWithId(
                                   TraceType,
                                   this.TraceId,
                                   $"#{this.Input.WorkerId}: kvsData: {string.Join(",", kvsData)} ");

                                foreach (var kv in kvsData)
                                {
                                    cancellationToken.ThrowIfCancellationRequested();
                                    var rcValue = await this.StateProvider.GetValueByKeyAsync(kv.Key, cancellationToken);

                                    if (this.StateProvider.CompareKVSandRCValue(kv.Value, rcValue, kv.Key))
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        result = false;
                                        break;
                                    }
                                }
                            }

                            if (result == false)
                            {
                                break;
                            }

                            responseLine = streamReader.ReadLine();
                        }
                    }
                }

                return result;
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
            ActorTrace.Source.WriteNoiseWithId(
                                TraceType,
                                this.TraceId,
                                $"#{this.Input.WorkerId}: Aborting Data Validation Worker");

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

        private HttpRequestMessage CreateKvsApiRequestMessage(Uri baseUri, List<string> migratedKeysChunk)
        {
            var requestserializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(List<string>));

            var stream = new MemoryStream();
            requestserializer.WriteObject(stream, migratedKeysChunk);

            var content = Encoding.UTF8.GetString(stream.ToArray());

            return new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(baseUri, $"{MigrationConstants.KVSMigrationControllerName}/{MigrationConstants.GetKVSValueByKeys}"),
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
            };
        }
    }
}
