// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes an update for a stateless service.
    /// </summary>
    public partial class StatelessServiceUpdateDescription : ServiceUpdateDescription
    {
        /// <summary>
        /// Initializes a new instance of the StatelessServiceUpdateDescription class.
        /// </summary>
        /// <param name="flags">Flags indicating whether other properties are set. Each of the associated properties
        /// corresponds to a flag, specified below, which, if set, indicate that the property is specified.
        /// This property can be a combination of those flags obtained using bitwise 'OR' operator.
        /// For example, if the provided value is 6 then the flags for ReplicaRestartWaitDuration (2) and
        /// QuorumLossWaitDuration (4) are set.
        /// 
        /// - None - Does not indicate any other properties are set. The value is zero.
        /// - TargetReplicaSetSize/InstanceCount - Indicates whether the TargetReplicaSetSize property (for Stateful services)
        /// or the InstanceCount property (for Stateless services) is set. The value is 1.
        /// - ReplicaRestartWaitDuration - Indicates the ReplicaRestartWaitDuration property is set. The value is  2.
        /// - QuorumLossWaitDuration - Indicates the QuorumLossWaitDuration property is set. The value is 4.
        /// - StandByReplicaKeepDuration - Indicates the StandByReplicaKeepDuration property is set. The value is 8.
        /// - MinReplicaSetSize - Indicates the MinReplicaSetSize property is set. The value is 16.
        /// - PlacementConstraints - Indicates the PlacementConstraints property is set. The value is 32.
        /// - PlacementPolicyList - Indicates the ServicePlacementPolicies property is set. The value is 64.
        /// - Correlation - Indicates the CorrelationScheme property is set. The value is 128.
        /// - Metrics - Indicates the ServiceLoadMetrics property is set. The value is 256.
        /// - DefaultMoveCost - Indicates the DefaultMoveCost property is set. The value is 512.
        /// - ScalingPolicy - Indicates the ScalingPolicies property is set. The value is 1024.
        /// - ServicePlacementTimeLimit - Indicates the ServicePlacementTimeLimit property is set. The value is 2048.
        /// - MinInstanceCount - Indicates the MinInstanceCount property is set. The value is 4096.
        /// - MinInstancePercentage - Indicates the MinInstancePercentage property is set. The value is 8192.
        /// - InstanceCloseDelayDuration - Indicates the InstanceCloseDelayDuration property is set. The value is 16384.
        /// - InstanceRestartWaitDuration - Indicates the InstanceCloseDelayDuration property is set. The value is 32768.
        /// - DropSourceReplicaOnMove - Indicates the DropSourceReplicaOnMove property is set. The value is 65536.
        /// - ServiceDnsName - Indicates the ServiceDnsName property is set. The value is 131072.
        /// - TagsForPlacement - Indicates the TagsForPlacement property is set. The value is 1048576.
        /// - TagsForRunning - Indicates the TagsForRunning property is set. The value is 2097152.
        /// </param>
        /// <param name="placementConstraints">The placement constraints as a string. Placement constraints are boolean
        /// expressions on node properties and allow for restricting a service to particular nodes based on the service
        /// requirements. For example, to place a service on nodes where NodeType is blue specify the following: "NodeColor ==
        /// blue)".</param>
        /// <param name="correlationScheme">The correlation scheme.</param>
        /// <param name="loadMetrics">The service load metrics.</param>
        /// <param name="servicePlacementPolicies">The service placement policies.</param>
        /// <param name="defaultMoveCost">The move cost for the service. Possible values include: 'Zero', 'Low', 'Medium',
        /// 'High', 'VeryHigh'
        /// 
        /// Specifies the move cost for the service.
        /// </param>
        /// <param name="scalingPolicies">Scaling policies for this service.</param>
        /// <param name="serviceDnsName">The DNS name of the service.</param>
        /// <param name="tagsForPlacement">Tags for placement of this service.</param>
        /// <param name="tagsForRunning">Tags for running of this service.</param>
        /// <param name="instanceCount">The instance count.</param>
        /// <param name="minInstanceCount">MinInstanceCount is the minimum number of instances that must be up to meet the
        /// EnsureAvailability safety check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstanceCount computation -1 is first converted into the number of
        /// nodes on which the instances are allowed to be placed according to the placement constraints on the service.
        /// </param>
        /// <param name="minInstancePercentage">MinInstancePercentage is the minimum percentage of InstanceCount that must be
        /// up to meet the EnsureAvailability safety check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstancePercentage computation, -1 is first converted into the
        /// number of nodes on which the instances are allowed to be placed according to the placement constraints on the
        /// service.
        /// </param>
        /// <param name="instanceCloseDelayDurationSeconds">Duration in seconds, to wait before a stateless instance is closed,
        /// to allow the active requests to drain gracefully. This would be effective when the instance is closing during the
        /// application/cluster upgrade and disabling node.
        /// The endpoint exposed on this instance is removed prior to starting the delay, which prevents new connections to
        /// this instance.
        /// In addition, clients that have subscribed to service endpoint change
        /// events(https://docs.microsoft.com/dotnet/api/system.fabric.fabricclient.servicemanagementclient.registerservicenotificationfilterasync),
        /// can do
        /// the following upon receiving the endpoint removal notification:
        /// - Stop sending new requests to this instance.
        /// - Close existing connections after in-flight requests have completed.
        /// - Connect to a different instance of the service partition for future requests.
        /// </param>
        /// <param name="instanceLifecycleDescription">Defines how instances of this service will behave during their
        /// lifecycle.</param>
        /// <param name="instanceRestartWaitDurationSeconds">When a stateless instance goes down, this timer starts. When it
        /// expires Service Fabric will create a new instance on any node in the cluster.
        /// This configuration is to reduce unnecessary creation of a new instance in situations where the instance going down
        /// is likely to recover in a short time. For example, during an upgrade.
        /// The default value is 0, which indicates that when stateless instance goes down, Service Fabric will immediately
        /// start building its replacement.
        /// </param>
        public StatelessServiceUpdateDescription(
            string flags = default(string),
            string placementConstraints = default(string),
            IEnumerable<ServiceCorrelationDescription> correlationScheme = default(IEnumerable<ServiceCorrelationDescription>),
            IEnumerable<ServiceLoadMetricDescription> loadMetrics = default(IEnumerable<ServiceLoadMetricDescription>),
            IEnumerable<ServicePlacementPolicyDescription> servicePlacementPolicies = default(IEnumerable<ServicePlacementPolicyDescription>),
            MoveCost? defaultMoveCost = default(MoveCost?),
            IEnumerable<ScalingPolicyDescription> scalingPolicies = default(IEnumerable<ScalingPolicyDescription>),
            string serviceDnsName = default(string),
            NodeTagsDescription tagsForPlacement = default(NodeTagsDescription),
            NodeTagsDescription tagsForRunning = default(NodeTagsDescription),
            int? instanceCount = default(int?),
            int? minInstanceCount = default(int?),
            int? minInstancePercentage = default(int?),
            string instanceCloseDelayDurationSeconds = default(string),
            InstanceLifecycleDescription instanceLifecycleDescription = default(InstanceLifecycleDescription),
            string instanceRestartWaitDurationSeconds = default(string))
            : base(
                Common.ServiceKind.Stateless,
                flags,
                placementConstraints,
                correlationScheme,
                loadMetrics,
                servicePlacementPolicies,
                defaultMoveCost,
                scalingPolicies,
                serviceDnsName,
                tagsForPlacement,
                tagsForRunning)
        {
            instanceCount?.ThrowIfLessThan("instanceCount", -1);
            this.InstanceCount = instanceCount;
            this.MinInstanceCount = minInstanceCount;
            this.MinInstancePercentage = minInstancePercentage;
            this.InstanceCloseDelayDurationSeconds = instanceCloseDelayDurationSeconds;
            this.InstanceLifecycleDescription = instanceLifecycleDescription;
            this.InstanceRestartWaitDurationSeconds = instanceRestartWaitDurationSeconds;
        }

        /// <summary>
        /// Gets the instance count.
        /// </summary>
        public int? InstanceCount { get; }

        /// <summary>
        /// Gets minInstanceCount is the minimum number of instances that must be up to meet the EnsureAvailability safety
        /// check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstanceCount computation -1 is first converted into the number of
        /// nodes on which the instances are allowed to be placed according to the placement constraints on the service.
        /// </summary>
        public int? MinInstanceCount { get; }

        /// <summary>
        /// Gets minInstancePercentage is the minimum percentage of InstanceCount that must be up to meet the
        /// EnsureAvailability safety check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstancePercentage computation, -1 is first converted into the
        /// number of nodes on which the instances are allowed to be placed according to the placement constraints on the
        /// service.
        /// </summary>
        public int? MinInstancePercentage { get; }

        /// <summary>
        /// Gets duration in seconds, to wait before a stateless instance is closed, to allow the active requests to drain
        /// gracefully. This would be effective when the instance is closing during the application/cluster upgrade and
        /// disabling node.
        /// The endpoint exposed on this instance is removed prior to starting the delay, which prevents new connections to
        /// this instance.
        /// In addition, clients that have subscribed to service endpoint change
        /// events(https://docs.microsoft.com/dotnet/api/system.fabric.fabricclient.servicemanagementclient.registerservicenotificationfilterasync),
        /// can do
        /// the following upon receiving the endpoint removal notification:
        /// - Stop sending new requests to this instance.
        /// - Close existing connections after in-flight requests have completed.
        /// - Connect to a different instance of the service partition for future requests.
        /// </summary>
        public string InstanceCloseDelayDurationSeconds { get; }

        /// <summary>
        /// Gets defines how instances of this service will behave during their lifecycle.
        /// </summary>
        public InstanceLifecycleDescription InstanceLifecycleDescription { get; }

        /// <summary>
        /// Gets when a stateless instance goes down, this timer starts. When it expires Service Fabric will create a new
        /// instance on any node in the cluster.
        /// This configuration is to reduce unnecessary creation of a new instance in situations where the instance going down
        /// is likely to recover in a short time. For example, during an upgrade.
        /// The default value is 0, which indicates that when stateless instance goes down, Service Fabric will immediately
        /// start building its replacement.
        /// </summary>
        public string InstanceRestartWaitDurationSeconds { get; }
    }
}
