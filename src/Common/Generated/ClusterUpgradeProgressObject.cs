// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a cluster upgrade.
    /// </summary>
    public partial class ClusterUpgradeProgressObject
    {
        /// <summary>
        /// Initializes a new instance of the ClusterUpgradeProgressObject class.
        /// </summary>
        /// <param name="codeVersion">The ServiceFabric code version of the cluster.</param>
        /// <param name="configVersion">The cluster configuration version (specified in the cluster manifest).</param>
        /// <param name="upgradeDomains">List of upgrade domains and their statuses. Not applicable to node-by-node
        /// upgrades.</param>
        /// <param name="upgradeUnits">List of upgrade units and their statuses.</param>
        /// <param name="upgradeState">The state of the upgrade domain. Possible values include: 'Invalid',
        /// 'RollingBackInProgress', 'RollingBackCompleted', 'RollingForwardPending', 'RollingForwardInProgress',
        /// 'RollingForwardCompleted', 'Failed'</param>
        /// <param name="nextUpgradeDomain">The name of the next upgrade domain to be processed. Not applicable to node-by-node
        /// upgrades.</param>
        /// <param name="rollingUpgradeMode">The mode used to monitor health during a rolling upgrade. The values are
        /// UnmonitoredAuto, UnmonitoredManual, Monitored, and UnmonitoredDeferred. Possible values include: 'Invalid',
        /// 'UnmonitoredAuto', 'UnmonitoredManual', 'Monitored', 'UnmonitoredDeferred'</param>
        /// <param name="upgradeDescription">Represents a ServiceFabric cluster upgrade</param>
        /// <param name="upgradeDurationInMilliseconds">The estimated elapsed time spent processing the current overall
        /// upgrade.</param>
        /// <param name="upgradeDomainDurationInMilliseconds">The estimated elapsed time spent processing the current upgrade
        /// domain. Not applicable to node-by-node upgrades.</param>
        /// <param name="unhealthyEvaluations">List of health evaluations that resulted in the current aggregated health
        /// state.</param>
        /// <param name="currentUpgradeDomainProgress">Information about the current in-progress upgrade domain. Not applicable
        /// to node-by-node upgrades.</param>
        /// <param name="currentUpgradeUnitsProgress">Information about the current in-progress upgrade units.</param>
        /// <param name="startTimestampUtc">The start time of the upgrade in UTC.</param>
        /// <param name="failureTimestampUtc">The failure time of the upgrade in UTC.</param>
        /// <param name="failureReason">The cause of an upgrade failure that resulted in FailureAction being executed. Possible
        /// values include: 'None', 'Interrupted', 'HealthCheck', 'UpgradeDomainTimeout', 'OverallUpgradeTimeout'</param>
        /// <param name="upgradeDomainProgressAtFailure">The detailed upgrade progress for nodes in the current upgrade domain
        /// at the point of failure. Not applicable to node-by-node upgrades.</param>
        /// <param name="isNodeByNode">Indicates whether this upgrade is node-by-node.</param>
        /// <param name="healthCheckElapsedTime">The estimated elapsed time spent in the current health check phase.</param>
        /// <param name="healthCheckPhase">Specifies the current health check phase of a monitored upgrade when upgrading an
        /// application instance or cluster. Possible values include: 'Invalid', 'WaitDuration', 'StableDuration',
        /// 'Retry'</param>
        /// <param name="healthCheckRetryFlips">The number of times the health evaluation has toggled, causing the
        /// HealthCheckPhase to change and resetting the HealthCheckElapsedTime.</param>
        public ClusterUpgradeProgressObject(
            string codeVersion = default(string),
            string configVersion = default(string),
            IEnumerable<UpgradeDomainInfo> upgradeDomains = default(IEnumerable<UpgradeDomainInfo>),
            IEnumerable<UpgradeUnitInfo> upgradeUnits = default(IEnumerable<UpgradeUnitInfo>),
            UpgradeState? upgradeState = default(UpgradeState?),
            string nextUpgradeDomain = default(string),
            UpgradeMode? rollingUpgradeMode = Common.UpgradeMode.UnmonitoredAuto,
            ClusterUpgradeDescriptionObject upgradeDescription = default(ClusterUpgradeDescriptionObject),
            string upgradeDurationInMilliseconds = default(string),
            string upgradeDomainDurationInMilliseconds = default(string),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>),
            CurrentUpgradeDomainProgressInfo currentUpgradeDomainProgress = default(CurrentUpgradeDomainProgressInfo),
            CurrentUpgradeUnitsProgressInfo currentUpgradeUnitsProgress = default(CurrentUpgradeUnitsProgressInfo),
            string startTimestampUtc = default(string),
            string failureTimestampUtc = default(string),
            FailureReason? failureReason = default(FailureReason?),
            FailedUpgradeDomainProgressObject upgradeDomainProgressAtFailure = default(FailedUpgradeDomainProgressObject),
            bool? isNodeByNode = false,
            string healthCheckElapsedTime = default(string),
            MonitoredUpgradeHealthCheckPhase? healthCheckPhase = Common.MonitoredUpgradeHealthCheckPhase.Invalid,
            int? healthCheckRetryFlips = default(int?))
        {
            this.CodeVersion = codeVersion;
            this.ConfigVersion = configVersion;
            this.UpgradeDomains = upgradeDomains;
            this.UpgradeUnits = upgradeUnits;
            this.UpgradeState = upgradeState;
            this.NextUpgradeDomain = nextUpgradeDomain;
            this.RollingUpgradeMode = rollingUpgradeMode;
            this.UpgradeDescription = upgradeDescription;
            this.UpgradeDurationInMilliseconds = upgradeDurationInMilliseconds;
            this.UpgradeDomainDurationInMilliseconds = upgradeDomainDurationInMilliseconds;
            this.UnhealthyEvaluations = unhealthyEvaluations;
            this.CurrentUpgradeDomainProgress = currentUpgradeDomainProgress;
            this.CurrentUpgradeUnitsProgress = currentUpgradeUnitsProgress;
            this.StartTimestampUtc = startTimestampUtc;
            this.FailureTimestampUtc = failureTimestampUtc;
            this.FailureReason = failureReason;
            this.UpgradeDomainProgressAtFailure = upgradeDomainProgressAtFailure;
            this.IsNodeByNode = isNodeByNode;
            this.HealthCheckElapsedTime = healthCheckElapsedTime;
            this.HealthCheckPhase = healthCheckPhase;
            this.HealthCheckRetryFlips = healthCheckRetryFlips;
        }

        /// <summary>
        /// Gets the ServiceFabric code version of the cluster.
        /// </summary>
        public string CodeVersion { get; }

        /// <summary>
        /// Gets the cluster configuration version (specified in the cluster manifest).
        /// </summary>
        public string ConfigVersion { get; }

        /// <summary>
        /// Gets list of upgrade domains and their statuses. Not applicable to node-by-node upgrades.
        /// </summary>
        public IEnumerable<UpgradeDomainInfo> UpgradeDomains { get; }

        /// <summary>
        /// Gets list of upgrade units and their statuses.
        /// </summary>
        public IEnumerable<UpgradeUnitInfo> UpgradeUnits { get; }

        /// <summary>
        /// Gets the state of the upgrade domain. Possible values include: 'Invalid', 'RollingBackInProgress',
        /// 'RollingBackCompleted', 'RollingForwardPending', 'RollingForwardInProgress', 'RollingForwardCompleted', 'Failed'
        /// </summary>
        public UpgradeState? UpgradeState { get; }

        /// <summary>
        /// Gets the name of the next upgrade domain to be processed. Not applicable to node-by-node upgrades.
        /// </summary>
        public string NextUpgradeDomain { get; }

        /// <summary>
        /// Gets the mode used to monitor health during a rolling upgrade. The values are UnmonitoredAuto, UnmonitoredManual,
        /// Monitored, and UnmonitoredDeferred. Possible values include: 'Invalid', 'UnmonitoredAuto', 'UnmonitoredManual',
        /// 'Monitored', 'UnmonitoredDeferred'
        /// </summary>
        public UpgradeMode? RollingUpgradeMode { get; }

        /// <summary>
        /// Gets represents a ServiceFabric cluster upgrade
        /// </summary>
        public ClusterUpgradeDescriptionObject UpgradeDescription { get; }

        /// <summary>
        /// Gets the estimated elapsed time spent processing the current overall upgrade.
        /// </summary>
        public string UpgradeDurationInMilliseconds { get; }

        /// <summary>
        /// Gets the estimated elapsed time spent processing the current upgrade domain. Not applicable to node-by-node
        /// upgrades.
        /// </summary>
        public string UpgradeDomainDurationInMilliseconds { get; }

        /// <summary>
        /// Gets list of health evaluations that resulted in the current aggregated health state.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }

        /// <summary>
        /// Gets information about the current in-progress upgrade domain. Not applicable to node-by-node upgrades.
        /// </summary>
        public CurrentUpgradeDomainProgressInfo CurrentUpgradeDomainProgress { get; }

        /// <summary>
        /// Gets information about the current in-progress upgrade units.
        /// </summary>
        public CurrentUpgradeUnitsProgressInfo CurrentUpgradeUnitsProgress { get; }

        /// <summary>
        /// Gets the start time of the upgrade in UTC.
        /// </summary>
        public string StartTimestampUtc { get; }

        /// <summary>
        /// Gets the failure time of the upgrade in UTC.
        /// </summary>
        public string FailureTimestampUtc { get; }

        /// <summary>
        /// Gets the cause of an upgrade failure that resulted in FailureAction being executed. Possible values include:
        /// 'None', 'Interrupted', 'HealthCheck', 'UpgradeDomainTimeout', 'OverallUpgradeTimeout'
        /// </summary>
        public FailureReason? FailureReason { get; }

        /// <summary>
        /// Gets the detailed upgrade progress for nodes in the current upgrade domain at the point of failure. Not applicable
        /// to node-by-node upgrades.
        /// </summary>
        public FailedUpgradeDomainProgressObject UpgradeDomainProgressAtFailure { get; }

        /// <summary>
        /// Gets indicates whether this upgrade is node-by-node.
        /// </summary>
        public bool? IsNodeByNode { get; }

        /// <summary>
        /// Gets the estimated elapsed time spent in the current health check phase.
        /// </summary>
        public string HealthCheckElapsedTime { get; }

        /// <summary>
        /// Gets specifies the current health check phase of a monitored upgrade when upgrading an application instance or
        /// cluster. Possible values include: 'Invalid', 'WaitDuration', 'StableDuration', 'Retry'
        /// </summary>
        public MonitoredUpgradeHealthCheckPhase? HealthCheckPhase { get; }

        /// <summary>
        /// Gets the number of times the health evaluation has toggled, causing the HealthCheckPhase to change and resetting
        /// the HealthCheckElapsedTime.
        /// </summary>
        public int? HealthCheckRetryFlips { get; }
    }
}
