// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state of the stateless service instance, which contains the instance ID and the aggregated
    /// health state.
    /// </summary>
    public partial class StatelessServiceInstanceHealthState : ReplicaHealthState
    {
        /// <summary>
        /// Initializes a new instance of the StatelessServiceInstanceHealthState class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="partitionId">The ID of the partition to which this replica belongs.</param>
        /// <param name="replicaId">Id of the stateless service instance on the wire this field is called ReplicaId.</param>
        public StatelessServiceInstanceHealthState(
            HealthState? aggregatedHealthState = default(HealthState?),
            PartitionId partitionId = default(PartitionId),
            ReplicaId replicaId = default(ReplicaId))
            : base(
                Common.ServiceKind.Stateless,
                aggregatedHealthState,
                partitionId)
        {
            this.ReplicaId = replicaId;
        }

        /// <summary>
        /// Gets id of the stateless service instance on the wire this field is called ReplicaId.
        /// </summary>
        public ReplicaId ReplicaId { get; }
    }
}
