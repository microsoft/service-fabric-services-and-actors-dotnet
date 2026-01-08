// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for a partition, containing information about the data and the algorithm used by
    /// health store to evaluate health. The evaluation is returned only when the aggregated health state is either Error
    /// or Warning.
    /// </summary>
    public partial class PartitionHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the PartitionHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="partitionId">Id of the partition whose health evaluation is described by this object.</param>
        /// <param name="unhealthyEvaluations">List of unhealthy evaluations that led to the current aggregated health state of
        /// the partition. The types of the unhealthy evaluations can be ReplicasHealthEvaluation or
        /// EventHealthEvaluation.</param>
        public PartitionHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            PartitionId partitionId = default(PartitionId),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.Partition,
                aggregatedHealthState,
                description)
        {
            this.PartitionId = partitionId;
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets id of the partition whose health evaluation is described by this object.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets list of unhealthy evaluations that led to the current aggregated health state of the partition. The types of
        /// the unhealthy evaluations can be ReplicasHealthEvaluation or EventHealthEvaluation.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
