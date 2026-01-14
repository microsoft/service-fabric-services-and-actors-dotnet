// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Container Exited event.
    /// </summary>
    public partial class ApplicationContainerInstanceExitedEvent : ApplicationEvent
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationContainerInstanceExitedEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="applicationId">The identity of the application. This is an encoded representation of the application
        /// name. This is used in the REST APIs to identify the application resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the application
        /// name is "fabric:/myapp/app1",
        /// the application identity would be "myapp\~app1" in 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name="serviceName">Name of Service.</param>
        /// <param name="servicePackageName">Name of Service package.</param>
        /// <param name="servicePackageActivationId">Activation Id of Service package.</param>
        /// <param name="isExclusive">Indicates IsExclusive flag.</param>
        /// <param name="codePackageName">Name of Code package.</param>
        /// <param name="entryPointType">Type of EntryPoint.</param>
        /// <param name="imageName">Name of Container image.</param>
        /// <param name="containerName">Name of Container.</param>
        /// <param name="hostId">Host Id.</param>
        /// <param name="exitCode">Exit code of process.</param>
        /// <param name="unexpectedTermination">Indicates if termination is unexpected.</param>
        /// <param name="startTime">Start time of process.</param>
        /// <param name="nodeId">An internal ID used by Service Fabric to uniquely identify a node. Node Id is
        /// deterministically generated from node name.</param>
        /// <param name="nodeInstanceId">the ID of a Node Instance</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        /// <param name="exitReason">reason for exit</param>
        public ApplicationContainerInstanceExitedEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            string applicationId,
            string serviceName,
            string servicePackageName,
            string servicePackageActivationId,
            bool? isExclusive,
            string codePackageName,
            string entryPointType,
            string imageName,
            string containerName,
            string hostId,
            long? exitCode,
            bool? unexpectedTermination,
            DateTime? startTime,
            NodeId nodeId,
            long? nodeInstanceId,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?),
            string exitReason = default(string))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ApplicationContainerInstanceExited,
                applicationId,
                category,
                hasCorrelatedEvents)
        {
            serviceName.ThrowIfNull(nameof(serviceName));
            servicePackageName.ThrowIfNull(nameof(servicePackageName));
            servicePackageActivationId.ThrowIfNull(nameof(servicePackageActivationId));
            isExclusive.ThrowIfNull(nameof(isExclusive));
            codePackageName.ThrowIfNull(nameof(codePackageName));
            entryPointType.ThrowIfNull(nameof(entryPointType));
            imageName.ThrowIfNull(nameof(imageName));
            containerName.ThrowIfNull(nameof(containerName));
            hostId.ThrowIfNull(nameof(hostId));
            exitCode.ThrowIfNull(nameof(exitCode));
            unexpectedTermination.ThrowIfNull(nameof(unexpectedTermination));
            startTime.ThrowIfNull(nameof(startTime));
            nodeId.ThrowIfNull(nameof(nodeId));
            nodeInstanceId.ThrowIfNull(nameof(nodeInstanceId));
            this.ServiceName = serviceName;
            this.ServicePackageName = servicePackageName;
            this.ServicePackageActivationId = servicePackageActivationId;
            this.IsExclusive = isExclusive;
            this.CodePackageName = codePackageName;
            this.EntryPointType = entryPointType;
            this.ImageName = imageName;
            this.ContainerName = containerName;
            this.HostId = hostId;
            this.ExitCode = exitCode;
            this.UnexpectedTermination = unexpectedTermination;
            this.StartTime = startTime;
            this.NodeId = nodeId;
            this.NodeInstanceId = nodeInstanceId;
            this.ExitReason = exitReason;
        }

        /// <summary>
        /// Gets name of Service.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets name of Service package.
        /// </summary>
        public string ServicePackageName { get; }

        /// <summary>
        /// Gets activation Id of Service package.
        /// </summary>
        public string ServicePackageActivationId { get; }

        /// <summary>
        /// Gets indicates IsExclusive flag.
        /// </summary>
        public bool? IsExclusive { get; }

        /// <summary>
        /// Gets name of Code package.
        /// </summary>
        public string CodePackageName { get; }

        /// <summary>
        /// Gets type of EntryPoint.
        /// </summary>
        public string EntryPointType { get; }

        /// <summary>
        /// Gets name of Container image.
        /// </summary>
        public string ImageName { get; }

        /// <summary>
        /// Gets name of Container.
        /// </summary>
        public string ContainerName { get; }

        /// <summary>
        /// Gets host Id.
        /// </summary>
        public string HostId { get; }

        /// <summary>
        /// Gets exit code of process.
        /// </summary>
        public long? ExitCode { get; }

        /// <summary>
        /// Gets indicates if termination is unexpected.
        /// </summary>
        public bool? UnexpectedTermination { get; }

        /// <summary>
        /// Gets start time of process.
        /// </summary>
        public DateTime? StartTime { get; }

        /// <summary>
        /// Gets reason for exit
        /// </summary>
        public string ExitReason { get; }

        /// <summary>
        /// Gets an internal ID used by Service Fabric to uniquely identify a node. Node Id is deterministically generated from
        /// node name.
        /// </summary>
        public NodeId NodeId { get; }

        /// <summary>
        /// Gets the ID of a Node Instance
        /// </summary>
        public long? NodeInstanceId { get; }
    }
}
