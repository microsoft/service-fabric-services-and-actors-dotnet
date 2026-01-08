// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for deployed applications in an upgrade domain, containing health evaluations for each
    /// unhealthy deployed application that impacted current aggregated health state. Can be returned when evaluating
    /// cluster health during cluster upgrade and the aggregated health state is either Error or Warning.
    /// </summary>
    public partial class UpgradeDomainDeployedApplicationsHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the UpgradeDomainDeployedApplicationsHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="upgradeDomainName">Name of the upgrade domain where deployed applications health is currently
        /// evaluated.</param>
        /// <param name="maxPercentUnhealthyDeployedApplications">Maximum allowed percentage of unhealthy deployed applications
        /// from the ClusterHealthPolicy.</param>
        /// <param name="totalCount">Total number of deployed applications in the current upgrade domain.</param>
        /// <param name="unhealthyEvaluations">List of unhealthy evaluations that led to the aggregated health state. Includes
        /// all the unhealthy DeployedApplicationHealthEvaluation that impacted the aggregated health.</param>
        public UpgradeDomainDeployedApplicationsHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            string upgradeDomainName = default(string),
            int? maxPercentUnhealthyDeployedApplications = default(int?),
            long? totalCount = default(long?),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.UpgradeDomainDeployedApplications,
                aggregatedHealthState,
                description)
        {
            this.UpgradeDomainName = upgradeDomainName;
            this.MaxPercentUnhealthyDeployedApplications = maxPercentUnhealthyDeployedApplications;
            this.TotalCount = totalCount;
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets name of the upgrade domain where deployed applications health is currently evaluated.
        /// </summary>
        public string UpgradeDomainName { get; }

        /// <summary>
        /// Gets maximum allowed percentage of unhealthy deployed applications from the ClusterHealthPolicy.
        /// </summary>
        public int? MaxPercentUnhealthyDeployedApplications { get; }

        /// <summary>
        /// Gets total number of deployed applications in the current upgrade domain.
        /// </summary>
        public long? TotalCount { get; }

        /// <summary>
        /// Gets list of unhealthy evaluations that led to the aggregated health state. Includes all the unhealthy
        /// DeployedApplicationHealthEvaluation that impacted the aggregated health.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
