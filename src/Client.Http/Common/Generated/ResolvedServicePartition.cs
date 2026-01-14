// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a service partition and its associated endpoints.
    /// </summary>
    public partial class ResolvedServicePartition
    {
        /// <summary>
        /// Initializes a new instance of the ResolvedServicePartition class.
        /// </summary>
        /// <param name="name">The full name of the service with 'fabric:' URI scheme.</param>
        /// <param name="partitionInformation">A representation of the resolved partition.</param>
        /// <param name="endpoints">List of resolved service endpoints of a service partition.</param>
        /// <param name="version">The version of this resolved service partition result. This version should be passed in the
        /// next time the ResolveService call is made via the PreviousRspVersion query parameter.</param>
        public ResolvedServicePartition(
            ServiceName name,
            PartitionInformation partitionInformation,
            IEnumerable<ResolvedServiceEndpoint> endpoints,
            string version)
        {
            name.ThrowIfNull(nameof(name));
            partitionInformation.ThrowIfNull(nameof(partitionInformation));
            endpoints.ThrowIfNull(nameof(endpoints));
            version.ThrowIfNull(nameof(version));
            this.Name = name;
            this.PartitionInformation = partitionInformation;
            this.Endpoints = endpoints;
            this.Version = version;
        }

        /// <summary>
        /// Gets the full name of the service with 'fabric:' URI scheme.
        /// </summary>
        public ServiceName Name { get; }

        /// <summary>
        /// Gets a representation of the resolved partition.
        /// </summary>
        public PartitionInformation PartitionInformation { get; }

        /// <summary>
        /// Gets list of resolved service endpoints of a service partition.
        /// </summary>
        public IEnumerable<ResolvedServiceEndpoint> Endpoints { get; }

        /// <summary>
        /// Gets the version of this resolved service partition result. This version should be passed in the next time the
        /// ResolveService call is made via the PreviousRspVersion query parameter.
        /// </summary>
        public string Version { get; }
    }
}
