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

        private async Task GetDataFromKvsAndSaveToRCAsync(MigrationPhase phase, long start, long enumerationSize, int workerIdentifier, CancellationToken cancellationToken)
        {
            var apiName = phase == MigrationPhase.Copy ? MigrationConstants.EnumeratebySNEndpoint : MigrationConstants.EnumerateKeysAndTombstonesEndpoint;
            bool includeDeletes = phase == MigrationPhase.Copy;
            var keyvaluepairserializer = new DataContractSerializer(typeof(List<KeyValuePair>));

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var response = await this.servicePartitionClient.InvokeWithRetryAsync<HttpResponseMessage>(async client =>
                {
                    return await client.HttpClient.SendAsync(this.CreateKvsApiRequestMessage(start, enumerationSize, includeDeletes, apiName), HttpCompletionOption.ResponseHeadersRead);
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

        private HttpRequestMessage CreateKvsApiRequestMessage(long startSN, long enumerationSize, bool includeDeletes, string apiName)
        {
            var requestserializer = new DataContractSerializer(typeof(EnumerationRequest));
            var enumerationRequestContent = this.CreateEnumerationRequestObject(startSN, enumerationSize, includeDeletes);

            using var memoryStream = new MemoryStream();
            var binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(memoryStream);
            requestserializer.WriteObject(binaryWriter, enumerationRequestContent);
            binaryWriter.Flush();

            var byteArray = memoryStream.ToArray();
            var content = new ByteArrayContent(byteArray);
            return new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{MigrationConstants.KVSMigrationControllerName}/{apiName}"),
                Content = content,
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
                    value = Encoding.ASCII.GetBytes(MigrationStatus.Completed.ToString());
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
                    value = Encoding.ASCII.GetBytes(MigrationStatus.Completed.ToString());
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
                    value = Encoding.ASCII.GetBytes(MigrationStatus.Completed.ToString());
                    await this.metadataDict.AddOrUpdateAsync(tx, key, value, (k, v) => value);

                    await tx.CommitAsync();
                }
            }
        }

        private void GetUserSettingsOrDefault(ActorTypeInformation actorTypeInfo)
        {
            this.itemsPerEnumeration = 1024 * 16;
            this.chunkSize = 100;
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
                }
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteError("MigrationWorker", e.Message);
            }
        }
    }
}
