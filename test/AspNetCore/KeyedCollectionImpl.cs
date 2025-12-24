// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.ObjectModel;

namespace Microsoft.ServiceFabric.AspNetCore.Tests
{
    /// <summary>
    /// Implementation for KeyedCollection to use with AspNetCoreCommunicationListener tests.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the collection.</typeparam>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    public class KeyedCollectionImpl<TKey, TItem> : KeyedCollection<TKey, TItem>
    {
        private readonly Func<TItem, TKey> getKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedCollectionImpl{TKey, TItem}"/> class.
        /// </summary>
        /// <param name="getKeyCallback">Callback method to get key.</param>
        protected internal KeyedCollectionImpl(Func<TItem, TKey> getKeyCallback)
            : base()
        {
            this.getKey = getKeyCallback;
        }

        /// <summary>
        /// Extracts the key from the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        /// <returns>The key for the specified element.</returns>
        protected override TKey GetKeyForItem(TItem item)
        {
            return this.getKey(item);
        }
    }
}
