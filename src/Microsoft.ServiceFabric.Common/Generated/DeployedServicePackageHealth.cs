// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the health of a service package for a specific application deployed on a Service Fabric node.
    /// </summary>
    public partial class DeployedServicePackageHealth : EntityHealth
    {
        /// <summary>
        /// Initializes a new instance of the DeployedServicePackageHealth class.
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
        /// <param name="applicationName">The name of the application, including the 'fabric:' URI scheme.</param>
        /// <param name="serviceManifestName">Name of the service manifest.</param>
        /// <param name="nodeName">Name of the node where this service package is deployed.</param>
        public DeployedServicePackageHealth(
            HealthState? aggregatedHealthState = default(HealthState?),
            IEnumerable<HealthEvent> healthEvents = default(IEnumerable<HealthEvent>),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>),
            HealthStatistics healthStatistics = default(HealthStatistics),
            ApplicationName applicationName = default(ApplicationName),
            string serviceManifestName = default(string),
            NodeName nodeName = default(NodeName))
            : base(
                aggregatedHealthState,
                healthEvents,
                unhealthyEvaluations,
                healthStatistics)
        {
            this.ApplicationName = applicationName;
            this.ServiceManifestName = serviceManifestName;
            this.NodeName = nodeName;
        }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName ApplicationName { get; }

        /// <summary>
        /// Gets name of the service manifest.
        /// </summary>
        public string ServiceManifestName { get; }

        /// <summary>
        /// Gets name of the node where this service package is deployed.
        /// </summary>
        public NodeName NodeName { get; }
    }
}
