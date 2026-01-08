// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Chaos Restart Code Package Fault Scheduled event.
    /// </summary>
    public partial class ChaosCodePackageRestartScheduledEvent : ApplicationEvent
    {
        /// <summary>
        /// Initializes a new instance of the ChaosCodePackageRestartScheduledEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="applicationId">The identity of the application. This is an encoded representation of the application
        /// name. This is used in the REST APIs to identify the application resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the application
        /// name is "fabric:/myapp/app1",
        /// the application identity would be "myapp\~app1" in 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name="faultGroupId">Id of fault group.</param>
        /// <param name="faultId">Id of fault.</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="serviceManifestName">Service manifest name.</param>
        /// <param name="codePackageName">Code package name.</param>
        /// <param name="servicePackageActivationId">Id of Service package activation.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ChaosCodePackageRestartScheduledEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            string applicationId,
            Guid? faultGroupId,
            Guid? faultId,
            NodeName nodeName,
            string serviceManifestName,
            string codePackageName,
            string servicePackageActivationId,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ChaosCodePackageRestartScheduled,
                applicationId,
                category,
                hasCorrelatedEvents)
        {
            faultGroupId.ThrowIfNull(nameof(faultGroupId));
            faultId.ThrowIfNull(nameof(faultId));
            nodeName.ThrowIfNull(nameof(nodeName));
            serviceManifestName.ThrowIfNull(nameof(serviceManifestName));
            codePackageName.ThrowIfNull(nameof(codePackageName));
            servicePackageActivationId.ThrowIfNull(nameof(servicePackageActivationId));
            this.FaultGroupId = faultGroupId;
            this.FaultId = faultId;
            this.NodeName = nodeName;
            this.ServiceManifestName = serviceManifestName;
            this.CodePackageName = codePackageName;
            this.ServicePackageActivationId = servicePackageActivationId;
        }

        /// <summary>
        /// Gets id of fault group.
        /// </summary>
        public Guid? FaultGroupId { get; }

        /// <summary>
        /// Gets id of fault.
        /// </summary>
        public Guid? FaultId { get; }

        /// <summary>
        /// Gets the name of a Service Fabric node.
        /// </summary>
        public NodeName NodeName { get; }

        /// <summary>
        /// Gets service manifest name.
        /// </summary>
        public string ServiceManifestName { get; }

        /// <summary>
        /// Gets code package name.
        /// </summary>
        public string CodePackageName { get; }

        /// <summary>
        /// Gets id of Service package activation.
        /// </summary>
        public string ServicePackageActivationId { get; }
    }
}
