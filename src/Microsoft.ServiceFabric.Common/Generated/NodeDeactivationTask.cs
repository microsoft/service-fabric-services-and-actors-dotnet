// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The task representing the deactivation operation on the node.
    /// </summary>
    public partial class NodeDeactivationTask
    {
        /// <summary>
        /// Initializes a new instance of the NodeDeactivationTask class.
        /// </summary>
        /// <param name="nodeDeactivationTaskId">Identity of the task related to deactivation operation on the node.</param>
        /// <param name="nodeDeactivationIntent">The intent or the reason for deactivating the node. Following are the possible
        /// values for it. Possible values include: 'Invalid', 'Pause', 'Restart', 'RemoveData', 'RemoveNode'</param>
        public NodeDeactivationTask(
            NodeDeactivationTaskId nodeDeactivationTaskId = default(NodeDeactivationTaskId),
            NodeDeactivationIntent? nodeDeactivationIntent = default(NodeDeactivationIntent?))
        {
            this.NodeDeactivationTaskId = nodeDeactivationTaskId;
            this.NodeDeactivationIntent = nodeDeactivationIntent;
        }

        /// <summary>
        /// Gets identity of the task related to deactivation operation on the node.
        /// </summary>
        public NodeDeactivationTaskId NodeDeactivationTaskId { get; }

        /// <summary>
        /// Gets the intent or the reason for deactivating the node. Following are the possible values for it. Possible values
        /// include: 'Invalid', 'Pause', 'Restart', 'RemoveData', 'RemoveNode'
        /// </summary>
        public NodeDeactivationIntent? NodeDeactivationIntent { get; }
    }
}
