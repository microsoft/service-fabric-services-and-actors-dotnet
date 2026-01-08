// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state of the stateful service replica, which contains the replica ID and the aggregated
    /// health state.
    /// </summary>
    public partial class StatefulServiceReplicaHealthState : ReplicaHealthState
    {
        /// <summary>
        /// Initializes a new instance of the StatefulServiceReplicaHealthState class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="partitionId">The ID of the partition to which this replica belongs.</param>
        /// <param name="replicaId">Id of a stateful service replica. ReplicaId is used by Service Fabric to uniquely identify
        /// a replica of a partition. It is unique within a partition and does not change for the lifetime of the replica. If a
        /// replica gets dropped and another replica gets created on the same node for the same partition, it will get a
        /// different value for the id. Sometimes the id of a stateless service instance is also referred as a replica
        /// id.</param>
        public StatefulServiceReplicaHealthState(
            HealthState? aggregatedHealthState = default(HealthState?),
            PartitionId partitionId = default(PartitionId),
            ReplicaId replicaId = default(ReplicaId))
            : base(
                Common.ServiceKind.Stateful,
                aggregatedHealthState,
                partitionId)
        {
            this.ReplicaId = replicaId;
        }

        /// <summary>
        /// Gets id of a stateful service replica. ReplicaId is used by Service Fabric to uniquely identify a replica of a
        /// partition. It is unique within a partition and does not change for the lifetime of the replica. If a replica gets
        /// dropped and another replica gets created on the same node for the same partition, it will get a different value for
        /// the id. Sometimes the id of a stateless service instance is also referred as a replica id.
        /// </summary>
        public ReplicaId ReplicaId { get; }
    }
}
