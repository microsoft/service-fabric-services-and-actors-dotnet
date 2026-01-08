// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a stateful service replica deployed on a node.
    /// </summary>
    public partial class DeployedStatefulServiceReplicaInfo : DeployedServiceReplicaInfo
    {
        /// <summary>
        /// Initializes a new instance of the DeployedStatefulServiceReplicaInfo class.
        /// </summary>
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
        /// <param name="replicaId">Id of a stateful service replica. ReplicaId is used by Service Fabric to uniquely identify
        /// a replica of a partition. It is unique within a partition and does not change for the lifetime of the replica. If a
        /// replica gets dropped and another replica gets created on the same node for the same partition, it will get a
        /// different value for the id. Sometimes the id of a stateless service instance is also referred as a replica
        /// id.</param>
        /// <param name="replicaRole">The role of a replica of a stateful service. Possible values include: 'Unknown', 'None',
        /// 'Primary', 'IdleSecondary', 'ActiveSecondary', 'IdleAuxiliary', 'ActiveAuxiliary', 'PrimaryAuxiliary'</param>
        /// <param name="reconfigurationInformation">Information about current reconfiguration like phase, type, previous
        /// configuration role of replica and reconfiguration start date time.</param>
        public DeployedStatefulServiceReplicaInfo(
            ServiceName serviceName = default(ServiceName),
            string serviceTypeName = default(string),
            string serviceManifestName = default(string),
            string codePackageName = default(string),
            PartitionId partitionId = default(PartitionId),
            ReplicaStatus? replicaStatus = default(ReplicaStatus?),
            string address = default(string),
            string servicePackageActivationId = default(string),
            string hostProcessId = default(string),
            ReplicaId replicaId = default(ReplicaId),
            ReplicaRole? replicaRole = default(ReplicaRole?),
            ReconfigurationInformation reconfigurationInformation = default(ReconfigurationInformation))
            : base(
                Common.ServiceKind.Stateful,
                serviceName,
                serviceTypeName,
                serviceManifestName,
                codePackageName,
                partitionId,
                replicaStatus,
                address,
                servicePackageActivationId,
                hostProcessId)
        {
            this.ReplicaId = replicaId;
            this.ReplicaRole = replicaRole;
            this.ReconfigurationInformation = reconfigurationInformation;
        }

        /// <summary>
        /// Gets id of a stateful service replica. ReplicaId is used by Service Fabric to uniquely identify a replica of a
        /// partition. It is unique within a partition and does not change for the lifetime of the replica. If a replica gets
        /// dropped and another replica gets created on the same node for the same partition, it will get a different value for
        /// the id. Sometimes the id of a stateless service instance is also referred as a replica id.
        /// </summary>
        public ReplicaId ReplicaId { get; }

        /// <summary>
        /// Gets the role of a replica of a stateful service. Possible values include: 'Unknown', 'None', 'Primary',
        /// 'IdleSecondary', 'ActiveSecondary', 'IdleAuxiliary', 'ActiveAuxiliary', 'PrimaryAuxiliary'
        /// </summary>
        public ReplicaRole? ReplicaRole { get; }

        /// <summary>
        /// Gets information about current reconfiguration like phase, type, previous configuration role of replica and
        /// reconfiguration start date time.
        /// </summary>
        public ReconfigurationInformation ReconfigurationInformation { get; }
    }
}
