// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a replica of a service resource.
    /// </summary>
    public partial class ServiceReplicaDescription : ServiceReplicaProperties
    {
        /// <summary>
        /// Initializes a new instance of the ServiceReplicaDescription class.
        /// </summary>
        /// <param name="osType">The operation system required by the code in service. Possible values include: 'Linux',
        /// 'Windows'</param>
        /// <param name="codePackages">Describes the set of code packages that forms the service. A code package describes the
        /// container and the properties for running it. All the code packages are started together on the same host and share
        /// the same context (network, process etc.).
        /// </param>
        /// <param name="replicaName">Name of the replica.</param>
        /// <param name="networkRefs">The names of the private networks that this service needs to be part of.</param>
        /// <param name="diagnostics">Reference to sinks in DiagnosticsDescription.</param>
        public ServiceReplicaDescription(
            OperatingSystemType? osType,
            IEnumerable<ContainerCodePackageProperties> codePackages,
            string replicaName,
            IEnumerable<NetworkRef> networkRefs = default(IEnumerable<NetworkRef>),
            DiagnosticsRef diagnostics = default(DiagnosticsRef))
            : base(
                osType,
                codePackages,
                networkRefs,
                diagnostics)
        {
            replicaName.ThrowIfNull(nameof(replicaName));
            this.ReplicaName = replicaName;
        }

        /// <summary>
        /// Gets name of the replica.
        /// </summary>
        public string ReplicaName { get; }
    }
}
