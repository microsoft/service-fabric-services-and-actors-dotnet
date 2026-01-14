// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a base class for stateful service replica or stateless service instance health state.
    /// </summary>
    public partial class ReplicaHealthState : EntityHealthState
    {
        /// <summary>
        /// Initializes a new instance of the ReplicaHealthState class.
        /// </summary>
        /// <param name="serviceKind">The kind of service (Stateless or Stateful).</param>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="partitionId">The ID of the partition to which this replica belongs.</param>
        public ReplicaHealthState(
            ServiceKind? serviceKind,
            HealthState? aggregatedHealthState = default(HealthState?),
            PartitionId partitionId = default(PartitionId))
            : base(
                aggregatedHealthState)
        {
            serviceKind.ThrowIfNull(nameof(serviceKind));
            this.ServiceKind = serviceKind;
            this.PartitionId = partitionId;
        }

        /// <summary>
        /// Gets the ID of the partition to which this replica belongs.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets the kind of service (Stateless or Stateful).
        /// </summary>
        public ServiceKind? ServiceKind { get; }
    }
}
