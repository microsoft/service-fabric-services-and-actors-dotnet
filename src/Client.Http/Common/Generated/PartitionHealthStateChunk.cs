// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state chunk of a partition, which contains the partition ID, its aggregated health state and
    /// any replicas that respect the filters in the cluster health chunk query description.
    /// </summary>
    public partial class PartitionHealthStateChunk : EntityHealthStateChunk
    {
        /// <summary>
        /// Initializes a new instance of the PartitionHealthStateChunk class.
        /// </summary>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="partitionId">The Id of the partition.</param>
        /// <param name="replicaHealthStateChunks">The list of replica health state chunks belonging to the partition that
        /// respect the filters in the cluster health chunk query description.
        /// </param>
        public PartitionHealthStateChunk(
            HealthState? healthState = default(HealthState?),
            PartitionId partitionId = default(PartitionId),
            ReplicaHealthStateChunkList replicaHealthStateChunks = default(ReplicaHealthStateChunkList))
            : base(
                healthState)
        {
            this.PartitionId = partitionId;
            this.ReplicaHealthStateChunks = replicaHealthStateChunks;
        }

        /// <summary>
        /// Gets the Id of the partition.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets the list of replica health state chunks belonging to the partition that respect the filters in the cluster
        /// health chunk query description.
        /// </summary>
        public ReplicaHealthStateChunkList ReplicaHealthStateChunks { get; }
    }
}
