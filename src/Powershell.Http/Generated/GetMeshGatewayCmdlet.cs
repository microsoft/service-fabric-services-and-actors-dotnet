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
    /// Lists all the gateway resources.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFMeshGateway", DefaultParameterSetName = "List")]
    public partial class GetMeshGatewayCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets GatewayResourceName. The identity of the gateway.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "Get")]
        public string GatewayResourceName { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("List"))
            {
                var continuationToken = default(ContinuationToken);
                do
                {
                    var result = this.ServiceFabricClient.MeshGateways.ListAsync().GetAwaiter().GetResult();

                    if (result == null)
                    {
                        break;
                    }

                    var count = 0;
                    foreach (var item in result.Data)
                    {
                        count++;
                        this.WriteObject(this.FormatOutput(item));
                    }

                    continuationToken = result.ContinuationToken;
                    this.WriteDebug(string.Format(Resource.MsgCountAndContinuationToken, count, continuationToken));
                }
                while (continuationToken.Next);
            }
            else if (this.ParameterSetName.Equals("Get"))
            {
                var result = this.ServiceFabricClient.MeshGateways.GetAsync(
                    gatewayResourceName: this.GatewayResourceName,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                if (result != null)
                {
                    this.WriteObject(this.FormatOutput(result));
                }
            }
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            var outputResult = output as GatewayResourceDescription;

            var result = new PSObject();

            result.Properties.Add(new PSNoteProperty("Name", outputResult.Name));
            result.Properties.Add(new PSNoteProperty("Description", outputResult.Properties.Description));
            result.Properties.Add(new PSNoteProperty("Status", outputResult.Properties.Status));
            result.Properties.Add(new PSNoteProperty("StatusDetails", outputResult.Properties.StatusDetails));

            return result;
        }
    }
}
