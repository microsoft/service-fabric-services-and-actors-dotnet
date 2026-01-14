// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents information about an operation in a terminal state (Completed or Faulted).
    /// </summary>
    public partial class NodeTransitionResult
    {
        /// <summary>
        /// Initializes a new instance of the NodeTransitionResult class.
        /// </summary>
        /// <param name="errorCode">If OperationState is Completed, this is 0.  If OperationState is Faulted, this is an error
        /// code indicating the reason.</param>
        /// <param name="nodeResult">Contains information about a node that was targeted by a user-induced operation.</param>
        public NodeTransitionResult(
            int? errorCode = default(int?),
            NodeResult nodeResult = default(NodeResult))
        {
            this.ErrorCode = errorCode;
            this.NodeResult = nodeResult;
        }

        /// <summary>
        /// Gets if OperationState is Completed, this is 0.  If OperationState is Faulted, this is an error code indicating the
        /// reason.
        /// </summary>
        public int? ErrorCode { get; }

        /// <summary>
        /// Gets contains information about a node that was targeted by a user-induced operation.
        /// </summary>
        public NodeResult NodeResult { get; }
    }
}
