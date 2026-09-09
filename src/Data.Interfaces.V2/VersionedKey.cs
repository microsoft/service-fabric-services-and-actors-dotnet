// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    /// <summary>
    /// Represents a key together with the sequence number that identifies its version.
    /// </summary>
    public struct VersionedKey<TKey>
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        public TKey Key { get; }

        /// <summary>
        /// Gets the sequence number that identifies the version of the <see cref="Key"/>.
        /// </summary>
        public long SequenceNumber { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedKey{TKey}"/> struct.
        /// </summary>
        public VersionedKey(TKey key, long sequenceNumber)
        {
            Key = key;
            SequenceNumber = sequenceNumber;
        }
    }
}
