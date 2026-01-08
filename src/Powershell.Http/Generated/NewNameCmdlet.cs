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
    /// Creates a Service Fabric name.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SFName")]
    public partial class NewNameCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Name. The Service Fabric name, including the 'fabric:' URI scheme.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public FabricName Name { get; set; }

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
            var fabricName = new FabricName(
            name: this.Name);

            this.ServiceFabricClient.Properties.CreateNameAsync(
                fabricName: fabricName,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
