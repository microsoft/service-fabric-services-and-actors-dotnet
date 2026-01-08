// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for FailureReason.
    /// </summary>
    public enum FailureReason
    {
        /// <summary>
        /// Indicates the reason is invalid or unknown. All Service Fabric enumerations have the invalid type. The value is
        /// zero.
        /// </summary>
        None,

        /// <summary>
        /// There was an external request to roll back the upgrade. The value is 1.
        /// </summary>
        Interrupted,

        /// <summary>
        /// The upgrade failed due to health policy violations. The value is 2.
        /// </summary>
        HealthCheck,

        /// <summary>
        /// An upgrade domain took longer than the allowed upgrade domain timeout to process. The value is 3.
        /// </summary>
        UpgradeDomainTimeout,

        /// <summary>
        /// The overall upgrade took longer than the allowed upgrade timeout to process. The value is 4.
        /// </summary>
        OverallUpgradeTimeout,
    }
}
