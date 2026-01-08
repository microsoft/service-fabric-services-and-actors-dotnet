// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.IO;
    using System.Management.Automation;
    using Microsoft.ServiceFabric.Client;

    /// <summary>
    /// Updates mesh secret resource in service fabric cluster.
    /// </summary>
    [Cmdlet(VerbsData.Update, "SFMeshSecret")]
    public class UpdateMeshSecretCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Secret name to update.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "json")]
        [Parameter(Mandatory = true, ParameterSetName = "jsonfile")]
        [ValidateNotNullOrEmpty]
        public string SecretResourceName { get; set; }

        /// <summary>
        /// Gets or sets the json containing the description of the secret to be updated.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "json")]
        [ValidateNotNullOrEmpty]
        public string JsonDescription { get; set; }

        /// <summary>
        /// Gets or sets the Json resource file containing the description of the secret to be updated.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "jsonfile")]
        [ValidateNotNullOrEmpty]
        public string ResourceDescriptionFile { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecordInternal()
        {
            var applicationResourceInfo = this.ServiceFabricClient.MeshSecrets.GetAsync(this.SecretResourceName, this.CancellationToken).GetAwaiter().GetResult();

            if (applicationResourceInfo == null)
            {
                throw new InvalidOperationException("Specified mesh secret doesn't exists in cluster.");
            }

            var jsonDescription = this.JsonDescription;

            if (this.ParameterSetName.Equals("jsonfile"))
            {
                jsonDescription = File.ReadAllText(this.ResourceDescriptionFile);
            }

            this.ServiceFabricClient.MeshSecrets.CreateOrUpdateAsync(
                secretResourceName: this.SecretResourceName,
                jsonDescription: jsonDescription,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();
        }
    }
}
