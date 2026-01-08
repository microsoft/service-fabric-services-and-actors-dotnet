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
    /// Updates an ongoing application upgrade in the Service Fabric cluster.
    /// </summary>
    [Cmdlet(VerbsData.Update, "SFApplicationUpgrade")]
    public partial class UpdateApplicationUpgradeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ApplicationId. The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets Name. The name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public ApplicationName Name { get; set; }

        /// <summary>
        /// Gets or sets UpgradeKind. The kind of upgrade out of the following possible values. Possible values include:
        /// 'Invalid', 'Rolling'
        /// </summary>
        [Parameter(Mandatory = true, Position = 2)]
        public UpgradeKind? UpgradeKind { get; set; } = Common.UpgradeKind.Rolling;

        /// <summary>
        /// Gets or sets RollingUpgradeMode. The mode used to monitor health during a rolling upgrade. The values are
        /// UnmonitoredAuto, UnmonitoredManual, Monitored, and UnmonitoredDeferred. Possible values include: 'Invalid',
        /// 'UnmonitoredAuto', 'UnmonitoredManual', 'Monitored', 'UnmonitoredDeferred'
        /// </summary>
        [Parameter(Mandatory = true, Position = 3)]
        public UpgradeMode? RollingUpgradeMode { get; set; } = Common.UpgradeMode.UnmonitoredAuto;

        /// <summary>
        /// Gets or sets ConsiderWarningAsError. Indicates whether warnings are treated with the same severity as errors.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
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
        [Parameter(Mandatory = false, Position = 5)]
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
        [Parameter(Mandatory = false, Position = 6)]
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
        [Parameter(Mandatory = false, Position = 7)]
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
        [Parameter(Mandatory = false, Position = 8)]
        public int? MaxPercentUnhealthyServices { get; set; } = 0;

        /// <summary>
        /// Gets or sets ServiceTypeHealthPolicyMap. The map with service type health policy per service type name. The map is
        /// empty by default.
        /// </summary>
        [Parameter(Mandatory = false, Position = 9)]
        public IEnumerable<ServiceTypeHealthPolicyMapItem> ServiceTypeHealthPolicyMap { get; set; }

        /// <summary>
        /// Gets or sets ForceRestart. If true, then processes are forcefully restarted during upgrade even when the code
        /// version has not changed (the upgrade only changes configuration or data).
        /// </summary>
        [Parameter(Mandatory = false, Position = 10)]
        public bool? ForceRestart { get; set; }

        /// <summary>
        /// Gets or sets ReplicaSetCheckTimeoutInMilliseconds. The maximum amount of time to block processing of an upgrade
        /// domain and prevent loss of availability when there are unexpected issues. When this timeout expires, processing of
        /// the upgrade domain will proceed regardless of availability loss issues. The timeout is reset at the start of each
        /// upgrade domain. Valid values are between 0 and 42949672925 inclusive. (unsigned 32-bit integer).
        /// </summary>
        [Parameter(Mandatory = false, Position = 11)]
        public long? ReplicaSetCheckTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets FailureAction. The compensating action to perform when a Monitored upgrade encounters monitoring
        /// policy or health policy violations.
        /// Invalid indicates the failure action is invalid. Rollback specifies that the upgrade will start rolling back
        /// automatically.
        /// Manual indicates that the upgrade will switch to UnmonitoredManual upgrade mode.
        /// . Possible values include: 'Invalid', 'Rollback', 'Manual'
        /// </summary>
        [Parameter(Mandatory = false, Position = 12)]
        public FailureAction? FailureAction { get; set; }

        /// <summary>
        /// Gets or sets HealthCheckWaitDurationInMilliseconds. The amount of time to wait after completing an upgrade domain
        /// before applying health policies. It is first interpreted as a string representing an ISO 8601 duration. If that
        /// fails, then it is interpreted as a number representing the total number of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 13)]
        public string HealthCheckWaitDurationInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets HealthCheckStableDurationInMilliseconds. The amount of time that the application or cluster must
        /// remain healthy before the upgrade proceeds to the next upgrade domain. It is first interpreted as a string
        /// representing an ISO 8601 duration. If that fails, then it is interpreted as a number representing the total number
        /// of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 14)]
        public string HealthCheckStableDurationInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets HealthCheckRetryTimeoutInMilliseconds. The amount of time to retry health evaluation when the
        /// application or cluster is unhealthy before FailureAction is executed. It is first interpreted as a string
        /// representing an ISO 8601 duration. If that fails, then it is interpreted as a number representing the total number
        /// of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 15)]
        public string HealthCheckRetryTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets UpgradeTimeoutInMilliseconds. The amount of time the overall upgrade has to complete before
        /// FailureAction is executed. It is first interpreted as a string representing an ISO 8601 duration. If that fails,
        /// then it is interpreted as a number representing the total number of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 16)]
        public string UpgradeTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets UpgradeDomainTimeoutInMilliseconds. The amount of time each upgrade domain has to complete before
        /// FailureAction is executed. It is first interpreted as a string representing an ISO 8601 duration. If that fails,
        /// then it is interpreted as a number representing the total number of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 17)]
        public string UpgradeDomainTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets InstanceCloseDelayDurationInSeconds. Duration in seconds, to wait before a stateless instance is
        /// closed, to allow the active requests to drain gracefully. This would be effective when the instance is closing
        /// during the application/cluster
        /// upgrade, only for those instances which have a non-zero delay duration configured in the service description. See
        /// InstanceCloseDelayDurationSeconds property in $ref: "#/definitions/StatelessServiceDescription.yaml" for details.
        /// Note, the default value of InstanceCloseDelayDurationInSeconds is 4294967295, which indicates that the behavior
        /// will entirely depend on the delay configured in the stateless service description.
        /// </summary>
        [Parameter(Mandatory = false, Position = 18)]
        public long? InstanceCloseDelayDurationInSeconds { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 19)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var serviceTypeHealthPolicy = new ServiceTypeHealthPolicy(
            maxPercentUnhealthyPartitionsPerService: this.MaxPercentUnhealthyPartitionsPerService,
            maxPercentUnhealthyReplicasPerPartition: this.MaxPercentUnhealthyReplicasPerPartition,
            maxPercentUnhealthyServices: this.MaxPercentUnhealthyServices);

            var applicationHealthPolicy = new ApplicationHealthPolicy(
            considerWarningAsError: this.ConsiderWarningAsError,
            maxPercentUnhealthyDeployedApplications: this.MaxPercentUnhealthyDeployedApplications,
            defaultServiceTypeHealthPolicy: serviceTypeHealthPolicy,
            serviceTypeHealthPolicyMap: this.ServiceTypeHealthPolicyMap);

            var rollingUpgradeUpdateDescription = new RollingUpgradeUpdateDescription(
            rollingUpgradeMode: this.RollingUpgradeMode,
            forceRestart: this.ForceRestart,
            replicaSetCheckTimeoutInMilliseconds: this.ReplicaSetCheckTimeoutInMilliseconds,
            failureAction: this.FailureAction,
            healthCheckWaitDurationInMilliseconds: this.HealthCheckWaitDurationInMilliseconds,
            healthCheckStableDurationInMilliseconds: this.HealthCheckStableDurationInMilliseconds,
            healthCheckRetryTimeoutInMilliseconds: this.HealthCheckRetryTimeoutInMilliseconds,
            upgradeTimeoutInMilliseconds: this.UpgradeTimeoutInMilliseconds,
            upgradeDomainTimeoutInMilliseconds: this.UpgradeDomainTimeoutInMilliseconds,
            instanceCloseDelayDurationInSeconds: this.InstanceCloseDelayDurationInSeconds);

            var applicationUpgradeUpdateDescription = new ApplicationUpgradeUpdateDescription(
            name: this.Name,
            upgradeKind: this.UpgradeKind,
            applicationHealthPolicy: applicationHealthPolicy,
            updateDescription: rollingUpgradeUpdateDescription);

            this.ServiceFabricClient.Applications.UpdateApplicationUpgradeAsync(
                applicationId: this.ApplicationId,
                applicationUpgradeUpdateDescription: applicationUpgradeUpdateDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
