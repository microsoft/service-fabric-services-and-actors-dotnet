// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for a compose deployment upgrade.
    /// </summary>
    public partial class ComposeDeploymentUpgradeProgressInfo
    {
        /// <summary>
        /// Initializes a new instance of the ComposeDeploymentUpgradeProgressInfo class.
        /// </summary>
        /// <param name="deploymentName">The name of the target deployment.</param>
        /// <param name="applicationName">The name of the target application, including the 'fabric:' URI scheme.</param>
        /// <param name="upgradeState">The state of the compose deployment upgrade.
        /// . Possible values include: 'Invalid', 'ProvisioningTarget', 'RollingForwardInProgress', 'RollingForwardPending',
        /// 'UnprovisioningCurrent', 'RollingForwardCompleted', 'RollingBackInProgress', 'UnprovisioningTarget',
        /// 'RollingBackCompleted', 'Failed'</param>
        /// <param name="upgradeStatusDetails">Additional detailed information about the status of the pending upgrade.</param>
        /// <param name="upgradeKind">The kind of upgrade out of the following possible values. Possible values include:
        /// 'Invalid', 'Rolling'</param>
        /// <param name="rollingUpgradeMode">The mode used to monitor health during a rolling upgrade. The values are
        /// UnmonitoredAuto, UnmonitoredManual, Monitored, and UnmonitoredDeferred. Possible values include: 'Invalid',
        /// 'UnmonitoredAuto', 'UnmonitoredManual', 'Monitored', 'UnmonitoredDeferred'</param>
        /// <param name="forceRestart">If true, then processes are forcefully restarted during upgrade even when the code
        /// version has not changed (the upgrade only changes configuration or data).</param>
        /// <param name="upgradeReplicaSetCheckTimeoutInSeconds">The maximum amount of time to block processing of an upgrade
        /// domain and prevent loss of availability when there are unexpected issues. When this timeout expires, processing of
        /// the upgrade domain will proceed regardless of availability loss issues. The timeout is reset at the start of each
        /// upgrade domain. Valid values are between 0 and 42949672925 inclusive. (unsigned 32-bit integer).</param>
        /// <param name="monitoringPolicy">Describes the parameters for monitoring an upgrade in Monitored mode.</param>
        /// <param name="applicationHealthPolicy">Defines a health policy used to evaluate the health of an application or one
        /// of its children entities.
        /// </param>
        /// <param name="targetApplicationTypeVersion">The target application type version (found in the application manifest)
        /// for the application upgrade.</param>
        /// <param name="upgradeDuration">The estimated amount of time that the overall upgrade elapsed. It is first
        /// interpreted as a string representing an ISO 8601 duration. If that fails, then it is interpreted as a number
        /// representing the total number of milliseconds.</param>
        /// <param name="currentUpgradeDomainDuration">The estimated amount of time spent processing current Upgrade Domain. It
        /// is first interpreted as a string representing an ISO 8601 duration. If that fails, then it is interpreted as a
        /// number representing the total number of milliseconds.</param>
        /// <param name="applicationUnhealthyEvaluations">List of health evaluations that resulted in the current aggregated
        /// health state.</param>
        /// <param name="currentUpgradeDomainProgress">Information about the current in-progress upgrade domain. Not applicable
        /// to node-by-node upgrades.</param>
        /// <param name="startTimestampUtc">The estimated UTC datetime when the upgrade started.</param>
        /// <param name="failureTimestampUtc">The estimated UTC datetime when the upgrade failed and FailureAction was
        /// executed.</param>
        /// <param name="failureReason">The cause of an upgrade failure that resulted in FailureAction being executed. Possible
        /// values include: 'None', 'Interrupted', 'HealthCheck', 'UpgradeDomainTimeout', 'OverallUpgradeTimeout'</param>
        /// <param name="upgradeDomainProgressAtFailure">Information about the upgrade domain progress at the time of upgrade
        /// failure.</param>
        /// <param name="applicationUpgradeStatusDetails">Additional details of application upgrade including failure
        /// message.</param>
        public ComposeDeploymentUpgradeProgressInfo(
            string deploymentName = default(string),
            string applicationName = default(string),
            ComposeDeploymentUpgradeState? upgradeState = default(ComposeDeploymentUpgradeState?),
            string upgradeStatusDetails = default(string),
            UpgradeKind? upgradeKind = Common.UpgradeKind.Rolling,
            UpgradeMode? rollingUpgradeMode = Common.UpgradeMode.UnmonitoredAuto,
            bool? forceRestart = default(bool?),
            long? upgradeReplicaSetCheckTimeoutInSeconds = default(long?),
            MonitoringPolicyDescription monitoringPolicy = default(MonitoringPolicyDescription),
            ApplicationHealthPolicy applicationHealthPolicy = default(ApplicationHealthPolicy),
            string targetApplicationTypeVersion = default(string),
            string upgradeDuration = default(string),
            string currentUpgradeDomainDuration = default(string),
            IEnumerable<HealthEvaluationWrapper> applicationUnhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>),
            CurrentUpgradeDomainProgressInfo currentUpgradeDomainProgress = default(CurrentUpgradeDomainProgressInfo),
            string startTimestampUtc = default(string),
            string failureTimestampUtc = default(string),
            FailureReason? failureReason = default(FailureReason?),
            FailureUpgradeDomainProgressInfo upgradeDomainProgressAtFailure = default(FailureUpgradeDomainProgressInfo),
            string applicationUpgradeStatusDetails = default(string))
        {
            this.DeploymentName = deploymentName;
            this.ApplicationName = applicationName;
            this.UpgradeState = upgradeState;
            this.UpgradeStatusDetails = upgradeStatusDetails;
            this.UpgradeKind = upgradeKind;
            this.RollingUpgradeMode = rollingUpgradeMode;
            this.ForceRestart = forceRestart;
            this.UpgradeReplicaSetCheckTimeoutInSeconds = upgradeReplicaSetCheckTimeoutInSeconds;
            this.MonitoringPolicy = monitoringPolicy;
            this.ApplicationHealthPolicy = applicationHealthPolicy;
            this.TargetApplicationTypeVersion = targetApplicationTypeVersion;
            this.UpgradeDuration = upgradeDuration;
            this.CurrentUpgradeDomainDuration = currentUpgradeDomainDuration;
            this.ApplicationUnhealthyEvaluations = applicationUnhealthyEvaluations;
            this.CurrentUpgradeDomainProgress = currentUpgradeDomainProgress;
            this.StartTimestampUtc = startTimestampUtc;
            this.FailureTimestampUtc = failureTimestampUtc;
            this.FailureReason = failureReason;
            this.UpgradeDomainProgressAtFailure = upgradeDomainProgressAtFailure;
            this.ApplicationUpgradeStatusDetails = applicationUpgradeStatusDetails;
        }

        /// <summary>
        /// Gets the name of the target deployment.
        /// </summary>
        public string DeploymentName { get; }

        /// <summary>
        /// Gets the name of the target application, including the 'fabric:' URI scheme.
        /// </summary>
        public string ApplicationName { get; }

        /// <summary>
        /// Gets the state of the compose deployment upgrade.
        /// . Possible values include: 'Invalid', 'ProvisioningTarget', 'RollingForwardInProgress', 'RollingForwardPending',
        /// 'UnprovisioningCurrent', 'RollingForwardCompleted', 'RollingBackInProgress', 'UnprovisioningTarget',
        /// 'RollingBackCompleted', 'Failed'
        /// </summary>
        public ComposeDeploymentUpgradeState? UpgradeState { get; }

        /// <summary>
        /// Gets additional detailed information about the status of the pending upgrade.
        /// </summary>
        public string UpgradeStatusDetails { get; }

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
        /// Gets if true, then processes are forcefully restarted during upgrade even when the code version has not changed
        /// (the upgrade only changes configuration or data).
        /// </summary>
        public bool? ForceRestart { get; }

        /// <summary>
        /// Gets the maximum amount of time to block processing of an upgrade domain and prevent loss of availability when
        /// there are unexpected issues. When this timeout expires, processing of the upgrade domain will proceed regardless of
        /// availability loss issues. The timeout is reset at the start of each upgrade domain. Valid values are between 0 and
        /// 42949672925 inclusive. (unsigned 32-bit integer).
        /// </summary>
        public long? UpgradeReplicaSetCheckTimeoutInSeconds { get; }

        /// <summary>
        /// Gets the parameters for monitoring an upgrade in Monitored mode.
        /// </summary>
        public MonitoringPolicyDescription MonitoringPolicy { get; }

        /// <summary>
        /// Gets defines a health policy used to evaluate the health of an application or one of its children entities.
        /// </summary>
        public ApplicationHealthPolicy ApplicationHealthPolicy { get; }

        /// <summary>
        /// Gets the target application type version (found in the application manifest) for the application upgrade.
        /// </summary>
        public string TargetApplicationTypeVersion { get; }

        /// <summary>
        /// Gets the estimated amount of time that the overall upgrade elapsed. It is first interpreted as a string
        /// representing an ISO 8601 duration. If that fails, then it is interpreted as a number representing the total number
        /// of milliseconds.
        /// </summary>
        public string UpgradeDuration { get; }

        /// <summary>
        /// Gets the estimated amount of time spent processing current Upgrade Domain. It is first interpreted as a string
        /// representing an ISO 8601 duration. If that fails, then it is interpreted as a number representing the total number
        /// of milliseconds.
        /// </summary>
        public string CurrentUpgradeDomainDuration { get; }

        /// <summary>
        /// Gets list of health evaluations that resulted in the current aggregated health state.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> ApplicationUnhealthyEvaluations { get; }

        /// <summary>
        /// Gets information about the current in-progress upgrade domain. Not applicable to node-by-node upgrades.
        /// </summary>
        public CurrentUpgradeDomainProgressInfo CurrentUpgradeDomainProgress { get; }

        /// <summary>
        /// Gets the estimated UTC datetime when the upgrade started.
        /// </summary>
        public string StartTimestampUtc { get; }

        /// <summary>
        /// Gets the estimated UTC datetime when the upgrade failed and FailureAction was executed.
        /// </summary>
        public string FailureTimestampUtc { get; }

        /// <summary>
        /// Gets the cause of an upgrade failure that resulted in FailureAction being executed. Possible values include:
        /// 'None', 'Interrupted', 'HealthCheck', 'UpgradeDomainTimeout', 'OverallUpgradeTimeout'
        /// </summary>
        public FailureReason? FailureReason { get; }

        /// <summary>
        /// Gets information about the upgrade domain progress at the time of upgrade failure.
        /// </summary>
        public FailureUpgradeDomainProgressInfo UpgradeDomainProgressAtFailure { get; }

        /// <summary>
        /// Gets additional details of application upgrade including failure message.
        /// </summary>
        public string ApplicationUpgradeStatusDetails { get; }
    }
}
