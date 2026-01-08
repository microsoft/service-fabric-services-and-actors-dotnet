// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ServiceLoadMetricWeight.
    /// </summary>
    public enum ServiceLoadMetricWeight
    {
        /// <summary>
        /// Disables resource balancing for this metric. This value is zero.
        /// </summary>
        Zero,

        /// <summary>
        /// Specifies the metric weight of the service load as Low. The value is 1.
        /// </summary>
        Low,

        /// <summary>
        /// Specifies the metric weight of the service load as Medium. The value is 2.
        /// </summary>
        Medium,

        /// <summary>
        /// Specifies the metric weight of the service load as High. The value is 3.
        /// </summary>
        High,
    }
}
