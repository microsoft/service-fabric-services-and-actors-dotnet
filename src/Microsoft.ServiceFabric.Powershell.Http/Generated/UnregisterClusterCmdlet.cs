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
    /// Unprovision the code or configuration packages of a Service Fabric cluster.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Unregister, "SFCluster")]
    public partial class UnregisterClusterCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets CodeVersion. The cluster code package version.
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public string CodeVersion { get; set; }

        /// <summary>
        /// Gets or sets ConfigVersion. The cluster manifest version.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public string ConfigVersion { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets the force flag. If provided, then the destructive action will be performed without asking for
        /// confirmation prompt.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public SwitchParameter Force { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var unprovisionFabricDescription = new UnprovisionFabricDescription(
            codeVersion: this.CodeVersion,
            configVersion: this.ConfigVersion);

            if (((this.Force != null) && this.Force) || this.ShouldContinue(string.Empty, string.Empty))
            {
                this.ServiceFabricClient.Cluster.UnprovisionClusterAsync(
                    unprovisionFabricDescription: unprovisionFabricDescription,
                    serverTimeout: this.ServerTimeout,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                Console.WriteLine("Success!");
            }
        }
    }
}
