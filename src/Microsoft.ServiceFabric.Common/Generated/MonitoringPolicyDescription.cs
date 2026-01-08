// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for monitoring an upgrade in Monitored mode.
    /// </summary>
    public partial class MonitoringPolicyDescription
    {
        /// <summary>
        /// Initializes a new instance of the MonitoringPolicyDescription class.
        /// </summary>
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
        public MonitoringPolicyDescription(
            FailureAction? failureAction = default(FailureAction?),
            string healthCheckWaitDurationInMilliseconds = default(string),
            string healthCheckStableDurationInMilliseconds = default(string),
            string healthCheckRetryTimeoutInMilliseconds = default(string),
            string upgradeTimeoutInMilliseconds = default(string),
            string upgradeDomainTimeoutInMilliseconds = default(string))
        {
            this.FailureAction = failureAction;
            this.HealthCheckWaitDurationInMilliseconds = healthCheckWaitDurationInMilliseconds;
            this.HealthCheckStableDurationInMilliseconds = healthCheckStableDurationInMilliseconds;
            this.HealthCheckRetryTimeoutInMilliseconds = healthCheckRetryTimeoutInMilliseconds;
            this.UpgradeTimeoutInMilliseconds = upgradeTimeoutInMilliseconds;
            this.UpgradeDomainTimeoutInMilliseconds = upgradeDomainTimeoutInMilliseconds;
        }

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
    }
}
