// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the base for all Container Events.
    /// </summary>
    public partial class ContainerInstanceEvent : FabricEvent
    {
        /// <summary>
        /// Initializes a new instance of the ContainerInstanceEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ContainerInstanceEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ContainerInstanceEvent,
                category,
                hasCorrelatedEvents)
        {
        }
    }
}
