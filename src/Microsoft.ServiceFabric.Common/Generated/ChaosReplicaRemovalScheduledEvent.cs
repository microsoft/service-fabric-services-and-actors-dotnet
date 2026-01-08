// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Chaos Remove Replica Fault Scheduled event.
    /// </summary>
    public partial class ChaosReplicaRemovalScheduledEvent : ReplicaEvent
    {
        /// <summary>
        /// Initializes a new instance of the ChaosReplicaRemovalScheduledEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="partitionId">An internal ID used by Service Fabric to uniquely identify a partition. This is a
        /// randomly generated GUID when the service was created. The partition ID is unique and does not change for the
        /// lifetime of the service. If the same service was deleted and recreated the IDs of its partitions would be
        /// different.</param>
        /// <param name="replicaId">Id of a stateful service replica. ReplicaId is used by Service Fabric to uniquely identify
        /// a replica of a partition. It is unique within a partition and does not change for the lifetime of the replica. If a
        /// replica gets dropped and another replica gets created on the same node for the same partition, it will get a
        /// different value for the id. Sometimes the id of a stateless service instance is also referred as a replica
        /// id.</param>
        /// <param name="faultGroupId">Id of fault group.</param>
        /// <param name="faultId">Id of fault.</param>
        /// <param name="serviceUri">Service name.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ChaosReplicaRemovalScheduledEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            PartitionId partitionId,
            ReplicaId replicaId,
            Guid? faultGroupId,
            Guid? faultId,
            string serviceUri,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ChaosReplicaRemovalScheduled,
                partitionId,
                replicaId,
                category,
                hasCorrelatedEvents)
        {
            faultGroupId.ThrowIfNull(nameof(faultGroupId));
            faultId.ThrowIfNull(nameof(faultId));
            serviceUri.ThrowIfNull(nameof(serviceUri));
            this.FaultGroupId = faultGroupId;
            this.FaultId = faultId;
            this.ServiceUri = serviceUri;
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
        public string ServiceUri { get; }
    }
}
