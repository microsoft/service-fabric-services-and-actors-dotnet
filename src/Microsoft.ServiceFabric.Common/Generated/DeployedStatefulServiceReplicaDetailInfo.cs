// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a stateful replica running in a code package. Note DeployedServiceReplicaQueryResult will contain
    /// duplicate data like ServiceKind, ServiceName, PartitionId and replicaId.
    /// </summary>
    public partial class DeployedStatefulServiceReplicaDetailInfo : DeployedServiceReplicaDetailInfo
    {
        /// <summary>
        /// Initializes a new instance of the DeployedStatefulServiceReplicaDetailInfo class.
        /// </summary>
        /// <param name="serviceName">Full hierarchical name of the service in URI format starting with `fabric:`.</param>
        /// <param name="partitionId">An internal ID used by Service Fabric to uniquely identify a partition. This is a
        /// randomly generated GUID when the service was created. The partition ID is unique and does not change for the
        /// lifetime of the service. If the same service was deleted and recreated the IDs of its partitions would be
        /// different.</param>
        /// <param name="currentServiceOperation">Specifies the current active life-cycle operation on a stateful service
        /// replica or stateless service instance. Possible values include: 'Unknown', 'None', 'Open', 'ChangeRole', 'Close',
        /// 'Abort'</param>
        /// <param name="currentServiceOperationStartTimeUtc">The start time of the current service operation in UTC
        /// format.</param>
        /// <param name="reportedLoad">List of load reported by replica.</param>
        /// <param name="replicaId">Id of a stateful service replica. ReplicaId is used by Service Fabric to uniquely identify
        /// a replica of a partition. It is unique within a partition and does not change for the lifetime of the replica. If a
        /// replica gets dropped and another replica gets created on the same node for the same partition, it will get a
        /// different value for the id. Sometimes the id of a stateless service instance is also referred as a replica
        /// id.</param>
        /// <param name="currentReplicatorOperation">Specifies the operation currently being executed by the Replicator.
        /// Possible values include: 'Invalid', 'None', 'Open', 'ChangeRole', 'UpdateEpoch', 'Close', 'Abort', 'OnDataLoss',
        /// 'WaitForCatchup', 'Build'</param>
        /// <param name="readStatus">Specifies the access status of the partition. Possible values include: 'Invalid',
        /// 'Granted', 'ReconfigurationPending', 'NotPrimary', 'NoWriteQuorum'</param>
        /// <param name="writeStatus">Specifies the access status of the partition. Possible values include: 'Invalid',
        /// 'Granted', 'ReconfigurationPending', 'NotPrimary', 'NoWriteQuorum'</param>
        /// <param name="replicatorStatus">Represents a base class for primary or secondary replicator status.
        /// Contains information about the service fabric replicator like the replication/copy queue utilization, last
        /// acknowledgement received timestamp, etc.
        /// </param>
        /// <param name="replicaStatus">Key value store related information for the replica.</param>
        /// <param name="deployedServiceReplicaQueryResult">Information about a stateful service replica deployed on a
        /// node.</param>
        public DeployedStatefulServiceReplicaDetailInfo(
            ServiceName serviceName = default(ServiceName),
            PartitionId partitionId = default(PartitionId),
            ServiceOperationName? currentServiceOperation = default(ServiceOperationName?),
            DateTime? currentServiceOperationStartTimeUtc = default(DateTime?),
            IEnumerable<LoadMetricReportInfo> reportedLoad = default(IEnumerable<LoadMetricReportInfo>),
            ReplicaId replicaId = default(ReplicaId),
            ReplicatorOperationName? currentReplicatorOperation = default(ReplicatorOperationName?),
            PartitionAccessStatus? readStatus = default(PartitionAccessStatus?),
            PartitionAccessStatus? writeStatus = default(PartitionAccessStatus?),
            ReplicatorStatus replicatorStatus = default(ReplicatorStatus),
            KeyValueStoreReplicaStatus replicaStatus = default(KeyValueStoreReplicaStatus),
            DeployedStatefulServiceReplicaInfo deployedServiceReplicaQueryResult = default(DeployedStatefulServiceReplicaInfo))
            : base(
                Common.ServiceKind.Stateful,
                serviceName,
                partitionId,
                currentServiceOperation,
                currentServiceOperationStartTimeUtc,
                reportedLoad)
        {
            this.ReplicaId = replicaId;
            this.CurrentReplicatorOperation = currentReplicatorOperation;
            this.ReadStatus = readStatus;
            this.WriteStatus = writeStatus;
            this.ReplicatorStatus = replicatorStatus;
            this.ReplicaStatus = replicaStatus;
            this.DeployedServiceReplicaQueryResult = deployedServiceReplicaQueryResult;
        }

        /// <summary>
        /// Gets id of a stateful service replica. ReplicaId is used by Service Fabric to uniquely identify a replica of a
        /// partition. It is unique within a partition and does not change for the lifetime of the replica. If a replica gets
        /// dropped and another replica gets created on the same node for the same partition, it will get a different value for
        /// the id. Sometimes the id of a stateless service instance is also referred as a replica id.
        /// </summary>
        public ReplicaId ReplicaId { get; }

        /// <summary>
        /// Gets specifies the operation currently being executed by the Replicator. Possible values include: 'Invalid',
        /// 'None', 'Open', 'ChangeRole', 'UpdateEpoch', 'Close', 'Abort', 'OnDataLoss', 'WaitForCatchup', 'Build'
        /// </summary>
        public ReplicatorOperationName? CurrentReplicatorOperation { get; }

        /// <summary>
        /// Gets specifies the access status of the partition. Possible values include: 'Invalid', 'Granted',
        /// 'ReconfigurationPending', 'NotPrimary', 'NoWriteQuorum'
        /// </summary>
        public PartitionAccessStatus? ReadStatus { get; }

        /// <summary>
        /// Gets specifies the access status of the partition. Possible values include: 'Invalid', 'Granted',
        /// 'ReconfigurationPending', 'NotPrimary', 'NoWriteQuorum'
        /// </summary>
        public PartitionAccessStatus? WriteStatus { get; }

        /// <summary>
        /// Gets represents a base class for primary or secondary replicator status.
        /// Contains information about the service fabric replicator like the replication/copy queue utilization, last
        /// acknowledgement received timestamp, etc.
        /// </summary>
        public ReplicatorStatus ReplicatorStatus { get; }

        /// <summary>
        /// Gets key value store related information for the replica.
        /// </summary>
        public KeyValueStoreReplicaStatus ReplicaStatus { get; }

        /// <summary>
        /// Gets information about a stateful service replica deployed on a node.
        /// </summary>
        public DeployedStatefulServiceReplicaInfo DeployedServiceReplicaQueryResult { get; }
    }
}
