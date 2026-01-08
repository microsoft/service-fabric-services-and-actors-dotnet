// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Cluster Upgrades Nodes Completed Event.
    /// </summary>
    public partial class ClusterUpgradesNodesCompleteEvent : ClusterEvent
    {
        /// <summary>
        /// Initializes a new instance of the ClusterUpgradesNodesCompleteEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="targetClusterVersion">version of target cluster.</param>
        /// <param name="upgradeState">state of the upgrade</param>
        /// <param name="upgradeNodes">the nodes being upgraded</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ClusterUpgradesNodesCompleteEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            string targetClusterVersion,
            string upgradeState,
            string upgradeNodes,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ClusterUpgradesNodesComplete,
                category,
                hasCorrelatedEvents)
        {
            targetClusterVersion.ThrowIfNull(nameof(targetClusterVersion));
            upgradeState.ThrowIfNull(nameof(upgradeState));
            upgradeNodes.ThrowIfNull(nameof(upgradeNodes));
            this.TargetClusterVersion = targetClusterVersion;
            this.UpgradeState = upgradeState;
            this.UpgradeNodes = upgradeNodes;
        }

        /// <summary>
        /// Gets version of target cluster.
        /// </summary>
        public string TargetClusterVersion { get; }

        /// <summary>
        /// Gets state of the upgrade
        /// </summary>
        public string UpgradeState { get; }

        /// <summary>
        /// Gets the nodes being upgraded
        /// </summary>
        public string UpgradeNodes { get; }
    }
}
