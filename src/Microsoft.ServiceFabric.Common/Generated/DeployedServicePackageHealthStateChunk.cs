// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state chunk of a deployed service package, which contains the service manifest name and the
    /// service package aggregated health state.
    /// </summary>
    public partial class DeployedServicePackageHealthStateChunk : EntityHealthStateChunk
    {
        /// <summary>
        /// Initializes a new instance of the DeployedServicePackageHealthStateChunk class.
        /// </summary>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="serviceManifestName">The name of the service manifest.</param>
        /// <param name="servicePackageActivationId">The ActivationId of a deployed service package. If
        /// ServicePackageActivationMode specified at the time of creating the service
        /// is 'SharedProcess' (or if it is not specified, in which case it defaults to 'SharedProcess'), then value of
        /// ServicePackageActivationId
        /// is always an empty string.
        /// </param>
        public DeployedServicePackageHealthStateChunk(
            HealthState? healthState = default(HealthState?),
            string serviceManifestName = default(string),
            string servicePackageActivationId = default(string))
            : base(
                healthState)
        {
            this.ServiceManifestName = serviceManifestName;
            this.ServicePackageActivationId = servicePackageActivationId;
        }

        /// <summary>
        /// Gets the name of the service manifest.
        /// </summary>
        public string ServiceManifestName { get; }

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
