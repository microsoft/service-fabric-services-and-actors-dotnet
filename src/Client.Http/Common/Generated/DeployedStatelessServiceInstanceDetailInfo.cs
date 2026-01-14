// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a stateless instance running in a code package. Note that DeployedServiceReplicaQueryResult will
    /// contain duplicate data like ServiceKind, ServiceName, PartitionId and InstanceId.
    /// </summary>
    public partial class DeployedStatelessServiceInstanceDetailInfo : DeployedServiceReplicaDetailInfo
    {
        /// <summary>
        /// Initializes a new instance of the DeployedStatelessServiceInstanceDetailInfo class.
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
        /// <param name="instanceId">Id of a stateless service instance. InstanceId is used by Service Fabric to uniquely
        /// identify an instance of a partition of a stateless service. It is unique within a partition and does not change for
        /// the lifetime of the instance. If the instance has failed over on the same or different node, it will get a
        /// different value for the InstanceId.</param>
        /// <param name="deployedServiceReplicaQueryResult">Information about a stateless service instance deployed on a
        /// node.</param>
        public DeployedStatelessServiceInstanceDetailInfo(
            ServiceName serviceName = default(ServiceName),
            PartitionId partitionId = default(PartitionId),
            ServiceOperationName? currentServiceOperation = default(ServiceOperationName?),
            DateTime? currentServiceOperationStartTimeUtc = default(DateTime?),
            IEnumerable<LoadMetricReportInfo> reportedLoad = default(IEnumerable<LoadMetricReportInfo>),
            ReplicaId instanceId = default(ReplicaId),
            DeployedStatelessServiceInstanceInfo deployedServiceReplicaQueryResult = default(DeployedStatelessServiceInstanceInfo))
            : base(
                Common.ServiceKind.Stateless,
                serviceName,
                partitionId,
                currentServiceOperation,
                currentServiceOperationStartTimeUtc,
                reportedLoad)
        {
            this.InstanceId = instanceId;
            this.DeployedServiceReplicaQueryResult = deployedServiceReplicaQueryResult;
        }

        /// <summary>
        /// Gets id of a stateless service instance. InstanceId is used by Service Fabric to uniquely identify an instance of a
        /// partition of a stateless service. It is unique within a partition and does not change for the lifetime of the
        /// instance. If the instance has failed over on the same or different node, it will get a different value for the
        /// InstanceId.
        /// </summary>
        public ReplicaId InstanceId { get; }

        /// <summary>
        /// Gets information about a stateless service instance deployed on a node.
        /// </summary>
        public DeployedStatelessServiceInstanceInfo DeployedServiceReplicaQueryResult { get; }
    }
}
