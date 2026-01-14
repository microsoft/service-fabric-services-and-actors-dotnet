// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents load information for a partition, which contains the metrics load information about primary, all
    /// secondary replicas/instances or a specific secondary replica/instance on a specific node , all auxiliary replicas
    /// or a specific auxiliary replica on a specific node.
    /// </summary>
    public partial class PartitionMetricLoadDescription
    {
        /// <summary>
        /// Initializes a new instance of the PartitionMetricLoadDescription class.
        /// </summary>
        /// <param name="partitionId">Id of the partition.</param>
        /// <param name="primaryReplicaLoadEntries">Partition's load information for primary replica, in case partition is from
        /// a stateful service.
        /// </param>
        /// <param name="secondaryReplicasOrInstancesLoadEntries">Partition's load information for all secondary replicas or
        /// instances.
        /// </param>
        /// <param name="secondaryReplicaOrInstanceLoadEntriesPerNode">Partition's load information for a specific secondary
        /// replica or instance located on a specific node.
        /// </param>
        /// <param name="auxiliaryReplicasLoadEntries">Partition's load information for all auxiliary replicas.
        /// </param>
        /// <param name="auxiliaryReplicaLoadEntriesPerNode">Partition's load information for a specific auxiliary replica
        /// located on a specific node.
        /// </param>
        public PartitionMetricLoadDescription(
            PartitionId partitionId = default(PartitionId),
            IEnumerable<MetricLoadDescription> primaryReplicaLoadEntries = default(IEnumerable<MetricLoadDescription>),
            IEnumerable<MetricLoadDescription> secondaryReplicasOrInstancesLoadEntries = default(IEnumerable<MetricLoadDescription>),
            IEnumerable<ReplicaMetricLoadDescription> secondaryReplicaOrInstanceLoadEntriesPerNode = default(IEnumerable<ReplicaMetricLoadDescription>),
            IEnumerable<MetricLoadDescription> auxiliaryReplicasLoadEntries = default(IEnumerable<MetricLoadDescription>),
            IEnumerable<ReplicaMetricLoadDescription> auxiliaryReplicaLoadEntriesPerNode = default(IEnumerable<ReplicaMetricLoadDescription>))
        {
            this.PartitionId = partitionId;
            this.PrimaryReplicaLoadEntries = primaryReplicaLoadEntries;
            this.SecondaryReplicasOrInstancesLoadEntries = secondaryReplicasOrInstancesLoadEntries;
            this.SecondaryReplicaOrInstanceLoadEntriesPerNode = secondaryReplicaOrInstanceLoadEntriesPerNode;
            this.AuxiliaryReplicasLoadEntries = auxiliaryReplicasLoadEntries;
            this.AuxiliaryReplicaLoadEntriesPerNode = auxiliaryReplicaLoadEntriesPerNode;
        }

        /// <summary>
        /// Gets id of the partition.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets partition's load information for primary replica, in case partition is from a stateful service.
        /// </summary>
        public IEnumerable<MetricLoadDescription> PrimaryReplicaLoadEntries { get; }

        /// <summary>
        /// Gets partition's load information for all secondary replicas or instances.
        /// </summary>
        public IEnumerable<MetricLoadDescription> SecondaryReplicasOrInstancesLoadEntries { get; }

        /// <summary>
        /// Gets partition's load information for a specific secondary replica or instance located on a specific node.
        /// </summary>
        public IEnumerable<ReplicaMetricLoadDescription> SecondaryReplicaOrInstanceLoadEntriesPerNode { get; }

        /// <summary>
        /// Gets partition's load information for all auxiliary replicas.
        /// </summary>
        public IEnumerable<MetricLoadDescription> AuxiliaryReplicasLoadEntries { get; }

        /// <summary>
        /// Gets partition's load information for a specific auxiliary replica located on a specific node.
        /// </summary>
        public IEnumerable<ReplicaMetricLoadDescription> AuxiliaryReplicaLoadEntriesPerNode { get; }
    }
}
