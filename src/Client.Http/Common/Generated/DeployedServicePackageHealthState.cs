// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state of a deployed service package, containing the entity identifier and the aggregated
    /// health state.
    /// </summary>
    public partial class DeployedServicePackageHealthState : EntityHealthState
    {
        /// <summary>
        /// Initializes a new instance of the DeployedServicePackageHealthState class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="nodeName">Name of the node on which the service package is deployed.</param>
        /// <param name="applicationName">The name of the application, including the 'fabric:' URI scheme.</param>
        /// <param name="serviceManifestName">Name of the manifest describing the service package.</param>
        /// <param name="servicePackageActivationId">The ActivationId of a deployed service package. If
        /// ServicePackageActivationMode specified at the time of creating the service
        /// is 'SharedProcess' (or if it is not specified, in which case it defaults to 'SharedProcess'), then value of
        /// ServicePackageActivationId
        /// is always an empty string.
        /// </param>
        public DeployedServicePackageHealthState(
            HealthState? aggregatedHealthState = default(HealthState?),
            NodeName nodeName = default(NodeName),
            ApplicationName applicationName = default(ApplicationName),
            string serviceManifestName = default(string),
            string servicePackageActivationId = default(string))
            : base(
                aggregatedHealthState)
        {
            this.NodeName = nodeName;
            this.ApplicationName = applicationName;
            this.ServiceManifestName = serviceManifestName;
            this.ServicePackageActivationId = servicePackageActivationId;
        }

        /// <summary>
        /// Gets name of the node on which the service package is deployed.
        /// </summary>
        public NodeName NodeName { get; }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName ApplicationName { get; }

        /// <summary>
        /// Gets name of the manifest describing the service package.
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
