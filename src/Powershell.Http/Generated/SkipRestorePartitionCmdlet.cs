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
    /// Triggers Skipping of Restore of  partition if partition is stuck on trying to Restore and user wants to get out of
    /// OnDataLossAsync and continue with the replica data instead of restoring.
    /// </summary>
    [Cmdlet(VerbsCommon.Skip, "SFRestorePartition")]
    public partial class SkipRestorePartitionCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets PartitionId. The identity of the partition.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public PartitionId PartitionId { get; set; }

        /// <summary>
        /// Gets or sets RestoreTimeout. Specifies the maximum amount of time to wait, in minutes, for the Skip restore
        /// operation to complete. Post that, the operation returns back with timeout error. However, in certain corner cases
        /// it could be that the restore operation goes through even though it completes with timeout. In case of timeout
        /// error, its recommended to invoke this operation again with a greater timeout value. the default value for the same
        /// is 10 minutes.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public int? RestoreTimeout { get; set; }

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
            this.ServiceFabricClient.BackupRestore.SkipRestorePartitionAsync(
                partitionId: this.PartitionId,
                restoreTimeout: this.RestoreTimeout,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
