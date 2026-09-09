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
    /// Represents a Reliable Collection of key/value pairs that are persisted and replicated, adding key removal that does
    /// not read the value from disk.
    /// </summary>
    /// <inheritdoc path="/remarks" cref="IReliableDictionary3{TKey, TValue}"/>
    public interface IReliableDictionary4<TKey, TValue> : IReliableDictionary3<TKey, TValue>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        /// <summary>
        /// Asynchronously attempts to remove the value with the specified key without reading the value from disk
        /// and returns <see langword="true"/> if the key was removed from the Reliable Dictionary; otherwise, <see langword="false"/>.
        /// </summary>
        /// <param name="tx">The transaction to associate this operation with.</param>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. Primarily used to prevent deadlocks.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="ArgumentException"><paramref name="timeout"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is <see langword="null"/>, or <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotPrimaryException">The <see cref="IReliableDictionary4{TKey, TValue}"/> is not in <see cref="ReplicaRole.Primary"/>.</exception>
        /// <exception cref="FabricObjectClosedException">The <see cref="IReliableDictionary4{TKey, TValue}"/> is closed or deleted.</exception>
        /// <exception cref="InvalidOperationException">A method call is invalid for the object's current state, for example, the transaction is already committed or aborted.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled via <paramref name="cancellationToken"/>.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        Task<bool> RemoveAsync(ITransaction tx, TKey key, TimeSpan timeout, CancellationToken cancellationToken);
    }
}
