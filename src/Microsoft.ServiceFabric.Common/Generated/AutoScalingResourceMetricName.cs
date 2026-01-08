// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for AutoScalingResourceMetricName.
    /// </summary>
    public enum AutoScalingResourceMetricName
    {
        /// <summary>
        /// Indicates that the resource is CPU cores.
        /// </summary>
        Cpu,

        /// <summary>
        /// Indicates that the resource is memory in GB.
        /// </summary>
        MemoryInGB,
    }
}
