// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for KeyValueStoreReplicaCopyModeReason.
    /// </summary>
    public enum KeyValueStoreReplicaCopyModeReason
    {
        /// <summary>
        /// Physical copy is the default mode for full copies when all conditions allow it.
        /// </summary>
        DefaultPhysicalCopy,

        /// <summary>
        /// Logical copy was randomly selected based on configured probability for testing purposes.
        /// </summary>
        LogicalCopyProbability,

        /// <summary>
        /// The secondary's store format version is incompatible with the primary's, requiring logical copy.
        /// </summary>
        IncompatibleStoreFormatVersion,

        /// <summary>
        /// The primary database is empty, so physical copy is not needed.
        /// </summary>
        EmptyDatabase,

        /// <summary>
        /// The secondary replica does not support the file stream full copy protocol.
        /// </summary>
        FileStreamFullCopyNotSupportedBySecondary,

        /// <summary>
        /// The FullCopyMode configuration is explicitly set to Logical.
        /// </summary>
        FullCopyModeConfiguredAsLogical,

        /// <summary>
        /// The EnableFileStreamFullCopy configuration setting is disabled.
        /// </summary>
        EnableFileStreamFullCopySetToFalse,
    }
}
