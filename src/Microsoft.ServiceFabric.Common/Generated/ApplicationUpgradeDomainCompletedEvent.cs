// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Application Upgrade Domain Completed event.
    /// </summary>
    public partial class ApplicationUpgradeDomainCompletedEvent : ApplicationEvent
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationUpgradeDomainCompletedEvent class.
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
        /// <param name="upgradeState">State of upgrade.</param>
        /// <param name="upgradeDomains">Upgrade domains.</param>
        /// <param name="upgradeDomainElapsedTimeInMs">Upgrade time of domain in milli-seconds.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ApplicationUpgradeDomainCompletedEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            string applicationId,
            string applicationTypeName,
            string currentApplicationTypeVersion,
            string applicationTypeVersion,
            string upgradeState,
            string upgradeDomains,
            double? upgradeDomainElapsedTimeInMs,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ApplicationUpgradeDomainCompleted,
                applicationId,
                category,
                hasCorrelatedEvents)
        {
            applicationTypeName.ThrowIfNull(nameof(applicationTypeName));
            currentApplicationTypeVersion.ThrowIfNull(nameof(currentApplicationTypeVersion));
            applicationTypeVersion.ThrowIfNull(nameof(applicationTypeVersion));
            upgradeState.ThrowIfNull(nameof(upgradeState));
            upgradeDomains.ThrowIfNull(nameof(upgradeDomains));
            upgradeDomainElapsedTimeInMs.ThrowIfNull(nameof(upgradeDomainElapsedTimeInMs));
            this.ApplicationTypeName = applicationTypeName;
            this.CurrentApplicationTypeVersion = currentApplicationTypeVersion;
            this.ApplicationTypeVersion = applicationTypeVersion;
            this.UpgradeState = upgradeState;
            this.UpgradeDomains = upgradeDomains;
            this.UpgradeDomainElapsedTimeInMs = upgradeDomainElapsedTimeInMs;
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
        /// Gets state of upgrade.
        /// </summary>
        public string UpgradeState { get; }

        /// <summary>
        /// Gets upgrade domains.
        /// </summary>
        public string UpgradeDomains { get; }

        /// <summary>
        /// Gets upgrade time of domain in milli-seconds.
        /// </summary>
        public double? UpgradeDomainElapsedTimeInMs { get; }
    }
}
