// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the properties of a service replica.
    /// </summary>
    public partial class ServiceReplicaProperties
    {
        /// <summary>
        /// Initializes a new instance of the ServiceReplicaProperties class.
        /// </summary>
        /// <param name="osType">The operation system required by the code in service. Possible values include: 'Linux',
        /// 'Windows'</param>
        /// <param name="codePackages">Describes the set of code packages that forms the service. A code package describes the
        /// container and the properties for running it. All the code packages are started together on the same host and share
        /// the same context (network, process etc.).
        /// </param>
        /// <param name="networkRefs">The names of the private networks that this service needs to be part of.</param>
        /// <param name="diagnostics">Reference to sinks in DiagnosticsDescription.</param>
        public ServiceReplicaProperties(
            OperatingSystemType? osType,
            IEnumerable<ContainerCodePackageProperties> codePackages,
            IEnumerable<NetworkRef> networkRefs = default(IEnumerable<NetworkRef>),
            DiagnosticsRef diagnostics = default(DiagnosticsRef))
        {
            osType.ThrowIfNull(nameof(osType));
            codePackages.ThrowIfNull(nameof(codePackages));
            this.OsType = osType;
            this.CodePackages = codePackages;
            this.NetworkRefs = networkRefs;
            this.Diagnostics = diagnostics;
        }

        /// <summary>
        /// Gets the operation system required by the code in service. Possible values include: 'Linux', 'Windows'
        /// </summary>
        public OperatingSystemType? OsType { get; }

        /// <summary>
        /// Gets the set of code packages that forms the service. A code package the container and the properties for running
        /// it. All the code packages are started together on the same host and share the same context (network, process etc.).
        /// </summary>
        public IEnumerable<ContainerCodePackageProperties> CodePackages { get; }

        /// <summary>
        /// Gets the names of the private networks that this service needs to be part of.
        /// </summary>
        public IEnumerable<NetworkRef> NetworkRefs { get; }

        /// <summary>
        /// Gets reference to sinks in DiagnosticsDescription.
        /// </summary>
        public DiagnosticsRef Diagnostics { get; }
    }
}
