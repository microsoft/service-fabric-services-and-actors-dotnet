// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Node Up event.
    /// </summary>
    public partial class NodeUpEvent : NodeEvent
    {
        /// <summary>
        /// Initializes a new instance of the NodeUpEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="nodeInstance">Id of Node instance.</param>
        /// <param name="lastNodeDownAt">Time when Node was last down.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public NodeUpEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            NodeName nodeName,
            long? nodeInstance,
            DateTime? lastNodeDownAt,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.NodeUp,
                nodeName,
                category,
                hasCorrelatedEvents)
        {
            nodeInstance.ThrowIfNull(nameof(nodeInstance));
            lastNodeDownAt.ThrowIfNull(nameof(lastNodeDownAt));
            this.NodeInstance = nodeInstance;
            this.LastNodeDownAt = lastNodeDownAt;
        }

        /// <summary>
        /// Gets id of Node instance.
        /// </summary>
        public long? NodeInstance { get; }

        /// <summary>
        /// Gets time when Node was last down.
        /// </summary>
        public DateTime? LastNodeDownAt { get; }
    }
}
