// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the base for all Service Events.
    /// </summary>
    public partial class ServiceEvent : FabricEvent
    {
        /// <summary>
        /// Initializes a new instance of the ServiceEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="kind">The kind of FabricEvent.</param>
        /// <param name="serviceId">The identity of the service. This ID is an encoded representation of the service name. This
        /// is used in the REST APIs to identify the service resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the service name
        /// is "fabric:/myapp/app1/svc1",
        /// the service identity would be "myapp~app1\~svc1" in 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ServiceEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            FabricEventKind? kind,
            string serviceId,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ServiceEvent,
                category,
                hasCorrelatedEvents)
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            this.ServiceId = serviceId;
        }

        /// <summary>
        /// Gets the identity of the service. This ID is an encoded representation of the service name. This is used in the
        /// REST APIs to identify the service resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the service name
        /// is "fabric:/myapp/app1/svc1",
        /// the service identity would be "myapp~app1\~svc1" in 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </summary>
        public string ServiceId { get; }
    }
}
