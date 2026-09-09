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
    /// Provides extension methods for <see cref="IReliableDictionary4{TKey, TValue}"/>.
    /// </summary>
    public static class IReliableDictionaryExtensions
    {
        /// <inheritdoc path="/summary" cref="IReliableDictionary4{TKey, TValue}.RemoveAsync(ITransaction, TKey, TimeSpan, CancellationToken)"/>
        /// <remarks>
        /// This overload removes the key with a fixed four-second timeout and cannot be canceled. Call
        /// <see cref="IReliableDictionary4{TKey, TValue}.RemoveAsync(ITransaction, TKey, TimeSpan, CancellationToken)"/> to control
        /// the timeout or cancel the operation.
        /// </remarks>
        /// <inheritdoc path="/param[@name='tx']" cref="IReliableDictionary4{TKey, TValue}.RemoveAsync(ITransaction, TKey, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='key']" cref="IReliableDictionary4{TKey, TValue}.RemoveAsync(ITransaction, TKey, TimeSpan, CancellationToken)"/>
        /// <exception cref="TimeoutException">The operation failed to complete within the four-second timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="IReliableDictionary4{TKey, TValue}.RemoveAsync(ITransaction, TKey, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="IReliableDictionary4{TKey, TValue}.RemoveAsync(ITransaction, TKey, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="IReliableDictionary4{TKey, TValue}.RemoveAsync(ITransaction, TKey, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="IReliableDictionary4{TKey, TValue}.RemoveAsync(ITransaction, TKey, TimeSpan, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="IReliableDictionary4{TKey, TValue}.RemoveAsync(ITransaction, TKey, TimeSpan, CancellationToken)"/>
        public static Task<bool> RemoveAsync<TKey, TValue>(this IReliableDictionary4<TKey, TValue> reliableDictionary4Interface, ITransaction tx, TKey key)
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            return reliableDictionary4Interface.RemoveAsync(tx, key, TimeSpan.FromSeconds(4), CancellationToken.None);
        }
    }
}
