// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state of a node, which contains the node identifier and its aggregated health state.
    /// </summary>
    public partial class NodeHealthState : EntityHealthState
    {
        /// <summary>
        /// Initializes a new instance of the NodeHealthState class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="name">The name of a Service Fabric node.</param>
        /// <param name="id">An internal ID used by Service Fabric to uniquely identify a node. Node Id is deterministically
        /// generated from node name.</param>
        public NodeHealthState(
            HealthState? aggregatedHealthState = default(HealthState?),
            NodeName name = default(NodeName),
            NodeId id = default(NodeId))
            : base(
                aggregatedHealthState)
        {
            this.Name = name;
            this.Id = id;
        }

        /// <summary>
        /// Gets the name of a Service Fabric node.
        /// </summary>
        public NodeName Name { get; }

        /// <summary>
        /// Gets an internal ID used by Service Fabric to uniquely identify a node. Node Id is deterministically generated from
        /// node name.
        /// </summary>
        public NodeId Id { get; }
    }
}
