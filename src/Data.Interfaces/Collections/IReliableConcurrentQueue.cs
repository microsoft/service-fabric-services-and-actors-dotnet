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
    /// Represents a reliable collection of persisted, replicated values with best-effort first-in first-out ordering.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// Intended as an alternative to <see cref="IReliableQueue{T}"/> for workloads where strict ordering is not required, as by relaxing
    /// the ordering constraint, concurrency can be greatly improved.  IReliableQueue&lt;T&gt; restricts concurrent consumers
    /// and producers to a maximum of one each, while this queue imposes no such restriction.
    /// </para>
    /// <para>
    /// This queue does not offer the same transaction isolation semantics as
    /// <see cref="IReliableDictionary{TKey,TValue}"/> and IReliableQueue&lt;T&gt;.
    /// See the individual operations and properties (<see cref="EnqueueAsync"/>, <see cref="TryDequeueAsync"/>
    /// and <see cref="Count"/>) for details on what isolation, if any, they provide.
    /// </para>
    /// <para>
    /// It is expected that values will be relatively short-lived in the queue; in other words, that the egress (TryDequeueAsync) rate is 
    /// equal to or greater than the ingress (EnqueueAsync) rate.  Violating this expectation 
    /// may worsen system performance.
    /// </para>
    /// <para>
    /// As the ordering of values is not strictly guaranteed, assumptions about the ordering of any two values in the queue MUST NOT
    /// be made.  The best-effort first-in first-out ordering is provided for fairness; the time that a value spends in the queue should
    /// be related to the failure rate (failures may alter the queue's ordering) and the dequeue rate, but not the enqueue rate.
    /// </para>
    /// <para>
    /// This queue does not offer a Peek operation; however, by combining TryDequeueAsync and <see cref="ITransaction.Abort"/>
    /// the same semantics can be achieved.  See TryDequeueAsync for additional details.
    /// </para>
    /// <para>
    /// Values stored in this queue MUST NOT be mutated outside the context of an operation on the queue. It is
    /// highly recommended to make <typeparamref name="T"/> immutable in order to avoid accidental data corruption.
    /// </para>
    /// <para>
    /// An <see cref="ITransaction"/> is the unit of concurrency: Users can have multiple transactions in-flight at any time, but for a given transaction each API must be called one at a time.
    /// APIs on this and other reliable collection types that accept a transaction and return a <see cref="Task"/> must be awaited one at a time.
    /// </para>
    /// </remarks>
    public interface IReliableConcurrentQueue<T> : IReliableState
    {
        /// <summary>
        /// Asynchronously stages the enqueue of a value into the queue.
        /// </summary>
        /// 
        /// <param name="tx">The transaction in which to enqueue the value.</param>
        /// <param name="value">The value to add to the queue. Can be <see langword="null"/> for reference types.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. The default is <see langword="null"/>. If <see langword="null"/> is passed, a default timeout will be used.</param>
        /// 
        /// <remarks>
        /// A <see cref="TryDequeueAsync"/> operation cannot return any value whose enqueue has not yet been committed.
        /// This includes the transaction in which the value was enqueued; as a consequence, the queue does not support Read-Your-Writes.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException"><paramref name="tx"/> is not a valid transaction, or <paramref name="timeout"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricException">The replica saw a non-retriable failure other than <see cref="FabricNotPrimaryException"/>, <see cref="FabricObjectClosedException"/>, or <see cref="FabricTransientException"/>.</exception>
        /// <exception cref="FabricNotPrimaryException">The replica is no longer in <see cref="ReplicaRole.Primary"/>.</exception>
        /// <exception cref="FabricObjectClosedException">The queue was closed by the runtime.</exception>
        /// <exception cref="FabricTransientException">The replica saw a transient failure. Retry the operation on a new transaction.</exception>
        /// <exception cref="InvalidOperationException">
        /// A method call is invalid for the object's current state.
        /// For example, the transaction used was already terminated: committed or aborted by the user.
        /// This exception typically indicates a bug in the service's use of transactions.
        /// </exception>
        /// <exception cref="OperationCanceledException">The operation was canceled via <paramref name="cancellationToken"/>.</exception>
        /// <exception cref="QueueFullException">The queue has reached its capacity. Retry the operation after dequeue operations free up space.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        /// 
        /// <example>
        /// This example shows how to enqueue a value with retry.
        /// <code language="csharp">
        /// <![CDATA[
        /// protected override async Task RunAsync(CancellationToken cancellationToken)
        /// {
        ///     var concurrentQueue = await this.StateManager.GetOrAddAsync<IReliableConcurrentQueue<long>>(new Uri("fabric:/concurrentQueue"));
        ///
        ///     while (true)
        ///     {
        ///         cancellationToken.ThrowIfCancellationRequested();
        /// 
        ///         try
        ///         {
        ///             using (var tx = this.StateManager.CreateTransaction())
        ///             {
        ///                 await concurrentQueue.EnqueueAsync(tx, 12L, cancellationToken);
        ///                 await tx.CommitAsync();
        ///
        ///                 return;
        ///             }
        ///         }
        ///         catch (TransactionFaultedException e)
        ///         {
        ///             // This indicates that the transaction was internally faulted by the system. One possible
        ///             // cause for this is that the transaction was long running and blocked a checkpoint.
        ///             // Increasing the "ReliableStateManagerReplicatorSettings.CheckpointThresholdInMB" will
        ///             // help reduce the chances of running into this exception.
        ///             Console.WriteLine("Transaction was internally faulted, retrying the transaction: " + e);
        ///         }
        ///         catch (FabricNotPrimaryException e)
        ///         {
        ///             // Gracefully exit RunAsync as the new primary should have RunAsync invoked on it and continue work.
        ///             // If instead enqueue was being executed as part of a client request, the client would be signaled to re-resolve.
        ///             Console.WriteLine("Replica is not primary, exiting RunAsync: " + e);
        ///             return;
        ///         }
        ///         catch (FabricObjectClosedException e)
        ///         {
        ///             // Gracefully exit RunAsync as this is happening due to replica close.
        ///             // If instead enqueue was being executed as part of a client request, the client would be signaled to re-resolve.
        ///             Console.WriteLine("Replica is closing, exiting RunAsync: " + e);
        ///             return;
        ///         }
        ///         catch (TimeoutException e)
        ///         {
        ///             Console.WriteLine("Encountered TimeoutException during EnqueueAsync, retrying the transaction: " + e);
        ///         }
        ///         catch (FabricTransientException e)
        ///         {
        ///             // Retry until the queue is writable or a different exception is thrown.
        ///             Console.WriteLine("Queue is currently not writable, retrying the transaction: " + e);
        ///         }
        ///
        ///         // Delay and retry.
        ///         await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        Task EnqueueAsync(ITransaction tx, T value, CancellationToken cancellationToken = default(CancellationToken), TimeSpan? timeout = null);

        /// <summary>
        /// Asynchronously returns the value dequeued from the head of the queue, or an empty result if the queue is empty.
        /// </summary>
        /// 
        /// <param name="tx">The transaction in which to dequeue the value.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. The default is <see langword="null"/>. If <see langword="null"/> is passed, a default timeout will be used.</param>
        /// 
        /// <remarks>
        /// <para>
        /// If the queue is empty, the dequeue operation returns an empty result immediately rather than waiting for a value to become available.
        /// </para>
        /// <para>
        /// While a dequeue can only return values for which the corresponding <see cref="EnqueueAsync"/> was committed, dequeue operations are not isolated
        /// from one another.  Once a transaction has dequeued a value, other transactions cannot dequeue it, but are not blocked from dequeuing other values.
        /// </para>
        /// <para>
        /// When a transaction or transactions including one or more dequeue operations abort, the dequeued values will be added back at
        /// the head of the queue in an arbitrary order.  This will ensure that these values will be dequeued again soon, improving the fairness of the
        /// data structure, but without enforcing strict ordering (which would require reducing the allowed concurrency, as in <see cref="IReliableQueue{T}"/>).
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException"><paramref name="tx"/> is not a valid transaction, or <paramref name="timeout"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricException">The replica saw a non-retriable failure other than <see cref="FabricNotPrimaryException"/>, <see cref="FabricNotReadableException"/>, <see cref="FabricObjectClosedException"/>, or <see cref="FabricTransientException"/>.</exception>
        /// <exception cref="FabricNotPrimaryException">The replica is no longer in <see cref="ReplicaRole.Primary"/>.</exception>
        /// <exception cref="FabricNotReadableException">The replica is currently not readable.</exception>
        /// <exception cref="FabricObjectClosedException">The queue was closed by the runtime.</exception>
        /// <exception cref="FabricTransientException">The replica saw a transient failure. Retry the operation on a new transaction.</exception>
        /// <exception cref="InvalidOperationException">
        /// A method call is invalid for the object's current state.
        /// For example, the transaction used was already terminated: committed or aborted by the user.
        /// This exception typically indicates a bug in the service's use of transactions.
        /// </exception>
        /// <exception cref="OperationCanceledException">The operation was canceled via <paramref name="cancellationToken"/>.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        /// <returns>
        /// The value dequeued from the head of the queue via <see cref="ConditionalValue{T}.Value"/> with
        /// <see cref="ConditionalValue{T}.HasValue"/> set to <see langword="true"/> when the queue was not empty;
        /// otherwise, HasValue is <see langword="false"/>.
        /// </returns>
        /// <example>
        /// This example shows how to dequeue and log continuously with retry, until the cancellation token is canceled.  
        /// <code language="csharp">
        /// <![CDATA[
        /// protected override async Task RunAsync(CancellationToken cancellationToken)
        /// {
        ///     var concurrentQueue = await this.StateManager.GetOrAddAsync<IReliableConcurrentQueue<long>>(new Uri("fabric:/concurrentQueue"));
        /// 
        ///     // Assumption: values are being enqueued by another source (e.g. the communication listener).
        ///     while (true)
        ///     {
        ///         cancellationToken.ThrowIfCancellationRequested();
        /// 
        ///         try
        ///         {
        ///             using (var tx = this.StateManager.CreateTransaction())
        ///             {
        ///                 var dequeueOutput = await concurrentQueue.TryDequeueAsync(tx, cancellationToken, TimeSpan.FromMilliseconds(100));
        ///                 await tx.CommitAsync();
        /// 
        ///                 if (dequeueOutput.HasValue)
        ///                 {
        ///                     Console.WriteLine("Dequeue # " + dequeueOutput.Value);
        ///                 }
        ///                 else
        ///                 {
        ///                     Console.WriteLine("Queue was empty");
        ///                 }
        ///             }
        ///         }
        ///         catch (TransactionFaultedException e)
        ///         {
        ///             // This indicates that the transaction was internally faulted by the system. One possible
        ///             // cause for this is that the transaction was long running and blocked a checkpoint.
        ///             // Increasing the "ReliableStateManagerReplicatorSettings.CheckpointThresholdInMB" will
        ///             // help reduce the chances of running into this exception.
        ///             Console.WriteLine("Transaction was internally faulted, retrying the transaction: " + e);
        ///         }
        ///         catch (FabricNotPrimaryException e)
        ///         {
        ///             // Gracefully exit RunAsync as the new primary should have RunAsync invoked on it and continue work.
        ///             // If instead dequeue was being executed as part of a client request, the client would be signaled to re-resolve.
        ///             Console.WriteLine("Replica is not primary, exiting RunAsync: " + e);
        ///             return;
        ///         }
        ///         catch (FabricNotReadableException e)
        ///         {
        ///             // Retry until the queue is readable or a different exception is thrown.
        ///             Console.WriteLine("Queue is not readable, retrying the transaction: " + e);
        ///         }
        ///         catch (FabricObjectClosedException e)
        ///         {
        ///             // Gracefully exit RunAsync as this is happening due to replica close.
        ///             // If instead dequeue was being executed as part of a client request, the client would be signaled to re-resolve.
        ///             Console.WriteLine("Replica is closing, exiting RunAsync: " + e);
        ///             return;
        ///         }
        ///         catch (TimeoutException e)
        ///         {
        ///             Console.WriteLine("Encountered TimeoutException during TryDequeueAsync, retrying the transaction: " + e);
        ///         }
        ///         catch (FabricTransientException e)
        ///         {
        ///             // Retry until the transient failure is resolved or a different exception is thrown.
        ///             Console.WriteLine("Encountered a transient error, retrying the transaction: " + e);
        ///         }
        /// 
        ///         // Delay and retry.
        ///         await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        Task<ConditionalValue<T>> TryDequeueAsync(ITransaction tx, CancellationToken cancellationToken = default(CancellationToken), TimeSpan? timeout = null);

        /// <summary>
        /// Gets the number of values in the queue.
        /// </summary>
        ///  
        /// <remarks>
        /// <para>
        /// This count represents the number of values currently visible to <see cref="TryDequeueAsync"/>.  Uncommitted enqueues will not
        /// increase the count; however, uncommitted dequeues will decrease the count.
        /// </para>
        /// <para>
        /// Since the effects of TryDequeueAsync are not isolated from other transactions, the count also
        /// cannot be isolated from other transactions.
        /// </para>
        /// </remarks>
        /// <exception cref="FabricNotReadableException">The replica is currently not readable.</exception>
        /// <exception cref="FabricObjectClosedException">The queue was closed by the runtime.</exception>
        /// <exception cref="InvalidOperationException">The object's current state is invalid for this operation.</exception>
        ///  
        /// <example>
        /// This example shows how to monitor the queue's count continuously, until the cancellation token is canceled.
        /// <code language="csharp">
        /// <![CDATA[
        /// protected override async Task RunAsync(CancellationToken cancellationToken)
        /// {
        ///     var concurrentQueue = await this.StateManager.GetOrAddAsync<IReliableConcurrentQueue<long>>(new Uri("fabric:/concurrentQueue"));
        ///
        ///     // Assumption: values are being enqueued/dequeued in another place (e.g. the communication listener).
        ///     while (true)
        ///     {
        ///         cancellationToken.ThrowIfCancellationRequested();
        ///
        ///         try
        ///         {
        ///             Console.WriteLine("Count: " + concurrentQueue.Count);
        ///         }
        ///         catch (FabricNotReadableException e)
        ///         {
        ///             // Retry until the queue is readable or a different exception is thrown.
        ///             Console.WriteLine("Queue is not readable, retrying the observation: " + e);
        ///         }
        ///         catch (FabricObjectClosedException e)
        ///         {
        ///             // Gracefully exit RunAsync as this is happening due to replica close.
        ///             Console.WriteLine("Replica is closing, exiting RunAsync: " + e);
        ///             return;
        ///         }
        ///
        ///         await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        long Count { get; }
    }
}
