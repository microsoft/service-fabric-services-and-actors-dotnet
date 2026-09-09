// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System;
    using System.Fabric;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Data.Collections;

    /// <summary>
    /// Represents a sequence of operations performed as a single logical unit of work.
    /// </summary>
    /// <remarks>
    /// A transaction must exhibit the following <see href="https://learn.microsoft.com/windows/win32/cossdk/acid-properties">ACID properties</see>:
    /// <list type="bullet">
    ///     <item>
    ///         <term>Atomicity</term>
    ///         <description>A transaction must be an atomic unit of work; either all of its data modifications are performed,
    ///         or none of them is performed.</description>
    ///     </item>
    ///     <item>
    ///         <term>Consistency</term>
    ///         <description>When completed, a transaction must leave all data in a consistent state. All internal data structures
    ///         must be correct at the end of the transaction.</description>
    ///     </item>
    ///     <item>
    ///         <term>Isolation</term>
    ///         <description>Modifications made by concurrent transactions must be isolated from the modifications made by
    ///         any other concurrent transactions. The isolation level used for an operation is determined by the <see cref="IReliableState"/>
    ///         performing the operation.</description>
    ///     </item>
    ///     <item>
    ///         <term>Durability</term>
    ///         <description>After a transaction has completed, its effects are permanently in place in the system. The modifications
    ///         persist even in the event of a system failure.</description>
    ///     </item>
    /// </list>
    /// <para>
    /// Instance members of this type are not guaranteed to be thread-safe.
    /// This makes transactions the unit of concurrency: Users can have multiple transactions in-flight at any time, but
    /// for a given transaction each API must be called one at a time.
    /// </para>
    /// <para>
    /// Every reliable data structure API (on <see cref="IReliableCollection{T}"/>, <see cref="IReliableConcurrentQueue{T}"/>,
    /// etc.) that accepts a transaction and returns a <see cref="Task"/> must be awaited one at a time.
    /// </para>
    /// <para>
    /// Disposing a transaction that has not been committed implicitly aborts it, rolling back its uncommitted operations
    /// and releasing the locks it holds. Disposal is idempotent, and any failure during the implicit abort is ignored.
    /// </para>
    /// </remarks>
    /// <seealso cref="IReliableStateManager.CreateTransaction"/>
    /// <example>
    /// The following is an example of correct usage.
    /// <code language="csharp">
    /// <![CDATA[
    /// while (true)
    /// {
    ///     cancellation.ThrowIfCancellationRequested();
    /// 
    ///     try
    ///     {
    ///         using ITransaction tx = this.StateManager.CreateTransaction();
    ///         await concurrentQueue.EnqueueAsync(tx, 12L, cancellation);
    ///         await tx.CommitAsync();
    ///
    ///         return;
    ///     }
    ///     catch (TransactionFaultedException e)
    ///     {
    ///         // This indicates that the transaction was internally faulted by the system. One possible cause for this
    ///         // is that the transaction was long running and blocked a checkpoint. Increasing the
    ///         // "ReliableStateManagerReplicatorSettings.CheckpointThresholdInMB" will help reduce the chances of
    ///         // running into this exception.
    ///         Console.WriteLine("Transaction was internally faulted, retrying the transaction: " + e);
    ///     }
    ///     catch (FabricNotPrimaryException e)
    ///     {
    ///         // Gracefully exit RunAsync as the new primary should have RunAsync invoked on it and continue work.
    ///         // If instead enqueue was being executed as part of a client request, the client would be signaled to re-resolve.
    ///         Console.WriteLine("Replica is not primary, exiting RunAsync: " + e);
    ///         return;
    ///     }
    ///     catch (FabricNotReadableException e)
    ///     {
    ///         // Retry until the queue is readable or a different exception is thrown.
    ///         Console.WriteLine("Queue is not readable, retrying the transaction: " + e);
    ///     }
    ///     catch (FabricObjectClosedException e)
    ///     {
    ///         // Gracefully exit RunAsync as this is happening due to replica close.
    ///         // If instead enqueue was being executed as part of a client request, the client would be signaled to re-resolve.
    ///         Console.WriteLine("Replica is closing, exiting RunAsync: " + e);
    ///         return;
    ///     }
    ///     catch (TimeoutException e)
    ///     {
    ///         Console.WriteLine("Encountered TimeoutException during EnqueueAsync, retrying the transaction: " + e);
    ///     }
    ///
    ///     // Delay and retry.
    ///     await Task.Delay(TimeSpan.FromMilliseconds(100), cancellation);
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <example>
    /// The following is an example of incorrect usage that has undefined behavior.
    /// <code language="csharp">
    /// <![CDATA[
    /// using ITransaction tx = this.StateManager.CreateTransaction();
    /// List<Task<ConditionalValue<T>>> tasks = new();
    /// tasks.Add(concurrentQueue.TryDequeueAsync(tx, cancellation));
    /// tasks.Add(concurrentQueue.TryDequeueAsync(tx, cancellation));
    ///
    /// // Both TryDequeueAsync calls run on the same transaction and are awaited together; per the await-serialization
    /// // rule documented on this interface, every reliable data structure API (IReliableCollection<T>, IReliableConcurrentQueue<T>, etc.) on a transaction must be awaited one at a time.
    /// await Task.WhenAll(tasks);
    /// await tx.CommitAsync();
    /// ]]>
    /// </code>
    /// </example>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// Gets the sequence number assigned to the transaction when it was committed.
        /// </summary>
        /// <remarks>
        /// The value is <c>-1</c> before <see cref="CommitAsync"/> completes successfully, after <see cref="Abort"/>, or
        /// after the transaction is internally faulted.
        /// For a successfully committed read-only transaction the value also remains <c>-1</c>.
        /// </remarks>
        long CommitSequenceNumber { get; }

        /// <summary>
        /// Asynchronously commits the transaction.
        /// </summary>
        /// <exception cref="FabricNotPrimaryException">
        /// The transaction includes updates to <see cref="IReliableState"/> and the replica is not in the <see cref="ReplicaRole.Primary"/> role.
        /// Only <see cref="ReplicaRole.Primary"/> replicas are given write status.
        /// </exception>
        /// <exception cref="FabricObjectClosedException">The replica or a reliable state used by the transaction is closed.</exception>
        /// <exception cref="InvalidOperationException">The transaction is not in a valid state for this operation, for example,
        /// it has already been committed or aborted, or another operation is in progress on it.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry
        /// the operation on a new transaction.</exception>
        Task CommitAsync();

        /// <summary>
        /// Aborts the transaction, rolling back any uncommitted operations.
        /// </summary>
        /// <exception cref="InvalidOperationException">The transaction is not in a valid state for this operation, for example,
        /// it has already been committed or aborted, or another operation is in progress on it.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry
        /// the operation on a new transaction.</exception>
        void Abort();

        /// <summary>
        /// Gets the identifier assigned to the transaction when it was created.
        /// </summary>
        /// <remarks>
        /// The identifier remains stable for the lifetime of the transaction, which makes it useful for correlating the
        /// transaction across its operations and diagnostic traces.
        /// Identifiers are unique and monotonically increasing within the process that created the transaction, but can
        /// recur across replicas, partitions, and services.
        /// </remarks>
        long TransactionId { get; }

        /// <summary>
        /// Asynchronously returns the sequence number at or below which this transaction observes committed state.
        /// </summary>
        /// <remarks>
        /// The first call establishes the snapshot the transaction reads from, ensuring its reads observe a consistent point
        /// in time even as concurrent transactions commit. Use the returned value to correlate this transaction's snapshot
        /// reads with other versioned operations.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The transaction is not in a valid state for this operation, for example,
        /// it has already been committed or aborted, or another operation is in progress on it.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry
        /// the operation on a new transaction.</exception>
        Task<long> GetVisibilitySequenceNumberAsync();
    }
}
