// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Readiness Probe Failed Event.
    /// </summary>
    public partial class ReadinessProbeFailedEvent : ApplicationEvent
    {
        /// <summary>
        /// Initializes a new instance of the ReadinessProbeFailedEvent class.
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
        /// <param name="containerImage">the Container Image or Exe Name</param>
        /// <param name="hostId">Host Id.</param>
        /// <param name="entryPointType">Type of EntryPoint.</param>
        /// <param name="failureCount">count of Failures</param>
        /// <param name="exitCode">Exit Code of process</param>
        /// <param name="stdOut">the standard output stream</param>
        /// <param name="stdErr">the standard error stream</param>
        /// <param name="errorCode">the error code for the process</param>
        /// <param name="message">message emitted from the failed event</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        /// <param name="codePackageName">Name of Code package.</param>
        /// <param name="containerName">Name of Container.</param>
        public ReadinessProbeFailedEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            string applicationId,
            string serviceName,
            string servicePackageName,
            string servicePackageActivationId,
            bool? isExclusive,
            string containerImage,
            string hostId,
            string entryPointType,
            long? failureCount,
            string exitCode,
            string stdOut,
            string stdErr,
            string errorCode,
            string message,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?),
            string codePackageName = default(string),
            string containerName = default(string))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ReadinessProbeFailed,
                applicationId,
                category,
                hasCorrelatedEvents)
        {
            serviceName.ThrowIfNull(nameof(serviceName));
            servicePackageName.ThrowIfNull(nameof(servicePackageName));
            servicePackageActivationId.ThrowIfNull(nameof(servicePackageActivationId));
            isExclusive.ThrowIfNull(nameof(isExclusive));
            containerImage.ThrowIfNull(nameof(containerImage));
            hostId.ThrowIfNull(nameof(hostId));
            entryPointType.ThrowIfNull(nameof(entryPointType));
            failureCount.ThrowIfNull(nameof(failureCount));
            exitCode.ThrowIfNull(nameof(exitCode));
            stdOut.ThrowIfNull(nameof(stdOut));
            stdErr.ThrowIfNull(nameof(stdErr));
            errorCode.ThrowIfNull(nameof(errorCode));
            message.ThrowIfNull(nameof(message));
            this.ServiceName = serviceName;
            this.ServicePackageName = servicePackageName;
            this.ServicePackageActivationId = servicePackageActivationId;
            this.IsExclusive = isExclusive;
            this.ContainerImage = containerImage;
            this.HostId = hostId;
            this.EntryPointType = entryPointType;
            this.FailureCount = failureCount;
            this.ExitCode = exitCode;
            this.StdOut = stdOut;
            this.StdErr = stdErr;
            this.ErrorCode = errorCode;
            this.Message = message;
            this.CodePackageName = codePackageName;
            this.ContainerName = containerName;
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
        /// Gets the Container Image or Exe Name
        /// </summary>
        public string ContainerImage { get; }

        /// <summary>
        /// Gets name of Code package.
        /// </summary>
        public string CodePackageName { get; }

        /// <summary>
        /// Gets name of Container.
        /// </summary>
        public string ContainerName { get; }

        /// <summary>
        /// Gets host Id.
        /// </summary>
        public string HostId { get; }

        /// <summary>
        /// Gets type of EntryPoint.
        /// </summary>
        public string EntryPointType { get; }

        /// <summary>
        /// Gets count of Failures
        /// </summary>
        public long? FailureCount { get; }

        /// <summary>
        /// Gets exit Code of process
        /// </summary>
        public string ExitCode { get; }

        /// <summary>
        /// Gets the standard output stream
        /// </summary>
        public string StdOut { get; }

        /// <summary>
        /// Gets the standard error stream
        /// </summary>
        public string StdErr { get; }

        /// <summary>
        /// Gets the error code for the process
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Gets message emitted from the failed event
        /// </summary>
        public string Message { get; }
    }
}
