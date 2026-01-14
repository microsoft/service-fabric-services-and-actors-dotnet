// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for MonitoredUpgradeHealthCheckPhase.
    /// </summary>
    public enum MonitoredUpgradeHealthCheckPhase
    {
        /// <summary>
        /// Indicates the upgrade mode is invalid. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// In this phase, ServiceFabric waits before it performs the initial health check after it finishes the upgrade on the
        /// upgrade domain. The value is 1.
        /// </summary>
        WaitDuration,

        /// <summary>
        /// In this phase, Service Fabric waits in order to verify that the application instance or cluster is stable before
        /// moving to the next upgrade domain or completing. The value is 2.
        /// </summary>
        StableDuration,

        /// <summary>
        /// In this phase, Service Fabric retries the health check if the previous health check fails. If the health check
        /// passes, the health check moves to StableDuration. The value is 3.
        /// </summary>
        Retry,
    }
}
