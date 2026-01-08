// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the base for all Partition Events.
    /// </summary>
    public partial class PartitionEvent : FabricEvent
    {
        /// <summary>
        /// Initializes a new instance of the PartitionEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="kind">The kind of FabricEvent.</param>
        /// <param name="partitionId">An internal ID used by Service Fabric to uniquely identify a partition. This is a
        /// randomly generated GUID when the service was created. The partition ID is unique and does not change for the
        /// lifetime of the service. If the same service was deleted and recreated the IDs of its partitions would be
        /// different.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public PartitionEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            FabricEventKind? kind,
            PartitionId partitionId,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.PartitionEvent,
                category,
                hasCorrelatedEvents)
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            this.PartitionId = partitionId;
        }

        /// <summary>
        /// Gets an internal ID used by Service Fabric to uniquely identify a partition. This is a randomly generated GUID when
        /// the service was created. The partition ID is unique and does not change for the lifetime of the service. If the
        /// same service was deleted and recreated the IDs of its partitions would be different.
        /// </summary>
        public PartitionId PartitionId { get; }
    }
}
