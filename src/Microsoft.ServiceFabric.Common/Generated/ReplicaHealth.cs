// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a base class for stateful service replica or stateless service instance health.
    /// Contains the replica aggregated health state, the health events and the unhealthy evaluations.
    /// </summary>
    public partial class ReplicaHealth : EntityHealth
    {
        /// <summary>
        /// Initializes a new instance of the ReplicaHealth class.
        /// </summary>
        /// <param name="serviceKind">The kind of service (Stateless or Stateful).</param>
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
        public ReplicaHealth(
            ServiceKind? serviceKind,
            HealthState? aggregatedHealthState = default(HealthState?),
            IEnumerable<HealthEvent> healthEvents = default(IEnumerable<HealthEvent>),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>),
            HealthStatistics healthStatistics = default(HealthStatistics),
            PartitionId partitionId = default(PartitionId))
            : base(
                aggregatedHealthState,
                healthEvents,
                unhealthyEvaluations,
                healthStatistics)
        {
            serviceKind.ThrowIfNull(nameof(serviceKind));
            this.ServiceKind = serviceKind;
            this.PartitionId = partitionId;
        }

        /// <summary>
        /// Gets id of the partition to which this replica belongs.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets the kind of service (Stateless or Stateful).
        /// </summary>
        public ServiceKind? ServiceKind { get; }
    }
}
