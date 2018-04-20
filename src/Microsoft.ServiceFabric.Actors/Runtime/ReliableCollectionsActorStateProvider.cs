// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Text;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services;
    using SR = Microsoft.ServiceFabric.Actors.SR;

    /// <summary>
    /// Provides an implementation of <see cref="IActorStateProvider"/> which 
    /// uses <see cref="IReliableStateManager"/> to store and persist the actor state.
    /// </summary>
    public sealed class ReliableCollectionsActorStateProvider :
        IActorStateProvider, VolatileLogicalTimeManager.ISnapshotHandler, IActorStateProviderInternal
    {
        #region Private Data members

        private const int StateProviderInitRetryDelayMilliseconds = 500;
        private const int DefaultActorStateDictionaryCount = 32;
        private const int DefaultReminderDictionaryCount = 8;
        private const string TraceType = "ReliableCollectionsActorStateProvider";
        private const string LogicalTimestampKey = "VLTM";
        private const string ActorStateDictionaryNameFormat = "Store://ActorStateDictionary//{0}";
        private const string ReminderDictionaryNameFormat = "Store://reminderDictionary//{0}";
        private const string ActorPresenceDictionaryName = "Store://ActorPresenceDictionary";
        private const string ReminderCompletedDictionaryName = "Store://ReminderCompletedDataDictionary";
        private const string LogicalTimeDictionaryName = "Store://LogicalTimeDictionary";
        private readonly byte[] actorPresenceValue = { byte.MinValue };

        private readonly ReliableStateManagerConfiguration userDefinedStateManagerConfig;
        private readonly int userDefinedActorStateDictionaryCount;
        private readonly int userDefinedReminderDictionaryCount;
        private readonly ActorStateProviderHelper stateProviderHelper;
        private readonly VolatileLogicalTimeManager logicalTimeManager;
        private readonly ActorStateProviderSerializer actorStateSerializer;

        private string traceId;
        private ReplicaRole replicaRole;
        private IStatefulServicePartition servicePartition;
        private ActorTypeInformation actorTypeInformation;
        private Func<CancellationToken, Task<bool>> onDataLossAsyncFunc;
        private Func<CancellationToken, Task> onRestoreCompletedAsyncFunc;
        private StatefulServiceInitializationParameters initParams;
        private bool isLogicalTimeManagerInitialized;
        private bool isDictionariesInitialized;
        private Task stateProviderInitTask;
        private CancellationTokenSource stateProviderInitCts;
        private IReliableStateManagerReplica2 stateManager;
        private IReliableDictionary2<string, byte[]> actorPresenceDictionary;
        private IReliableDictionary2<string, byte[]> reminderCompletedDictionary;
        private IReliableDictionary2<string, byte[]> logicalTimeDictionary;
        private IReliableDictionary2<string, byte[]>[] actorStateDictionaries;
        private IReliableDictionary2<string, byte[]>[] reminderDictionaries;

        private ReliableCollectionsActorStateProviderSettings stateProviderSettings;
        private long roleChangeTracker;

        #endregion

        #region C'tors

        /// <summary>
        /// <see cref="ReliableCollectionsActorStateProvider"/> is currently in PREVIEW.
        ///  Initializes a new instance of the ReliableCollectionsActorStateProvider class.
        /// </summary>
        public ReliableCollectionsActorStateProvider()
            : this(null)
        {
        }

        /// <summary>
        /// <see cref="ReliableCollectionsActorStateProvider"/> is currently in PREVIEW.
        /// Initializes a new instance of the ReliableCollectionsActorStateProvider class.
        /// with specified configuration. This is currently in PREVIEW.
        /// </summary>
        /// <param name="stateManagerConfig">
        /// A <see cref="ReliableStateManagerConfiguration"/> that describes <see cref="IReliableStateManager"/> configuration. 
        /// </param>
        public ReliableCollectionsActorStateProvider(ReliableStateManagerConfiguration stateManagerConfig)
            : this(stateManagerConfig, DefaultActorStateDictionaryCount, DefaultReminderDictionaryCount)
        {
        }

        /// <summary>
        /// <see cref="ReliableCollectionsActorStateProvider"/> is currently in PREVIEW.
        /// Initializes a new instance of the ReliableDictionaryActorStateProvider class 
        /// with specified configuration.
        /// </summary>
        /// <param name="stateManagerConfig">
        /// A <see cref="ReliableStateManagerConfiguration"/> that describes <see cref="IReliableStateManager"/> configuration.
        /// </param>
        /// <param name="actorStateDictionaryCount">
        /// Number of <see cref="IReliableDictionary2{TKey, TValue}"/> across which actor states will be partitioned and stored.
        /// </param>
        /// <param name="reminderDictionaryCount">
        /// Number of <see cref="IReliableDictionary2{TKey, TValue}"/> across which reminders will be partitioned and stored.
        /// </param>
        /// <remarks>
        /// Values for <paramref name="actorStateDictionaryCount"/> and <paramref name="reminderDictionaryCount"/> can be specified
        /// only once when the Actor Service is created for first time. It cannot be changed after that and 
        /// <see cref="ReliableCollectionsActorStateProvider"/> will ignore any values that are different from first time.
        /// </remarks>
        public ReliableCollectionsActorStateProvider(
            ReliableStateManagerConfiguration stateManagerConfig,
            int actorStateDictionaryCount,
            int reminderDictionaryCount)
        {
            if (actorStateDictionaryCount < 1)
            {
                throw new ArgumentException("Value for actorStateDictionaryCount cannot be less than 1.");
            }

            if (reminderDictionaryCount < 1)
            {
                throw new ArgumentException("Value for reminderDictionaryCount cannot be less than 1.");
            }

            this.traceId = string.Empty;
            this.isLogicalTimeManagerInitialized = false;
            this.isDictionariesInitialized = false;
            this.replicaRole = ReplicaRole.Unknown;
            this.roleChangeTracker = DateTime.UtcNow.Ticks;
            this.userDefinedStateManagerConfig = stateManagerConfig;
            this.userDefinedActorStateDictionaryCount = actorStateDictionaryCount;
            this.userDefinedReminderDictionaryCount = reminderDictionaryCount;
            this.logicalTimeManager = new VolatileLogicalTimeManager(this);
            this.actorStateSerializer = new ActorStateProviderSerializer();
            this.stateProviderHelper = new ActorStateProviderHelper(this);
        }

        #endregion

        #region IActorStateProvider Members

        void IActorStateProvider.Initialize(ActorTypeInformation actorTypeInformation)
        {
            this.actorTypeInformation = actorTypeInformation;
        }

        async Task IActorStateProvider.ActorActivatedAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            await this.EnsureStateProviderInitializedAsync(cancellationToken);

            var key = CreateStorageKeyPrefix(actorId);

            await this.stateProviderHelper.ExecuteWithRetriesAsync(
                async () =>
                {
                    using (var tx = this.stateManager.CreateTransaction())
                    {
                        await this.actorPresenceDictionary.TryAddAsync(tx, key, this.actorPresenceValue);
                        await tx.CommitAsync();
                    }
                },
                string.Format("ActorActivatedAsync[{0}]", actorId),
                cancellationToken);
        }

        async Task IActorStateProvider.ReminderCallbackCompletedAsync(ActorId actorId, IActorReminder reminder, CancellationToken cancellationToken)
        {
            await this.EnsureStateProviderInitializedAsync(cancellationToken);

            var key = CreateStorageKey(actorId, reminder.Name);
            var reminderData = new ReminderCompletedData(this.logicalTimeManager.CurrentLogicalTime, DateTime.UtcNow);
            var data = ReminderCompletedDataSerializer.Serialize(reminderData);

            await this.stateProviderHelper.ExecuteWithRetriesAsync(
                async () =>
                {
                    using (var tx = this.stateManager.CreateTransaction())
                    {
                        await this.reminderCompletedDictionary.AddOrUpdateAsync(tx, key, data, (k, v) => data);
                        await tx.CommitAsync();
                    }
                },
                string.Format("ReminderCallbackCompletedAsync[{0}]", actorId),
                cancellationToken);
        }

        async Task<T> IActorStateProvider.LoadStateAsync<T>(ActorId actorId, string stateName, CancellationToken cancellationToken)
        {
            Requires.Argument("stateName", stateName).NotNull();

            await this.EnsureStateProviderInitializedAsync(cancellationToken);

            var key = CreateStorageKey(actorId, stateName);

            return await this.stateProviderHelper.ExecuteWithRetriesAsync(
                async () =>
                {
                    using (var tx = this.stateManager.CreateTransaction())
                    {
                        var result = await this.GetActorStateDictionary(actorId).TryGetValueAsync(tx, key);

                        if (result.HasValue)
                        {
                            return this.actorStateSerializer.Deserialize<T>(result.Value);
                        }

                        throw new KeyNotFoundException(string.Format(SR.ErrorNamedActorStateNotFound, stateName));
                    }
                },
                string.Format("LoadStateAsync[{0}]", actorId),
                cancellationToken);
        }

        async Task IActorStateProvider.SaveStateAsync(ActorId actorId, IReadOnlyCollection<ActorStateChange> stateChanges, CancellationToken cancellationToken)
        {
            await this.EnsureStateProviderInitializedAsync(cancellationToken);

            var serializedStateChanges = new List<SerializedStateChange>();

            foreach (var stateChange in stateChanges)
            {
                var key = CreateStorageKey(actorId, stateChange.StateName);

                byte[] buffer = null;
                if (stateChange.ChangeKind == StateChangeKind.Add ||
                    stateChange.ChangeKind == StateChangeKind.Update)
                {
                    buffer = this.actorStateSerializer.Serialize(stateChange.Type, stateChange.Value);
                }

                serializedStateChanges.Add(new SerializedStateChange(stateChange.ChangeKind, key, buffer));
            }

            await this.stateProviderHelper.ExecuteWithRetriesAsync(
                () => this.SaveStateAtomicallyAsync(actorId, serializedStateChanges, cancellationToken),
                string.Format("SaveStateAsync[{0}]", actorId),
                cancellationToken);
        }

        async Task<bool> IActorStateProvider.ContainsStateAsync(ActorId actorId, string stateName, CancellationToken cancellationToken)
        {
            Requires.Argument("stateName", stateName).NotNull();

            await this.EnsureStateProviderInitializedAsync(cancellationToken);

            var key = CreateStorageKey(actorId, stateName);

            return await this.stateProviderHelper.ExecuteWithRetriesAsync(
                () =>
                {
                    using (var tx = this.stateManager.CreateTransaction())
                    {
                        return this.GetActorStateDictionary(actorId).ContainsKeyAsync(tx, key);
                    }
                },
                string.Format("ContainsStateAsync[{0}]", actorId),
                cancellationToken);
        }

        async Task IActorStateProvider.RemoveActorAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            await this.EnsureStateProviderInitializedAsync(cancellationToken);

            await this.stateProviderHelper.ExecuteWithRetriesAsync(
                () => this.RemoveActorAtomicallyAsync(actorId, cancellationToken),
                string.Format("RemoveActorAsync[{0}]", actorId),
                cancellationToken);
        }

        async Task<IEnumerable<string>> IActorStateProvider.EnumerateStateNamesAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            await this.EnsureStateProviderInitializedAsync(cancellationToken);

            return await this.stateProviderHelper.ExecuteWithRetriesAsync(
                () => this.GetStateNamesAsync(actorId),
                string.Format("EnumerateStateNamesAsync[{0}]", actorId),
                cancellationToken);
        }

        async Task<PagedResult<ActorId>> IActorStateProvider.GetActorsAsync(int itemsCount, ContinuationToken continuationToken, CancellationToken cancellationToken)
        {
            await this.EnsureStateProviderInitializedAsync(cancellationToken);

            return await this.stateProviderHelper.ExecuteWithRetriesAsync(
                () => this.GetStoredActorIdsAsync(itemsCount, continuationToken, cancellationToken),
                "GetActorsAsync",
                cancellationToken);
        }

        async Task IActorStateProvider.SaveReminderAsync(ActorId actorId, IActorReminder reminder, CancellationToken cancellationToken)
        {
            await this.EnsureStateProviderInitializedAsync(cancellationToken);

            var key = CreateStorageKey(actorId, reminder.Name);
            var reminderData = new ActorReminderData(actorId, reminder, this.logicalTimeManager.CurrentLogicalTime);
            var data = ActorReminderDataSerializer.Serialize(reminderData);

            await this.stateProviderHelper.ExecuteWithRetriesAsync(
                async () =>
                {
                    using (var tx = this.stateManager.CreateTransaction())
                    {
                        await this.GetReminderDictionary(actorId).AddOrUpdateAsync(tx, key, data, (rowKey, rowValue) => data);
                        await this.reminderCompletedDictionary.TryRemoveAsync(tx, key);

                        await tx.CommitAsync();
                    }
                },
                string.Format("SaveReminderAsync[{0}]", actorId),
                cancellationToken);
        }

        async Task<IActorReminderCollection> IActorStateProvider.LoadRemindersAsync(CancellationToken cancellationToken)
        {
            await this.EnsureStateProviderInitializedAsync(cancellationToken);

            return await this.stateProviderHelper.ExecuteWithRetriesAsync(
                () => this.EnumerateRemindersAsync(cancellationToken),
                "LoadRemindersAsync",
                cancellationToken);
        }

        async Task IActorStateProvider.DeleteReminderAsync(ActorId actorId, string reminderName, CancellationToken cancellationToken)
        {
            await this.EnsureStateProviderInitializedAsync(cancellationToken);

            var reminderkey = CreateStorageKey(actorId, reminderName);

            var reminderKeys = new Dictionary<ActorId, IReadOnlyCollection<string>>()
            {
                { actorId, new List<string>() { reminderkey } }
            };

            await this.DeleteRemindersInternalAsync(reminderKeys, $"DeleteReminderAsync[{actorId}]", cancellationToken);
        }

        async Task IActorStateProvider.DeleteRemindersAsync(
            IReadOnlyDictionary<ActorId, IReadOnlyCollection<string>> reminderNames, CancellationToken cancellationToken)
        {
            await this.EnsureStateProviderInitializedAsync(cancellationToken);

            var reminderKeys = this.GetReminderKeys(reminderNames, out var totalCount);

            await this.DeleteRemindersInternalAsync(reminderKeys, $"DeleteRemindersAsync[{totalCount}]", cancellationToken);
        }

        #endregion

        #region ISnapshotHandler Members

        async Task VolatileLogicalTimeManager.ISnapshotHandler.OnSnapshotAsync(TimeSpan currentLogicalTime)
        {
            var logicalTimedata = new LogicalTimestamp(currentLogicalTime);
            var data = LogicalTimestampSerializer.Serialize(logicalTimedata);

            try
            {
                using (var tx = this.stateManager.CreateTransaction())
                {
                    await this.logicalTimeDictionary.AddOrUpdateAsync(tx, LogicalTimestampKey, data, (k, v) => data);
                    await tx.CommitAsync();
                }
            }
            catch (FabricException)
            {
                // Ignore any fabric exception.
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.traceId,
                    string.Format("OnSnapshotAsync() unexpected exception: {0}.", ex.ToString()));
            }
        }

        #endregion

        #region IStateProviderReplica Members

        Func<CancellationToken, Task<bool>> IStateProviderReplica.OnDataLossAsync
        {
            set
            {
                ReleaseAssert.AssertIfNot(this.onDataLossAsyncFunc == null, "ondataloss event handler can only be set once.");
                this.onDataLossAsyncFunc = value;
            }
        }

        void IStateProviderReplica.Initialize(StatefulServiceInitializationParameters initializationParameters)
        {
            this.initParams = initializationParameters;
            this.traceId = ActorTrace.GetTraceIdForReplica(this.initParams.PartitionId, this.initParams.ReplicaId);

            this.LoadActorStateProviderSettings();

            var statefulServiceContext = new StatefulServiceContext(
                FabricRuntime.GetNodeContext(),
                this.initParams.CodePackageActivationContext,
                this.initParams.ServiceTypeName,
                this.initParams.ServiceName,
                this.initParams.InitializationData,
                this.initParams.PartitionId,
                this.initParams.ReplicaId);

            var stateManagerConfig = this.userDefinedStateManagerConfig;

            if (stateManagerConfig == null)
            {
                var actorImplType = this.actorTypeInformation.ImplementationType;

                stateManagerConfig = new ReliableStateManagerConfiguration(
                    ActorNameFormat.GetConfigPackageName(actorImplType),
                    ActorNameFormat.GetFabricServiceReplicatorSecurityConfigSectionName(actorImplType),
                    ActorNameFormat.GetFabricServiceReplicatorConfigSectionName(actorImplType));
            }

            this.stateManager = new ReliableStateManager(statefulServiceContext, stateManagerConfig);

            ReleaseAssert.AssertIfNull(this.onDataLossAsyncFunc, "onDataLossAsync event handler cannot be null.");
            this.stateManager.OnDataLossAsync = this.onDataLossAsyncFunc;

            ReleaseAssert.AssertIfNull(this.onRestoreCompletedAsyncFunc, "onRestoreCompletedAsync event handler cannot be null.");
            this.stateManager.OnRestoreCompletedAsync = this.onRestoreCompletedAsyncFunc;

            this.stateManager.Initialize(this.initParams);
        }

        Task<IReplicator> IStateProviderReplica.OpenAsync(
            ReplicaOpenMode openMode,
            IStatefulServicePartition partition,
            CancellationToken cancellationToken)
        {
            this.servicePartition = partition;
            return this.stateManager.OpenAsync(openMode, partition, cancellationToken);
        }

        async Task IStateProviderReplica.ChangeRoleAsync(
            ReplicaRole newRole,
            CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref this.roleChangeTracker);

            await this.stateManager.ChangeRoleAsync(newRole, cancellationToken);

            // Set replica role for other components to use (logical time manager initialization, client call etc.).
            this.replicaRole = newRole;

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
        }

        async Task IStateProviderReplica.CloseAsync(CancellationToken cancellationToken)
        {
            await this.stateManager.CloseAsync(cancellationToken);
            await this.CancelStateProviderInitializationAsync();
        }

        void IStateProviderReplica.Abort()
        {
            this.stateManager.Abort();
            this.CancelStateProviderInitializationAsync().ContinueWith(t => t.Exception, TaskContinuationOptions.OnlyOnFaulted);
        }

        Task IStateProviderReplica.BackupAsync(Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            return this.stateManager.BackupAsync(backupCallback);
        }

        Task IStateProviderReplica.BackupAsync(
            BackupOption option,
            TimeSpan timeout,
            CancellationToken cancellationToken,
            Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            return this.stateManager.BackupAsync(option, timeout, cancellationToken, backupCallback);
        }

        Task IStateProviderReplica.RestoreAsync(string backupFolderPath)
        {
            return this.stateManager.RestoreAsync(backupFolderPath);
        }

        Task IStateProviderReplica.RestoreAsync(string backupFolderPath, RestorePolicy restorePolicy, CancellationToken cancellationToken)
        {
            return this.stateManager.RestoreAsync(backupFolderPath, restorePolicy, cancellationToken);
        }

        #endregion

        #region IStateProviderReplica2 Members

        Func<CancellationToken, Task> IStateProviderReplica2.OnRestoreCompletedAsync
        {
            set
            {
                ReleaseAssert.AssertIfNot(this.onRestoreCompletedAsyncFunc == null, "onrestorecompleted event handler can only be set once.");
                this.onRestoreCompletedAsyncFunc = value;
            }
        }

        #endregion

        #region Helper Functions

        private void LoadActorStateProviderSettings()
        {
            var configPackageName = ActorNameFormat.GetConfigPackageName(this.actorTypeInformation.ImplementationType);
            var sectionName = ActorNameFormat.GetActorStateProviderSettingsSectionName(this.actorTypeInformation.ImplementationType);

            this.stateProviderSettings = ReliableCollectionsActorStateProviderSettings.LoadFrom(
                this.initParams.CodePackageActivationContext,
                configPackageName,
                sectionName);

            ActorTrace.Source.WriteInfoWithId(
                TraceType, this.traceId, "ReliableCollectionsActorStateProviderSettings: {0}", this.stateProviderSettings);
        }

        private Task DeleteRemindersInternalAsync(
            IReadOnlyDictionary<ActorId, IReadOnlyCollection<string>> reminderKeys,
            string functionNameTag,
            CancellationToken cancellationToken)
        {
            if (reminderKeys.Count == 0)
            {
                return Task.FromResult(true);
            }

            return this.stateProviderHelper.ExecuteWithRetriesAsync(
                async () =>
                {
                    using (var tx = this.stateManager.CreateTransaction())
                    {
                        foreach (var reminderKeysPerActor in reminderKeys)
                        {
                            var actorId = reminderKeysPerActor.Key;

                            foreach (var reminderKey in reminderKeysPerActor.Value)
                            {
                                await this.GetReminderDictionary(actorId).TryRemoveAsync(tx, reminderKey);
                                await this.reminderCompletedDictionary.TryRemoveAsync(tx, reminderKey);
                            }
                        }

                        await tx.CommitAsync();
                    }
                },
                functionNameTag,
                cancellationToken);
        }

        private IReadOnlyDictionary<ActorId, IReadOnlyCollection<string>> GetReminderKeys(
            IReadOnlyDictionary<ActorId, IReadOnlyCollection<string>> reminderNames,
            out int totalCount)
        {
            var reminderKeyInfoList = new Dictionary<ActorId, IReadOnlyCollection<string>>();
            totalCount = 0;

            foreach (var reminderNamesPerActor in reminderNames)
            {
                if (reminderNamesPerActor.Value.Count > 0)
                {
                    var actorId = reminderNamesPerActor.Key;
                    var reminderKeys = new List<string>();

                    foreach (var reminderName in reminderNamesPerActor.Value)
                    {
                        var reminderKey = CreateStorageKey(actorId, reminderName);

                        reminderKeys.Add(reminderKey);
                        totalCount++;
                    }

                    reminderKeyInfoList.Add(actorId, reminderKeys);
                }
            }

            return reminderKeyInfoList;
        }

        private async Task EnsureStateProviderInitializedAsync(CancellationToken cancellationToken)
        {
            var retryCount = 0;

            while (this.replicaRole == ReplicaRole.Primary &&
                   (!this.isDictionariesInitialized || !this.isLogicalTimeManagerInitialized))
            {
                retryCount++;
                await Task.Delay(retryCount * StateProviderInitRetryDelayMilliseconds, cancellationToken);
            }

            if (this.replicaRole != ReplicaRole.Primary)
            {
                throw new FabricNotPrimaryException(FabricErrorCode.NotPrimary);
            }
        }

        private async Task StartStateProviderInitializationAsync(CancellationToken cancellationToken)
        {
            Exception unexpectedException = null;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await this.stateProviderHelper.ExecuteWithRetriesAsync(
                    async () =>
                    {
                        await this.WaitForWriteStatusAsync(cancellationToken);
                        await this.InitializeReliableDictionariesAsync(cancellationToken);
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

                this.servicePartition.ReportFault(FaultType.Transient);
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

            // Stop logical timer if it is running
            if (this.isLogicalTimeManagerInitialized == true)
            {
                ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Stopping logical time manager...");

                this.logicalTimeManager.Stop();
                this.isLogicalTimeManagerInitialized = false;
            }
        }

        private async Task WaitForWriteStatusAsync(CancellationToken cancellationToken)
        {
            var retryCount = 0;

            while (!cancellationToken.IsCancellationRequested &&
                   this.servicePartition.WriteStatus != PartitionAccessStatus.Granted)
            {
                retryCount++;
                await Task.Delay(retryCount * StateProviderInitRetryDelayMilliseconds, cancellationToken);
            }
        }

        private async Task InitializeReliableDictionariesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Registering state provider dictionaries...");

            if (this.isDictionariesInitialized)
            {
                ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Reliable dictionaries already registered.");
                return;
            }

            IReliableDictionary2<string, byte[]> presenceDict;
            IReliableDictionary2<string, byte[]> reminderCompletedDict;
            IReliableDictionary2<string, byte[]> logicalTimeDict;
            Dictionary<int, IReliableDictionary2<string, byte[]>> actorStateDicts;
            Dictionary<int, IReliableDictionary2<string, byte[]>> reminderDicts;

            using (var tx = this.stateManager.CreateTransaction())
            {
                presenceDict = await this.GetOrAddDictionaryAsync(tx, ActorPresenceDictionaryName);
                reminderCompletedDict = await this.GetOrAddDictionaryAsync(tx, ReminderCompletedDictionaryName);
                logicalTimeDict = await this.GetOrAddDictionaryAsync(tx, LogicalTimeDictionaryName);
                actorStateDicts = await this.GetOrAddDictionariesAsync(tx, ActorStateDictionaryNameFormat, this.userDefinedActorStateDictionaryCount);
                reminderDicts = await this.GetOrAddDictionariesAsync(tx, ReminderDictionaryNameFormat, this.userDefinedReminderDictionaryCount);

                // Check for cancellation before commmitting
                cancellationToken.ThrowIfCancellationRequested();

                await tx.CommitAsync();
            }

            this.actorPresenceDictionary = presenceDict;
            this.reminderCompletedDictionary = reminderCompletedDict;
            this.logicalTimeDictionary = logicalTimeDict;
            this.actorStateDictionaries = DictionaryToArray(actorStateDicts);
            this.reminderDictionaries = DictionaryToArray(reminderDicts);

            Volatile.Write(ref this.isDictionariesInitialized, true);
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Registering reliable dictionaries SUCCEEDED.");
        }

        private async Task InitializeAndStartLogicalTimeManagerAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Initializing logical time manager...");

            if (this.isLogicalTimeManagerInitialized == true)
            {
                ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Logical time manager already initialized...");
                return;
            }

            using (var tx = this.stateManager.CreateTransaction())
            {
                var condVal = await this.logicalTimeDictionary.TryGetValueAsync(tx, LogicalTimestampKey);

                if (condVal.HasValue)
                {
                    var logicalTimeStamp = LogicalTimestampSerializer.Deserialize(condVal.Value);
                    this.logicalTimeManager.CurrentLogicalTime = logicalTimeStamp.Timestamp;
                }
            }

            this.logicalTimeManager.Start();
            Volatile.Write(ref this.isLogicalTimeManagerInitialized, true);

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Initializing logical time manager SUCCEEDED.");
        }

        private Task<IReliableDictionary2<string, byte[]>> GetOrAddDictionaryAsync(ITransaction tx, string dictionaryName)
        {
            return this.stateManager.GetOrAddAsync<IReliableDictionary2<string, byte[]>>(tx, dictionaryName);
        }

        private Task<ConditionalValue<IReliableDictionary2<string, byte[]>>> TryGetDictionaryAsync(string dictionaryName)
        {
            return this.stateManager.TryGetAsync<IReliableDictionary2<string, byte[]>>(dictionaryName);
        }

        private async Task<Dictionary<int, IReliableDictionary2<string, byte[]>>> GetOrAddDictionariesAsync(
            ITransaction tx, string dictionaryNameFormat, int dictionaryCount)
        {
            var dicts = new Dictionary<int, IReliableDictionary2<string, byte[]>>();

            var storageIndex = 0;
            var dictName = string.Format(dictionaryNameFormat, storageIndex);
            var condVal = await this.TryGetDictionaryAsync(dictName);

            if (condVal.HasValue)
            {
                do
                {
                    dicts.Add(storageIndex, condVal.Value);
                    storageIndex++;

                    dictName = string.Format(dictionaryNameFormat, storageIndex);
                    condVal = await this.TryGetDictionaryAsync(dictName);
                }
                while (condVal.HasValue);
            }
            else
            {
                // We are here means dictionaries are being created for first time.

                for (var i = 0; i < dictionaryCount; i++)
                {
                    dictName = string.Format(dictionaryNameFormat, i);
                    var dict = await this.GetOrAddDictionaryAsync(tx, dictName);
                    dicts.Add(i, dict);
                }
            }

            return dicts;
        }

        private static T[] DictionaryToArray<T>(Dictionary<int, T> dict)
        {
            var arr = new T[dict.Count];

            foreach (var kvPair in dict)
            {
                arr[kvPair.Key] = kvPair.Value;
            }

            return arr;
        }

        private IReliableDictionary2<string, byte[]> GetActorStateDictionary(ActorId actorId)
        {
            var bytes = Encoding.UTF8.GetBytes(actorId.GetStorageKey());
            var storageIdx = CRC64.ToCRC64(bytes) % (ulong)this.actorStateDictionaries.Length;
            return this.actorStateDictionaries[storageIdx];
        }

        private IReliableDictionary2<string, byte[]> GetReminderDictionary(ActorId actorId)
        {
            var bytes = Encoding.UTF8.GetBytes(actorId.GetStorageKey());
            var storageIdx = CRC64.ToCRC64(bytes) % (ulong)this.reminderDictionaries.Length;
            return this.reminderDictionaries[storageIdx];
        }

        private async Task<Dictionary<string, ReminderCompletedData>> GetReminderCompletedDataMapAsync(CancellationToken cancellationToken)
        {
            var reminderCompletedDataDict = new Dictionary<string, ReminderCompletedData>();

            using (var tx = this.stateManager.CreateTransaction())
            {
                var enumerable = await this.reminderCompletedDictionary.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (enumerator.Current.Value != null)
                    {
                        var reminderCompletedData = ReminderCompletedDataSerializer.Deserialize(enumerator.Current.Value);
                        reminderCompletedDataDict.Add(enumerator.Current.Key, reminderCompletedData);
                    }
                }
            }

            return reminderCompletedDataDict;
        }

        private async Task EnumerateRemindersAsync(
            IReliableDictionary2<string, byte[]> reminderDictionary,
            Dictionary<string, ReminderCompletedData> reminderCompletedDataDict,
            ActorReminderCollection reminderCollection,
            CancellationToken cancellationToken)
        {
            using (var tx = this.stateManager.CreateTransaction())
            {
                var enumerable = await reminderDictionary.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    var data = enumerator.Current.Value;

                    if (data == null)
                    {
                        continue;
                    }

                    var reminderData = ActorReminderDataSerializer.Deserialize(data);
                    var key = CreateStorageKey(reminderData.ActorId, reminderData.Name);

                    reminderCompletedDataDict.TryGetValue(key, out var reminderCompletedData);

                    reminderCollection.Add(
                        reminderData.ActorId,
                        new ActorReminderState(reminderData, this.logicalTimeManager.CurrentLogicalTime, reminderCompletedData));
                }
            }
        }

        private async Task<IActorReminderCollection> EnumerateRemindersAsync(CancellationToken cancellationToken)
        {
            var reminderCollection = new ActorReminderCollection();
            var reminderCompletedDataDict = await this.GetReminderCompletedDataMapAsync(cancellationToken);

            var enumTasks = new List<Task>();

            foreach (var reminderDict in this.reminderDictionaries)
            {
                var tsk = this.EnumerateRemindersAsync(
                    reminderDict, reminderCompletedDataDict, reminderCollection, cancellationToken);

                enumTasks.Add(tsk);
            }

            await Task.WhenAll(enumTasks);

            return reminderCollection;
        }

        private async Task SaveStateAtomicallyAsync(
            ActorId actorId,
            IEnumerable<SerializedStateChange> serializedStateChanges,
            CancellationToken cancellationToken)
        {
            var actorStateDict = this.GetActorStateDictionary(actorId);

            // Check for cancellation before creating the transaction.
            cancellationToken.ThrowIfCancellationRequested();

            using (var tx = this.stateManager.CreateTransaction())
            {
                foreach (var stateChange in serializedStateChanges)
                {
                    switch (stateChange.ChangeKind)
                    {
                        case StateChangeKind.Add:
                            await actorStateDict.AddAsync(tx, stateChange.Key, stateChange.SerializedState);
                            break;
                        case StateChangeKind.Update:
                            await actorStateDict.SetAsync(tx, stateChange.Key, stateChange.SerializedState);
                            break;
                        case StateChangeKind.Remove:
                            await actorStateDict.TryRemoveAsync(tx, stateChange.Key);
                            break;
                        default:
                            throw new InvalidOperationException(Actors.SR.InvalidStateChangeKind);
                    }
                }

                await tx.CommitAsync();
            }
        }

        private async Task<PagedResult<ActorId>> GetStoredActorIdsAsync(int itemsCount, ContinuationToken continuationToken, CancellationToken cancellationToken)
        {
            using (var tx = this.stateManager.CreateTransaction())
            {
                var previousActorCount = continuationToken == null ? 0 : long.Parse((string)continuationToken.Marker);

                long currentActorCount = 0;
                var actorIdList = new List<ActorId>();
                var actorQueryResult = new PagedResult<ActorId>();

                var enumerable = await this.actorPresenceDictionary.CreateKeyEnumerableAsync(tx, EnumerationMode.Ordered);
                var enumerator = enumerable.GetAsyncEnumerator();

                // Move the enumerator to point to first entry
                var enumHasMoreEntries = await enumerator.MoveNextAsync(cancellationToken);

                if (!enumHasMoreEntries)
                {
                    return actorQueryResult;
                }

                // Skip the previous returned entries
                while (currentActorCount < previousActorCount)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    enumHasMoreEntries = await enumerator.MoveNextAsync(cancellationToken);
                    currentActorCount++;

                    if (!enumHasMoreEntries)
                    {
                        // We are here means the current snapshot that enumerator represents
                        // has less number of entries that what ContinuationToken contains.
                        return actorQueryResult;
                    }
                }

                while (enumHasMoreEntries)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var actorId = GetActorIdFromPresenceStorageKey(enumerator.Current);

                    if (actorId != null)
                    {
                        actorIdList.Add(actorId);
                    }
                    else
                    {
                        ActorTrace.Source.WriteWarningWithId(
                            TraceType,
                            this.traceId,
                            string.Format("Failed to parse ActorId from storage key: {0}", enumerator.Current));
                    }

                    enumHasMoreEntries = await enumerator.MoveNextAsync(cancellationToken);
                    currentActorCount++;

                    if (actorIdList.Count == itemsCount)
                    {
                        actorQueryResult.Items = actorIdList.AsReadOnly();

                        // If enumerator has more elements, then set the continuation token
                        if (enumHasMoreEntries)
                        {
                            actorQueryResult.ContinuationToken = new ContinuationToken(currentActorCount.ToString());
                        }

                        return actorQueryResult;
                    }
                }

                // We are here means 'actorIdList' contains less than 'itemsCount' 
                // item or it is empty. The continuation token will remain null.
                actorQueryResult.Items = actorIdList.AsReadOnly();

                return actorQueryResult;
            }
        }

        private static async Task RemoveKeysWithPrefixAsync(
            ITransaction tx,
            IReliableDictionary2<string, byte[]> relDict,
            string keyPrefix,
            CancellationToken cancellationToken)
        {
            var enumerable = await relDict.CreateKeyEnumerableAsync(tx, EnumerationMode.Ordered);
            var enumerator = enumerable.GetAsyncEnumerator();

            var canBreak = false;
            while (await enumerator.MoveNextAsync(cancellationToken))
            {
                if (enumerator.Current.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    canBreak = true;
                    await relDict.TryRemoveAsync(tx, enumerator.Current);
                }
                else
                {
                    if (canBreak)
                    {
                        break;
                    }
                }
            }
        }

        private async Task RemoveActorAtomicallyAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            using (var tx = this.stateManager.CreateTransaction())
            {
                var keyPrefix = CreateStorageKeyPrefix(actorId);

                // Remove actor states
                var actorStateDict = this.GetActorStateDictionary(actorId);
                await RemoveKeysWithPrefixAsync(tx, actorStateDict, keyPrefix, cancellationToken);

                // Remove reminders
                var reminderDict = this.GetReminderDictionary(actorId);
                await RemoveKeysWithPrefixAsync(tx, reminderDict, keyPrefix, cancellationToken);

                // Remove reminder completed data
                await RemoveKeysWithPrefixAsync(
                    tx,
                    this.reminderCompletedDictionary,
                    keyPrefix,
                    cancellationToken);

                // Remove actor presence data
                await this.actorPresenceDictionary.TryRemoveAsync(tx, keyPrefix);

                await tx.CommitAsync();
            }
        }

        private async Task<IEnumerable<string>> GetStateNamesAsync(ActorId actorId)
        {
            var keyPrefix = CreateStorageKeyPrefix(actorId, string.Empty);
            var actorStateDict = this.GetActorStateDictionary(actorId);

            using (var tx = this.stateManager.CreateTransaction())
            {
                var enumerable = await actorStateDict.CreateKeyEnumerableAsync(tx, EnumerationMode.Ordered);
                var enumerator = enumerable.GetAsyncEnumerator();

                var canBreak = false;
                var result = new List<string>();
                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    if (enumerator.Current.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        canBreak = true;
                        result.Add(GetStateNameFromStorageKey(actorId, enumerator.Current));
                    }
                    else
                    {
                        if (canBreak)
                        {
                            break;
                        }
                    }
                }

                return result;
            }
        }

        private static string CreateStorageKeyPrefix(ActorId actorId, string keyPrefix = null)
        {
            if (string.IsNullOrEmpty(keyPrefix))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}_", actorId.GetStorageKey());
            }

            return CreateStorageKey(actorId, keyPrefix);
        }

        private static string CreateStorageKey(ActorId actorId, string stateName)
        {
            Requires.Argument("stateName", stateName.Trim()).NotNullOrEmpty();
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", actorId.GetStorageKey(), stateName);
        }

        private static string GetStateNameFromStorageKey(ActorId actorId, string storageKey)
        {
            var storageKeyPrefix = CreateStorageKeyPrefix(actorId, string.Empty);
            return storageKey.Substring(storageKeyPrefix.Length);
        }

        private static ActorId GetActorIdFromPresenceStorageKey(string storageKey)
        {
            return ActorId.TryGetActorIdFromStorageKey(storageKey.Substring(0, storageKey.Length - 1));
        }

        #endregion

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
            get { return this.replicaRole; }
        }

        TimeSpan IActorStateProviderInternal.TransientErrorRetryDelay
        {
            get { return this.stateProviderSettings.TransientErrorRetryDelay; }
        }

        TimeSpan IActorStateProviderInternal.CurrentLogicalTime
        {
            get { return this.logicalTimeManager.CurrentLogicalTime; }
        }

        TimeSpan IActorStateProviderInternal.OperationTimeout
        {
            get { return this.stateProviderSettings.OperationTimeout; }
        }

        long IActorStateProviderInternal.RoleChangeTracker
        {
            get
            {
                return Interlocked.Read(ref this.roleChangeTracker);
            }
        }

        #endregion
    }
}
