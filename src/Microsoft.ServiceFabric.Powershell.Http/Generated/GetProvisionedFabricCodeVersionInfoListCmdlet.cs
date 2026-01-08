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
    /// Gets a list of fabric code versions that are provisioned in a Service Fabric cluster.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFProvisionedFabricCodeVersionInfoList")]
    public partial class GetProvisionedFabricCodeVersionInfoListCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets CodeVersion. The product version of Service Fabric.
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public string CodeVersion { get; set; }

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
            var result = this.ServiceFabricClient.Cluster.GetProvisionedFabricCodeVersionInfoListAsync(
                codeVersion: this.CodeVersion,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            if (result != null)
            {
                foreach (var item in result)
                {
                    this.WriteObject(this.FormatOutput(item));
                }
            }
        }
    }
}
