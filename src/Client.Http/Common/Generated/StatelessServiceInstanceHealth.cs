// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health of the stateless service instance.
    /// Contains the instance aggregated health state, the health events and the unhealthy evaluations.
    /// </summary>
    public partial class StatelessServiceInstanceHealth : ReplicaHealth
    {
        /// <summary>
        /// Initializes a new instance of the StatelessServiceInstanceHealth class.
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
        /// <param name="partitionId">Id of the partition to which this replica belongs.</param>
        /// <param name="instanceId">Id of a stateless service instance. InstanceId is used by Service Fabric to uniquely
        /// identify an instance of a partition of a stateless service. It is unique within a partition and does not change for
        /// the lifetime of the instance. If the instance has failed over on the same or different node, it will get a
        /// different value for the InstanceId.</param>
        public StatelessServiceInstanceHealth(
            HealthState? aggregatedHealthState = default(HealthState?),
            IEnumerable<HealthEvent> healthEvents = default(IEnumerable<HealthEvent>),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>),
            HealthStatistics healthStatistics = default(HealthStatistics),
            PartitionId partitionId = default(PartitionId),
            ReplicaId instanceId = default(ReplicaId))
            : base(
                Common.ServiceKind.Stateless,
                aggregatedHealthState,
                healthEvents,
                unhealthyEvaluations,
                healthStatistics,
                partitionId)
        {
            this.InstanceId = instanceId;
        }

        /// <summary>
        /// Gets id of a stateless service instance. InstanceId is used by Service Fabric to uniquely identify an instance of a
        /// partition of a stateless service. It is unique within a partition and does not change for the lifetime of the
        /// instance. If the instance has failed over on the same or different node, it will get a different value for the
        /// InstanceId.
        /// </summary>
        public ReplicaId InstanceId { get; }
    }
}
