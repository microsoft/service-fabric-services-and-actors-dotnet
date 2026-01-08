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
    /// Changes the verbosity of service placement health reporting.
    /// </summary>
    [Cmdlet(VerbsCommon.Switch, "SFVerboseServicePlacementHealthReporting")]
    public partial class SwitchVerboseServicePlacementHealthReportingCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Enabled. The verbosity of service placement health reporting.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public bool? Enabled { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            this.ServiceFabricClient.Cluster.ToggleVerboseServicePlacementHealthReportingAsync(
                enabled: this.Enabled,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
