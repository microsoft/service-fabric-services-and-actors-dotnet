// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a service type that is defined in a service manifest of a provisioned application type.
    /// </summary>
    public partial class ServiceTypeInfo
    {
        /// <summary>
        /// Initializes a new instance of the ServiceTypeInfo class.
        /// </summary>
        /// <param name="serviceTypeDescription">Describes a service type defined in the service manifest of a provisioned
        /// application type. The properties the ones defined in the service manifest.</param>
        /// <param name="serviceManifestName">The name of the service manifest in which this service type is defined.</param>
        /// <param name="serviceManifestVersion">The version of the service manifest in which this service type is
        /// defined.</param>
        /// <param name="isServiceGroup">Indicates whether the service is a service group. If it is, the property value is true
        /// otherwise false.</param>
        public ServiceTypeInfo(
            ServiceTypeDescription serviceTypeDescription = default(ServiceTypeDescription),
            string serviceManifestName = default(string),
            string serviceManifestVersion = default(string),
            bool? isServiceGroup = default(bool?))
        {
            this.ServiceTypeDescription = serviceTypeDescription;
            this.ServiceManifestName = serviceManifestName;
            this.ServiceManifestVersion = serviceManifestVersion;
            this.IsServiceGroup = isServiceGroup;
        }

        /// <summary>
        /// Gets a service type defined in the service manifest of a provisioned application type. The properties the ones
        /// defined in the service manifest.
        /// </summary>
        public ServiceTypeDescription ServiceTypeDescription { get; }

        /// <summary>
        /// Gets the name of the service manifest in which this service type is defined.
        /// </summary>
        public string ServiceManifestName { get; }

        /// <summary>
        /// Gets the version of the service manifest in which this service type is defined.
        /// </summary>
        public string ServiceManifestVersion { get; }

        /// <summary>
        /// Gets indicates whether the service is a service group. If it is, the property value is true otherwise false.
        /// </summary>
        public bool? IsServiceGroup { get; }
    }
}
