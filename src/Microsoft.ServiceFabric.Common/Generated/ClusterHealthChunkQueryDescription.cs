// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The cluster health chunk query description, which can specify the health policies to evaluate cluster health and
    /// very expressive filters to select which cluster entities to include in response.
    /// </summary>
    public partial class ClusterHealthChunkQueryDescription
    {
        /// <summary>
        /// Initializes a new instance of the ClusterHealthChunkQueryDescription class.
        /// </summary>
        /// <param name="nodeFilters">Defines a list of filters that specify which nodes to be included in the returned cluster
        /// health chunk.
        /// If no filters are specified, no nodes are returned. All the nodes are used to evaluate the cluster's aggregated
        /// health state, regardless of the input filters.
        /// The cluster health chunk query may specify multiple node filters.
        /// For example, it can specify a filter to return all nodes with health state Error and another filter to always
        /// include a node identified by its NodeName.
        /// </param>
        /// <param name="applicationFilters">Defines a list of filters that specify which applications to be included in the
        /// returned cluster health chunk.
        /// If no filters are specified, no applications are returned. All the applications are used to evaluate the cluster's
        /// aggregated health state, regardless of the input filters.
        /// The cluster health chunk query may specify multiple application filters.
        /// For example, it can specify a filter to return all applications with health state Error and another filter to
        /// always include applications of a specified application type.
        /// </param>
        /// <param name="clusterHealthPolicy">Defines a health policy used to evaluate the health of the cluster or of a
        /// cluster node.
        /// </param>
        /// <param name="applicationHealthPolicies">Defines the application health policy map used to evaluate the health of an
        /// application or one of its children entities.
        /// </param>
        public ClusterHealthChunkQueryDescription(
            IEnumerable<NodeHealthStateFilter> nodeFilters = default(IEnumerable<NodeHealthStateFilter>),
            IEnumerable<ApplicationHealthStateFilter> applicationFilters = default(IEnumerable<ApplicationHealthStateFilter>),
            ClusterHealthPolicy clusterHealthPolicy = default(ClusterHealthPolicy),
            ApplicationHealthPolicies applicationHealthPolicies = default(ApplicationHealthPolicies))
        {
            this.NodeFilters = nodeFilters;
            this.ApplicationFilters = applicationFilters;
            this.ClusterHealthPolicy = clusterHealthPolicy;
            this.ApplicationHealthPolicies = applicationHealthPolicies;
        }

        /// <summary>
        /// Gets defines a list of filters that specify which nodes to be included in the returned cluster health chunk.
        /// If no filters are specified, no nodes are returned. All the nodes are used to evaluate the cluster's aggregated
        /// health state, regardless of the input filters.
        /// The cluster health chunk query may specify multiple node filters.
        /// For example, it can specify a filter to return all nodes with health state Error and another filter to always
        /// include a node identified by its NodeName.
        /// </summary>
        public IEnumerable<NodeHealthStateFilter> NodeFilters { get; }

        /// <summary>
        /// Gets defines a list of filters that specify which applications to be included in the returned cluster health chunk.
        /// If no filters are specified, no applications are returned. All the applications are used to evaluate the cluster's
        /// aggregated health state, regardless of the input filters.
        /// The cluster health chunk query may specify multiple application filters.
        /// For example, it can specify a filter to return all applications with health state Error and another filter to
        /// always include applications of a specified application type.
        /// </summary>
        public IEnumerable<ApplicationHealthStateFilter> ApplicationFilters { get; }

        /// <summary>
        /// Gets defines a health policy used to evaluate the health of the cluster or of a cluster node.
        /// </summary>
        public ClusterHealthPolicy ClusterHealthPolicy { get; }

        /// <summary>
        /// Gets defines the application health policy map used to evaluate the health of an application or one of its children
        /// entities.
        /// </summary>
        public ApplicationHealthPolicies ApplicationHealthPolicies { get; }
    }
}
