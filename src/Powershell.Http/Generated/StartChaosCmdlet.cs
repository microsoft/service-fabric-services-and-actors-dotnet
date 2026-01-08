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
    /// Starts Chaos in the cluster.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "SFChaos")]
    public partial class StartChaosCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets TimeToRunInSeconds. Total time (in seconds) for which Chaos will run before automatically stopping.
        /// The maximum allowed value is 4,294,967,295 (System.UInt32.MaxValue).
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public string TimeToRunInSeconds { get; set; } = "4294967295";

        /// <summary>
        /// Gets or sets MaxClusterStabilizationTimeoutInSeconds. The maximum amount of time to wait for all cluster entities
        /// to become stable and healthy. Chaos executes in iterations and at the start of each iteration it validates the
        /// health of cluster entities.
        /// During validation if a cluster entity is not stable and healthy within MaxClusterStabilizationTimeoutInSeconds,
        /// Chaos generates a validation failed event.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public long? MaxClusterStabilizationTimeoutInSeconds { get; set; } = 60;

        /// <summary>
        /// Gets or sets MaxConcurrentFaults. MaxConcurrentFaults is the maximum number of concurrent faults induced per
        /// iteration.
        /// Chaos executes in iterations and two consecutive iterations are separated by a validation phase.
        /// The higher the concurrency, the more aggressive the injection of faults, leading to inducing more complex series of
        /// states to uncover bugs.
        /// The recommendation is to start with a value of 2 or 3 and to exercise caution while moving up.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public long? MaxConcurrentFaults { get; set; } = 1;

        /// <summary>
        /// Gets or sets EnableMoveReplicaFaults. Enables or disables the move primary and move secondary faults.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public bool? EnableMoveReplicaFaults { get; set; } = true;

        /// <summary>
        /// Gets or sets WaitTimeBetweenFaultsInSeconds. Wait time (in seconds) between consecutive faults within a single
        /// iteration.
        /// The larger the value, the lower the overlapping between faults and the simpler the sequence of state transitions
        /// that the cluster goes through.
        /// The recommendation is to start with a value between 1 and 5 and exercise caution while moving up.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public long? WaitTimeBetweenFaultsInSeconds { get; set; } = 20;

        /// <summary>
        /// Gets or sets WaitTimeBetweenIterationsInSeconds. Time-separation (in seconds) between two consecutive iterations of
        /// Chaos.
        /// The larger the value, the lower the fault injection rate.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public long? WaitTimeBetweenIterationsInSeconds { get; set; } = 30;

        /// <summary>
        /// Gets or sets ConsiderWarningAsError. Indicates whether warnings are treated with the same severity as errors.
        /// </summary>
        [Parameter(Mandatory = false, Position = 6)]
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
        [Parameter(Mandatory = false, Position = 7)]
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
        [Parameter(Mandatory = false, Position = 8)]
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
        [Parameter(Mandatory = false, Position = 9)]
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
        [Parameter(Mandatory = false, Position = 10)]
        public IEnumerable<NodeTypeHealthPolicyMapItem> NodeTypeHealthPolicyMap { get; set; }

        /// <summary>
        /// Gets or sets Map. Describes a map that contains a collection of ChaosContextMapItem's.
        /// </summary>
        [Parameter(Mandatory = false, Position = 11)]
        public System.Collections.Hashtable Map { get; set; }

        /// <summary>
        /// Gets or sets NodeTypeInclusionList. A list of node types to include in Chaos faults.
        /// All types of faults (restart node, restart code package, remove replica, restart replica, move primary, and move
        /// secondary) are enabled for the nodes of these node types.
        /// If a node type (say NodeTypeX) does not appear in the NodeTypeInclusionList, then node level faults (like
        /// NodeRestart) will never be enabled for the nodes of
        /// NodeTypeX, but code package and replica faults can still be enabled for NodeTypeX if an application in the
        /// ApplicationInclusionList.
        /// happens to reside on a node of NodeTypeX.
        /// At most 100 node type names can be included in this list, to increase this number, a config upgrade is required for
        /// MaxNumberOfNodeTypesInChaosEntityFilter configuration.
        /// </summary>
        [Parameter(Mandatory = false, Position = 12)]
        public IEnumerable<string> NodeTypeInclusionList { get; set; }

        /// <summary>
        /// Gets or sets ApplicationInclusionList. A list of application URIs to include in Chaos faults.
        /// All replicas belonging to services of these applications are amenable to replica faults (restart replica, remove
        /// replica, move primary, and move secondary) by Chaos.
        /// Chaos may restart a code package only if the code package hosts replicas of these applications only.
        /// If an application does not appear in this list, it can still be faulted in some Chaos iteration if the application
        /// ends up on a node of a node type that is included in NodeTypeInclusionList.
        /// However, if applicationX is tied to nodeTypeY through placement constraints and applicationX is absent from
        /// ApplicationInclusionList and nodeTypeY is absent from NodeTypeInclusionList, then applicationX will never be
        /// faulted.
        /// At most 1000 application names can be included in this list, to increase this number, a config upgrade is required
        /// for MaxNumberOfApplicationsInChaosEntityFilter configuration.
        /// </summary>
        [Parameter(Mandatory = false, Position = 13)]
        public IEnumerable<string> ApplicationInclusionList { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 14)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var clusterHealthPolicy = new ClusterHealthPolicy(
            considerWarningAsError: this.ConsiderWarningAsError,
            maxPercentUnhealthyNodes: this.MaxPercentUnhealthyNodes,
            maxPercentUnhealthyApplications: this.MaxPercentUnhealthyApplications,
            applicationTypeHealthPolicyMap: this.ApplicationTypeHealthPolicyMap,
            nodeTypeHealthPolicyMap: this.NodeTypeHealthPolicyMap);

            var chaosContext = new ChaosContext(
            map: this.Map?.ToDictionary<string, string>());

            var chaosTargetFilter = new ChaosTargetFilter(
            nodeTypeInclusionList: this.NodeTypeInclusionList,
            applicationInclusionList: this.ApplicationInclusionList);

            var chaosParameters = new ChaosParameters(
            timeToRunInSeconds: this.TimeToRunInSeconds,
            maxClusterStabilizationTimeoutInSeconds: this.MaxClusterStabilizationTimeoutInSeconds,
            maxConcurrentFaults: this.MaxConcurrentFaults,
            enableMoveReplicaFaults: this.EnableMoveReplicaFaults,
            waitTimeBetweenFaultsInSeconds: this.WaitTimeBetweenFaultsInSeconds,
            waitTimeBetweenIterationsInSeconds: this.WaitTimeBetweenIterationsInSeconds,
            clusterHealthPolicy: clusterHealthPolicy,
            context: chaosContext,
            chaosTargetFilter: chaosTargetFilter);

            this.ServiceFabricClient.ChaosClient.StartChaosAsync(
                chaosParameters: chaosParameters,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
