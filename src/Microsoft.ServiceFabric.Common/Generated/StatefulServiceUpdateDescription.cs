// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes an update for a stateful service.
    /// </summary>
    public partial class StatefulServiceUpdateDescription : ServiceUpdateDescription
    {
        /// <summary>
        /// Initializes a new instance of the StatefulServiceUpdateDescription class.
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
        /// <param name="targetReplicaSetSize">The target replica set size as a number.</param>
        /// <param name="minReplicaSetSize">The minimum replica set size as a number.</param>
        /// <param name="replicaRestartWaitDurationSeconds">The duration, in seconds, between when a replica goes down and when
        /// a new replica is created.</param>
        /// <param name="quorumLossWaitDurationSeconds">The maximum duration, in seconds, for which a partition is allowed to
        /// be in a state of quorum loss.</param>
        /// <param name="standByReplicaKeepDurationSeconds">The definition on how long StandBy replicas should be maintained
        /// before being removed.</param>
        /// <param name="servicePlacementTimeLimitSeconds">The duration for which replicas can stay InBuild before reporting
        /// that build is stuck.</param>
        /// <param name="dropSourceReplicaOnMove">Indicates whether to drop source Secondary replica even if the target replica
        /// has not finished build. If desired behavior is to drop it as soon as possible the value of this property is true,
        /// if not it is false.</param>
        /// <param name="replicaLifecycleDescription">Defines how replicas of this service will behave during ther
        /// lifecycle.</param>
        /// <param name="auxiliaryReplicaCount">The auxiliary replica count as a number. To use Auxiliary replicas, the
        /// following must be true: AuxiliaryReplicaCount &lt; (TargetReplicaSetSize+1)/2 and TargetReplicaSetSize >=3.</param>
        /// <param name="serviceSensitivityDescription">Defines default levels of replica sensitivity of this service.</param>
        public StatefulServiceUpdateDescription(
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
            int? targetReplicaSetSize = default(int?),
            int? minReplicaSetSize = default(int?),
            string replicaRestartWaitDurationSeconds = default(string),
            string quorumLossWaitDurationSeconds = default(string),
            string standByReplicaKeepDurationSeconds = default(string),
            string servicePlacementTimeLimitSeconds = default(string),
            bool? dropSourceReplicaOnMove = default(bool?),
            ReplicaLifecycleDescription replicaLifecycleDescription = default(ReplicaLifecycleDescription),
            int? auxiliaryReplicaCount = default(int?),
            ServiceSensitivityDescription serviceSensitivityDescription = default(ServiceSensitivityDescription))
            : base(
                Common.ServiceKind.Stateful,
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
            targetReplicaSetSize?.ThrowIfLessThan("targetReplicaSetSize", 1);
            minReplicaSetSize?.ThrowIfLessThan("minReplicaSetSize", 1);
            auxiliaryReplicaCount?.ThrowIfLessThan("auxiliaryReplicaCount", 0);
            this.TargetReplicaSetSize = targetReplicaSetSize;
            this.MinReplicaSetSize = minReplicaSetSize;
            this.ReplicaRestartWaitDurationSeconds = replicaRestartWaitDurationSeconds;
            this.QuorumLossWaitDurationSeconds = quorumLossWaitDurationSeconds;
            this.StandByReplicaKeepDurationSeconds = standByReplicaKeepDurationSeconds;
            this.ServicePlacementTimeLimitSeconds = servicePlacementTimeLimitSeconds;
            this.DropSourceReplicaOnMove = dropSourceReplicaOnMove;
            this.ReplicaLifecycleDescription = replicaLifecycleDescription;
            this.AuxiliaryReplicaCount = auxiliaryReplicaCount;
            this.ServiceSensitivityDescription = serviceSensitivityDescription;
        }

        /// <summary>
        /// Gets the target replica set size as a number.
        /// </summary>
        public int? TargetReplicaSetSize { get; }

        /// <summary>
        /// Gets the minimum replica set size as a number.
        /// </summary>
        public int? MinReplicaSetSize { get; }

        /// <summary>
        /// Gets the duration, in seconds, between when a replica goes down and when a new replica is created.
        /// </summary>
        public string ReplicaRestartWaitDurationSeconds { get; }

        /// <summary>
        /// Gets the maximum duration, in seconds, for which a partition is allowed to be in a state of quorum loss.
        /// </summary>
        public string QuorumLossWaitDurationSeconds { get; }

        /// <summary>
        /// Gets the definition on how long StandBy replicas should be maintained before being removed.
        /// </summary>
        public string StandByReplicaKeepDurationSeconds { get; }

        /// <summary>
        /// Gets the duration for which replicas can stay InBuild before reporting that build is stuck.
        /// </summary>
        public string ServicePlacementTimeLimitSeconds { get; }

        /// <summary>
        /// Gets indicates whether to drop source Secondary replica even if the target replica has not finished build. If
        /// desired behavior is to drop it as soon as possible the value of this property is true, if not it is false.
        /// </summary>
        public bool? DropSourceReplicaOnMove { get; }

        /// <summary>
        /// Gets defines how replicas of this service will behave during ther lifecycle.
        /// </summary>
        public ReplicaLifecycleDescription ReplicaLifecycleDescription { get; }

        /// <summary>
        /// Gets the auxiliary replica count as a number. To use Auxiliary replicas, the following must be true:
        /// AuxiliaryReplicaCount &amp;lt; (TargetReplicaSetSize+1)/2 and TargetReplicaSetSize &amp;gt;=3.
        /// </summary>
        public int? AuxiliaryReplicaCount { get; }

        /// <summary>
        /// Gets defines default levels of replica sensitivity of this service.
        /// </summary>
        public ServiceSensitivityDescription ServiceSensitivityDescription { get; }
    }
}
