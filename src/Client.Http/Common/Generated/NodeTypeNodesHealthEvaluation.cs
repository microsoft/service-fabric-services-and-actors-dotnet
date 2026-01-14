// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for nodes of a particular node type. The node type nodes evaluation can be returned
    /// when cluster health evaluation returns unhealthy aggregated health state, either Error or Warning. It contains
    /// health evaluations for each unhealthy node of the included node type that impacted current aggregated health state.
    /// </summary>
    public partial class NodeTypeNodesHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the NodeTypeNodesHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="nodeTypeName">The node type name as defined in the cluster manifest.</param>
        /// <param name="maxPercentUnhealthyNodes">Maximum allowed percentage of unhealthy nodes for the node type, specified
        /// as an entry in NodeTypeHealthPolicyMap.</param>
        /// <param name="totalCount">Total number of nodes of the node type found in the health store.</param>
        /// <param name="unhealthyEvaluations">List of unhealthy evaluations that led to the aggregated health state. Includes
        /// all the unhealthy NodeHealthEvaluation of this node type that impacted the aggregated health.</param>
        public NodeTypeNodesHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            string nodeTypeName = default(string),
            int? maxPercentUnhealthyNodes = default(int?),
            long? totalCount = default(long?),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.NodeTypeNodes,
                aggregatedHealthState,
                description)
        {
            this.NodeTypeName = nodeTypeName;
            this.MaxPercentUnhealthyNodes = maxPercentUnhealthyNodes;
            this.TotalCount = totalCount;
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets the node type name as defined in the cluster manifest.
        /// </summary>
        public string NodeTypeName { get; }

        /// <summary>
        /// Gets maximum allowed percentage of unhealthy nodes for the node type, specified as an entry in
        /// NodeTypeHealthPolicyMap.
        /// </summary>
        public int? MaxPercentUnhealthyNodes { get; }

        /// <summary>
        /// Gets total number of nodes of the node type found in the health store.
        /// </summary>
        public long? TotalCount { get; }

        /// <summary>
        /// Gets list of unhealthy evaluations that led to the aggregated health state. Includes all the unhealthy
        /// NodeHealthEvaluation of this node type that impacted the aggregated health.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
