// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes an application resource upgrade.
    /// </summary>
    public partial class ApplicationResourceUpgradeProgressInfo
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationResourceUpgradeProgressInfo class.
        /// </summary>
        /// <param name="name">Name of the Application resource.</param>
        /// <param name="targetApplicationTypeVersion">The target application version for the application upgrade.</param>
        /// <param name="startTimestampUtc">The estimated UTC datetime when the upgrade started.</param>
        /// <param name="upgradeState">The state of the application resource upgrade.
        /// . Possible values include: 'Invalid', 'ProvisioningTarget', 'RollingForward', 'UnprovisioningCurrent',
        /// 'CompletedRollforward', 'RollingBack', 'UnprovisioningTarget', 'CompletedRollback', 'Failed'</param>
        /// <param name="percentCompleted">The estimated percent of replicas are completed in the upgrade.</param>
        /// <param name="serviceUpgradeProgress">List of service upgrade progresses.</param>
        /// <param name="rollingUpgradeMode">The mode used to monitor health during a rolling upgrade. The values are
        /// UnmonitoredAuto, UnmonitoredManual, and Monitored. Possible values include: 'Invalid', 'UnmonitoredAuto',
        /// 'UnmonitoredManual', 'Monitored'</param>
        /// <param name="upgradeDuration">The estimated amount of time that the overall upgrade elapsed. It is first
        /// interpreted as a string representing an ISO 8601 duration. If that fails, then it is interpreted as a number
        /// representing the total number of milliseconds.</param>
        /// <param name="applicationUpgradeStatusDetails">Additional detailed information about the status of the pending
        /// upgrade.</param>
        /// <param name="upgradeReplicaSetCheckTimeoutInSeconds">The maximum amount of time to block processing of an upgrade
        /// domain and prevent loss of availability when there are unexpected issues. When this timeout expires, processing of
        /// the upgrade domain will proceed regardless of availability loss issues. The timeout is reset at the start of each
        /// upgrade domain. Valid values are between 0 and 42949672925 inclusive. (unsigned 32-bit integer).</param>
        /// <param name="failureTimestampUtc">The estimated UTC datetime when the upgrade failed and FailureAction was
        /// executed.</param>
        public ApplicationResourceUpgradeProgressInfo(
            string name = default(string),
            string targetApplicationTypeVersion = default(string),
            string startTimestampUtc = default(string),
            ApplicationResourceUpgradeState? upgradeState = default(ApplicationResourceUpgradeState?),
            string percentCompleted = default(string),
            IEnumerable<ServiceUpgradeProgress> serviceUpgradeProgress = default(IEnumerable<ServiceUpgradeProgress>),
            RollingUpgradeMode? rollingUpgradeMode = Common.RollingUpgradeMode.Monitored,
            string upgradeDuration = "PT0H2M0S",
            string applicationUpgradeStatusDetails = default(string),
            long? upgradeReplicaSetCheckTimeoutInSeconds = 42949672925,
            string failureTimestampUtc = default(string))
        {
            this.Name = name;
            this.TargetApplicationTypeVersion = targetApplicationTypeVersion;
            this.StartTimestampUtc = startTimestampUtc;
            this.UpgradeState = upgradeState;
            this.PercentCompleted = percentCompleted;
            this.ServiceUpgradeProgress = serviceUpgradeProgress;
            this.RollingUpgradeMode = rollingUpgradeMode;
            this.UpgradeDuration = upgradeDuration;
            this.ApplicationUpgradeStatusDetails = applicationUpgradeStatusDetails;
            this.UpgradeReplicaSetCheckTimeoutInSeconds = upgradeReplicaSetCheckTimeoutInSeconds;
            this.FailureTimestampUtc = failureTimestampUtc;
        }

        /// <summary>
        /// Gets name of the Application resource.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the target application version for the application upgrade.
        /// </summary>
        public string TargetApplicationTypeVersion { get; }

        /// <summary>
        /// Gets the estimated UTC datetime when the upgrade started.
        /// </summary>
        public string StartTimestampUtc { get; }

        /// <summary>
        /// Gets the state of the application resource upgrade.
        /// . Possible values include: 'Invalid', 'ProvisioningTarget', 'RollingForward', 'UnprovisioningCurrent',
        /// 'CompletedRollforward', 'RollingBack', 'UnprovisioningTarget', 'CompletedRollback', 'Failed'
        /// </summary>
        public ApplicationResourceUpgradeState? UpgradeState { get; }

        /// <summary>
        /// Gets the estimated percent of replicas are completed in the upgrade.
        /// </summary>
        public string PercentCompleted { get; }

        /// <summary>
        /// Gets list of service upgrade progresses.
        /// </summary>
        public IEnumerable<ServiceUpgradeProgress> ServiceUpgradeProgress { get; }

        /// <summary>
        /// Gets the mode used to monitor health during a rolling upgrade. The values are UnmonitoredAuto, UnmonitoredManual,
        /// and Monitored. Possible values include: 'Invalid', 'UnmonitoredAuto', 'UnmonitoredManual', 'Monitored'
        /// </summary>
        public RollingUpgradeMode? RollingUpgradeMode { get; }

        /// <summary>
        /// Gets the estimated amount of time that the overall upgrade elapsed. It is first interpreted as a string
        /// representing an ISO 8601 duration. If that fails, then it is interpreted as a number representing the total number
        /// of milliseconds.
        /// </summary>
        public string UpgradeDuration { get; }

        /// <summary>
        /// Gets additional detailed information about the status of the pending upgrade.
        /// </summary>
        public string ApplicationUpgradeStatusDetails { get; }

        /// <summary>
        /// Gets the maximum amount of time to block processing of an upgrade domain and prevent loss of availability when
        /// there are unexpected issues. When this timeout expires, processing of the upgrade domain will proceed regardless of
        /// availability loss issues. The timeout is reset at the start of each upgrade domain. Valid values are between 0 and
        /// 42949672925 inclusive. (unsigned 32-bit integer).
        /// </summary>
        public long? UpgradeReplicaSetCheckTimeoutInSeconds { get; }

        /// <summary>
        /// Gets the estimated UTC datetime when the upgrade failed and FailureAction was executed.
        /// </summary>
        public string FailureTimestampUtc { get; }
    }
}
