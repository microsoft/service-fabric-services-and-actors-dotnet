// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Cluster Upgrade Started event.
    /// </summary>
    public partial class ClusterUpgradeStartedEvent : ClusterEvent
    {
        /// <summary>
        /// Initializes a new instance of the ClusterUpgradeStartedEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="currentClusterVersion">Current Cluster version.</param>
        /// <param name="targetClusterVersion">Target Cluster version.</param>
        /// <param name="upgradeType">Type of upgrade.</param>
        /// <param name="rollingUpgradeMode">Mode of upgrade.</param>
        /// <param name="failureAction">Action if failed.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ClusterUpgradeStartedEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            string currentClusterVersion,
            string targetClusterVersion,
            string upgradeType,
            string rollingUpgradeMode,
            string failureAction,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ClusterUpgradeStarted,
                category,
                hasCorrelatedEvents)
        {
            currentClusterVersion.ThrowIfNull(nameof(currentClusterVersion));
            targetClusterVersion.ThrowIfNull(nameof(targetClusterVersion));
            upgradeType.ThrowIfNull(nameof(upgradeType));
            rollingUpgradeMode.ThrowIfNull(nameof(rollingUpgradeMode));
            failureAction.ThrowIfNull(nameof(failureAction));
            this.CurrentClusterVersion = currentClusterVersion;
            this.TargetClusterVersion = targetClusterVersion;
            this.UpgradeType = upgradeType;
            this.RollingUpgradeMode = rollingUpgradeMode;
            this.FailureAction = failureAction;
        }

        /// <summary>
        /// Gets current Cluster version.
        /// </summary>
        public string CurrentClusterVersion { get; }

        /// <summary>
        /// Gets target Cluster version.
        /// </summary>
        public string TargetClusterVersion { get; }

        /// <summary>
        /// Gets type of upgrade.
        /// </summary>
        public string UpgradeType { get; }

        /// <summary>
        /// Gets mode of upgrade.
        /// </summary>
        public string RollingUpgradeMode { get; }

        /// <summary>
        /// Gets action if failed.
        /// </summary>
        public string FailureAction { get; }
    }
}
