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
    /// Provision the code or configuration packages of a Service Fabric cluster.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Register, "SFCluster")]
    public partial class RegisterClusterCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets CodeFilePath. The cluster code package file path.
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public string CodeFilePath { get; set; }

        /// <summary>
        /// Gets or sets ClusterManifestFilePath. The cluster manifest file path.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public string ClusterManifestFilePath { get; set; }

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
            var provisionFabricDescription = new ProvisionFabricDescription(
            codeFilePath: this.CodeFilePath,
            clusterManifestFilePath: this.ClusterManifestFilePath);

            this.ServiceFabricClient.Cluster.ProvisionClusterAsync(
                provisionFabricDescription: provisionFabricDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
