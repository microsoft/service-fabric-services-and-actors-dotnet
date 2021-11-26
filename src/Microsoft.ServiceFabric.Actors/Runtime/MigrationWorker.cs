// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
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
    using Microsoft.ServiceFabric.Data.Collections;

    internal class MigrationWorker
    {
        private static readonly HttpClient Client = new HttpClient();
        private KVStoRCMigrationActorStateProvider stateProvider;
        private IReliableDictionary2<string, byte[]> metadataDict;
        private StatefulServiceInitializationParameters initParams;
        private long chunkSize;
        private long itemsPerEnumeration;
        private string endpoint;

        public MigrationWorker(KVStoRCMigrationActorStateProvider stateProvider, ActorTypeInformation actorTypeInfo)
        {
            this.stateProvider = stateProvider;
            this.initParams = this.stateProvider.GetInitParams();
            this.GetUserSettingsOrDefault(actorTypeInfo);
        }

        internal async Task StartMigrationWorker(MigrationPhase phase, long startSN, long endSN, int workerIdentifier, CancellationToken cancellationToken)
        {
            this.metadataDict = await this.stateProvider.GetMetadataDictionaryAsync();
            if (endSN - startSN < this.itemsPerEnumeration || this.itemsPerEnumeration == -1)
            {
                var dataToMigrate = await this.GetEnumeratedDataFromKvsAsync(phase, startSN, endSN - startSN);
                await this.stateProvider.SaveStatetoRCAsync(dataToMigrate, cancellationToken);
                await this.UpdateMetadataOnCompletion(phase, workerIdentifier, endSN);
            }
            else
            {
                var lastUpdatedSN = startSN - 1;
                var endSNForChunk = lastUpdatedSN + this.itemsPerEnumeration;
                while (endSNForChunk <= endSN)
                {
                    var dataToMigrate = await this.GetEnumeratedDataFromKvsAsync(phase, lastUpdatedSN + 1, this.itemsPerEnumeration);
                    await this.stateProvider.SaveStatetoRCAsync(dataToMigrate, cancellationToken);
                    lastUpdatedSN = endSNForChunk;
                    endSNForChunk += this.itemsPerEnumeration;
                    await this.UpdateMetadataForLastAppliedSN(phase, workerIdentifier, lastUpdatedSN);
                }

                if (lastUpdatedSN < endSN)
                {
                    var dataToMigrate = await this.GetEnumeratedDataFromKvsAsync(phase, lastUpdatedSN + 1, endSN - lastUpdatedSN);
                    await this.stateProvider.SaveStatetoRCAsync(dataToMigrate, cancellationToken);
                }

                await this.UpdateMetadataOnCompletion(phase, workerIdentifier, endSN);
            }
        }

        private async Task<List<KeyValuePair<string, byte[]>>> GetEnumeratedDataFromKvsAsync(MigrationPhase phase, long start, long enumerationSize)
        {
            var apiName = phase == MigrationPhase.Copy ? MigrationConstants.EnumeratebySNEndpoint : MigrationConstants.EnumerateKeysAndTombstonesEndpoint;
            bool includeDeletes = phase == MigrationPhase.Copy;
            var requestserializer = new DataContractSerializer(typeof(EnumerationRequest));
            var keyvaluepairserializer = new DataContractSerializer(typeof(List<KeyValuePair<string, byte[]>>));

            var enumerationRequestContent = this.CreateEnumerationRequestObject(start, enumerationSize, includeDeletes);

            using var memoryStream = new MemoryStream();
            var binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(memoryStream);
            requestserializer.WriteObject(binaryWriter, enumerationRequestContent);
            binaryWriter.Flush();

            var byteArray = memoryStream.ToArray();
            var content = new ByteArrayContent(byteArray);
            var kvsApiRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(this.endpoint + apiName),
                Content = content,
            };

            var response = await Client.SendAsync(kvsApiRequest);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();

            return (List<KeyValuePair<string, byte[]>>)keyvaluepairserializer.ReadObject(stream);
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

        private async Task UpdateMetadataForLastAppliedSN(MigrationPhase phase, int workerIdentifier, long lastAppliedSN)
        {
            if (phase == MigrationPhase.Copy)
            {
                using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                {
                    string key = MigrationConstants.GetCopyWorkerLastAppliedSNKey(workerIdentifier);
                    byte[] value = Encoding.ASCII.GetBytes(lastAppliedSN.ToString());
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
                    await tx.CommitAsync();
                }
            }
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
            this.itemsPerEnumeration = -1;
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

                    if (migrationSettings.Parameters.Contains("KVSActorServiceUri"))
                    {
                        this.endpoint = migrationSettings.Parameters["KVSActorServiceUri"].Value;
                    }
                    else
                    {
                        ActorTrace.Source.WriteError("MigrationWorker", "Kvs actor service endpoint not provided in settings.");
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
