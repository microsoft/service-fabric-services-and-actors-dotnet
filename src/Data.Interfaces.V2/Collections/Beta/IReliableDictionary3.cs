// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data.Collections.Beta
{
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// (Beta) Not for production use - API is subject to change in the future.
    /// Represents a reliable collection of key/value pairs that are persisted and replicated.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the reliable dictionary.</typeparam>
    /// <typeparam name="TValue">
    /// The type of the values in the reliable dictionary.</typeparam>
    /// <remarks>Keys or values stored in this dictionary MUST NOT be mutated outside the context of an operation on the
    /// dictionary.  It is highly recommended to make both <typeparamref name="TKey"/> and <typeparamref name="TValue"/>
    /// immutable in order to avoid accidental data corruption.
    ///
    /// <para>
    /// The transaction is the unit of concurrency. Users can have multiple transactions in-flight at any given point of time, but for a given transaction each API must be called one at a time.
    /// When calling any asynchronous Reliable Collection method that takes an <see cref="ITransaction"/>, you must wait for completion of the returned Task before calling
    /// another method using the same transaction.
    /// </para>
    /// </remarks>
    public interface IReliableDictionary3<TKey, TValue> : IReliableDictionary2<TKey, TValue>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        /// <summary>
        /// (Beta) Attempts to get the sequence number associated with the specified key from the Reliable Dictionary.
        /// </summary>
        /// <param name="tx">Transaction to associate this operation with.</param>
        /// <param name="key">The key of the element's sequence number to get.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is null, or <paramref name="key"/> is null or cannot be serialized.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="FabricNotReadableException">
        /// Indicates that the IReliableDictionary cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">Indicates that the Reliable Dictionary is closed or deleted.</exception>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result is a tuple indicating
        /// whether the key was found in the Reliable Dictionary and if found, the sequence number.
        /// </returns>
        Task<ConditionalValue<long>> TryGetSequenceNumberAsync(
            ITransaction tx,
            TKey key);

        /// <summary>
        /// (Beta) Attempts to get the sequence number associated with the specified key from the Reliable Dictionary.
        /// </summary>
        /// <param name="tx">Transaction to associate this operation with.</param>
        /// <param name="key">The key of the element's sequence number to get.</param>
        /// <param name="lockMode">Type of locking to use for this read operation.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is null, or <paramref name="key"/> is null or cannot be serialized.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="FabricNotReadableException">
        /// Indicates that the IReliableDictionary cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">Indicates that the Reliable Dictionary is closed or deleted.</exception>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result is a tuple indicating
        /// whether the key was found in the Reliable Dictionary and if found, the sequence number.
        /// </returns>
        Task<ConditionalValue<long>> TryGetSequenceNumberAsync(
            ITransaction tx,
            TKey key,
            LockMode lockMode);

        /// <summary>
        /// (Beta) Attempts to get the sequence number associated with the specified key from the Reliable Dictionary.
        /// </summary>
        /// <param name="tx">Transaction to associate this operation with.</param>
        /// <param name="key">The key of the element's sequence number to get.</param>
        /// <param name="lockMode">Type of locking to use for this read operation.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a TimeoutException. Primarily used to prevent deadlocks. The default is 4 seconds.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default is None.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is null, or <paramref name="key"/> is null or cannot be serialized.</exception>
        /// <exception cref="ArgumentException"><paramref name="timeout"/> is negative.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="FabricNotReadableException">
        /// Indicates that the IReliableDictionary cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">Indicates that the Reliable Dictionary is closed or deleted.</exception>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result is a tuple indicating
        /// whether the key was found in the Reliable Dictionary and if found, the sequence number.
        /// </returns>
        Task<ConditionalValue<long>> TryGetSequenceNumberAsync(
            ITransaction tx,
            TKey key,
            LockMode lockMode,
            TimeSpan timeout,
            CancellationToken cancellationToken);


        /// <summary>
        /// (Beta) Attempts to get the versioned element associated with the specified key from the Reliable Dictionary.
        /// </summary>
        /// <param name="tx">Transaction to associate this operation with.</param>
        /// <param name="key">The key of the versioned element to get.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is null, or <paramref name="key"/> is null or cannot be serialized.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="FabricNotReadableException">
        /// Indicates that the IReliableDictionary cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">Indicates that the Reliable Dictionary is closed or deleted.</exception>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result is a tuple indicating
        /// whether the key was found in the Reliable Dictionary and if found, the value and sequence number.
        /// </returns>
        /// <returns></returns>
        Task<ConditionalValue<VersionedKeyValuePair<TKey, TValue>>> TryGetVersionedKeyValuePairAsync(
            ITransaction tx,
            TKey key);

        /// <summary>
        /// (Beta) Attempts to get the versioned element associated with the specified key from the Reliable Dictionary.
        /// </summary>
        /// <param name="tx">Transaction to associate this operation with.</param>
        /// <param name="key">The key of the versioned element to get.</param>
        /// <param name="lockMode">Type of locking to use for this read operation.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is null, or <paramref name="key"/> is null or cannot be serialized.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="FabricNotReadableException">
        /// Indicates that the IReliableDictionary cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">Indicates that the Reliable Dictionary is closed or deleted.</exception>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result is a tuple indicating
        /// whether the key was found in the Reliable Dictionary and if found, the value and sequence number.
        /// </returns>
        Task<ConditionalValue<VersionedKeyValuePair<TKey, TValue>>> TryGetVersionedKeyValuePairAsync(
            ITransaction tx,
            TKey key,
            LockMode lockMode);

        /// <summary>
        /// (Beta) Attempts to get the versioned element associated with the specified key from the Reliable Dictionary.
        /// </summary>
        /// <param name="tx">Transaction to associate this operation with.</param>
        /// <param name="key">The key of the versioned element to get.</param>
        /// <param name="lockMode">Type of locking to use for this read operation.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a TimeoutException. Primarily used to prevent deadlocks. The default is 4 seconds.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default is None.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is null, or <paramref name="key"/> is null or cannot be serialized.</exception>
        /// <exception cref="ArgumentException"><paramref name="timeout"/> is negative.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="FabricNotReadableException">
        /// Indicates that the IReliableDictionary cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">Indicates that the Reliable Dictionary is closed or deleted.</exception>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result is a tuple indicating
        /// whether the key was found in the Reliable Dictionary and if found, the value and sequence number.
        /// </returns>
        Task<ConditionalValue<VersionedKeyValuePair<TKey, TValue>>> TryGetVersionedKeyValuePairAsync(
            ITransaction tx,
            TKey key,
            LockMode lockMode,
            TimeSpan timeout,
            CancellationToken cancellationToken);

        /// <summary>
        /// (Beta) Attempts to update the value for the specified key given the sequence number is matching.
        /// </summary>
        /// <param name="tx">Transaction to associate this operation with.</param>
        /// <param name="key">The key of the element to be updated.</param>
        /// <param name="newValue">The value to be updated to if the specified <paramref name="key"/> has the expected <paramref name="checkSequenceNumber"/>.</param>
        /// <param name="checkSequenceNumber">The expected sequence number of the element to be updated.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is null, or <paramref name="key"/> is null or cannot be serialized.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <exception cref="FabricNotPrimaryException">The exception that is thrown when the <see cref="IReliableDictionary{TKey, TValue}"/> is not in <see cref="ReplicaRole.Primary"/>.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">Indicates that the Reliable Dictionary is closed or deleted.</exception>
        /// <returns>A task that represents the asynchronous update operation. The task result indicates whether the object was updated.</returns>
        Task<bool> TryUpdateAsync(ITransaction tx, TKey key, TValue newValue, long checkSequenceNumber);

        /// <summary>
        /// (Beta) Attempts to update the value for the specified key given the sequence number is matching.
        /// </summary>
        /// <param name="tx">Transaction to associate this operation with.</param>
        /// <param name="key">The key of the element to be updated.</param>
        /// <param name="newValue">The value to be updated to if the specified <paramref name="key"/> has the expected <paramref name="checkSequenceNumber"/>.</param>
        /// <param name="checkSequenceNumber">The expected sequence number of the element to be updated.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a TimeoutException. Primarily used to prevent deadlocks. The default is 4 seconds.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default is None.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is null, or <paramref name="key"/> is null or cannot be serialized.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <exception cref="FabricNotPrimaryException">The exception that is thrown when the <see cref="IReliableDictionary{TKey, TValue}"/> is not in <see cref="ReplicaRole.Primary"/>.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">Indicates that the Reliable Dictionary is closed or deleted.</exception>
        /// <returns>A task that represents the asynchronous update operation. The task result indicates whether the object was updated.</returns>
        Task<bool> TryUpdateAsync(ITransaction tx, TKey key, TValue newValue, long checkSequenceNumber, TimeSpan timeout, CancellationToken cancellationToken);

        /// <summary>
        /// (Beta) Attempts to remove the value with the specified key given the sequence number is matching.
        /// </summary>
        /// <param name="tx">Transaction to associate this operation with.</param>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="checkSequenceNumber">The expected sequence number of the element to be removed.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is null, or <paramref name="key"/> is null or cannot be serialized.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="FabricNotPrimaryException">The exception that is thrown when the <see cref="IReliableDictionary{TKey, TValue}"/> is not in <see cref="ReplicaRole.Primary"/>.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">Indicates that the Reliable Dictionary is closed or deleted.</exception>
        /// <returns>
        /// Task that represents the asynchronous remove operation. The task result is a tuple indicating
        /// whether the key was removed from the Reliable Dictionary and if so, the value.
        /// </returns>
        Task<bool> TryRemoveAsync(ITransaction tx, TKey key, long checkSequenceNumber);

        /// <summary>
        /// (Beta) Attempts to remove the value with the specified key given the sequence number is matching.
        /// </summary>
        /// <param name="tx">Transaction to associate this operation with.</param>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="checkSequenceNumber">The expected sequence number of the element to be removed.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a TimeoutException. Primarily used to prevent deadlocks. The default is 4 seconds.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default is None.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is null, or <paramref name="key"/> is null or cannot be serialized.</exception>
        /// <exception cref="ArgumentException"><paramref name="timeout"/> is negative.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="FabricNotPrimaryException">The exception that is thrown when the <see cref="IReliableDictionary{TKey, TValue}"/> is not in <see cref="ReplicaRole.Primary"/>.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">Indicates that the Reliable Dictionary is closed or deleted.</exception>
        /// <returns>
        /// Task that represents the asynchronous remove operation. The task result is a tuple indicating
        /// whether the key was removed from the Reliable Dictionary and if so, the value.
        /// </returns>
        Task<bool> TryRemoveAsync(ITransaction tx, TKey key, long checkSequenceNumber, TimeSpan timeout, CancellationToken cancellationToken);

        /// <summary>
        /// (Beta) Creates an async enumerator over the <see cref="IReliableDictionary3{TKey,TValue}"/> to enumerate the versioned keys.
        /// </summary>
        /// <param name="txn">Transaction to associate this operation with.</param>
        /// <exception cref="FabricNotReadableException">
        /// Exception indicates that the Reliable Dictionary cannot serve reads at the moment.
        /// <see cref="FabricNotReadableException"/> can be thrown in all <see cref="ReplicaRole"/>s.
        /// One example for it being thrown in the <see cref="ReplicaRole.Primary"/> is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One example for it being thrown in the <see cref="ReplicaRole.ActiveSecondary"/> is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">Indicates that the Reliable Dictionary is closed or deleted.</exception>
        /// <remarks>
        /// The enumerable returned from the <see cref="IReliableDictionary3{TKey,TValue}"/> is safe to use concurrently with reads and writes
        /// to the dictionary. It represents a snapshot consistent view of the dictionary.
        /// </remarks>
        /// <returns>An enumerable for the <see cref="IReliableDictionary3{TKey,TValue}"/> versioned keys.</returns>
        Task<IAsyncEnumerable<VersionedKey<TKey>>> CreateVersionedKeyEnumerableAsync(ITransaction txn);

        /// <summary>
        /// (Beta) Creates an async enumerator over the <see cref="IReliableDictionary3{TKey,TValue}"/> to enumerate the versioned keys.
        /// </summary>
        /// <param name="txn">Transaction to associate this operation with.</param>
        /// <param name="firstKey">The key to start enumerating from in ordered enumeration.</param>
        /// <exception cref="FabricNotReadableException">
        /// Exception indicates that the Reliable Dictionary cannot serve reads at the moment.
        /// <see cref="FabricNotReadableException"/> can be thrown in all <see cref="ReplicaRole"/>s.
        /// One example for it being thrown in the <see cref="ReplicaRole.Primary"/> is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One example for it being thrown in the <see cref="ReplicaRole.ActiveSecondary"/> is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">Indicates that the Reliable Dictionary is closed or deleted.</exception>
        /// <remarks>
        /// The enumerable returned from the <see cref="IReliableDictionary3{TKey,TValue}"/> is safe to use concurrently with reads and writes
        /// to the dictionary. It represents a snapshot consistent view of the dictionary.
        /// </remarks>
        /// <returns>An enumerable for the <see cref="IReliableDictionary3{TKey,TValue}"/> versioned keys.</returns>
        Task<IAsyncEnumerable<VersionedKey<TKey>>> CreateVersionedKeyEnumerableAsync(ITransaction txn, TKey firstKey);

        /// <summary>
        /// (Beta) Creates an async enumerator over the <see cref="IReliableDictionary3{TKey,TValue}"/> to enumerate the versioned keys.
        /// </summary>
        /// <param name="txn">Transaction to associate this operation with.</param>
        /// <param name="firstKey">The key to start enumerating from in ordered enumeration.</param>
        /// <param name="lastKey">The key to stop enumerating at in ordered enumeration.</param>
        /// <exception cref="FabricNotReadableException">
        /// Exception indicates that the Reliable Dictionary cannot serve reads at the moment.
        /// <see cref="FabricNotReadableException"/> can be thrown in all <see cref="ReplicaRole"/>s.
        /// One example for it being thrown in the <see cref="ReplicaRole.Primary"/> is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One example for it being thrown in the <see cref="ReplicaRole.ActiveSecondary"/> is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">Indicates that the Reliable Dictionary is closed or deleted.</exception>
        /// <remarks>
        /// The enumerable returned from the <see cref="IReliableDictionary3{TKey,TValue}"/> is safe to use concurrently with reads and writes
        /// to the dictionary. It represents a snapshot consistent view of the dictionary.
        /// </remarks>
        /// <returns>An enumerable for the <see cref="IReliableDictionary3{TKey,TValue}"/> versioned keys.</returns>
        Task<IAsyncEnumerable<VersionedKey<TKey>>> CreateVersionedKeyEnumerableAsync(ITransaction txn, TKey firstKey, TKey lastKey);

        /// <summary>
        /// (Beta) Creates an asynchronous enumerator over the <see cref="IReliableDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="txn">The transaction to associate this operation with.</param>
        /// <exception cref="FabricNotReadableException">
        /// Indicates that the IReliableDictionary cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">
        /// <para>Indicates that the Reliable Dictionary is closed or deleted.</para>
        /// </exception>
        /// <remarks>
        /// <para>The returned enumerator is safe to use concurrently with reads and writes to the Reliable Dictionary.
        /// It represents a snapshot consistent view. Please note that <see cref="IAsyncEnumerable{T}.GetAsyncEnumerator"/> needs to be called on
        /// the returned IAsyncEnumerable in order to enumerate. Example usage can be
        /// seen <see href="https://github.com/Azure-Samples/service-fabric-dotnet-web-reference-app/blob/master/ReferenceApp/Inventory.Service/InventoryService.cs">here</see>.</para>
        /// </remarks>
        /// <returns>
        /// <para>A task that represents the asynchronous create enumerable operation. The task result is an enumerator for the Reliable Dictionary.</para>
        /// </returns>
        Task<IAsyncEnumerable<VersionedKeyValuePair<TKey, TValue>>> CreateVersionedEnumerableAsync(ITransaction txn);

        /// <summary>
        /// (Beta) Creates an asynchronous enumerator over the <see cref="IReliableDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="txn">The transaction to associate this operation with.</param>
        /// <param name="firstKey">The key to start enumerating from in ordered enumeration.</param>
        /// <exception cref="FabricNotReadableException">
        /// Indicates that the IReliableDictionary cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">
        /// <para>Indicates that the Reliable Dictionary is closed or deleted.</para>
        /// </exception>
        /// <remarks>
        /// <para>The returned enumerator is safe to use concurrently with reads and writes to the Reliable Dictionary.
        /// It represents a snapshot consistent view. Please note that <see cref="IAsyncEnumerable{T}.GetAsyncEnumerator"/> needs to be called on
        /// the returned IAsyncEnumerable in order to enumerate. Example usage can be
        /// seen <see href="https://github.com/Azure-Samples/service-fabric-dotnet-web-reference-app/blob/master/ReferenceApp/Inventory.Service/InventoryService.cs">here</see>.</para>
        /// </remarks>
        /// <returns>
        /// <para>A task that represents the asynchronous create enumerable operation. The task result is an enumerator for the Reliable Dictionary.</para>
        /// </returns>
        Task<IAsyncEnumerable<VersionedKeyValuePair<TKey, TValue>>> CreateVersionedEnumerableAsync(ITransaction txn, TKey firstKey);

        /// <summary>
        /// (Beta) Creates an asynchronous enumerator over the <see cref="IReliableDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="txn">The transaction to associate this operation with.</param>
        /// <param name="firstKey">The key to start enumerating from in ordered enumeration.</param>
        /// <param name="lastKey">The key to stop enumerating at in ordered enumeration.</param>
        /// <exception cref="FabricNotReadableException">
        /// Indicates that the IReliableDictionary cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">
        /// <para>Indicates that the Reliable Dictionary is closed or deleted.</para>
        /// </exception>
        /// <remarks>
        /// <para>The returned enumerator is safe to use concurrently with reads and writes to the Reliable Dictionary.
        /// It represents a snapshot consistent view. Please note that <see cref="IAsyncEnumerable{T}.GetAsyncEnumerator"/> needs to be called on
        /// the returned IAsyncEnumerable in order to enumerate. Example usage can be
        /// seen <see href="https://github.com/Azure-Samples/service-fabric-dotnet-web-reference-app/blob/master/ReferenceApp/Inventory.Service/InventoryService.cs">here</see>.</para>
        /// </remarks>
        /// <returns>
        /// <para>A task that represents the asynchronous create enumerable operation. The task result is an enumerator for the Reliable Dictionary.</para>
        /// </returns>
        Task<IAsyncEnumerable<VersionedKeyValuePair<TKey, TValue>>> CreateVersionedEnumerableAsync(ITransaction txn, TKey firstKey, TKey lastKey);

        /// <summary>
        /// (Beta) Creates an asynchronous enumerator over the <see cref="IReliableDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="txn">The transaction to associate this operation with.</param>
        /// <param name="filter">Predicate that filters the versioned key-value pairs to include in the enumeration based on the key.</param>
        /// <exception cref="FabricNotReadableException">
        /// Indicates that the IReliableDictionary cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">
        /// <para>Indicates that the Reliable Dictionary is closed or deleted.</para>
        /// </exception>
        /// <remarks>
        /// <para>The returned enumerator is safe to use concurrently with reads and writes to the Reliable Dictionary.
        /// It represents a snapshot consistent view. Please note that <see cref="IAsyncEnumerable{T}.GetAsyncEnumerator"/> needs to be called on
        /// the returned IAsyncEnumerable in order to enumerate. Example usage can be
        /// seen <see href="https://github.com/Azure-Samples/service-fabric-dotnet-web-reference-app/blob/master/ReferenceApp/Inventory.Service/InventoryService.cs">here</see>.</para>
        /// </remarks>
        /// <returns>
        /// <para>A task that represents the asynchronous create enumerable operation. The task result is an enumerator for the Reliable Dictionary.</para>
        /// </returns>
        Task<IAsyncEnumerable<VersionedKeyValuePair<TKey, TValue>>> CreateVersionedEnumerableAsync(ITransaction txn, Func<TKey, bool> filter);

        /// <summary>
        /// (Beta) Creates an asynchronous enumerator over the <see cref="IReliableDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="txn">The transaction to associate this operation with.</param>
        /// <param name="filter">Predicate that filters the versioned key-value pairs to include in the enumeration based on the key.</param>
        /// <param name="firstKey">The key to start enumerating from in ordered enumeration.</param>
        /// <exception cref="FabricNotReadableException">
        /// Indicates that the IReliableDictionary cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">
        /// <para>Indicates that the Reliable Dictionary is closed or deleted.</para>
        /// </exception>
        /// <remarks>
        /// <para>The returned enumerator is safe to use concurrently with reads and writes to the Reliable Dictionary.
        /// It represents a snapshot consistent view. Please note that <see cref="IAsyncEnumerable{T}.GetAsyncEnumerator"/> needs to be called on
        /// the returned IAsyncEnumerable in order to enumerate. Example usage can be
        /// seen <see href="https://github.com/Azure-Samples/service-fabric-dotnet-web-reference-app/blob/master/ReferenceApp/Inventory.Service/InventoryService.cs">here</see>.</para>
        /// </remarks>
        /// <returns>
        /// <para>A task that represents the asynchronous create enumerable operation. The task result is an enumerator for the Reliable Dictionary.</para>
        /// </returns>
        Task<IAsyncEnumerable<VersionedKeyValuePair<TKey, TValue>>> CreateVersionedEnumerableAsync(ITransaction txn, Func<TKey, bool> filter, TKey firstKey);

        /// <summary>
        /// (Beta) Creates an asynchronous enumerator over the <see cref="IReliableDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="txn">The transaction to associate this operation with.</param>
        /// <param name="filter">Predicate that filters the versioned key-value pairs to include in the enumeration based on the key.</param>
        /// <param name="firstKey">The key to start enumerating from in ordered enumeration.</param>
        /// <param name="lastKey">The key to stop enumerating at in ordered enumeration.</param>
        /// <exception cref="FabricNotReadableException">
        /// Indicates that the IReliableDictionary cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that Reliable Collection's state is not yet consistent.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a method call is invalid for the object's current state.
        /// Example, transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service code of the use of transactions.
        /// </exception>
        /// <exception cref="System.Fabric.FabricObjectClosedException">
        /// <para>Indicates that the Reliable Dictionary is closed or deleted.</para>
        /// </exception>
        /// <remarks>
        /// <para>The returned enumerator is safe to use concurrently with reads and writes to the Reliable Dictionary.
        /// It represents a snapshot consistent view. Please note that <see cref="IAsyncEnumerable{T}.GetAsyncEnumerator"/> needs to be called on
        /// the returned IAsyncEnumerable in order to enumerate. Example usage can be
        /// seen <see href="https://github.com/Azure-Samples/service-fabric-dotnet-web-reference-app/blob/master/ReferenceApp/Inventory.Service/InventoryService.cs">here</see>.</para>
        /// </remarks>
        /// <returns>
        /// <para>A task that represents the asynchronous create enumerable operation. The task result is an enumerator for the Reliable Dictionary.</para>
        /// </returns>
        Task<IAsyncEnumerable<VersionedKeyValuePair<TKey, TValue>>> CreateVersionedEnumerableAsync(ITransaction txn, Func<TKey, bool> filter, TKey firstKey, TKey lastKey);
    }
}
