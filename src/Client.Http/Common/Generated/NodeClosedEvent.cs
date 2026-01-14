// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Node Closed event.
    /// </summary>
    public partial class NodeClosedEvent : NodeEvent
    {
        /// <summary>
        /// Initializes a new instance of the NodeClosedEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="nodeId">Id of Node.</param>
        /// <param name="nodeInstance">Id of Node instance.</param>
        /// <param name="error">Describes error.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public NodeClosedEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            NodeName nodeName,
            string nodeId,
            long? nodeInstance,
            string error,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.NodeClosed,
                nodeName,
                category,
                hasCorrelatedEvents)
        {
            nodeId.ThrowIfNull(nameof(nodeId));
            nodeInstance.ThrowIfNull(nameof(nodeInstance));
            error.ThrowIfNull(nameof(error));
            this.NodeId = nodeId;
            this.NodeInstance = nodeInstance;
            this.Error = error;
        }

        /// <summary>
        /// Gets id of Node.
        /// </summary>
        public string NodeId { get; }

        /// <summary>
        /// Gets id of Node instance.
        /// </summary>
        public long? NodeInstance { get; }

        /// <summary>
        /// Gets error.
        /// </summary>
        public string Error { get; }
    }
}
