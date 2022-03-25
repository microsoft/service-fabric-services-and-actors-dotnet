// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration.Models;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;

    /// <summary>
    /// Provides an implementation of <see cref="KVStoRCMigrationActorStateProvider"/> which
    /// uses <see cref="ReliableCollectionsActorStateProvider"/> to store and persist the actor state.
    /// </summary>
    internal class KVStoRCMigrationActorStateProvider :
        IActorStateProvider, VolatileLogicalTimeManager.ISnapshotHandler, IActorStateProviderInternal
    {
        private static readonly DataContractSerializer ReminderCompletedDataContractSerializer = new DataContractSerializer(typeof(ReminderCompletedData));
        private static readonly DataContractSerializer ReminderDataContractSerializer = new DataContractSerializer(typeof(ActorReminderData));
        private static readonly DataContractSerializer LogicalTimestampDataContractSerializer = new DataContractSerializer(typeof(LogicalTimestamp));

        private string traceId;
        private ReliableCollectionsActorStateProvider rcStateProvider;
        private IStatefulServicePartition servicePartition;
        private IReliableDictionary2<string, string> metadataDictionary;
        private bool isMetadataDictInitialized = false;
        private Task stateProviderInitTask;
        private StatefulServiceInitializationParameters initParams;
        private CancellationTokenSource stateProviderInitCts;

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
        public Task<ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>> GetRemindersAsync(int numItemsToReturn, ActorId actorId, ContinuationToken continuationToken, CancellationToken cancellationToken)
        {
            return ((IActorStateProvider)this.rcStateProvider).GetRemindersAsync(numItemsToReturn, actorId, continuationToken, cancellationToken);
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
                this.stateProviderInitCts = new CancellationTokenSource();
                this.stateProviderInitTask = this.StartStateProviderInitialization(this.stateProviderInitCts.Token);
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
        /// <param name="kvsData">
        /// Data from KVS store that needs to be modified and saved in RC
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token
        /// </param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task<long> SaveStateAsync(List<KeyValuePair> kvsData, CancellationToken cancellationToken)
        {
            // TODO: Add noise level tracing to emit each kv pair for debugging purposes.
            List<string> keysMigrated = new List<string>();
            int presenceKeyCount = 0, reminderCompletedKeyCount = 0, logicalTimeCount = 0, actorStateCount = 0, reminderCount = 0;
            long lastAppliedSN = -1;
            using (var tx = this.GetStateManager().CreateTransaction())
            {
                try
                {
                    foreach (var data in kvsData)
                    {
                        byte[] rcValue = { };
                        IReliableDictionary2<string, byte[]> dictionary = null;
                        if (data.Key.StartsWith("@@"))
                        {
                            rcValue = data.Value;
                            dictionary = this.rcStateProvider.GetActorPresenceDictionary();
                            presenceKeyCount++;
                        }
                        else if (data.Key.StartsWith("Actor"))
                        {
                            rcValue = data.Value;
                            var startIndex = this.GetNthIndex(data.Key, '_', 2);
                            var endIndex = this.GetNthIndex(data.Key, '_', 3);
                            var actorId = new ActorId(data.Key.Substring(startIndex + 1, endIndex - startIndex - 1));
                            dictionary = this.rcStateProvider.GetActorStateDictionary(actorId);
                            actorStateCount++;
                        }
                        else if (data.Key.StartsWith("RC@@"))
                        {
                            ReminderCompletedData reminderCompletedData = this.DeserializeReminderCompletedData(data.Key, data.Value);
                            rcValue = this.SerializeReminderCompletedData(data.Key, reminderCompletedData);
                            dictionary = this.rcStateProvider.GetReminderCompletedDictionary();
                            reminderCompletedKeyCount++;
                        }
                        else if (data.Key.Equals("Timestamp_VLTM"))
                        {
                            LogicalTimestamp logicalTimestamp = this.DeserializeLogicalTime(data.Key, data.Value);
                            rcValue = this.SerializeLogicalTime(data.Key, logicalTimestamp);
                            dictionary = this.rcStateProvider.GetLogicalTimeDictionary();
                            logicalTimeCount++;
                        }
                        else if (data.Key.StartsWith("Reminder"))
                        {
                            ActorReminderData actorReminderData = this.DeserializeReminder(data.Key, data.Value);
                            rcValue = this.SerializeReminder(data.Key, actorReminderData);
                            var startIndex = this.GetNthIndex(data.Key, '_', 2);
                            var endIndex = this.GetNthIndex(data.Key, '_', 3);
                            var actorId = new ActorId(data.Key.Substring(startIndex + 1, endIndex - startIndex - 1));
                            dictionary = this.rcStateProvider.GetReminderDictionary(actorId);
                            reminderCount++;
                        }
                        else if (data.Key.StartsWith(MigrationConstants.RejectWritesKey))
                        {
                            ActorTrace.Source.WriteInfoWithId(
                                this.TraceType,
                                this.traceId,
                                "Ignoring KVS key - {0}",
                                data.Key);
                            continue;
                        }
                        else
                        {
                            var message = "Migration Error: Failed to parse the KVS key - " + data.Key;

                            ActorTrace.Source.WriteInfoWithId(
                                this.TraceType,
                                this.traceId,
                                message);

                            continue;
                        }

                        var rcKey = this.TransformKVSKeyToRCFormat(data.Key);
                        if (rcValue.Length > 0)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await dictionary.AddOrUpdateAsync(tx, rcKey, rcValue, (k, v) => rcValue);
                        }

                        keysMigrated.Add(data.Key);
                        lastAppliedSN = data.Version;
                    }
                }
                catch (Exception ex)
                {
                    await this.metadataDictionary.TryAddAsync(
                       tx,
                       MigrationConstants.MigrationEndDateTimeUTC,
                       DateTime.UtcNow.ToString(),
                       MigrationConstants.DefaultRCTimeout,
                       cancellationToken);

                    await this.metadataDictionary.AddOrUpdateAsync(
                        tx,
                        MigrationConstants.MigrationCurrentStatus,
                        MigrationState.Aborted.ToString(),
                        (_, __) => MigrationState.Aborted.ToString(),
                        MigrationConstants.DefaultRCTimeout,
                        cancellationToken);

                    // Commit with the same transaction to avoid race condition during failover.
                    await tx.CommitAsync();

                    throw ex;
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

            return keysMigrated.Count;
        }

        internal async Task<IReliableDictionary2<string, string>> GetMetadataDictionaryAsync()
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

        private ReminderCompletedData DeserializeReminderCompletedData(string key, byte[] data)
        {
            using (var reader = XmlDictionaryReader.CreateBinaryReader(data, XmlDictionaryReaderQuotas.Max))
            {
                try
                {
                    var res = (ReminderCompletedData)ReminderCompletedDataContractSerializer.ReadObject(reader);

                    ActorTrace.Source.WriteNoiseWithId(
                        this.TraceType,
                        this.traceId,
                        $"Successfully deserialized Reminder Completed Data - Key : {key}, UtcTime : {res.UtcTime}, LogicalTime : {res.LogicalTime}");
                    return res;
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteErrorWithId(
                        this.TraceType,
                        this.traceId,
                        $"Failed to deserialize Reminder Completed Data - Key : {key}, ErrorMessage : {ex.Message}");

                    throw ex;
                }
            }
        }

        private byte[] SerializeReminderCompletedData(string key, ReminderCompletedData data)
        {
            try
            {
                var res = ReminderCompletedDataSerializer.Serialize(data);
                ActorTrace.Source.WriteNoiseWithId(
                    this.TraceType,
                    this.traceId,
                    $"Successfully serialized Reminder Completed Data - Key : {key}");

                return res;
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    this.TraceType,
                    this.traceId,
                    $"Failed to serialize Reminder Completed Data - Key : {key}, ErrorMessage : {ex.Message}");

                throw ex;
            }
        }

        private ActorReminderData DeserializeReminder(string key, byte[] data)
        {
            using (var reader = XmlDictionaryReader.CreateBinaryReader(data, XmlDictionaryReaderQuotas.Max))
            {
                try
                {
                    var res = (ActorReminderData)ReminderDataContractSerializer.ReadObject(reader);

                    ActorTrace.Source.WriteNoiseWithId(
                        this.TraceType,
                        this.traceId,
                        $"Successfully deserialized Reminder - Key : {key}, ActorId : {res.ActorId}, DueTime : {res.DueTime}, IsReadOnly : {res.IsReadOnly}, LogicalCreationTime : {res.LogicalCreationTime}, Name : {res.Name}, Period : {res.Period}, State : {res.State}");
                    return res;
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteErrorWithId(
                        this.TraceType,
                        this.traceId,
                        $"Failed to deserialize Reminder - Key : {key}, ErrorMessage : {ex.Message}");

                    throw ex;
                }
            }
        }

        private byte[] SerializeReminder(string key, ActorReminderData data)
        {
            try
            {
                var res = ActorReminderDataSerializer.Serialize(data);
                ActorTrace.Source.WriteNoiseWithId(
                    this.TraceType,
                    this.traceId,
                    $"Successfully serialized Reminder - Key : {key}");

                return res;
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    this.TraceType,
                    this.traceId,
                    $"Failed to serialize Reminder - Key : {key}, ErrorMessage : {ex.Message}");

                throw ex;
            }
        }

        private LogicalTimestamp DeserializeLogicalTime(string key, byte[] data)
        {
            using (var reader = XmlDictionaryReader.CreateBinaryReader(data, XmlDictionaryReaderQuotas.Max))
            {
                try
                {
                    var res = (LogicalTimestamp)LogicalTimestampDataContractSerializer.ReadObject(reader);

                    ActorTrace.Source.WriteNoiseWithId(
                        this.TraceType,
                        this.traceId,
                        $"Successfully deserialized LogicalTimestamp - Key : {key}, Timestamp : {res.Timestamp}");
                    return res;
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteErrorWithId(
                        this.TraceType,
                        this.traceId,
                        $"Failed to deserialize LogicalTimestamp - Key : {key}, ErrorMessage : {ex.Message}");

                    throw ex;
                }
            }
        }

        private byte[] SerializeLogicalTime(string key, LogicalTimestamp data)
        {
            try
            {
                var res = LogicalTimestampSerializer.Serialize(data);
                ActorTrace.Source.WriteNoiseWithId(
                    this.TraceType,
                    this.traceId,
                    $"Successfully serialized LogicalTimestamp - Key : {key}");

                return res;
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    this.TraceType,
                    this.traceId,
                    $"Failed to serialize LogicalTimestamp - Key : {key}, ErrorMessage : {ex.Message}");

                throw ex;
            }
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

            IReliableDictionary2<string, string> metadataDict = null;

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

        private Task<IReliableDictionary2<string, string>> GetOrAddDictionaryAsync(ITransaction tx, string dictionaryName)
        {
            return this.rcStateProvider.GetStateManager().GetOrAddAsync<IReliableDictionary2<string, string>>(tx, dictionaryName);
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
            if (this.stateProviderInitCts != null
                && this.stateProviderInitTask != null
                && this.stateProviderInitCts.IsCancellationRequested == false)
            {
                try
                {
                    ActorTrace.Source.WriteInfoWithId(this.TraceType, this.traceId, "Canceling state provider initialization");

                    this.stateProviderInitCts.Cancel();

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
                    this.stateProviderInitCts = null;
                    this.stateProviderInitTask = null;
                }
            }
        }
    }
}
