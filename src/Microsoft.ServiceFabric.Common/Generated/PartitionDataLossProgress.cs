// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a partition data loss user-induced operation.
    /// </summary>
    public partial class PartitionDataLossProgress
    {
        /// <summary>
        /// Initializes a new instance of the PartitionDataLossProgress class.
        /// </summary>
        /// <param name="state">The state of the operation. Possible values include: 'Invalid', 'Running', 'RollingBack',
        /// 'Completed', 'Faulted', 'Cancelled', 'ForceCancelled'</param>
        /// <param name="invokeDataLossResult">Represents information about an operation in a terminal state (Completed or
        /// Faulted).</param>
        public PartitionDataLossProgress(
            OperationState? state = default(OperationState?),
            InvokeDataLossResult invokeDataLossResult = default(InvokeDataLossResult))
        {
            this.State = state;
            this.InvokeDataLossResult = invokeDataLossResult;
        }

        /// <summary>
        /// Gets the state of the operation. Possible values include: 'Invalid', 'Running', 'RollingBack', 'Completed',
        /// 'Faulted', 'Cancelled', 'ForceCancelled'
        /// </summary>
        public OperationState? State { get; }

        /// <summary>
        /// Gets represents information about an operation in a terminal state (Completed or Faulted).
        /// </summary>
        public InvokeDataLossResult InvokeDataLossResult { get; }
    }
}
