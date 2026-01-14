// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health of the application. Contains the application aggregated health state and the service and
    /// deployed application health states.
    /// </summary>
    public partial class ApplicationHealth : EntityHealth
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationHealth class.
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
        /// <param name="name">The name of the application, including the 'fabric:' URI scheme.</param>
        /// <param name="serviceHealthStates">Service health states as found in the health store.</param>
        /// <param name="deployedApplicationHealthStates">Deployed application health states as found in the health
        /// store.</param>
        public ApplicationHealth(
            HealthState? aggregatedHealthState = default(HealthState?),
            IEnumerable<HealthEvent> healthEvents = default(IEnumerable<HealthEvent>),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>),
            HealthStatistics healthStatistics = default(HealthStatistics),
            ApplicationName name = default(ApplicationName),
            IEnumerable<ServiceHealthState> serviceHealthStates = default(IEnumerable<ServiceHealthState>),
            IEnumerable<DeployedApplicationHealthState> deployedApplicationHealthStates = default(IEnumerable<DeployedApplicationHealthState>))
            : base(
                aggregatedHealthState,
                healthEvents,
                unhealthyEvaluations,
                healthStatistics)
        {
            this.Name = name;
            this.ServiceHealthStates = serviceHealthStates;
            this.DeployedApplicationHealthStates = deployedApplicationHealthStates;
        }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName Name { get; }

        /// <summary>
        /// Gets service health states as found in the health store.
        /// </summary>
        public IEnumerable<ServiceHealthState> ServiceHealthStates { get; }

        /// <summary>
        /// Gets deployed application health states as found in the health store.
        /// </summary>
        public IEnumerable<DeployedApplicationHealthState> DeployedApplicationHealthStates { get; }
    }
}
