// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ScalingMechanismKind.
    /// </summary>
    public enum ScalingMechanismKind
    {
        /// <summary>
        /// Indicates the scaling mechanism is invalid. All Service Fabric enumerations have the invalid type. The value is
        /// zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates a mechanism for scaling where new instances are added or removed from a partition. The value is 1.
        /// </summary>
        PartitionInstanceCount,

        /// <summary>
        /// Indicates a mechanism for scaling where new named partitions are added or removed from a service. The value is 2.
        /// </summary>
        AddRemoveIncrementalNamedPartition,
    }
}
