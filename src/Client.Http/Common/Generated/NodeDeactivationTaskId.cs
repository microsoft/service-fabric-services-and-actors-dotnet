// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Identity of the task related to deactivation operation on the node.
    /// </summary>
    public partial class NodeDeactivationTaskId
    {
        /// <summary>
        /// Initializes a new instance of the NodeDeactivationTaskId class.
        /// </summary>
        /// <param name="id">Value of the task id.</param>
        /// <param name="nodeDeactivationTaskType">The type of the task that performed the node deactivation. Following are the
        /// possible values. Possible values include: 'Invalid', 'Infrastructure', 'Repair', 'Client'</param>
        public NodeDeactivationTaskId(
            string id = default(string),
            NodeDeactivationTaskType? nodeDeactivationTaskType = default(NodeDeactivationTaskType?))
        {
            this.Id = id;
            this.NodeDeactivationTaskType = nodeDeactivationTaskType;
        }

        /// <summary>
        /// Gets value of the task id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the type of the task that performed the node deactivation. Following are the possible values. Possible values
        /// include: 'Invalid', 'Infrastructure', 'Repair', 'Client'
        /// </summary>
        public NodeDeactivationTaskType? NodeDeactivationTaskType { get; }
    }
}
