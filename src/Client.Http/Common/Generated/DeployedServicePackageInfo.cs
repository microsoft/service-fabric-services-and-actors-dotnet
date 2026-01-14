// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about service package deployed on a Service Fabric node.
    /// </summary>
    public partial class DeployedServicePackageInfo
    {
        /// <summary>
        /// Initializes a new instance of the DeployedServicePackageInfo class.
        /// </summary>
        /// <param name="name">The name of the service package as specified in the service manifest.</param>
        /// <param name="version">The version of the service package specified in service manifest.</param>
        /// <param name="status">Specifies the status of a deployed application or service package on a Service Fabric node.
        /// . Possible values include: 'Invalid', 'Downloading', 'Activating', 'Active', 'Upgrading', 'Deactivating',
        /// 'RanToCompletion', 'Failed'</param>
        /// <param name="servicePackageActivationId">The ActivationId of a deployed service package. If
        /// ServicePackageActivationMode specified at the time of creating the service
        /// is 'SharedProcess' (or if it is not specified, in which case it defaults to 'SharedProcess'), then value of
        /// ServicePackageActivationId
        /// is always an empty string.
        /// </param>
        public DeployedServicePackageInfo(
            string name = default(string),
            string version = default(string),
            DeploymentStatus? status = default(DeploymentStatus?),
            string servicePackageActivationId = default(string))
        {
            this.Name = name;
            this.Version = version;
            this.Status = status;
            this.ServicePackageActivationId = servicePackageActivationId;
        }

        /// <summary>
        /// Gets the name of the service package as specified in the service manifest.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the version of the service package specified in service manifest.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets specifies the status of a deployed application or service package on a Service Fabric node.
        /// . Possible values include: 'Invalid', 'Downloading', 'Activating', 'Active', 'Upgrading', 'Deactivating',
        /// 'RanToCompletion', 'Failed'
        /// </summary>
        public DeploymentStatus? Status { get; }

        /// <summary>
        /// Gets the ActivationId of a deployed service package. If ServicePackageActivationMode specified at the time of
        /// creating the service
        /// is 'SharedProcess' (or if it is not specified, in which case it defaults to 'SharedProcess'), then value of
        /// ServicePackageActivationId
        /// is always an empty string.
        /// </summary>
        public string ServicePackageActivationId { get; }
    }
}
