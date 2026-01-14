// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state chunk of a deployed application, which contains the node where the application is
    /// deployed, the aggregated health state and any deployed service packages that respect the chunk query description
    /// filters.
    /// </summary>
    public partial class DeployedApplicationHealthStateChunk : EntityHealthStateChunk
    {
        /// <summary>
        /// Initializes a new instance of the DeployedApplicationHealthStateChunk class.
        /// </summary>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="nodeName">The name of node where the application is deployed.</param>
        /// <param name="deployedServicePackageHealthStateChunks">The list of deployed service package health state chunks
        /// belonging to the deployed application that respect the filters in the cluster health chunk query description.
        /// </param>
        public DeployedApplicationHealthStateChunk(
            HealthState? healthState = default(HealthState?),
            string nodeName = default(string),
            DeployedServicePackageHealthStateChunkList deployedServicePackageHealthStateChunks = default(DeployedServicePackageHealthStateChunkList))
            : base(
                healthState)
        {
            this.NodeName = nodeName;
            this.DeployedServicePackageHealthStateChunks = deployedServicePackageHealthStateChunks;
        }

        /// <summary>
        /// Gets the name of node where the application is deployed.
        /// </summary>
        public string NodeName { get; }

        /// <summary>
        /// Gets the list of deployed service package health state chunks belonging to the deployed application that respect
        /// the filters in the cluster health chunk query description.
        /// </summary>
        public DeployedServicePackageHealthStateChunkList DeployedServicePackageHealthStateChunks { get; }
    }
}
