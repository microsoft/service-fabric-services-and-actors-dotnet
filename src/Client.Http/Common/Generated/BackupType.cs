// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for BackupType.
    /// </summary>
    public enum BackupType
    {
        /// <summary>
        /// Indicates an invalid backup type. All Service Fabric enumerations have the invalid type.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates a full backup.
        /// </summary>
        Full,

        /// <summary>
        /// Indicates an incremental backup. A backup chain is comprised of a full backup followed by 0 or more incremental
        /// backups.
        /// </summary>
        Incremental,
    }
}
