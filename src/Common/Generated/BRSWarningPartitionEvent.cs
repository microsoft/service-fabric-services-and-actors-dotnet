// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Backup Restore Service Warning Event.
    /// </summary>
    public partial class BRSWarningPartitionEvent : PartitionEvent
    {
        /// <summary>
        /// Initializes a new instance of the BRSWarningPartitionEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="partitionId">An internal ID used by Service Fabric to uniquely identify a partition. This is a
        /// randomly generated GUID when the service was created. The partition ID is unique and does not change for the
        /// lifetime of the service. If the same service was deleted and recreated the IDs of its partitions would be
        /// different.</param>
        /// <param name="operation">name of operation</param>
        /// <param name="operationStatus">status of operation</param>
        /// <param name="operationId">the Id of the operation</param>
        /// <param name="details">details regarding the event</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public BRSWarningPartitionEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            PartitionId partitionId,
            string operation,
            string operationStatus,
            Guid? operationId,
            string details,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.BRSWarningPartition,
                partitionId,
                category,
                hasCorrelatedEvents)
        {
            operation.ThrowIfNull(nameof(operation));
            operationStatus.ThrowIfNull(nameof(operationStatus));
            operationId.ThrowIfNull(nameof(operationId));
            details.ThrowIfNull(nameof(details));
            this.Operation = operation;
            this.OperationStatus = operationStatus;
            this.OperationId = operationId;
            this.Details = details;
        }

        /// <summary>
        /// Gets name of operation
        /// </summary>
        public string Operation { get; }

        /// <summary>
        /// Gets status of operation
        /// </summary>
        public string OperationStatus { get; }

        /// <summary>
        /// Gets the Id of the operation
        /// </summary>
        public Guid? OperationId { get; }

        /// <summary>
        /// Gets details regarding the event
        /// </summary>
        public string Details { get; }
    }
}
