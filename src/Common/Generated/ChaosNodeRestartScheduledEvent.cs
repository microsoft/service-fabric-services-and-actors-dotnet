// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Chaos Restart Node Fault Scheduled event.
    /// </summary>
    public partial class ChaosNodeRestartScheduledEvent : NodeEvent
    {
        /// <summary>
        /// Initializes a new instance of the ChaosNodeRestartScheduledEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="nodeInstanceId">Id of Node instance.</param>
        /// <param name="faultGroupId">Id of fault group.</param>
        /// <param name="faultId">Id of fault.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ChaosNodeRestartScheduledEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            NodeName nodeName,
            long? nodeInstanceId,
            Guid? faultGroupId,
            Guid? faultId,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ChaosNodeRestartScheduled,
                nodeName,
                category,
                hasCorrelatedEvents)
        {
            nodeInstanceId.ThrowIfNull(nameof(nodeInstanceId));
            faultGroupId.ThrowIfNull(nameof(faultGroupId));
            faultId.ThrowIfNull(nameof(faultId));
            this.NodeInstanceId = nodeInstanceId;
            this.FaultGroupId = faultGroupId;
            this.FaultId = faultId;
        }

        /// <summary>
        /// Gets id of Node instance.
        /// </summary>
        public long? NodeInstanceId { get; }

        /// <summary>
        /// Gets id of fault group.
        /// </summary>
        public Guid? FaultGroupId { get; }

        /// <summary>
        /// Gets id of fault.
        /// </summary>
        public Guid? FaultId { get; }
    }
}
