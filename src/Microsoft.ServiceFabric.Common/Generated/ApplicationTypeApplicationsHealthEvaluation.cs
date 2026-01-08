// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for applications of a particular application type. The application type applications
    /// evaluation can be returned when cluster health evaluation returns unhealthy aggregated health state, either Error
    /// or Warning. It contains health evaluations for each unhealthy application of the included application type that
    /// impacted current aggregated health state.
    /// </summary>
    public partial class ApplicationTypeApplicationsHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationTypeApplicationsHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="applicationTypeName">The application type name as defined in the application manifest.</param>
        /// <param name="maxPercentUnhealthyApplications">Maximum allowed percentage of unhealthy applications for the
        /// application type, specified as an entry in ApplicationTypeHealthPolicyMap.</param>
        /// <param name="totalCount">Total number of applications of the application type found in the health store.</param>
        /// <param name="unhealthyEvaluations">List of unhealthy evaluations that led to the aggregated health state. Includes
        /// all the unhealthy ApplicationHealthEvaluation of this application type that impacted the aggregated health.</param>
        public ApplicationTypeApplicationsHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            string applicationTypeName = default(string),
            int? maxPercentUnhealthyApplications = default(int?),
            long? totalCount = default(long?),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.ApplicationTypeApplications,
                aggregatedHealthState,
                description)
        {
            this.ApplicationTypeName = applicationTypeName;
            this.MaxPercentUnhealthyApplications = maxPercentUnhealthyApplications;
            this.TotalCount = totalCount;
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets the application type name as defined in the application manifest.
        /// </summary>
        public string ApplicationTypeName { get; }

        /// <summary>
        /// Gets maximum allowed percentage of unhealthy applications for the application type, specified as an entry in
        /// ApplicationTypeHealthPolicyMap.
        /// </summary>
        public int? MaxPercentUnhealthyApplications { get; }

        /// <summary>
        /// Gets total number of applications of the application type found in the health store.
        /// </summary>
        public long? TotalCount { get; }

        /// <summary>
        /// Gets list of unhealthy evaluations that led to the aggregated health state. Includes all the unhealthy
        /// ApplicationHealthEvaluation of this application type that impacted the aggregated health.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
