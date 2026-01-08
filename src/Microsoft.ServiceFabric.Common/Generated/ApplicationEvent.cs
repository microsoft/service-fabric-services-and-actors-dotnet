// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the base for all Application Events.
    /// </summary>
    public partial class ApplicationEvent : FabricEvent
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="kind">The kind of FabricEvent.</param>
        /// <param name="applicationId">The identity of the application. This is an encoded representation of the application
        /// name. This is used in the REST APIs to identify the application resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the application
        /// name is "fabric:/myapp/app1",
        /// the application identity would be "myapp\~app1" in 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ApplicationEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            FabricEventKind? kind,
            string applicationId,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ApplicationEvent,
                category,
                hasCorrelatedEvents)
        {
            applicationId.ThrowIfNull(nameof(applicationId));
            this.ApplicationId = applicationId;
        }

        /// <summary>
        /// Gets the identity of the application. This is an encoded representation of the application name. This is used in
        /// the REST APIs to identify the application resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the application
        /// name is "fabric:/myapp/app1",
        /// the application identity would be "myapp\~app1" in 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        public string ApplicationId { get; }
    }
}
