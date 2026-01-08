// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the base for all Node Events.
    /// </summary>
    public partial class NodeEvent : FabricEvent
    {
        /// <summary>
        /// Initializes a new instance of the NodeEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="kind">The kind of FabricEvent.</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public NodeEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            FabricEventKind? kind,
            NodeName nodeName,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.NodeEvent,
                category,
                hasCorrelatedEvents)
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            this.NodeName = nodeName;
        }

        /// <summary>
        /// Gets the name of a Service Fabric node.
        /// </summary>
        public NodeName NodeName { get; }
    }
}
