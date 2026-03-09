// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The information about the Failover Manager Manager (FMM).
    /// </summary>
    public partial class FailoverManagerManagerInformation
    {
        /// <summary>
        /// Initializes a new instance of the FailoverManagerManagerInformation class.
        /// </summary>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="nodeId">An internal ID used by Service Fabric to uniquely identify a node. Node Id is
        /// deterministically generated from node name.</param>
        /// <param name="nodeInstanceId">The ID of a node instance. This is a unique identifier for every instance of a node.
        /// Each time a node restarts, even on the same physical node, it gets assigned a new instance ID.</param>
        public FailoverManagerManagerInformation(
            NodeName nodeName = default(NodeName),
            NodeId nodeId = default(NodeId),
            long? nodeInstanceId = default(long?))
        {
            this.NodeName = nodeName;
            this.NodeId = nodeId;
            this.NodeInstanceId = nodeInstanceId;
        }

        /// <summary>
        /// Gets the name of a Service Fabric node.
        /// </summary>
        public NodeName NodeName { get; }

        /// <summary>
        /// Gets an internal ID used by Service Fabric to uniquely identify a node. Node Id is deterministically generated from
        /// node name.
        /// </summary>
        public NodeId NodeId { get; }

        /// <summary>
        /// Gets the ID of a node instance. This is a unique identifier for every instance of a node. Each time a node
        /// restarts, even on the same physical node, it gets assigned a new instance ID.
        /// </summary>
        public long? NodeInstanceId { get; }
    }
}
