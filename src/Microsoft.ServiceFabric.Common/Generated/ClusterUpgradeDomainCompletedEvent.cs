// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Cluster Upgrade Domain Completed event.
    /// </summary>
    public partial class ClusterUpgradeDomainCompletedEvent : ClusterEvent
    {
        /// <summary>
        /// Initializes a new instance of the ClusterUpgradeDomainCompletedEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="targetClusterVersion">Target Cluster version.</param>
        /// <param name="upgradeState">State of upgrade.</param>
        /// <param name="upgradeDomains">Upgrade domains.</param>
        /// <param name="upgradeDomainElapsedTimeInMs">Duration of domain upgrade in milli-seconds.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ClusterUpgradeDomainCompletedEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            string targetClusterVersion,
            string upgradeState,
            string upgradeDomains,
            double? upgradeDomainElapsedTimeInMs,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ClusterUpgradeDomainCompleted,
                category,
                hasCorrelatedEvents)
        {
            targetClusterVersion.ThrowIfNull(nameof(targetClusterVersion));
            upgradeState.ThrowIfNull(nameof(upgradeState));
            upgradeDomains.ThrowIfNull(nameof(upgradeDomains));
            upgradeDomainElapsedTimeInMs.ThrowIfNull(nameof(upgradeDomainElapsedTimeInMs));
            this.TargetClusterVersion = targetClusterVersion;
            this.UpgradeState = upgradeState;
            this.UpgradeDomains = upgradeDomains;
            this.UpgradeDomainElapsedTimeInMs = upgradeDomainElapsedTimeInMs;
        }

        /// <summary>
        /// Gets target Cluster version.
        /// </summary>
        public string TargetClusterVersion { get; }

        /// <summary>
        /// Gets state of upgrade.
        /// </summary>
        public string UpgradeState { get; }

        /// <summary>
        /// Gets upgrade domains.
        /// </summary>
        public string UpgradeDomains { get; }

        /// <summary>
        /// Gets duration of domain upgrade in milli-seconds.
        /// </summary>
        public double? UpgradeDomainElapsedTimeInMs { get; }
    }
}
