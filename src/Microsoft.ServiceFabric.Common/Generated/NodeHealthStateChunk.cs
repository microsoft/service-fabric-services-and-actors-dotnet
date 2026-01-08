// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state chunk of a node, which contains the node name and its aggregated health state.
    /// </summary>
    public partial class NodeHealthStateChunk : EntityHealthStateChunk
    {
        /// <summary>
        /// Initializes a new instance of the NodeHealthStateChunk class.
        /// </summary>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        public NodeHealthStateChunk(
            HealthState? healthState = default(HealthState?),
            NodeName nodeName = default(NodeName))
            : base(
                healthState)
        {
            this.NodeName = nodeName;
        }

        /// <summary>
        /// Gets the name of a Service Fabric node.
        /// </summary>
        public NodeName NodeName { get; }
    }
}
