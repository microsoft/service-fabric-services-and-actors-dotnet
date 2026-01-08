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
    /// Gets a list of user-induced fault operations filtered by provided input.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFFaultOperationList")]
    public partial class GetFaultOperationListCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets TypeFilter. Used to filter on OperationType for user-induced operations.
        /// 
        /// - 65535 - select all
        /// - 1 - select PartitionDataLoss.
        /// - 2 - select PartitionQuorumLoss.
        /// - 4 - select PartitionRestart.
        /// - 8 - select NodeTransition.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public int? TypeFilter { get; set; }

        /// <summary>
        /// Gets or sets StateFilter. Used to filter on OperationState's for user-induced operations.
        /// 
        /// - 65535 - select All
        /// - 1 - select Running
        /// - 2 - select RollingBack
        /// - 8 - select Completed
        /// - 16 - select Faulted
        /// - 32 - select Cancelled
        /// - 64 - select ForceCancelled
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public int? StateFilter { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var result = this.ServiceFabricClient.Faults.GetFaultOperationListAsync(
                typeFilter: this.TypeFilter,
                stateFilter: this.StateFilter,
                serverTimeout: this.ServerTimeout,
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
