// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a stateful service replica. This includes information about the identity, role, status, health, node
    /// name, uptime, and other details about the replica.
    /// </summary>
    public partial class StatefulServiceReplicaInfo : ReplicaInfo
    {
        /// <summary>
        /// Initializes a new instance of the StatefulServiceReplicaInfo class.
        /// </summary>
        /// <param name="replicaStatus">The status of a replica of a service. Possible values include: 'Invalid', 'InBuild',
        /// 'Standby', 'Ready', 'Down', 'Dropped', 'Completed', 'ToBeRemoved'</param>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="address">The address the replica is listening on.</param>
        /// <param name="lastInBuildDurationInSeconds">The last in build duration of the replica in seconds.</param>
        /// <param name="replicaRole">The role of a replica of a stateful service. Possible values include: 'Unknown', 'None',
        /// 'Primary', 'IdleSecondary', 'ActiveSecondary', 'IdleAuxiliary', 'ActiveAuxiliary', 'PrimaryAuxiliary'</param>
        /// <param name="replicaId">Id of a stateful service replica. ReplicaId is used by Service Fabric to uniquely identify
        /// a replica of a partition. It is unique within a partition and does not change for the lifetime of the replica. If a
        /// replica gets dropped and another replica gets created on the same node for the same partition, it will get a
        /// different value for the id. Sometimes the id of a stateless service instance is also referred as a replica
        /// id.</param>
        /// <param name="previousReplicaRole">The previous role of a replica of a stateful service during a reconfiguration.
        /// Possible values include: 'Unknown', 'None', 'Primary', 'IdleSecondary', 'ActiveSecondary', 'IdleAuxiliary',
        /// 'ActiveAuxiliary', 'PrimaryAuxiliary'
        /// 
        /// The role of a replica of a stateful service.
        /// </param>
        /// <param name="toBeRemovedExpirationTimeUtc">The best effort time when a ToBeRemoved replica will automatically be
        /// deleted if no manual action to recover is taken. This value has no effect if the replica status is not
        /// ToBeRemoved.</param>
        public StatefulServiceReplicaInfo(
            ReplicaStatus? replicaStatus = default(ReplicaStatus?),
            HealthState? healthState = default(HealthState?),
            NodeName nodeName = default(NodeName),
            string address = default(string),
            string lastInBuildDurationInSeconds = default(string),
            ReplicaRole? replicaRole = default(ReplicaRole?),
            ReplicaId replicaId = default(ReplicaId),
            ReplicaRole? previousReplicaRole = default(ReplicaRole?),
            DateTime? toBeRemovedExpirationTimeUtc = default(DateTime?))
            : base(
                Common.ServiceKind.Stateful,
                replicaStatus,
                healthState,
                nodeName,
                address,
                lastInBuildDurationInSeconds)
        {
            this.ReplicaRole = replicaRole;
            this.ReplicaId = replicaId;
            this.PreviousReplicaRole = previousReplicaRole;
            this.ToBeRemovedExpirationTimeUtc = toBeRemovedExpirationTimeUtc;
        }

        /// <summary>
        /// Gets the role of a replica of a stateful service. Possible values include: 'Unknown', 'None', 'Primary',
        /// 'IdleSecondary', 'ActiveSecondary', 'IdleAuxiliary', 'ActiveAuxiliary', 'PrimaryAuxiliary'
        /// </summary>
        public ReplicaRole? ReplicaRole { get; }

        /// <summary>
        /// Gets id of a stateful service replica. ReplicaId is used by Service Fabric to uniquely identify a replica of a
        /// partition. It is unique within a partition and does not change for the lifetime of the replica. If a replica gets
        /// dropped and another replica gets created on the same node for the same partition, it will get a different value for
        /// the id. Sometimes the id of a stateless service instance is also referred as a replica id.
        /// </summary>
        public ReplicaId ReplicaId { get; }

        /// <summary>
        /// Gets the previous role of a replica of a stateful service during a reconfiguration. Possible values include:
        /// 'Unknown', 'None', 'Primary', 'IdleSecondary', 'ActiveSecondary', 'IdleAuxiliary', 'ActiveAuxiliary',
        /// 'PrimaryAuxiliary'
        /// 
        /// The role of a replica of a stateful service.
        /// </summary>
        public ReplicaRole? PreviousReplicaRole { get; }

        /// <summary>
        /// Gets the best effort time when a ToBeRemoved replica will automatically be deleted if no manual action to recover
        /// is taken. This value has no effect if the replica status is not ToBeRemoved.
        /// </summary>
        public DateTime? ToBeRemovedExpirationTimeUtc { get; }
    }
}
