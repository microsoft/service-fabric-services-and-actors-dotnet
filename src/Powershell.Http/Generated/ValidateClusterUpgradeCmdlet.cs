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
    /// Validate and assess the impact of a code or configuration version update of a Service Fabric cluster.
    /// </summary>
    [Cmdlet(VerbsCommon.Validate, "SFClusterUpgrade")]
    public partial class ValidateClusterUpgradeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets CodeVersion. The cluster code version.
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public string CodeVersion { get; set; }

        /// <summary>
        /// Gets or sets ConfigVersion. The cluster configuration version.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public string ConfigVersion { get; set; }

        /// <summary>
        /// Gets or sets UpgradeKind. The kind of upgrade out of the following possible values. Possible values include:
        /// 'Invalid', 'Rolling'
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public UpgradeKind? UpgradeKind { get; set; } = Common.UpgradeKind.Rolling;

        /// <summary>
        /// Gets or sets RollingUpgradeMode. The mode used to monitor health during a rolling upgrade. The values are
        /// UnmonitoredAuto, UnmonitoredManual, Monitored, and UnmonitoredDeferred. Possible values include: 'Invalid',
        /// 'UnmonitoredAuto', 'UnmonitoredManual', 'Monitored', 'UnmonitoredDeferred'
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public UpgradeMode? RollingUpgradeMode { get; set; } = Common.UpgradeMode.UnmonitoredAuto;

        /// <summary>
        /// Gets or sets UpgradeReplicaSetCheckTimeoutInSeconds. The maximum amount of time to block processing of an upgrade
        /// domain and prevent loss of availability when there are unexpected issues. When this timeout expires, processing of
        /// the upgrade domain will proceed regardless of availability loss issues. The timeout is reset at the start of each
        /// upgrade domain. Valid values are between 0 and 42949672925 inclusive. (unsigned 32-bit integer).
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public long? UpgradeReplicaSetCheckTimeoutInSeconds { get; set; }

        /// <summary>
        /// Gets or sets ForceRestart. If true, then processes are forcefully restarted during upgrade even when the code
        /// version has not changed (the upgrade only changes configuration or data).
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public bool? ForceRestart { get; set; }

        /// <summary>
        /// Gets or sets SortOrder. Defines the order in which an upgrade proceeds through the cluster. Possible values
        /// include: 'Invalid', 'Default', 'Numeric', 'Lexicographical', 'ReverseNumeric', 'ReverseLexicographical'
        /// </summary>
        [Parameter(Mandatory = false, Position = 6)]
        public UpgradeSortOrder? SortOrder { get; set; } = Common.UpgradeSortOrder.Default;

        /// <summary>
        /// Gets or sets FailureAction. The compensating action to perform when a Monitored upgrade encounters monitoring
        /// policy or health policy violations.
        /// Invalid indicates the failure action is invalid. Rollback specifies that the upgrade will start rolling back
        /// automatically.
        /// Manual indicates that the upgrade will switch to UnmonitoredManual upgrade mode.
        /// . Possible values include: 'Invalid', 'Rollback', 'Manual'
        /// </summary>
        [Parameter(Mandatory = false, Position = 7)]
        public FailureAction? FailureAction { get; set; }

        /// <summary>
        /// Gets or sets HealthCheckWaitDurationInMilliseconds. The amount of time to wait after completing an upgrade domain
        /// before applying health policies. It is first interpreted as a string representing an ISO 8601 duration. If that
        /// fails, then it is interpreted as a number representing the total number of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 8)]
        public string HealthCheckWaitDurationInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets HealthCheckStableDurationInMilliseconds. The amount of time that the application or cluster must
        /// remain healthy before the upgrade proceeds to the next upgrade domain. It is first interpreted as a string
        /// representing an ISO 8601 duration. If that fails, then it is interpreted as a number representing the total number
        /// of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 9)]
        public string HealthCheckStableDurationInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets HealthCheckRetryTimeoutInMilliseconds. The amount of time to retry health evaluation when the
        /// application or cluster is unhealthy before FailureAction is executed. It is first interpreted as a string
        /// representing an ISO 8601 duration. If that fails, then it is interpreted as a number representing the total number
        /// of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 10)]
        public string HealthCheckRetryTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets UpgradeTimeoutInMilliseconds. The amount of time the overall upgrade has to complete before
        /// FailureAction is executed. It is first interpreted as a string representing an ISO 8601 duration. If that fails,
        /// then it is interpreted as a number representing the total number of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 11)]
        public string UpgradeTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets UpgradeDomainTimeoutInMilliseconds. The amount of time each upgrade domain has to complete before
        /// FailureAction is executed. It is first interpreted as a string representing an ISO 8601 duration. If that fails,
        /// then it is interpreted as a number representing the total number of milliseconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 12)]
        public string UpgradeDomainTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets ConsiderWarningAsError. Indicates whether warnings are treated with the same severity as errors.
        /// </summary>
        [Parameter(Mandatory = false, Position = 13)]
        public bool? ConsiderWarningAsError { get; set; } = false;

        /// <summary>
        /// Gets or sets MaxPercentUnhealthyNodes. The maximum allowed percentage of unhealthy nodes before reporting an error.
        /// For example, to allow 10% of nodes to be unhealthy, this value would be 10.
        /// 
        /// The percentage represents the maximum tolerated percentage of nodes that can be unhealthy before the cluster is
        /// considered in error.
        /// If the percentage is respected but there is at least one unhealthy node, the health is evaluated as Warning.
        /// The percentage is calculated by dividing the number of unhealthy nodes over the total number of nodes in the
        /// cluster.
        /// The computation rounds up to tolerate one failure on small numbers of nodes. Default percentage is zero.
        /// 
        /// In large clusters, some nodes will always be down or out for repairs, so this percentage should be configured to
        /// tolerate that.
        /// </summary>
        [Parameter(Mandatory = false, Position = 14)]
        public int? MaxPercentUnhealthyNodes { get; set; } = 0;

        /// <summary>
        /// Gets or sets MaxPercentUnhealthyApplications. The maximum allowed percentage of unhealthy applications before
        /// reporting an error. For example, to allow 10% of applications to be unhealthy, this value would be 10.
        /// 
        /// The percentage represents the maximum tolerated percentage of applications that can be unhealthy before the cluster
        /// is considered in error.
        /// If the percentage is respected but there is at least one unhealthy application, the health is evaluated as Warning.
        /// This is calculated by dividing the number of unhealthy applications over the total number of application instances
        /// in the cluster, excluding applications of application types that are included in the
        /// ApplicationTypeHealthPolicyMap.
        /// The computation rounds up to tolerate one failure on small numbers of applications. Default percentage is zero.
        /// </summary>
        [Parameter(Mandatory = false, Position = 15)]
        public int? MaxPercentUnhealthyApplications { get; set; } = 0;

        /// <summary>
        /// Gets or sets ApplicationTypeHealthPolicyMap. Defines a map with max percentage unhealthy applications for specific
        /// application types.
        /// Each entry specifies as key the application type name and as value an integer that represents the
        /// MaxPercentUnhealthyApplications percentage used to evaluate the applications of the specified application type.
        /// 
        /// The application type health policy map can be used during cluster health evaluation to describe special application
        /// types.
        /// The application types included in the map are evaluated against the percentage specified in the map, and not with
        /// the global MaxPercentUnhealthyApplications defined in the cluster health policy.
        /// The applications of application types specified in the map are not counted against the global pool of applications.
        /// For example, if some applications of a type are critical, the cluster administrator can add an entry to the map for
        /// that application type
        /// and assign it a value of 0% (that is, do not tolerate any failures).
        /// All other applications can be evaluated with MaxPercentUnhealthyApplications set to 20% to tolerate some failures
        /// out of the thousands of application instances.
        /// The application type health policy map is used only if the cluster manifest enables application type health
        /// evaluation using the configuration entry for HealthManager/EnableApplicationTypeHealthEvaluation.
        /// </summary>
        [Parameter(Mandatory = false, Position = 16)]
        public IEnumerable<ApplicationTypeHealthPolicyMapItem> ApplicationTypeHealthPolicyMap { get; set; }

        /// <summary>
        /// Gets or sets NodeTypeHealthPolicyMap. Defines a map with max percentage unhealthy nodes for specific node types.
        /// Each entry specifies as key the node type name and as value an integer that represents the MaxPercentUnhealthyNodes
        /// percentage used to evaluate the nodes of the specified node type.
        /// 
        /// The node type health policy map can be used during cluster health evaluation to describe special node types.
        /// They are evaluated against the percentages associated with their node type name in the map.
        /// Setting this has no impact on the global pool of nodes used for MaxPercentUnhealthyNodes.
        /// The node type health policy map is used only if the cluster manifest enables node type health evaluation using the
        /// configuration entry for HealthManager/EnableNodeTypeHealthEvaluation.
        /// 
        /// For example, given a cluster with many nodes of different types, with important work hosted on node type
        /// "SpecialNodeType" that should not tolerate any nodes down.
        /// You can specify global MaxPercentUnhealthyNodes to 20% to tolerate some failures for all nodes, but for the node
        /// type "SpecialNodeType", set the MaxPercentUnhealthyNodes to 0 by
        /// setting the value in the key value pair in NodeTypeHealthPolicyMapItem. The key is the node type name.
        /// This way, as long as no nodes of type "SpecialNodeType" are in Error state,
        /// even if some of the many nodes in the global pool are in Error state, but below the global unhealthy percentage,
        /// the cluster would be evaluated to Warning.
        /// A Warning health state does not impact cluster upgrade or other monitoring triggered by Error health state.
        /// But even one node of type SpecialNodeType in Error would make cluster unhealthy (in Error rather than Warning/Ok),
        /// which triggers rollback or pauses the cluster upgrade, depending on the upgrade configuration.
        /// 
        /// Conversely, setting the global MaxPercentUnhealthyNodes to 0, and setting SpecialNodeType's max percent unhealthy
        /// nodes to 100,
        /// with one node of type SpecialNodeType in Error state would still put the cluster in an Error state, since the
        /// global restriction is more strict in this case.
        /// </summary>
        [Parameter(Mandatory = false, Position = 17)]
        public IEnumerable<NodeTypeHealthPolicyMapItem> NodeTypeHealthPolicyMap { get; set; }

        /// <summary>
        /// Gets or sets EnableDeltaHealthEvaluation. When true, enables delta health evaluation rather than absolute health
        /// evaluation after completion of each upgrade domain.
        /// </summary>
        [Parameter(Mandatory = false, Position = 18)]
        public bool? EnableDeltaHealthEvaluation { get; set; }

        /// <summary>
        /// Gets or sets MaxPercentDeltaUnhealthyNodes. The maximum allowed percentage of nodes health degradation allowed
        /// during cluster upgrades. The delta is measured between the state of the nodes at the beginning of upgrade and the
        /// state of the nodes at the time of the health evaluation. The check is performed after every upgrade domain upgrade
        /// completion to make sure the global state of the cluster is within tolerated limits. The default value is 10%.
        /// </summary>
        [Parameter(Mandatory = false, Position = 19)]
        public int? MaxPercentDeltaUnhealthyNodes { get; set; }

        /// <summary>
        /// Gets or sets MaxPercentUpgradeDomainDeltaUnhealthyNodes. The maximum allowed percentage of upgrade domain nodes
        /// health degradation allowed during cluster upgrades. The delta is measured between the state of the upgrade domain
        /// nodes at the beginning of upgrade and the state of the upgrade domain nodes at the time of the health evaluation.
        /// The check is performed after every upgrade domain upgrade completion for all completed upgrade domains to make sure
        /// the state of the upgrade domains is within tolerated limits. The default value is 15%.
        /// </summary>
        [Parameter(Mandatory = false, Position = 20)]
        public int? MaxPercentUpgradeDomainDeltaUnhealthyNodes { get; set; }

        /// <summary>
        /// Gets or sets ApplicationHealthPolicyMap. The wrapper that contains the map with application health policies used to
        /// evaluate specific applications in the cluster.
        /// </summary>
        [Parameter(Mandatory = false, Position = 21)]
        public IEnumerable<ApplicationHealthPolicyMapItem> ApplicationHealthPolicyMap { get; set; }

        /// <summary>
        /// Gets or sets InstanceCloseDelayDurationInSeconds. Duration in seconds, to wait before a stateless instance is
        /// closed, to allow the active requests to drain gracefully. This would be effective when the instance is closing
        /// during the application/cluster
        /// upgrade, only for those instances which have a non-zero delay duration configured in the service description. See
        /// InstanceCloseDelayDurationSeconds property in $ref: "#/definitions/StatelessServiceDescription.yaml" for details.
        /// Note, the default value of InstanceCloseDelayDurationInSeconds is 4294967295, which indicates that the behavior
        /// will entirely depend on the delay configured in the stateless service description.
        /// </summary>
        [Parameter(Mandatory = false, Position = 22)]
        public long? InstanceCloseDelayDurationInSeconds { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 23)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var monitoringPolicyDescription = new MonitoringPolicyDescription(
            failureAction: this.FailureAction,
            healthCheckWaitDurationInMilliseconds: this.HealthCheckWaitDurationInMilliseconds,
            healthCheckStableDurationInMilliseconds: this.HealthCheckStableDurationInMilliseconds,
            healthCheckRetryTimeoutInMilliseconds: this.HealthCheckRetryTimeoutInMilliseconds,
            upgradeTimeoutInMilliseconds: this.UpgradeTimeoutInMilliseconds,
            upgradeDomainTimeoutInMilliseconds: this.UpgradeDomainTimeoutInMilliseconds);

            var clusterHealthPolicy = new ClusterHealthPolicy(
            considerWarningAsError: this.ConsiderWarningAsError,
            maxPercentUnhealthyNodes: this.MaxPercentUnhealthyNodes,
            maxPercentUnhealthyApplications: this.MaxPercentUnhealthyApplications,
            applicationTypeHealthPolicyMap: this.ApplicationTypeHealthPolicyMap,
            nodeTypeHealthPolicyMap: this.NodeTypeHealthPolicyMap);

            var clusterUpgradeHealthPolicyObject = new ClusterUpgradeHealthPolicyObject(
            maxPercentDeltaUnhealthyNodes: this.MaxPercentDeltaUnhealthyNodes,
            maxPercentUpgradeDomainDeltaUnhealthyNodes: this.MaxPercentUpgradeDomainDeltaUnhealthyNodes);

            var applicationHealthPolicies = new ApplicationHealthPolicies(
            applicationHealthPolicyMap: this.ApplicationHealthPolicyMap);

            var startClusterUpgradeDescription = new StartClusterUpgradeDescription(
            codeVersion: this.CodeVersion,
            configVersion: this.ConfigVersion,
            upgradeKind: this.UpgradeKind,
            rollingUpgradeMode: this.RollingUpgradeMode,
            upgradeReplicaSetCheckTimeoutInSeconds: this.UpgradeReplicaSetCheckTimeoutInSeconds,
            forceRestart: this.ForceRestart,
            sortOrder: this.SortOrder,
            monitoringPolicy: monitoringPolicyDescription,
            clusterHealthPolicy: clusterHealthPolicy,
            enableDeltaHealthEvaluation: this.EnableDeltaHealthEvaluation,
            clusterUpgradeHealthPolicy: clusterUpgradeHealthPolicyObject,
            applicationHealthPolicyMap: applicationHealthPolicies,
            instanceCloseDelayDurationInSeconds: this.InstanceCloseDelayDurationInSeconds);

            var result = this.ServiceFabricClient.Cluster.ValidateClusterUpgradeAsync(
                startClusterUpgradeDescription: startClusterUpgradeDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            if (result != null)
            {
                this.WriteObject(this.FormatOutput(result));
            }
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            return output;
        }
    }
}
