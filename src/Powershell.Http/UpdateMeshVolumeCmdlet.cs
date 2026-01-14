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
    /// Updates mesh volume resource in service fabric cluster.
    /// </summary>
    [Cmdlet(VerbsData.Update, "SFMeshVolume")]
    public class UpdateMeshVolumeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets volume name to create.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "json")]
        [Parameter(Mandatory = true, ParameterSetName = "jsonfile")]
        [ValidateNotNullOrEmpty]
        public string VolumeResourceName { get; set; }

        /// <summary>
        /// Gets or sets the json containing the description of the volume to be updated.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "json")]
        [ValidateNotNullOrEmpty]
        public string JsonDescription { get; set; }

        /// <summary>
        /// Gets or sets the Json resource file containing the description of the volume to be updated.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "jsonfile")]
        [ValidateNotNullOrEmpty]
        public string ResourceDescriptionFile { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecordInternal()
        {
            VolumeResourceDescription volumeResourceInfo = ServiceFabricClient.MeshVolumes.GetAsync(VolumeResourceName, cancellationToken: CancellationToken).GetAwaiter().GetResult();
            if (volumeResourceInfo == null)
                throw new InvalidOperationException("Specified volume resource doesn't exists in cluster.");

            string jsonDescription = JsonDescription;
            if (ParameterSetName.Equals("jsonfile"))
                jsonDescription = File.ReadAllText(ResourceDescriptionFile);

            ServiceFabricClient.MeshVolumes.CreateOrUpdateAsync(
                VolumeResourceName,
                jsonDescription,
                cancellationToken: CancellationToken).GetAwaiter().GetResult();
        }
    }
}
