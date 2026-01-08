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
    /// Enables periodic backup of stateful partitions under this Service Fabric application.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Enable, "SFApplicationBackup")]
    public partial class EnableApplicationBackupCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ApplicationId. The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public string ApplicationId { get; set; }

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

            this.ServiceFabricClient.BackupRestore.EnableApplicationBackupAsync(
                applicationId: this.ApplicationId,
                enableBackupDescription: enableBackupDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
