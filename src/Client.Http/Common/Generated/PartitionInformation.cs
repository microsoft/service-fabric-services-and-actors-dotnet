// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the partition identity, partitioning scheme and keys supported by it.
    /// </summary>
    public abstract partial class PartitionInformation
    {
        /// <summary>
        /// Initializes a new instance of the PartitionInformation class.
        /// </summary>
        /// <param name="servicePartitionKind">The kind of partitioning scheme used to partition the service.</param>
        /// <param name="id">An internal ID used by Service Fabric to uniquely identify a partition. This is a randomly
        /// generated GUID when the service was created. The partition ID is unique and does not change for the lifetime of the
        /// service. If the same service was deleted and recreated the IDs of its partitions would be different.</param>
        protected PartitionInformation(
            ServicePartitionKind? servicePartitionKind,
            PartitionId id = default(PartitionId))
        {
            servicePartitionKind.ThrowIfNull(nameof(servicePartitionKind));
            this.ServicePartitionKind = servicePartitionKind;
            this.Id = id;
        }

        /// <summary>
        /// Gets an internal ID used by Service Fabric to uniquely identify a partition. This is a randomly generated GUID when
        /// the service was created. The partition ID is unique and does not change for the lifetime of the service. If the
        /// same service was deleted and recreated the IDs of its partitions would be different.
        /// </summary>
        public PartitionId Id { get; }

        /// <summary>
        /// Gets the kind of partitioning scheme used to partition the service.
        /// </summary>
        public ServicePartitionKind? ServicePartitionKind { get; }
    }
}
