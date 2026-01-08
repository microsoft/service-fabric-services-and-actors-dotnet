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
    /// Adds the specified value as a new version of the specified secret resource.
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "SFMeshSecretValue")]
    public class NewMeshSecretValueCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets name of the secret resource.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "json")]
        [Parameter(Mandatory = true, ParameterSetName = "jsonfile")]
        [ValidateNotNullOrEmpty]
        public string SecretResourceName { get; set; }

        /// <summary>
        /// Gets or sets name of the secret value resource.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "json")]
        [Parameter(Mandatory = true, ParameterSetName = "jsonfile")]
        [ValidateNotNullOrEmpty]
        public string SecretValueResourceName { get; set; }

        /// <summary>
        /// Gets or sets the json containing the description of the secret value to be added.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "json")]
        [ValidateNotNullOrEmpty]
        public string JsonDescription { get; set; }

        /// <summary>
        /// Gets or sets the Json resource file containing the description of the secret value to be added.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "jsonfile")]
        [ValidateNotNullOrEmpty]
        public string ResourceDescriptionFile { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecordInternal()
        {
            var jsonDescription = this.JsonDescription;

            if (this.ParameterSetName.Equals("jsonfile"))
            {
                jsonDescription = File.ReadAllText(this.ResourceDescriptionFile);
            }

            this.ServiceFabricClient.MeshSecretValues.AddValueAsync(
                secretResourceName: this.SecretResourceName,
                secretValueResourceName: this.SecretValueResourceName,
                jsonDescription: jsonDescription,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();
        }
    }
}
