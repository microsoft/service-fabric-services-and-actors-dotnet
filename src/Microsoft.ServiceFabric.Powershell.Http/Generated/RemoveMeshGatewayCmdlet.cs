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
    /// Deletes the Gateway resource.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "SFMeshGateway")]
    public partial class RemoveMeshGatewayCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets GatewayResourceName. The identity of the gateway.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string GatewayResourceName { get; set; }

        /// <summary>
        /// Gets or sets the force flag. If provided, then the destructive action will be performed without asking for
        /// confirmation prompt.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public SwitchParameter Force { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (((this.Force != null) && this.Force) || this.ShouldContinue(string.Empty, string.Empty))
            {
                this.ServiceFabricClient.MeshGateways.DeleteAsync(
                    gatewayResourceName: this.GatewayResourceName,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                Console.WriteLine("Success!");
            }
        }
    }
}
