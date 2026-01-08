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
    /// Starts rolling back a compose deployment upgrade in the Service Fabric cluster.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "SFRollbackComposeDeploymentUpgrade")]
    public partial class StartRollbackComposeDeploymentUpgradeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets DeploymentName. The identity of the deployment.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string DeploymentName { get; set; }

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
            this.ServiceFabricClient.ComposeDeployments.StartRollbackComposeDeploymentUpgradeAsync(
                deploymentName: this.DeploymentName,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
