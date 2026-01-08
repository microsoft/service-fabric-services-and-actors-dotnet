// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for a replica, containing information about the data and the algorithm used by health
    /// store to evaluate health. The evaluation is returned only when the aggregated health state is either Error or
    /// Warning.
    /// </summary>
    public partial class ReplicaHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the ReplicaHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="partitionId">Id of the partition to which the replica belongs.</param>
        /// <param name="replicaOrInstanceId">Id of a stateful service replica or a stateless service instance. This ID is used
        /// in the queries that apply to both stateful and stateless services. It is used by Service Fabric to uniquely
        /// identify a replica of a partition of a stateful service or an instance of a stateless service partition. It is
        /// unique within a partition and does not change for the lifetime of the replica or the instance. If a stateful
        /// replica gets dropped and another replica gets created on the same node for the same partition, it will get a
        /// different value for the ID. If a stateless instance is failed over on the same or different node it will get a
        /// different value for the ID.</param>
        /// <param name="unhealthyEvaluations">List of unhealthy evaluations that led to the current aggregated health state of
        /// the replica. The types of the unhealthy evaluations can be EventHealthEvaluation.</param>
        public ReplicaHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            PartitionId partitionId = default(PartitionId),
            string replicaOrInstanceId = default(string),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.Replica,
                aggregatedHealthState,
                description)
        {
            this.PartitionId = partitionId;
            this.ReplicaOrInstanceId = replicaOrInstanceId;
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets id of the partition to which the replica belongs.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets id of a stateful service replica or a stateless service instance. This ID is used in the queries that apply to
        /// both stateful and stateless services. It is used by Service Fabric to uniquely identify a replica of a partition of
        /// a stateful service or an instance of a stateless service partition. It is unique within a partition and does not
        /// change for the lifetime of the replica or the instance. If a stateful replica gets dropped and another replica gets
        /// created on the same node for the same partition, it will get a different value for the ID. If a stateless instance
        /// is failed over on the same or different node it will get a different value for the ID.
        /// </summary>
        public string ReplicaOrInstanceId { get; }

        /// <summary>
        /// Gets list of unhealthy evaluations that led to the current aggregated health state of the replica. The types of the
        /// unhealthy evaluations can be EventHealthEvaluation.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
