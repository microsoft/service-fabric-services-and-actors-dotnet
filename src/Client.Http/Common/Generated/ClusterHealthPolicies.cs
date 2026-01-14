// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Health policies to evaluate cluster health.
    /// </summary>
    public partial class ClusterHealthPolicies
    {
        /// <summary>
        /// Initializes a new instance of the ClusterHealthPolicies class.
        /// </summary>
        /// <param name="applicationHealthPolicyMap">Defines a map that contains specific application health policies for
        /// different applications.
        /// Each entry specifies as key the application name and as value an ApplicationHealthPolicy used to evaluate the
        /// application health.
        /// If an application is not specified in the map, the application health evaluation uses the ApplicationHealthPolicy
        /// found in its application manifest or the default application health policy (if no health policy is defined in the
        /// manifest).
        /// The map is empty by default.
        /// </param>
        /// <param name="clusterHealthPolicy">Defines a health policy used to evaluate the health of the cluster or of a
        /// cluster node.
        /// </param>
        public ClusterHealthPolicies(
            IEnumerable<ApplicationHealthPolicyMapItem> applicationHealthPolicyMap = default(IEnumerable<ApplicationHealthPolicyMapItem>),
            ClusterHealthPolicy clusterHealthPolicy = default(ClusterHealthPolicy))
        {
            this.ApplicationHealthPolicyMap = applicationHealthPolicyMap;
            this.ClusterHealthPolicy = clusterHealthPolicy;
        }

        /// <summary>
        /// Gets defines a map that contains specific application health policies for different applications.
        /// Each entry specifies as key the application name and as value an ApplicationHealthPolicy used to evaluate the
        /// application health.
        /// If an application is not specified in the map, the application health evaluation uses the ApplicationHealthPolicy
        /// found in its application manifest or the default application health policy (if no health policy is defined in the
        /// manifest).
        /// The map is empty by default.
        /// </summary>
        public IEnumerable<ApplicationHealthPolicyMapItem> ApplicationHealthPolicyMap { get; }

        /// <summary>
        /// Gets defines a health policy used to evaluate the health of the cluster or of a cluster node.
        /// </summary>
        public ClusterHealthPolicy ClusterHealthPolicy { get; }
    }
}
