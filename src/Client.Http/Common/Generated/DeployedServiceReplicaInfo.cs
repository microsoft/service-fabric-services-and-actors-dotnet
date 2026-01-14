// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a Service Fabric service replica deployed on a node.
    /// </summary>
    public abstract partial class DeployedServiceReplicaInfo
    {
        /// <summary>
        /// Initializes a new instance of the DeployedServiceReplicaInfo class.
        /// </summary>
        /// <param name="serviceKind">The kind of service (Stateless or Stateful).</param>
        /// <param name="serviceName">The full name of the service with 'fabric:' URI scheme.</param>
        /// <param name="serviceTypeName">Name of the service type as specified in the service manifest.</param>
        /// <param name="serviceManifestName">The name of the service manifest in which this service type is defined.</param>
        /// <param name="codePackageName">The name of the code package that hosts this replica.</param>
        /// <param name="partitionId">An internal ID used by Service Fabric to uniquely identify a partition. This is a
        /// randomly generated GUID when the service was created. The partition ID is unique and does not change for the
        /// lifetime of the service. If the same service was deleted and recreated the IDs of its partitions would be
        /// different.</param>
        /// <param name="replicaStatus">The status of a replica of a service. Possible values include: 'Invalid', 'InBuild',
        /// 'Standby', 'Ready', 'Down', 'Dropped', 'Completed', 'ToBeRemoved'</param>
        /// <param name="address">The last address returned by the replica in Open or ChangeRole.</param>
        /// <param name="servicePackageActivationId">The ActivationId of a deployed service package. If
        /// ServicePackageActivationMode specified at the time of creating the service
        /// is 'SharedProcess' (or if it is not specified, in which case it defaults to 'SharedProcess'), then value of
        /// ServicePackageActivationId
        /// is always an empty string.
        /// </param>
        /// <param name="hostProcessId">Host process ID of the process that is hosting the replica. This will be zero if the
        /// replica is down. In hyper-v containers this host process ID will be from different kernel.</param>
        protected DeployedServiceReplicaInfo(
            ServiceKind? serviceKind,
            ServiceName serviceName = default(ServiceName),
            string serviceTypeName = default(string),
            string serviceManifestName = default(string),
            string codePackageName = default(string),
            PartitionId partitionId = default(PartitionId),
            ReplicaStatus? replicaStatus = default(ReplicaStatus?),
            string address = default(string),
            string servicePackageActivationId = default(string),
            string hostProcessId = default(string))
        {
            serviceKind.ThrowIfNull(nameof(serviceKind));
            this.ServiceKind = serviceKind;
            this.ServiceName = serviceName;
            this.ServiceTypeName = serviceTypeName;
            this.ServiceManifestName = serviceManifestName;
            this.CodePackageName = codePackageName;
            this.PartitionId = partitionId;
            this.ReplicaStatus = replicaStatus;
            this.Address = address;
            this.ServicePackageActivationId = servicePackageActivationId;
            this.HostProcessId = hostProcessId;
        }

        /// <summary>
        /// Gets the full name of the service with 'fabric:' URI scheme.
        /// </summary>
        public ServiceName ServiceName { get; }

        /// <summary>
        /// Gets name of the service type as specified in the service manifest.
        /// </summary>
        public string ServiceTypeName { get; }

        /// <summary>
        /// Gets the name of the service manifest in which this service type is defined.
        /// </summary>
        public string ServiceManifestName { get; }

        /// <summary>
        /// Gets the name of the code package that hosts this replica.
        /// </summary>
        public string CodePackageName { get; }

        /// <summary>
        /// Gets an internal ID used by Service Fabric to uniquely identify a partition. This is a randomly generated GUID when
        /// the service was created. The partition ID is unique and does not change for the lifetime of the service. If the
        /// same service was deleted and recreated the IDs of its partitions would be different.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets the status of a replica of a service. Possible values include: 'Invalid', 'InBuild', 'Standby', 'Ready',
        /// 'Down', 'Dropped', 'Completed', 'ToBeRemoved'
        /// </summary>
        public ReplicaStatus? ReplicaStatus { get; }

        /// <summary>
        /// Gets the last address returned by the replica in Open or ChangeRole.
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// Gets the ActivationId of a deployed service package. If ServicePackageActivationMode specified at the time of
        /// creating the service
        /// is 'SharedProcess' (or if it is not specified, in which case it defaults to 'SharedProcess'), then value of
        /// ServicePackageActivationId
        /// is always an empty string.
        /// </summary>
        public string ServicePackageActivationId { get; }

        /// <summary>
        /// Gets host process ID of the process that is hosting the replica. This will be zero if the replica is down. In
        /// hyper-v containers this host process ID will be from different kernel.
        /// </summary>
        public string HostProcessId { get; }

        /// <summary>
        /// Gets the kind of service (Stateless or Stateful).
        /// </summary>
        public ServiceKind? ServiceKind { get; }
    }
}
