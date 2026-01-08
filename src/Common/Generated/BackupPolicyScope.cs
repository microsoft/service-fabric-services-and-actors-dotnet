// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for BackupPolicyScope.
    /// </summary>
    public enum BackupPolicyScope
    {
        /// <summary>
        /// Indicates an invalid backup policy scope type. All Service Fabric enumerations have the invalid type.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates the backup policy is applied at partition level. Hence overriding any policy which may have applied at
        /// partition's service or application level.
        /// </summary>
        Partition,

        /// <summary>
        /// Indicates the backup policy is applied at service level. All partitions of the service inherit this policy unless
        /// explicitly overridden at partition level.
        /// </summary>
        Service,

        /// <summary>
        /// Indicates the backup policy is applied at application level. All services and partitions of the application inherit
        /// this policy unless explicitly overridden at service or partition level.
        /// </summary>
        Application,
    }
}
