// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the health of an application deployed on a Service Fabric node.
    /// </summary>
    public partial class DeployedApplicationHealth : EntityHealth
    {
        /// <summary>
        /// Initializes a new instance of the DeployedApplicationHealth class.
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
        /// <param name="name">Name of the application deployed on the node whose health information is described by this
        /// object.</param>
        /// <param name="nodeName">Name of the node where this application is deployed.</param>
        /// <param name="deployedServicePackageHealthStates">Deployed service package health states for the current deployed
        /// application as found in the health store.</param>
        public DeployedApplicationHealth(
            HealthState? aggregatedHealthState = default(HealthState?),
            IEnumerable<HealthEvent> healthEvents = default(IEnumerable<HealthEvent>),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>),
            HealthStatistics healthStatistics = default(HealthStatistics),
            ApplicationName name = default(ApplicationName),
            NodeName nodeName = default(NodeName),
            IEnumerable<DeployedServicePackageHealthState> deployedServicePackageHealthStates = default(IEnumerable<DeployedServicePackageHealthState>))
            : base(
                aggregatedHealthState,
                healthEvents,
                unhealthyEvaluations,
                healthStatistics)
        {
            this.Name = name;
            this.NodeName = nodeName;
            this.DeployedServicePackageHealthStates = deployedServicePackageHealthStates;
        }

        /// <summary>
        /// Gets name of the application deployed on the node whose health information is described by this object.
        /// </summary>
        public ApplicationName Name { get; }

        /// <summary>
        /// Gets name of the node where this application is deployed.
        /// </summary>
        public NodeName NodeName { get; }

        /// <summary>
        /// Gets deployed service package health states for the current deployed application as found in the health store.
        /// </summary>
        public IEnumerable<DeployedServicePackageHealthState> DeployedServicePackageHealthStates { get; }
    }
}
