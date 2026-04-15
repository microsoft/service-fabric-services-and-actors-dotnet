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
    /// Approves or acts on an impact approval object.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "SFImpactAction")]
    public partial class InvokeImpactActionCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ImpactId. The unique ID (GUID) of the impact approval object to act on.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid? ImpactId { get; set; }

        /// <summary>
        /// Gets or sets ImpactAction. The action to perform on the impact object. Possible values include: 'Unknown',
        /// 'Approve'
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public ImpactActionKind? ImpactAction { get; set; }

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
            this.ServiceFabricClient.Cluster.InvokeImpactActionAsync(
                impactId: this.ImpactId,
                impactAction: this.ImpactAction,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
