// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for BackupSuspensionScope.
    /// </summary>
    public enum BackupSuspensionScope
    {
        /// <summary>
        /// Indicates an invalid backup suspension scope type also indicating entity is not suspended. All Service Fabric
        /// enumerations have the invalid type.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates the backup suspension is applied at partition level.
        /// </summary>
        Partition,

        /// <summary>
        /// Indicates the backup suspension is applied at service level. All partitions of the service are hence suspended for
        /// backup.
        /// </summary>
        Service,

        /// <summary>
        /// Indicates the backup suspension is applied at application level. All services and partitions of the application are
        /// hence suspended for backup.
        /// </summary>
        Application,
    }
}
