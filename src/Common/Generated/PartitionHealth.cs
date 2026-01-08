// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the health of a Service Fabric partition.
    /// </summary>
    public partial class PartitionHealth : EntityHealth
    {
        /// <summary>
        /// Initializes a new instance of the PartitionHealth class.
        /// </summary>
        /// <param name="aggregatedHealthState">The HealthState representing the aggregated health state of the entity computed
        /// by Health Manager.
        /// The health evaluation of the entity reflects all events reported on the entity and its children (if any).
        /// The aggregation is done by applying the desired health policy.
        /// . Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'
        /// 
        /// The health state of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica etc.
        /// </param>
        /// <param name="healthEvents">The list of health events reported on the entity.</param>
        /// <param name="unhealthyEvaluations">The unhealthy evaluations that show why the current aggregated health state was
        /// returned by Health Manager.</param>
        /// <param name="healthStatistics">Shows the health statistics for all children types of the queried entity.</param>
        /// <param name="partitionId">ID of the partition whose health information is described by this object.</param>
        /// <param name="replicaHealthStates">The list of replica health states associated with the partition.</param>
        public PartitionHealth(
            HealthState? aggregatedHealthState = default(HealthState?),
            IEnumerable<HealthEvent> healthEvents = default(IEnumerable<HealthEvent>),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>),
            HealthStatistics healthStatistics = default(HealthStatistics),
            PartitionId partitionId = default(PartitionId),
            IEnumerable<ReplicaHealthState> replicaHealthStates = default(IEnumerable<ReplicaHealthState>))
            : base(
                aggregatedHealthState,
                healthEvents,
                unhealthyEvaluations,
                healthStatistics)
        {
            this.PartitionId = partitionId;
            this.ReplicaHealthStates = replicaHealthStates;
        }

        /// <summary>
        /// Gets ID of the partition whose health information is described by this object.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets the list of replica health states associated with the partition.
        /// </summary>
        public IEnumerable<ReplicaHealthState> ReplicaHealthStates { get; }
    }
}
