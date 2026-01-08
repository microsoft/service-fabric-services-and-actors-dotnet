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
    /// Cancels a user-induced fault operation.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Stop, "SFOperation")]
    public partial class StopOperationCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets OperationId. A GUID that identifies a call of this API.  This is passed into the corresponding
        /// GetProgress API
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public Guid? OperationId { get; set; }

        /// <summary>
        /// Gets or sets Force. Indicates whether to gracefully roll back and clean up internal system state modified by
        /// executing the user-induced operation.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public bool? Force { get; set; }

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
            this.ServiceFabricClient.Faults.CancelOperationAsync(
                operationId: this.OperationId,
                force: this.Force,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
