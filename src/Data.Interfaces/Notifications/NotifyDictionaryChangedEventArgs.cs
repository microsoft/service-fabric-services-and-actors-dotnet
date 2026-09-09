// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Data.Collections;

namespace Microsoft.ServiceFabric.Data.Notifications
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Identifies the operation that produced a <see cref="NotifyDictionaryChangedEventArgs{TKey, TValue}"/> notification.
    /// </summary>
    /// <remarks>
    /// <see cref="Rebuild"/> is delivered through <see cref="IReliableDictionary{TKey, TValue}.RebuildNotificationAsyncCallback"/>.
    /// All other values are delivered through <see cref="IReliableDictionary{TKey, TValue}.DictionaryChanged"/>.
    /// </remarks>
    public enum NotifyDictionaryChangedAction : int
    {
        /// <summary>
        /// Specifies that a key/value pair was added; cast to <see cref="NotifyDictionaryItemAddedEventArgs{TKey, TValue}"/>.
        /// </summary>
        Add = 0,

        /// <summary>
        /// Specifies that a key's value was replaced; cast to <see cref="NotifyDictionaryItemUpdatedEventArgs{TKey, TValue}"/>.
        /// </summary>
        Update = 1,

        /// <summary>
        /// Specifies that a key was removed; cast to <see cref="NotifyDictionaryItemRemovedEventArgs{TKey, TValue}"/>.
        /// </summary>
        Remove = 2,

        /// <summary>
        /// Specifies that all entries were removed by a clear operation; cast to <see cref="NotifyDictionaryClearEventArgs{TKey, TValue}"/>.
        /// </summary>
        Clear = 3,

        /// <summary>
        /// Specifies that the <see cref="IReliableDictionary{TKey, TValue}"/> was repopulated from copy, restore, or recovery; delivered as <see cref="NotifyDictionaryRebuildEventArgs{TKey, TValue}"/>.
        /// </summary>
        Rebuild = 4
    }

    /// <summary>
    /// Provides data for an <see cref="IReliableDictionary{TKey, TValue}"/> change notification.
    /// </summary>
    /// <remarks>
    /// <see cref="IReliableDictionary{TKey, TValue}.DictionaryChanged"/> notifications are synchronously fired by the <see cref="IReliableDictionary{TKey, TValue}"/> as part of applying the operation.
    /// Holding up the completion of these events can cause the replica to be blocked on the completion of the event.
    /// It is recommended that the events are handled as fast as possible.
    /// </remarks>
    public abstract class NotifyDictionaryChangedEventArgs<TKey, TValue> : EventArgs
    {
        private readonly NotifyDictionaryChangedAction action;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyDictionaryChangedEventArgs{TKey, TValue}"/> class.
        /// </summary>
        public NotifyDictionaryChangedEventArgs(NotifyDictionaryChangedAction action)
        {
            this.action = action;
        }

        /// <summary>
        /// Gets the <see cref="NotifyDictionaryChangedAction"/> that produced the notification.
        /// </summary>
        public NotifyDictionaryChangedAction Action
        {
            get
            {
                return this.action;
            }
        }
    }

    /// <summary>
    /// Provides data for an <see cref="IReliableDictionary{TKey, TValue}.DictionaryChanged"/> notification caused by a transactional operation.
    /// </summary>
    public abstract class NotifyDictionaryTransactionalEventArgs<TKey, TValue> : NotifyDictionaryChangedEventArgs<TKey, TValue>
    {
        private readonly ITransaction transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyDictionaryTransactionalEventArgs{TKey, TValue}"/> class.
        /// </summary>
        // TODO: <exception cref="ArgumentNullException"><paramref name="transaction"/> is <see langword="null"/>.</exception>
        public NotifyDictionaryTransactionalEventArgs(ITransaction transaction, NotifyDictionaryChangedAction action) : base(action)
        {
            this.transaction = transaction;
        }

        /// <summary>
        /// Gets the <see cref="ITransaction"/> that the operation belongs to.
        /// </summary>
        public ITransaction Transaction
        {
            get
            {
                return this.transaction;
            }
        }
    }

    /// <summary>
    /// Provides data for a rebuild notification delivered through <see cref="IReliableDictionary{TKey, TValue}.RebuildNotificationAsyncCallback"/>.
    /// </summary>
    /// <remarks>
    /// A rebuild notification is fired at the end of recovery, copy, or restore of <see cref="IReliableState"/>.
    /// Until the callback completes, rebuild of the <see cref="IReliableDictionary{TKey, TValue}"/> will not complete, blocking the replica from proceeding.
    /// Asynchronous iteration over the <see cref="State"/> may require I/O.
    /// </remarks>
    public class NotifyDictionaryRebuildEventArgs<TKey, TValue> : NotifyDictionaryChangedEventArgs<TKey, TValue>
    {
        private readonly Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<TKey, TValue>> enumerableState;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyDictionaryRebuildEventArgs{TKey, TValue}"/> class.
        /// </summary>
        // TODO: <exception cref="ArgumentNullException"><paramref name="enumerableState"/> is <see langword="null"/>.</exception>
        public NotifyDictionaryRebuildEventArgs(Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<TKey, TValue>> enumerableState) : base(NotifyDictionaryChangedAction.Rebuild)
        {
            this.enumerableState = enumerableState;
        }

        /// <summary>
        /// Gets an <see cref="Data.IAsyncEnumerable{T}"/> that contains all items in the <see cref="IReliableDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// The enumerable is valid only while <see cref="IReliableDictionary{TKey, TValue}.RebuildNotificationAsyncCallback"/> is executing
        /// and becomes invalid once the callback completes.
        /// </remarks>
        public Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<TKey, TValue>> State
        {
            get
            {
                return this.enumerableState;
            }
        }
    }

    /// <summary>
    /// Provides data for an <see cref="IReliableDictionary{TKey, TValue}.DictionaryChanged"/> notification caused by a clear operation.
    /// </summary>
    public class NotifyDictionaryClearEventArgs<TKey, TValue> : NotifyDictionaryChangedEventArgs<TKey, TValue>
    {
        private readonly long commitSequenceNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyDictionaryClearEventArgs{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="commitSequenceNumber">
        /// The commit sequence number of the transaction that cleared the <see cref="IReliableDictionary{TKey, TValue}"/>.
        /// </param>
        public NotifyDictionaryClearEventArgs(long commitSequenceNumber) : base(NotifyDictionaryChangedAction.Clear)
        {
            this.commitSequenceNumber = commitSequenceNumber;
        }

        /// <summary>
        /// Gets the commit sequence number of the transaction that cleared the <see cref="IReliableDictionary{TKey, TValue}"/>.
        /// </summary>
        public long CommitSequenceNumber
        {
            get
            {
                return this.commitSequenceNumber;
            }
        }
    }

    /// <summary>
    /// Provides data for an <see cref="IReliableDictionary{TKey, TValue}.DictionaryChanged"/> notification indicating that a key/value pair was added.
    /// </summary>
    public class NotifyDictionaryItemAddedEventArgs<TKey, TValue> : NotifyDictionaryTransactionalEventArgs<TKey, TValue>
    {
        private readonly TKey key;
        private readonly TValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyDictionaryItemAddedEventArgs{TKey, TValue}"/> class.
        /// </summary>
        public NotifyDictionaryItemAddedEventArgs(ITransaction transaction, TKey key, TValue value) : base(transaction, NotifyDictionaryChangedAction.Add)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Gets the key that was added.
        /// </summary>
        public TKey Key
        {
            get
            {
                return this.key;
            }
        }

        /// <summary>
        /// Gets the value that was added.
        /// </summary>
        public TValue Value
        {
            get
            {
                return this.value;
            }
        }
    }

    /// <summary>
    /// Provides data for an <see cref="IReliableDictionary{TKey, TValue}.DictionaryChanged"/> notification indicating that a key's value was updated.
    /// </summary>
    public class NotifyDictionaryItemUpdatedEventArgs<TKey, TValue> : NotifyDictionaryTransactionalEventArgs<TKey, TValue>
    {
        private readonly TKey key;
        private readonly TValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyDictionaryItemUpdatedEventArgs{TKey, TValue}"/> class.
        /// </summary>
        public NotifyDictionaryItemUpdatedEventArgs(ITransaction transaction, TKey key, TValue value) : base(transaction, NotifyDictionaryChangedAction.Update)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Gets the key whose value was updated.
        /// </summary>
        public TKey Key
        {
            get
            {
                return this.key;
            }
        }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public TValue Value
        {
            get
            {
                return this.value;
            }
        }
    }

    /// <summary>
    /// Provides data for an <see cref="IReliableDictionary{TKey, TValue}.DictionaryChanged"/> notification indicating that a key was removed.
    /// </summary>
    public class NotifyDictionaryItemRemovedEventArgs<TKey, TValue> : NotifyDictionaryTransactionalEventArgs<TKey, TValue>
    {
        private readonly TKey key;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyDictionaryItemRemovedEventArgs{TKey, TValue}"/> class.
        /// </summary>
        public NotifyDictionaryItemRemovedEventArgs(ITransaction transaction, TKey key) : base(transaction, NotifyDictionaryChangedAction.Remove)
        {
            this.key = key;
        }

        /// <summary>
        /// Gets the key that was removed.
        /// </summary>
        public TKey Key
        {
            get
            {
                return this.key;
            }
        }
    }
}
