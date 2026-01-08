// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ReconfigurationType.
    /// </summary>
    public enum ReconfigurationType
    {
        /// <summary>
        /// Indicates the invalid reconfiguration type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Specifies that the primary replica is being swapped with a different replica.
        /// </summary>
        SwapPrimary,

        /// <summary>
        /// Reconfiguration triggered in response to a primary going down. This could be due to many reasons such as primary
        /// replica crashing etc.
        /// </summary>
        Failover,

        /// <summary>
        /// Reconfigurations where the primary replica is not changing.
        /// </summary>
        Other,
    }
}
