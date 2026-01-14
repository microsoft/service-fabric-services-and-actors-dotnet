// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about service type deployed on a node, information such as the status of the service type registration
    /// on a node.
    /// </summary>
    public partial class DeployedServiceTypeInfo
    {
        /// <summary>
        /// Initializes a new instance of the DeployedServiceTypeInfo class.
        /// </summary>
        /// <param name="serviceTypeName">Name of the service type as specified in the service manifest.</param>
        /// <param name="serviceManifestName">The name of the service manifest in which this service type is defined.</param>
        /// <param name="codePackageName">The name of the code package that registered the service type.</param>
        /// <param name="status">The status of the service type registration on the node. Possible values include: 'Invalid',
        /// 'Disabled', 'Enabled', 'Registered'</param>
        /// <param name="servicePackageActivationId">The ActivationId of a deployed service package. If
        /// ServicePackageActivationMode specified at the time of creating the service
        /// is 'SharedProcess' (or if it is not specified, in which case it defaults to 'SharedProcess'), then value of
        /// ServicePackageActivationId
        /// is always an empty string.
        /// </param>
        public DeployedServiceTypeInfo(
            string serviceTypeName = default(string),
            string serviceManifestName = default(string),
            string codePackageName = default(string),
            ServiceTypeRegistrationStatus? status = default(ServiceTypeRegistrationStatus?),
            string servicePackageActivationId = default(string))
        {
            this.ServiceTypeName = serviceTypeName;
            this.ServiceManifestName = serviceManifestName;
            this.CodePackageName = codePackageName;
            this.Status = status;
            this.ServicePackageActivationId = servicePackageActivationId;
        }

        /// <summary>
        /// Gets name of the service type as specified in the service manifest.
        /// </summary>
        public string ServiceTypeName { get; }

        /// <summary>
        /// Gets the name of the service manifest in which this service type is defined.
        /// </summary>
        public string ServiceManifestName { get; }

        /// <summary>
        /// Gets the name of the code package that registered the service type.
        /// </summary>
        public string CodePackageName { get; }

        /// <summary>
        /// Gets the status of the service type registration on the node. Possible values include: 'Invalid', 'Disabled',
        /// 'Enabled', 'Registered'
        /// </summary>
        public ServiceTypeRegistrationStatus? Status { get; }

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
