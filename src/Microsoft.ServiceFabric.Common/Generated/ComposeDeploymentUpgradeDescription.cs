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
    public partial class ComposeDeploymentUpgradeDescription
    {
        /// <summary>
        /// Initializes a new instance of the ComposeDeploymentUpgradeDescription class.
        /// </summary>
        /// <param name="deploymentName">The name of the deployment.</param>
        /// <param name="composeFileContent">The content of the compose file that describes the deployment to create.</param>
        /// <param name="upgradeKind">The kind of upgrade out of the following possible values. Possible values include:
        /// 'Invalid', 'Rolling'</param>
        /// <param name="registryCredential">Credential information to connect to container registry.</param>
        /// <param name="rollingUpgradeMode">The mode used to monitor health during a rolling upgrade. The values are
        /// UnmonitoredAuto, UnmonitoredManual, Monitored, and UnmonitoredDeferred. Possible values include: 'Invalid',
        /// 'UnmonitoredAuto', 'UnmonitoredManual', 'Monitored', 'UnmonitoredDeferred'</param>
        /// <param name="upgradeReplicaSetCheckTimeoutInSeconds">The maximum amount of time to block processing of an upgrade
        /// domain and prevent loss of availability when there are unexpected issues. When this timeout expires, processing of
        /// the upgrade domain will proceed regardless of availability loss issues. The timeout is reset at the start of each
        /// upgrade domain. Valid values are between 0 and 42949672925 inclusive. (unsigned 32-bit integer).</param>
        /// <param name="forceRestart">If true, then processes are forcefully restarted during upgrade even when the code
        /// version has not changed (the upgrade only changes configuration or data).</param>
        /// <param name="monitoringPolicy">Describes the parameters for monitoring an upgrade in Monitored mode.</param>
        /// <param name="applicationHealthPolicy">Defines a health policy used to evaluate the health of an application or one
        /// of its children entities.
        /// </param>
        public ComposeDeploymentUpgradeDescription(
            string deploymentName,
            string composeFileContent,
            UpgradeKind? upgradeKind = Common.UpgradeKind.Rolling,
            RegistryCredential registryCredential = default(RegistryCredential),
            UpgradeMode? rollingUpgradeMode = Common.UpgradeMode.UnmonitoredAuto,
            long? upgradeReplicaSetCheckTimeoutInSeconds = default(long?),
            bool? forceRestart = default(bool?),
            MonitoringPolicyDescription monitoringPolicy = default(MonitoringPolicyDescription),
            ApplicationHealthPolicy applicationHealthPolicy = default(ApplicationHealthPolicy))
        {
            deploymentName.ThrowIfNull(nameof(deploymentName));
            composeFileContent.ThrowIfNull(nameof(composeFileContent));
            upgradeKind.ThrowIfNull(nameof(upgradeKind));
            this.DeploymentName = deploymentName;
            this.ComposeFileContent = composeFileContent;
            this.UpgradeKind = upgradeKind;
            this.RegistryCredential = registryCredential;
            this.RollingUpgradeMode = rollingUpgradeMode;
            this.UpgradeReplicaSetCheckTimeoutInSeconds = upgradeReplicaSetCheckTimeoutInSeconds;
            this.ForceRestart = forceRestart;
            this.MonitoringPolicy = monitoringPolicy;
            this.ApplicationHealthPolicy = applicationHealthPolicy;
        }

        /// <summary>
        /// Gets the name of the deployment.
        /// </summary>
        public string DeploymentName { get; }

        /// <summary>
        /// Gets the content of the compose file that describes the deployment to create.
        /// </summary>
        public string ComposeFileContent { get; }

        /// <summary>
        /// Gets credential information to connect to container registry.
        /// </summary>
        public RegistryCredential RegistryCredential { get; }

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
        /// Gets the parameters for monitoring an upgrade in Monitored mode.
        /// </summary>
        public MonitoringPolicyDescription MonitoringPolicy { get; }

        /// <summary>
        /// Gets defines a health policy used to evaluate the health of an application or one of its children entities.
        /// </summary>
        public ApplicationHealthPolicy ApplicationHealthPolicy { get; }
    }
}
