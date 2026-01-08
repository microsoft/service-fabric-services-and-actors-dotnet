// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class returns information about the partition that the user-induced operation acted upon.
    /// </summary>
    public partial class SelectedPartition
    {
        /// <summary>
        /// Initializes a new instance of the SelectedPartition class.
        /// </summary>
        /// <param name="serviceName">The name of the service the partition belongs to.</param>
        /// <param name="partitionId">An internal ID used by Service Fabric to uniquely identify a partition. This is a
        /// randomly generated GUID when the service was created. The partition ID is unique and does not change for the
        /// lifetime of the service. If the same service was deleted and recreated the IDs of its partitions would be
        /// different.</param>
        public SelectedPartition(
            ServiceName serviceName = default(ServiceName),
            PartitionId partitionId = default(PartitionId))
        {
            this.ServiceName = serviceName;
            this.PartitionId = partitionId;
        }

        /// <summary>
        /// Gets the name of the service the partition belongs to.
        /// </summary>
        public ServiceName ServiceName { get; }

        /// <summary>
        /// Gets an internal ID used by Service Fabric to uniquely identify a partition. This is a randomly generated GUID when
        /// the service was created. The partition ID is unique and does not change for the lifetime of the service. If the
        /// same service was deleted and recreated the IDs of its partitions would be different.
        /// </summary>
        public PartitionId PartitionId { get; }
    }
}
