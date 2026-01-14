// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a health policy used to evaluate the health of the cluster during a cluster upgrade.
    /// </summary>
    public partial class ClusterUpgradeHealthPolicyObject
    {
        /// <summary>
        /// Initializes a new instance of the ClusterUpgradeHealthPolicyObject class.
        /// </summary>
        /// <param name="maxPercentDeltaUnhealthyNodes">The maximum allowed percentage of nodes health degradation allowed
        /// during cluster upgrades. The delta is measured between the state of the nodes at the beginning of upgrade and the
        /// state of the nodes at the time of the health evaluation. The check is performed after every upgrade domain upgrade
        /// completion to make sure the global state of the cluster is within tolerated limits. The default value is
        /// 10%.</param>
        /// <param name="maxPercentUpgradeDomainDeltaUnhealthyNodes">The maximum allowed percentage of upgrade domain nodes
        /// health degradation allowed during cluster upgrades. The delta is measured between the state of the upgrade domain
        /// nodes at the beginning of upgrade and the state of the upgrade domain nodes at the time of the health evaluation.
        /// The check is performed after every upgrade domain upgrade completion for all completed upgrade domains to make sure
        /// the state of the upgrade domains is within tolerated limits. The default value is 15%.</param>
        public ClusterUpgradeHealthPolicyObject(
            int? maxPercentDeltaUnhealthyNodes = default(int?),
            int? maxPercentUpgradeDomainDeltaUnhealthyNodes = default(int?))
        {
            maxPercentDeltaUnhealthyNodes?.ThrowIfOutOfInclusiveRange("maxPercentDeltaUnhealthyNodes", 0, 100);
            maxPercentUpgradeDomainDeltaUnhealthyNodes?.ThrowIfOutOfInclusiveRange("maxPercentUpgradeDomainDeltaUnhealthyNodes", 0, 100);
            this.MaxPercentDeltaUnhealthyNodes = maxPercentDeltaUnhealthyNodes;
            this.MaxPercentUpgradeDomainDeltaUnhealthyNodes = maxPercentUpgradeDomainDeltaUnhealthyNodes;
        }

        /// <summary>
        /// Gets the maximum allowed percentage of nodes health degradation allowed during cluster upgrades. The delta is
        /// measured between the state of the nodes at the beginning of upgrade and the state of the nodes at the time of the
        /// health evaluation. The check is performed after every upgrade domain upgrade completion to make sure the global
        /// state of the cluster is within tolerated limits. The default value is 10%.
        /// </summary>
        public int? MaxPercentDeltaUnhealthyNodes { get; }

        /// <summary>
        /// Gets the maximum allowed percentage of upgrade domain nodes health degradation allowed during cluster upgrades. The
        /// delta is measured between the state of the upgrade domain nodes at the beginning of upgrade and the state of the
        /// upgrade domain nodes at the time of the health evaluation. The check is performed after every upgrade domain
        /// upgrade completion for all completed upgrade domains to make sure the state of the upgrade domains is within
        /// tolerated limits. The default value is 15%.
        /// </summary>
        public int? MaxPercentUpgradeDomainDeltaUnhealthyNodes { get; }
    }
}
