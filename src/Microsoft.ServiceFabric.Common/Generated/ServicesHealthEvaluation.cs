// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for services of a certain service type belonging to an application, containing health
    /// evaluations for each unhealthy service that impacted current aggregated health state. Can be returned when
    /// evaluating application health and the aggregated health state is either Error or Warning.
    /// </summary>
    public partial class ServicesHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the ServicesHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="serviceTypeName">Name of the service type of the services.</param>
        /// <param name="maxPercentUnhealthyServices">Maximum allowed percentage of unhealthy services from the
        /// ServiceTypeHealthPolicy.</param>
        /// <param name="totalCount">Total number of services of the current service type in the application from the health
        /// store.</param>
        /// <param name="unhealthyEvaluations">List of unhealthy evaluations that led to the aggregated health state. Includes
        /// all the unhealthy ServiceHealthEvaluation that impacted the aggregated health.</param>
        public ServicesHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            string serviceTypeName = default(string),
            int? maxPercentUnhealthyServices = default(int?),
            long? totalCount = default(long?),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.Services,
                aggregatedHealthState,
                description)
        {
            this.ServiceTypeName = serviceTypeName;
            this.MaxPercentUnhealthyServices = maxPercentUnhealthyServices;
            this.TotalCount = totalCount;
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets name of the service type of the services.
        /// </summary>
        public string ServiceTypeName { get; }

        /// <summary>
        /// Gets maximum allowed percentage of unhealthy services from the ServiceTypeHealthPolicy.
        /// </summary>
        public int? MaxPercentUnhealthyServices { get; }

        /// <summary>
        /// Gets total number of services of the current service type in the application from the health store.
        /// </summary>
        public long? TotalCount { get; }

        /// <summary>
        /// Gets list of unhealthy evaluations that led to the aggregated health state. Includes all the unhealthy
        /// ServiceHealthEvaluation that impacted the aggregated health.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
