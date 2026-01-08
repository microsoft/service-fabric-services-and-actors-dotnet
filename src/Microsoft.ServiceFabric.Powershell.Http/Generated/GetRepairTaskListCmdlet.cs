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
    /// Gets a list of repair tasks matching the given filters.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFRepairTaskList")]
    public partial class GetRepairTaskListCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets TaskIdFilter. The repair task ID prefix to be matched.
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public string TaskIdFilter { get; set; }

        /// <summary>
        /// Gets or sets StateFilter. A bitwise-OR of the following values, specifying which task states should be included in
        /// the result list.
        /// 
        /// - 1 - Created
        /// - 2 - Claimed
        /// - 4 - Preparing
        /// - 8 - Approved
        /// - 16 - Executing
        /// - 32 - Restoring
        /// - 64 - Completed
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public int? StateFilter { get; set; }

        /// <summary>
        /// Gets or sets ExecutorFilter. The name of the repair executor whose claimed tasks should be included in the list.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public string ExecutorFilter { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var result = this.ServiceFabricClient.Repairs.GetRepairTaskListAsync(
                taskIdFilter: this.TaskIdFilter,
                stateFilter: this.StateFilter,
                executorFilter: this.ExecutorFilter,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            if (result != null)
            {
                foreach (var item in result)
                {
                    this.WriteObject(this.FormatOutput(item));
                }
            }
        }
    }
}
