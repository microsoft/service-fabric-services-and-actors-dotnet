// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A ServiceUpdateDescription contains all of the information necessary to update a service.
    /// </summary>
    public abstract partial class ServiceUpdateDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServiceUpdateDescription class.
        /// </summary>
        /// <param name="serviceKind">The kind of service (Stateless or Stateful).</param>
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
        protected ServiceUpdateDescription(
            ServiceKind? serviceKind,
            string flags = default(string),
            string placementConstraints = default(string),
            IEnumerable<ServiceCorrelationDescription> correlationScheme = default(IEnumerable<ServiceCorrelationDescription>),
            IEnumerable<ServiceLoadMetricDescription> loadMetrics = default(IEnumerable<ServiceLoadMetricDescription>),
            IEnumerable<ServicePlacementPolicyDescription> servicePlacementPolicies = default(IEnumerable<ServicePlacementPolicyDescription>),
            MoveCost? defaultMoveCost = default(MoveCost?),
            IEnumerable<ScalingPolicyDescription> scalingPolicies = default(IEnumerable<ScalingPolicyDescription>),
            string serviceDnsName = default(string),
            NodeTagsDescription tagsForPlacement = default(NodeTagsDescription),
            NodeTagsDescription tagsForRunning = default(NodeTagsDescription))
        {
            serviceKind.ThrowIfNull(nameof(serviceKind));
            this.ServiceKind = serviceKind;
            this.Flags = flags;
            this.PlacementConstraints = placementConstraints;
            this.CorrelationScheme = correlationScheme;
            this.LoadMetrics = loadMetrics;
            this.ServicePlacementPolicies = servicePlacementPolicies;
            this.DefaultMoveCost = defaultMoveCost;
            this.ScalingPolicies = scalingPolicies;
            this.ServiceDnsName = serviceDnsName;
            this.TagsForPlacement = tagsForPlacement;
            this.TagsForRunning = tagsForRunning;
        }

        /// <summary>
        /// Gets flags indicating whether other properties are set. Each of the associated properties corresponds to a flag,
        /// specified below, which, if set, indicate that the property is specified.
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
        /// </summary>
        public string Flags { get; }

        /// <summary>
        /// Gets the placement constraints as a string. Placement constraints are boolean expressions on node properties and
        /// allow for restricting a service to particular nodes based on the service requirements. For example, to place a
        /// service on nodes where NodeType is blue specify the following: "NodeColor == blue)".
        /// </summary>
        public string PlacementConstraints { get; }

        /// <summary>
        /// Gets the correlation scheme.
        /// </summary>
        public IEnumerable<ServiceCorrelationDescription> CorrelationScheme { get; }

        /// <summary>
        /// Gets the service load metrics.
        /// </summary>
        public IEnumerable<ServiceLoadMetricDescription> LoadMetrics { get; }

        /// <summary>
        /// Gets the service placement policies.
        /// </summary>
        public IEnumerable<ServicePlacementPolicyDescription> ServicePlacementPolicies { get; }

        /// <summary>
        /// Gets the move cost for the service. Possible values include: 'Zero', 'Low', 'Medium', 'High', 'VeryHigh'
        /// 
        /// Specifies the move cost for the service.
        /// </summary>
        public MoveCost? DefaultMoveCost { get; }

        /// <summary>
        /// Gets scaling policies for this service.
        /// </summary>
        public IEnumerable<ScalingPolicyDescription> ScalingPolicies { get; }

        /// <summary>
        /// Gets the DNS name of the service.
        /// </summary>
        public string ServiceDnsName { get; }

        /// <summary>
        /// Gets tags for placement of this service.
        /// </summary>
        public NodeTagsDescription TagsForPlacement { get; }

        /// <summary>
        /// Gets tags for running of this service.
        /// </summary>
        public NodeTagsDescription TagsForRunning { get; }

        /// <summary>
        /// Gets the kind of service (Stateless or Stateful).
        /// </summary>
        public ServiceKind? ServiceKind { get; }
    }
}
