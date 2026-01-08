// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Chaos Stopped event.
    /// </summary>
    public partial class ChaosStoppedEvent : ClusterEvent
    {
        /// <summary>
        /// Initializes a new instance of the ChaosStoppedEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="reason">Describes reason.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ChaosStoppedEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            string reason,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ChaosStopped,
                category,
                hasCorrelatedEvents)
        {
            reason.ThrowIfNull(nameof(reason));
            this.Reason = reason;
        }

        /// <summary>
        /// Gets reason.
        /// </summary>
        public string Reason { get; }
    }
}
