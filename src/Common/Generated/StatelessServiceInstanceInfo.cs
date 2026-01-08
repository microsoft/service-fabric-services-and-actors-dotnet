// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a stateless service instance. This includes information about the identity, status, health, node name,
    /// uptime, and other details about the instance.
    /// </summary>
    public partial class StatelessServiceInstanceInfo : ReplicaInfo
    {
        /// <summary>
        /// Initializes a new instance of the StatelessServiceInstanceInfo class.
        /// </summary>
        /// <param name="replicaStatus">The status of a replica of a service. Possible values include: 'Invalid', 'InBuild',
        /// 'Standby', 'Ready', 'Down', 'Dropped', 'Completed', 'ToBeRemoved'</param>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="address">The address the replica is listening on.</param>
        /// <param name="lastInBuildDurationInSeconds">The last in build duration of the replica in seconds.</param>
        /// <param name="instanceId">Id of a stateless service instance. InstanceId is used by Service Fabric to uniquely
        /// identify an instance of a partition of a stateless service. It is unique within a partition and does not change for
        /// the lifetime of the instance. If the instance has failed over on the same or different node, it will get a
        /// different value for the InstanceId.</param>
        public StatelessServiceInstanceInfo(
            ReplicaStatus? replicaStatus = default(ReplicaStatus?),
            HealthState? healthState = default(HealthState?),
            NodeName nodeName = default(NodeName),
            string address = default(string),
            string lastInBuildDurationInSeconds = default(string),
            ReplicaId instanceId = default(ReplicaId))
            : base(
                Common.ServiceKind.Stateless,
                replicaStatus,
                healthState,
                nodeName,
                address,
                lastInBuildDurationInSeconds)
        {
            this.InstanceId = instanceId;
        }

        /// <summary>
        /// Gets id of a stateless service instance. InstanceId is used by Service Fabric to uniquely identify an instance of a
        /// partition of a stateless service. It is unique within a partition and does not change for the lifetime of the
        /// instance. If the instance has failed over on the same or different node, it will get a different value for the
        /// InstanceId.
        /// </summary>
        public ReplicaId InstanceId { get; }
    }
}
