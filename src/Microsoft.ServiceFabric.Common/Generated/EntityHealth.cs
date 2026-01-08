// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Health information common to all entities in the cluster. It contains the aggregated health state, health events
    /// and unhealthy evaluation.
    /// </summary>
    public partial class EntityHealth
    {
        /// <summary>
        /// Initializes a new instance of the EntityHealth class.
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
        public EntityHealth(
            HealthState? aggregatedHealthState = default(HealthState?),
            IEnumerable<HealthEvent> healthEvents = default(IEnumerable<HealthEvent>),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>),
            HealthStatistics healthStatistics = default(HealthStatistics))
        {
            this.AggregatedHealthState = aggregatedHealthState;
            this.HealthEvents = healthEvents;
            this.UnhealthyEvaluations = unhealthyEvaluations;
            this.HealthStatistics = healthStatistics;
        }

        /// <summary>
        /// Gets the HealthState representing the aggregated health state of the entity computed by Health Manager.
        /// The health evaluation of the entity reflects all events reported on the entity and its children (if any).
        /// The aggregation is done by applying the desired health policy.
        /// . Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'
        /// 
        /// The health state of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica etc.
        /// </summary>
        public HealthState? AggregatedHealthState { get; }

        /// <summary>
        /// Gets the list of health events reported on the entity.
        /// </summary>
        public IEnumerable<HealthEvent> HealthEvents { get; }

        /// <summary>
        /// Gets the unhealthy evaluations that show why the current aggregated health state was returned by Health Manager.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }

        /// <summary>
        /// Gets shows the health statistics for all children types of the queried entity.
        /// </summary>
        public HealthStatistics HealthStatistics { get; }
    }
}
