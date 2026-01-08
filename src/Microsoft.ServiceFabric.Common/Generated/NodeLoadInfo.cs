// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about load on a Service Fabric node. It holds a summary of all metrics and their load on a node.
    /// </summary>
    public partial class NodeLoadInfo
    {
        /// <summary>
        /// Initializes a new instance of the NodeLoadInfo class.
        /// </summary>
        /// <param name="nodeName">Name of the node for which the load information is provided by this object.</param>
        /// <param name="nodeLoadMetricInformation">List that contains metrics and their load information on this node.</param>
        public NodeLoadInfo(
            NodeName nodeName = default(NodeName),
            IEnumerable<NodeLoadMetricInformation> nodeLoadMetricInformation = default(IEnumerable<NodeLoadMetricInformation>))
        {
            this.NodeName = nodeName;
            this.NodeLoadMetricInformation = nodeLoadMetricInformation;
        }

        /// <summary>
        /// Gets name of the node for which the load information is provided by this object.
        /// </summary>
        public NodeName NodeName { get; }

        /// <summary>
        /// Gets list that contains metrics and their load information on this node.
        /// </summary>
        public IEnumerable<NodeLoadMetricInformation> NodeLoadMetricInformation { get; }
    }
}
