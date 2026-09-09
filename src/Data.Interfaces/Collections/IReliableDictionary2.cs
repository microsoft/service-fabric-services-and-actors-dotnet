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
    /// Represents a Reliable Collection of key/value pairs that are persisted and replicated, adding ordered key enumeration
    /// via <see cref="CreateKeyEnumerableAsync(ITransaction)"/> and a non-transactional <see cref="Count"/>.
    /// </summary>
    /// <inheritdoc path="/remarks" cref="IReliableDictionary{TKey,TValue}"/>
    public interface IReliableDictionary2<TKey, TValue> : IReliableDictionary<TKey, TValue>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        /// <inheritdoc cref="CreateKeyEnumerableAsync(ITransaction, EnumerationMode, TimeSpan, CancellationToken)"/>
        Task<IAsyncEnumerable<TKey>> CreateKeyEnumerableAsync(ITransaction txn);

        /// <inheritdoc cref="CreateKeyEnumerableAsync(ITransaction, EnumerationMode, TimeSpan, CancellationToken)"/>
        Task<IAsyncEnumerable<TKey>> CreateKeyEnumerableAsync(ITransaction txn, EnumerationMode enumerationMode);

        /// <summary>
        /// Asynchronously returns an enumerable over the keys of the <see cref="IReliableDictionary2{TKey,TValue}"/>.
        /// </summary>
        /// <param name="txn">The <see cref="ITransaction"/> to associate this operation with.</param>
        /// <param name="enumerationMode">An ignored enumeration mode. Results are always returned in ordered mode.</param>
        /// <param name="timeout">An ignored timeout.</param>
        /// <param name="cancellationToken">An ignored cancellation token.</param>
        /// <exception cref="ArgumentNullException"><paramref name="txn"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotReadableException">
        /// The <see cref="IReliableDictionary2{TKey,TValue}"/> cannot serve reads at the moment.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// One reason it may be thrown in the <see cref="ReplicaRole.Primary"/> role is loss of <see cref="IStatefulServicePartition.ReadStatus"/>.
        /// One reason it may be thrown in the <see cref="ReplicaRole.ActiveSecondary"/> role is that the state of the <see cref="IReliableDictionary2{TKey,TValue}"/> is not yet consistent.
        /// </exception>
        /// <exception cref="FabricObjectClosedException">The <see cref="IReliableDictionary2{TKey,TValue}"/> is closed or deleted.</exception>
        /// <exception cref="InvalidOperationException">A method call is invalid for the object's current state, for example, the transaction is already committed or aborted.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        /// <remarks>
        /// The enumerable returned from the Reliable Dictionary is safe to use concurrently with reads and writes
        /// to the dictionary. It represents a snapshot consistent view of the dictionary. Keys are always enumerated in ordered mode.
        /// </remarks>
        Task<IAsyncEnumerable<TKey>> CreateKeyEnumerableAsync(
            ITransaction txn, 
            EnumerationMode enumerationMode,
            TimeSpan timeout,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="IReliableDictionary2{TKey,TValue}"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="IReliableDictionary2{TKey,TValue}"/> has not been registered.</exception>
        /// <remarks>
        /// This property does not have transactional semantics. It represents the best-effort number of items 
        /// in the dictionary at the moment when the property was accessed.
        /// </remarks>
        long Count { get; }
    }
}
