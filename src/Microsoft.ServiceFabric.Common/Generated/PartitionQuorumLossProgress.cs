// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a partition quorum loss user-induced operation.
    /// </summary>
    public partial class PartitionQuorumLossProgress
    {
        /// <summary>
        /// Initializes a new instance of the PartitionQuorumLossProgress class.
        /// </summary>
        /// <param name="state">The state of the operation. Possible values include: 'Invalid', 'Running', 'RollingBack',
        /// 'Completed', 'Faulted', 'Cancelled', 'ForceCancelled'</param>
        /// <param name="invokeQuorumLossResult">Represents information about an operation in a terminal state (Completed or
        /// Faulted).</param>
        public PartitionQuorumLossProgress(
            OperationState? state = default(OperationState?),
            InvokeQuorumLossResult invokeQuorumLossResult = default(InvokeQuorumLossResult))
        {
            this.State = state;
            this.InvokeQuorumLossResult = invokeQuorumLossResult;
        }

        /// <summary>
        /// Gets the state of the operation. Possible values include: 'Invalid', 'Running', 'RollingBack', 'Completed',
        /// 'Faulted', 'Cancelled', 'ForceCancelled'
        /// </summary>
        public OperationState? State { get; }

        /// <summary>
        /// Gets represents information about an operation in a terminal state (Completed or Faulted).
        /// </summary>
        public InvokeQuorumLossResult InvokeQuorumLossResult { get; }
    }
}
