// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.IO;
using System.Management.Automation;
using Microsoft.ServiceFabric.Common;

namespace Microsoft.ServiceFabric.Powershell.Http
{
    /// <summary>
    /// Updates mesh gateway resource in service fabric cluster.
    /// </summary>
    [Cmdlet(VerbsData.Update, "SFMeshGateway")]
    public class UpdateMeshGatewayCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Gateway name to update.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "json")]
        [Parameter(Mandatory = true, ParameterSetName = "jsonfile")]
        [ValidateNotNullOrEmpty]
        public string GatewayResourceName { get; set; }

        /// <summary>
        /// Gets or sets the json containing the description of the gateway to be updated.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "json")]
        [ValidateNotNullOrEmpty]
        public string JsonDescription { get; set; }

        /// <summary>
        /// Gets or sets the Json resource file containing the description of the gateway to be updated.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "jsonfile")]
        [ValidateNotNullOrEmpty]
        public string ResourceDescriptionFile { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecordInternal()
        {
            GatewayResourceDescription gatewayResourceInfo = ServiceFabricClient.MeshGateways.GetAsync(GatewayResourceName, CancellationToken).GetAwaiter().GetResult();
            if (gatewayResourceInfo == null)
                throw new InvalidOperationException("Specified mesh gateway doesn't exist in cluster.");

            string jsonDescription = JsonDescription;
            if (ParameterSetName.Equals("jsonfile"))
                jsonDescription = File.ReadAllText(ResourceDescriptionFile);

            ServiceFabricClient.MeshGateways.CreateOrUpdateAsync(
                GatewayResourceName,
                jsonDescription,
                cancellationToken: CancellationToken).GetAwaiter().GetResult();
        }
    }
}
