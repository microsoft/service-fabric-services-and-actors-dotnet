// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for KeyValueStoreReplicaCopyTypeReason.
    /// </summary>
    public enum KeyValueStoreReplicaCopyTypeReason
    {
        /// <summary>
        /// The reason has not been determined.
        /// </summary>
        Unknown,

        /// <summary>
        /// The secondary's epoch is invalid or corrupted, requiring a full copy.
        /// </summary>
        InvalidSecondaryEpoch,

        /// <summary>
        /// The secondary has no data (brand new replica), requiring a full copy.
        /// </summary>
        EmptySecondary,

        /// <summary>
        /// The secondary made progress on operations the primary does not recognize, requiring a full copy.
        /// </summary>
        FalseProgress,

        /// <summary>
        /// The secondary's configuration number matches an entry in the primary's epoch history, allowing a partial copy.
        /// </summary>
        MatchedConfigurationNumber,

        /// <summary>
        /// The secondary's epoch is not found in the primary's epoch history, requiring a full copy.
        /// </summary>
        EpochNotFound,

        /// <summary>
        /// The secondary has fallen too far behind for incremental copy.
        /// </summary>
        StaleSecondary,

        /// <summary>
        /// The primary's tombstone data has not been truncated yet, requiring a full copy.
        /// </summary>
        PrimaryTombstonesNotTruncated,
    }
}
