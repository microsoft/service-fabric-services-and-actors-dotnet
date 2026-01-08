// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides various statistics of the queue used in the service fabric replicator.
    /// Contains information about the service fabric replicator like the replication/copy queue utilization, last
    /// acknowledgement received timestamp, etc.
    /// Depending on the role of the replicator, the properties in this type imply different meanings.
    /// </summary>
    public partial class ReplicatorQueueStatus
    {
        /// <summary>
        /// Initializes a new instance of the ReplicatorQueueStatus class.
        /// </summary>
        /// <param name="queueUtilizationPercentage">Represents the utilization of the queue. A value of 0 indicates that the
        /// queue is empty and a value of 100 indicates the queue is full.</param>
        /// <param name="queueMemorySize">Represents the virtual memory consumed by the queue in bytes.</param>
        /// <param name="firstSequenceNumber">On a primary replicator, this is semantically the sequence number of the
        /// operation for which all the secondary replicas have sent an acknowledgement.
        /// On a secondary replicator, this is the smallest sequence number of the operation that is present in the queue.
        /// </param>
        /// <param name="completedSequenceNumber">On a primary replicator, this is semantically the highest sequence number of
        /// the operation for which all the secondary replicas have sent an acknowledgement.
        /// On a secondary replicator, this is semantically the highest sequence number that has been applied to the persistent
        /// state.
        /// </param>
        /// <param name="committedSequenceNumber">On a primary replicator, this is semantically the highest sequence number of
        /// the operation for which a write quorum of the secondary replicas have sent an acknowledgement.
        /// On a secondary replicator, this is semantically the highest sequence number of the in-order operation received from
        /// the primary.
        /// </param>
        /// <param name="lastSequenceNumber">Represents the latest sequence number of the operation that is available in the
        /// queue.</param>
        public ReplicatorQueueStatus(
            int? queueUtilizationPercentage = default(int?),
            string queueMemorySize = default(string),
            string firstSequenceNumber = default(string),
            string completedSequenceNumber = default(string),
            string committedSequenceNumber = default(string),
            string lastSequenceNumber = default(string))
        {
            this.QueueUtilizationPercentage = queueUtilizationPercentage;
            this.QueueMemorySize = queueMemorySize;
            this.FirstSequenceNumber = firstSequenceNumber;
            this.CompletedSequenceNumber = completedSequenceNumber;
            this.CommittedSequenceNumber = committedSequenceNumber;
            this.LastSequenceNumber = lastSequenceNumber;
        }

        /// <summary>
        /// Gets represents the utilization of the queue. A value of 0 indicates that the queue is empty and a value of 100
        /// indicates the queue is full.
        /// </summary>
        public int? QueueUtilizationPercentage { get; }

        /// <summary>
        /// Gets represents the virtual memory consumed by the queue in bytes.
        /// </summary>
        public string QueueMemorySize { get; }

        /// <summary>
        /// Gets on a primary replicator, this is semantically the sequence number of the operation for which all the secondary
        /// replicas have sent an acknowledgement.
        /// On a secondary replicator, this is the smallest sequence number of the operation that is present in the queue.
        /// </summary>
        public string FirstSequenceNumber { get; }

        /// <summary>
        /// Gets on a primary replicator, this is semantically the highest sequence number of the operation for which all the
        /// secondary replicas have sent an acknowledgement.
        /// On a secondary replicator, this is semantically the highest sequence number that has been applied to the persistent
        /// state.
        /// </summary>
        public string CompletedSequenceNumber { get; }

        /// <summary>
        /// Gets on a primary replicator, this is semantically the highest sequence number of the operation for which a write
        /// quorum of the secondary replicas have sent an acknowledgement.
        /// On a secondary replicator, this is semantically the highest sequence number of the in-order operation received from
        /// the primary.
        /// </summary>
        public string CommittedSequenceNumber { get; }

        /// <summary>
        /// Gets represents the latest sequence number of the operation that is available in the queue.
        /// </summary>
        public string LastSequenceNumber { get; }
    }
}
