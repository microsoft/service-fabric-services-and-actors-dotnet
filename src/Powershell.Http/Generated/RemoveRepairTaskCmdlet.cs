// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Deletes a completed repair task.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "SFRepairTask")]
    public partial class RemoveRepairTaskCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets TaskId. The ID of the completed repair task to be deleted.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string TaskId { get; set; }

        /// <summary>
        /// Gets or sets Version. The current version number of the repair task. If non-zero, then the request will only
        /// succeed if this value matches the actual current version of the repair task. If zero, then no version check is
        /// performed.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the force flag. If provided, then the destructive action will be performed without asking for
        /// confirmation prompt.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public SwitchParameter Force { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var repairTaskDeleteDescription = new RepairTaskDeleteDescription(
            taskId: this.TaskId,
            version: this.Version);

            if (((this.Force != null) && this.Force) || this.ShouldContinue(string.Empty, string.Empty))
            {
                this.ServiceFabricClient.Repairs.DeleteRepairTaskAsync(
                    repairTaskDeleteDescription: repairTaskDeleteDescription,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                Console.WriteLine("Success!");
            }
        }
    }
}
