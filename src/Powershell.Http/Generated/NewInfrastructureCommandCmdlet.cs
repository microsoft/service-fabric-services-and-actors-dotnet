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
    /// Invokes an administrative command on the given Infrastructure Service instance.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SFInfrastructureCommand")]
    public partial class NewInfrastructureCommandCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Command. The text of the command to be invoked. The content of the command is infrastructure-specific.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets ServiceId. The identity of the infrastructure service. This is the full name of the infrastructure
        /// service without the 'fabric:' URI scheme. This parameter required only for the cluster that has more than one
        /// instance of infrastructure service running.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public string ServiceId { get; set; }

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
            var result = this.ServiceFabricClient.Infrastructure.InvokeInfrastructureCommandAsync(
                command: this.Command,
                serviceId: this.ServiceId,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            this.WriteObject(result, true);
        }
    }
}
