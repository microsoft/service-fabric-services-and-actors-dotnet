// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.StateMigration.Tests.MockTypes
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Moq;

    internal class MockReliableCollectionsStateProvider :
        IActorStateProvider, IActorStateProviderInternal, IReliableCollectionsActorStateProviderInternal
    {
        private IReliableDictionary2<string, byte[]> presenceDict = new MockReliableDictionary<string, byte[]>();
        private IReliableDictionary2<string, byte[]> stateDict = new MockReliableDictionary<string, byte[]>();
        private IReliableDictionary2<string, byte[]> reminderDict = new MockReliableDictionary<string, byte[]>();
        private IReliableDictionary2<string, byte[]> reminderCompletedDict = new MockReliableDictionary<string, byte[]>();
        private IReliableDictionary2<string, byte[]> logicalTimeDict = new MockReliableDictionary<string, byte[]>();

        public Func<CancellationToken, Task> OnRestoreCompletedAsync { set => throw new NotImplementedException(); }

        public Func<CancellationToken, Task<bool>> OnDataLossAsync { set => throw new NotImplementedException(); }

        public string TraceType { get => "MockReliableCollectionsStateProvider"; }

        public string TraceId { get => "MockReliableCollectionsStateProvider"; }

        public ReplicaRole CurrentReplicaRole { get => ReplicaRole.Primary; }

        public TimeSpan TransientErrorRetryDelay { get => TimeSpan.FromSeconds(1); }

        public TimeSpan OperationTimeout { get => TimeSpan.FromSeconds(10); }

        public TimeSpan CurrentLogicalTime { get => TimeSpan.FromSeconds(1); }

        public long RoleChangeTracker { get => 0L; }

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public Task ActorActivatedAsync(ActorId actorId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task BackupAsync(Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            throw new NotImplementedException();
        }

        public Task BackupAsync(BackupOption option, TimeSpan timeout, CancellationToken cancellationToken, Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            throw new NotImplementedException();
        }

        public Task ChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ContainsStateAsync(ActorId actorId, string stateName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteReminderAsync(ActorId actorId, string reminderName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRemindersAsync(IReadOnlyDictionary<ActorId, IReadOnlyCollection<string>> reminderNames, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> EnumerateStateNamesAsync(ActorId actorId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IReliableDictionary2<string, byte[]> GetActorPresenceDictionary()
        {
            return this.presenceDict;
        }

        public Task<PagedResult<ActorId>> GetActorsAsync(int numItemsToReturn, ContinuationToken continuationToken, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IReliableDictionary2<string, byte[]> GetActorStateDictionary(ActorId actorId)
        {
            return this.stateDict;
        }

        public ActorStateProviderHelper GetActorStateProviderHelper()
        {
           return new ActorStateProviderHelper(this);
        }

        public IReliableDictionary2<string, byte[]> GetLogicalTimeDictionary()
        {
            return this.logicalTimeDict;
        }

        public IReliableDictionary2<string, byte[]> GetReminderCompletedDictionary()
        {
            return this.reminderCompletedDict;
        }

        public IReliableDictionary2<string, byte[]> GetReminderDictionary(ActorId actorId)
        {
            return this.reminderDict;
        }

        public Task<ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>> GetRemindersAsync(int numItemsToReturn, ActorId actorId, ContinuationToken continuationToken, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IReliableStateManagerReplica2 GetStateManager()
        {
            var mockSM = new Mock<IReliableStateManagerReplica2>();
            mockSM.Setup(sm => sm.CreateTransaction()).Returns(() =>
            {
                var mockTx = new Mock<Data.ITransaction>();
                mockTx.Setup(tx => tx.CommitAsync()).Returns(Task.CompletedTask);
                return mockTx.Object;
            });

            return mockSM.Object;
        }

        public void Initialize(ActorTypeInformation actorTypeInformation)
        {
            throw new NotImplementedException();
        }

        public void Initialize(StatefulServiceInitializationParameters initializationParameters)
        {
            throw new NotImplementedException();
        }

        public Task<IActorReminderCollection> LoadRemindersAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<T> LoadStateAsync<T>(ActorId actorId, string stateName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReplicator> OpenAsync(ReplicaOpenMode openMode, IStatefulServicePartition partition, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ReminderCallbackCompletedAsync(ActorId actorId, IActorReminder reminder, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveActorAsync(ActorId actorId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RestoreAsync(string backupFolderPath)
        {
            throw new NotImplementedException();
        }

        public Task RestoreAsync(string backupFolderPath, RestorePolicy restorePolicy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SaveReminderAsync(ActorId actorId, IActorReminder reminder, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SaveStateAsync(ActorId actorId, IReadOnlyCollection<ActorStateChange> stateChanges, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
