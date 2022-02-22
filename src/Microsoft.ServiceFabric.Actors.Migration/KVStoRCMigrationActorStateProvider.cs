// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Fabric.Health;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Migration.Models;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;

    /// <summary>
    /// Provides an implementation of <see cref="KVStoRCMigrationActorStateProvider"/> which
    /// uses <see cref="ReliableCollectionsActorStateProvider"/> to store and persist the actor state.
    /// </summary>
    public class KVStoRCMigrationActorStateProvider :
        IActorStateProvider, VolatileLogicalTimeManager.ISnapshotHandler, IActorStateProviderInternal
    {
        private static readonly DataContractSerializer ReminderCompletedDataContractSerializer = new DataContractSerializer(typeof(ReminderCompletedData));
        private static readonly DataContractSerializer ReminderDataContractSerializer = new DataContractSerializer(typeof(ActorReminderData));
        private static readonly DataContractSerializer LogicalTimestampDataContractSerializer = new DataContractSerializer(typeof(LogicalTimestamp));
        private static readonly DataContractSerializer MigratedKeysDataContractSerializer = new DataContractSerializer(typeof(List<string>));

        private string traceId;
        private ReliableCollectionsActorStateProvider rcStateProvider;
        private IStatefulServicePartition servicePartition;
        private IReliableDictionary2<string, byte[]> metadataDictionary;
        private bool isMetadataDictInitialized = false;
        private Task stateProviderInitTask;
        private StatefulServiceInitializationParameters initParams;

        /// <summary>
        /// Initializes a new instance of the <see cref="KVStoRCMigrationActorStateProvider"/> class.
        /// </summary>
        public KVStoRCMigrationActorStateProvider()
        {
            this.rcStateProvider = new ReliableCollectionsActorStateProvider();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KVStoRCMigrationActorStateProvider"/> class with specified reliableCollectionsActorStateProvider
        /// </summary>
        /// <param name="reliableCollectionsActorStateProvider">
        /// The <see cref="ReliableCollectionsActorStateProvider"/> that carries out regular operations of state provider.
        /// </param>
        public KVStoRCMigrationActorStateProvider(ReliableCollectionsActorStateProvider reliableCollectionsActorStateProvider)
        {
            this.rcStateProvider = reliableCollectionsActorStateProvider;
        }

        /// <inheritdoc/>
        public Func<CancellationToken, Task> OnRestoreCompletedAsync { set => ((IStateProviderReplica2)this.rcStateProvider).OnRestoreCompletedAsync = value; }

        /// <inheritdoc/>
        public Func<CancellationToken, Task<bool>> OnDataLossAsync { set => ((IStateProviderReplica)this.rcStateProvider).OnDataLossAsync = value; }

        /// <inheritdoc/>
        public string TraceType
        {
            get { return "KVStoRCMigrationActorStateProvider"; }
        }

        /// <inheritdoc/>
        public string TraceId { get => ((IActorStateProviderInternal)this.rcStateProvider).TraceId; }

        /// <inheritdoc/>
        public ReplicaRole CurrentReplicaRole { get => ((IActorStateProviderInternal)this.rcStateProvider).CurrentReplicaRole; }

        /// <inheritdoc/>
        public TimeSpan TransientErrorRetryDelay { get => ((IActorStateProviderInternal)this.rcStateProvider).TransientErrorRetryDelay; }

        /// <inheritdoc/>
        public TimeSpan OperationTimeout { get => ((IActorStateProviderInternal)this.rcStateProvider).OperationTimeout; }

        /// <inheritdoc/>
        public TimeSpan CurrentLogicalTime { get => ((IActorStateProviderInternal)this.rcStateProvider).CurrentLogicalTime; }

        /// <inheritdoc/>
        public long RoleChangeTracker { get => ((IActorStateProviderInternal)this.rcStateProvider).RoleChangeTracker; }

        internal IStatefulServicePartition StatefulServicePartition { get => this.servicePartition; }

        /// <inheritdoc/>
        public void Initialize(ActorTypeInformation actorTypeInformation)
        {
            ((IActorStateProvider)this.rcStateProvider).Initialize(actorTypeInformation);
        }

        /// <inheritdoc/>
        public Task ActorActivatedAsync(ActorId actorId, CancellationToken cancellationToken = default)
        {
            return ((IActorStateProvider)this.rcStateProvider).ActorActivatedAsync(actorId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task ReminderCallbackCompletedAsync(ActorId actorId, IActorReminder reminder, CancellationToken cancellationToken = default)
        {
            return ((IActorStateProvider)this.rcStateProvider).ReminderCallbackCompletedAsync(actorId, reminder, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<T> LoadStateAsync<T>(ActorId actorId, string stateName, CancellationToken cancellationToken = default)
        {
            return ((IActorStateProvider)this.rcStateProvider).LoadStateAsync<T>(actorId, stateName, cancellationToken);
        }

        /// <inheritdoc/>
        public Task SaveStateAsync(ActorId actorId, IReadOnlyCollection<ActorStateChange> stateChanges, CancellationToken cancellationToken = default)
        {
            return ((IActorStateProvider)this.rcStateProvider).SaveStateAsync(actorId, stateChanges, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> ContainsStateAsync(ActorId actorId, string stateName, CancellationToken cancellationToken = default)
        {
            return ((IActorStateProvider)this.rcStateProvider).ContainsStateAsync(actorId, stateName, cancellationToken);
        }

        /// <inheritdoc/>
        public Task RemoveActorAsync(ActorId actorId, CancellationToken cancellationToken = default)
        {
            return ((IActorStateProvider)this.rcStateProvider).RemoveActorAsync(actorId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<string>> EnumerateStateNamesAsync(ActorId actorId, CancellationToken cancellationToken = default)
        {
            return ((IActorStateProvider)this.rcStateProvider).EnumerateStateNamesAsync(actorId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<PagedResult<ActorId>> GetActorsAsync(int numItemsToReturn, ContinuationToken continuationToken, CancellationToken cancellationToken)
        {
            return ((IActorStateProvider)this.rcStateProvider).GetActorsAsync(numItemsToReturn, continuationToken, cancellationToken);
        }

        /// <inheritdoc/>
        public Task SaveReminderAsync(ActorId actorId, IActorReminder reminder, CancellationToken cancellationToken = default)
        {
            return ((IActorStateProvider)this.rcStateProvider).SaveReminderAsync(actorId, reminder, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteReminderAsync(ActorId actorId, string reminderName, CancellationToken cancellationToken = default)
        {
            return ((IActorStateProvider)this.rcStateProvider).DeleteReminderAsync(actorId, reminderName, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteRemindersAsync(IReadOnlyDictionary<ActorId, IReadOnlyCollection<string>> reminderNames, CancellationToken cancellationToken = default)
        {
            return ((IActorStateProvider)this.rcStateProvider).DeleteRemindersAsync(reminderNames, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IActorReminderCollection> LoadRemindersAsync(CancellationToken cancellationToken = default)
        {
            return ((IActorStateProvider)this.rcStateProvider).LoadRemindersAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public void Initialize(StatefulServiceInitializationParameters initializationParameters)
        {
            this.traceId = ActorTrace.GetTraceIdForReplica(initializationParameters.PartitionId, initializationParameters.ReplicaId);
            this.initParams = initializationParameters;
            ((IStateProviderReplica)this.rcStateProvider).Initialize(initializationParameters);
        }

        /// <inheritdoc/>
        public Task<IReplicator> OpenAsync(ReplicaOpenMode openMode, IStatefulServicePartition partition, CancellationToken cancellationToken)
        {
            this.servicePartition = partition;
            return ((IStateProviderReplica)this.rcStateProvider).OpenAsync(openMode, partition, cancellationToken);
        }

        /// <inheritdoc/>
        async Task IStateProviderReplica.ChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
        {
            await ((IStateProviderReplica)this.rcStateProvider).ChangeRoleAsync(newRole, cancellationToken);
            if (newRole == ReplicaRole.Primary)
            {
                this.stateProviderInitTask = this.StartStateProviderInitialization(cancellationToken);
            }
            else
            {
                await this.CancelStateProviderInitializationAsync();
            }
        }

        /// <inheritdoc/>
        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return ((IStateProviderReplica)this.rcStateProvider).CloseAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public void Abort()
        {
            ((IStateProviderReplica)this.rcStateProvider).Abort();
        }

        /// <inheritdoc/>
        public Task BackupAsync(Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            return ((IStateProviderReplica)this.rcStateProvider).BackupAsync(backupCallback);
        }

        /// <inheritdoc/>
        public Task BackupAsync(BackupOption option, TimeSpan timeout, CancellationToken cancellationToken, Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            return ((IStateProviderReplica)this.rcStateProvider).BackupAsync(option, timeout, cancellationToken, backupCallback);
        }

        /// <inheritdoc/>
        public Task RestoreAsync(string backupFolderPath)
        {
            return ((IStateProviderReplica)this.rcStateProvider).RestoreAsync(backupFolderPath);
        }

        /// <inheritdoc/>
        public Task RestoreAsync(string backupFolderPath, RestorePolicy restorePolicy, CancellationToken cancellationToken)
        {
            return ((IStateProviderReplica)this.rcStateProvider).RestoreAsync(backupFolderPath, restorePolicy, cancellationToken);
        }

        /// <inheritdoc/>
        public Task OnSnapshotAsync(TimeSpan currentLogicalTime)
        {
            return ((VolatileLogicalTimeManager.ISnapshotHandler)this.rcStateProvider).OnSnapshotAsync(currentLogicalTime);
        }

        /// <summary>
        /// Modifies data from KVS store into a suitable format for RC. Saves the modified data in RC store.
        /// </summary>
        /// <param name="keyValuePairs">
        /// Data from KVS store that needs to be modified and saved in RC
        /// </param>
        /// <param name="lastAppliedSNKey">
        /// Key to update metadata dictionary after save is completed
        /// </param>
        /// <param name="lastAppliedSNvalue">
        /// last applied sequence number to update metadata dictionary after save is complete
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token
        /// </param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task SaveStatetoRCAsync(List<KeyValuePair<string, byte[]>> keyValuePairs, string lastAppliedSNKey, byte[] lastAppliedSNvalue, CancellationToken cancellationToken)
        {
            List<string> keysMigrated = new List<string>();
            int presenceKeyCount = 0, reminderCompletedKeyCount = 0, logicalTimeCount = 0, actorStateCount = 0, reminderCount = 0;
            using (var tx = this.rcStateProvider.GetStateManager().CreateTransaction())
            {
                foreach (KeyValuePair<string, byte[]> pair in keyValuePairs)
                {
                    byte[] rcValue = { };
                    IReliableDictionary2<string, byte[]> dictionary = null;
                    if (pair.Key.StartsWith("@@"))
                    {
                        rcValue = pair.Value;
                        dictionary = this.rcStateProvider.GetActorPresenceDictionary();
                        presenceKeyCount++;
                    }
                    else if (pair.Key.StartsWith("Actor"))
                    {
                        rcValue = pair.Value;
                        var startIndex = this.GetNthIndex(pair.Key, '_', 2);
                        var endIndex = this.GetNthIndex(pair.Key, '_', 3);
                        var actorId = new ActorId(pair.Key.Substring(startIndex + 1, endIndex - startIndex - 1));
                        dictionary = this.rcStateProvider.GetActorStateDictionary(actorId);
                        actorStateCount++;
                    }
                    else if (pair.Key.StartsWith("RC@@"))
                    {
                        ReminderCompletedData reminderCompletedData = this.DeserializeReminderCompletedData(pair.Value);
                        rcValue = this.SerializeReminderCompletedData(reminderCompletedData);
                        dictionary = this.rcStateProvider.GetReminderCompletedDictionary();
                        reminderCompletedKeyCount++;
                    }
                    else if (pair.Key.Equals("Timestamp_VLTM"))
                    {
                        LogicalTimestamp logicalTimestamp = this.DeserializeLogicalTime(pair.Value);
                        rcValue = this.SerializeLogicalTime(logicalTimestamp);
                        dictionary = this.rcStateProvider.GetLogicalTimeDictionary();
                        logicalTimeCount++;
                    }
                    else if (pair.Key.StartsWith("Reminder"))
                    {
                        ActorReminderData actorReminderData = this.DeserializeReminder(pair.Value);
                        rcValue = this.SerializeReminder(actorReminderData);
                        var startIndex = this.GetNthIndex(pair.Key, '_', 2);
                        var endIndex = this.GetNthIndex(pair.Key, '_', 3);
                        var actorId = new ActorId(pair.Key.Substring(startIndex + 1, endIndex - startIndex - 1));
                        dictionary = this.rcStateProvider.GetReminderDictionary(actorId);
                        reminderCount++;
                    }
                    else if (pair.Key.StartsWith(Constants.RejectWritesKey))
                    {
                        ActorTrace.Source.WriteInfoWithId(
                            this.TraceType,
                            this.traceId,
                            "Ignoring KVS key - {0}",
                            pair.Key);
                        continue;
                    }
                    else
                    {
                        var message = "Migration Error: Failed to parse the KVS key - " + pair.Key;

                        ActorTrace.Source.WriteErrorWithId(
                            this.TraceType,
                            this.traceId,
                            message);

                        this.servicePartition.ReportPartitionHealth(new HealthInformation(this.TraceType, message, HealthState.Error));
                        continue;
                    }

                    var rcKey = this.TransformKVSKeyToRCFormat(pair.Key);
                    if (rcValue.Length > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await dictionary.AddOrUpdateAsync(tx, rcKey, rcValue, (k, v) => rcValue);
                    }

                    keysMigrated.Add(pair.Key);
                }

                await this.metadataDictionary.AddOrUpdateAsync(tx, lastAppliedSNKey, lastAppliedSNvalue, (k, v) => lastAppliedSNvalue);
                await this.AddOrUpdateMigratedKeysAsync(tx, keysMigrated, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();
                await tx.CommitAsync();

                ActorTrace.Source.WriteNoiseWithId(this.TraceType, this.traceId, string.Join(",", keysMigrated));

                string infoLevelMessage = "Migrated " + presenceKeyCount + " presence keys, "
                    + reminderCompletedKeyCount + " reminder completed keys, "
                    + logicalTimeCount + " logical timestamps, "
                    + actorStateCount + " actor states and "
                    + reminderCount + " reminders.";
                ActorTrace.Source.WriteInfoWithId(this.TraceType, this.traceId, infoLevelMessage);
            }
        }

        /// <summary>
        /// Gets list of migrated keys
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of migrated keys</returns>
        public async Task<List<string>> GetMigratedKeysAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var migratedKeys = new List<string>();
            using (var tx = this.rcStateProvider.GetStateManager().CreateTransaction())
            {
                var savedMigratedKeys = await this.metadataDictionary.TryGetValueAsync(tx, MigrationConstants.MigratedKeysKey);
                if (savedMigratedKeys.HasValue)
                {
                    using var readMemoryStream = new MemoryStream();
                    readMemoryStream.Write(savedMigratedKeys.Value, 0, savedMigratedKeys.Value.Length);
                    readMemoryStream.Position = 0;
                    using var reader = XmlDictionaryReader.CreateTextReader(readMemoryStream, XmlDictionaryReaderQuotas.Max);
                    migratedKeys = (List<string>)MigratedKeysDataContractSerializer.ReadObject(reader);
                }
            }

            return migratedKeys;
        }

        /// <summary>
        /// Gets Value for given Key from RC state provider
        /// </summary>
        /// <param name="key">Key to fetch</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>Value for given key</returns>
        public async Task<byte[]> GetValueByKeyAsync(string key, CancellationToken cancellationToken)
        {
            using (var tx = this.rcStateProvider.GetStateManager().CreateTransaction())
            {
                IReliableDictionary2<string, byte[]> dictionary = null;
                if (key.StartsWith("@@"))
                {
                    dictionary = this.rcStateProvider.GetActorPresenceDictionary();
                }
                else if (key.StartsWith("Actor"))
                {
                    var startIndex = this.GetNthIndex(key, '_', 2);
                    var endIndex = this.GetNthIndex(key, '_', 3);
                    var actorId = new ActorId(key.Substring(startIndex + 1, endIndex - startIndex - 1));
                    dictionary = this.rcStateProvider.GetActorStateDictionary(actorId);
                }
                else if (key.StartsWith("RC@@"))
                {
                    dictionary = this.rcStateProvider.GetReminderCompletedDictionary();
                }
                else if (key.Equals("Timestamp_VLTM"))
                {
                    dictionary = this.rcStateProvider.GetLogicalTimeDictionary();
                }
                else if (key.StartsWith("Reminder"))
                {
                    var startIndex = this.GetNthIndex(key, '_', 2);
                    var endIndex = this.GetNthIndex(key, '_', 3);
                    var actorId = new ActorId(key.Substring(startIndex + 1, endIndex - startIndex - 1));
                    dictionary = this.rcStateProvider.GetReminderDictionary(actorId);
                }
                else
                {
                    var message = "Migration Validation Error: Failed to parse the KVS key - " + key;

                    ActorTrace.Source.WriteErrorWithId(
                        this.TraceType,
                        this.traceId,
                        message);

                    this.servicePartition.ReportPartitionHealth(new HealthInformation(this.TraceType, message, HealthState.Error));
                }

                var rcKey = this.TransformKVSKeyToRCFormat(key);
                cancellationToken.ThrowIfCancellationRequested();
                var rcValue = await dictionary.TryGetValueAsync(tx, rcKey);
                if (rcValue.HasValue)
                {
                    return rcValue.Value;
                }

                return new byte[] { };
            }
        }

        /// <summary>
        /// Compares kvsvalue to rcvalue
        /// </summary>
        /// <param name="kvsValue">Value in KVS</param>
        /// <param name="rcValue">Value in RC</param>
        /// <param name="key">Migrated Key</param>
        /// <returns>True or False depending on comparision</returns>
        public bool CompareKVSandRCValue(byte[] kvsValue, byte[] rcValue, string key)
        {
            if (key.StartsWith("@@"))
            {
                return kvsValue == rcValue;
            }
            else if (key.StartsWith("Actor"))
            {
                return kvsValue == rcValue;
            }
            else if (key.StartsWith("RC@@"))
            {
                ReminderCompletedData kvsReminderCompletedData = this.DeserializeReminderCompletedData(kvsValue);
                ReminderCompletedData rcReminderCompletedData = ReminderCompletedDataSerializer.Deserialize(rcValue);
                return kvsReminderCompletedData.UtcTime == rcReminderCompletedData.UtcTime
                    && kvsReminderCompletedData.LogicalTime == rcReminderCompletedData.LogicalTime;
            }
            else if (key.Equals("Timestamp_VLTM"))
            {
                LogicalTimestamp kvsLogicalTimestamp = this.DeserializeLogicalTime(kvsValue);
                LogicalTimestamp rcLogicalTimestamp = LogicalTimestampSerializer.Deserialize(rcValue);
                return kvsLogicalTimestamp.Timestamp == rcLogicalTimestamp.Timestamp;
            }
            else if (key.StartsWith("Reminder"))
            {
                ActorReminderData kvsActorReminderData = this.DeserializeReminder(kvsValue);
                ActorReminderData rcActorReminderData = ActorReminderDataSerializer.Deserialize(rcValue);
                return kvsActorReminderData.ActorId == rcActorReminderData.ActorId
                    && kvsActorReminderData.Name == rcActorReminderData.Name
                    && kvsActorReminderData.DueTime == rcActorReminderData.DueTime
                    && kvsActorReminderData.Period == rcActorReminderData.Period
                    && kvsActorReminderData.State == rcActorReminderData.State
                    && kvsActorReminderData.LogicalCreationTime == rcActorReminderData.LogicalCreationTime;
            }
            else
            {
                var message = "Migration Error: Failed to parse the KVS key - " + key;

                ActorTrace.Source.WriteErrorWithId(
                    this.TraceType,
                    this.traceId,
                    message);

                this.servicePartition.ReportPartitionHealth(new HealthInformation(this.TraceType, message, HealthState.Error));
            }

            return false;
        }

        internal async Task AddOrUpdateMigratedKeysAsync(ITransaction tx, List<string> keysMigrated, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ActorTrace.Source.WriteNoiseWithId(this.TraceType, this.traceId, "New keys migrated [{0}]", string.Join(",", keysMigrated));

            var savedMigratedKeys = await this.metadataDictionary.TryGetValueAsync(tx, MigrationConstants.MigratedKeysKey);
            var savedMigratedKeysData = new List<string>();
            if (savedMigratedKeys.HasValue)
            {
                using var readMemoryStream = new MemoryStream();
                readMemoryStream.Write(savedMigratedKeys.Value, 0, savedMigratedKeys.Value.Length);
                readMemoryStream.Position = 0;
                using var reader = XmlDictionaryReader.CreateTextReader(readMemoryStream, XmlDictionaryReaderQuotas.Max);
                savedMigratedKeysData = (List<string>)MigratedKeysDataContractSerializer.ReadObject(reader);

                ActorTrace.Source.WriteNoiseWithId(this.TraceType, this.traceId, "Previously Saved Migrated Keys [{0}]", string.Join(",", savedMigratedKeysData));
            }

            savedMigratedKeysData.AddRange(keysMigrated);

            using var writeMemoryStream = new MemoryStream();
            var binaryWriter = XmlDictionaryWriter.CreateTextWriter(writeMemoryStream);
            MigratedKeysDataContractSerializer.WriteObject(binaryWriter, savedMigratedKeysData);
            binaryWriter.Flush();
            var savedMigratedKeysBytes = writeMemoryStream.ToArray();

            cancellationToken.ThrowIfCancellationRequested();
            await this.metadataDictionary.AddOrUpdateAsync(tx, MigrationConstants.MigratedKeysKey, savedMigratedKeysBytes, (k, v) => savedMigratedKeysBytes);

            ActorTrace.Source.WriteNoiseWithId(this.TraceType, this.traceId, "Final Saved Migrated Keys [{0}]", string.Join(",", savedMigratedKeysData));
        }

        internal async Task<IReliableDictionary2<string, byte[]>> GetMetadataDictionaryAsync()
        {
            await this.stateProviderInitTask;
            return this.metadataDictionary;
        }

        internal StatefulServiceInitializationParameters GetInitParams()
        {
            return this.initParams;
        }

        internal IReliableStateManagerReplica2 GetStateManager()
        {
            return this.rcStateProvider.GetStateManager();
        }

        internal async Task<MigrationState> GetMigrationStateAsync()
        {
            var metaDataDictionary = await this.GetMetadataDictionaryAsync();
            ConditionalValue<byte[]> migrationStateValue;
            using (var tx = this.GetStateManager().CreateTransaction())
            {
                migrationStateValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.MigrationStateKey);
            }

            if (migrationStateValue.HasValue)
            {
                if (MigrationState.TryParse(Encoding.ASCII.GetString(migrationStateValue.Value), out MigrationState migrationState))
                {
                    return migrationState;
                }
            }

            return MigrationState.Uninitialized;
        }

        internal async Task<MigrationStatus> GetMigrationStatusAsync(CancellationToken cancellationToken)
        {
            var metaDataDictionary = await this.GetMetadataDictionaryAsync();
            var migrationStatus = new MigrationStatus();
            var workerStatuses = new List<WorkerStatus>();

            migrationStatus.ParitionId = this.servicePartition.PartitionInfo.Id;

            try
            {
                ConditionalValue<byte[]> migrationPhaseValue;
                cancellationToken.ThrowIfCancellationRequested();
                using (var tx = this.GetStateManager().CreateTransaction())
                {
                    migrationPhaseValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.MigrationPhaseKey);
                }

                if (migrationPhaseValue.HasValue)
                {
                    if (MigrationPhase.TryParse(Encoding.ASCII.GetString(migrationPhaseValue.Value), out MigrationPhase phase))
                    {
                        migrationStatus.CurrentMigrationPhase = phase.ToString();
                    }

                    ConditionalValue<byte[]> migrationStartTimeUtcValue, currentMigrationPhaseStartTimeUtcValue;
                    ConditionalValue<byte[]> lastAppliedSNValue;

                    cancellationToken.ThrowIfCancellationRequested();
                    using (var tx = this.GetStateManager().CreateTransaction())
                    {
                        migrationStartTimeUtcValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.MigrationStartTimeUtcKey);
                        currentMigrationPhaseStartTimeUtcValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.CurrentMigrationPhaseStartTimeUtcKey);
                    }

                    migrationStatus.MigrationStartTimeUtc = DateTime.Parse(Encoding.ASCII.GetString(migrationStartTimeUtcValue.Value));
                    migrationStatus.CurrentMigrationPhaseStartTimeUtc = DateTime.Parse(Encoding.ASCII.GetString(migrationStartTimeUtcValue.Value));

                    switch (phase)
                    {
                        case MigrationPhase.Copy:
                            ConditionalValue<byte[]> workerCountValue, endSNValue;
                            cancellationToken.ThrowIfCancellationRequested();
                            using (var tx = this.GetStateManager().CreateTransaction())
                            {
                                workerCountValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.CopyWorkerCountKey);
                                endSNValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.CopyPhaseEndSNKey);
                            }

                            migrationStatus.KVS_LSN = long.Parse(Encoding.ASCII.GetString(endSNValue.Value));

                            int workerCount = int.Parse(Encoding.ASCII.GetString(workerCountValue.Value));
                            for (int workerNo = 0; workerNo < workerCount; workerNo++)
                            {
                                ConditionalValue<byte[]> startSNMetadataValue, lastAppliedSNMetadataValue;
                                cancellationToken.ThrowIfCancellationRequested();
                                using (var tx = this.GetStateManager().CreateTransaction())
                                {
                                    startSNMetadataValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.GetCopyWorkerStartSNKey(workerNo));
                                    lastAppliedSNMetadataValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.GetCopyWorkerLastAppliedSNKey(workerNo));
                                }

                                migrationStatus.WorkerStatuses.Add(new WorkerStatus
                                {
                                    WorkerId = "CopyWorker_" + workerNo.ToString(),
                                    FirstAppliedSeqNum = long.Parse(Encoding.ASCII.GetString(startSNMetadataValue.Value)),
                                    LastAppliedSeqNum = long.Parse(Encoding.ASCII.GetString(lastAppliedSNMetadataValue.Value)),
                                });
                            }

                            break;
                        case MigrationPhase.Catchup:
                            ConditionalValue<byte[]> iterationValue, startSNValue;
                            cancellationToken.ThrowIfCancellationRequested();
                            using (var tx = this.GetStateManager().CreateTransaction())
                            {
                                iterationValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.CatchupIterationKey);
                                startSNValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.CatchupStartSNKey);
                            }

                            migrationStatus.KVS_LSN = long.Parse(Encoding.ASCII.GetString(startSNValue.Value));

                            var workerStatus = new WorkerStatus();
                            workerStatus.WorkerId = "Catchup Worker";
                            workerStatus.FirstAppliedSeqNum = migrationStatus.KVS_LSN;

                            if (iterationValue.HasValue)
                            {
                                var iterationCount = int.Parse(Encoding.ASCII.GetString(iterationValue.Value));
                                cancellationToken.ThrowIfCancellationRequested();
                                using (var tx = this.GetStateManager().CreateTransaction())
                                {
                                    lastAppliedSNValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.GetCatchupWorkerLastAppliedSNKey(iterationCount));
                                }

                                workerStatus.LastAppliedSeqNum = long.Parse(Encoding.ASCII.GetString(lastAppliedSNValue.Value));
                            }

                            migrationStatus.WorkerStatuses.Add(workerStatus);
                            break;
                        case MigrationPhase.Downtime:
                            ConditionalValue<byte[]> lastSNValue;
                            cancellationToken.ThrowIfCancellationRequested();
                            using (var tx = this.GetStateManager().CreateTransaction())
                            {
                                lastAppliedSNValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.DowntimeWorkerLastAppliedSNKey);
                                startSNValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.DowntimeStartSNKey);
                                lastSNValue = await metaDataDictionary.TryGetValueAsync(tx, MigrationConstants.DowntimeEndSNKey);
                            }

                            migrationStatus.KVS_LSN = long.Parse(Encoding.ASCII.GetString(lastSNValue.Value));

                            migrationStatus.WorkerStatuses.Add(new WorkerStatus
                            {
                                WorkerId = "DownTime Worker",
                                FirstAppliedSeqNum = long.Parse(Encoding.ASCII.GetString(startSNValue.Value)),
                                LastAppliedSeqNum = long.Parse(Encoding.ASCII.GetString(lastAppliedSNValue.Value)),
                            });
                            break;
                        case MigrationPhase.Completed:
                        case MigrationPhase.Uninitialized:
                        default:
                            break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (ArgumentNullException e)
            {
                ActorTrace.Source.WriteWarningWithId(this.TraceType, this.traceId, $"GetMigrationStatusAsync Failed. {e.Message} \n {e.StackTrace}");
            }
            catch (FormatException e)
            {
                ActorTrace.Source.WriteWarningWithId(this.TraceType, this.traceId, $"GetMigrationStatusAsync Failed. {e.Message} \n {e.StackTrace}");
            }
            catch (OverflowException e)
            {
                ActorTrace.Source.WriteWarningWithId(this.TraceType, this.traceId, $"GetMigrationStatusAsync Failed. {e.Message} \n {e.StackTrace}");
            }

            return migrationStatus;
        }

        private int GetNthIndex(string s, char t, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == t)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private ReminderCompletedData DeserializeReminderCompletedData(byte[] data)
        {
            using (var reader = XmlDictionaryReader.CreateBinaryReader(data, XmlDictionaryReaderQuotas.Max))
            {
                return (ReminderCompletedData)ReminderCompletedDataContractSerializer.ReadObject(reader);
            }
        }

        private byte[] SerializeReminderCompletedData(ReminderCompletedData data)
        {
            return ReminderCompletedDataSerializer.Serialize(data);
        }

        private ActorReminderData DeserializeReminder(byte[] data)
        {
            using (var reader = XmlDictionaryReader.CreateBinaryReader(data, XmlDictionaryReaderQuotas.Max))
            {
                return (ActorReminderData)ReminderDataContractSerializer.ReadObject(reader);
            }
        }

        private byte[] SerializeReminder(ActorReminderData data)
        {
            return ActorReminderDataSerializer.Serialize(data);
        }

        private LogicalTimestamp DeserializeLogicalTime(byte[] data)
        {
            using (var reader = XmlDictionaryReader.CreateBinaryReader(data, XmlDictionaryReaderQuotas.Max))
            {
                return (LogicalTimestamp)LogicalTimestampDataContractSerializer.ReadObject(reader);
            }
        }

        private byte[] SerializeLogicalTime(LogicalTimestamp data)
        {
            return LogicalTimestampSerializer.Serialize(data);
        }

        private string TransformKVSKeyToRCFormat(string key)
        {
            int firstUnderscorePosition = key.IndexOf("_");
            if (key.StartsWith("@@"))
            {
                return key.Substring(firstUnderscorePosition + 1) + "_";
            }

            return key.Substring(firstUnderscorePosition + 1);
        }

        private async Task InitializeMetadataDictAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ActorTrace.Source.WriteInfoWithId(this.TraceType, this.traceId, "Initializing metadata dictionary");

            if (this.isMetadataDictInitialized)
            {
                ActorTrace.Source.WriteInfoWithId(this.TraceType, this.traceId, "Metadata dictionary already registered.");
                return;
            }

            IReliableDictionary2<string, byte[]> metadataDict = null;

            using (var tx = this.rcStateProvider.GetStateManager().CreateTransaction())
            {
                try
                {
                    metadataDict = await this.GetOrAddDictionaryAsync(tx, MigrationConstants.MetadataDictionaryName);
                    cancellationToken.ThrowIfCancellationRequested();
                    await tx.CommitAsync();
                }
                catch (Exception e)
                {
                    ActorTrace.Source.WriteInfoWithId(this.TraceType, this.traceId, e.Message);
                }
            }

            this.metadataDictionary = metadataDict;

            Volatile.Write(ref this.isMetadataDictInitialized, true);
            ActorTrace.Source.WriteInfoWithId(this.TraceType, this.traceId, "Registering Metadata dictionary SUCCEEDED.");
        }

        private Task<IReliableDictionary2<string, byte[]>> GetOrAddDictionaryAsync(ITransaction tx, string dictionaryName)
        {
            return this.rcStateProvider.GetStateManager().GetOrAddAsync<IReliableDictionary2<string, byte[]>>(tx, dictionaryName);
        }

        private async Task WaitForWriteStatusAsync(CancellationToken cancellationToken)
        {
            var retryCount = 0;

            while (!cancellationToken.IsCancellationRequested &&
                   this.servicePartition.WriteStatus != PartitionAccessStatus.Granted)
            {
                retryCount++;
                await Task.Delay(retryCount * 500, cancellationToken);
            }
        }

        private async Task StartStateProviderInitialization(CancellationToken cancellationToken)
        {
            Exception unexpectedException = null;
            try
            {
                var stateProviderHelper = new ActorStateProviderHelper(this);
                cancellationToken.ThrowIfCancellationRequested();
                await stateProviderHelper.ExecuteWithRetriesAsync(
                    async () =>
                    {
                        await this.WaitForWriteStatusAsync(cancellationToken);
                        await this.InitializeMetadataDictAsync(cancellationToken);
                    },
                    "StartStateProviderInitialization",
                    cancellationToken);
            }
            catch (OperationCanceledException opEx)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    unexpectedException = opEx;
                }
            }
            catch (FabricObjectClosedException)
            {
                // This can happen when replica is closing. CancellationToken should get signaled.
                // Fall through and let the task check for CancellationToken.
            }
            catch (FabricNotPrimaryException)
            {
                // This replica is no more primary. CancellationToken should get signaled.
                // Fall through and let the task check for CancellationToken.
            }
            catch (Exception ex)
            {
                unexpectedException = ex;
            }

            if (unexpectedException != null)
            {
                var msgFormat = "StartStateProviderInitialization() failed due to " +
                                 "an unexpected Exception causing replica to fault: {0}";

                ActorTrace.Source.WriteErrorWithId(
                    this.TraceType,
                    this.traceId,
                    string.Format(msgFormat, unexpectedException.ToString()));

                this.servicePartition.ReportFault(FaultType.Transient);
            }
        }

        private async Task CancelStateProviderInitializationAsync()
        {
            try
            {
                await this.stateProviderInitTask;
            }
            catch (Exception ex)
            {
                var msgFormat = "StartStateProviderInitialization() failed due to " +
                                 "an unexpected Exception causing replica to fault: {0}";

                ActorTrace.Source.WriteErrorWithId(
                    this.TraceType,
                    this.traceId,
                    string.Format(msgFormat, ex.ToString()));

                this.servicePartition.ReportFault(FaultType.Transient);
            }
            finally
            {
                this.stateProviderInitTask = null;
            }
        }
    }
}
