// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Starts upgrading a compose deployment in the Service Fabric cluster.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "SFComposeDeploymentUpgrade")]
    public partial class StartComposeDeploymentUpgradeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets DeploymentName. The identity of the deployment.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public string DeploymentName { get; set; }

        /// <summary>
        /// Gets or sets ComposeFileContent. The content of the compose file that describes the deployment to create.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string ComposeFileContent { get; set; }

        /// <summary>
        /// Gets or sets UpgradeKind. The kind of upgrade out of the following possible values. Possible values include:
        /// 'Invalid', 'Rolling'
        /// </summary>
        [Parameter(Mandatory = true, Position = 2)]
        public UpgradeKind? UpgradeKind { get; set; } = Common.UpgradeKind.Rolling;

        /// <summary>
        /// Gets or sets RegistryUserName. The user name to connect to container registry.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public string RegistryUserName { get; set; }

        /// <summary>
        /// Gets or sets RegistryPassword. The password for supplied username to connect to container registry.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public string RegistryPassword { get; set; }

        /// <summary>
        /// Gets or sets PasswordEncrypted. Indicates that supplied container registry password is encrypted.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public bool? PasswordEncrypted { get; set; }

        /// <summary>
        /// Gets or sets RollingUpgradeMode. The mode used to monitor health during a rolling upgrade. The values are
        /// UnmonitoredAuto, UnmonitoredManual, Monitored, and UnmonitoredDeferred. Possible values include: 'Invalid',
        /// 'UnmonitoredAuto', 'UnmonitoredManual', 'Monitored', 'UnmonitoredDeferred'
        /// </summary>
        [Parameter(Mandatory = false, Position = 6)]
        public UpgradeMode? RollingUpgradeMode { get; set; } = Common.UpgradeMode.UnmonitoredAuto;

        /// <summary>
        /// Gets or sets UpgradeReplicaSetCheckTimeoutInSeconds. The maximum amount of time to block processing of an upgrade
        /// domain and prevent loss of availability when there are unexpected issues. When this timeout expires, processing of
        /// the upgrade domain will proceed regardless of availability loss issues. The timeout is reset at the start of each
        /// upgrade domain. Valid values are between 0 and 42949672925 inclusive. (unsigned 32-bit integer).
        /// </summary>
        [Parameter(Mandatory = false, Position = 7)]
        public long? UpgradeReplicaSetCheckTimeoutInSeconds { get; set; }

        /// <summary>
        /// Gets or sets ForceRestart. If true, then processes are forcefully restarted during upgrade even when the code
        /// version has not changed (the upgrade only changes configuration or data).
        /// </summary>
        [Parameter(Mandatory = false, Position = 8)]
        public bool? ForceRestart { get; set; }

        /// <summary>
        /// Gets or sets FailureAction. The compensating action to perform when a Monitored upgrade encounters monitoring
        /// policy or health policy violations.
        /// Invalid indicates the failure action is invalid. Rollback specifies that the upgrade will start rolling back
        /// automatically.
        /// Manual indicates that the upgrade will switch to UnmonitoredManual upgrade mode.
        /// . Possible values include: 'Invalid', 'Rollback', 'Manual'
        /// </summary>
        [Parameter(Mandatory = false, Position = 9)]
        public FailureAction? FailureAction { get; set; }

        /// <summary>
        /// Gets or sets HealthCheckWaitDurationInMilliseconds. The amount of time to wait after completing an upgrade domain
        /// before applying health policies. It is first interpreted as a string representing an ISO 8601 duration. If that
        /// fails, then it is interpreted as a number representing the total number of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 10)]
        public string HealthCheckWaitDurationInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets HealthCheckStableDurationInMilliseconds. The amount of time that the application or cluster must
        /// remain healthy before the upgrade proceeds to the next upgrade domain. It is first interpreted as a string
        /// representing an ISO 8601 duration. If that fails, then it is interpreted as a number representing the total number
        /// of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 11)]
        public string HealthCheckStableDurationInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets HealthCheckRetryTimeoutInMilliseconds. The amount of time to retry health evaluation when the
        /// application or cluster is unhealthy before FailureAction is executed. It is first interpreted as a string
        /// representing an ISO 8601 duration. If that fails, then it is interpreted as a number representing the total number
        /// of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 12)]
        public string HealthCheckRetryTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets UpgradeTimeoutInMilliseconds. The amount of time the overall upgrade has to complete before
        /// FailureAction is executed. It is first interpreted as a string representing an ISO 8601 duration. If that fails,
        /// then it is interpreted as a number representing the total number of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 13)]
        public string UpgradeTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets UpgradeDomainTimeoutInMilliseconds. The amount of time each upgrade domain has to complete before
        /// FailureAction is executed. It is first interpreted as a string representing an ISO 8601 duration. If that fails,
        /// then it is interpreted as a number representing the total number of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 14)]
        public string UpgradeDomainTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets ConsiderWarningAsError. Indicates whether warnings are treated with the same severity as errors.
        /// </summary>
        [Parameter(Mandatory = false, Position = 15)]
        public bool? ConsiderWarningAsError { get; set; } = false;

        /// <summary>
        /// Gets or sets MaxPercentUnhealthyDeployedApplications. The maximum allowed percentage of unhealthy deployed
        /// applications. Allowed values are Byte values from zero to 100.
        /// The percentage represents the maximum tolerated percentage of deployed applications that can be unhealthy before
        /// the application is considered in error.
        /// This is calculated by dividing the number of unhealthy deployed applications over the number of nodes where the
        /// application is currently deployed on in the cluster.
        /// The computation rounds up to tolerate one failure on small numbers of nodes. Default percentage is zero.
        /// </summary>
        [Parameter(Mandatory = false, Position = 16)]
        public int? MaxPercentUnhealthyDeployedApplications { get; set; } = 0;

        /// <summary>
        /// Gets or sets MaxPercentUnhealthyPartitionsPerService. The maximum allowed percentage of unhealthy partitions per
        /// service. Allowed values are Byte values from zero to 100
        /// 
        /// The percentage represents the maximum tolerated percentage of partitions that can be unhealthy before the service
        /// is considered in error.
        /// If the percentage is respected but there is at least one unhealthy partition, the health is evaluated as Warning.
        /// The percentage is calculated by dividing the number of unhealthy partitions over the total number of partitions in
        /// the service.
        /// The computation rounds up to tolerate one failure on small numbers of partitions. Default percentage is zero.
        /// </summary>
        [Parameter(Mandatory = false, Position = 17)]
        public int? MaxPercentUnhealthyPartitionsPerService { get; set; } = 0;

        /// <summary>
        /// Gets or sets MaxPercentUnhealthyReplicasPerPartition. The maximum allowed percentage of unhealthy replicas per
        /// partition. Allowed values are Byte values from zero to 100.
        /// 
        /// The percentage represents the maximum tolerated percentage of replicas that can be unhealthy before the partition
        /// is considered in error.
        /// If the percentage is respected but there is at least one unhealthy replica, the health is evaluated as Warning.
        /// The percentage is calculated by dividing the number of unhealthy replicas over the total number of replicas in the
        /// partition.
        /// The computation rounds up to tolerate one failure on small numbers of replicas. Default percentage is zero.
        /// </summary>
        [Parameter(Mandatory = false, Position = 18)]
        public int? MaxPercentUnhealthyReplicasPerPartition { get; set; } = 0;

        /// <summary>
        /// Gets or sets MaxPercentUnhealthyServices. The maximum allowed percentage of unhealthy services. Allowed values are
        /// Byte values from zero to 100.
        /// 
        /// The percentage represents the maximum tolerated percentage of services that can be unhealthy before the application
        /// is considered in error.
        /// If the percentage is respected but there is at least one unhealthy service, the health is evaluated as Warning.
        /// This is calculated by dividing the number of unhealthy services of the specific service type over the total number
        /// of services of the specific service type.
        /// The computation rounds up to tolerate one failure on small numbers of services. Default percentage is zero.
        /// </summary>
        [Parameter(Mandatory = false, Position = 19)]
        public int? MaxPercentUnhealthyServices { get; set; } = 0;

        /// <summary>
        /// Gets or sets ServiceTypeHealthPolicyMap. The map with service type health policy per service type name. The map is
        /// empty by default.
        /// </summary>
        [Parameter(Mandatory = false, Position = 20)]
        public IEnumerable<ServiceTypeHealthPolicyMapItem> ServiceTypeHealthPolicyMap { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 21)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var registryCredential = new RegistryCredential(
            registryUserName: this.RegistryUserName,
            registryPassword: this.RegistryPassword,
            passwordEncrypted: this.PasswordEncrypted);

            var monitoringPolicyDescription = new MonitoringPolicyDescription(
            failureAction: this.FailureAction,
            healthCheckWaitDurationInMilliseconds: this.HealthCheckWaitDurationInMilliseconds,
            healthCheckStableDurationInMilliseconds: this.HealthCheckStableDurationInMilliseconds,
            healthCheckRetryTimeoutInMilliseconds: this.HealthCheckRetryTimeoutInMilliseconds,
            upgradeTimeoutInMilliseconds: this.UpgradeTimeoutInMilliseconds,
            upgradeDomainTimeoutInMilliseconds: this.UpgradeDomainTimeoutInMilliseconds);

            var serviceTypeHealthPolicy = new ServiceTypeHealthPolicy(
            maxPercentUnhealthyPartitionsPerService: this.MaxPercentUnhealthyPartitionsPerService,
            maxPercentUnhealthyReplicasPerPartition: this.MaxPercentUnhealthyReplicasPerPartition,
            maxPercentUnhealthyServices: this.MaxPercentUnhealthyServices);

            var applicationHealthPolicy = new ApplicationHealthPolicy(
            considerWarningAsError: this.ConsiderWarningAsError,
            maxPercentUnhealthyDeployedApplications: this.MaxPercentUnhealthyDeployedApplications,
            defaultServiceTypeHealthPolicy: serviceTypeHealthPolicy,
            serviceTypeHealthPolicyMap: this.ServiceTypeHealthPolicyMap);

            var composeDeploymentUpgradeDescription = new ComposeDeploymentUpgradeDescription(
            deploymentName: this.DeploymentName,
            composeFileContent: this.ComposeFileContent,
            upgradeKind: this.UpgradeKind,
            registryCredential: registryCredential,
            rollingUpgradeMode: this.RollingUpgradeMode,
            upgradeReplicaSetCheckTimeoutInSeconds: this.UpgradeReplicaSetCheckTimeoutInSeconds,
            forceRestart: this.ForceRestart,
            monitoringPolicy: monitoringPolicyDescription,
            applicationHealthPolicy: applicationHealthPolicy);

            this.ServiceFabricClient.ComposeDeployments.StartComposeDeploymentUpgradeAsync(
                deploymentName: this.DeploymentName,
                composeDeploymentUpgradeDescription: composeDeploymentUpgradeDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
