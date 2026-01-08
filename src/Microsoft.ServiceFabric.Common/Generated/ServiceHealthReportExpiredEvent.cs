// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Service Health Report Expired event.
    /// </summary>
    public partial class ServiceHealthReportExpiredEvent : ServiceEvent
    {
        /// <summary>
        /// Initializes a new instance of the ServiceHealthReportExpiredEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="serviceId">The identity of the service. This ID is an encoded representation of the service name. This
        /// is used in the REST APIs to identify the service resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the service name
        /// is "fabric:/myapp/app1/svc1",
        /// the service identity would be "myapp~app1\~svc1" in 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name="instanceId">Id of Service instance.</param>
        /// <param name="sourceId">Id of report source.</param>
        /// <param name="property">Describes the property.</param>
        /// <param name="healthState">Describes the property health state.</param>
        /// <param name="timeToLiveMs">Time to live in milli-seconds.</param>
        /// <param name="sequenceNumber">Sequence number of report.</param>
        /// <param name="description">Description of report.</param>
        /// <param name="removeWhenExpired">Indicates the removal when it expires.</param>
        /// <param name="sourceUtcTimestamp">Source time.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ServiceHealthReportExpiredEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            string serviceId,
            long? instanceId,
            string sourceId,
            string property,
            string healthState,
            long? timeToLiveMs,
            long? sequenceNumber,
            string description,
            bool? removeWhenExpired,
            DateTime? sourceUtcTimestamp,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ServiceHealthReportExpired,
                serviceId,
                category,
                hasCorrelatedEvents)
        {
            instanceId.ThrowIfNull(nameof(instanceId));
            sourceId.ThrowIfNull(nameof(sourceId));
            property.ThrowIfNull(nameof(property));
            healthState.ThrowIfNull(nameof(healthState));
            timeToLiveMs.ThrowIfNull(nameof(timeToLiveMs));
            sequenceNumber.ThrowIfNull(nameof(sequenceNumber));
            description.ThrowIfNull(nameof(description));
            removeWhenExpired.ThrowIfNull(nameof(removeWhenExpired));
            sourceUtcTimestamp.ThrowIfNull(nameof(sourceUtcTimestamp));
            this.InstanceId = instanceId;
            this.SourceId = sourceId;
            this.Property = property;
            this.HealthState = healthState;
            this.TimeToLiveMs = timeToLiveMs;
            this.SequenceNumber = sequenceNumber;
            this.Description = description;
            this.RemoveWhenExpired = removeWhenExpired;
            this.SourceUtcTimestamp = sourceUtcTimestamp;
        }

        /// <summary>
        /// Gets id of Service instance.
        /// </summary>
        public long? InstanceId { get; }

        /// <summary>
        /// Gets id of report source.
        /// </summary>
        public string SourceId { get; }

        /// <summary>
        /// Gets the property.
        /// </summary>
        public string Property { get; }

        /// <summary>
        /// Gets the property health state.
        /// </summary>
        public string HealthState { get; }

        /// <summary>
        /// Gets time to live in milli-seconds.
        /// </summary>
        public long? TimeToLiveMs { get; }

        /// <summary>
        /// Gets sequence number of report.
        /// </summary>
        public long? SequenceNumber { get; }

        /// <summary>
        /// Gets description of report.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets indicates the removal when it expires.
        /// </summary>
        public bool? RemoveWhenExpired { get; }

        /// <summary>
        /// Gets source time.
        /// </summary>
        public DateTime? SourceUtcTimestamp { get; }
    }
}
