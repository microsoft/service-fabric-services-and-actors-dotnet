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
    /// Deactivate a Service Fabric cluster node with the specified deactivation intent.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Disable, "SFNode")]
    public partial class DisableNodeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets NodeName. The name of the node.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public NodeName NodeName { get; set; }

        /// <summary>
        /// Gets or sets DeactivationIntent. Describes the intent or reason for deactivating the node. The possible values are
        /// following.
        /// . Possible values include: 'Pause', 'Restart', 'RemoveData', 'RemoveNode'
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public DeactivationIntent? DeactivationIntent { get; set; }

        /// <summary>
        /// Gets or sets DeactivationDescription. Describes the reason or more information about node deactivation. Maximum 
        /// number of characters allowed is 200.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public string DeactivationDescription { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets the force flag. If provided, then the destructive action will be performed without asking for
        /// confirmation prompt.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public SwitchParameter Force { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var deactivationIntentDescription = new DeactivationIntentDescription(
            deactivationIntent: this.DeactivationIntent,
            deactivationDescription: this.DeactivationDescription);

            if (((this.Force != null) && this.Force) || this.ShouldContinue(string.Empty, string.Empty))
            {
                this.ServiceFabricClient.Nodes.DisableNodeAsync(
                    nodeName: this.NodeName,
                    deactivationIntentDescription: deactivationIntentDescription,
                    serverTimeout: this.ServerTimeout,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                Console.WriteLine("Success!");
            }
        }
    }
}
