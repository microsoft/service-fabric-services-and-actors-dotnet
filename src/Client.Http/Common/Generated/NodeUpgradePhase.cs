// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for NodeUpgradePhase.
    /// </summary>
    public enum NodeUpgradePhase
    {
        /// <summary>
        /// Indicates the upgrade state is invalid. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// The upgrade has not started yet due to pending safety checks. The value is 1.
        /// </summary>
        PreUpgradeSafetyCheck,

        /// <summary>
        /// The upgrade is in progress. The value is 2.
        /// </summary>
        Upgrading,

        /// <summary>
        /// The upgrade has completed and post upgrade safety checks are being performed. The value is 3.
        /// </summary>
        PostUpgradeSafetyCheck,
    }
}
