// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for starting a cluster upgrade.
    /// </summary>
    public partial class StartClusterUpgradeDescription
    {
        /// <summary>
        /// Initializes a new instance of the StartClusterUpgradeDescription class.
        /// </summary>
        /// <param name="codeVersion">The cluster code version.</param>
        /// <param name="configVersion">The cluster configuration version.</param>
        /// <param name="upgradeKind">The kind of upgrade out of the following possible values. Possible values include:
        /// 'Invalid', 'Rolling'</param>
        /// <param name="rollingUpgradeMode">The mode used to monitor health during a rolling upgrade. The values are
        /// UnmonitoredAuto, UnmonitoredManual, Monitored, and UnmonitoredDeferred. Possible values include: 'Invalid',
        /// 'UnmonitoredAuto', 'UnmonitoredManual', 'Monitored', 'UnmonitoredDeferred'</param>
        /// <param name="upgradeReplicaSetCheckTimeoutInSeconds">The maximum amount of time to block processing of an upgrade
        /// domain and prevent loss of availability when there are unexpected issues. When this timeout expires, processing of
        /// the upgrade domain will proceed regardless of availability loss issues. The timeout is reset at the start of each
        /// upgrade domain. Valid values are between 0 and 42949672925 inclusive. (unsigned 32-bit integer).</param>
        /// <param name="forceRestart">If true, then processes are forcefully restarted during upgrade even when the code
        /// version has not changed (the upgrade only changes configuration or data).</param>
        /// <param name="sortOrder">Defines the order in which an upgrade proceeds through the cluster. Possible values
        /// include: 'Invalid', 'Default', 'Numeric', 'Lexicographical', 'ReverseNumeric', 'ReverseLexicographical'</param>
        /// <param name="monitoringPolicy">Describes the parameters for monitoring an upgrade in Monitored mode.</param>
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
        /// <param name="instanceCloseDelayDurationInSeconds">Duration in seconds, to wait before a stateless instance is
        /// closed, to allow the active requests to drain gracefully. This would be effective when the instance is closing
        /// during the application/cluster
        /// upgrade, only for those instances which have a non-zero delay duration configured in the service description. See
        /// InstanceCloseDelayDurationSeconds property in $ref: "#/definitions/StatelessServiceDescription.yaml" for details.
        /// Note, the default value of InstanceCloseDelayDurationInSeconds is 4294967295, which indicates that the behavior
        /// will entirely depend on the delay configured in the stateless service description.
        /// </param>
        public StartClusterUpgradeDescription(
            string codeVersion = default(string),
            string configVersion = default(string),
            UpgradeKind? upgradeKind = Common.UpgradeKind.Rolling,
            UpgradeMode? rollingUpgradeMode = Common.UpgradeMode.UnmonitoredAuto,
            long? upgradeReplicaSetCheckTimeoutInSeconds = default(long?),
            bool? forceRestart = default(bool?),
            UpgradeSortOrder? sortOrder = Common.UpgradeSortOrder.Default,
            MonitoringPolicyDescription monitoringPolicy = default(MonitoringPolicyDescription),
            ClusterHealthPolicy clusterHealthPolicy = default(ClusterHealthPolicy),
            bool? enableDeltaHealthEvaluation = default(bool?),
            ClusterUpgradeHealthPolicyObject clusterUpgradeHealthPolicy = default(ClusterUpgradeHealthPolicyObject),
            ApplicationHealthPolicies applicationHealthPolicyMap = default(ApplicationHealthPolicies),
            long? instanceCloseDelayDurationInSeconds = default(long?))
        {
            this.CodeVersion = codeVersion;
            this.ConfigVersion = configVersion;
            this.UpgradeKind = upgradeKind;
            this.RollingUpgradeMode = rollingUpgradeMode;
            this.UpgradeReplicaSetCheckTimeoutInSeconds = upgradeReplicaSetCheckTimeoutInSeconds;
            this.ForceRestart = forceRestart;
            this.SortOrder = sortOrder;
            this.MonitoringPolicy = monitoringPolicy;
            this.ClusterHealthPolicy = clusterHealthPolicy;
            this.EnableDeltaHealthEvaluation = enableDeltaHealthEvaluation;
            this.ClusterUpgradeHealthPolicy = clusterUpgradeHealthPolicy;
            this.ApplicationHealthPolicyMap = applicationHealthPolicyMap;
            this.InstanceCloseDelayDurationInSeconds = instanceCloseDelayDurationInSeconds;
        }

        /// <summary>
        /// Gets the cluster code version.
        /// </summary>
        public string CodeVersion { get; }

        /// <summary>
        /// Gets the cluster configuration version.
        /// </summary>
        public string ConfigVersion { get; }

        /// <summary>
        /// Gets the kind of upgrade out of the following possible values. Possible values include: 'Invalid', 'Rolling'
        /// </summary>
        public UpgradeKind? UpgradeKind { get; }

        /// <summary>
        /// Gets the mode used to monitor health during a rolling upgrade. The values are UnmonitoredAuto, UnmonitoredManual,
        /// Monitored, and UnmonitoredDeferred. Possible values include: 'Invalid', 'UnmonitoredAuto', 'UnmonitoredManual',
        /// 'Monitored', 'UnmonitoredDeferred'
        /// </summary>
        public UpgradeMode? RollingUpgradeMode { get; }

        /// <summary>
        /// Gets the maximum amount of time to block processing of an upgrade domain and prevent loss of availability when
        /// there are unexpected issues. When this timeout expires, processing of the upgrade domain will proceed regardless of
        /// availability loss issues. The timeout is reset at the start of each upgrade domain. Valid values are between 0 and
        /// 42949672925 inclusive. (unsigned 32-bit integer).
        /// </summary>
        public long? UpgradeReplicaSetCheckTimeoutInSeconds { get; }

        /// <summary>
        /// Gets if true, then processes are forcefully restarted during upgrade even when the code version has not changed
        /// (the upgrade only changes configuration or data).
        /// </summary>
        public bool? ForceRestart { get; }

        /// <summary>
        /// Gets defines the order in which an upgrade proceeds through the cluster. Possible values include: 'Invalid',
        /// 'Default', 'Numeric', 'Lexicographical', 'ReverseNumeric', 'ReverseLexicographical'
        /// </summary>
        public UpgradeSortOrder? SortOrder { get; }

        /// <summary>
        /// Gets the parameters for monitoring an upgrade in Monitored mode.
        /// </summary>
        public MonitoringPolicyDescription MonitoringPolicy { get; }

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

        /// <summary>
        /// Gets duration in seconds, to wait before a stateless instance is closed, to allow the active requests to drain
        /// gracefully. This would be effective when the instance is closing during the application/cluster
        /// upgrade, only for those instances which have a non-zero delay duration configured in the service description. See
        /// InstanceCloseDelayDurationSeconds property in $ref: "#/definitions/StatelessServiceDescription.yaml" for details.
        /// Note, the default value of InstanceCloseDelayDurationInSeconds is 4294967295, which indicates that the behavior
        /// will entirely depend on the delay configured in the stateless service description.
        /// </summary>
        public long? InstanceCloseDelayDurationInSeconds { get; }
    }
}
