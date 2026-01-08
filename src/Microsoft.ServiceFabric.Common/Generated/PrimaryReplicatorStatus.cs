// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides statistics about the Service Fabric Replicator, when it is functioning in a Primary role.
    /// </summary>
    public partial class PrimaryReplicatorStatus : ReplicatorStatus
    {
        /// <summary>
        /// Initializes a new instance of the PrimaryReplicatorStatus class.
        /// </summary>
        /// <param name="replicationQueueStatus">Details about the replication queue on the primary replicator.</param>
        /// <param name="remoteReplicators">The status of all the active and idle secondary replicators that the primary is
        /// aware of.</param>
        public PrimaryReplicatorStatus(
            ReplicatorQueueStatus replicationQueueStatus = default(ReplicatorQueueStatus),
            IEnumerable<RemoteReplicatorStatus> remoteReplicators = default(IEnumerable<RemoteReplicatorStatus>))
            : base(
                Common.ReplicaRole.Primary)
        {
            this.ReplicationQueueStatus = replicationQueueStatus;
            this.RemoteReplicators = remoteReplicators;
        }

        /// <summary>
        /// Gets details about the replication queue on the primary replicator.
        /// </summary>
        public ReplicatorQueueStatus ReplicationQueueStatus { get; }

        /// <summary>
        /// Gets the status of all the active and idle secondary replicators that the primary is aware of.
        /// </summary>
        public IEnumerable<RemoteReplicatorStatus> RemoteReplicators { get; }
    }
}
