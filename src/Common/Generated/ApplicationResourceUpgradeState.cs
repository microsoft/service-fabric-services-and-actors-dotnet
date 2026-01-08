// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ApplicationResourceUpgradeState.
    /// </summary>
    public enum ApplicationResourceUpgradeState
    {
        /// <summary>
        /// Indicates the upgrade state is invalid. All Service Fabric enumerations have the invalid type. The value is 0.
        /// </summary>
        Invalid,

        /// <summary>
        /// The upgrade is in the progress of provisioning target application type version. The value is 1.
        /// </summary>
        ProvisioningTarget,

        /// <summary>
        /// The upgrade is rolling forward to the target version but is not complete yet. The value is 2.
        /// </summary>
        RollingForward,

        /// <summary>
        /// The upgrade is in the progress of unprovisioning current application type version and rolling forward to the target
        /// version is completed. The value is 3.
        /// </summary>
        UnprovisioningCurrent,

        /// <summary>
        /// The upgrade has finished rolling forward. The value is 4.
        /// </summary>
        CompletedRollforward,

        /// <summary>
        /// The upgrade is rolling back to the previous version but is not complete yet. The value is 5.
        /// </summary>
        RollingBack,

        /// <summary>
        /// The upgrade is in the progress of unprovisioning target application type version and rolling back to the current
        /// version is completed. The value is 6.
        /// </summary>
        UnprovisioningTarget,

        /// <summary>
        /// The upgrade has finished rolling back. The value is 7.
        /// </summary>
        CompletedRollback,

        /// <summary>
        /// The upgrade has failed and is unable to execute FailureAction. The value is 8.
        /// </summary>
        Failed,
    }
}
