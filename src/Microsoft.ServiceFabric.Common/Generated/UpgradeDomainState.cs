// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for UpgradeDomainState.
    /// </summary>
    public enum UpgradeDomainState
    {
        /// <summary>
        /// Indicates the upgrade domain state is invalid. All Service Fabric enumerations have the invalid type. The value is
        /// zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// The upgrade domain has not started upgrading yet. The value is 1.
        /// </summary>
        Pending,

        /// <summary>
        /// The upgrade domain is being upgraded but not complete yet. The value is 2.
        /// </summary>
        InProgress,

        /// <summary>
        /// The upgrade domain has completed upgrade. The value is 3.
        /// </summary>
        Completed,
    }
}
