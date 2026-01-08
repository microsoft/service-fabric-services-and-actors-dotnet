// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Backup Restore Service Info Event.
    /// </summary>
    public partial class BRSInfoClusterEvent : ClusterEvent
    {
        /// <summary>
        /// Initializes a new instance of the BRSInfoClusterEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="operation">name of operation</param>
        /// <param name="operationStatus">status of operation</param>
        /// <param name="details">details regarding the event</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public BRSInfoClusterEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            string operation,
            string operationStatus,
            string details,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.BRSInfoCluster,
                category,
                hasCorrelatedEvents)
        {
            operation.ThrowIfNull(nameof(operation));
            operationStatus.ThrowIfNull(nameof(operationStatus));
            details.ThrowIfNull(nameof(details));
            this.Operation = operation;
            this.OperationStatus = operationStatus;
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
        /// Gets details regarding the event
        /// </summary>
        public string Details { get; }
    }
}
