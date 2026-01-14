// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for applications, containing health evaluations for each unhealthy application that
    /// impacted current aggregated health state.
    /// </summary>
    public partial class ApplicationsHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationsHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="maxPercentUnhealthyApplications">Maximum allowed percentage of unhealthy applications from the
        /// ClusterHealthPolicy.</param>
        /// <param name="totalCount">Total number of applications from the health store.</param>
        /// <param name="unhealthyEvaluations">List of unhealthy evaluations that led to the aggregated health state. Includes
        /// all the unhealthy ApplicationHealthEvaluation that impacted the aggregated health.</param>
        public ApplicationsHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            int? maxPercentUnhealthyApplications = default(int?),
            long? totalCount = default(long?),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.Applications,
                aggregatedHealthState,
                description)
        {
            this.MaxPercentUnhealthyApplications = maxPercentUnhealthyApplications;
            this.TotalCount = totalCount;
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets maximum allowed percentage of unhealthy applications from the ClusterHealthPolicy.
        /// </summary>
        public int? MaxPercentUnhealthyApplications { get; }

        /// <summary>
        /// Gets total number of applications from the health store.
        /// </summary>
        public long? TotalCount { get; }

        /// <summary>
        /// Gets list of unhealthy evaluations that led to the aggregated health state. Includes all the unhealthy
        /// ApplicationHealthEvaluation that impacted the aggregated health.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
