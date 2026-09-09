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
    /// Represents a Reliable Collection of key/value pairs that are persisted and replicated, adding sequence-number reads,
    /// versioned key/value reads, sequence-number-checked updates and removals, and versioned-key and versioned key/value
    /// enumeration.
    /// </summary>
    /// <inheritdoc path="/remarks" cref="IReliableDictionary2{TKey, TValue}"/>
    public interface IReliableDictionary3<TKey, TValue> : IReliableDictionary2<TKey, TValue>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        /// <inheritdoc path="/summary" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='key']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        Task<ConditionalValue<long>> TryGetSequenceNumberAsync(
            ITransaction tx,
            TKey key);

        /// <inheritdoc path="/summary" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='key']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='lockMode']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <exception cref="ArgumentException"><paramref name="lockMode"/> is not a valid <see cref="LockMode"/> value.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="TryGetSequenceNumberAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        Task<ConditionalValue<long>> TryGetSequenceNumberAsync(
            ITransaction tx,
            TKey key,
            LockMode lockMode);

        /// <summary>
        /// (Beta) Asynchronously attempts to get the sequence number associated with the specified key from the <see cref="IReliableDictionary3{TKey, TValue}"/> and returns a result indicating whether the key was found and, if so, its sequence number.
        /// </summary>
        /// <param name="tx">The transaction to associate this operation with.</param>
        /// <param name="key">The key of the element whose sequence number is to be retrieved.</param>
        /// <param name="lockMode">One of the enumeration values that specifies the type of locking to use for this read operation.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. Primarily used to prevent deadlocks.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="ArgumentException"><paramref name="timeout"/> is negative, or <paramref name="lockMode"/> is not a valid <see cref="LockMode"/> value.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is <see langword="null"/>, or <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotReadableException">
        /// The <see cref="IReliableDictionary3{TKey, TValue}"/> cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that the state of the <see cref="IReliableDictionary3{TKey, TValue}"/> is not yet consistent.
        /// </exception>
        /// <exception cref="FabricObjectClosedException">The <see cref="IReliableDictionary3{TKey, TValue}"/> is closed or deleted.</exception>
        /// <exception cref="InvalidOperationException">A method call is invalid for the object's current state, for example, the transaction is already committed or aborted.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled via <paramref name="cancellationToken"/>.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        Task<ConditionalValue<long>> TryGetSequenceNumberAsync(
            ITransaction tx,
            TKey key,
            LockMode lockMode,
            TimeSpan timeout,
            CancellationToken cancellationToken);


        /// <inheritdoc path="/summary" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='key']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        Task<ConditionalValue<VersionedKeyValuePair<TKey, TValue>>> TryGetVersionedKeyValuePairAsync(
            ITransaction tx,
            TKey key);

        /// <inheritdoc path="/summary" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='key']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='lockMode']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <exception cref="ArgumentException"><paramref name="lockMode"/> is not a valid <see cref="LockMode"/> value.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="TryGetVersionedKeyValuePairAsync(ITransaction, TKey, LockMode, TimeSpan, CancellationToken)"/>
        Task<ConditionalValue<VersionedKeyValuePair<TKey, TValue>>> TryGetVersionedKeyValuePairAsync(
            ITransaction tx,
            TKey key,
            LockMode lockMode);

        /// <summary>
        /// (Beta) Asynchronously attempts to get the versioned element associated with the specified key from the <see cref="IReliableDictionary3{TKey, TValue}"/> and returns a result indicating whether the key was found and, if so, its value and sequence number.
        /// </summary>
        /// <param name="tx">The transaction to associate this operation with.</param>
        /// <param name="key">The key of the versioned element to get.</param>
        /// <param name="lockMode">One of the enumeration values that specifies the type of locking to use for this read operation.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. Primarily used to prevent deadlocks.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="ArgumentException"><paramref name="timeout"/> is negative, or <paramref name="lockMode"/> is not a valid <see cref="LockMode"/> value.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is <see langword="null"/>, or <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotReadableException">
        /// The <see cref="IReliableDictionary3{TKey, TValue}"/> cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that the state of the <see cref="IReliableDictionary3{TKey, TValue}"/> is not yet consistent.
        /// </exception>
        /// <exception cref="FabricObjectClosedException">The <see cref="IReliableDictionary3{TKey, TValue}"/> is closed or deleted.</exception>
        /// <exception cref="InvalidOperationException">A method call is invalid for the object's current state, for example, the transaction is already committed or aborted.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled via <paramref name="cancellationToken"/>.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        Task<ConditionalValue<VersionedKeyValuePair<TKey, TValue>>> TryGetVersionedKeyValuePairAsync(
            ITransaction tx,
            TKey key,
            LockMode lockMode,
            TimeSpan timeout,
            CancellationToken cancellationToken);

        /// <inheritdoc path="/summary" cref="TryUpdateAsync(ITransaction, TKey, TValue, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="TryUpdateAsync(ITransaction, TKey, TValue, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='key']" cref="TryUpdateAsync(ITransaction, TKey, TValue, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='newValue']" cref="TryUpdateAsync(ITransaction, TKey, TValue, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='checkSequenceNumber']" cref="TryUpdateAsync(ITransaction, TKey, TValue, long, TimeSpan, CancellationToken)"/>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="TryUpdateAsync(ITransaction, TKey, TValue, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="TryUpdateAsync(ITransaction, TKey, TValue, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="TryUpdateAsync(ITransaction, TKey, TValue, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="TryUpdateAsync(ITransaction, TKey, TValue, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="TryUpdateAsync(ITransaction, TKey, TValue, long, TimeSpan, CancellationToken)"/>
        Task<bool> TryUpdateAsync(ITransaction tx, TKey key, TValue newValue, long checkSequenceNumber);

        /// <summary>
        /// (Beta) Asynchronously attempts to update the value for the specified key if its current sequence number matches <paramref name="checkSequenceNumber"/> and returns <see langword="true"/> if the value was updated; otherwise returns <see langword="false"/>.
        /// </summary>
        /// <param name="tx">The transaction to associate this operation with.</param>
        /// <param name="key">The key of the element to be updated.</param>
        /// <param name="newValue">The value to be updated to if the specified <paramref name="key"/> has the expected <paramref name="checkSequenceNumber"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <param name="checkSequenceNumber">The expected sequence number of the element to be updated.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. Primarily used to prevent deadlocks.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="ArgumentException"><paramref name="timeout"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is <see langword="null"/>, or <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotPrimaryException">The <see cref="IReliableDictionary3{TKey, TValue}"/> is not in <see cref="ReplicaRole.Primary"/>.</exception>
        /// <exception cref="FabricObjectClosedException">The <see cref="IReliableDictionary3{TKey, TValue}"/> is closed or deleted.</exception>
        /// <exception cref="InvalidOperationException">A method call is invalid for the object's current state, for example, the transaction is already committed or aborted.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled via <paramref name="cancellationToken"/>.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        Task<bool> TryUpdateAsync(ITransaction tx, TKey key, TValue newValue, long checkSequenceNumber, TimeSpan timeout, CancellationToken cancellationToken);

        /// <inheritdoc path="/summary" cref="TryRemoveAsync(ITransaction, TKey, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="TryRemoveAsync(ITransaction, TKey, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='key']" cref="TryRemoveAsync(ITransaction, TKey, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='checkSequenceNumber']" cref="TryRemoveAsync(ITransaction, TKey, long, TimeSpan, CancellationToken)"/>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="TryRemoveAsync(ITransaction, TKey, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="TryRemoveAsync(ITransaction, TKey, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="TryRemoveAsync(ITransaction, TKey, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="TryRemoveAsync(ITransaction, TKey, long, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="TryRemoveAsync(ITransaction, TKey, long, TimeSpan, CancellationToken)"/>
        Task<bool> TryRemoveAsync(ITransaction tx, TKey key, long checkSequenceNumber);

        /// <summary>
        /// (Beta) Asynchronously attempts to remove the value with the specified key if its current sequence number matches <paramref name="checkSequenceNumber"/> and returns <see langword="true"/> if the element was removed; otherwise returns <see langword="false"/>.
        /// </summary>
        /// <param name="tx">The transaction to associate this operation with.</param>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="checkSequenceNumber">The expected sequence number of the element to be removed.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. Primarily used to prevent deadlocks.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="ArgumentException"><paramref name="timeout"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is <see langword="null"/>, or <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotPrimaryException">The <see cref="IReliableDictionary3{TKey, TValue}"/> is not in <see cref="ReplicaRole.Primary"/>.</exception>
        /// <exception cref="FabricObjectClosedException">The <see cref="IReliableDictionary3{TKey, TValue}"/> is closed or deleted.</exception>
        /// <exception cref="InvalidOperationException">A method call is invalid for the object's current state, for example, the transaction is already committed or aborted.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled via <paramref name="cancellationToken"/>.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        Task<bool> TryRemoveAsync(ITransaction tx, TKey key, long checkSequenceNumber, TimeSpan timeout, CancellationToken cancellationToken);

        /// <inheritdoc path="/summary" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='txn']" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        /// <exception cref="ArgumentNullException"><paramref name="txn"/> is <see langword="null"/>.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        /// <inheritdoc path="/remarks" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        Task<IAsyncEnumerable<VersionedKey<TKey>>> CreateVersionedKeyEnumerableAsync(ITransaction txn);

        /// <inheritdoc path="/summary" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='txn']" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='firstKey']" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        /// <exception cref="ArgumentNullException"><paramref name="txn"/> or <paramref name="firstKey"/> is <see langword="null"/>.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        /// <inheritdoc path="/remarks" cref="CreateVersionedKeyEnumerableAsync(ITransaction, TKey, TKey)"/>
        Task<IAsyncEnumerable<VersionedKey<TKey>>> CreateVersionedKeyEnumerableAsync(ITransaction txn, TKey firstKey);

        /// <summary>
        /// (Beta) Asynchronously returns an enumerable over the versioned keys in the <see cref="IReliableDictionary3{TKey, TValue}"/>.
        /// </summary>
        /// <param name="txn">The transaction to associate this operation with.</param>
        /// <param name="firstKey">The inclusive key to start enumerating from in ordered enumeration.</param>
        /// <param name="lastKey">The inclusive key to stop enumerating at in ordered enumeration.</param>
        /// <exception cref="ArgumentException"><paramref name="firstKey"/> is greater than <paramref name="lastKey"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="txn"/>, <paramref name="firstKey"/>, or <paramref name="lastKey"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotReadableException">
        /// The <see cref="IReliableDictionary3{TKey, TValue}"/> cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that the state of the <see cref="IReliableDictionary3{TKey, TValue}"/> is not yet consistent.
        /// </exception>
        /// <exception cref="FabricObjectClosedException">The <see cref="IReliableDictionary3{TKey, TValue}"/> is closed or deleted.</exception>
        /// <exception cref="InvalidOperationException">A method call is invalid for the object's current state, for example, the transaction is already committed or aborted.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        /// <remarks>The returned enumerable is safe to use concurrently with reads and writes to the Reliable Dictionary.
        /// It represents a snapshot consistent view.</remarks>
        Task<IAsyncEnumerable<VersionedKey<TKey>>> CreateVersionedKeyEnumerableAsync(ITransaction txn, TKey firstKey, TKey lastKey);

        /// <inheritdoc path="/summary" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='txn']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <exception cref="ArgumentNullException"><paramref name="txn"/> is <see langword="null"/>.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/remarks" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        Task<IAsyncEnumerable<VersionedKeyValuePair<TKey, TValue>>> CreateVersionedEnumerableAsync(ITransaction txn);

        /// <inheritdoc path="/summary" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='txn']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='firstKey']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <exception cref="ArgumentNullException"><paramref name="txn"/> or <paramref name="firstKey"/> is <see langword="null"/>.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/remarks" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        Task<IAsyncEnumerable<VersionedKeyValuePair<TKey, TValue>>> CreateVersionedEnumerableAsync(ITransaction txn, TKey firstKey);

        /// <inheritdoc path="/summary" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='txn']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='firstKey']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='lastKey']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/remarks" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        Task<IAsyncEnumerable<VersionedKeyValuePair<TKey, TValue>>> CreateVersionedEnumerableAsync(ITransaction txn, TKey firstKey, TKey lastKey);

        /// <inheritdoc path="/summary" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='txn']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='filter']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <exception cref="ArgumentNullException"><paramref name="txn"/> is <see langword="null"/>.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/remarks" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        Task<IAsyncEnumerable<VersionedKeyValuePair<TKey, TValue>>> CreateVersionedEnumerableAsync(ITransaction txn, Func<TKey, bool> filter);

        /// <inheritdoc path="/summary" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='txn']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='filter']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/param[@name='firstKey']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <exception cref="ArgumentNullException"><paramref name="txn"/> or <paramref name="firstKey"/> is <see langword="null"/>.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        /// <inheritdoc path="/remarks" cref="CreateVersionedEnumerableAsync(ITransaction, Func{TKey, bool}, TKey, TKey)"/>
        Task<IAsyncEnumerable<VersionedKeyValuePair<TKey, TValue>>> CreateVersionedEnumerableAsync(ITransaction txn, Func<TKey, bool> filter, TKey firstKey);

        /// <summary>
        /// (Beta) Asynchronously returns an enumerable over the versioned key/value pairs in the <see cref="IReliableDictionary3{TKey, TValue}"/>.
        /// </summary>
        /// <param name="txn">The transaction to associate this operation with.</param>
        /// <param name="filter">The predicate that filters the versioned key/value pairs to include in the enumeration based on the key, or <see langword="null"/> to include all versioned key/value pairs.</param>
        /// <param name="firstKey">The inclusive key to start enumerating from in ordered enumeration.</param>
        /// <param name="lastKey">The inclusive key to stop enumerating at in ordered enumeration.</param>
        /// <exception cref="ArgumentException"><paramref name="firstKey"/> is greater than <paramref name="lastKey"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="txn"/>, <paramref name="firstKey"/>, or <paramref name="lastKey"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotReadableException">
        /// The <see cref="IReliableDictionary3{TKey, TValue}"/> cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that the state of the <see cref="IReliableDictionary3{TKey, TValue}"/> is not yet consistent.
        /// </exception>
        /// <exception cref="FabricObjectClosedException">The <see cref="IReliableDictionary3{TKey, TValue}"/> is closed or deleted.</exception>
        /// <exception cref="InvalidOperationException">A method call is invalid for the object's current state, for example, the transaction is already committed or aborted.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        /// <remarks>The returned enumerable is safe to use concurrently with reads and writes to the Reliable Dictionary.
        /// It represents a snapshot consistent view. Example usage can be
        /// seen <see href="https://github.com/Azure-Samples/service-fabric-dotnet-web-reference-app/blob/master/ReferenceApp/Inventory.Service/InventoryService.cs">here</see>.</remarks>
        Task<IAsyncEnumerable<VersionedKeyValuePair<TKey, TValue>>> CreateVersionedEnumerableAsync(ITransaction txn, Func<TKey, bool> filter, TKey firstKey, TKey lastKey);
    }
}
