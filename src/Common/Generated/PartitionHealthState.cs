// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state of a partition, which contains the partition identifier and its aggregated health
    /// state.
    /// </summary>
    public partial class PartitionHealthState : EntityHealthState
    {
        /// <summary>
        /// Initializes a new instance of the PartitionHealthState class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="partitionId">Id of the partition whose health state is described by this object.</param>
        public PartitionHealthState(
            HealthState? aggregatedHealthState = default(HealthState?),
            PartitionId partitionId = default(PartitionId))
            : base(
                aggregatedHealthState)
        {
            this.PartitionId = partitionId;
        }

        /// <summary>
        /// Gets id of the partition whose health state is described by this object.
        /// </summary>
        public PartitionId PartitionId { get; }
    }
}
