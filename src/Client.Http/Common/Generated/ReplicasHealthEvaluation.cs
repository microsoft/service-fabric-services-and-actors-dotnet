// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for replicas, containing health evaluations for each unhealthy replica that impacted
    /// current aggregated health state. Can be returned when evaluating partition health and the aggregated health state
    /// is either Error or Warning.
    /// </summary>
    public partial class ReplicasHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the ReplicasHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="maxPercentUnhealthyReplicasPerPartition">Maximum allowed percentage of unhealthy replicas per
        /// partition from the ApplicationHealthPolicy.</param>
        /// <param name="totalCount">Total number of replicas in the partition from the health store.</param>
        /// <param name="unhealthyEvaluations">List of unhealthy evaluations that led to the aggregated health state. Includes
        /// all the unhealthy ReplicaHealthEvaluation that impacted the aggregated health.</param>
        public ReplicasHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            int? maxPercentUnhealthyReplicasPerPartition = default(int?),
            long? totalCount = default(long?),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.Replicas,
                aggregatedHealthState,
                description)
        {
            this.MaxPercentUnhealthyReplicasPerPartition = maxPercentUnhealthyReplicasPerPartition;
            this.TotalCount = totalCount;
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets maximum allowed percentage of unhealthy replicas per partition from the ApplicationHealthPolicy.
        /// </summary>
        public int? MaxPercentUnhealthyReplicasPerPartition { get; }

        /// <summary>
        /// Gets total number of replicas in the partition from the health store.
        /// </summary>
        public long? TotalCount { get; }

        /// <summary>
        /// Gets list of unhealthy evaluations that led to the aggregated health state. Includes all the unhealthy
        /// ReplicaHealthEvaluation that impacted the aggregated health.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
