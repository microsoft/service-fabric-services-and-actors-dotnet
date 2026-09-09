// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a key-value pair together with the sequence number that identifies its version.
    /// </summary>
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
        /// Gets the sequence number that identifies the version of the <see cref="Key"/>.
        /// </summary>
        public long SequenceNumber => VersionedKey.SequenceNumber;

        /// <summary>
        /// Gets the <see cref="VersionedKey{TKey}"/>.
        /// </summary>
        public VersionedKey<TKey> VersionedKey { get; }

        /// <summary>
        /// Gets the <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </summary>
        public KeyValuePair<TKey, TValue> KeyValuePair => new KeyValuePair<TKey, TValue>(Key, Value);

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedKeyValuePair{TKey, TValue}"/> struct.
        /// </summary>
        public VersionedKeyValuePair(TKey key, TValue value, long sequenceNumber)
        {
            VersionedKey = new VersionedKey<TKey>(key, sequenceNumber);
            Value = value;
        }
    }
}
