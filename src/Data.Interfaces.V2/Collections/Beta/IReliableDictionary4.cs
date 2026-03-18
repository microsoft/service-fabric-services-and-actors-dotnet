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
    public interface IReliableDictionary4<TKey, TValue> : IReliableDictionary3<TKey, TValue>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        /// <summary>
        /// Attempts to remove the value with the specified key without reading data from the disk.
        /// </summary>
        /// <param name="tx">Transaction to associate this operation with.</param>
        /// <param name="key">The key of the element to remove.</param>
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
        /// Task that represents the asynchronous remove operation. The task result is a bool indicating
        /// whether the key was removed from the Reliable Dictionary.
        /// </returns>
        Task<bool> RemoveAsync(ITransaction tx, TKey key, TimeSpan timeout, CancellationToken cancellationToken);
    }
}
