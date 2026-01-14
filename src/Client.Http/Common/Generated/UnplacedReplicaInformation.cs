// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains information for an unplaced replica.
    /// </summary>
    public partial class UnplacedReplicaInformation
    {
        /// <summary>
        /// Initializes a new instance of the UnplacedReplicaInformation class.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="partitionId">The ID of the partition.</param>
        /// <param name="unplacedReplicaDetails">List of reasons due to which a replica cannot be placed.</param>
        public UnplacedReplicaInformation(
            ServiceName serviceName = default(ServiceName),
            PartitionId partitionId = default(PartitionId),
            IEnumerable<string> unplacedReplicaDetails = default(IEnumerable<string>))
        {
            this.ServiceName = serviceName;
            this.PartitionId = partitionId;
            this.UnplacedReplicaDetails = unplacedReplicaDetails;
        }

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        public ServiceName ServiceName { get; }

        /// <summary>
        /// Gets the ID of the partition.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets list of reasons due to which a replica cannot be placed.
        /// </summary>
        public IEnumerable<string> UnplacedReplicaDetails { get; }
    }
}
