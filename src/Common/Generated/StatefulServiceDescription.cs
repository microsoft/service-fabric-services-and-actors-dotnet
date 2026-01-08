// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a stateful service.
    /// </summary>
    public partial class StatefulServiceDescription : ServiceDescription
    {
        /// <summary>
        /// Initializes a new instance of the StatefulServiceDescription class.
        /// </summary>
        /// <param name="serviceName">The full name of the service with 'fabric:' URI scheme.</param>
        /// <param name="serviceTypeName">Name of the service type as specified in the service manifest.</param>
        /// <param name="partitionDescription">The partition description as an object.</param>
        /// <param name="targetReplicaSetSize">The target replica set size as a number.</param>
        /// <param name="minReplicaSetSize">The minimum replica set size as a number.</param>
        /// <param name="hasPersistedState">A flag indicating whether this is a persistent service which stores states on the
        /// local disk. If it is then the value of this property is true, if not it is false.</param>
        /// <param name="applicationName">The name of the application, including the 'fabric:' URI scheme.</param>
        /// <param name="initializationData">The initialization data as an array of bytes. Initialization data is passed to
        /// service instances or replicas when they are created.</param>
        /// <param name="placementConstraints">The placement constraints as a string. Placement constraints are boolean
        /// expressions on node properties and allow for restricting a service to particular nodes based on the service
        /// requirements. For example, to place a service on nodes where NodeType is blue specify the following: "NodeColor ==
        /// blue)".</param>
        /// <param name="correlationScheme">The correlation scheme.</param>
        /// <param name="serviceLoadMetrics">The service load metrics.</param>
        /// <param name="servicePlacementPolicies">The service placement policies.</param>
        /// <param name="defaultMoveCost">The move cost for the service. Possible values include: 'Zero', 'Low', 'Medium',
        /// 'High', 'VeryHigh'
        /// 
        /// Specifies the move cost for the service.
        /// </param>
        /// <param name="isDefaultMoveCostSpecified">Indicates if the DefaultMoveCost property is specified.</param>
        /// <param name="servicePackageActivationMode">The activation mode of service package to be used for a service.
        /// Possible values include: 'SharedProcess', 'ExclusiveProcess'
        /// 
        /// The activation mode of service package to be used for a Service Fabric service. This is specified at the time of
        /// creating the Service.
        /// </param>
        /// <param name="serviceDnsName">The DNS name of the service. It requires the DNS system service to be enabled in
        /// Service Fabric cluster.</param>
        /// <param name="scalingPolicies">Scaling policies for this service.</param>
        /// <param name="tagsRequiredToPlace">Tags for placement of this service.</param>
        /// <param name="tagsRequiredToRun">Tags for running of this service.</param>
        /// <param name="flags">Flags indicating whether other properties are set. Each of the associated properties
        /// corresponds to a flag, specified below, which, if set, indicate that the property is specified.
        /// This property can be a combination of those flags obtained using bitwise 'OR' operator.
        /// For example, if the provided value is 6 then the flags for QuorumLossWaitDuration (2) and
        /// StandByReplicaKeepDuration(4) are set.
        /// 
        /// - None - Does not indicate any other properties are set. The value is zero.
        /// - ReplicaRestartWaitDuration - Indicates the ReplicaRestartWaitDuration property is set. The value is 1.
        /// - QuorumLossWaitDuration - Indicates the QuorumLossWaitDuration property is set. The value is 2.
        /// - StandByReplicaKeepDuration - Indicates the StandByReplicaKeepDuration property is set. The value is 4.
        /// - ServicePlacementTimeLimit - Indicates the ServicePlacementTimeLimit property is set. The value is 8.
        /// - DropSourceReplicaOnMove - Indicates the DropSourceReplicaOnMove property is set. The value is 16.
        /// </param>
        /// <param name="replicaRestartWaitDurationSeconds">The duration, in seconds, between when a replica goes down and when
        /// a new replica is created.</param>
        /// <param name="quorumLossWaitDurationSeconds">The maximum duration, in seconds, for which a partition is allowed to
        /// be in a state of quorum loss.</param>
        /// <param name="standByReplicaKeepDurationSeconds">Defines how long StandBy replicas should be maintained before being
        /// removed.</param>
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
        public StatefulServiceDescription(
            ServiceName serviceName,
            string serviceTypeName,
            PartitionSchemeDescription partitionDescription,
            int? targetReplicaSetSize,
            int? minReplicaSetSize,
            bool? hasPersistedState,
            ApplicationName applicationName = default(ApplicationName),
            byte[] initializationData = default(byte[]),
            string placementConstraints = default(string),
            IEnumerable<ServiceCorrelationDescription> correlationScheme = default(IEnumerable<ServiceCorrelationDescription>),
            IEnumerable<ServiceLoadMetricDescription> serviceLoadMetrics = default(IEnumerable<ServiceLoadMetricDescription>),
            IEnumerable<ServicePlacementPolicyDescription> servicePlacementPolicies = default(IEnumerable<ServicePlacementPolicyDescription>),
            MoveCost? defaultMoveCost = default(MoveCost?),
            bool? isDefaultMoveCostSpecified = default(bool?),
            ServicePackageActivationMode? servicePackageActivationMode = default(ServicePackageActivationMode?),
            string serviceDnsName = default(string),
            IEnumerable<ScalingPolicyDescription> scalingPolicies = default(IEnumerable<ScalingPolicyDescription>),
            NodeTagsDescription tagsRequiredToPlace = default(NodeTagsDescription),
            NodeTagsDescription tagsRequiredToRun = default(NodeTagsDescription),
            int? flags = default(int?),
            long? replicaRestartWaitDurationSeconds = default(long?),
            long? quorumLossWaitDurationSeconds = default(long?),
            long? standByReplicaKeepDurationSeconds = default(long?),
            long? servicePlacementTimeLimitSeconds = default(long?),
            bool? dropSourceReplicaOnMove = default(bool?),
            ReplicaLifecycleDescription replicaLifecycleDescription = default(ReplicaLifecycleDescription),
            int? auxiliaryReplicaCount = default(int?),
            ServiceSensitivityDescription serviceSensitivityDescription = default(ServiceSensitivityDescription))
            : base(
                serviceName,
                serviceTypeName,
                partitionDescription,
                Common.ServiceKind.Stateful,
                applicationName,
                initializationData,
                placementConstraints,
                correlationScheme,
                serviceLoadMetrics,
                servicePlacementPolicies,
                defaultMoveCost,
                isDefaultMoveCostSpecified,
                servicePackageActivationMode,
                serviceDnsName,
                scalingPolicies,
                tagsRequiredToPlace,
                tagsRequiredToRun)
        {
            targetReplicaSetSize.ThrowIfNull(nameof(targetReplicaSetSize));
            minReplicaSetSize.ThrowIfNull(nameof(minReplicaSetSize));
            hasPersistedState.ThrowIfNull(nameof(hasPersistedState));
            targetReplicaSetSize?.ThrowIfLessThan("targetReplicaSetSize", 1);
            minReplicaSetSize?.ThrowIfLessThan("minReplicaSetSize", 1);
            replicaRestartWaitDurationSeconds?.ThrowIfOutOfInclusiveRange("replicaRestartWaitDurationSeconds", 0, 4294967295);
            quorumLossWaitDurationSeconds?.ThrowIfOutOfInclusiveRange("quorumLossWaitDurationSeconds", 0, 4294967295);
            standByReplicaKeepDurationSeconds?.ThrowIfOutOfInclusiveRange("standByReplicaKeepDurationSeconds", 0, 4294967295);
            servicePlacementTimeLimitSeconds?.ThrowIfOutOfInclusiveRange("servicePlacementTimeLimitSeconds", 0, 4294967295);
            auxiliaryReplicaCount?.ThrowIfLessThan("auxiliaryReplicaCount", 0);
            this.TargetReplicaSetSize = targetReplicaSetSize;
            this.MinReplicaSetSize = minReplicaSetSize;
            this.HasPersistedState = hasPersistedState;
            this.Flags = flags;
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
        /// Gets a flag indicating whether this is a persistent service which stores states on the local disk. If it is then
        /// the value of this property is true, if not it is false.
        /// </summary>
        public bool? HasPersistedState { get; }

        /// <summary>
        /// Gets flags indicating whether other properties are set. Each of the associated properties corresponds to a flag,
        /// specified below, which, if set, indicate that the property is specified.
        /// This property can be a combination of those flags obtained using bitwise 'OR' operator.
        /// For example, if the provided value is 6 then the flags for QuorumLossWaitDuration (2) and
        /// StandByReplicaKeepDuration(4) are set.
        /// 
        /// - None - Does not indicate any other properties are set. The value is zero.
        /// - ReplicaRestartWaitDuration - Indicates the ReplicaRestartWaitDuration property is set. The value is 1.
        /// - QuorumLossWaitDuration - Indicates the QuorumLossWaitDuration property is set. The value is 2.
        /// - StandByReplicaKeepDuration - Indicates the StandByReplicaKeepDuration property is set. The value is 4.
        /// - ServicePlacementTimeLimit - Indicates the ServicePlacementTimeLimit property is set. The value is 8.
        /// - DropSourceReplicaOnMove - Indicates the DropSourceReplicaOnMove property is set. The value is 16.
        /// </summary>
        public int? Flags { get; }

        /// <summary>
        /// Gets the duration, in seconds, between when a replica goes down and when a new replica is created.
        /// </summary>
        public long? ReplicaRestartWaitDurationSeconds { get; }

        /// <summary>
        /// Gets the maximum duration, in seconds, for which a partition is allowed to be in a state of quorum loss.
        /// </summary>
        public long? QuorumLossWaitDurationSeconds { get; }

        /// <summary>
        /// Gets defines how long StandBy replicas should be maintained before being removed.
        /// </summary>
        public long? StandByReplicaKeepDurationSeconds { get; }

        /// <summary>
        /// Gets the duration for which replicas can stay InBuild before reporting that build is stuck.
        /// </summary>
        public long? ServicePlacementTimeLimitSeconds { get; }

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
