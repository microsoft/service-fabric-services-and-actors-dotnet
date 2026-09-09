// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data.Collections
{
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a reliable first-in, first-out collection of persisted and replicated elements of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Values stored in this queue MUST NOT be mutated outside the context of an operation on the queue. It is
    /// highly recommended to make <typeparamref name="T"/> immutable to avoid accidental data corruption.
    /// </para>
    /// <para>
    /// An <see cref="ITransaction"/> is the unit of concurrency. Multiple transactions can be in-flight at any time, but operations within
    /// a given transaction must be called sequentially.
    /// <see cref="IReliableCollection{T}"/> APIs that take a transaction and return a <see cref="Task"/> must be awaited
    /// one at a time.
    /// </para>
    /// <para>
    /// If a retriable exception is thrown by an operation on this queue, dispose the transaction and retry
    /// with a new transaction.
    /// </para>
    /// </remarks>
    /// <seealso cref="ITransaction"/>
    public interface IReliableQueue<T> : IReliableCollection<T>
    {
        /// <inheritdoc path="/summary" cref="EnqueueAsync(ITransaction, T, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="EnqueueAsync(ITransaction, T, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='item']" cref="EnqueueAsync(ITransaction, T, TimeSpan, CancellationToken)"/>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="EnqueueAsync(ITransaction, T, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="EnqueueAsync(ITransaction, T, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="EnqueueAsync(ITransaction, T, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="EnqueueAsync(ITransaction, T, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="EnqueueAsync(ITransaction, T, TimeSpan, CancellationToken)"/>
        Task EnqueueAsync(ITransaction tx, T item);

        /// <summary>
        /// Asynchronously adds a value to the end of the queue.
        /// </summary>
        /// <param name="tx">The <see cref="ITransaction"/> to associate this operation with.</param>
        /// <param name="item">The value to add. Can be <see langword="null"/> for reference types.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. Primarily used to prevent deadlocks.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="ArgumentException"><paramref name="timeout"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotPrimaryException">The <see cref="IReliableQueue{T}"/> is not in the <see cref="ReplicaRole.Primary"/> role.</exception>
        /// <exception cref="FabricObjectClosedException">The <see cref="IReliableQueue{T}"/> is closed or deleted.</exception>
        /// <exception cref="InvalidOperationException">
        /// A method call is invalid for the object's current state.
        /// For example, the transaction used was already terminated: committed or aborted by the user.
        /// This exception typically indicates a bug in the service's use of transactions.
        /// </exception>
        /// <exception cref="OperationCanceledException">The operation was canceled via <paramref name="cancellationToken"/>.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        Task EnqueueAsync(ITransaction tx, T item, TimeSpan timeout, CancellationToken cancellationToken);

        /// <inheritdoc path="/summary" cref="TryDequeueAsync(ITransaction, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="TryDequeueAsync(ITransaction, TimeSpan, CancellationToken)"/>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="TryDequeueAsync(ITransaction, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="TryDequeueAsync(ITransaction, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="TryDequeueAsync(ITransaction, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="TryDequeueAsync(ITransaction, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="TryDequeueAsync(ITransaction, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/returns" cref="TryDequeueAsync(ITransaction, TimeSpan, CancellationToken)"/>
        Task<ConditionalValue<T>> TryDequeueAsync(ITransaction tx);

        /// <summary>
        /// Asynchronously returns the value removed from the beginning of the queue, or an empty result if the queue is empty.
        /// </summary>
        /// <param name="tx">The <see cref="ITransaction"/> to associate this operation with.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. Primarily used to prevent deadlocks.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="ArgumentException"><paramref name="timeout"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotPrimaryException">The <see cref="IReliableQueue{T}"/> is not in the <see cref="ReplicaRole.Primary"/> role.</exception>
        /// <exception cref="FabricObjectClosedException">The <see cref="IReliableQueue{T}"/> is closed or deleted.</exception>
        /// <exception cref="InvalidOperationException">
        /// A method call is invalid for the object's current state.
        /// For example, the transaction used was already terminated: committed or aborted by the user.
        /// This exception typically indicates a bug in the service's use of transactions.
        /// </exception>
        /// <exception cref="OperationCanceledException">The operation was canceled via <paramref name="cancellationToken"/>.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        /// <returns>
        /// The value removed from the beginning of the queue via <see cref="ConditionalValue{T}.Value"/> with
        /// <see cref="ConditionalValue{T}.HasValue"/> set to <see langword="true"/> when the queue was not empty;
        /// otherwise, <see cref="ConditionalValue{T}.HasValue"/> is <see langword="false"/>.
        /// </returns>
        Task<ConditionalValue<T>> TryDequeueAsync(ITransaction tx, TimeSpan timeout,
            CancellationToken cancellationToken);

        /// <inheritdoc path="/summary" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/returns" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        Task<ConditionalValue<T>> TryPeekAsync(ITransaction tx);

        /// <inheritdoc path="/summary" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='timeout']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='cancellationToken']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/returns" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        Task<ConditionalValue<T>> TryPeekAsync(ITransaction tx, TimeSpan timeout,
            CancellationToken cancellationToken);

        /// <inheritdoc path="/summary" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='lockMode']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/returns" cref="TryPeekAsync(ITransaction, LockMode, TimeSpan, CancellationToken)"/>
        Task<ConditionalValue<T>> TryPeekAsync(ITransaction tx, LockMode lockMode);

        /// <summary>
        /// Asynchronously returns the value at the beginning of the queue without removing it, or an empty result if the queue is empty.
        /// </summary>
        /// <param name="tx">The <see cref="ITransaction"/> to associate this operation with.</param>
        /// <param name="lockMode">One of the enumeration values that specifies the type of locking to use for this read operation.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. Primarily used to prevent deadlocks.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="ArgumentException"><paramref name="timeout"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotReadableException">
        /// The <see cref="IReliableQueue{T}"/> cannot serve reads.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that the state of the <see cref="IReliableQueue{T}"/> is not yet consistent.
        /// </exception>
        /// <exception cref="FabricObjectClosedException">The <see cref="IReliableQueue{T}"/> is closed or deleted.</exception>
        /// <exception cref="InvalidOperationException">
        /// A method call is invalid for the object's current state.
        /// For example, the transaction used was already terminated: committed or aborted by the user.
        /// This exception typically indicates a bug in the service's use of transactions.
        /// </exception>
        /// <exception cref="OperationCanceledException">The operation was canceled via <paramref name="cancellationToken"/>.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        /// <returns>
        /// The value at the beginning of the queue via <see cref="ConditionalValue{T}.Value"/> with
        /// <see cref="ConditionalValue{T}.HasValue"/> set to <see langword="true"/> when the queue was not empty;
        /// otherwise, <see cref="ConditionalValue{T}.HasValue"/> is <see langword="false"/>.
        /// </returns>
        Task<ConditionalValue<T>> TryPeekAsync(ITransaction tx, LockMode lockMode, TimeSpan timeout,
            CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously returns an <see cref="IAsyncEnumerable{T}"/> over the <see cref="IReliableQueue{T}"/>.
        /// </summary>
        /// <param name="tx">The <see cref="ITransaction"/> to associate this operation with.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotReadableException">
        /// The <see cref="IReliableQueue{T}"/> cannot serve reads.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that the state of the <see cref="IReliableQueue{T}"/> is not yet consistent.
        /// </exception>
        /// <exception cref="FabricObjectClosedException">The <see cref="IReliableQueue{T}"/> is closed or deleted.</exception>
        /// <exception cref="InvalidOperationException">
        /// A method call is invalid for the object's current state.
        /// For example, the transaction used was already terminated: committed or aborted by the user.
        /// This exception typically indicates a bug in the service's use of transactions.
        /// </exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        /// <remarks>
        /// The returned enumerable provides a snapshot-consistent view of the <see cref="IReliableQueue{T}"/>, traversing
        /// its values in first-in, first-out order. <see cref="IAsyncEnumerable{T}.GetAsyncEnumerator"/> must be called on
        /// the returned instance to enumerate.
        /// </remarks>
        Task<IAsyncEnumerable<T>> CreateEnumerableAsync(ITransaction tx);
    }
}

