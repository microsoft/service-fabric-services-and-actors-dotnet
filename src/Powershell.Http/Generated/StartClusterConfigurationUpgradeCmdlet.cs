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
    /// Start upgrading the configuration of a Service Fabric standalone cluster.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "SFClusterConfigurationUpgrade")]
    public partial class StartClusterConfigurationUpgradeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ClusterConfig. The cluster configuration as a JSON string. For example, [this
        /// file](https://github.com/Azure-Samples/service-fabric-dotnet-standalone-cluster-configuration/blob/master/Samples/ClusterConfig.Unsecure.DevCluster.json)
        /// contains JSON describing the [nodes and other properties of the
        /// cluster](https://docs.microsoft.com/azure/service-fabric/service-fabric-cluster-manifest).
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string ClusterConfig { get; set; }

        /// <summary>
        /// Gets or sets HealthCheckRetryTimeout. The length of time between attempts to perform health checks if the
        /// application or cluster is not healthy.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public TimeSpan? HealthCheckRetryTimeout { get; set; } = System.Xml.XmlConvert.ToTimeSpan("PT0H0M0S");

        /// <summary>
        /// Gets or sets HealthCheckWaitDurationInSeconds. The length of time to wait after completing an upgrade domain before
        /// starting the health checks process.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public TimeSpan? HealthCheckWaitDurationInSeconds { get; set; } = System.Xml.XmlConvert.ToTimeSpan("PT0H0M0S");

        /// <summary>
        /// Gets or sets HealthCheckStableDurationInSeconds. The length of time that the application or cluster must remain
        /// healthy before the upgrade proceeds to the next upgrade domain.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public TimeSpan? HealthCheckStableDurationInSeconds { get; set; } = System.Xml.XmlConvert.ToTimeSpan("PT0H0M0S");

        /// <summary>
        /// Gets or sets UpgradeDomainTimeoutInSeconds. The timeout for the upgrade domain.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public TimeSpan? UpgradeDomainTimeoutInSeconds { get; set; } = System.Xml.XmlConvert.ToTimeSpan("PT0H0M0S");

        /// <summary>
        /// Gets or sets UpgradeTimeoutInSeconds. The upgrade timeout.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public TimeSpan? UpgradeTimeoutInSeconds { get; set; } = System.Xml.XmlConvert.ToTimeSpan("PT0H0M0S");

        /// <summary>
        /// Gets or sets MaxPercentUnhealthyApplications. The maximum allowed percentage of unhealthy applications during the
        /// upgrade. Allowed values are integer values from zero to 100.
        /// </summary>
        [Parameter(Mandatory = false, Position = 6)]
        public int? MaxPercentUnhealthyApplications { get; set; } = 0;

        /// <summary>
        /// Gets or sets MaxPercentUnhealthyNodes. The maximum allowed percentage of unhealthy nodes during the upgrade.
        /// Allowed values are integer values from zero to 100.
        /// </summary>
        [Parameter(Mandatory = false, Position = 7)]
        public int? MaxPercentUnhealthyNodes { get; set; } = 0;

        /// <summary>
        /// Gets or sets MaxPercentDeltaUnhealthyNodes. The maximum allowed percentage of delta health degradation during the
        /// upgrade. Allowed values are integer values from zero to 100.
        /// </summary>
        [Parameter(Mandatory = false, Position = 8)]
        public int? MaxPercentDeltaUnhealthyNodes { get; set; } = 0;

        /// <summary>
        /// Gets or sets MaxPercentUpgradeDomainDeltaUnhealthyNodes. The maximum allowed percentage of upgrade domain delta
        /// health degradation during the upgrade. Allowed values are integer values from zero to 100.
        /// </summary>
        [Parameter(Mandatory = false, Position = 9)]
        public int? MaxPercentUpgradeDomainDeltaUnhealthyNodes { get; set; } = 0;

        /// <summary>
        /// Gets or sets ApplicationHealthPolicyMap. The wrapper that contains the map with application health policies used to
        /// evaluate specific applications in the cluster.
        /// </summary>
        [Parameter(Mandatory = false, Position = 10)]
        public IEnumerable<ApplicationHealthPolicyMapItem> ApplicationHealthPolicyMap { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 11)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var applicationHealthPolicies = new ApplicationHealthPolicies(
            applicationHealthPolicyMap: this.ApplicationHealthPolicyMap);

            var clusterConfigurationUpgradeDescription = new ClusterConfigurationUpgradeDescription(
            clusterConfig: this.ClusterConfig,
            healthCheckRetryTimeout: this.HealthCheckRetryTimeout,
            healthCheckWaitDurationInSeconds: this.HealthCheckWaitDurationInSeconds,
            healthCheckStableDurationInSeconds: this.HealthCheckStableDurationInSeconds,
            upgradeDomainTimeoutInSeconds: this.UpgradeDomainTimeoutInSeconds,
            upgradeTimeoutInSeconds: this.UpgradeTimeoutInSeconds,
            maxPercentUnhealthyApplications: this.MaxPercentUnhealthyApplications,
            maxPercentUnhealthyNodes: this.MaxPercentUnhealthyNodes,
            maxPercentDeltaUnhealthyNodes: this.MaxPercentDeltaUnhealthyNodes,
            maxPercentUpgradeDomainDeltaUnhealthyNodes: this.MaxPercentUpgradeDomainDeltaUnhealthyNodes,
            applicationHealthPolicies: applicationHealthPolicies);

            this.ServiceFabricClient.Cluster.StartClusterConfigurationUpgradeAsync(
                clusterConfigurationUpgradeDescription: clusterConfigurationUpgradeDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
