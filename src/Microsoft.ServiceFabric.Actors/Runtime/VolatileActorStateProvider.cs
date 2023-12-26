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
    using ActorStateDataWrapper = VolatileActorStateTable<
        VolatileActorStateProvider.ActorStateType,
        string,
        VolatileActorStateProvider.ActorStateData>.ActorStateDataWrapper;
    using ActorStateTable = VolatileActorStateTable<
        VolatileActorStateProvider.ActorStateType,
        string,
        VolatileActorStateProvider.ActorStateData>;
    using ReleaseAssert = Microsoft_ServiceFabric_Internal::System.Fabric.Common.ReleaseAssert;
    using Requires = Microsoft_ServiceFabric_Internal::System.Fabric.Common.Requires;
    using SR = Microsoft.ServiceFabric.Actors.SR;

    /// <summary>
    /// Provides an implementation of <see cref="IActorStateProvider"/> where actor state is kept in-memory and is volatile.
    /// </summary>
    public class VolatileActorStateProvider :
        IActorStateProvider, IStateProvider, VolatileLogicalTimeManager.ISnapshotHandler, IActorStateProviderInternal
    {
        private const string LogicalTimestampKey = "LogicalTimestamp";
        private const string TraceType = "VolatileActorStateProvider";
        private const int StateProviderInitRetryDelayMilliseconds = 500;
        private static readonly ActorStateData ActorPresenceValue = new ActorStateData(new[] { byte.MinValue });

        private readonly ActorStateTable stateTable;
        private readonly DataContractSerializer copyOrReplicationOperationSerializer;
        private readonly VolatileLogicalTimeManager logicalTimeManager;
        private readonly ActorStateProviderSerializer actorStateSerializer;
        private readonly object replicationLock;
        private readonly ActorStateProviderHelper actorStateProviderHelper;
        private readonly ReplicatorSettings userDefinedReplicatorSettings;
        private SecondaryPump secondaryPump;
        private ActorTypeInformation actorTypeInformation;
        private FabricReplicator fabricReplicator;
        private IStateReplicator2 stateReplicator;
        private ReplicaRole replicaRole;
        private IStatefulServicePartition partition;
        private string traceId;
        private StatefulServiceInitializationParameters initParams;

        private VolatileActorStateProviderSettings stateProviderSettings;
        private long roleChangeTracker;
        private bool isLogicalTimeManagerInitialized;
        private CancellationTokenSource stateProviderInitCts;
        private Task stateProviderInitTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="VolatileActorStateProvider"/> class.
        /// </summary>
        public VolatileActorStateProvider()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the  <see cref="VolatileActorStateProvider"/> class with
        /// specified replicator settings.
        /// </summary>
        /// <param name="replicatorSettings">
        /// A <see cref="ReplicatorSettings"/> that describes replicator settings.
        /// </param>
        public VolatileActorStateProvider(ReplicatorSettings replicatorSettings)
        {
            this.userDefinedReplicatorSettings = replicatorSettings;
            this.stateTable = new ActorStateTable();
            this.copyOrReplicationOperationSerializer = CreateCopyOrReplicationOperationSerializer();
            this.actorStateSerializer = new ActorStateProviderSerializer();
            this.logicalTimeManager = new VolatileLogicalTimeManager(this);
            this.secondaryPump = null;
            this.replicationLock = new object();
            this.replicaRole = ReplicaRole.Unknown;
            this.roleChangeTracker = DateTime.UtcNow.Ticks;
            this.actorStateProviderHelper = new ActorStateProviderHelper(this);
            this.isLogicalTimeManagerInitialized = false;
            this.stateProviderInitCts = null;
            this.stateProviderInitTask = null;
        }

        internal enum ActorStateType
        {
            LogicalTimestamp = 0,
            Actor = 1,
            Reminder = 2,
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
        /// Sets the function called post restore has been performed on the replica.
        /// </summary>
        /// <value>
        /// A function representing on restore completed callback function.
        /// </value>
        public Func<CancellationToken, Task> OnRestoreCompletedAsync { private get; set; }

        /// <summary>
        /// Sets the function called during suspected data-loss.
        /// </summary>
        /// <value>
        /// A function representing data-loss callback function.
        /// </value>
        public Func<CancellationToken, Task<bool>> OnDataLossAsync { private get; set; }

        /// <summary>
        /// Initializes the actor state provider with type information
        /// of the actor type associated with it.
        /// </summary>
        /// <param name="actorTypeInfo">Type information of the actor class</param>
        void IActorStateProvider.Initialize(ActorTypeInformation actorTypeInfo)
        {
            this.actorTypeInformation = actorTypeInfo;
        }

        /// <summary>
        /// This method is invoked as part of the activation process of the actor with the specified Id.
        /// </summary>
        /// <param name="actorId">ID of the actor that is activated.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <returns> A task that represents the asynchronous Actor activation notification processing.</returns>
        async Task IActorStateProvider.ActorActivatedAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            var key = ActorStateProviderHelper.CreateActorPresenceStorageKey(actorId);

            if (!this.stateTable.TryGetValue(ActorStateType.Actor, key, out var data))
            {
                await this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                    () =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        return this.ReplicateUpdateAsync(ActorStateType.Actor, key, ActorPresenceValue);
                    },
                    string.Format("ActorActivatedAsync[{0}]", actorId),
                    cancellationToken);
            }
        }

        /// <summary>
        /// This method is invoked when a reminder fires and finishes executing its callback
        /// <see cref="IRemindable.ReceiveReminderAsync"/> successfully.
        /// </summary>
        /// <param name="actorId">ID of the actor which own reminder</param>
        /// <param name="reminder">Actor reminder that completed successfully.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous reminder callback completed notification processing.
        /// </returns>
        async Task IActorStateProvider.ReminderCallbackCompletedAsync(ActorId actorId, IActorReminder reminder, CancellationToken cancellationToken)
        {
            await this.EnsureLogicalTimeManagerInitializedAsync(cancellationToken);

            var reminderCompletedKey = ActorStateProviderHelper.CreateReminderCompletedStorageKey(actorId, reminder.Name);
            var reminderCompletedData = new ReminderCompletedData(this.logicalTimeManager.CurrentLogicalTime, DateTime.UtcNow);
            var actorStateData = new ActorStateData(reminderCompletedData);

            await this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return this.ReplicateUpdateAsync(ActorStateType.Actor, reminderCompletedKey, actorStateData);
                },
                string.Format("ReminderCallbackCompletedAsync[{0}]", actorId),
                cancellationToken);
        }

        /// <summary>
        /// Loads the actor state associated with the specified state name.
        /// </summary>
        /// <typeparam name="T">Type of value of actor state associated with given state name.</typeparam>
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

            if (this.stateTable.TryGetValue(ActorStateType.Actor, key, out var data))
            {
                var result = this.actorStateSerializer.Deserialize<T>(data.ActorState);
                return Task.FromResult(result);
            }

            throw new KeyNotFoundException(string.Format(SR.ErrorNamedActorStateNotFound, stateName));
        }

        /// <summary>
        /// Saves the specified set of actor state changes atomically.
        /// </summary>
        /// <param name="actorId">The ID of the actor for which to save the state changes.</param>
        /// <param name="stateChanges">The collection of state changes to save.</param>
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
        async Task IActorStateProvider.SaveStateAsync(ActorId actorId, IReadOnlyCollection<ActorStateChange> stateChanges, CancellationToken cancellationToken)
        {
            var actorStateDataWrapperList = new List<ActorStateDataWrapper>();

            foreach (var stateChange in stateChanges)
            {
                var key = CreateActorStorageKey(actorId, stateChange.StateName);

                if (stateChange.ChangeKind == StateChangeKind.Add || stateChange.ChangeKind == StateChangeKind.Update)
                {
                    var buffer = this.actorStateSerializer.Serialize(stateChange.Type, stateChange.Value);
                    actorStateDataWrapperList.Add(
                        ActorStateDataWrapper.CreateForUpdate(ActorStateType.Actor, key, new ActorStateData(buffer)));
                }
                else if (stateChange.ChangeKind == StateChangeKind.Remove)
                {
                    actorStateDataWrapperList.Add(ActorStateDataWrapper.CreateForDelete(ActorStateType.Actor, key));
                }
            }

            if (actorStateDataWrapperList.Count > 0)
            {
                await this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                    async () =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await this.ReplicateStateChangesAsync(actorStateDataWrapperList);
                    },
                    string.Format("SaveStateAsync[{0}]", actorId),
                    cancellationToken);
            }
        }

        /// <summary>
        /// Checks whether actor state provider contains an actor state with
        /// specified state name.
        /// </summary>
        /// <param name="actorId">The ID of the actor for which to check state existence.</param>
        /// <param name="stateName">The name of the actor state to check for existence.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous check operation. The value of TResult
        /// parameter is <c>true</c> if state with specified name exists otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task<bool> IActorStateProvider.ContainsStateAsync(ActorId actorId, string stateName, CancellationToken cancellationToken)
        {
            Requires.Argument("stateName", stateName).NotNull();

            var key = CreateActorStorageKey(actorId, stateName);

            if (this.stateTable.TryGetValue(ActorStateType.Actor, key, out var data))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        /// <summary>
        /// Removes all the existing states and reminders associated with specified actor atomically.
        /// </summary>
        /// <param name="actorId">ID of the actor for which to remove state.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous remove operation.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        async Task IActorStateProvider.RemoveActorAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            var actorStateDataWrapperList = new List<ActorStateDataWrapper>();

            ActorStateDataWrapper actorStateDataWrapper;

            // Actor keys to delete
            var actorStorageKeyPrefix = CreateActorStorageKeyPrefix(actorId, string.Empty);

            // Reminder last completed keys to delete
            var reminderCompletedKeyPrefix = ActorStateProviderHelper.CreateReminderCompletedStorageKeyPrefix(actorId);

            var actorStateEnumerator = this.stateTable.GetShallowCopiesEnumerator(ActorStateType.Actor);

            while ((actorStateDataWrapper = actorStateEnumerator.GetNext()) != null)
            {
                if (actorStateDataWrapper.Key == actorStorageKeyPrefix ||
                    actorStateDataWrapper.Key.StartsWith(actorStorageKeyPrefix + "_") ||
                    actorStateDataWrapper.Key.StartsWith(reminderCompletedKeyPrefix))
                {
                    actorStateDataWrapperList.Add(
                        ActorStateDataWrapper.CreateForDelete(ActorStateType.Actor, actorStateDataWrapper.Key));
                }
            }

            // Reminder keys to delete
            var reminderStorgaeKeyPrefix = CreateReminderStorageKeyPrefix(actorId, string.Empty);

            var reminderEnumerator = this.stateTable.GetShallowCopiesEnumerator(ActorStateType.Reminder);

            while ((actorStateDataWrapper = reminderEnumerator.GetNext()) != null)
            {
                if (actorStateDataWrapper.Key.StartsWith(reminderStorgaeKeyPrefix))
                {
                    actorStateDataWrapperList.Add(
                        ActorStateDataWrapper.CreateForDelete(ActorStateType.Reminder, actorStateDataWrapper.Key));
                }
            }

            // Actor presence key to delete.
            var key = ActorStateProviderHelper.CreateActorPresenceStorageKey(actorId);
            actorStateDataWrapperList.Add(ActorStateDataWrapper.CreateForDelete(ActorStateType.Actor, key));

            if (actorStateDataWrapperList.Count > 0)
            {
                await this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                    async () =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await this.ReplicateStateChangesAsync(actorStateDataWrapperList);
                    },
                    string.Format("RemoveActorAsync[{0}]", actorId),
                    cancellationToken);
            }
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
            var stateNameList = new List<string>();
            var storageKeyPrefix = CreateActorStorageKeyPrefix(actorId, string.Empty);

            var actorStateEnumerator = this.stateTable.GetShallowCopiesEnumerator(ActorStateType.Actor);

            ActorStateTable.ActorStateDataWrapper actorStateDataWrapper;

            while ((actorStateDataWrapper = actorStateEnumerator.GetNext()) != null)
            {
                if (actorStateDataWrapper.Key == storageKeyPrefix ||
                    actorStateDataWrapper.Key.StartsWith(storageKeyPrefix + "_"))
                {
                    stateNameList.Add(ExtractStateName(actorId, actorStateDataWrapper.Key));
                }
            }

            return Task.FromResult((IEnumerable<string>)stateNameList);
        }

        /// <summary>
        /// Gets ActorIds from the State Provider.
        /// </summary>
        /// <param name="itemsCount">Number of items requested to be returned.</param>
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
            int itemsCount,
            ContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            return this.actorStateProviderHelper.GetStoredActorIdsAsync(
                itemsCount,
                continuationToken,
                () => this.stateTable.GetSortedStorageKeyEnumerator(
                    ActorStateType.Actor,
                    key => key.StartsWith(ActorStateProviderHelper.ActorPresenceStorageKeyPrefix)),
                storageKey => storageKey,
                cancellationToken);
        }

        /// <inheritdoc/>
        async Task<ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>> IActorStateProvider.GetRemindersAsync(
            int numItemsToReturn,
            ActorId actorId,
            ContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            await this.EnsureLogicalTimeManagerInitializedAsync(cancellationToken);

            return await this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                async () =>
                {
                    return await Task.Run(() =>
                    {
                        var result = new ConcurrentDictionary<ActorId, List<ActorReminderState>>();
                        Func<string, bool> filterFunc = key => true;
                        if (actorId != null)
                        {
                            filterFunc = key => key.StartsWith(actorId.GetStorageKey());
                        }

                        using var enumerator = this.stateTable.GetSortedValueEnumerator(
                            ActorStateType.Reminder,
                            filterFunc,
                            delegate(ActorStateData r1, ActorStateData r2)
                            {
                                if (r1 == null && r2 == null)
                                {
                                    return 0;
                                }
                                else if (r1 == null)
                                {
                                    return -1;
                                }
                                else if (r2 == null)
                                {
                                    return 1;
                                }
                                else
                                {
                                    return CreateReminderStorageKey(r1.ActorReminderData.ActorId, r1.ActorReminderData.Name)
                                        .CompareTo(CreateReminderStorageKey(r2.ActorReminderData.ActorId, r2.ActorReminderData.Name));
                                }
                            });

                        var hasMore = false;
                        var itemCount = 0;
                        var nextMarker = string.Empty;
                        while (hasMore = enumerator.MoveNext())
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            var reminderData = enumerator.Current.ActorReminderData;
                            if (reminderData == null)
                            {
                                continue;
                            }

                            var key = CreateReminderStorageKey(reminderData.ActorId, reminderData.Name);
                            if (continuationToken != null &&
                                    string.Compare(key, continuationToken.Marker.ToString(), StringComparison.InvariantCulture) <= 0)
                            {
                                continue;
                            }

                            if (itemCount++ >= numItemsToReturn)
                            {
                                break;
                            }

                            var reminderCompletedKey =
                                ActorStateProviderHelper.CreateReminderCompletedStorageKey(reminderData.ActorId, reminderData.Name);
                            ReminderCompletedData reminderCompletedData = null;
                            if (this.stateTable.TryGetValue(ActorStateType.Actor, reminderCompletedKey, out var data))
                            {
                                reminderCompletedData = data.ReminderLastCompletedData;
                            }

                            nextMarker = key;
                            result.GetOrAdd(reminderData.ActorId, new List<ActorReminderState>())
                                .Add(new ActorReminderState(reminderData, this.logicalTimeManager.CurrentLogicalTime, reminderCompletedData));
                        }

                        return new ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>()
                        {
                            Items = result.AsEnumerable(),
                            ContinuationToken = hasMore && nextMarker != string.Empty ? new ContinuationToken(nextMarker) : null,
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

            var reminderData = new ActorStateData(new ActorReminderData(actorId, reminder, this.logicalTimeManager.CurrentLogicalTime));

            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "Saving Reminder - {0}",
                reminderData);

            var actorStateDataWrapperList = new List<ActorStateDataWrapper>
            {
                ActorStateDataWrapper.CreateForUpdate(
                    ActorStateType.Reminder,
                    CreateReminderStorageKey(actorId, reminder.Name),
                    reminderData),

                ActorStateDataWrapper.CreateForDelete(
                    ActorStateType.Actor,
                    ActorStateProviderHelper.CreateReminderCompletedStorageKey(actorId, reminder.Name)),
            };

            await this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return this.ReplicateStateChangesAsync(actorStateDataWrapperList);
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
            var actorStateDataWrapperList = new List<ActorStateDataWrapper>
            {
                ActorStateDataWrapper.CreateForDelete(
                    ActorStateType.Reminder,
                    CreateReminderStorageKey(actorId, reminderName)),

                ActorStateDataWrapper.CreateForDelete(
                    ActorStateType.Actor,
                    ActorStateProviderHelper.CreateReminderCompletedStorageKey(actorId, reminderName)),
            };

            return this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return this.ReplicateStateChangesAsync(actorStateDataWrapperList);
                },
                string.Format("DeleteReminderAsync[{0}]", actorId),
                cancellationToken);
        }

        /// <summary>
        /// Deletes the specified set of reminders.
        /// </summary>
        /// <param name="reminderNames">The set of reminders to delete.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task IActorStateProvider.DeleteRemindersAsync(
            IReadOnlyDictionary<ActorId, IReadOnlyCollection<string>> reminderNames,
            CancellationToken cancellationToken)
        {
            var actorStateDataWrapperList = this.GetReminderDataWrapperList(reminderNames);

            if (actorStateDataWrapperList.Count == 0)
            {
                return Task.FromResult(true);
            }

            return this.actorStateProviderHelper.ExecuteWithRetriesAsync(
                () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return this.ReplicateStateChangesAsync(actorStateDataWrapperList);
                },
                $"DeleteRemindersAsync[{actorStateDataWrapperList.Count / 2}]",
                cancellationToken);
        }

        /// <summary>
        /// Loads all the reminders contained in the actor state provider.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for asynchronous load operation.</param>
        /// <returns>
        /// A task that represents the asynchronous load operation. The value of TResult
        /// parameter is a collection of all actor reminders contained in the actor state provider.
        /// </returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        async Task<IActorReminderCollection> IActorStateProvider.LoadRemindersAsync(CancellationToken cancellationToken)
        {
            await this.EnsureLogicalTimeManagerInitializedAsync(cancellationToken);

            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "Enumerating all reminders. Current Logical Time - {0}",
                this.logicalTimeManager.CurrentLogicalTime);

            var reminderCollection = new ActorReminderCollection();
            var stateDictionary = this.stateTable.GetActorStateDictionary(ActorStateType.Reminder);

            foreach (var kvPair in stateDictionary)
            {
                var reminderData = kvPair.Value.ActorReminderData;

                if (reminderData == null)
                {
                    continue;
                }

                var reminderCompletedKey =
                    ActorStateProviderHelper.CreateReminderCompletedStorageKey(reminderData.ActorId, reminderData.Name);

                ReminderCompletedData reminderCompletedData = null;
                if (this.stateTable.TryGetValue(ActorStateType.Actor, reminderCompletedKey, out var data))
                {
                    reminderCompletedData = data.ReminderLastCompletedData;
                }

                reminderCollection.Add(
                    reminderData.ActorId,
                    new ActorReminderState(reminderData, this.logicalTimeManager.CurrentLogicalTime, reminderCompletedData));
            }

            return (IActorReminderCollection)reminderCollection;
        }

        /// <summary>
        /// Initialize the state provider replica using the service initialization information.
        /// </summary>
        /// <remarks>
        /// No complex processing should be done during Initialize. Expensive or long-running initialization should be done in OpenAsync.
        /// </remarks>
        /// <param name="initializationParameters">Service initialization information such as service name, partition id, replica id, and code package information.</param>
        void IStateProviderReplica.Initialize(StatefulServiceInitializationParameters initializationParameters)
        {
            this.initParams = initializationParameters;
            this.traceId = ActorTrace.GetTraceIdForReplica(initializationParameters.PartitionId, initializationParameters.ReplicaId);

            this.LoadActorStateProviderSettings();
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
            this.fabricReplicator = partition.CreateReplicator(this, this.GetReplicatorSettings());
            this.stateReplicator = this.fabricReplicator.StateReplicator2;
            this.partition = partition;

            this.secondaryPump = new SecondaryPump(
                this.partition,
                this.stateTable,
                this.stateReplicator,
                this.copyOrReplicationOperationSerializer,
                this.logicalTimeManager,
                this.traceId);

            return Task.FromResult<IReplicator>(this.fabricReplicator);
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

            switch (newRole)
            {
                case ReplicaRole.IdleSecondary:
                    await this.CancelStateProviderInitializationAsync();
                    this.secondaryPump.StartCopyAndReplicationPump();
                    break;

                case ReplicaRole.ActiveSecondary:
                    await this.CancelStateProviderInitializationAsync();

                    ActorStateData data;
                    if (this.stateTable.TryGetValue(ActorStateType.LogicalTimestamp, LogicalTimestampKey, out data)
                        && data.LogicalTimestamp.HasValue)
                    {
                        this.logicalTimeManager.CurrentLogicalTime = data.LogicalTimestamp.Value;
                    }

                    // Start replication pump if we are changing from primary
                    if (this.replicaRole == ReplicaRole.Primary)
                    {
                        this.secondaryPump.StartReplicationPump();
                    }

                    break;

                case ReplicaRole.Primary:
                    this.stateProviderInitCts = new CancellationTokenSource();
                    this.stateProviderInitTask = this.StartStateProviderInitializationAsync(this.stateProviderInitCts.Token);

                    // Wait for secondary pump to make sure there is no
                    // outstanding task in-flight after processing NULL
                    // operation from the replication queue.
                    await this.secondaryPump.WaitForPumpCompletionAsync();
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
            await this.CancelStateProviderInitializationAsync();

            // Wait for secondary pump to make sure there is no
            // outstanding task in-flight after processing NULL
            // operation from the replication queue.
            await this.secondaryPump.WaitForPumpCompletionAsync();
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
            this.CancelStateProviderInitializationAsync().ContinueWith(
                t => t.Exception,
                TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// Performs a full backup of state managed by this actor state provider
        /// </summary>
        /// <param name="backupCallback">Callback to be called when the backup folder has been created locally and is ready to be moved out of the node.</param>
        /// <returns>Task that represents the asynchronous backup operation.</returns>
        /// <remarks>
        /// Backup/restore is not supported by <see cref="VolatileActorStateProvider"/>.
        /// </remarks>
        Task IStateProviderReplica.BackupAsync(Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture, Actors.SR.ErrorMethodNotSupported, "Backup", this.GetType()));
        }

        /// <summary>
        /// Performs backup of state managed by this actor sate provider.
        /// </summary>
        /// <param name="option">The option for the backup.</param>
        /// <param name="timeout">The timeout for the backup.</param>
        /// <param name="cancellationToken">The cancellation token for the backup.</param>
        /// <param name="backupCallback">The callback to be called once the backup folder is ready.</param>
        /// <returns>Task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// Backup/restore is not supported by <see cref="VolatileActorStateProvider"/>.
        /// </remarks>
        Task IStateProviderReplica.BackupAsync(
            BackupOption option,
            TimeSpan timeout,
            CancellationToken cancellationToken,
            Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture, Actors.SR.ErrorMethodNotSupported, "Backup", this.GetType()));
        }

        /// <summary>
        /// Restore a backup taken by <see cref="IStateProviderReplica.BackupAsync(Func{BackupInfo, CancellationToken, Task{bool}})"/> or
        /// <see cref="IStateProviderReplica.BackupAsync(BackupOption, TimeSpan, CancellationToken, Func{BackupInfo, CancellationToken, Task{bool}})"/>.
        /// </summary>
        /// <param name="backupFolderPath">
        /// The directory where the replica is to be restored from.
        /// </param>
        /// <remarks>
        /// Backup/restore is not supported by <see cref="VolatileActorStateProvider"/>.
        /// </remarks>
        /// <returns>Task that represents the asynchronous restore operation.</returns>
        Task IStateProviderReplica.RestoreAsync(string backupFolderPath)
        {
            throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture, Actors.SR.ErrorMethodNotSupported, "Restore", this.GetType()));
        }

        /// <summary>
        /// Restore a backup taken by <see cref="IStateProviderReplica.BackupAsync(Func{BackupInfo, CancellationToken, Task{bool}})"/> or
        /// <see cref="IStateProviderReplica.BackupAsync(BackupOption, TimeSpan, CancellationToken, Func{BackupInfo, CancellationToken, Task{bool}})"/>.
        /// </summary>
        /// <param name="backupFolderPath">
        /// The directory where the replica is to be restored from.
        /// </param>
        /// <param name="restorePolicy">The restore policy.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>Task that represents the asynchronous restore operation.</returns>
        /// /// <remarks>
        /// Backup/restore is not supported by <see cref="VolatileActorStateProvider"/>.
        /// </remarks>
        Task IStateProviderReplica.RestoreAsync(string backupFolderPath, RestorePolicy restorePolicy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture, Actors.SR.ErrorMethodNotSupported, "Restore", this.GetType()));
        }

        /// <summary>
        /// <para>Obtains context on a Secondary replica after it is created and opened to send context to the Primary replica.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns <see cref="IOperationDataStream" />.</para>
        /// </returns>
        /// <remarks>
        /// <para>The Primary replica analyzes the context and sends back state via <see cref="IStateProvider.GetCopyState(long,IOperationDataStream)" />.</para>
        /// <para>
        ///     <see cref="IStateProvider.GetCopyContext" /> is called on newly created, idle Secondary replicas and provides
        ///     a mechanism to asynchronously establish a bidirectional conversation with the Primary replica. The Secondary replica sends <see cref="OperationData" />
        ///     objects with which the Primary replica can determine the progress of collecting context on the Secondary replica. The Primary replica responds by sending the required state back.
        ///     See <see cref="IStateProvider.GetCopyState(long,IOperationDataStream)" /> at the Primary replica for the other half of the exchange. </para>
        /// <para>For in-memory services, the <see cref="IStateProvider.GetCopyContext" /> method is not called,
        /// as the state of the Secondary replicas is known (they are empty and will require all of the state).</para>
        /// </remarks>
        IOperationDataStream IStateProvider.GetCopyContext()
        {
            // not expected for volatile services
            return null;
        }

        /// <summary>
        /// <para>Obtains the state on a Primary replica that is required to build a Secondary replica.</para>
        /// </summary>
        /// <param name="upToSequenceNumber">
        /// <para>The maximum last sequence number (LSN) that should be placed in the copy stream via the <see cref="IStateReplicator.GetCopyStream" /> method.
        /// LSNs greater than this number are delivered to the Secondary replica as a part of the replication stream via the <see cref="IStateReplicator.GetReplicationStream" /> method.</para>
        /// </param>
        /// <param name="copyContext">
        /// <para>An <see cref="IOperationDataStream" /> that contains the <see cref="OperationData" /> objects that are created by the Secondary replica. </para>
        /// </param>
        /// <returns>
        /// <para>Returns <see cref="IOperationDataStream" />.</para>
        /// </returns>
        /// <remarks>
        /// <para>Just as <see cref="IStateProvider.GetCopyContext" /> enables the Secondary replica to send context to the Primary replica via
        /// an <see cref="IOperationDataStream" />, <see cref="IStateProvider.GetCopyState(long,IOperationDataStream)" /> enables the Primary
        /// replica to respond with an <see cref="IOperationDataStream" />. The stream contains objects that are delivered to the Secondary replica
        /// via the <see cref="IStateReplicator.GetCopyStream" /> method of the <see cref="FabricReplicator" /> class. The objects implement
        /// <see cref="IOperation" /> and contain the specified data. </para>
        /// <para> When the Primary replica receives this call, it should create and return another <see cref="IOperationDataStream" />
        /// that contains <see cref="OperationData" />. <see cref="OperationData" /> represents the data/state that the Secondary replica
        /// requires to catch up to the provided <paramref name="upToSequenceNumber" /> maximum LSN.
        /// How much and which state has to be sent can be determined via the context information that the Secondary replica provides via
        /// <see cref="IStateProvider.GetCopyContext"/> method.</para>
        /// </remarks>
        IOperationDataStream IStateProvider.GetCopyState(long upToSequenceNumber, IOperationDataStream copyContext)
        {
            lock (this.replicationLock)
            {
                var highestSequenceNumber = this.stateTable.GetHighestKnownSequenceNumber();
                if (highestSequenceNumber < upToSequenceNumber)
                {
                    // This is not expected. If we have acquired this.replicationLock
                    // this means our state table must have this sequence number.
                    // Please see comment in method ReplicateStateChangesAsync()
                    var ex = new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Actors.SR.ErrorHighestSequenceNumberLessThanUpToSequenceNumber,
                            highestSequenceNumber,
                            upToSequenceNumber));

                    ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        this.traceId,
                        "IStateProvider.GetCopyState failed with unexpected error: {0}",
                        ex.ToString());

                    throw ex;
                }
            }

            var replicatorSettings = this.stateReplicator.GetReplicatorSettings();
            if (replicatorSettings.MaxReplicationMessageSize == null)
            {
                // This is unexpected. The MaxReplicationMessageSize must not be null after
                // replicator settings was initialized from configuration with valid value.
                var ex = new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Actors.SR.ErrorReplicatorSettings,
                        "MaxReplicationMessageSize"));

                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.traceId,
                    "IStateProvider.GetCopyState failed with unexpected error: {0}",
                    ex.ToString());

                throw ex;
            }

            return new CopyStateEnumerator(
                this.stateTable.GetShallowCopiesEnumerator(upToSequenceNumber),
                this.copyOrReplicationOperationSerializer,
                upToSequenceNumber,
                replicatorSettings.MaxReplicationMessageSize.Value / 2);
        }

        /// <summary>
        /// <para>Obtains the last sequence number that the service has committed. </para>
        /// </summary>
        /// <returns>
        /// <para>Returns <see cref="long" />.</para>
        /// </returns>
        /// <remarks>
        /// <para>This method is called on a service when it first starts up, in case it has any persistent state, and when data loss is suspected.
        /// When a stateful service replica starts up, it has the option to restore any data that might have persisted from previous updates.
        /// If it restores some state in this manner, its current progress is the last written sequence number for that data. A volatile service can simply return 0.
        /// Note that this method is not called to determine a new primary election during fail-over,
        /// because the current committed progress is already known by the <see cref="FabricReplicator" /> class at that time. </para>
        /// </remarks>
        long IStateProvider.GetLastCommittedSequenceNumber()
        {
            return this.stateTable.GetHighestCommittedSequenceNumber();
        }

        /// <summary>
        /// <para>Indicates that a write quorum of replicas in this replica set  has been lost, and that therefore data loss might have occurred.
        /// The replica set consists of a majority of replicas, which includes the Primary replica. </para>
        /// </summary>
        /// <param name="cancellationToken">
        /// <para>The <see cref="CancellationToken" /> object that the operation is observing.
        /// It can be used to send a notification that the operation should be canceled. Note that cancellation is advisory and that the operation might still be completed even if it is canceled.</para>
        /// </param>
        /// <returns>
        /// <para>Returns <see cref="Task{T}" /> of type <see cref="bool" />, that indicates whether state changed.
        /// When it changed, the method returns true or when it did not change, the method returns false.</para>
        /// </returns>
        /// <remarks>
        /// <para>When the Service Fabric runtime observes the failure of a quorum of replicas, which includes the Primary replica,
        /// it elects a new Primary replica and immediately calls this method on the new Primary replica. A Primary replica that is informed of possible data loss
        /// can choose to restore its state from some external data source or can continue to run with the state that it currently has. If the service continues to run with its current state,
        /// it should return false from this method, which indicates that no state change has been made. If it has restored or altered its state,
        /// such as rolling back incomplete work, it should return true. If true is returned, then the state in other replicas must be assumed to be incorrect.
        /// Therefore, the Service Fabric runtime removes the other replicas from the replica set and recreates them.</para>
        /// </remarks>
        Task<bool> IStateProvider.OnDataLossAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// <para>Indicates to a replica that the configuration of a replica set has changed due to a change or attempted change to the Primary replica.
        /// The change occurs due to failure or load balancing of the previous Primary replica. Epoch changes act as a barrier by segmenting operations
        /// into the exact configuration periods in which they were sent by a specific Primary replica.</para>
        /// </summary>
        /// <param name="epoch">
        /// <para>The new <see cref="Epoch" />.</para>
        /// </param>
        /// <param name="previousEpochLastSequenceNumber">
        /// <para> The maximum sequence number (LSN) that should have been observed in the previous epoch.</para>
        /// </param>
        /// <param name="cancellationToken">
        /// <para>The <see cref="CancellationToken" /> object that the operation is observing. It can be used to send a notification
        /// that the operation should be canceled. Note that cancellation is advisory and that the operation might still be completed even if it is canceled.</para>
        /// </param>
        /// <returns>
        /// <para>Returns <see cref="Task" />.</para>
        /// </returns>
        /// <remarks>
        /// <para>This method is called because the Primary replica of the replica set has changed, or a change was attempted.
        /// Secondary replicas receive this method either when they are about to become the new Primary replica, or, if they are not the new Primary replica,
        /// they receive it when they attempt to get the first operation from the new Primary replica from the replication stream.
        /// Primary replicas might occasionally receive this method if there is an attempt to swap the Primary replica, which fails.</para>
        /// <para>The information in the <see cref="IStateProvider.UpdateEpochAsync(Epoch,long,CancellationToken)" />
        /// method enables the service to maintain a progress vector, which is a list of each epoch that the replica has received, and the maximum LSN that they contained. The progress vector data along with the current applied maximum LSN is useful for a Secondary replica to send during the copy operation  to describe how far the operation has progressed. Comparing progress vectors that are received from Secondary replicas during the copy operation enables Primary replicas to determine whether the Secondary replica is up-to-date, what state must be sent to the Secondary replica, and whether the Secondary replica has made false progress. False progress means that an LSN in a previous epoch was greater than the LSN that the Primary replica receives. </para>
        /// </remarks>
        Task IStateProvider.UpdateEpochAsync(
            Epoch epoch,
            long previousEpochLastSequenceNumber,
            CancellationToken cancellationToken)
        {
            // no-op
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        async Task VolatileLogicalTimeManager.ISnapshotHandler.OnSnapshotAsync(TimeSpan currentLogicalTime)
        {
            try
            {
                await this.ReplicateUpdateAsync(
                    ActorStateType.LogicalTimestamp,
                    LogicalTimestampKey,
                    new ActorStateData(currentLogicalTime));
            }
            catch (Exception)
            {
                // Ignore exception.
            }
        }

        internal static OperationData SerializeToOperationData(
            DataContractSerializer serializer,
            CopyOrReplicationOperation copyOrReplicationOperation)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(memoryStream);
                serializer.WriteObject(binaryWriter, copyOrReplicationOperation);
                binaryWriter.Flush();

                return new OperationData(memoryStream.ToArray());
            }
        }

        internal static DataContractSerializer CreateCopyOrReplicationOperationSerializer()
        {
            return new DataContractSerializer(typeof(CopyOrReplicationOperation));
        }

        private static string CreateReminderStorageKey(ActorId actorId, string reminderName)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", actorId.GetStorageKey(), reminderName);
        }

        private static string CreateReminderStorageKeyPrefix(ActorId actorId, string reminderNamePrefix)
        {
            return CreateReminderStorageKey(actorId, reminderNamePrefix);
        }

        private static string CreateActorStorageKey(ActorId actorId, string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
            {
                // Backward compatibility for Actor<TState> (before named actor state was introduced)
                return actorId.GetStorageKey();
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", actorId.GetStorageKey(), stateName);
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

            ActorStateData data;
            if (this.stateTable.TryGetValue(ActorStateType.LogicalTimestamp, LogicalTimestampKey, out data)
                && data.LogicalTimestamp.HasValue)
            {
                this.logicalTimeManager.CurrentLogicalTime = data.LogicalTimestamp.Value;
            }

            this.logicalTimeManager.Start();
            Volatile.Write(ref this.isLogicalTimeManagerInitialized, true);

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Initializing logical time manager SUCCEEDED.");
        }

        private void StopLogicalTimeManager()
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Stopping logical time manager...");

            // Stop logical timer if it is running
            if (this.isLogicalTimeManagerInitialized == true)
            {
                this.logicalTimeManager.Stop();
                this.isLogicalTimeManagerInitialized = false;
            }

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Stopped logical time manager...");
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
                ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Waiting for logical Time manager to be initialized");
                await Task.Delay(retryCount * StateProviderInitRetryDelayMilliseconds, cancellationToken);
            }

            if (this.replicaRole != ReplicaRole.Primary)
            {
                throw new FabricNotPrimaryException(FabricErrorCode.NotPrimary);
            }
        }

        private void LoadActorStateProviderSettings()
        {
            var configPackageName = ActorNameFormat.GetConfigPackageName(this.actorTypeInformation.ImplementationType);
            var sectionName = ActorNameFormat.GetActorStateProviderSettingsSectionName(this.actorTypeInformation.ImplementationType);

            this.stateProviderSettings = VolatileActorStateProviderSettings.LoadFrom(
                this.initParams.CodePackageActivationContext,
                configPackageName,
                sectionName);

            ActorTrace.Source.WriteInfoWithId(
                TraceType, this.traceId, "VolatileActorStateProviderSettingss: {0}", this.stateProviderSettings);
        }

        private List<ActorStateDataWrapper> GetReminderDataWrapperList(IReadOnlyDictionary<ActorId, IReadOnlyCollection<string>> reminderNames)
        {
            var actorStateDataWrapperList = new List<ActorStateDataWrapper>();

            foreach (var reminderNamesPerActor in reminderNames)
            {
                var actorId = reminderNamesPerActor.Key;

                foreach (var reminderName in reminderNamesPerActor.Value)
                {
                    actorStateDataWrapperList.Add(
                        ActorStateDataWrapper.CreateForDelete(
                            ActorStateType.Reminder,
                            CreateReminderStorageKey(actorId, reminderName)));

                    actorStateDataWrapperList.Add(
                        ActorStateDataWrapper.CreateForDelete(
                            ActorStateType.Actor,
                            ActorStateProviderHelper.CreateReminderCompletedStorageKey(actorId, reminderName)));
                }
            }

            return actorStateDataWrapperList;
        }

        private ReplicatorSettings GetReplicatorSettings()
        {
            if (this.userDefinedReplicatorSettings != null)
            {
                return this.userDefinedReplicatorSettings;
            }

            this.initParams.CodePackageActivationContext.ConfigurationPackageModifiedEvent += this.OnConfigurationPackageModified;
            return this.LoadReplicatorSettings();
        }

        private ReplicatorSettings LoadReplicatorSettings()
        {
            return ActorStateProviderHelper.GetActorReplicatorSettings(
                this.initParams.CodePackageActivationContext,
                this.actorTypeInformation.ImplementationType);
        }

        private void OnConfigurationPackageModified(object sender, PackageModifiedEventArgs<ConfigurationPackage> e)
        {
            try
            {
                var replicatorSettings = this.LoadReplicatorSettings();
                this.stateReplicator.UpdateReplicatorSettings(replicatorSettings);
            }
            catch (FabricElementNotFoundException ex)
            {
                // Trace and Report fault when section is not found for ReplicatorSettings.
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.traceId,
                    "FabricElementNotFoundException while loading replicator settings from configuration.",
                    ex);
                this.partition.ReportFault(FaultType.Transient);
            }
            catch (FabricException ex)
            {
                // Trace and Report fault if user intended to provide Replicator Security config but provided it incorrectly.
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.traceId,
                    "FabricException while loading replicator security settings from configuration.",
                    ex);
                this.partition.ReportFault(FaultType.Transient);
            }
            catch (ArgumentException ex)
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.traceId,
                    "ArgumentException while updating replicator settings from configuration.",
                    ex);
                this.partition.ReportFault(FaultType.Transient);
            }
        }

        private Task ReplicateUpdateAsync(ActorStateType type, string key, ActorStateData data)
        {
            return this.ReplicateStateChangesAsync(ActorStateTable.ActorStateDataWrapper.CreateForUpdate(type, key, data));
        }

        private Task ReplicateStateChangesAsync(ActorStateDataWrapper actorStateDataWrapper)
        {
            var actorStateDataWrapperList = new[] { actorStateDataWrapper };
            return this.ReplicateStateChangesAsync(actorStateDataWrapperList);
        }

        private async Task ReplicateStateChangesAsync(IEnumerable<ActorStateDataWrapper> actorStateDataWrapperList)
        {
            Task replicationTask;
            long sequenceNumber;
            Exception replicationException = null;

            var operationData = SerializeToOperationData(
                this.copyOrReplicationOperationSerializer,
                new CopyOrReplicationOperation(actorStateDataWrapperList));

            lock (this.replicationLock)
            {
                // As soon as this call returns replicator assumes that state
                // for this sequence number is available and we may get a call
                // to IStateProvider.GetCopyState() with this sequence number.
                replicationTask = this.stateReplicator.ReplicateAsync(
                    operationData,
                    CancellationToken.None,
                    out sequenceNumber);

                // We add this state to uncommitted list entries before releasing the
                // lock. A call to IStateProvider.GetCopyState() can come before we
                // have executed this which will then block on this.replicationLock.
                this.stateTable.PrepareUpdate(actorStateDataWrapperList, sequenceNumber);
            }

            try
            {
                await replicationTask;
            }
            catch (Exception ex)
            {
                replicationException = ex;
            }

            await this.stateTable.CommitUpdateAsync(sequenceNumber, replicationException);
        }

        [DataContract]
        internal class CopyOrReplicationOperation
        {
            [DataMember]
            private readonly IEnumerable<ActorStateDataWrapper> actorStateDataWrapperList;

            public CopyOrReplicationOperation(IEnumerable<ActorStateDataWrapper> dataWrapperList)
            {
                this.actorStateDataWrapperList = dataWrapperList;
            }

            public IEnumerable<ActorStateDataWrapper> ActorStateDataWrapperList
            {
                get { return this.actorStateDataWrapperList; }
            }
        }

        // Boxes different types of state data for storage in VolatileActorStateTable
        [DataContract]
        internal sealed class ActorStateData
        {
            public ActorStateData(TimeSpan logicalTimestamp)
            {
                this.LogicalTimestamp = logicalTimestamp;
            }

            public ActorStateData(byte[] state)
            {
                this.ActorState = state;
            }

            public ActorStateData(ActorReminderData reminderData)
            {
                this.ActorReminderData = reminderData;
            }

            public ActorStateData(ReminderCompletedData reminderCompletedData)
            {
                this.ReminderLastCompletedData = reminderCompletedData;
            }

            [DataMember]
            public TimeSpan? LogicalTimestamp { get; private set; }

            [DataMember]
            public byte[] ActorState { get; private set; }

            [DataMember]
            public ActorReminderData ActorReminderData { get; private set; }

            [DataMember]
            public ReminderCompletedData ReminderLastCompletedData { get; private set; }

            public long EstimateDataLength()
            {
                var timestampLength = !this.LogicalTimestamp.HasValue ? 0 : sizeof(long);
                var stateLength = this.ActorState == null ? 0 : (this.ActorState.Length * sizeof(byte));
                var reminderLength = this.ActorReminderData == null ? 0 : this.ActorReminderData.EstimateDataLength();
                var reminderCompletedDataLength = this.ReminderLastCompletedData == null ? 0 : this.ReminderLastCompletedData.EstimateDataLength();

                return timestampLength + stateLength + reminderLength + reminderCompletedDataLength;
            }
        }

        internal sealed class CopyStateEnumerator : IOperationDataStream
        {
            private readonly DataContractSerializer copyOperationSerializer;

            private readonly long maxSequenceNumber;
            private readonly long maxDataLength;

            /// <summary>
            /// This LinkedList represents actor state data grouped by sequence number in increasing
            /// order of sequence number. Each entry in the LinkedList contains sequence number and
            /// all the ActorStateDataWrapper entries which belong to that sequence number.
            /// This grouping is required to maintain replication boundary during copy operation
            /// to build a replica.
            /// </summary>
            private readonly LinkedList<CopyStateData> copyStateList;

            public CopyStateEnumerator(
                ActorStateTable.ActorStateEnumerator actorStateDataEnumerator,
                DataContractSerializer copyOperationSerializer,
                long maxSequenceNumber,
                long maxDataLength)
            {
                this.copyOperationSerializer = copyOperationSerializer;
                this.maxSequenceNumber = maxSequenceNumber;
                this.maxDataLength = maxDataLength;

                this.copyStateList = new LinkedList<CopyStateData>();
                this.GroupActorStateDataBySequenceNumber(actorStateDataEnumerator);
            }

            Task<OperationData> IOperationDataStream.GetNextAsync(CancellationToken cancellationToken)
            {
                if (this.copyStateList.Count == 0)
                {
                    return Task.FromResult<OperationData>(null);
                }

                var dataList = new List<ActorStateTable.ActorStateDataWrapper>();

                long totalEstimatedDataLength = 0;
                var nextEstimatedDataLength = this.copyStateList.First.Value.GetEstimatedDataLength();

                do
                {
                    var data = this.copyStateList.First.Value;
                    this.copyStateList.RemoveFirst();

                    // Allow the dataItem to exceed maxDataLength if it's only
                    // one item per copy operation.
                    if (data.SequenceNumber <= this.maxSequenceNumber)
                    {
                        dataList.AddRange(data.ActorStateDataWrapperList);
                        totalEstimatedDataLength += nextEstimatedDataLength;
                    }

                    nextEstimatedDataLength = 0;
                    if (this.copyStateList.Count != 0 && this.copyStateList.First.Value.SequenceNumber <= this.maxSequenceNumber)
                    {
                        nextEstimatedDataLength = this.copyStateList.First.Value.GetEstimatedDataLength();
                    }
                }
                while (nextEstimatedDataLength > 0 && totalEstimatedDataLength + nextEstimatedDataLength <= this.maxDataLength);

                var operationData = SerializeToOperationData(this.copyOperationSerializer, new CopyOrReplicationOperation(dataList));

                return Task.FromResult(operationData);
            }

            private void GroupActorStateDataBySequenceNumber(ActorStateTable.ActorStateEnumerator actorStateEnumerator)
            {
                while (actorStateEnumerator.PeekNext() != null)
                {
                    var peek = actorStateEnumerator.PeekNext();
                    var copyStateDataWrapper = new CopyStateData(peek.SequenceNumber);

                    do
                    {
                        copyStateDataWrapper.ActorStateDataWrapperList.Add(actorStateEnumerator.GetNext());
                        peek = actorStateEnumerator.PeekNext();
                    }
                    while (peek != null && peek.SequenceNumber == copyStateDataWrapper.SequenceNumber);

                    this.copyStateList.AddLast(copyStateDataWrapper);
                }
            }

            private sealed class CopyStateData
            {
                private readonly long sequenceNumber;
                private readonly List<ActorStateDataWrapper> actorStateDataWrapperList;

                public CopyStateData(long sequenceNumber)
                {
                    this.sequenceNumber = sequenceNumber;
                    this.actorStateDataWrapperList = new List<ActorStateDataWrapper>();
                }

                public long SequenceNumber
                {
                    get { return this.sequenceNumber; }
                }

                public List<ActorStateDataWrapper> ActorStateDataWrapperList
                {
                    get { return this.actorStateDataWrapperList; }
                }

                public long GetEstimatedDataLength()
                {
                    long dataLength = 0;

                    foreach (var actorStateDataWrapper in this.ActorStateDataWrapperList)
                    {
                        dataLength += sizeof(int) // Type
                                      + (actorStateDataWrapper.Key.Length * sizeof(char))
                                      + (actorStateDataWrapper.IsDelete ? 0 : actorStateDataWrapper.Value.EstimateDataLength())
                                      + sizeof(long); // SequenceNumber;
                    }

                    return dataLength;
                }
            }
        }

        internal sealed class SecondaryPump
        {
            private readonly IStatefulServicePartition partition;
            private readonly string traceId;
            private readonly ActorStateTable stateTable;
            private readonly IStateReplicator stateReplicator;
            private readonly DataContractSerializer copyOrReplicationOperationSerializer;
            private readonly VolatileLogicalTimeManager logicalTimeManager;
            private Task pumpTask;

            public SecondaryPump(
                IStatefulServicePartition partition,
                ActorStateTable stateTable,
                IStateReplicator stateReplicator,
                DataContractSerializer copyOrReplicationOperationSerializer,
                VolatileLogicalTimeManager logicalTimeManager,
                string traceId)
            {
                this.partition = partition;
                this.stateTable = stateTable;
                this.stateReplicator = stateReplicator;
                this.copyOrReplicationOperationSerializer = copyOrReplicationOperationSerializer;
                this.logicalTimeManager = logicalTimeManager;
                this.pumpTask = null;
                this.traceId = traceId;
            }

            public void StartCopyAndReplicationPump()
            {
                this.pumpTask = this.PumpLoop(true);
            }

            public void StartReplicationPump()
            {
                this.pumpTask = this.PumpLoop(false);
            }

            public async Task WaitForPumpCompletionAsync()
            {
                if (this.pumpTask != null)
                {
                    await this.pumpTask;
                }
            }

            private async Task PumpLoop(bool isCopy)
            {
                try
                {
                    ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.traceId,
                        "Starting PumpLoop. isCopy: {0}",
                        isCopy);

                    var operationStream = this.GetOperationStream(isCopy);

                    var donePumping = false;
                    IOperation operation;
                    do
                    {
                        operation = await operationStream.GetOperationAsync(CancellationToken.None);

                        if (operation != null)
                        {
                            using (var memoryStream = new MemoryStream(operation.Data[0].Array))
                            {
                                var binaryReader = XmlDictionaryReader.CreateBinaryReader(
                                    memoryStream,
                                    XmlDictionaryReaderQuotas.Max);

                                this.DeserializeAndApply(binaryReader, operation, isCopy);
                            }

                            operation.Acknowledge();
                        }
                        else
                        {
                            ActorTrace.Source.WriteInfoWithId(
                                TraceType,
                                this.traceId,
                                "PumpLoop (isCopy: {0}) processed operation NULL.",
                                isCopy);

                            if (isCopy)
                            {
                                // If we are doing copy operation, kick off replication pump now.
                                operationStream = this.GetOperationStream(false);
                                isCopy = false;
                            }
                            else
                            {
                                donePumping = true;
                            }
                        }
                    }
                    while (operation != null || !donePumping);
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        this.traceId,
                        "PumpLoop failed: {0}",
                        ex.ToString());

                    this.partition.ReportFault(FaultType.Transient);
                }
            }

            private IOperationStream GetOperationStream(bool isCopy)
            {
                return isCopy ? this.stateReplicator.GetCopyStream() : this.stateReplicator.GetReplicationStream();
            }

            private void DeserializeAndApply(
                XmlDictionaryReader binaryReader,
                IOperation operation,
                bool isCopy)
            {
                var deserialized = this.copyOrReplicationOperationSerializer.ReadObject(binaryReader);
                if (deserialized is CopyOrReplicationOperation result)
                {
                    var dataWrapperList = result.ActorStateDataWrapperList;

                    if (!isCopy)
                    {
                        foreach (var dataWrapper in dataWrapperList)
                        {
                            dataWrapper.UpdateSequenceNumber(operation.SequenceNumber);
                            this.UpdateLogicalTimestamp(dataWrapper);
                        }
                    }

                    this.stateTable.ApplyUpdates(dataWrapperList);
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Actors.SR.ErrorCasting,
                            deserialized.GetType(),
                            typeof(CopyOrReplicationOperation)));
                }
            }

            private void UpdateLogicalTimestamp(ActorStateDataWrapper dataWrapper)
            {
                if (!dataWrapper.IsDelete && dataWrapper.Value.LogicalTimestamp.HasValue)
                {
                    this.logicalTimeManager.CurrentLogicalTime = dataWrapper.Value.LogicalTimestamp.Value;
                }
            }
        }
    }
}
