// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for UpgradeUnitState.
    /// </summary>
    public enum UpgradeUnitState
    {
        /// <summary>
        /// Indicates the upgrade unit state is invalid. All Service Fabric enumerations have the invalid type. The value is
        /// zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// The upgrade unit has not started upgrading yet. The value is 1.
        /// </summary>
        Pending,

        /// <summary>
        /// The upgrade unit is being upgraded but not complete yet. The value is 2.
        /// </summary>
        InProgress,

        /// <summary>
        /// The upgrade unit has completed upgrade. The value is 3.
        /// </summary>
        Completed,

        /// <summary>
        /// The upgrade unit has failed to upgrade. The value is 4.
        /// </summary>
        Failed,
    }
}
