// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    /// <summary>
    /// Defines a key with a sequence number.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public struct VersionedKey<TKey>
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        public TKey Key { get; }

        /// <summary>
        /// Gets the sequence number.
        /// </summary>
        public long SequenceNumber { get; }

        /// <summary>
        /// Initializes a new instance of the VersionedKey structure with the specified key and sequence number.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sequenceNumber"></param>
        public VersionedKey(TKey key, long sequenceNumber)
        {
            Key = key;
            SequenceNumber = sequenceNumber;
        }
    }
}
