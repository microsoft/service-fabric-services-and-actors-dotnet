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
    /// Creates mesh application resource in service fabric cluster.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SFMeshApplication")]
    public class NewMeshApplicationCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Application resource name to create.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "json")]
        [Parameter(Mandatory = true, ParameterSetName = "jsonfile")]
        [ValidateNotNullOrEmpty]
        public string ApplicationResourceName { get; set; }

        /// <summary>
        /// Gets or sets the json containing the description of the application to be created.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "json")]
        [ValidateNotNullOrEmpty]
        public string JsonDescription { get; set; }

        /// <summary>
        /// Gets or sets the Json resource file containing the description of the application to be created.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "jsonfile")]
        [ValidateNotNullOrEmpty]
        public string ResourceDescriptionFile { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecordInternal()
        {
            ApplicationResourceDescription applicationResourceInfo = ServiceFabricClient.MeshApplications.GetAsync(ApplicationResourceName, CancellationToken).GetAwaiter().GetResult();

            if (applicationResourceInfo != null)
                throw new InvalidOperationException("Specified mesh application already exists in cluster. If you want to update it, use Update-SFMeshApplication");

            string jsonDescription = JsonDescription;
            if (ParameterSetName.Equals("jsonfile"))
                jsonDescription = File.ReadAllText(ResourceDescriptionFile);

            ServiceFabricClient.MeshApplications.CreateOrUpdateAsync(
                ApplicationResourceName,
                jsonDescription,
                cancellationToken: CancellationToken).GetAwaiter().GetResult();
        }
    }
}
