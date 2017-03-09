// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Data;
    using SR = Microsoft.ServiceFabric.Actors.SR;

    internal class NullActorStateProvider : IActorStateProvider, IStateProvider, IActorStateProviderInternal
    {
        private const string TraceType = "NullActorStateProvider";
        private const string ActorKeyPrefix = "Actor";
        private const string ReminderKeyPrefix = "Reminder";

        private readonly ConcurrentDictionary<string, object> stateDictionary;
        private readonly ActorStateProviderHelper actorStateProviderHelper;

        private string traceId;
        private ReplicaRole currentRole;
        private StatefulServiceInitializationParameters initParams;
        private IStateReplicator2 replicator;
        private IStatefulServicePartition servicePartition;
        private ActorTypeInformation actorTypeInformation;
        private Func<CancellationToken, Task<bool>> onDataLoFunc;

        public NullActorStateProvider()
        {
            this.currentRole = ReplicaRole.Unknown;
            this.stateDictionary = new ConcurrentDictionary<string, object>();
            this.actorStateProviderHelper = new ActorStateProviderHelper(this);
        }

        #region IActorStateProvider

        void IActorStateProvider.Initialize(ActorTypeInformation actorTypeInfo)
        {
            this.actorTypeInformation = actorTypeInfo;
        }

        Task IActorStateProvider.ActorActivatedAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            var key = ActorStateProviderHelper.CreateActorPresenceStorageKey(actorId);
            this.stateDictionary.TryAdd(key, null);

            return Task.FromResult(true);
        }
        
        Task IActorStateProvider.ReminderCallbackCompletedAsync(ActorId actorId, IActorReminder reminder, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        Task<T> IActorStateProvider.LoadStateAsync<T>(ActorId actorId, string stateName, CancellationToken cancellationToken)
        {
            Requires.Argument("stateName", stateName).NotNull();

            var key = CreateActorStorageKey(actorId, stateName);

            object value;
            if (this.stateDictionary.TryGetValue(key, out value))
            {
                return Task.FromResult((T) value);
            }

            throw new KeyNotFoundException(string.Format(SR.ErrorNamedActorStateNotFound, stateName));
        }

        Task IActorStateProvider.SaveStateAsync(ActorId actorId, IReadOnlyCollection<ActorStateChange> stateChanges, CancellationToken cancellationToken)
        {
            foreach (var stateChange in stateChanges)
            {
                var key = CreateActorStorageKey(actorId, stateChange.StateName);

                switch (stateChange.ChangeKind)
                {
                    case StateChangeKind.Add:
                    case StateChangeKind.Update:
                        this.stateDictionary.AddOrUpdate(key, stateChange.Value, (k, v) => stateChange.Value);
                        break;
                    case StateChangeKind.Remove:
                        object value;
                        this.stateDictionary.TryRemove(key, out value);
                        break;
                }
            }

            return Task.FromResult(true);
        }

        Task<bool> IActorStateProvider.ContainsStateAsync(ActorId actorId, string stateName, CancellationToken cancellationToken)
        {
            Requires.Argument("stateName", stateName).NotNull();

            var key = CreateActorStorageKey(actorId, stateName);
            return Task.FromResult(this.stateDictionary.ContainsKey(key));
        }

        Task IActorStateProvider.RemoveActorAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            var actorStorageKeyPrefix = CreateActorStorageKeyPrefix(actorId, string.Empty);
            var reminderStorgaeKeyPrefix = CreateReminderStorageKeyPrefix(actorId, string.Empty);

            object value;
            foreach (var kvPair in this.stateDictionary)
            {
                if (kvPair.Key.StartsWith(actorStorageKeyPrefix) ||
                    kvPair.Key.StartsWith(reminderStorgaeKeyPrefix))
                {
                    this.stateDictionary.TryRemove(kvPair.Key, out value);
                }
            }

            // Delete actor presence key
            var key = ActorStateProviderHelper.CreateActorPresenceStorageKey(actorId);
            this.stateDictionary.TryRemove(key, out value);

            return Task.FromResult(true);
        }

        Task<IEnumerable<string>> IActorStateProvider.EnumerateStateNamesAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            var stateNameList = new List<string>();
            var storageKeyPrefix = CreateActorStorageKeyPrefix(actorId, string.Empty);

            foreach (var kvPair in this.stateDictionary)
            {
                if (kvPair.Key.StartsWith(storageKeyPrefix))
                {
                    stateNameList.Add(ExtractStateName(actorId, kvPair.Key));
                }
            }

            return Task.FromResult((IEnumerable<string>) stateNameList);
        }

        Task<PagedResult<ActorId>> IActorStateProvider.GetActorsAsync(
            int itemsCount,
            ContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            return this.actorStateProviderHelper.GetStoredActorIds(
                itemsCount,
                continuationToken,
                this.GetActorPresenceKeyEnumerator,
                storageKey => storageKey,
                cancellationToken);
        }

        Task IActorStateProvider.SaveReminderAsync(ActorId actorId, IActorReminder state, CancellationToken cancellationToken)
        {
            var key = CreateReminderStorageKey(actorId, state.Name);
            var reminderData = new ActorReminderData(actorId, state, TimeSpan.Zero);

            this.stateDictionary.AddOrUpdate(key, reminderData, (s, o) => reminderData);

            return Task.FromResult(true);
        }

        Task IActorStateProvider.DeleteReminderAsync(ActorId actorId, string reminderName, CancellationToken cancellationToken)
        {
            var key = CreateReminderStorageKey(actorId, reminderName);

            object value;
            this.stateDictionary.TryRemove(key, out value);

            return Task.FromResult(true);
        }

        Task<IActorReminderCollection> IActorStateProvider.LoadRemindersAsync(CancellationToken cancellationToken)
        {
            var reminderCollection = new ActorReminderCollection();

            foreach (var kvPair in this.stateDictionary)
            {
                if (kvPair.Key.StartsWith(ReminderKeyPrefix))
                {
                    var reminderData = kvPair.Value as ActorReminderData;
                    reminderCollection.Add(reminderData.ActorId, new ActorReminderState(reminderData, TimeSpan.Zero, null));
                }
            }

            return Task.FromResult((IActorReminderCollection)reminderCollection);
        }

        #endregion
        
        #region IStateProviderReplica

        Func<CancellationToken, Task<bool>> IStateProviderReplica.OnDataLossAsync
        {
            set { this.onDataLoFunc = value; }
        }

        void IStateProviderReplica.Initialize(StatefulServiceInitializationParameters initializationParameters)
        {
            this.initParams = initializationParameters;
            this.traceId = ActorTrace.GetTraceIdForReplica(this.initParams.PartitionId, this.initParams.ReplicaId);
        }

        Task<IReplicator> IStateProviderReplica.OpenAsync(
            ReplicaOpenMode openMode,
            IStatefulServicePartition partition,
            CancellationToken cancellationToken)
        {
            var fabricReplicator = partition.CreateReplicator(this, this.GetReplicatorSettings());
            this.replicator = fabricReplicator.StateReplicator2;
            this.servicePartition = partition;

            return Task.FromResult<IReplicator>(fabricReplicator);
        }

        Task IStateProviderReplica.ChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
        {
            this.stateDictionary.Clear();

            switch (newRole)
            {
                case ReplicaRole.IdleSecondary:
                    this.StartSecondaryCopyAndReplicationPump();
                    break;

                case ReplicaRole.ActiveSecondary:
                    // Start replication pump if we are changing from primary
                    if (this.currentRole == ReplicaRole.Primary)
                    {
                        this.StartSecondaryReplicationPump();
                    }
                    break;
            }

            this.currentRole = newRole;
            return Task.FromResult(true);
        }

        Task IStateProviderReplica.CloseAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        void IStateProviderReplica.Abort()
        {
            // No-op
        }

        Task IStateProviderReplica.BackupAsync(Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            throw new NotImplementedException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Actors.SR.ErrorMethodNotSupported,
                    "Backup",
                    this.GetType()));
        }

        Task IStateProviderReplica.BackupAsync(
            BackupOption option,
            TimeSpan timeout,
            CancellationToken cancellationToken,
            Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            throw new NotImplementedException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Actors.SR.ErrorMethodNotSupported,
                    "Backup",
                    this.GetType()));
        }

        Task IStateProviderReplica.RestoreAsync(string backupFolderPath)
        {
            throw new NotImplementedException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Actors.SR.ErrorMethodNotSupported,
                    "Restore",
                    this.GetType()));
        }

        Task IStateProviderReplica.RestoreAsync(
            string backupFolderPath,
            RestorePolicy restorePolicy,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Actors.SR.ErrorMethodNotSupported,
                    "Restore",
                    this.GetType()));
        }

        #endregion IStateProviderReplica

        #region IStateProvider

        IOperationDataStream IStateProvider.GetCopyContext()
        {
            return new NullOperationDataStream();
        }

        IOperationDataStream IStateProvider.GetCopyState(long upToSequenceNumber, IOperationDataStream copyContext)
        {
            return new NullOperationDataStream();
        }

        long IStateProvider.GetLastCommittedSequenceNumber()
        {
            return 0;
        }

        Task<bool> IStateProvider.OnDataLossAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        Task IStateProvider.UpdateEpochAsync(Epoch epoch, long previousEpochLastSequenceNumber, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(null);
        }

        #endregion IStateProvider

        #region Secondary Pump Operation

        private void StartSecondaryCopyAndReplicationPump()
        {
            this.StartSecondaryPumpOperation(true);
        }

        private void StartSecondaryReplicationPump()
        {
            this.StartSecondaryPumpOperation(false);
        }

        private void StartSecondaryPumpOperation(bool isCopy)
        {
            Task.Run(
                async () =>
                {
                    ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.traceId,
                        "Starting SecondaryPumpOperation (isCopy: {0}).",
                        isCopy);

                    var operationStream = this.GetOperationStream(isCopy);

                    try
                    {
                        var operation = await operationStream.GetOperationAsync(CancellationToken.None);

                        if (operation == null)
                        {
                            // Since we are not replicating any data, we should always get null.
                            ActorTrace.Source.WriteInfoWithId(
                                TraceType,
                                this.traceId,
                                "Reached end of operation stream (isCopy: {0}).",
                                isCopy);

                            if (isCopy)
                            {
                                // If we are doing copy operation, kick off replication pump now.
                                this.StartSecondaryPumpOperation(false);
                            }
                        }
                        else
                        {
                            // We don't expect any replication operations. It is an error if we get one.
                            ActorTrace.Source.WriteErrorWithId(
                                TraceType,
                                this.traceId,
                                "An operation was unexpectedly received while pumping operation stream (isCopy: {0}).",
                                isCopy);

                            this.servicePartition.ReportFault(FaultType.Transient);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Failure to get operation stream usually mean the replica
                        // is about to close, abort or change role to None.
                        ActorTrace.Source.WriteWarningWithId(
                            TraceType,
                            this.traceId,
                            "Error while pumping operation stream (isCopy: {0}). Exception info: {1}",
                            isCopy,
                            ex.ToString());
                    }
                });
        }

        #endregion Secondary Pump Operation

        #region Private Helper Methods

        private IOperationStream GetOperationStream(bool isCopy)
        {
            return isCopy ? this.replicator.GetCopyStream() : this.replicator.GetReplicationStream();
        }

        private ReplicatorSettings GetReplicatorSettings()
        {
            // Even though NullActorStateProvider don't replicate any state, we need
            // to keep the copy stream and the replication stream drained at all times.
            // This is required in order to unblock role changes. Hence we need to 
            // specify a valid replicator address in the settings.
            return ActorStateProviderHelper.GetActorReplicatorSettings(
                this.initParams.CodePackageActivationContext,
                this.actorTypeInformation.ImplementationType);
        }

        private static string CreateActorStorageKey(ActorId actorId, string stateName)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}", ActorKeyPrefix, actorId.GetStorageKey(), stateName);
        }

        private static string CreateActorStorageKeyPrefix(ActorId actorId, string stateNamePrefix)
        {
            return CreateActorStorageKey(actorId, stateNamePrefix);
        }

        private static string CreateReminderStorageKey(ActorId actorId, string reminderName)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}", ReminderKeyPrefix, actorId.GetStorageKey(), reminderName);
        }

        private static string CreateReminderStorageKeyPrefix(ActorId actorId, string reminderNamePrefix)
        {
            return CreateReminderStorageKey(actorId, reminderNamePrefix);
        }

        private static string ExtractStateName(ActorId actorId, string storageKey)
        {
            var storageKeyPrefix = CreateActorStorageKeyPrefix(actorId, string.Empty);
            return storageKey.Substring(storageKeyPrefix.Length);
        }

        private IEnumerator<string> GetActorPresenceKeyEnumerator()
        {
            var actorPresenceKeyList = new List<string>();

            foreach (var kvPair in this.stateDictionary)
            {
                if (kvPair.Key.StartsWith(ActorStateProviderHelper.ActorPresenceStorageKeyPrefix))
                {
                    actorPresenceKeyList.Add(kvPair.Key);
                }
            }

            return actorPresenceKeyList.GetEnumerator();
        }

        #endregion

        #region Private Helper Classes

        private class NullOperationDataStream : IOperationDataStream
        {
            public Task<OperationData> GetNextAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult<OperationData>(null);
            }
        }

        #endregion #region Private Helper Classes

        #region IActorStateProviderInternal

        string IActorStateProviderInternal.TraceType
        {
            get { return TraceType; }
        }

        string IActorStateProviderInternal.TraceId
        {
            get { return this.traceId; }
        }

        ReplicaRole IActorStateProviderInternal.CurrentReplicaRole
        {
            get { return this.currentRole; }
        }

        TimeSpan IActorStateProviderInternal.TransientErrorRetryDelay
        {
            get { return TimeSpan.Zero; }
        }

        TimeSpan IActorStateProviderInternal.CurrentLogicalTime
        {
            get { return TimeSpan.Zero; }
        }
        
        #endregion
    }
}