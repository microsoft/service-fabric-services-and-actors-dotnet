// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Management.ServiceModel;
    using System.IO;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Migration.Models;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal class MigrationWorker
    {
        private KVStoRCMigrationActorStateProvider stateProvider;
        private IReliableDictionary2<string, byte[]> metadataDict;
        private StatefulServiceInitializationParameters initParams;
        private long chunkSize;
        private long itemsPerEnumeration;
        private ServicePartitionClient<HttpCommunicationClient> servicePartitionClient;
        private float validateDataPercent;

        public MigrationWorker(KVStoRCMigrationActorStateProvider stateProvider, ActorTypeInformation actorTypeInfo, ServicePartitionClient<HttpCommunicationClient> servicePartitionClient)
        {
            this.stateProvider = stateProvider;
            this.initParams = this.stateProvider.GetInitParams();
            this.GetUserSettingsOrDefault(actorTypeInfo);
            this.servicePartitionClient = servicePartitionClient;
        }

        internal async Task StartMigrationWorker(MigrationPhase phase, long startSN, long endSN, int workerIdentifier, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfo("MigrationWorker", "Inititating migration worker " + workerIdentifier.ToString() + " in " + phase.ToString() + " phase.");
            this.metadataDict = await this.stateProvider.GetMetadataDictionaryAsync();
            if (endSN - startSN < this.itemsPerEnumeration || this.itemsPerEnumeration == -1)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await this.GetDataFromKvsAndSaveToRCAsync(phase, startSN, endSN - startSN, workerIdentifier, cancellationToken);
            }
            else
            {
                var lastUpdatedSN = startSN - 1;
                var endSNForChunk = lastUpdatedSN + this.itemsPerEnumeration;
                while (endSNForChunk <= endSN)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await this.GetDataFromKvsAndSaveToRCAsync(phase, lastUpdatedSN + 1, this.itemsPerEnumeration, workerIdentifier, cancellationToken);
                    lastUpdatedSN = endSNForChunk;
                    endSNForChunk += this.itemsPerEnumeration;
                }

                if (lastUpdatedSN < endSN)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await this.GetDataFromKvsAndSaveToRCAsync(phase, lastUpdatedSN + 1, endSN - lastUpdatedSN, workerIdentifier, cancellationToken);
                }

                ActorTrace.Source.WriteInfo("MigrationWorker", "Completed migration worker " + workerIdentifier.ToString() + " payload in " + phase.ToString() + " phase.");
                await this.UpdateMetadataOnCompletion(phase, workerIdentifier, endSN);
            }
        }

        internal async Task StartMigrationValidationWorker(List<string> migratedKeys, long startIndex, long endIndex, int workerIdentifier, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfo("MigrationWorker", "Inititating migration validation worker {0}.", workerIdentifier.ToString());
            this.metadataDict = await this.stateProvider.GetMetadataDictionaryAsync();
            cancellationToken.ThrowIfCancellationRequested();
            for (long index = startIndex; index <= endIndex; index++)
            {
                // TODO: Use PercentageOfMigratedDataToValidate to limit number of keys validated
                if (await this.GetDataFromKvsAndValidateWithRCAsync(migratedKeys[((int)index)], workerIdentifier, cancellationToken))
                {
                    continue;
                }
                else
                {
                    byte[] metadataEntry = Encoding.ASCII.GetBytes(MigrationState.Failed.ToString());
                    using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                    {
                        await this.metadataDict.AddOrUpdateAsync(tx, MigrationConstants.GetValidationWorkerStatusKey(workerIdentifier), metadataEntry, (k, v) => metadataEntry);
                        await tx.CommitAsync();
                    }

                    break;
                }
            }

            ActorTrace.Source.WriteInfo("MigrationWorker", "Completed migration Validation worker {0}.", workerIdentifier.ToString());
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
                var rcValue = await this.stateProvider.GetValueByKeyAsync(migratedKey, cancellationToken);

                if (kvsValue == rcValue)
                {
                    return true;
                }
                else
                {
                    ActorTrace.Source.WriteError("Migration Validation Worker failed for key{0}", migratedKey);
                    return false;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Exception occured while fetching and validating Kvs data with RC for worker {0}.\n{1}", workerIdentifier.ToString(), ex.Message);
                ActorTrace.Source.WriteError("MigrationValidationWorker", message);
                throw ex;
            }
        }

        private async Task GetDataFromKvsAndSaveToRCAsync(MigrationPhase phase, long start, long enumerationSize, int workerIdentifier, CancellationToken cancellationToken)
        {
            bool includeDeletes = phase == MigrationPhase.Copy;
            var keyvaluepairserializer = new DataContractSerializer(typeof(List<KeyValuePair>));

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var response = await this.servicePartitionClient.InvokeWithRetryAsync<HttpResponseMessage>(async client =>
                {
                    return await client.HttpClient.SendAsync(this.CreateKvsApiRequestMessage(client.EndpointUri, start, enumerationSize, includeDeletes, MigrationConstants.EnumeratebySNEndpoint), HttpCompletionOption.ResponseHeadersRead);
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
                                await this.SaveKvsDataInRC(kvsData, workerIdentifier, phase, cancellationToken);
                            }

                            responseLine = streamReader.ReadLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Exception occured while fetching and saving Kvs data to RC for worker " + workerIdentifier.ToString() + " in " + phase.ToString() + " phase." + ex.Message;
                ActorTrace.Source.WriteError("MigrationWorker", message);
                throw ex;
            }
        }

        private EnumerationRequest CreateEnumerationRequestObject(long startSN, long enumerationSize, bool includeDeletes)
        {
            var req = new EnumerationRequest();
            req.StartSN = startSN;
            req.ChunkSize = this.chunkSize;
            req.NoOfItems = enumerationSize;
            req.IncludeDeletes = includeDeletes;

            return req;
        }

        private HttpRequestMessage CreateKvsApiRequestMessage(Uri baseUri, long startSN, long enumerationSize, bool includeDeletes, string apiName)
        {
            var requestserializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(EnumerationRequest));
            var enumerationRequestContent = this.CreateEnumerationRequestObject(startSN, enumerationSize, includeDeletes);

            var stream = new MemoryStream();
            requestserializer.WriteObject(stream, enumerationRequestContent);

            var content = Encoding.UTF8.GetString(stream.ToArray());

            return new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(baseUri, $"{MigrationConstants.KVSMigrationControllerName}/{apiName}"),
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
            };
        }

        private async Task SaveKvsDataInRC(List<KeyValuePair> kvsData, int workerIdentifier, MigrationPhase phase, CancellationToken cancellationToken)
        {
            var endSN = kvsData[kvsData.Count - 1].Version;
            List<KeyValuePair<string, byte[]>> dataToSave = new List<KeyValuePair<string, byte[]>>();

            foreach (KeyValuePair data in kvsData)
            {
                dataToSave.Add(new KeyValuePair<string, byte[]>(data.Key, data.Value));
            }

            cancellationToken.ThrowIfCancellationRequested();
            await this.stateProvider.SaveStatetoRCAsync(
                dataToSave,
                this.GetKeyForLastAppliedSN(phase, workerIdentifier),
                this.GetLastAppliedSNByteArray(endSN),
                cancellationToken);
        }

        private string GetKeyForLastAppliedSN(MigrationPhase phase, int workerIdentifier)
        {
            string key = string.Empty;
            if (phase == MigrationPhase.Copy)
            {
                key = MigrationConstants.GetCopyWorkerLastAppliedSNKey(workerIdentifier);
            }
            else if (phase == MigrationPhase.Catchup)
            {
                key = MigrationConstants.GetCatchupWorkerLastAppliedSNKey(workerIdentifier);
            }
            else if (phase == MigrationPhase.Downtime)
            {
                key = MigrationConstants.DowntimeWorkerLastAppliedSNKey;
            }

            return key;
        }

        private byte[] GetLastAppliedSNByteArray(long lastAppliedSN)
        {
            return Encoding.ASCII.GetBytes(lastAppliedSN.ToString());
        }

        private async Task UpdateMetadataOnCompletion(MigrationPhase phase, int workerIdentifier, long lastAppliedSN)
        {
            if (phase == MigrationPhase.Copy)
            {
                using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                {
                    string key = MigrationConstants.GetCopyWorkerLastAppliedSNKey(workerIdentifier);
                    byte[] value = Encoding.ASCII.GetBytes(lastAppliedSN.ToString());
                    await this.metadataDict.AddOrUpdateAsync(tx, key, value, (k, v) => value);

                    key = MigrationConstants.GetCopyWorkerStatusKey(workerIdentifier);
                    value = Encoding.ASCII.GetBytes(MigrationState.Completed.ToString());
                    await this.metadataDict.AddOrUpdateAsync(tx, key, value, (k, v) => value);
                    await tx.CommitAsync();
                }
            }
            else if (phase == MigrationPhase.Catchup)
            {
                using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                {
                    string key = MigrationConstants.GetCatchupWorkerLastAppliedSNKey(workerIdentifier);
                    byte[] value = Encoding.ASCII.GetBytes(lastAppliedSN.ToString());
                    await this.metadataDict.AddOrUpdateAsync(tx, key, value, (k, v) => value);

                    key = MigrationConstants.GetCatchupWorkerStatusKey(workerIdentifier);
                    value = Encoding.ASCII.GetBytes(MigrationState.Completed.ToString());
                    await this.metadataDict.AddOrUpdateAsync(tx, key, value, (k, v) => value);
                    await tx.CommitAsync();
                }
            }
            else if (phase == MigrationPhase.Downtime)
            {
                using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                {
                    string key = MigrationConstants.DowntimeWorkerLastAppliedSNKey;
                    byte[] value = Encoding.ASCII.GetBytes(lastAppliedSN.ToString());
                    await this.metadataDict.AddOrUpdateAsync(tx, key, value, (k, v) => value);

                    key = MigrationConstants.DowntimeWorkerStatusKey;
                    value = Encoding.ASCII.GetBytes(MigrationState.Completed.ToString());
                    await this.metadataDict.AddOrUpdateAsync(tx, key, value, (k, v) => value);

                    await tx.CommitAsync();
                }
            }
        }

        private void GetUserSettingsOrDefault(ActorTypeInformation actorTypeInfo)
        {
            this.itemsPerEnumeration = 1024 * 16;
            this.chunkSize = 100;
            this.validateDataPercent = 10.00f;

            var configPackageName = ActorNameFormat.GetConfigPackageName();
            try
            {
                var configPackageObj = this.initParams.CodePackageActivationContext.GetConfigurationPackageObject(configPackageName);
                var migrationConfigLabel = ActorNameFormat.GetMigrationConfigSectionName(actorTypeInfo.ImplementationType);
                if (configPackageObj.Settings.Sections.Contains(migrationConfigLabel))
                {
                    var migrationSettings = configPackageObj.Settings.Sections[migrationConfigLabel];

                    if (migrationSettings.Parameters.Contains("ItemsPerEnumeration"))
                    {
                        this.itemsPerEnumeration = int.Parse(migrationSettings.Parameters["ItemsPerEnumeration"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("ItemsPerChunk"))
                    {
                        this.chunkSize = int.Parse(migrationSettings.Parameters["ItemsPerChunk"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("PercentageOfMigratedDataToValidate"))
                    {
                        this.validateDataPercent = float.Parse(migrationSettings.Parameters["PercentageOfMigratedDataToValidate"].Value);
                    }
                }
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteError("MigrationWorker", e.Message);
            }
        }
    }
}
