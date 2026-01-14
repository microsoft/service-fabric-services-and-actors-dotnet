// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides details about the remote replicators from the primary replicator's point of view.
    /// </summary>
    public partial class RemoteReplicatorAcknowledgementStatus
    {
        /// <summary>
        /// Initializes a new instance of the RemoteReplicatorAcknowledgementStatus class.
        /// </summary>
        /// <param name="replicationStreamAcknowledgementDetail">Details about the acknowledgements for operations that are
        /// part of the replication stream data.</param>
        /// <param name="copyStreamAcknowledgementDetail">Details about the acknowledgements for operations that are part of
        /// the copy stream data.</param>
        public RemoteReplicatorAcknowledgementStatus(
            RemoteReplicatorAcknowledgementDetail replicationStreamAcknowledgementDetail = default(RemoteReplicatorAcknowledgementDetail),
            RemoteReplicatorAcknowledgementDetail copyStreamAcknowledgementDetail = default(RemoteReplicatorAcknowledgementDetail))
        {
            this.ReplicationStreamAcknowledgementDetail = replicationStreamAcknowledgementDetail;
            this.CopyStreamAcknowledgementDetail = copyStreamAcknowledgementDetail;
        }

        /// <summary>
        /// Gets details about the acknowledgements for operations that are part of the replication stream data.
        /// </summary>
        public RemoteReplicatorAcknowledgementDetail ReplicationStreamAcknowledgementDetail { get; }

        /// <summary>
        /// Gets details about the acknowledgements for operations that are part of the copy stream data.
        /// </summary>
        public RemoteReplicatorAcknowledgementDetail CopyStreamAcknowledgementDetail { get; }
    }
}
