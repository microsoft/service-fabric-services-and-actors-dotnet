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
    /// Downloads all of the code packages associated with specified service manifest on the specified node.
    /// </summary>
    [Cmdlet(VerbsCommon.Copy, "SFServicePackageToNode")]
    public partial class CopyServicePackageToNodeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets NodeName. The name of the node.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public NodeName NodeName { get; set; }

        /// <summary>
        /// Gets or sets ServiceManifestName. The name of service manifest whose packages need to be downloaded.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string ServiceManifestName { get; set; }

        /// <summary>
        /// Gets or sets ApplicationTypeName. The application type name as defined in the application manifest.
        /// </summary>
        [Parameter(Mandatory = true, Position = 2)]
        public string ApplicationTypeName { get; set; }

        /// <summary>
        /// Gets or sets ApplicationTypeVersion. The version of the application type as defined in the application manifest.
        /// </summary>
        [Parameter(Mandatory = true, Position = 3)]
        public string ApplicationTypeVersion { get; set; }

        /// <summary>
        /// Gets or sets PackageSharingPolicy. List of package sharing policy information.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public IEnumerable<PackageSharingPolicyInfo> PackageSharingPolicy { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var deployServicePackageToNodeDescription = new DeployServicePackageToNodeDescription(
            serviceManifestName: this.ServiceManifestName,
            applicationTypeName: this.ApplicationTypeName,
            applicationTypeVersion: this.ApplicationTypeVersion,
            nodeName: this.NodeName,
            packageSharingPolicy: this.PackageSharingPolicy);

            this.ServiceFabricClient.ServicePackages.DeployServicePackageToNodeAsync(
                nodeName: this.NodeName,
                deployServicePackageToNodeDescription: deployServicePackageToNodeDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
