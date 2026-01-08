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
    /// Updates the health policy of the given repair task.
    /// </summary>
    [Cmdlet(VerbsData.Update, "SFRepairTaskHealthPolicy")]
    public partial class UpdateRepairTaskHealthPolicyCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets TaskId. The ID of the repair task to be updated.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string TaskId { get; set; }

        /// <summary>
        /// Gets or sets Version. The current version number of the repair task. If non-zero, then the request will only
        /// succeed if this value matches the actual current value of the repair task. If zero, then no version check is
        /// performed.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets PerformPreparingHealthCheck. A boolean indicating if health check is to be performed in the Preparing
        /// stage of the repair task. If not specified the existing value should not be altered. Otherwise, specify the desired
        /// new value.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public bool? PerformPreparingHealthCheck { get; set; }

        /// <summary>
        /// Gets or sets PerformRestoringHealthCheck. A boolean indicating if health check is to be performed in the Restoring
        /// stage of the repair task. If not specified the existing value should not be altered. Otherwise, specify the desired
        /// new value.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public bool? PerformRestoringHealthCheck { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var repairTaskUpdateHealthPolicyDescription = new RepairTaskUpdateHealthPolicyDescription(
            taskId: this.TaskId,
            version: this.Version,
            performPreparingHealthCheck: this.PerformPreparingHealthCheck,
            performRestoringHealthCheck: this.PerformRestoringHealthCheck);

            var result = this.ServiceFabricClient.Repairs.UpdateRepairTaskHealthPolicyAsync(
                repairTaskUpdateHealthPolicyDescription: repairTaskUpdateHealthPolicyDescription,
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
