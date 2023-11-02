// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    extern alias Microsoft_ServiceFabric_Internal;

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Health;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Services.Runtime;

    using CopyCompletionCallback = System.Action<System.Fabric.KeyValueStoreEnumerator>;
    using DataLossCallback = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<bool>>;
    using FabricDirectory = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricDirectory;
    using ReleaseAssert = Microsoft_ServiceFabric_Internal::System.Fabric.Common.ReleaseAssert;
    using ReplicationCallback = System.Action<System.Collections.Generic.IEnumerator<System.Fabric.KeyValueStoreNotification>>;
    using Requires = Microsoft_ServiceFabric_Internal::System.Fabric.Common.Requires;
    using RestoreCompletedCallback = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task>;
    using SR = Microsoft.ServiceFabric.Actors.SR;

    /// <summary>
    /// Provides an implementation of <see cref="IActorStateProvider"/> which
    /// uses <see cref="KeyValueStoreReplica"/> to store and persist the actor state.
    /// </summary>
    public abstract class KvsActorStateProviderBase
        : IActorStateProvider, VolatileLogicalTimeManager.ISnapshotHandler, IActorStateProviderInternal, IInternalStatefulServiceReplica
    {
        private const string ActorStorageKeyPrefix = "Actor";
        private const string ReminderStorageKeyPrefix = "Reminder";
        private const string LogicalTimestampKey = "Timestamp_VLTM";
        private const string TraceType = "KvsActorStateProviderBase";
        private const string LocalBackupFolderName = "B";
        private const string BackupRootFolderPrefix = "kvsasp_";
        private const string KvsHealthSourceId = "KvsActorStateProvider";
        private const string BackupCallbackSlowCancellationHealthProperty = "BackupCallbackSlowCancellation";
        private const int StateProviderInitRetryDelayMilliseconds = 500;
        private static readonly byte[] ActorPresenceValue = { byte.MinValue };

        private readonly DataContractSerializer reminderSerializer;
        private readonly DataContractSerializer reminderCompletedDataSerializer;
        private readonly DataContractSerializer timestampSerializer;
        private readonly VolatileLogicalTimeManager logicalTimeManager;
        private readonly ActorStateProviderSerializer actorStateSerializer;
        private readonly ActorStateProviderHelper actorStateProviderHelper;
        private readonly ReplicatorSettings userDefinedReplicatorSettings;

        /// <summary>
        /// Used to synchronize between backup callback invocation and replica close/abort
        /// </summary>
        private readonly SemaphoreSlim backupCallbackLock;
        private ReplicaRole replicaRole;
        private IStatefulServicePartition partition;
        private string traceId;
        private Func<CancellationToken, Task<bool>> onDataLossAsyncFunction;
        private Func<CancellationToken, Task> onRestoreCompletedAsyncFunction;
        private StatefulServiceInitializationParameters initParams;
        private ActorTypeInformation actorTypeInformation;
        private KeyValueStoreReplica storeReplica;

        private KvsActorStateProviderSettings stateProviderSettings;
        private long roleChangeTracker;

        /// <summary>
        /// Ensures single backup in progress at ActorStateProvider level.
        /// This enables cleaning up the backup directory before invoking into KeyValueStoreReplica's backup.
        /// </summary>
        private int isBackupInProgress;

        private CancellationTokenSource backupCallbackCts;
        private Task<bool> backupCallbackTask;
        private bool isClosingOrAborting;
        private bool isLogicalTimeManagerInitialized;
        private CancellationTokenSource stateProviderInitCts;
        private Task stateProviderInitTask;

        internal KvsActorStateProviderBase(ReplicatorSettings replicatorSettings)
        {
            this.userDefinedReplicatorSettings = replicatorSettings;

            this.reminderSerializer = new DataContractSerializer(typeof(ActorReminderData));
            this.reminderCompletedDataSerializer = new DataContractSerializer(typeof(ReminderCompletedData));
            this.timestampSerializer = new DataContractSerializer(typeof(LogicalTimestamp));
            this.actorStateSerializer = new ActorStateProviderSerializer();

            // KVS supports logical time natively, but this feature is not currently exposed publicly.
            // Use the same time manager logic as volatile state provider in lieu of the KVS feature.
            this.logicalTimeManager = new VolatileLogicalTimeManager(this);

            this.replicaRole = ReplicaRole.Unknown;
            this.roleChangeTracker = DateTime.UtcNow.Ticks;
            this.actorStateProviderHelper = new ActorStateProviderHelper(this);
            this.isBackupInProgress = 0;
            this.backupCallbackLock = new SemaphoreSlim(1);
            this.backupCallbackCts = null;
            this.backupCallbackTask = null;
            this.isClosingOrAborting = false;
            this.isLogicalTimeManagerInitialized = false;
            this.stateProviderInitCts = null;
            this.stateProviderInitTask = null;
        }

        /// <inheritdoc/>
        string IActorStateProviderInternal.TraceType
        {
            get { return TraceType; }
        }

        /// <inheritdoc/>
        string IActorStateProviderInternal.TraceId
        {
            get { return this.traceId; }
        }

        /// <inheritdoc/>
        ReplicaRole IActorStateProviderInternal.CurrentReplicaRole
        {
            get { return this.replicaRole; }
        }

        /// <inheritdoc/>
        TimeSpan IActorStateProviderInternal.TransientErrorRetryDelay
        {
            get { return this.stateProviderSettings.TransientErrorRetryDelay; }
        }

        /// <inheritdoc/>
        TimeSpan IActorStateProviderInternal.CurrentLogicalTime
        {
            get { return this.logicalTimeManager.CurrentLogicalTime; }
        }

        /// <inheritdoc/>
        TimeSpan IActorStateProviderInternal.OperationTimeout
        {
            get { return this.stateProviderSettings.OperationTimeout; }
        }

        /// <inheritdoc/>
        long IActorStateProviderInternal.RoleChangeTracker
        {
            get
            {
                return Interlocked.Read(ref this.roleChangeTracker);
            }
        }

        /// <summary>
        /// Sets the function to be called during suspected data-loss.
        /// </summary>
        /// <value>
        /// A function representing data-loss callback function.
        /// </value>
        public Func<CancellationToken, Task<bool>> OnDataLossAsync
        {
            private get => this.onDataLossAsyncFunction;

            set
            {
                if (this.onDataLossAsyncFunction != null)
                {
                    throw new InvalidOperationException(Actors.SR.ErrorOnDataLossAsyncReset);
                }

                this.onDataLossAsyncFunction = value;
            }
        }

        /// <summary>
        /// Sets the function to be called after the partition state has been restored automatically by the system
        /// </summary>
        /// <value>
        /// A function representing on restore completed callback function.
        /// </value>
        public Func<CancellationToken, Task> OnRestoreCompletedAsync
        {
            private get => this.onRestoreCompletedAsyncFunction;

            set
            {
                if (this.onRestoreCompletedAsyncFunction != null)
                {
                    throw new InvalidOperationException(Actors.SR.ErrorOnRestoreCompletedAsyncReset);
                }

                this.onRestoreCompletedAsyncFunction = value;
            }
        }

        internal IStatefulServicePartition StatefulServicePartition { get => this.partition; }

        internal StatefulServiceInitializationParameters InitParams => this.initParams;

        internal ActorTypeInformation ActorTypeInformation => this.actorTypeInformation;

        /// <summary>
        /// Initializes the actor state provider with type information
        /// of the actor type associated with it.
        /// </summary>
        /// <param name="actorTypeInformation">Type information of the actor class</param>
        void IActorStateProvider.Initialize(ActorTypeInformation actorTypeInformation)
        {
            this.actorTypeInformation = actorTypeInformation;
        }

        /// <summary>
        /// This method is invoked as part of the activation process of the actor with the specified Id.
        /// </summary>
        /// <param name="actorId">The ID of the actor that is activated.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <returns> A task that represents the asynchronous Actor activation notification processing.</returns>
        Task IActorStateProvider.ActorActivatedAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            var key = ActorStateProviderHelper.CreateActorPresenceStorageKey(actorId);

            return this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var tx = this.storeReplica.CreateTransaction())
                    {
                        this.storeReplica.TryAdd(tx, key, ActorPresenceValue);
                        await tx.CommitAsync();
                    }
                },
                string.Format("ActorActivatedAsync[{0}]", actorId),
                cancellationToken);
        }

        /// <summary>
        /// This method is invoked when a reminder fires and finishes executing its callback
        /// <see cref="IRemindable.ReceiveReminderAsync"/> successfully.
        /// </summary>
        /// <param name="actorId">The ID of the actor which own reminder</param>
        /// <param name="reminder">The actor reminder that completed successfully.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous reminder callback completed notification processing.
        /// </returns>
        async Task IActorStateProvider.ReminderCallbackCompletedAsync(ActorId actorId, IActorReminder reminder, CancellationToken cancellationToken)
        {
            await this.EnsureLogicalTimeManagerInitializedAsync(cancellationToken);

            var key = ActorStateProviderHelper.CreateReminderCompletedStorageKey(actorId, reminder.Name);
            var data = new ReminderCompletedData(this.logicalTimeManager.CurrentLogicalTime, DateTime.UtcNow);
            var buffer = this.SerializeReminderCompletedData(data);

            await this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return this.UpdateOrAddAsync(key, buffer);
                },
                string.Format("ReminderCallbackCompletedAsync[{0}]", actorId),
                cancellationToken);
        }

        /// <summary>
        /// Loads the actor state associated with the specified state name.
        /// </summary>
        /// <typeparam name="T">The type of value of actor state associated with given state name.</typeparam>
        /// <param name="actorId">The ID of the actor for which to load the state.</param>
        /// <param name="stateName">The name of the actor state to load.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="KeyNotFoundException">The actor state associated with specified state name does not exist.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <returns>
        /// A task that represents the asynchronous load operation. The value of TResult
        /// parameter contains value of actor state associated with given state name.
        /// </returns>
        Task<T> IActorStateProvider.LoadStateAsync<T>(ActorId actorId, string stateName, CancellationToken cancellationToken)
        {
            Requires.Argument("stateName", stateName).NotNull();

            var key = CreateActorStorageKey(actorId, stateName);

            return this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var tx = this.storeReplica.CreateTransaction())
                    {
                        var value = this.storeReplica.TryGetValue(tx, key);

                        if (value != null)
                        {
                            return Task.FromResult(this.actorStateSerializer.Deserialize<T>(value));
                        }

                        throw new KeyNotFoundException(string.Format(SR.ErrorNamedActorStateNotFound, stateName));
                    }
                },
                string.Format("LoadStateAsync[{0}]", actorId),
                cancellationToken);
        }

        /// <summary>
        /// Saves the specified set of actor state changes atomically.
        /// </summary>
        /// <param name="actorId">ID of the actor for which to save the state changes.</param>
        /// <param name="stateChanges">Collection of state changes to save.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        /// <remarks>
        /// The collection of state changes should contain only one item for a given state name.
        /// The save operation will fail on trying to add an actor state which already exists
        /// or update/remove an actor state which does not exist.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        /// When <see cref="StateChangeKind"/> is <see cref="StateChangeKind.None"/>
        /// </exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task IActorStateProvider.SaveStateAsync(ActorId actorId, IReadOnlyCollection<ActorStateChange> stateChanges, CancellationToken cancellationToken)
        {
            if (stateChanges.Count == 0)
            {
                return Task.FromResult(true);
            }

            var serializedStateChanges = new List<SerializedStateChange>();

            foreach (var stateChange in stateChanges)
            {
                var key = CreateActorStorageKey(actorId, stateChange.StateName);

                byte[] buffer = null;
                if (stateChange.ChangeKind == StateChangeKind.Add ||
                    stateChange.ChangeKind == StateChangeKind.Update)
                {
                    buffer = this.actorStateSerializer.Serialize(stateChange.Type, stateChange.Value);
                }

                serializedStateChanges.Add(new SerializedStateChange(stateChange.ChangeKind, key, buffer));
            }

            return this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                () => this.SaveStateAtomicallyAsync(serializedStateChanges, cancellationToken),
                string.Format("SaveStateAsync[{0}]", actorId),
                cancellationToken);
        }

        /// <summary>
        /// Checks whether the actor state provider contains an actor state with
        /// specified state name.
        /// </summary>
        /// <param name="actorId">ID of the actor for which to check state existence.</param>
        /// <param name="stateName">Name of the actor state to check for existence.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous check operation. The value of TResult
        /// parameter is <c>true</c> if state with specified name exists otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task<bool> IActorStateProvider.ContainsStateAsync(ActorId actorId, string stateName, CancellationToken cancellationToken)
        {
            var key = CreateActorStorageKey(actorId, stateName);

            return this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var tx = this.storeReplica.CreateTransaction())
                    {
                        var res = this.storeReplica.Contains(tx, key);
                        return Task.FromResult(res);
                    }
                },
                string.Format("ContainsStateAsync[{0}]", actorId),
                cancellationToken);
        }

        /// <summary>
        /// Removes all the existing states and reminders associated with specified actor atomically.
        /// </summary>
        /// <param name="actorId">ID of the actor for which to remove state.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous remove operation.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task IActorStateProvider.RemoveActorAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            return this.RemoveActorAtomicallyAsync(actorId, cancellationToken);
        }

        /// <summary>
        /// Creates an enumerable of all the state names associated with specified actor.
        /// </summary>
        /// <remarks>
        /// The enumerator returned from actor state provider is safe to use concurrently
        /// with reads and writes to the state provider. It represents a snapshot consistent
        /// view of the state provider.
        /// </remarks>
        /// <param name="actorId">The ID of the actor for which to create enumerable.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous enumeration operation. The value of TResult
        /// parameter is an enumerable of all state names associated with specified actor.
        /// </returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task<IEnumerable<string>> IActorStateProvider.EnumerateStateNamesAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            return this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                () => this.GetStateNamesAsync(actorId, cancellationToken),
                string.Format("EnumerateStateNamesAsync[{0}]", actorId),
                cancellationToken);
        }

        /// <summary>
        /// Gets the ActorIds from the State Provider.
        /// </summary>
        /// <param name="numItemsToReturn">The number of items requested to be returned.</param>
        /// <param name="continuationToken">
        /// A continuation token to start querying the results from.
        /// A null value of continuation token means start returning values form the beginning.
        /// </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation of call to server.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <remarks>
        /// The <paramref name="continuationToken"/> is relative to the state of actor state provider
        /// at the time of invocation of this API. If the state of actor state provider changes (i.e.
        /// new actors are activated or existing actors are deleted) in between calls to this API and
        /// the continuation token from previous call (before the state was modified) is supplied, the
        /// result may contain entries that were already fetched in previous calls.
        /// </remarks>
        Task<PagedResult<ActorId>> IActorStateProvider.GetActorsAsync(
            int numItemsToReturn,
            ContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            return this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                () => this.GetStoredActorIdsAsync(numItemsToReturn, continuationToken, cancellationToken),
                "GetActorsAsync",
                cancellationToken);
        }

        /// <inheritdoc/>
        async Task<ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>> IActorStateProvider.GetRemindersAsync(int numItemsToReturn, ActorId actorId, ContinuationToken continuationToken, CancellationToken cancellationToken)
        {
            await this.EnsureLogicalTimeManagerInitializedAsync(cancellationToken);

            return await this.actorStateProviderHelper.ExecuteWithRetriesAsync(
               async () =>
               {
                   return await Task.Run(() =>
                   {
                       var result = new ConcurrentDictionary<ActorId, List<ActorReminderState>>();
                       var reminderkey = actorId == null
                           ? ReminderStorageKeyPrefix
                           : $"{ReminderStorageKeyPrefix}_{actorId.GetStorageKey()}";
                       var reminders = new List<ActorReminderData>();
                       var nextContinuationMarker = string.Empty;
                       var nextKey = string.Empty;
                       var hasMore = false;

                       using (var tx = this.storeReplica.CreateTransaction())
                       {
                           IEnumerator<KeyValueStoreItem> enumerator = null;
                           try
                           {
                               if (continuationToken != null)
                               {
                                   enumerator = this.storeReplica.Enumerate(tx, continuationToken.Marker.ToString(), false);
                                   hasMore = enumerator.MoveNext();
                                   if (hasMore && enumerator.Current.Metadata.Key == continuationToken.Marker.ToString())
                                   {
                                       // Check if the next element is equal to continuation token and skip.
                                       // ContinuationToken wouldn't match with next element if the reminder is deleted between paging calls.
                                       hasMore = enumerator.MoveNext();
                                   }
                               }
                               else
                               {
                                   enumerator = this.storeReplica.Enumerate(tx, reminderkey, true);
                                   hasMore = enumerator.MoveNext();
                               }

                               int itemCount = 0;
                               while (hasMore)
                               {
                                   cancellationToken.ThrowIfCancellationRequested();
                                   var currentKey = enumerator.Current.Metadata.Key;
                                   nextKey = currentKey;
                                   if (!currentKey.StartsWith(reminderkey) || itemCount++ >= numItemsToReturn)
                                   {
                                       break;
                                   }

                                   var item = enumerator.Current;
                                   var reminderData = this.DeserializeReminder(item.Value);
                                   if (reminderData != null)
                                   {
                                       reminders.Add(reminderData);
                                   }

                                   nextContinuationMarker = currentKey;
                                   hasMore = enumerator.MoveNext();
                               }
                           }
                           finally
                           {
                               if (enumerator != null)
                               {
                                   enumerator.Dispose();
                               }
                           }

                           foreach (var reminderData in reminders)
                           {
                               cancellationToken.ThrowIfCancellationRequested();
                               var reminderCompletedKey = ActorStateProviderHelper.CreateReminderCompletedStorageKey(reminderData.ActorId, reminderData.Name);
                               var completedValue = this.storeReplica.TryGetValue(tx, reminderCompletedKey);
                               ReminderCompletedData reminderCompletedData = null;
                               if (completedValue != null)
                               {
                                   reminderCompletedData = this.DeserializeReminderCompletedData(completedValue);
                               }

                               result.GetOrAdd(reminderData.ActorId, a => new List<ActorReminderState>())
                                   .Add(new ActorReminderState(reminderData, this.logicalTimeManager.CurrentLogicalTime, reminderCompletedData));
                           }
                       }

                       return new ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>()
                       {
                           Items = result.AsEnumerable(),
                           ContinuationToken = hasMore && nextKey.StartsWith(reminderkey) ? new ContinuationToken(nextContinuationMarker) : null,
                       };
                   });
               },
               "GetRemindersAsync",
               cancellationToken);
        }

        /// <summary>
        /// Saves the specified actor reminder. If an actor reminder with
        /// given name does not exist, it adds the actor reminder otherwise
        /// existing actor reminder with same name is updated.
        /// </summary>
        /// <param name="actorId">The ID of the actor for which to save the reminder.</param>
        /// <param name="reminder">The actor reminder to save.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        async Task IActorStateProvider.SaveReminderAsync(ActorId actorId, IActorReminder reminder, CancellationToken cancellationToken)
        {
            await this.EnsureLogicalTimeManagerInitializedAsync(cancellationToken);

            var reminderKey = CreateReminderStorageKey(actorId, reminder.Name);
            var data = new ActorReminderData(actorId, reminder, this.logicalTimeManager.CurrentLogicalTime);
            var buffer = this.SerializeReminder(data);

            var reminderCompletedKey = ActorStateProviderHelper.CreateReminderCompletedStorageKey(actorId, reminder.Name);

            await this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return this.AddOrUpdateReminderAsync(reminderKey, buffer, reminderCompletedKey);
                },
                string.Format("SaveReminderAsync[{0}]", actorId),
                cancellationToken);
        }

        /// <summary>
        /// Deletes the specified actor reminder if it exists.
        /// </summary>
        /// <param name="actorId">The ID of the actor for which to delete the reminder.</param>
        /// <param name="reminderName">The name of the reminder to delete.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task IActorStateProvider.DeleteReminderAsync(ActorId actorId, string reminderName, CancellationToken cancellationToken)
        {
            var reminderKey = CreateReminderStorageKey(actorId, reminderName);
            var reminderCompletedKey = ActorStateProviderHelper.CreateReminderCompletedStorageKey(actorId, reminderName);

            var reminderKeyInfo = new List<ReminderKeyInfo>
            {
                new ReminderKeyInfo(reminderKey, reminderCompletedKey),
            };

            return this.DeleteRemindersInternalAsync(reminderKeyInfo, $"DeleteReminderAsync[{actorId}]", cancellationToken);
        }

        /// <summary>
        /// Deletes the specified set of reminders.
        /// </summary>
        /// <param name="reminderNames">The set of reminders to delete.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task IActorStateProvider.DeleteRemindersAsync(IReadOnlyDictionary<ActorId, IReadOnlyCollection<string>> reminderNames, CancellationToken cancellationToken)
        {
            var reminderKeyInfoList = this.GetReminderKeyInfoList(reminderNames);

            return this.DeleteRemindersInternalAsync(
                reminderKeyInfoList, $"DeleteRemindersAsync[{reminderNames.Count}]", cancellationToken);
        }

        /// <summary>
        /// Loads all the reminders contained in the actor state provider.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for asynchronous load operation.</param>
        /// <returns>
        /// A task that represents the asynchronous load operation. The value of TResult
        /// parameter is a collection of all actor reminders contained in the actor state provider.
        /// </returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        async Task<IActorReminderCollection> IActorStateProvider.LoadRemindersAsync(CancellationToken cancellationToken)
        {
            await this.EnsureLogicalTimeManagerInitializedAsync(cancellationToken);

            return await this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                () => this.EnumerateReminderAsync(cancellationToken),
                "LoadRemindersAsync",
                cancellationToken);
        }

        /// <summary>
        /// Initialize the state provider replica using the service initialization information.
        /// </summary>
        /// <remarks>
        /// No complex processing should be done during Initialize. Expensive or long-running initialization should be done in OpenAsync.
        /// </remarks>
        /// <param name="initializationParameters">
        /// Service initialization information such as service name, partition id, replica id, and code package information.
        /// </param>
        void IStateProviderReplica.Initialize(StatefulServiceInitializationParameters initializationParameters)
        {
            this.traceId = ActorTrace.GetTraceIdForReplica(initializationParameters.PartitionId, initializationParameters.ReplicaId);
            this.initParams = initializationParameters;
            this.initParams.CodePackageActivationContext.ConfigurationPackageModifiedEvent += this.OnConfigurationPackageModified;
            this.LoadActorStateProviderSettings();

            this.storeReplica = this.OnCreateAndInitializeReplica(
                this.initParams,
                this.OnCopyComplete,
                this.OnReplicationOperation,
                this.OnDataLossAsync,
                this.OnRestoreCompletedAsync);
        }

        /// <summary>
        /// Open the state provider replica for use.
        /// </summary>
        /// <remarks>
        /// Extended state provider initialization tasks can be started at this time.
        /// </remarks>
        /// <param name="openMode">Indicates whether this is a new or existing replica.</param>
        /// <param name="partition">The partition this replica belongs to.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// Task that represents the asynchronous open operation. The result contains the replicator
        /// responsible for replicating state between other state provider replicas in the partition.
        /// </returns>
        Task<IReplicator> IStateProviderReplica.OpenAsync(
            ReplicaOpenMode openMode,
            IStatefulServicePartition partition,
            CancellationToken cancellationToken)
        {
            this.partition = partition;
            this.isBackupInProgress = 0;
            this.backupCallbackCts = null;
            this.backupCallbackTask = null;
            this.isClosingOrAborting = false;

            return this.storeReplica.OpenAsync(openMode, partition, cancellationToken);
        }

        /// <summary>
        /// Notify the state provider replica that its role is changing, for example to Primary or Secondary.
        /// </summary>
        /// <param name="newRole">The new replica role, such as primary or secondary.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>Task that represents the asynchronous change role operation.</returns>
        async Task IStateProviderReplica.ChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref this.roleChangeTracker);

            await this.storeReplica.ChangeRoleAsync(newRole, cancellationToken);

            switch (newRole)
            {
                case ReplicaRole.Primary:
                    this.stateProviderInitCts = new CancellationTokenSource();
                    this.stateProviderInitTask = this.StartStateProviderInitializationAsync(this.stateProviderInitCts.Token);
                    break;

                default:
                    await this.CancelStateProviderInitializationAsync();
                    break;
            }

            this.replicaRole = newRole;
        }

        /// <summary>
        /// Gracefully close the state provider replica.
        /// </summary>
        /// <remarks>
        /// This generally occurs when the replica's code is being upgrade, the replica is being moved
        /// due to load balancing, or a transient fault is detected.
        /// </remarks>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>Task that represents the asynchronous close operation.</returns>
        async Task IStateProviderReplica.CloseAsync(CancellationToken cancellationToken)
        {
            await this.storeReplica.CloseAsync(cancellationToken).ConfigureAwait(false);

            // KeyValueStoreReplica aborts any in-flight backup when it closes and backup callback is not invoked
            // with actual ESE backup finishing with error. However, if ESE backup has finished successfully and
            // backup callback is in-flight, it does not wait for the backup callback to finish, .
            await this.CancelAndAwaitBackupCallbackIfAnyAsync();
            await this.CancelStateProviderInitializationAsync();
        }

        /// <summary>
        /// Forcefully abort the state provider replica.
        /// </summary>
        /// <remarks>
        /// This generally occurs when a permanent fault is detected on the node, or when
        /// Service Fabric cannot reliably manage the replica's life-cycle due to internal failures.
        /// </remarks>
        void IStateProviderReplica.Abort()
        {
            this.storeReplica.Abort();
            this.CancelAndAwaitBackupCallbackIfAnyAsync().ContinueWith(
                t => t.Exception,
                TaskContinuationOptions.OnlyOnFaulted);
            this.CancelStateProviderInitializationAsync().ContinueWith(
                t => t.Exception,
                TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// Performs a full backup of all reliable state managed by this actor sate provider.
        /// </summary>
        /// <param name="backupCallback">Callback to be called when the backup folder has been created locally and is ready to be moved out of the node.</param>
        /// <returns>Task that represents the asynchronous backup operation.</returns>
        /// <remarks>
        /// A FULL backup will be performed with a one-hour timeout.
        /// Boolean returned by the backupCallback indicate whether the service was able to successfully move the backup folder to an external location.
        /// If false is returned, BackupAsync throws InvalidOperationException with the relevant message indicating backupCallback returned false.
        /// Also, backup will be marked as unsuccessful.
        /// </remarks>
        Task IStateProviderReplica.BackupAsync(Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            return ((IStateProviderReplica)this).BackupAsync(BackupOption.Full, Timeout.InfiniteTimeSpan, CancellationToken.None, backupCallback);
        }

        /// <summary>
        /// Performs backup of reliable state managed by this actor sate provider.
        /// </summary>
        /// <param name="option">The option for the backup.</param>
        /// <param name="timeout">The timeout for the backup.</param>
        /// <param name="cancellationToken">The cancellation token for the backup.</param>
        /// <param name="backupCallback">The callback to be called once the backup folder is ready.</param>
        /// <returns>Task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// KvsActorStateProviderBase Backup only support Full backup. KVS BackupInfo does not contain backup version.
        /// The Backup version is set to invalid.
        /// </remarks>
        async Task IStateProviderReplica.BackupAsync(
            BackupOption option,
            TimeSpan timeout,
            CancellationToken cancellationToken,
            Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            this.EnsureReplicaIsPrimary();

            this.AcquireBackupLock();

            try
            {
                var backupDirectoryPath = this.GetLocalBackupFolderPath();
                PrepareBackupFolder(backupDirectoryPath);

                await this.storeReplica.BackupAsync(
                    backupDirectoryPath,
                    option == BackupOption.Full ? StoreBackupOption.Full : StoreBackupOption.Incremental,
                    info => this.UserBackupCallbackHandler(info, backupCallback),
                    cancellationToken);
            }
            finally
            {
                this.ReleaseBackupLock();
            }
        }

        /// <summary>
        /// Restore a backup taken by <see cref="IStateProviderReplica.BackupAsync(Func{BackupInfo, CancellationToken, Task{bool}})"/> or
        /// <see cref="IStateProviderReplica.BackupAsync(BackupOption, TimeSpan, CancellationToken, Func{BackupInfo, CancellationToken, Task{bool}})"/>.
        /// </summary>
        /// <param name="backupFolderPath">
        /// The directory where the replica is to be restored from.
        /// This parameter cannot be null, empty or contain just whitespace.
        /// UNC paths may also be provided.
        /// </param>
        /// <returns>Task that represents the asynchronous restore operation.</returns>
        Task IStateProviderReplica.RestoreAsync(string backupFolderPath)
        {
            var restoreSettings = new RestoreSettings(true);
            return this.storeReplica.RestoreAsync(backupFolderPath, restoreSettings, CancellationToken.None);
        }

        /// <summary>
        /// Restore a backup taken by <see cref="IStateProviderReplica.BackupAsync(Func{BackupInfo, CancellationToken, Task{bool}})"/> or
        /// <see cref="IStateProviderReplica.BackupAsync(BackupOption, TimeSpan, CancellationToken, Func{BackupInfo, CancellationToken, Task{bool}})"/>.
        /// </summary>
        /// <param name="backupFolderPath">
        /// The directory where the replica is to be restored from.
        /// This parameter cannot be null, empty or contain just whitespace.
        /// UNC paths may also be provided.
        /// </param>
        /// /// <param name="restorePolicy">The restore policy.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>Task that represents the asynchronous restore operation.</returns>
        Task IStateProviderReplica.RestoreAsync(string backupFolderPath, RestorePolicy restorePolicy, CancellationToken cancellationToken)
        {
            var enableLsnCheck = (restorePolicy == RestorePolicy.Safe);
            var restoreSettings = new RestoreSettings(true, enableLsnCheck);

            return this.storeReplica.RestoreAsync(backupFolderPath, restoreSettings, cancellationToken);
        }

        /// <inheritdoc/>
        async Task VolatileLogicalTimeManager.ISnapshotHandler.OnSnapshotAsync(TimeSpan currentLogicalTime)
        {
            var data = new LogicalTimestamp(currentLogicalTime);

            try
            {
                var buffer = this.SerializeLogicalTimestamp(data);
                await this.UpdateOrAddAsync(LogicalTimestampKey, buffer);
            }
            catch (Exception)
            {
                // Ignore exception.
            }
        }

        /// <summary>
        /// Gets the replica status.
        /// </summary>
        /// <returns>Replica status</returns>
        object IInternalStatefulServiceReplica.GetStatus()
        {
            var internalReplica = this.storeReplica as IInternalStatefulServiceReplica;
            return internalReplica?.GetStatus();
        }

        internal abstract KeyValueStoreReplica OnCreateAndInitializeReplica(
            StatefulServiceInitializationParameters initParams,
            CopyCompletionCallback copyHandler,
            ReplicationCallback replicationHandler,
            DataLossCallback onDataLossHandler,
            RestoreCompletedCallback restoreCompletedHandler);

        internal ReplicatorSettings GetReplicatorSettings()
        {
            if (this.userDefinedReplicatorSettings != null)
            {
                return this.userDefinedReplicatorSettings;
            }

            return this.LoadReplicatorSettings();
        }

        internal KeyValueStoreReplica GetStoreReplica()
        {
            return this.storeReplica;
        }

        internal ActorStateProviderHelper GetActorStateProviderHelper()
        {
            return this.actorStateProviderHelper;
        }

        internal void ReportPartitionHealth(HealthInformation healthInformation)
        {
            try
            {
                this.partition.ReportPartitionHealth(healthInformation);
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.traceId,
                    "ReportPartitionHealth() failed with: {0} while reporting health information: {1}.",
                    ex.ToString(),
                    healthInformation.ToString());
            }
        }

        private static string CreateActorStorageKey(ActorId actorId, string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
            {
                // Backward compatibility for Actor<TState> (before named actor state was introduced)
                return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", ActorStorageKeyPrefix, actorId.GetStorageKey());
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}", ActorStorageKeyPrefix, actorId.GetStorageKey(), stateName);
        }

        private static string CreateActorStorageKeyPrefix(ActorId actorId, string stateNamePrefix)
        {
            return CreateActorStorageKey(actorId, stateNamePrefix);
        }

        private static string ExtractStateName(ActorId actorId, string storageKey)
        {
            var storageKeyPrefix = CreateActorStorageKeyPrefix(actorId, string.Empty);

            if (storageKey == storageKeyPrefix)
            {
                return string.Empty;
            }

            return storageKey.Substring(storageKeyPrefix.Length + 1);
        }

        private static string CreateReminderStorageKey(ActorId actorId, string reminderName)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}_{1}_{2}",
                ReminderStorageKeyPrefix,
                actorId.GetStorageKey(),
                reminderName);
        }

        private static string CreateReminderStorageKeyPrefix(ActorId actorId, string reminderNamePrefix)
        {
            return CreateReminderStorageKey(actorId, reminderNamePrefix);
        }

        private static void PrepareBackupFolder(string backupFolder)
        {
            try
            {
                FabricDirectory.Delete(backupFolder, true);
            }
            catch (DirectoryNotFoundException)
            {
                // Already empty
            }

            FabricDirectory.CreateDirectory(backupFolder);
        }

        private static byte[] Serialize<T>(DataContractSerializer serializer, T data)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(memoryStream);
                serializer.WriteObject(binaryWriter, data);
                binaryWriter.Flush();

                return memoryStream.ToArray();
            }
        }

        private static object Deserialize(DataContractSerializer serializer, byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                var binaryReader = XmlDictionaryReader.CreateBinaryReader(
                    memoryStream,
                    XmlDictionaryReaderQuotas.Max);

                return serializer.ReadObject(binaryReader);
            }
        }

        private async Task StartStateProviderInitializationAsync(CancellationToken cancellationToken)
        {
            Exception unexpectedException = null;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                    async () =>
                    {
                        await this.InitializeAndStartLogicalTimeManagerAsync(cancellationToken);
                    },
                    "StartStateProviderInitializationAsync",
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
                var mssgFormat = "StartStateProviderInitializationAsync() failed due to " +
                                 "an unexpected Exception causing replica to fault: {0}";

                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.traceId,
                    string.Format(mssgFormat, unexpectedException.ToString()));

                this.partition.ReportFault(FaultType.Transient);
            }
        }

        private async Task CancelStateProviderInitializationAsync()
        {
            if (this.stateProviderInitCts != null &&
                this.stateProviderInitCts.IsCancellationRequested == false)
            {
                ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Canceling state provider initialization...");

                this.stateProviderInitCts.Cancel();

                try
                {
                    await this.stateProviderInitTask;
                }
                catch (Exception ex)
                {
                    // Code should never come here.
                    ReleaseAssert.Failfast(
                        "CancelStateProviderInitializationAsync() unexpected exception: {0}.",
                        ex.ToString());
                }
                finally
                {
                    this.stateProviderInitCts = null;
                    this.stateProviderInitTask = null;
                }
            }

            this.StopLogicalTimeManager();
        }

        private async Task InitializeAndStartLogicalTimeManagerAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Initializing logical time manager...");

            if (this.isLogicalTimeManagerInitialized == true)
            {
                ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Logical time manager already initialized...");
                return;
            }

            // wait for read status
            await this.WaitForReadStatusAsync(cancellationToken);

            using (var tx = this.storeReplica.CreateTransaction())
            {
                var enumerator = this.storeReplica.Enumerate(tx, LogicalTimestampKey);

                while (enumerator.MoveNext())
                {
                    var item = enumerator.Current;
                    this.TryDeserializeAndApplyLogicalTimestamp(item.Metadata.Key, item.Value);
                }
            }

            this.logicalTimeManager.Start();
            Volatile.Write(ref this.isLogicalTimeManagerInitialized, true);

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Initializing logical time manager SUCCEEDED.");
        }

        private void StopLogicalTimeManager()
        {
            // Stop logical timer if it is running
            if (this.isLogicalTimeManagerInitialized == true)
            {
                ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Stopping logical time manager...");

                this.logicalTimeManager.Stop();
                this.isLogicalTimeManagerInitialized = false;
            }
        }

        private async Task WaitForReadStatusAsync(CancellationToken cancellationToken)
        {
            var retryCount = 0;

            while (!cancellationToken.IsCancellationRequested &&
                   this.partition.ReadStatus != PartitionAccessStatus.Granted)
            {
                retryCount++;
                ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Waiting for Read Status to be Granted");
                await Task.Delay(retryCount * StateProviderInitRetryDelayMilliseconds, cancellationToken);
            }
        }

        private async Task EnsureLogicalTimeManagerInitializedAsync(CancellationToken cancellationToken)
        {
            var retryCount = 0;

            while (this.replicaRole == ReplicaRole.Primary && !this.isLogicalTimeManagerInitialized)
            {
                retryCount++;
                await Task.Delay(retryCount * StateProviderInitRetryDelayMilliseconds, cancellationToken);
            }

            if (this.replicaRole != ReplicaRole.Primary)
            {
                throw new FabricNotPrimaryException(FabricErrorCode.NotPrimary);
            }
        }

        private void OnCopyComplete(KeyValueStoreEnumerator enumerator)
        {
            var inner = enumerator.Enumerate(LogicalTimestampKey);

            while (inner.MoveNext())
            {
                var item = inner.Current;
                this.TryDeserializeAndApplyLogicalTimestamp(item.Metadata.Key, item.Value);
            }
        }

        private void OnReplicationOperation(IEnumerator<KeyValueStoreNotification> notification)
        {
            while (notification.MoveNext())
            {
                var item = notification.Current;
                this.TryDeserializeAndApplyLogicalTimestamp(item.Metadata.Key, item.Value);
            }
        }

        private void OnConfigurationPackageModified(object sender, PackageModifiedEventArgs<ConfigurationPackage> e)
        {
            this.UpdateReplicatorSettings();
        }

        private void UpdateReplicatorSettings()
        {
            try
            {
                var replicatorSettings = this.LoadReplicatorSettings();
                this.storeReplica.UpdateReplicatorSettings(replicatorSettings);
            }
            catch (FabricElementNotFoundException ex)
            {
                // Trace and Report fault when section is not found for ReplicatorSettings.
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.traceId,
                    "FabricElementNotFoundException while loading replicator settings from configuation.",
                    ex);
                this.partition.ReportFault(FaultType.Transient);
            }
            catch (FabricException ex)
            {
                // Trace and Report fault if user intended to provide Replicator Security config but provided it incorrectly.
                ActorTrace.Source.WriteErrorWithId(TraceType, this.traceId, "FabricException while loading replicator security settings from configuation.", ex);
                this.partition.ReportFault(FaultType.Transient);
            }
            catch (ArgumentException ex)
            {
                ActorTrace.Source.WriteWarningWithId(TraceType, this.traceId, "ArgumentException while updating replicator settings from configuation.", ex);
                this.partition.ReportFault(FaultType.Transient);
            }
        }

        private void EnsureReplicaIsPrimary()
        {
            if (this.replicaRole != ReplicaRole.Primary)
            {
                throw new FabricNotPrimaryException();
            }
        }

        private async Task<bool> UserBackupCallbackHandler(
            StoreBackupInfo storeBackupInfo,
            Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            this.EnsureReplicaIsPrimary();

            var backupInfo = new BackupInfo(
                storeBackupInfo.BackupFolder,
                storeBackupInfo.BackupOption == StoreBackupOption.Full ? BackupOption.Full : BackupOption.Incremental,
                BackupInfo.BackupVersion.InvalidBackupVersion);

            await this.backupCallbackLock.WaitAsync();

            try
            {
                if (this.isClosingOrAborting)
                {
                    // this replica is already closing.
                    throw new FabricObjectClosedException();
                }

                this.backupCallbackCts = new CancellationTokenSource();
                this.backupCallbackTask = backupCallback.Invoke(backupInfo, this.backupCallbackCts.Token);
            }
            catch (Exception)
            {
                this.backupCallbackCts = null;
                this.backupCallbackTask = null;

                throw;
            }
            finally
            {
                this.backupCallbackLock.Release();
            }

            return await this.backupCallbackTask;
        }

        private string GetLocalBackupFolderPath()
        {
            return Path.Combine(
                this.initParams.CodePackageActivationContext.WorkDirectory,
                BackupRootFolderPrefix + this.initParams.PartitionId,
                this.initParams.ReplicaId.ToString(),
                LocalBackupFolderName);
        }

        private void CleanupBackupFolder()
        {
            try
            {
                FabricDirectory.Delete(this.GetLocalBackupFolderPath(), true);
            }
            catch (DirectoryNotFoundException)
            {
                // Already empty
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.traceId,
                    "CleanupBackupFolder() failed with: {0}.",
                    ex.ToString());
            }
        }

        private async Task CancelAndAwaitBackupCallbackIfAnyAsync()
        {
            await this.backupCallbackLock.WaitAsync();

            this.isClosingOrAborting = true;

            this.backupCallbackLock.Release();

            try
            {
                if (this.backupCallbackCts != null &&
                    this.backupCallbackCts.IsCancellationRequested == false)
                {
                    this.backupCallbackCts.Cancel();
                }

                await this.AwaitBackupCallbackWithHealthReportingAsync();
            }
            finally
            {
                this.CleanupBackupFolder();
            }
        }

        private void AcquireBackupLock()
        {
            if (Interlocked.CompareExchange(ref this.isBackupInProgress, 1, 0) == 1)
            {
                throw new FabricBackupInProgressException();
            }

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Acquired backup lock.");
        }

        private void ReleaseBackupLock()
        {
            Volatile.Write(ref this.isBackupInProgress, 0);
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Released backup lock.");
        }

        private async Task AwaitBackupCallbackWithHealthReportingAsync()
        {
            if (this.backupCallbackTask != null)
            {
                ActorTrace.Source.WriteNoiseWithId(TraceType, this.traceId, "Awaiting backupCallbackTask started...");

                while (true)
                {
                    var delayTaskCts = new CancellationTokenSource();
                    var delayTask = Task.Delay(this.stateProviderSettings.BackupCallbackExpectedCancellationTime, delayTaskCts.Token);

                    var finishedTask = await Task.WhenAny(this.backupCallbackTask, delayTask);

                    if (finishedTask == this.backupCallbackTask)
                    {
                        delayTaskCts.Cancel();

                        ServiceHelper.ObserveExceptionIfAny(delayTask);

                        break;
                    }

                    this.ReportBackupCallbackSlowCancellationHealth();
                }

                ActorTrace.Source.WriteNoiseWithId(TraceType, this.traceId, "Awaiting backupCallbackTask finished...");
            }
        }

        private void ReportBackupCallbackSlowCancellationHealth()
        {
            var description = string.Format(
                "BackupCallback is taking longer than expected time ({0}s) to cancel.",
                this.stateProviderSettings.BackupCallbackExpectedCancellationTime.TotalSeconds);

            var healthInfo = new HealthInformation(KvsHealthSourceId, BackupCallbackSlowCancellationHealthProperty, HealthState.Warning)
            {
                TimeToLive = this.stateProviderSettings.BackupCallbackSlowCancellationHealthReportTimeToLive,
                RemoveWhenExpired = true,
                Description = description,
            };

            this.ReportPartitionHealth(healthInfo);
        }

        private ReplicatorSettings LoadReplicatorSettings()
        {
            return ActorStateProviderHelper.GetActorReplicatorSettings(
                this.initParams.CodePackageActivationContext,
                this.actorTypeInformation.ImplementationType);
        }

        private void LoadActorStateProviderSettings()
        {
            var configPackageName = ActorNameFormat.GetConfigPackageName(this.actorTypeInformation.ImplementationType);
            var sectionName = ActorNameFormat.GetActorStateProviderSettingsSectionName(this.actorTypeInformation.ImplementationType);

            this.stateProviderSettings = KvsActorStateProviderSettings.LoadFrom(
                this.initParams.CodePackageActivationContext,
                configPackageName,
                sectionName);

            ActorTrace.Source.WriteInfoWithId(
                TraceType, this.traceId, "KvsActorStateProviderSettings: {0}", this.stateProviderSettings);
        }

        private byte[] SerializeReminder(ActorReminderData reminder)
        {
            return Serialize(this.reminderSerializer, reminder);
        }

        private byte[] SerializeLogicalTimestamp(LogicalTimestamp timestamp)
        {
            return Serialize(this.timestampSerializer, timestamp);
        }

        private byte[] SerializeReminderCompletedData(ReminderCompletedData data)
        {
            return Serialize(this.reminderCompletedDataSerializer, data);
        }

        private ActorReminderData DeserializeReminder(byte[] data)
        {
            return Deserialize(this.reminderSerializer, data) as ActorReminderData;
        }

        private LogicalTimestamp DeserializeLogicalTimeStamp(byte[] data)
        {
            return Deserialize(this.timestampSerializer, data) as LogicalTimestamp;
        }

        private ReminderCompletedData DeserializeReminderCompletedData(byte[] data)
        {
            return Deserialize(this.reminderCompletedDataSerializer, data) as ReminderCompletedData;
        }

        private void TryDeserializeAndApplyLogicalTimestamp(string key, byte[] value)
        {
            if (key.Equals(LogicalTimestampKey))
            {
                var timestamp = this.DeserializeLogicalTimeStamp(value);

                if (timestamp != null)
                {
                    this.logicalTimeManager.CurrentLogicalTime = timestamp.Timestamp;
                }
            }
        }

        private async Task AddOrUpdateReminderAsync(string reminderKey, byte[] state, string reminderCompletedKey)
        {
            using (var tx = this.storeReplica.CreateTransaction())
            {
                if (!this.storeReplica.TryUpdate(tx, reminderKey, state))
                {
                    this.storeReplica.Add(tx, reminderKey, state);
                }

                // Remove last reminder completed data if any.
                this.storeReplica.TryRemove(tx, reminderCompletedKey);

                await tx.CommitAsync();
            }
        }

        private Task DeleteRemindersInternalAsync(List<ReminderKeyInfo> reminderKeyInfoList, string functionNameTag, CancellationToken cancellationToken)
        {
            if (reminderKeyInfoList.Count == 0)
            {
                return Task.FromResult(true);
            }

            return this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return this.DeleteReminderAsync(reminderKeyInfoList);
                },
                functionNameTag,
                cancellationToken);
        }

        private async Task DeleteReminderAsync(IEnumerable<ReminderKeyInfo> reminderKeyInfoList)
        {
            using (var tx = this.storeReplica.CreateTransaction())
            {
                foreach (var reminderKeyInfo in reminderKeyInfoList)
                {
                    this.storeReplica.TryRemove(tx, reminderKeyInfo.ReminderKey);
                    this.storeReplica.TryRemove(tx, reminderKeyInfo.ReminderCompletedKey);
                }

                await tx.CommitAsync();
            }
        }

        private List<ReminderKeyInfo> GetReminderKeyInfoList(IReadOnlyDictionary<ActorId, IReadOnlyCollection<string>> reminderNames)
        {
            var reminderKeyInfoList = new List<ReminderKeyInfo>();

            foreach (var reminderNamesPerActor in reminderNames)
            {
                var actorId = reminderNamesPerActor.Key;

                foreach (var reminderName in reminderNamesPerActor.Value)
                {
                    var reminderKey = CreateReminderStorageKey(actorId, reminderName);
                    var reminderCompletedKey = ActorStateProviderHelper.CreateReminderCompletedStorageKey(actorId, reminderName);

                    reminderKeyInfoList.Add(new ReminderKeyInfo(reminderKey, reminderCompletedKey));
                }
            }

            return reminderKeyInfoList;
        }

        private async Task UpdateOrAddAsync(string key, byte[] state)
        {
            using (var tx = this.storeReplica.CreateTransaction())
            {
                if (!this.storeReplica.TryUpdate(tx, key, state))
                {
                    this.storeReplica.Add(tx, key, state);
                }

                await tx.CommitAsync();
            }
        }

        private Task<Dictionary<string, ReminderCompletedData>> GetReminderCompletedDataMapAsync(Transaction tx, CancellationToken cancellationToken)
        {
            var reminderCompletedDataMap = new Dictionary<string, ReminderCompletedData>();

            using var enumerator = this.storeReplica.Enumerate(tx, ActorStateProviderHelper.ReminderCompletedStorageKeyPrefix);

            while (enumerator.MoveNext())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var item = enumerator.Current;
                var data = this.DeserializeReminderCompletedData(item.Value);

                if (data != null)
                {
                    reminderCompletedDataMap.Add(item.Metadata.Key, data);
                }
            }

            return Task.FromResult(reminderCompletedDataMap);
        }

        private async Task<IActorReminderCollection> EnumerateReminderAsync(CancellationToken cancellationToken)
        {
            var reminderCollection = new ActorReminderCollection();

            using (var tx = this.storeReplica.CreateTransaction())
            {
                var reminderCompletedDataMap = await this.GetReminderCompletedDataMapAsync(tx, cancellationToken);

                using var enumerator = this.storeReplica.Enumerate(tx, ReminderStorageKeyPrefix);

                while (enumerator.MoveNext())
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var item = enumerator.Current;
                    var reminderData = this.DeserializeReminder(item.Value);

                    if (reminderData != null)
                    {
                        var reminderCompletedKey =
                            ActorStateProviderHelper.CreateReminderCompletedStorageKey(reminderData.ActorId, reminderData.Name);

                        reminderCompletedDataMap.TryGetValue(reminderCompletedKey, out var reminderCompletedData);

                        reminderCollection.Add(
                            reminderData.ActorId,
                            new ActorReminderState(reminderData, this.logicalTimeManager.CurrentLogicalTime, reminderCompletedData));
                    }
                }
            }

            return reminderCollection;
        }

        private async Task SaveStateAtomicallyAsync(
            IEnumerable<SerializedStateChange> serializedStateChanges,
            CancellationToken cancellationToken)
        {
            // Check for cancellation before creating the transaction.
            cancellationToken.ThrowIfCancellationRequested();

            using (var tx = this.storeReplica.CreateTransaction())
            {
                foreach (var stateChange in serializedStateChanges)
                {
                    switch (stateChange.ChangeKind)
                    {
                        case StateChangeKind.Add:
                            this.storeReplica.Add(tx, stateChange.Key, stateChange.SerializedState);
                            break;
                        case StateChangeKind.Update:
                            this.storeReplica.Update(tx, stateChange.Key, stateChange.SerializedState);
                            break;
                        case StateChangeKind.Remove:
                            this.storeReplica.Remove(tx, stateChange.Key);
                            break;
                        default:
                            throw new InvalidOperationException(Actors.SR.InvalidStateChangeKind);
                    }
                }

                await tx.CommitAsync();
            }
        }

        private void RemoveKeysWithPrefixAsync(Transaction tx, string keyPrefix)
        {
            using var stateMetadataEnumerator = this.storeReplica.EnumerateMetadata(tx, keyPrefix);

            while (stateMetadataEnumerator.MoveNext())
            {
                this.storeReplica.TryRemove(tx, stateMetadataEnumerator.Current.Key);
            }
        }

        private Task RemoveActorAtomicallyAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            var stateKeyPrefix = CreateActorStorageKeyPrefix(actorId, string.Empty);
            var reminderKeyPrefix = CreateReminderStorageKeyPrefix(actorId, string.Empty);
            var reminderCompletedKeyPrefix = ActorStateProviderHelper.CreateReminderCompletedStorageKeyPrefix(actorId);
            var actorePresenceStorageKey = ActorStateProviderHelper.CreateActorPresenceStorageKey(actorId);

            return this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                async () =>
                {
                    // Check for cancellation before creating transaction.
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var tx = this.storeReplica.CreateTransaction())
                    {
                        this.RemoveKeysWithPrefixAsync(tx, stateKeyPrefix + "_");
                        this.RemoveKeysWithPrefixAsync(tx, reminderKeyPrefix);
                        this.RemoveKeysWithPrefixAsync(tx, reminderCompletedKeyPrefix);

                        // Remove old style actor key if any
                        this.storeReplica.TryRemove(tx, stateKeyPrefix);

                        // Remove actor presence key
                        this.storeReplica.TryRemove(tx, actorePresenceStorageKey);

                        await tx.CommitAsync();
                    }
                },
                string.Format("RemoveActorAsync[{0}]", actorId),
                cancellationToken);
        }

        private Task<IEnumerable<string>> GetStateNamesAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            var stateNameList = new List<string>();
            var keyPrefix = CreateActorStorageKeyPrefix(actorId, string.Empty);

            // Check for cancellation before creating transaction.
            cancellationToken.ThrowIfCancellationRequested();

            using (var tx = this.storeReplica.CreateTransaction())
            {
                using var enumerator = this.storeReplica.EnumerateMetadata(tx, keyPrefix + "_");

                while (enumerator.MoveNext())
                {
                    stateNameList.Add(ExtractStateName(actorId, enumerator.Current.Key));
                }

                if (this.storeReplica.Contains(tx, keyPrefix))
                {
                    stateNameList.Add(string.Empty);
                }
            }

            return Task.FromResult((IEnumerable<string>)stateNameList);
        }

        /// <summary>
        /// KVS enumerates its entries in alphabetical order. The implementation of this
        /// function takes this into account while doing continuation token based enumeration.
        /// </summary>
        private Task<PagedResult<ActorId>> GetStoredActorIdsAsync(
            int itemsCount,
            ContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            using (var tx = this.storeReplica.CreateTransaction())
            {
                return this.GetStoredActorIdsForKvsAsync(
                    itemsCount,
                    continuationToken,
                    this.storeReplica,
                    cancellationToken);
            }
        }

        private Task<PagedResult<ActorId>> GetStoredActorIdsForKvsAsync(
            int itemsCount,
            ContinuationToken continuationToken,
            KeyValueStoreReplica replica,
            CancellationToken cancellationToken)
        {
            var actorIdList = new List<ActorId>();
            var actorQueryResult = new PagedResult<ActorId>();

            IEnumerator<KeyValueStoreItem> enumerator = null;
            bool enumHasMoreEntries;
            using var txn = replica.CreateTransaction();

            try
            {
                // Find continuation point for enumeration
                if (continuationToken != null)
                {
                    long previousActorCount = 0L;
                    if (long.TryParse((string)continuationToken.Marker, out previousActorCount))
                    {
                        enumerator = replica.Enumerate(txn, ActorStateProviderHelper.ActorPresenceStorageKeyPrefix, true);
                        enumHasMoreEntries = this.actorStateProviderHelper.GetContinuationPointByActorCount(previousActorCount, enumerator, cancellationToken);
                        enumHasMoreEntries = enumerator.MoveNext();
                    }
                    else
                    {
                        string lastSeenActorStorageKey = continuationToken.Marker.ToString();
                        enumerator = replica.Enumerate(txn, lastSeenActorStorageKey, false);
                        enumHasMoreEntries = enumerator.MoveNext();
                        var storageKey = enumerator.Current.Metadata.Key;
                        if (enumHasMoreEntries && storageKey == lastSeenActorStorageKey)
                        {
                            enumHasMoreEntries = enumerator.MoveNext();
                        }
                    }

                    if (!enumHasMoreEntries)
                    {
                        // We are here means the current snapshot that enumerator represents
                        // has less entries that what ContinuationToken contains.
                        return Task.FromResult(actorQueryResult);
                    }
                }
                else
                {
                    enumerator = replica.Enumerate(txn, ActorStateProviderHelper.ActorPresenceStorageKeyPrefix, true);
                    enumHasMoreEntries = enumerator.MoveNext();
                }

                if (!enumHasMoreEntries)
                {
                    return Task.FromResult(actorQueryResult);
                }

                while (enumHasMoreEntries && enumerator.Current.Metadata.Key.StartsWith(ActorStateProviderHelper.ActorPresenceStorageKeyPrefix))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var storageKey = enumerator.Current.Metadata.Key;
                    var actorId = ActorStateProviderHelper.GetActorIdFromPresenceStorageKey(storageKey);

                    if (actorId != null)
                    {
                        actorIdList.Add(actorId);
                    }
                    else
                    {
                        ActorTrace.Source.WriteWarningWithId(
                            TraceType,
                            this.traceId,
                            string.Format("Failed to parse ActorId from storage key: {0}", storageKey));
                    }

                    enumHasMoreEntries = enumerator.MoveNext();

                    if (actorIdList.Count == itemsCount)
                    {
                        actorQueryResult.Items = actorIdList.AsReadOnly();

                        // If enumerator has more elements, then set the continuation token.
                        if (enumHasMoreEntries && enumerator.Current.Metadata.Key.StartsWith(ActorStateProviderHelper.ActorPresenceStorageKeyPrefix))
                        {
                            actorQueryResult.ContinuationToken = new ContinuationToken(storageKey.ToString());
                        }

                        return Task.FromResult(actorQueryResult);
                    }
                }
            }
            finally
            {
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }

            // We are here means 'actorIdList' contains less than 'itemsCount'
            // item or it is empty. The continuation token will remain null.
            actorQueryResult.Items = actorIdList.AsReadOnly();

            return Task.FromResult(actorQueryResult);
        }

        private class ReminderKeyInfo
        {
            public ReminderKeyInfo(string reminderKey, string reminderCompletedKey)
            {
                this.ReminderKey = reminderKey;
                this.ReminderCompletedKey = reminderCompletedKey;
            }

            public string ReminderKey { get; private set; }

            public string ReminderCompletedKey { get; private set; }
        }
    }
}
