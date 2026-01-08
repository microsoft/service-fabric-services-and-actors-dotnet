// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for UpgradeMode.
    /// </summary>
    public enum UpgradeMode
    {
        /// <summary>
        /// Indicates the upgrade mode is invalid. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// The upgrade will proceed automatically without performing any health monitoring. The value is 1.
        /// </summary>
        UnmonitoredAuto,

        /// <summary>
        /// The upgrade will stop after completing each upgrade domain, giving the opportunity to manually monitor health
        /// before proceeding. The value is 2.
        /// </summary>
        UnmonitoredManual,

        /// <summary>
        /// The upgrade will stop after completing each upgrade domain and automatically monitor health before proceeding. The
        /// value is 3.
        /// </summary>
        Monitored,

        /// <summary>
        /// Perform a node-by-node upgrade. No action is performed when upgrade starts; upgrade is applied on each node when it
        /// is deactivated with intent restart or higher. The value is 4.
        /// </summary>
        UnmonitoredDeferred,
    }
}
