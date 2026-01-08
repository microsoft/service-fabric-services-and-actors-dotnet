// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health of the cluster.
    /// Contains the cluster aggregated health state, the cluster application and node health states as well as the health
    /// events and the unhealthy evaluations.
    /// </summary>
    public partial class ClusterHealth : EntityHealth
    {
        /// <summary>
        /// Initializes a new instance of the ClusterHealth class.
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
        /// <param name="nodeHealthStates">Cluster node health states as found in the health store.</param>
        /// <param name="applicationHealthStates">Cluster application health states as found in the health store.</param>
        public ClusterHealth(
            HealthState? aggregatedHealthState = default(HealthState?),
            IEnumerable<HealthEvent> healthEvents = default(IEnumerable<HealthEvent>),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>),
            HealthStatistics healthStatistics = default(HealthStatistics),
            IEnumerable<NodeHealthState> nodeHealthStates = default(IEnumerable<NodeHealthState>),
            IEnumerable<ApplicationHealthState> applicationHealthStates = default(IEnumerable<ApplicationHealthState>))
            : base(
                aggregatedHealthState,
                healthEvents,
                unhealthyEvaluations,
                healthStatistics)
        {
            this.NodeHealthStates = nodeHealthStates;
            this.ApplicationHealthStates = applicationHealthStates;
        }

        /// <summary>
        /// Gets cluster node health states as found in the health store.
        /// </summary>
        public IEnumerable<NodeHealthState> NodeHealthStates { get; }

        /// <summary>
        /// Gets cluster application health states as found in the health store.
        /// </summary>
        public IEnumerable<ApplicationHealthState> ApplicationHealthStates { get; }
    }
}
