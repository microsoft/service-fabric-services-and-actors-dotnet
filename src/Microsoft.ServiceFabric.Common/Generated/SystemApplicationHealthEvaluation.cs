// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for the fabric:/System application, containing information about the data and the
    /// algorithm used by health store to evaluate health. The evaluation is returned only when the aggregated health state
    /// of the cluster is either Error or Warning.
    /// </summary>
    public partial class SystemApplicationHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the SystemApplicationHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="unhealthyEvaluations">List of unhealthy evaluations that led to the current aggregated health state of
        /// the system application. The types of the unhealthy evaluations can be DeployedApplicationsHealthEvaluation,
        /// ServicesHealthEvaluation or EventHealthEvaluation.</param>
        public SystemApplicationHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.SystemApplication,
                aggregatedHealthState,
                description)
        {
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets list of unhealthy evaluations that led to the current aggregated health state of the system application. The
        /// types of the unhealthy evaluations can be DeployedApplicationsHealthEvaluation, ServicesHealthEvaluation or
        /// EventHealthEvaluation.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
