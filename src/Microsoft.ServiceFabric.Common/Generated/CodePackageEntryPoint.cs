// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about setup or main entry point of a code package deployed on a Service Fabric node.
    /// </summary>
    public partial class CodePackageEntryPoint
    {
        /// <summary>
        /// Initializes a new instance of the CodePackageEntryPoint class.
        /// </summary>
        /// <param name="entryPointLocation">The location of entry point executable on the node.</param>
        /// <param name="processId">The process ID of the entry point.</param>
        /// <param name="runAsUserName">The user name under which entry point executable is run on the node.</param>
        /// <param name="codePackageEntryPointStatistics">Statistics about setup or main entry point  of a code package
        /// deployed on a Service Fabric node.</param>
        /// <param name="status">Specifies the status of the code package entry point deployed on a Service Fabric node.
        /// Possible values include: 'Invalid', 'Pending', 'Starting', 'Started', 'Stopping', 'Stopped'</param>
        /// <param name="nextActivationTime">The time (in UTC) when the entry point executable will be run next.</param>
        /// <param name="instanceId">The instance ID for current running entry point. For a code package setup entry point (if
        /// specified) runs first and after it finishes main entry point is started. Each time entry point executable is run,
        /// its instance id will change.</param>
        /// <param name="containerId">The container ID of the entry point. Only valid for container hosts.</param>
        public CodePackageEntryPoint(
            string entryPointLocation = default(string),
            string processId = default(string),
            string runAsUserName = default(string),
            CodePackageEntryPointStatistics codePackageEntryPointStatistics = default(CodePackageEntryPointStatistics),
            EntryPointStatus? status = default(EntryPointStatus?),
            DateTime? nextActivationTime = default(DateTime?),
            string instanceId = default(string),
            string containerId = default(string))
        {
            this.EntryPointLocation = entryPointLocation;
            this.ProcessId = processId;
            this.RunAsUserName = runAsUserName;
            this.CodePackageEntryPointStatistics = codePackageEntryPointStatistics;
            this.Status = status;
            this.NextActivationTime = nextActivationTime;
            this.InstanceId = instanceId;
            this.ContainerId = containerId;
        }

        /// <summary>
        /// Gets the location of entry point executable on the node.
        /// </summary>
        public string EntryPointLocation { get; }

        /// <summary>
        /// Gets the process ID of the entry point.
        /// </summary>
        public string ProcessId { get; }

        /// <summary>
        /// Gets the user name under which entry point executable is run on the node.
        /// </summary>
        public string RunAsUserName { get; }

        /// <summary>
        /// Gets statistics about setup or main entry point  of a code package deployed on a Service Fabric node.
        /// </summary>
        public CodePackageEntryPointStatistics CodePackageEntryPointStatistics { get; }

        /// <summary>
        /// Gets specifies the status of the code package entry point deployed on a Service Fabric node. Possible values
        /// include: 'Invalid', 'Pending', 'Starting', 'Started', 'Stopping', 'Stopped'
        /// </summary>
        public EntryPointStatus? Status { get; }

        /// <summary>
        /// Gets the time (in UTC) when the entry point executable will be run next.
        /// </summary>
        public DateTime? NextActivationTime { get; }

        /// <summary>
        /// Gets the instance ID for current running entry point. For a code package setup entry point (if specified) runs
        /// first and after it finishes main entry point is started. Each time entry point executable is run, its instance id
        /// will change.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        /// Gets the container ID of the entry point. Only valid for container hosts.
        /// </summary>
        public string ContainerId { get; }
    }
}
