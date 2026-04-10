// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a key-value pair with a sequence number.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public struct VersionedKeyValuePair<TKey, TValue>
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        public TKey Key => VersionedKey.Key;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// Gets the sequence number.
        /// </summary>
        public long SequenceNumber => VersionedKey.SequenceNumber;

        /// <summary>
        /// Gets the versioned key.
        /// </summary>
        public VersionedKey<TKey> VersionedKey { get; }

        /// <summary>
        /// Gets the key-value pair.
        /// </summary>
        public KeyValuePair<TKey, TValue> KeyValuePair => new KeyValuePair<TKey, TValue>(Key, Value);

        /// <summary>
        /// Initializes a new instance of the VersionedKeyValuePair structure with the specified key, value, and sequence number.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="sequenceNumber">The item's sequence number.</param>
        public VersionedKeyValuePair(TKey key, TValue value, long sequenceNumber)
        {
            VersionedKey = new VersionedKey<TKey>(key, sequenceNumber);
            Value = value;
        }
    }
}