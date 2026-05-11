// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for KvsReplicaCopyModeReason.
    /// </summary>
    public enum KvsReplicaCopyModeReason
    {
        /// <summary>
        /// Physical copy is the default mode for full copies when all conditions allow it. The value is 0.
        /// </summary>
        DefaultPhysicalCopy,

        /// <summary>
        /// Logical copy was randomly selected based on configured probability for testing purposes. The value is 1.
        /// </summary>
        LogicalCopyProbability,

        /// <summary>
        /// The secondary's store format version is incompatible with the primary's, requiring logical copy. The value is 2.
        /// </summary>
        IncompatibleStoreFormatVersion,

        /// <summary>
        /// The primary database is empty, so physical copy is not needed. The value is 3.
        /// </summary>
        EmptyDatabase,

        /// <summary>
        /// The secondary replica does not support the file stream full copy protocol. The value is 4.
        /// </summary>
        FileStreamFullCopyNotSupportedBySecondary,

        /// <summary>
        /// The FullCopyMode configuration is explicitly set to Logical. The value is 5.
        /// </summary>
        FullCopyModeConfiguredAsLogical,

        /// <summary>
        /// The EnableFileStreamFullCopy configuration setting is disabled. The value is 6.
        /// </summary>
        EnableFileStreamFullCopySetToFalse,
    }
}
