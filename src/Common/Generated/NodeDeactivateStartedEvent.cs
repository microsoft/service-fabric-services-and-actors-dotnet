// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Node Deactivate Started event.
    /// </summary>
    public partial class NodeDeactivateStartedEvent : NodeEvent
    {
        /// <summary>
        /// Initializes a new instance of the NodeDeactivateStartedEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="nodeInstance">Id of Node instance.</param>
        /// <param name="batchId">Batch Id.</param>
        /// <param name="deactivateIntent">Describes deactivate intent.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public NodeDeactivateStartedEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            NodeName nodeName,
            long? nodeInstance,
            string batchId,
            string deactivateIntent,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.NodeDeactivateStarted,
                nodeName,
                category,
                hasCorrelatedEvents)
        {
            nodeInstance.ThrowIfNull(nameof(nodeInstance));
            batchId.ThrowIfNull(nameof(batchId));
            deactivateIntent.ThrowIfNull(nameof(deactivateIntent));
            this.NodeInstance = nodeInstance;
            this.BatchId = batchId;
            this.DeactivateIntent = deactivateIntent;
        }

        /// <summary>
        /// Gets id of Node instance.
        /// </summary>
        public long? NodeInstance { get; }

        /// <summary>
        /// Gets batch Id.
        /// </summary>
        public string BatchId { get; }

        /// <summary>
        /// Gets deactivate intent.
        /// </summary>
        public string DeactivateIntent { get; }
    }
}
