// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for KvsReplicaCopyTypeReason.
    /// </summary>
    public enum KvsReplicaCopyTypeReason
    {
        /// <summary>
        /// The reason has not been determined. The value is 0.
        /// </summary>
        Unknown,

        /// <summary>
        /// The secondary's epoch is invalid or corrupted, requiring a full copy. The value is 1.
        /// </summary>
        InvalidSecondaryEpoch,

        /// <summary>
        /// The secondary has no data (brand new replica), requiring a full copy. The value is 2.
        /// </summary>
        EmptySecondary,

        /// <summary>
        /// The secondary made progress on operations the primary does not recognize, requiring a full copy. The value is 3.
        /// </summary>
        FalseProgress,

        /// <summary>
        /// The secondary's configuration number matches an entry in the primary's epoch history, allowing a partial copy. The
        /// value is 4.
        /// </summary>
        MatchedConfigurationNumber,

        /// <summary>
        /// The secondary's epoch is not found in the primary's epoch history, requiring a full copy. The value is 5.
        /// </summary>
        EpochNotFound,

        /// <summary>
        /// The secondary has fallen too far behind for incremental copy. The value is 6.
        /// </summary>
        StaleSecondary,

        /// <summary>
        /// The primary's tombstone data has not been truncated yet, requiring a full copy. The value is 7.
        /// </summary>
        PrimaryTombstonesNotTruncated,
    }
}
