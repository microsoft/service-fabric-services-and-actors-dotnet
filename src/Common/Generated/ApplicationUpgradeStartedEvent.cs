// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Application Upgrade Started event.
    /// </summary>
    public partial class ApplicationUpgradeStartedEvent : ApplicationEvent
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationUpgradeStartedEvent class.
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
        /// <param name="upgradeType">Type of upgrade.</param>
        /// <param name="rollingUpgradeMode">Mode of upgrade.</param>
        /// <param name="failureAction">Action if failed.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ApplicationUpgradeStartedEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            string applicationId,
            string applicationTypeName,
            string currentApplicationTypeVersion,
            string applicationTypeVersion,
            string upgradeType,
            string rollingUpgradeMode,
            string failureAction,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ApplicationUpgradeStarted,
                applicationId,
                category,
                hasCorrelatedEvents)
        {
            applicationTypeName.ThrowIfNull(nameof(applicationTypeName));
            currentApplicationTypeVersion.ThrowIfNull(nameof(currentApplicationTypeVersion));
            applicationTypeVersion.ThrowIfNull(nameof(applicationTypeVersion));
            upgradeType.ThrowIfNull(nameof(upgradeType));
            rollingUpgradeMode.ThrowIfNull(nameof(rollingUpgradeMode));
            failureAction.ThrowIfNull(nameof(failureAction));
            this.ApplicationTypeName = applicationTypeName;
            this.CurrentApplicationTypeVersion = currentApplicationTypeVersion;
            this.ApplicationTypeVersion = applicationTypeVersion;
            this.UpgradeType = upgradeType;
            this.RollingUpgradeMode = rollingUpgradeMode;
            this.FailureAction = failureAction;
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
        /// Gets type of upgrade.
        /// </summary>
        public string UpgradeType { get; }

        /// <summary>
        /// Gets mode of upgrade.
        /// </summary>
        public string RollingUpgradeMode { get; }

        /// <summary>
        /// Gets action if failed.
        /// </summary>
        public string FailureAction { get; }
    }
}
