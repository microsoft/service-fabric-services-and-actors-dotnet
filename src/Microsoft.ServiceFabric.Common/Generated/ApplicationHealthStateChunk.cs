// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state chunk of a application.
    /// The application health state chunk contains the application name, its aggregated health state and any children
    /// services and deployed applications that respect the filters in cluster health chunk query description.
    /// </summary>
    public partial class ApplicationHealthStateChunk : EntityHealthStateChunk
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationHealthStateChunk class.
        /// </summary>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="applicationName">The name of the application, including the 'fabric:' URI scheme.</param>
        /// <param name="applicationTypeName">The application type name as defined in the application manifest.</param>
        /// <param name="serviceHealthStateChunks">The list of service health state chunks in the cluster that respect the
        /// filters in the cluster health chunk query description.
        /// </param>
        /// <param name="deployedApplicationHealthStateChunks">The list of deployed application health state chunks in the
        /// cluster that respect the filters in the cluster health chunk query description.
        /// </param>
        public ApplicationHealthStateChunk(
            HealthState? healthState = default(HealthState?),
            ApplicationName applicationName = default(ApplicationName),
            string applicationTypeName = default(string),
            ServiceHealthStateChunkList serviceHealthStateChunks = default(ServiceHealthStateChunkList),
            DeployedApplicationHealthStateChunkList deployedApplicationHealthStateChunks = default(DeployedApplicationHealthStateChunkList))
            : base(
                healthState)
        {
            this.ApplicationName = applicationName;
            this.ApplicationTypeName = applicationTypeName;
            this.ServiceHealthStateChunks = serviceHealthStateChunks;
            this.DeployedApplicationHealthStateChunks = deployedApplicationHealthStateChunks;
        }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName ApplicationName { get; }

        /// <summary>
        /// Gets the application type name as defined in the application manifest.
        /// </summary>
        public string ApplicationTypeName { get; }

        /// <summary>
        /// Gets the list of service health state chunks in the cluster that respect the filters in the cluster health chunk
        /// query description.
        /// </summary>
        public ServiceHealthStateChunkList ServiceHealthStateChunks { get; }

        /// <summary>
        /// Gets the list of deployed application health state chunks in the cluster that respect the filters in the cluster
        /// health chunk query description.
        /// </summary>
        public DeployedApplicationHealthStateChunkList DeployedApplicationHealthStateChunks { get; }
    }
}
