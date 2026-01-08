// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides various statistics of the acknowledgements that are being received from the remote replicator.
    /// </summary>
    public partial class RemoteReplicatorAcknowledgementDetail
    {
        /// <summary>
        /// Initializes a new instance of the RemoteReplicatorAcknowledgementDetail class.
        /// </summary>
        /// <param name="averageReceiveDuration">Represents the average duration it takes for the remote replicator to receive
        /// an operation.</param>
        /// <param name="averageApplyDuration">Represents the average duration it takes for the remote replicator to apply an
        /// operation. This usually entails writing the operation to disk.</param>
        /// <param name="notReceivedCount">Represents the number of operations not yet received by a remote replicator.</param>
        /// <param name="receivedAndNotAppliedCount">Represents the number of operations received and not yet applied by a
        /// remote replicator.</param>
        public RemoteReplicatorAcknowledgementDetail(
            string averageReceiveDuration = default(string),
            string averageApplyDuration = default(string),
            string notReceivedCount = default(string),
            string receivedAndNotAppliedCount = default(string))
        {
            this.AverageReceiveDuration = averageReceiveDuration;
            this.AverageApplyDuration = averageApplyDuration;
            this.NotReceivedCount = notReceivedCount;
            this.ReceivedAndNotAppliedCount = receivedAndNotAppliedCount;
        }

        /// <summary>
        /// Gets represents the average duration it takes for the remote replicator to receive an operation.
        /// </summary>
        public string AverageReceiveDuration { get; }

        /// <summary>
        /// Gets represents the average duration it takes for the remote replicator to apply an operation. This usually entails
        /// writing the operation to disk.
        /// </summary>
        public string AverageApplyDuration { get; }

        /// <summary>
        /// Gets represents the number of operations not yet received by a remote replicator.
        /// </summary>
        public string NotReceivedCount { get; }

        /// <summary>
        /// Gets represents the number of operations received and not yet applied by a remote replicator.
        /// </summary>
        public string ReceivedAndNotAppliedCount { get; }
    }
}
