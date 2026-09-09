// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    /// <summary>
    /// Exposes an <see cref="IAsyncEnumerator{T}"/> for asynchronous iteration over a collection.
    /// </summary>
    public interface IAsyncEnumerable<out T>
    {
        /// <summary>
        /// Returns an <see cref="IAsyncEnumerator{T}"/> for the collection.
        /// </summary>
        IAsyncEnumerator<T> GetAsyncEnumerator();
    }
}
