// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about an NodeTransition operation.  This class contains an OperationState and a NodeTransitionResult. 
    /// The NodeTransitionResult is not valid until OperationState
    /// is Completed or Faulted.
    /// </summary>
    public partial class NodeTransitionProgress
    {
        /// <summary>
        /// Initializes a new instance of the NodeTransitionProgress class.
        /// </summary>
        /// <param name="state">The state of the operation. Possible values include: 'Invalid', 'Running', 'RollingBack',
        /// 'Completed', 'Faulted', 'Cancelled', 'ForceCancelled'</param>
        /// <param name="nodeTransitionResult">Represents information about an operation in a terminal state (Completed or
        /// Faulted).</param>
        public NodeTransitionProgress(
            OperationState? state = default(OperationState?),
            NodeTransitionResult nodeTransitionResult = default(NodeTransitionResult))
        {
            this.State = state;
            this.NodeTransitionResult = nodeTransitionResult;
        }

        /// <summary>
        /// Gets the state of the operation. Possible values include: 'Invalid', 'Running', 'RollingBack', 'Completed',
        /// 'Faulted', 'Cancelled', 'ForceCancelled'
        /// </summary>
        public OperationState? State { get; }

        /// <summary>
        /// Gets represents information about an operation in a terminal state (Completed or Faulted).
        /// </summary>
        public NodeTransitionResult NodeTransitionResult { get; }
    }
}
