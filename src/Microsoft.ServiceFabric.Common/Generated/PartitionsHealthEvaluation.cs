// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for the partitions of a service, containing health evaluations for each unhealthy
    /// partition that impacts current aggregated health state. Can be returned when evaluating service health and the
    /// aggregated health state is either Error or Warning.
    /// </summary>
    public partial class PartitionsHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the PartitionsHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="maxPercentUnhealthyPartitionsPerService">Maximum allowed percentage of unhealthy partitions per
        /// service from the ServiceTypeHealthPolicy.</param>
        /// <param name="totalCount">Total number of partitions of the service from the health store.</param>
        /// <param name="unhealthyEvaluations">List of unhealthy evaluations that led to the aggregated health state. Includes
        /// all the unhealthy PartitionHealthEvaluation that impacted the aggregated health.</param>
        public PartitionsHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            int? maxPercentUnhealthyPartitionsPerService = default(int?),
            long? totalCount = default(long?),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.Partitions,
                aggregatedHealthState,
                description)
        {
            this.MaxPercentUnhealthyPartitionsPerService = maxPercentUnhealthyPartitionsPerService;
            this.TotalCount = totalCount;
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets maximum allowed percentage of unhealthy partitions per service from the ServiceTypeHealthPolicy.
        /// </summary>
        public int? MaxPercentUnhealthyPartitionsPerService { get; }

        /// <summary>
        /// Gets total number of partitions of the service from the health store.
        /// </summary>
        public long? TotalCount { get; }

        /// <summary>
        /// Gets list of unhealthy evaluations that led to the aggregated health state. Includes all the unhealthy
        /// PartitionHealthEvaluation that impacted the aggregated health.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
