// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Chaos Move Primary Fault Scheduled event.
    /// </summary>
    public partial class ChaosPartitionPrimaryMoveScheduledEvent : PartitionEvent
    {
        /// <summary>
        /// Initializes a new instance of the ChaosPartitionPrimaryMoveScheduledEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="partitionId">An internal ID used by Service Fabric to uniquely identify a partition. This is a
        /// randomly generated GUID when the service was created. The partition ID is unique and does not change for the
        /// lifetime of the service. If the same service was deleted and recreated the IDs of its partitions would be
        /// different.</param>
        /// <param name="faultGroupId">Id of fault group.</param>
        /// <param name="faultId">Id of fault.</param>
        /// <param name="serviceName">Service name.</param>
        /// <param name="nodeTo">The name of a Service Fabric node.</param>
        /// <param name="forcedMove">Indicates a forced move.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ChaosPartitionPrimaryMoveScheduledEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            PartitionId partitionId,
            Guid? faultGroupId,
            Guid? faultId,
            string serviceName,
            NodeName nodeTo,
            bool? forcedMove,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ChaosPartitionPrimaryMoveScheduled,
                partitionId,
                category,
                hasCorrelatedEvents)
        {
            faultGroupId.ThrowIfNull(nameof(faultGroupId));
            faultId.ThrowIfNull(nameof(faultId));
            serviceName.ThrowIfNull(nameof(serviceName));
            nodeTo.ThrowIfNull(nameof(nodeTo));
            forcedMove.ThrowIfNull(nameof(forcedMove));
            this.FaultGroupId = faultGroupId;
            this.FaultId = faultId;
            this.ServiceName = serviceName;
            this.NodeTo = nodeTo;
            this.ForcedMove = forcedMove;
        }

        /// <summary>
        /// Gets id of fault group.
        /// </summary>
        public Guid? FaultGroupId { get; }

        /// <summary>
        /// Gets id of fault.
        /// </summary>
        public Guid? FaultId { get; }

        /// <summary>
        /// Gets service name.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets the name of a Service Fabric node.
        /// </summary>
        public NodeName NodeTo { get; }

        /// <summary>
        /// Gets indicates a forced move.
        /// </summary>
        public bool? ForcedMove { get; }
    }
}
