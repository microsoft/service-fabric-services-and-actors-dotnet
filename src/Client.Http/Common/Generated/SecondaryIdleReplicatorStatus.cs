// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Status of the secondary replicator when it is in idle mode and is being built by the primary.
    /// </summary>
    public partial class SecondaryIdleReplicatorStatus : SecondaryReplicatorStatus
    {
        /// <summary>
        /// Initializes a new instance of the SecondaryIdleReplicatorStatus class.
        /// </summary>
        /// <param name="replicationQueueStatus">Details about the replication queue on the secondary replicator.</param>
        /// <param name="lastReplicationOperationReceivedTimeUtc">The last time-stamp (UTC) at which a replication operation
        /// was received from the primary.
        /// UTC 0 represents an invalid value, indicating that a replication operation message was never received.
        /// </param>
        /// <param name="isInBuild">Value that indicates whether the replica is currently being built.</param>
        /// <param name="copyQueueStatus">Details about the copy queue on the secondary replicator.</param>
        /// <param name="lastCopyOperationReceivedTimeUtc">The last time-stamp (UTC) at which a copy operation was received
        /// from the primary.
        /// UTC 0 represents an invalid value, indicating that a copy operation message was never received.
        /// </param>
        /// <param name="lastAcknowledgementSentTimeUtc">The last time-stamp (UTC) at which an acknowledgment was sent to the
        /// primary replicator.
        /// UTC 0 represents an invalid value, indicating that an acknowledgment message was never sent.
        /// </param>
        public SecondaryIdleReplicatorStatus(
            ReplicatorQueueStatus replicationQueueStatus = default(ReplicatorQueueStatus),
            DateTime? lastReplicationOperationReceivedTimeUtc = default(DateTime?),
            bool? isInBuild = default(bool?),
            ReplicatorQueueStatus copyQueueStatus = default(ReplicatorQueueStatus),
            DateTime? lastCopyOperationReceivedTimeUtc = default(DateTime?),
            DateTime? lastAcknowledgementSentTimeUtc = default(DateTime?))
            : base(
                Common.ReplicaRole.IdleSecondary,
                replicationQueueStatus,
                lastReplicationOperationReceivedTimeUtc,
                isInBuild,
                copyQueueStatus,
                lastCopyOperationReceivedTimeUtc,
                lastAcknowledgementSentTimeUtc)
        {
        }
    }
}
