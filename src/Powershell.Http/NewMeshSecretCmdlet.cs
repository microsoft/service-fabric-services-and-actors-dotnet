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
    /// Creates mesh secret resource in service fabric cluster.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SFMeshSecret")]
    public class NewMeshSecretCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Secret name to create.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "json")]
        [Parameter(Mandatory = true, ParameterSetName = "jsonfile")]
        [ValidateNotNullOrEmpty]
        public string SecretResourceName { get; set; }

        /// <summary>
        /// Gets or sets the json containing the description of the secret to be created.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "json")]
        [ValidateNotNullOrEmpty]
        public string JsonDescription { get; set; }

        /// <summary>
        /// Gets or sets the Json resource file containing the description of the secret to be created.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "jsonfile")]
        [ValidateNotNullOrEmpty]
        public string ResourceDescriptionFile { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecordInternal()
        {
            SecretResourceDescription secretResourceInfo = ServiceFabricClient.MeshSecrets.GetAsync(SecretResourceName, CancellationToken).GetAwaiter().GetResult();
            if (secretResourceInfo != null)
                throw new InvalidOperationException("Specified mesh secret already exists in cluster. If you want to update it use Update-SFMeshSecret");

            string jsonDescription = JsonDescription;
            if (ParameterSetName.Equals("jsonfile"))
                jsonDescription = File.ReadAllText(ResourceDescriptionFile);

            ServiceFabricClient.MeshSecrets.CreateOrUpdateAsync(
                SecretResourceName,
                jsonDescription,
                cancellationToken: CancellationToken).GetAwaiter().GetResult();
        }
    }
}
