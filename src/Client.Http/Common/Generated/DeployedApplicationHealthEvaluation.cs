// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for a deployed application, containing information about the data and the algorithm
    /// used by the health store to evaluate health.
    /// </summary>
    public partial class DeployedApplicationHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the DeployedApplicationHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="nodeName">Name of the node where the application is deployed to.</param>
        /// <param name="applicationName">The name of the application, including the 'fabric:' URI scheme.</param>
        /// <param name="unhealthyEvaluations">List of  unhealthy evaluations that led to the current aggregated health state
        /// of the deployed application.
        /// The types of the unhealthy evaluations can be DeployedServicePackagesHealthEvaluation or EventHealthEvaluation.
        /// </param>
        public DeployedApplicationHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            NodeName nodeName = default(NodeName),
            ApplicationName applicationName = default(ApplicationName),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.DeployedApplication,
                aggregatedHealthState,
                description)
        {
            this.NodeName = nodeName;
            this.ApplicationName = applicationName;
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets name of the node where the application is deployed to.
        /// </summary>
        public NodeName NodeName { get; }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName ApplicationName { get; }

        /// <summary>
        /// Gets list of  unhealthy evaluations that led to the current aggregated health state of the deployed application.
        /// The types of the unhealthy evaluations can be DeployedServicePackagesHealthEvaluation or EventHealthEvaluation.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
