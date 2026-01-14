// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state chunk of a service, which contains the service name, its aggregated health state and
    /// any partitions that respect the filters in the cluster health chunk query description.
    /// </summary>
    public partial class ServiceHealthStateChunk : EntityHealthStateChunk
    {
        /// <summary>
        /// Initializes a new instance of the ServiceHealthStateChunk class.
        /// </summary>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="serviceName">The name of the service whose health state chunk is provided in this object.</param>
        /// <param name="partitionHealthStateChunks">The list of partition health state chunks belonging to the service that
        /// respect the filters in the cluster health chunk query description.
        /// </param>
        public ServiceHealthStateChunk(
            HealthState? healthState = default(HealthState?),
            ServiceName serviceName = default(ServiceName),
            PartitionHealthStateChunkList partitionHealthStateChunks = default(PartitionHealthStateChunkList))
            : base(
                healthState)
        {
            this.ServiceName = serviceName;
            this.PartitionHealthStateChunks = partitionHealthStateChunks;
        }

        /// <summary>
        /// Gets the name of the service whose health state chunk is provided in this object.
        /// </summary>
        public ServiceName ServiceName { get; }

        /// <summary>
        /// Gets the list of partition health state chunks belonging to the service that respect the filters in the cluster
        /// health chunk query description.
        /// </summary>
        public PartitionHealthStateChunkList PartitionHealthStateChunks { get; }
    }
}
