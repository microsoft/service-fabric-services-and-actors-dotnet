// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the identity, status, health, node name, uptime, and other details about the replica.
    /// </summary>
    public abstract partial class ReplicaInfo
    {
        /// <summary>
        /// Initializes a new instance of the ReplicaInfo class.
        /// </summary>
        /// <param name="serviceKind">The kind of service (Stateless or Stateful).</param>
        /// <param name="replicaStatus">The status of a replica of a service. Possible values include: 'Invalid', 'InBuild',
        /// 'Standby', 'Ready', 'Down', 'Dropped', 'Completed', 'ToBeRemoved'</param>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="address">The address the replica is listening on.</param>
        /// <param name="lastInBuildDurationInSeconds">The last in build duration of the replica in seconds.</param>
        protected ReplicaInfo(
            ServiceKind? serviceKind,
            ReplicaStatus? replicaStatus = default(ReplicaStatus?),
            HealthState? healthState = default(HealthState?),
            NodeName nodeName = default(NodeName),
            string address = default(string),
            string lastInBuildDurationInSeconds = default(string))
        {
            serviceKind.ThrowIfNull(nameof(serviceKind));
            this.ServiceKind = serviceKind;
            this.ReplicaStatus = replicaStatus;
            this.HealthState = healthState;
            this.NodeName = nodeName;
            this.Address = address;
            this.LastInBuildDurationInSeconds = lastInBuildDurationInSeconds;
        }

        /// <summary>
        /// Gets the status of a replica of a service. Possible values include: 'Invalid', 'InBuild', 'Standby', 'Ready',
        /// 'Down', 'Dropped', 'Completed', 'ToBeRemoved'
        /// </summary>
        public ReplicaStatus? ReplicaStatus { get; }

        /// <summary>
        /// Gets the health state of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica
        /// etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'
        /// </summary>
        public HealthState? HealthState { get; }

        /// <summary>
        /// Gets the name of a Service Fabric node.
        /// </summary>
        public NodeName NodeName { get; }

        /// <summary>
        /// Gets the address the replica is listening on.
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// Gets the last in build duration of the replica in seconds.
        /// </summary>
        public string LastInBuildDurationInSeconds { get; }

        /// <summary>
        /// Gets the kind of service (Stateless or Stateful).
        /// </summary>
        public ServiceKind? ServiceKind { get; }
    }
}
