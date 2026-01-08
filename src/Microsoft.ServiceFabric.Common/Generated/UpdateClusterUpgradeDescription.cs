// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Parameters for updating a cluster upgrade.
    /// </summary>
    public partial class UpdateClusterUpgradeDescription
    {
        /// <summary>
        /// Initializes a new instance of the UpdateClusterUpgradeDescription class.
        /// </summary>
        /// <param name="upgradeKind">The type of upgrade out of the following possible values. Possible values include:
        /// 'Invalid', 'Rolling', 'Rolling_ForceRestart'</param>
        /// <param name="updateDescription">Describes the parameters for updating a rolling upgrade of application or
        /// cluster.</param>
        /// <param name="clusterHealthPolicy">Defines a health policy used to evaluate the health of the cluster or of a
        /// cluster node.
        /// </param>
        /// <param name="enableDeltaHealthEvaluation">When true, enables delta health evaluation rather than absolute health
        /// evaluation after completion of each upgrade domain.</param>
        /// <param name="clusterUpgradeHealthPolicy">Defines a health policy used to evaluate the health of the cluster during
        /// a cluster upgrade.</param>
        /// <param name="applicationHealthPolicyMap">Defines the application health policy map used to evaluate the health of
        /// an application or one of its children entities.
        /// </param>
        public UpdateClusterUpgradeDescription(
            UpgradeType? upgradeKind = Common.UpgradeType.Rolling,
            RollingUpgradeUpdateDescription updateDescription = default(RollingUpgradeUpdateDescription),
            ClusterHealthPolicy clusterHealthPolicy = default(ClusterHealthPolicy),
            bool? enableDeltaHealthEvaluation = default(bool?),
            ClusterUpgradeHealthPolicyObject clusterUpgradeHealthPolicy = default(ClusterUpgradeHealthPolicyObject),
            ApplicationHealthPolicies applicationHealthPolicyMap = default(ApplicationHealthPolicies))
        {
            this.UpgradeKind = upgradeKind;
            this.UpdateDescription = updateDescription;
            this.ClusterHealthPolicy = clusterHealthPolicy;
            this.EnableDeltaHealthEvaluation = enableDeltaHealthEvaluation;
            this.ClusterUpgradeHealthPolicy = clusterUpgradeHealthPolicy;
            this.ApplicationHealthPolicyMap = applicationHealthPolicyMap;
        }

        /// <summary>
        /// Gets the type of upgrade out of the following possible values. Possible values include: 'Invalid', 'Rolling',
        /// 'Rolling_ForceRestart'
        /// </summary>
        public UpgradeType? UpgradeKind { get; }

        /// <summary>
        /// Gets the parameters for updating a rolling upgrade of application or cluster.
        /// </summary>
        public RollingUpgradeUpdateDescription UpdateDescription { get; }

        /// <summary>
        /// Gets defines a health policy used to evaluate the health of the cluster or of a cluster node.
        /// </summary>
        public ClusterHealthPolicy ClusterHealthPolicy { get; }

        /// <summary>
        /// Gets when true, enables delta health evaluation rather than absolute health evaluation after completion of each
        /// upgrade domain.
        /// </summary>
        public bool? EnableDeltaHealthEvaluation { get; }

        /// <summary>
        /// Gets defines a health policy used to evaluate the health of the cluster during a cluster upgrade.
        /// </summary>
        public ClusterUpgradeHealthPolicyObject ClusterUpgradeHealthPolicy { get; }

        /// <summary>
        /// Gets defines the application health policy map used to evaluate the health of an application or one of its children
        /// entities.
        /// </summary>
        public ApplicationHealthPolicies ApplicationHealthPolicyMap { get; }
    }
}
