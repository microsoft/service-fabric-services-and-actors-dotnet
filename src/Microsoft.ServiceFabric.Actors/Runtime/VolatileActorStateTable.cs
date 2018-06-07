// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Common;

    internal class VolatileActorStateTable<TType, TKey, TValue>
    {
        private readonly Dictionary<TType, Dictionary<TKey, TableEntry>> committedEntriesTable;

        /// <summary>
        /// Operations are only committed in sequence number order. This is needed
        /// to perform builds correctly - i.e. without sequence number "holes" in
        /// the copy data. ReplicationContext tracks whether a replication operation is
        ///
        ///     1) quorum acked
        ///     2) completed
        ///
        /// A replication operation is only completed when it is quorum acked and there
        /// are no other operations with lower sequence numbers that are not yet
        /// quorum acked.
        /// </summary>
        private readonly Dictionary<long, ReplicationContext> pendingReplicationContexts;

        /// <summary>
        /// Lists of entries are in non-decreasing sequence number order and used to
        /// take a snapshot of the current state when performing builds. The sequence numbers
        /// will not be contiguous if there were deletes.
        /// </summary>
        private readonly LinkedList<ListEntry> committedEntriesList;

        private readonly LinkedList<ListEntry> uncommittedEntriesList;
        private readonly RwLock rwLock;

        #region Public API

        public VolatileActorStateTable()
        {
            this.committedEntriesTable = new Dictionary<TType, Dictionary<TKey, TableEntry>>();
            this.pendingReplicationContexts = new Dictionary<long, ReplicationContext>();
            this.committedEntriesList = new LinkedList<ListEntry>();
            this.uncommittedEntriesList = new LinkedList<ListEntry>();
            this.rwLock = new RwLock();
        }

        public long GetHighestKnownSequenceNumber()
        {
            using (this.rwLock.AcquireReadLock())
            {
                if (this.uncommittedEntriesList.Count > 0)
                {
                    return this.uncommittedEntriesList.Last.Value.ActorStateDataWrapper.SequenceNumber;
                }
                else if (this.committedEntriesList.Count > 0)
                {
                    return this.committedEntriesList.Last.Value.ActorStateDataWrapper.SequenceNumber;
                }
                else
                {
                    return 0;
                }
            }
        }

        public long GetHighestCommittedSequenceNumber()
        {
            using (this.rwLock.AcquireReadLock())
            {
                if (this.committedEntriesList.Count > 0)
                {
                    return this.committedEntriesList.Last.Value.ActorStateDataWrapper.SequenceNumber;
                }
                else
                {
                    return 0;
                }
            }
        }

        public void PrepareUpdate(IEnumerable<ActorStateDataWrapper> actorStateDataWrapperList, long sequenceNumber)
        {
            // Invalid LSN
            if (sequenceNumber == 0)
            {
                return;
            }

            foreach (var actorStateDataWrapper in actorStateDataWrapperList)
            {
                actorStateDataWrapper.UpdateSequenceNumber(sequenceNumber);
            }

            using (this.rwLock.AcquireWriteLock())
            {
                var replicationContext = new ReplicationContext();

                foreach (var actorStateDataWrapper in actorStateDataWrapperList)
                {
                    var entry = new ListEntry(actorStateDataWrapper, replicationContext);
                    this.uncommittedEntriesList.AddLast(entry);
                }

                this.pendingReplicationContexts.Add(sequenceNumber, replicationContext);
            }
        }

        public async Task CommitUpdateAsync(long sequenceNumber, Exception ex = null)
        {
            // Invalid LSN
            if (sequenceNumber == 0)
            {
                if (ex != null)
                {
                    throw ex;
                }

                throw new FabricException(FabricErrorCode.SequenceNumberCheckFailed);
            }

            // This list is used to store the replication contexts that have been commited
            // and are then marked as complete outside the read/write lock. Marking as complete
            // outside the lock is important because when the replication context is marked as
            // complete by calling TaskCompletionSource.SetResult(), the task associated with
            // TaskCompletionSource, immediately starts executing synchronously in the same thread
            // (while the lock still being held) which then tries to again acquire read/write lock
            // causing System.Threading.LockRecursionException.
            //
            // In .Net 4.6, TaskCompletionSource.SetResult() accepts an additional argument which
            // makes the task associated with TaskCompletionSource execute asynchronously on a different
            // thread. Till we move to .Net 4.6, we will adopt the above approach.
            var committedReplicationContexts = new List<ReplicationContext>();

            ReplicationContext replicationContext = null;

            using (this.rwLock.AcquireWriteLock())
            {
                replicationContext = this.pendingReplicationContexts[sequenceNumber];

                replicationContext.SetReplicationComplete(ex);

                if (sequenceNumber == this.uncommittedEntriesList.First.Value.ActorStateDataWrapper.SequenceNumber)
                {
                    while (
                        this.uncommittedEntriesList.Count > 0 &&
                        this.uncommittedEntriesList.First.Value.IsReplicationComplete)
                    {
                        var listNode = this.uncommittedEntriesList.First;

                        this.uncommittedEntriesList.RemoveFirst();

                        if (!listNode.Value.IsFailed)
                        {
                            this.ApplyUpdate_UnderWriteLock(listNode);
                        }

                        listNode.Value.CompleteReplication();

                        var seqNum = listNode.Value.ActorStateDataWrapper.SequenceNumber;
                        if (this.pendingReplicationContexts[seqNum].IsAllEntriesComplete)
                        {
                            committedReplicationContexts.Add(this.pendingReplicationContexts[seqNum]);
                            this.pendingReplicationContexts.Remove(seqNum);
                        }
                    }

                    replicationContext = null;
                }
            }

            // Mark the committed replication contexts as complete in order of increasing LSN
            foreach (var repCtx in committedReplicationContexts)
            {
                repCtx.MarkAsCompleted();
            }

            if (replicationContext != null)
            {
                await replicationContext.WaitForCompletionAsync();
            }
        }

        public void ApplyUpdates(IEnumerable<ActorStateDataWrapper> actorStateDataList)
        {
            using (this.rwLock.AcquireWriteLock())
            {
                foreach (var actorStateData in actorStateDataList)
                {
                    this.ApplyUpdate_UnderWriteLock(new LinkedListNode<ListEntry>(
                        new ListEntry(actorStateData, null)));
                }
            }
        }

        public bool TryGetValue(TType type, TKey key, out TValue value)
        {
            using (this.rwLock.AcquireReadLock())
            {
                var exists = this.committedEntriesTable.TryGetValue(type, out var keyTable);

                if (exists)
                {
                    exists = keyTable.TryGetValue(key, out var tableEntry);

                    value = exists ? tableEntry.ActorStateDataWrapper.Value : default(TValue);
                }
                else
                {
                    value = default(TValue);
                }

                return exists;
            }
        }

        public IEnumerator<TKey> GetSortedStorageKeyEnumerator(TType type, Func<TKey, bool> filter)
        {
            using (this.rwLock.AcquireWriteLock())
            {
                var committedStorageKeyList = new List<TKey>();

                // Though VolatileActorStateTable can use SortedDictionary<>
                // which will also us to always get the keys in sorted order,
                // the lookup in SortedDictionary<> is O(logN) as opposed to
                // Dictionary<> which is average O(1).
                if (this.committedEntriesTable.TryGetValue(type, out var keyTable))
                {
                    foreach (var kvPair in keyTable)
                    {
                        if (filter(kvPair.Key))
                        {
                            committedStorageKeyList.Add(kvPair.Key);
                        }
                    }

                    // SortedList<> is designed to be used when we need the list
                    // to be sorted through its intermediate stages. If the list
                    // needs to be sorted only at the end of adding all entries
                    // then using List<> is better.
                    committedStorageKeyList.Sort();
                }

                return committedStorageKeyList.GetEnumerator();
            }
        }

        public IReadOnlyDictionary<TKey, TValue> GetActorStateDictionary(TType type)
        {
            using (this.rwLock.AcquireReadLock())
            {
                var actorStateDict = new Dictionary<TKey, TValue>();

                if (this.committedEntriesTable.TryGetValue(type, out var keyTable))
                {
                    foreach (var entry in keyTable.Values)
                    {
                        actorStateDict.Add(entry.ActorStateDataWrapper.Key, entry.ActorStateDataWrapper.Value);
                    }
                }

                return actorStateDict;
            }
        }

        public ActorStateEnumerator GetShallowCopiesEnumerator(TType type)
        {
            using (this.rwLock.AcquireReadLock())
            {
                var committedEntriesListShallowCopy = new List<ActorStateDataWrapper>();

                if (this.committedEntriesTable.TryGetValue(type, out var keyTable))
                {
                    foreach (var entry in keyTable.Values)
                    {
                        committedEntriesListShallowCopy.Add(entry.ActorStateDataWrapper);
                    }
                }

                return new ActorStateEnumerator(committedEntriesListShallowCopy, new List<ActorStateDataWrapper>());
            }
        }

        /// <summary>
        /// The use of read/write locks means that the process of creating shallow
        /// copies will necessarily compete with the replication operations. i.e.
        /// The process of preparing for a copy will block replication.
        /// </summary>
        public ActorStateEnumerator GetShallowCopiesEnumerator(long maxSequenceNumber)
        {
            using (this.rwLock.AcquireReadLock())
            {
                var committedEntriesListShallowCopy = new List<ActorStateDataWrapper>();
                var uncommittedEntriesListShallowCopy = new List<ActorStateDataWrapper>();

                long copiedSequenceNumber = 0;
                foreach (var entry in this.committedEntriesList)
                {
                    if (entry.ActorStateDataWrapper.SequenceNumber <= maxSequenceNumber)
                    {
                        copiedSequenceNumber = entry.ActorStateDataWrapper.SequenceNumber;

                        committedEntriesListShallowCopy.Add(entry.ActorStateDataWrapper);
                    }
                    else
                    {
                        break;
                    }
                }

                if (copiedSequenceNumber < maxSequenceNumber)
                {
                    foreach (var entry in this.uncommittedEntriesList)
                    {
                        if (entry.ActorStateDataWrapper.SequenceNumber <= maxSequenceNumber)
                        {
                            uncommittedEntriesListShallowCopy.Add(entry.ActorStateDataWrapper);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                return new ActorStateEnumerator(
                    committedEntriesListShallowCopy,
                    uncommittedEntriesListShallowCopy);
            }
        }

        #endregion Public API

        #region Helper methods

        private void ApplyUpdate_UnderWriteLock(LinkedListNode<ListEntry> listNode)
        {
            var type = listNode.Value.ActorStateDataWrapper.Type;
            var key = listNode.Value.ActorStateDataWrapper.Key;
            var isDelete = listNode.Value.ActorStateDataWrapper.IsDelete;
            var newTableEntry = new TableEntry(
                listNode.Value.ActorStateDataWrapper,
                listNode);

            if (!this.committedEntriesTable.TryGetValue(type, out var keyTable))
            {
                if (isDelete)
                {
                    // Nothing else to do
                    return;
                }

                keyTable = new Dictionary<TKey, TableEntry>();

                this.committedEntriesTable[type] = keyTable;
            }

            if (keyTable.TryGetValue(key, out var oldTableEntry))
            {
                this.committedEntriesList.Remove(oldTableEntry.ListNode);
            }

            if (isDelete)
            {
                keyTable.Remove(key);
            }
            else
            {
                keyTable[key] = newTableEntry;
            }

            // The last element in the committed entries list must reflect the
            // sequence number of the last commit, which may be a delete.
            // If the current last element represents a delete, then it can
            // be removed now since we're adding a new last element.
            if (this.committedEntriesList.Count > 0 &&
                this.committedEntriesList.Last.Value.ActorStateDataWrapper.IsDelete)
            {
                this.committedEntriesList.RemoveLast();
            }

            this.committedEntriesList.AddLast(listNode);
        }

        #endregion Helper methods

        #region Public inner classes

        [DataContract]
        public class ActorStateDataWrapper
        {
            private ActorStateDataWrapper(
                TType type,
                TKey key,
                TValue value)
            {
                this.Type = type;
                this.Key = key;
                this.Value = value;
                this.IsDelete = false;
                this.SequenceNumber = 0;
            }

            private ActorStateDataWrapper(
                TType type,
                TKey key)
            {
                this.Type = type;
                this.Key = key;
                this.Value = default(TValue);
                this.IsDelete = true;
                this.SequenceNumber = 0;
            }

            [DataMember]
            public TType Type { get; private set; }

            [DataMember]
            public TKey Key { get; private set; }

            [DataMember]
            public TValue Value { get; private set; }

            [DataMember]
            public bool IsDelete { get; private set; }

            [DataMember]
            public long SequenceNumber { get; private set; }

            public static ActorStateDataWrapper CreateForUpdate(
                TType type,
                TKey key,
                TValue value)
            {
                return new ActorStateDataWrapper(type, key, value);
            }

            public static ActorStateDataWrapper CreateForDelete(
                TType type,
                TKey key)
            {
                return new ActorStateDataWrapper(type, key);
            }

            internal void UpdateSequenceNumber(long sequenceNumber)
            {
                this.SequenceNumber = sequenceNumber;
            }
        }

        public class ActorStateEnumerator : IReadOnlyCollection<ActorStateDataWrapper>,
            IEnumerator<ActorStateDataWrapper>
        {
            private readonly List<ActorStateDataWrapper> committedEntriesListShallowCopy;
            private readonly List<ActorStateDataWrapper> uncommittedEntriesListShallowCopy;

            private int index;
            private ActorStateDataWrapper current;

            public ActorStateEnumerator(
                List<ActorStateDataWrapper> committedEntriesList,
                List<ActorStateDataWrapper> uncommittedEntriesList)
            {
                this.committedEntriesListShallowCopy = committedEntriesList;
                this.uncommittedEntriesListShallowCopy = uncommittedEntriesList;
                this.index = 0;
                this.current = null;
            }

            public long CommittedCount
            {
                get { return this.committedEntriesListShallowCopy.Count; }
            }

            public long UncommittedCount
            {
                get { return this.uncommittedEntriesListShallowCopy.Count; }
            }

            int IReadOnlyCollection<ActorStateDataWrapper>.Count
            {
                get
                {
                    return this.committedEntriesListShallowCopy.Count + this.uncommittedEntriesListShallowCopy.Count;
                }
            }

            ActorStateDataWrapper IEnumerator<ActorStateDataWrapper>.Current
            {
                get { return this.current; }
            }

            object IEnumerator.Current
            {
                get { return this.current; }
            }

            public ActorStateDataWrapper PeekNext()
            {
                ActorStateDataWrapper item = null;

                var committedCount = this.committedEntriesListShallowCopy.Count;
                var uncommittedCount = this.uncommittedEntriesListShallowCopy.Count;

                var next = this.index;

                if (next < committedCount)
                {
                    item = this.committedEntriesListShallowCopy[next];
                }
                else if (next < uncommittedCount + committedCount)
                {
                    item = this.uncommittedEntriesListShallowCopy[next - committedCount];
                }
                else
                {
                    item = null;
                }

                return item;
            }

            public ActorStateDataWrapper GetNext()
            {
                var committedCount = this.committedEntriesListShallowCopy.Count;
                var uncommittedCount = this.uncommittedEntriesListShallowCopy.Count;

                var next = this.index++;

                if (next < committedCount)
                {
                    this.current = this.committedEntriesListShallowCopy[next];
                }
                else if (next < uncommittedCount + committedCount)
                {
                    this.current = this.uncommittedEntriesListShallowCopy[next - committedCount];
                }
                else
                {
                    this.current = null;
                }

                return this.current;
            }

            IEnumerator<ActorStateDataWrapper> IEnumerable<ActorStateDataWrapper>.GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }

            void IDisposable.Dispose()
            {
                // no-op
            }

            bool IEnumerator.MoveNext()
            {
                return this.GetNext() != null;
            }

            void IEnumerator.Reset()
            {
                this.index = 0;
                this.current = null;
            }
        }

        #endregion Public inner classes

        #region Private inner classes

        private class ReplicationContext
        {
            private readonly TaskCompletionSource<object> pendingCommitTaskSource;
            private Exception replicationException;
            private long associatedEntryCount;

            public ReplicationContext()
            {
                this.IsReplicationComplete = false;
                this.replicationException = null;
                this.pendingCommitTaskSource = new TaskCompletionSource<object>();
                this.associatedEntryCount = 0;
            }

            public bool IsReplicationComplete { get; private set; }

            public bool IsFailed
            {
                get
                {
                    return (this.replicationException != null);
                }
            }

            public bool IsAllEntriesComplete
            {
                get { return (this.associatedEntryCount == 0); }
            }

            public void SetReplicationComplete(Exception replicationException)
            {
                this.IsReplicationComplete = true;
                this.replicationException = replicationException;
            }

            public void AssociateListEntry()
            {
                this.associatedEntryCount++;
            }

            public void CompleteListEntry()
            {
                this.associatedEntryCount--;
            }

            public void MarkAsCompleted()
            {
                if (this.replicationException != null)
                {
                    this.pendingCommitTaskSource.SetException(this.replicationException);
                }
                else
                {
                    this.pendingCommitTaskSource.SetResult(null);
                }
            }

            public Task WaitForCompletionAsync()
            {
                return this.pendingCommitTaskSource.Task;
            }
        }

        private class ListEntry
        {
            public ListEntry(
                ActorStateDataWrapper actorStateDataWrapper,
                ReplicationContext replicationContext)
            {
                this.ActorStateDataWrapper = actorStateDataWrapper;
                this.PendingReplicationContext = replicationContext;

                if (this.PendingReplicationContext != null)
                {
                    this.PendingReplicationContext.AssociateListEntry();
                }
            }

            public bool IsReplicationComplete
            {
                get { return (this.PendingReplicationContext == null || this.PendingReplicationContext.IsReplicationComplete); }
            }

            public bool IsFailed
            {
                get
                {
                    if (this.PendingReplicationContext != null)
                    {
                        return this.PendingReplicationContext.IsFailed;
                    }

                    return false;
                }
            }

            public ActorStateDataWrapper ActorStateDataWrapper { get; private set; }

            private ReplicationContext PendingReplicationContext { get; set; }

            public void CompleteReplication()
            {
                if (this.PendingReplicationContext != null)
                {
                    this.PendingReplicationContext.CompleteListEntry();
                    this.PendingReplicationContext = null;
                }
            }
        }

        private class TableEntry
        {
            public TableEntry(
                ActorStateDataWrapper actorStateDataWrapper,
                LinkedListNode<ListEntry> listNode)
            {
                this.ActorStateDataWrapper = actorStateDataWrapper;
                this.ListNode = listNode;
            }

            public ActorStateDataWrapper ActorStateDataWrapper { get; private set; }

            public LinkedListNode<ListEntry> ListNode { get; private set; }
        }

        #endregion Private inner classes
    }
}
