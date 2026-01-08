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
    /// Enables periodic backup of the stateful persisted partition.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Enable, "SFPartitionBackup")]
    public partial class EnablePartitionBackupCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets PartitionId. The identity of the partition.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public PartitionId PartitionId { get; set; }

        /// <summary>
        /// Gets or sets BackupPolicyName. Name of the backup policy to be used for enabling periodic backups.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string BackupPolicyName { get; set; }

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
            var enableBackupDescription = new EnableBackupDescription(
            backupPolicyName: this.BackupPolicyName);

            this.ServiceFabricClient.BackupRestore.EnablePartitionBackupAsync(
                partitionId: this.PartitionId,
                enableBackupDescription: enableBackupDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
