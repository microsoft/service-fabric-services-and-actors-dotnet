// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for an application upgrade. Note that upgrade description replaces the existing
    /// application description. This means that if the parameters are not specified, the existing parameters on the
    /// applications will be overwritten with the empty parameters list. This would result in the application using the
    /// default value of the parameters from the application manifest. If you do not want to change any existing parameter
    /// values, please get the application parameters first using the GetApplicationInfo query and then supply those values
    /// as Parameters in this ApplicationUpgradeDescription.
    /// </summary>
    public partial class ApplicationUpgradeDescription
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationUpgradeDescription class.
        /// </summary>
        /// <param name="name">The name of the target application, including the 'fabric:' URI scheme.</param>
        /// <param name="targetApplicationTypeVersion">The target application type version (found in the application manifest)
        /// for the application upgrade.</param>
        /// <param name="upgradeKind">The kind of upgrade out of the following possible values. Possible values include:
        /// 'Invalid', 'Rolling'</param>
        /// <param name="parameters">List of application parameters with overridden values from their default values specified
        /// in the application manifest.</param>
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
        /// <param name="applicationHealthPolicy">Defines a health policy used to evaluate the health of an application or one
        /// of its children entities.
        /// </param>
        /// <param name="instanceCloseDelayDurationInSeconds">Duration in seconds, to wait before a stateless instance is
        /// closed, to allow the active requests to drain gracefully. This would be effective when the instance is closing
        /// during the application/cluster
        /// upgrade, only for those instances which have a non-zero delay duration configured in the service description. See
        /// InstanceCloseDelayDurationSeconds property in $ref: "#/definitions/StatelessServiceDescription.yaml" for details.
        /// Note, the default value of InstanceCloseDelayDurationInSeconds is 4294967295, which indicates that the behavior
        /// will entirely depend on the delay configured in the stateless service description.
        /// </param>
        /// <param name="managedApplicationIdentity">Managed application identity description.</param>
        public ApplicationUpgradeDescription(
            string name,
            string targetApplicationTypeVersion,
            UpgradeKind? upgradeKind = Common.UpgradeKind.Rolling,
            IReadOnlyDictionary<string, string> parameters = default(IReadOnlyDictionary<string, string>),
            UpgradeMode? rollingUpgradeMode = Common.UpgradeMode.UnmonitoredAuto,
            long? upgradeReplicaSetCheckTimeoutInSeconds = default(long?),
            bool? forceRestart = default(bool?),
            UpgradeSortOrder? sortOrder = Common.UpgradeSortOrder.Default,
            MonitoringPolicyDescription monitoringPolicy = default(MonitoringPolicyDescription),
            ApplicationHealthPolicy applicationHealthPolicy = default(ApplicationHealthPolicy),
            long? instanceCloseDelayDurationInSeconds = default(long?),
            ManagedApplicationIdentityDescription managedApplicationIdentity = default(ManagedApplicationIdentityDescription))
        {
            name.ThrowIfNull(nameof(name));
            targetApplicationTypeVersion.ThrowIfNull(nameof(targetApplicationTypeVersion));
            upgradeKind.ThrowIfNull(nameof(upgradeKind));
            this.Name = name;
            this.TargetApplicationTypeVersion = targetApplicationTypeVersion;
            this.UpgradeKind = upgradeKind;
            this.Parameters = parameters;
            this.RollingUpgradeMode = rollingUpgradeMode;
            this.UpgradeReplicaSetCheckTimeoutInSeconds = upgradeReplicaSetCheckTimeoutInSeconds;
            this.ForceRestart = forceRestart;
            this.SortOrder = sortOrder;
            this.MonitoringPolicy = monitoringPolicy;
            this.ApplicationHealthPolicy = applicationHealthPolicy;
            this.InstanceCloseDelayDurationInSeconds = instanceCloseDelayDurationInSeconds;
            this.ManagedApplicationIdentity = managedApplicationIdentity;
        }

        /// <summary>
        /// Gets the name of the target application, including the 'fabric:' URI scheme.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the target application type version (found in the application manifest) for the application upgrade.
        /// </summary>
        public string TargetApplicationTypeVersion { get; }

        /// <summary>
        /// Gets list of application parameters with overridden values from their default values specified in the application
        /// manifest.
        /// </summary>
        public IReadOnlyDictionary<string, string> Parameters { get; }

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
        /// Gets defines a health policy used to evaluate the health of an application or one of its children entities.
        /// </summary>
        public ApplicationHealthPolicy ApplicationHealthPolicy { get; }

        /// <summary>
        /// Gets duration in seconds, to wait before a stateless instance is closed, to allow the active requests to drain
        /// gracefully. This would be effective when the instance is closing during the application/cluster
        /// upgrade, only for those instances which have a non-zero delay duration configured in the service description. See
        /// InstanceCloseDelayDurationSeconds property in $ref: "#/definitions/StatelessServiceDescription.yaml" for details.
        /// Note, the default value of InstanceCloseDelayDurationInSeconds is 4294967295, which indicates that the behavior
        /// will entirely depend on the delay configured in the stateless service description.
        /// </summary>
        public long? InstanceCloseDelayDurationInSeconds { get; }

        /// <summary>
        /// Gets managed application identity description.
        /// </summary>
        public ManagedApplicationIdentityDescription ManagedApplicationIdentity { get; }
    }
}
