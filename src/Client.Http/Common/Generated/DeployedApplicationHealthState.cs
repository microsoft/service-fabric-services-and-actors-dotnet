// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state of a deployed application, which contains the entity identifier and the aggregated
    /// health state.
    /// </summary>
    public partial class DeployedApplicationHealthState : EntityHealthState
    {
        /// <summary>
        /// Initializes a new instance of the DeployedApplicationHealthState class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="nodeName">Name of the node on which the service package is deployed.</param>
        /// <param name="applicationName">The name of the application, including the 'fabric:' URI scheme.</param>
        public DeployedApplicationHealthState(
            HealthState? aggregatedHealthState = default(HealthState?),
            NodeName nodeName = default(NodeName),
            ApplicationName applicationName = default(ApplicationName))
            : base(
                aggregatedHealthState)
        {
            this.NodeName = nodeName;
            this.ApplicationName = applicationName;
        }

        /// <summary>
        /// Gets name of the node on which the service package is deployed.
        /// </summary>
        public NodeName NodeName { get; }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName ApplicationName { get; }
    }
}
