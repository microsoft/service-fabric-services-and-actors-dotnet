// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Node Deactivate Completed event.
    /// </summary>
    public partial class NodeDeactivateCompletedEvent : NodeEvent
    {
        /// <summary>
        /// Initializes a new instance of the NodeDeactivateCompletedEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="nodeInstance">Id of Node instance.</param>
        /// <param name="effectiveDeactivateIntent">Describes deactivate intent.</param>
        /// <param name="batchIdsWithDeactivateIntent">Batch Ids.</param>
        /// <param name="startTime">Start time.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public NodeDeactivateCompletedEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            NodeName nodeName,
            long? nodeInstance,
            string effectiveDeactivateIntent,
            string batchIdsWithDeactivateIntent,
            DateTime? startTime,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.NodeDeactivateCompleted,
                nodeName,
                category,
                hasCorrelatedEvents)
        {
            nodeInstance.ThrowIfNull(nameof(nodeInstance));
            effectiveDeactivateIntent.ThrowIfNull(nameof(effectiveDeactivateIntent));
            batchIdsWithDeactivateIntent.ThrowIfNull(nameof(batchIdsWithDeactivateIntent));
            startTime.ThrowIfNull(nameof(startTime));
            this.NodeInstance = nodeInstance;
            this.EffectiveDeactivateIntent = effectiveDeactivateIntent;
            this.BatchIdsWithDeactivateIntent = batchIdsWithDeactivateIntent;
            this.StartTime = startTime;
        }

        /// <summary>
        /// Gets id of Node instance.
        /// </summary>
        public long? NodeInstance { get; }

        /// <summary>
        /// Gets deactivate intent.
        /// </summary>
        public string EffectiveDeactivateIntent { get; }

        /// <summary>
        /// Gets batch Ids.
        /// </summary>
        public string BatchIdsWithDeactivateIntent { get; }

        /// <summary>
        /// Gets start time.
        /// </summary>
        public DateTime? StartTime { get; }
    }
}
