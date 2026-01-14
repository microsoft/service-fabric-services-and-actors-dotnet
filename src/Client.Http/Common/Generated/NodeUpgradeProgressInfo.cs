// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the upgrading node and its status
    /// </summary>
    public partial class NodeUpgradeProgressInfo
    {
        /// <summary>
        /// Initializes a new instance of the NodeUpgradeProgressInfo class.
        /// </summary>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="upgradePhase">The state of the upgrading node. Possible values include: 'Invalid',
        /// 'PreUpgradeSafetyCheck', 'Upgrading', 'PostUpgradeSafetyCheck'</param>
        /// <param name="pendingSafetyChecks">List of pending safety checks</param>
        /// <param name="upgradeDuration">The estimated time spent processing the node since it was deactivated during a
        /// node-by-node upgrade.</param>
        public NodeUpgradeProgressInfo(
            NodeName nodeName = default(NodeName),
            NodeUpgradePhase? upgradePhase = default(NodeUpgradePhase?),
            IEnumerable<SafetyCheckWrapper> pendingSafetyChecks = default(IEnumerable<SafetyCheckWrapper>),
            string upgradeDuration = default(string))
        {
            this.NodeName = nodeName;
            this.UpgradePhase = upgradePhase;
            this.PendingSafetyChecks = pendingSafetyChecks;
            this.UpgradeDuration = upgradeDuration;
        }

        /// <summary>
        /// Gets the name of a Service Fabric node.
        /// </summary>
        public NodeName NodeName { get; }

        /// <summary>
        /// Gets the state of the upgrading node. Possible values include: 'Invalid', 'PreUpgradeSafetyCheck', 'Upgrading',
        /// 'PostUpgradeSafetyCheck'
        /// </summary>
        public NodeUpgradePhase? UpgradePhase { get; }

        /// <summary>
        /// Gets list of pending safety checks
        /// </summary>
        public IEnumerable<SafetyCheckWrapper> PendingSafetyChecks { get; }

        /// <summary>
        /// Gets the estimated time spent processing the node since it was deactivated during a node-by-node upgrade.
        /// </summary>
        public string UpgradeDuration { get; }
    }
}
