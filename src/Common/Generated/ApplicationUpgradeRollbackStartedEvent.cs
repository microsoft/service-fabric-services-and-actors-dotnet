// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Application Upgrade Rollback Started event.
    /// </summary>
    public partial class ApplicationUpgradeRollbackStartedEvent : ApplicationEvent
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationUpgradeRollbackStartedEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="applicationId">The identity of the application. This is an encoded representation of the application
        /// name. This is used in the REST APIs to identify the application resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the application
        /// name is "fabric:/myapp/app1",
        /// the application identity would be "myapp\~app1" in 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name="applicationTypeName">Application type name.</param>
        /// <param name="currentApplicationTypeVersion">Current Application type version.</param>
        /// <param name="applicationTypeVersion">Target Application type version.</param>
        /// <param name="failureReason">Describes reason of failure.</param>
        /// <param name="overallUpgradeElapsedTimeInMs">Overall upgrade time in milli-seconds.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ApplicationUpgradeRollbackStartedEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            string applicationId,
            string applicationTypeName,
            string currentApplicationTypeVersion,
            string applicationTypeVersion,
            string failureReason,
            double? overallUpgradeElapsedTimeInMs,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ApplicationUpgradeRollbackStarted,
                applicationId,
                category,
                hasCorrelatedEvents)
        {
            applicationTypeName.ThrowIfNull(nameof(applicationTypeName));
            currentApplicationTypeVersion.ThrowIfNull(nameof(currentApplicationTypeVersion));
            applicationTypeVersion.ThrowIfNull(nameof(applicationTypeVersion));
            failureReason.ThrowIfNull(nameof(failureReason));
            overallUpgradeElapsedTimeInMs.ThrowIfNull(nameof(overallUpgradeElapsedTimeInMs));
            this.ApplicationTypeName = applicationTypeName;
            this.CurrentApplicationTypeVersion = currentApplicationTypeVersion;
            this.ApplicationTypeVersion = applicationTypeVersion;
            this.FailureReason = failureReason;
            this.OverallUpgradeElapsedTimeInMs = overallUpgradeElapsedTimeInMs;
        }

        /// <summary>
        /// Gets application type name.
        /// </summary>
        public string ApplicationTypeName { get; }

        /// <summary>
        /// Gets current Application type version.
        /// </summary>
        public string CurrentApplicationTypeVersion { get; }

        /// <summary>
        /// Gets target Application type version.
        /// </summary>
        public string ApplicationTypeVersion { get; }

        /// <summary>
        /// Gets reason of failure.
        /// </summary>
        public string FailureReason { get; }

        /// <summary>
        /// Gets overall upgrade time in milli-seconds.
        /// </summary>
        public double? OverallUpgradeElapsedTimeInMs { get; }
    }
}
