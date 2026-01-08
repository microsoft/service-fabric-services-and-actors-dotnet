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
    /// Requests the cancellation of the given repair task.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Stop, "SFRepairTask")]
    public partial class StopRepairTaskCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets TaskId. The ID of the repair task.
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
        /// Gets or sets RequestAbort. _True_ if the repair should be stopped as soon as possible even if it has already
        /// started executing. _False_ if the repair should be cancelled only if execution has not yet started.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public bool? RequestAbort { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var repairTaskCancelDescription = new RepairTaskCancelDescription(
            taskId: this.TaskId,
            version: this.Version,
            requestAbort: this.RequestAbort);

            var result = this.ServiceFabricClient.Repairs.CancelRepairTaskAsync(
                repairTaskCancelDescription: repairTaskCancelDescription,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            if (result != null)
            {
                this.WriteObject(this.FormatOutput(result));
            }
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            return output;
        }
    }
}
