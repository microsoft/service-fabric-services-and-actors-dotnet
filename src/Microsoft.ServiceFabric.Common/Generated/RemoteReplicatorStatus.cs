// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the state of the secondary replicator from the primary replicator’s point of view.
    /// </summary>
    public partial class RemoteReplicatorStatus
    {
        /// <summary>
        /// Initializes a new instance of the RemoteReplicatorStatus class.
        /// </summary>
        /// <param name="replicaId">Represents the replica ID of the remote secondary replicator.</param>
        /// <param name="lastAcknowledgementProcessedTimeUtc">The last timestamp (in UTC) when an acknowledgement from the
        /// secondary replicator was processed on the primary.
        /// UTC 0 represents an invalid value, indicating that no acknowledgement messages were ever processed.
        /// </param>
        /// <param name="lastReceivedReplicationSequenceNumber">The highest replication operation sequence number that the
        /// secondary has received from the primary.</param>
        /// <param name="lastAppliedReplicationSequenceNumber">The highest replication operation sequence number that the
        /// secondary has applied to its state.</param>
        /// <param name="isInBuild">A value that indicates whether the secondary replica is in the process of being
        /// built.</param>
        /// <param name="lastReceivedCopySequenceNumber">The highest copy operation sequence number that the secondary has
        /// received from the primary.
        /// A value of -1 implies that the secondary has received all copy operations.
        /// </param>
        /// <param name="lastAppliedCopySequenceNumber">The highest copy operation sequence number that the secondary has
        /// applied to its state.
        /// A value of -1 implies that the secondary has applied all copy operations and the copy process is complete.
        /// </param>
        /// <param name="remoteReplicatorAcknowledgementStatus">Represents the acknowledgment status for the remote secondary
        /// replicator.</param>
        public RemoteReplicatorStatus(
            ReplicaId replicaId = default(ReplicaId),
            DateTime? lastAcknowledgementProcessedTimeUtc = default(DateTime?),
            string lastReceivedReplicationSequenceNumber = default(string),
            string lastAppliedReplicationSequenceNumber = default(string),
            bool? isInBuild = default(bool?),
            string lastReceivedCopySequenceNumber = default(string),
            string lastAppliedCopySequenceNumber = default(string),
            RemoteReplicatorAcknowledgementStatus remoteReplicatorAcknowledgementStatus = default(RemoteReplicatorAcknowledgementStatus))
        {
            this.ReplicaId = replicaId;
            this.LastAcknowledgementProcessedTimeUtc = lastAcknowledgementProcessedTimeUtc;
            this.LastReceivedReplicationSequenceNumber = lastReceivedReplicationSequenceNumber;
            this.LastAppliedReplicationSequenceNumber = lastAppliedReplicationSequenceNumber;
            this.IsInBuild = isInBuild;
            this.LastReceivedCopySequenceNumber = lastReceivedCopySequenceNumber;
            this.LastAppliedCopySequenceNumber = lastAppliedCopySequenceNumber;
            this.RemoteReplicatorAcknowledgementStatus = remoteReplicatorAcknowledgementStatus;
        }

        /// <summary>
        /// Gets represents the replica ID of the remote secondary replicator.
        /// </summary>
        public ReplicaId ReplicaId { get; }

        /// <summary>
        /// Gets the last timestamp (in UTC) when an acknowledgement from the secondary replicator was processed on the
        /// primary.
        /// UTC 0 represents an invalid value, indicating that no acknowledgement messages were ever processed.
        /// </summary>
        public DateTime? LastAcknowledgementProcessedTimeUtc { get; }

        /// <summary>
        /// Gets the highest replication operation sequence number that the secondary has received from the primary.
        /// </summary>
        public string LastReceivedReplicationSequenceNumber { get; }

        /// <summary>
        /// Gets the highest replication operation sequence number that the secondary has applied to its state.
        /// </summary>
        public string LastAppliedReplicationSequenceNumber { get; }

        /// <summary>
        /// Gets a value that indicates whether the secondary replica is in the process of being built.
        /// </summary>
        public bool? IsInBuild { get; }

        /// <summary>
        /// Gets the highest copy operation sequence number that the secondary has received from the primary.
        /// A value of -1 implies that the secondary has received all copy operations.
        /// </summary>
        public string LastReceivedCopySequenceNumber { get; }

        /// <summary>
        /// Gets the highest copy operation sequence number that the secondary has applied to its state.
        /// A value of -1 implies that the secondary has applied all copy operations and the copy process is complete.
        /// </summary>
        public string LastAppliedCopySequenceNumber { get; }

        /// <summary>
        /// Gets represents the acknowledgment status for the remote secondary replicator.
        /// </summary>
        public RemoteReplicatorAcknowledgementStatus RemoteReplicatorAcknowledgementStatus { get; }
    }
}
