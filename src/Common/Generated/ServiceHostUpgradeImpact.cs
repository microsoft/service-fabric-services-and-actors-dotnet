// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ServiceHostUpgradeImpact.
    /// </summary>
    public enum ServiceHostUpgradeImpact
    {
        /// <summary>
        /// Indicates the upgrade impact is invalid. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// The upgrade is not expected to cause service host restarts. The value is 1.
        /// </summary>
        None,

        /// <summary>
        /// The upgrade is expected to cause a service host restart. The value is 2.
        /// </summary>
        ServiceHostRestart,

        /// <summary>
        /// The upgrade will cause an unexpected service host restart. This indicates a bug in the Service Fabric runtime and
        /// proceeding with an upgrade with this impact may lead to skipped safety checks. Running the upgrade with the
        /// ForceRestart flag will force proper safety checks. The value is 3.
        /// </summary>
        UnexpectedServiceHostRestart,
    }
}
