// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines description for downloading packages associated with a service manifest to image cache on a Service Fabric
    /// node.
    /// </summary>
    public partial class DeployServicePackageToNodeDescription
    {
        /// <summary>
        /// Initializes a new instance of the DeployServicePackageToNodeDescription class.
        /// </summary>
        /// <param name="serviceManifestName">The name of service manifest whose packages need to be downloaded.</param>
        /// <param name="applicationTypeName">The application type name as defined in the application manifest.</param>
        /// <param name="applicationTypeVersion">The version of the application type as defined in the application
        /// manifest.</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="packageSharingPolicy">List of package sharing policy information.</param>
        public DeployServicePackageToNodeDescription(
            string serviceManifestName,
            string applicationTypeName,
            string applicationTypeVersion,
            NodeName nodeName,
            IEnumerable<PackageSharingPolicyInfo> packageSharingPolicy = default(IEnumerable<PackageSharingPolicyInfo>))
        {
            serviceManifestName.ThrowIfNull(nameof(serviceManifestName));
            applicationTypeName.ThrowIfNull(nameof(applicationTypeName));
            applicationTypeVersion.ThrowIfNull(nameof(applicationTypeVersion));
            nodeName.ThrowIfNull(nameof(nodeName));
            this.ServiceManifestName = serviceManifestName;
            this.ApplicationTypeName = applicationTypeName;
            this.ApplicationTypeVersion = applicationTypeVersion;
            this.NodeName = nodeName;
            this.PackageSharingPolicy = packageSharingPolicy;
        }

        /// <summary>
        /// Gets the name of service manifest whose packages need to be downloaded.
        /// </summary>
        public string ServiceManifestName { get; }

        /// <summary>
        /// Gets the application type name as defined in the application manifest.
        /// </summary>
        public string ApplicationTypeName { get; }

        /// <summary>
        /// Gets the version of the application type as defined in the application manifest.
        /// </summary>
        public string ApplicationTypeVersion { get; }

        /// <summary>
        /// Gets the name of a Service Fabric node.
        /// </summary>
        public NodeName NodeName { get; }

        /// <summary>
        /// Gets list of package sharing policy information.
        /// </summary>
        public IEnumerable<PackageSharingPolicyInfo> PackageSharingPolicy { get; }
    }
}
