// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ScalingTriggerKind.
    /// </summary>
    public enum ScalingTriggerKind
    {
        /// <summary>
        /// Indicates the scaling trigger is invalid. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates a trigger where scaling decisions are made based on average load of a partition. The value is 1.
        /// </summary>
        AveragePartitionLoad,

        /// <summary>
        /// Indicates a trigger where scaling decisions are made based on average load of a service. The value is 2.
        /// </summary>
        AverageServiceLoad,
    }
}
