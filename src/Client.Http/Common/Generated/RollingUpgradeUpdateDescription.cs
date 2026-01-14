// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for updating a rolling upgrade of application or cluster.
    /// </summary>
    public partial class RollingUpgradeUpdateDescription
    {
        /// <summary>
        /// Initializes a new instance of the RollingUpgradeUpdateDescription class.
        /// </summary>
        /// <param name="rollingUpgradeMode">The mode used to monitor health during a rolling upgrade. The values are
        /// UnmonitoredAuto, UnmonitoredManual, Monitored, and UnmonitoredDeferred. Possible values include: 'Invalid',
        /// 'UnmonitoredAuto', 'UnmonitoredManual', 'Monitored', 'UnmonitoredDeferred'</param>
        /// <param name="forceRestart">If true, then processes are forcefully restarted during upgrade even when the code
        /// version has not changed (the upgrade only changes configuration or data).</param>
        /// <param name="replicaSetCheckTimeoutInMilliseconds">The maximum amount of time to block processing of an upgrade
        /// domain and prevent loss of availability when there are unexpected issues. When this timeout expires, processing of
        /// the upgrade domain will proceed regardless of availability loss issues. The timeout is reset at the start of each
        /// upgrade domain. Valid values are between 0 and 42949672925 inclusive. (unsigned 32-bit integer).</param>
        /// <param name="failureAction">The compensating action to perform when a Monitored upgrade encounters monitoring
        /// policy or health policy violations.
        /// Invalid indicates the failure action is invalid. Rollback specifies that the upgrade will start rolling back
        /// automatically.
        /// Manual indicates that the upgrade will switch to UnmonitoredManual upgrade mode.
        /// . Possible values include: 'Invalid', 'Rollback', 'Manual'</param>
        /// <param name="healthCheckWaitDurationInMilliseconds">The amount of time to wait after completing an upgrade domain
        /// before applying health policies. It is first interpreted as a string representing an ISO 8601 duration. If that
        /// fails, then it is interpreted as a number representing the total number of milliseconds.</param>
        /// <param name="healthCheckStableDurationInMilliseconds">The amount of time that the application or cluster must
        /// remain healthy before the upgrade proceeds to the next upgrade domain. It is first interpreted as a string
        /// representing an ISO 8601 duration. If that fails, then it is interpreted as a number representing the total number
        /// of milliseconds.</param>
        /// <param name="healthCheckRetryTimeoutInMilliseconds">The amount of time to retry health evaluation when the
        /// application or cluster is unhealthy before FailureAction is executed. It is first interpreted as a string
        /// representing an ISO 8601 duration. If that fails, then it is interpreted as a number representing the total number
        /// of milliseconds.</param>
        /// <param name="upgradeTimeoutInMilliseconds">The amount of time the overall upgrade has to complete before
        /// FailureAction is executed. It is first interpreted as a string representing an ISO 8601 duration. If that fails,
        /// then it is interpreted as a number representing the total number of milliseconds.</param>
        /// <param name="upgradeDomainTimeoutInMilliseconds">The amount of time each upgrade domain has to complete before
        /// FailureAction is executed. It is first interpreted as a string representing an ISO 8601 duration. If that fails,
        /// then it is interpreted as a number representing the total number of milliseconds.</param>
        /// <param name="instanceCloseDelayDurationInSeconds">Duration in seconds, to wait before a stateless instance is
        /// closed, to allow the active requests to drain gracefully. This would be effective when the instance is closing
        /// during the application/cluster
        /// upgrade, only for those instances which have a non-zero delay duration configured in the service description. See
        /// InstanceCloseDelayDurationSeconds property in $ref: "#/definitions/StatelessServiceDescription.yaml" for details.
        /// Note, the default value of InstanceCloseDelayDurationInSeconds is 4294967295, which indicates that the behavior
        /// will entirely depend on the delay configured in the stateless service description.
        /// </param>
        public RollingUpgradeUpdateDescription(
            UpgradeMode? rollingUpgradeMode = Common.UpgradeMode.UnmonitoredAuto,
            bool? forceRestart = default(bool?),
            long? replicaSetCheckTimeoutInMilliseconds = default(long?),
            FailureAction? failureAction = default(FailureAction?),
            string healthCheckWaitDurationInMilliseconds = default(string),
            string healthCheckStableDurationInMilliseconds = default(string),
            string healthCheckRetryTimeoutInMilliseconds = default(string),
            string upgradeTimeoutInMilliseconds = default(string),
            string upgradeDomainTimeoutInMilliseconds = default(string),
            long? instanceCloseDelayDurationInSeconds = default(long?))
        {
            rollingUpgradeMode.ThrowIfNull(nameof(rollingUpgradeMode));
            this.RollingUpgradeMode = rollingUpgradeMode;
            this.ForceRestart = forceRestart;
            this.ReplicaSetCheckTimeoutInMilliseconds = replicaSetCheckTimeoutInMilliseconds;
            this.FailureAction = failureAction;
            this.HealthCheckWaitDurationInMilliseconds = healthCheckWaitDurationInMilliseconds;
            this.HealthCheckStableDurationInMilliseconds = healthCheckStableDurationInMilliseconds;
            this.HealthCheckRetryTimeoutInMilliseconds = healthCheckRetryTimeoutInMilliseconds;
            this.UpgradeTimeoutInMilliseconds = upgradeTimeoutInMilliseconds;
            this.UpgradeDomainTimeoutInMilliseconds = upgradeDomainTimeoutInMilliseconds;
            this.InstanceCloseDelayDurationInSeconds = instanceCloseDelayDurationInSeconds;
        }

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
        public long? ReplicaSetCheckTimeoutInMilliseconds { get; }

        /// <summary>
        /// Gets the compensating action to perform when a Monitored upgrade encounters monitoring policy or health policy
        /// violations.
        /// Invalid indicates the failure action is invalid. Rollback specifies that the upgrade will start rolling back
        /// automatically.
        /// Manual indicates that the upgrade will switch to UnmonitoredManual upgrade mode.
        /// . Possible values include: 'Invalid', 'Rollback', 'Manual'
        /// </summary>
        public FailureAction? FailureAction { get; }

        /// <summary>
        /// Gets the amount of time to wait after completing an upgrade domain before applying health policies. It is first
        /// interpreted as a string representing an ISO 8601 duration. If that fails, then it is interpreted as a number
        /// representing the total number of milliseconds.
        /// </summary>
        public string HealthCheckWaitDurationInMilliseconds { get; }

        /// <summary>
        /// Gets the amount of time that the application or cluster must remain healthy before the upgrade proceeds to the next
        /// upgrade domain. It is first interpreted as a string representing an ISO 8601 duration. If that fails, then it is
        /// interpreted as a number representing the total number of milliseconds.
        /// </summary>
        public string HealthCheckStableDurationInMilliseconds { get; }

        /// <summary>
        /// Gets the amount of time to retry health evaluation when the application or cluster is unhealthy before
        /// FailureAction is executed. It is first interpreted as a string representing an ISO 8601 duration. If that fails,
        /// then it is interpreted as a number representing the total number of milliseconds.
        /// </summary>
        public string HealthCheckRetryTimeoutInMilliseconds { get; }

        /// <summary>
        /// Gets the amount of time the overall upgrade has to complete before FailureAction is executed. It is first
        /// interpreted as a string representing an ISO 8601 duration. If that fails, then it is interpreted as a number
        /// representing the total number of milliseconds.
        /// </summary>
        public string UpgradeTimeoutInMilliseconds { get; }

        /// <summary>
        /// Gets the amount of time each upgrade domain has to complete before FailureAction is executed. It is first
        /// interpreted as a string representing an ISO 8601 duration. If that fails, then it is interpreted as a number
        /// representing the total number of milliseconds.
        /// </summary>
        public string UpgradeDomainTimeoutInMilliseconds { get; }

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
