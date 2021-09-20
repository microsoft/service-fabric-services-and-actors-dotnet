// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Health;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;

    /// <summary>
    /// Provides an implementation of <see cref="MigrationActorStateProvider"/> which
    /// uses <see cref="ReliableCollectionsActorStateProvider"/> to store and persist the actor state.
    /// </summary>
    public class MigrationActorStateProvider :
        IActorStateProvider, VolatileLogicalTimeManager.ISnapshotHandler, IActorStateProviderInternal
    {
        private static readonly DataContractSerializer ReminderCompletedDataContractSerializer = new DataContractSerializer(typeof(ReminderCompletedData));
        private static readonly DataContractSerializer ReminderDataContractSerializer = new DataContractSerializer(typeof(ActorReminderData));
        private static readonly DataContractSerializer LogicalTimestampDataContractSerializer = new DataContractSerializer(typeof(LogicalTimestamp));

        private string traceId;
        private IStatefulServicePartition servicePartition;
        private ReliableCollectionsActorStateProvider rcStateProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationActorStateProvider"/> class.
        /// </summary>
        public MigrationActorStateProvider()
        {
            this.rcStateProvider = new ReliableCollectionsActorStateProvider();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationActorStateProvider"/> class with specified reliableCollectionsActorStateProvider
        /// </summary>
        /// <param name="reliableCollectionsActorStateProvider">
        /// The <see cref="ReliableCollectionsActorStateProvider"/> that carries out regular operations of state provider.
        /// </param>
        public MigrationActorStateProvider(ReliableCollectionsActorStateProvider reliableCollectionsActorStateProvider)
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
            get { return "MigrationActorStateProvider"; }
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
            ((IStateProviderReplica)this.rcStateProvider).Initialize(initializationParameters);
        }

        /// <inheritdoc/>
        public Task<IReplicator> OpenAsync(ReplicaOpenMode openMode, IStatefulServicePartition partition, CancellationToken cancellationToken)
        {
            this.servicePartition = partition;
            return ((IStateProviderReplica)this.rcStateProvider).OpenAsync(openMode, partition, cancellationToken);
        }

        /// <inheritdoc/>
        public Task ChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
        {
            return ((IStateProviderReplica)this.rcStateProvider).ChangeRoleAsync(newRole, cancellationToken);
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
        /// <param name="cancellationToken">
        /// Cancellation token
        /// </param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task SaveStatetoRCAsync(List<KeyValuePair<string, byte[]>> keyValuePairs, CancellationToken cancellationToken)
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
                        var index = pair.Key.LastIndexOf('_');
                        var actorId = new ActorId(pair.Key.Substring(index + 1));
                        dictionary = this.rcStateProvider.GetActorStateDictionary(actorId);
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
                        var startIndex = this.GetNthIndex(pair.Key, '_', 2);
                        var endIndex = this.GetNthIndex(pair.Key, '_', 3);
                        var actorId = new ActorId(pair.Key.Substring(startIndex + 1, endIndex - startIndex - 1));
                        dictionary = this.rcStateProvider.GetReminderDictionary(actorId);
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
    }
}
