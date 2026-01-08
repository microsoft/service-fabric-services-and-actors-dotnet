// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a partition of a Service Fabric service.
    /// </summary>
    public abstract partial class ServicePartitionInfo
    {
        /// <summary>
        /// Initializes a new instance of the ServicePartitionInfo class.
        /// </summary>
        /// <param name="serviceKind">The kind of service (Stateless or Stateful).</param>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="partitionStatus">The status of the service fabric service partition. Possible values include:
        /// 'Invalid', 'Ready', 'NotReady', 'InQuorumLoss', 'Reconfiguring', 'Deleting'</param>
        /// <param name="partitionInformation">Information about the partition identity, partitioning scheme and keys supported
        /// by it.</param>
        protected ServicePartitionInfo(
            ServiceKind? serviceKind,
            HealthState? healthState = default(HealthState?),
            ServicePartitionStatus? partitionStatus = default(ServicePartitionStatus?),
            PartitionInformation partitionInformation = default(PartitionInformation))
        {
            serviceKind.ThrowIfNull(nameof(serviceKind));
            this.ServiceKind = serviceKind;
            this.HealthState = healthState;
            this.PartitionStatus = partitionStatus;
            this.PartitionInformation = partitionInformation;
        }

        /// <summary>
        /// Gets the health state of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica
        /// etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'
        /// </summary>
        public HealthState? HealthState { get; }

        /// <summary>
        /// Gets the status of the service fabric service partition. Possible values include: 'Invalid', 'Ready', 'NotReady',
        /// 'InQuorumLoss', 'Reconfiguring', 'Deleting'
        /// </summary>
        public ServicePartitionStatus? PartitionStatus { get; }

        /// <summary>
        /// Gets information about the partition identity, partitioning scheme and keys supported by it.
        /// </summary>
        public PartitionInformation PartitionInformation { get; }

        /// <summary>
        /// Gets the kind of service (Stateless or Stateful).
        /// </summary>
        public ServiceKind? ServiceKind { get; }
    }
}
