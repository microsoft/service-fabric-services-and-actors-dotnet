// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// DecisionOperational PLB event.
    /// </summary>
    public partial class DecisionEvent : ClusterEvent
    {
        /// <summary>
        /// Initializes a new instance of the DecisionEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="decisionId">Id of decision</param>
        /// <param name="metricString">metrics of this PLB event</param>
        /// <param name="information">more information regarding this event</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public DecisionEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            Guid? decisionId,
            string metricString,
            string information,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.Decision,
                category,
                hasCorrelatedEvents)
        {
            decisionId.ThrowIfNull(nameof(decisionId));
            metricString.ThrowIfNull(nameof(metricString));
            information.ThrowIfNull(nameof(information));
            this.DecisionId = decisionId;
            this.MetricString = metricString;
            this.Information = information;
        }

        /// <summary>
        /// Gets id of decision
        /// </summary>
        public Guid? DecisionId { get; }

        /// <summary>
        /// Gets metrics of this PLB event
        /// </summary>
        public string MetricString { get; }

        /// <summary>
        /// Gets more information regarding this event
        /// </summary>
        public string Information { get; }
    }
}
