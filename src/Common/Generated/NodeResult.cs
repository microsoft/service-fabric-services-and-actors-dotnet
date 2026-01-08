// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains information about a node that was targeted by a user-induced operation.
    /// </summary>
    public partial class NodeResult
    {
        /// <summary>
        /// Initializes a new instance of the NodeResult class.
        /// </summary>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="nodeInstanceId">The node instance id.</param>
        public NodeResult(
            NodeName nodeName = default(NodeName),
            string nodeInstanceId = default(string))
        {
            this.NodeName = nodeName;
            this.NodeInstanceId = nodeInstanceId;
        }

        /// <summary>
        /// Gets the name of a Service Fabric node.
        /// </summary>
        public NodeName NodeName { get; }

        /// <summary>
        /// Gets the node instance id.
        /// </summary>
        public string NodeInstanceId { get; }
    }
}
