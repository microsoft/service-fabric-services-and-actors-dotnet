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
    /// Contains extension methods for IReliableDictionary4
    /// </summary>
    public static class IReliableDictionaryExtensions
    {
        /// <summary>
        /// Attempts to remove the value with the specified key without reading data from the disk.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the reliable dictionary.</typeparam>
        /// <typeparam name="TValue">
        /// The type of the values in the reliable dictionary.</typeparam>
        /// <param name="reliableDictionary4Interface">The instance of the generic class of IReliableDictionary4.</param>
        /// <param name="tx">Transaction to associate this operation with.</param>
        /// <param name="key">The key of the element to remove.</param>
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
        /// Task that represents the asynchronous remove operation. The task result is a bool indicating
        /// whether the key was removed from the Reliable Dictionary.
        /// </returns>
        public static Task<bool> RemoveAsync<TKey, TValue>(this IReliableDictionary4<TKey, TValue> reliableDictionary4Interface, ITransaction tx, TKey key)
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            return reliableDictionary4Interface.RemoveAsync(tx, key, TimeSpan.FromSeconds(4), CancellationToken.None);
        }
    }
}
