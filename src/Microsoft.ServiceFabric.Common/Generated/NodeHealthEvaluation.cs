// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for a node, containing information about the data and the algorithm used by health
    /// store to evaluate health. The evaluation is returned only when the aggregated health state is either Error or
    /// Warning.
    /// </summary>
    public partial class NodeHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the NodeHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="unhealthyEvaluations">List of unhealthy evaluations that led to the current aggregated health state of
        /// the node. The types of the unhealthy evaluations can be EventHealthEvaluation.</param>
        public NodeHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            NodeName nodeName = default(NodeName),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.Node,
                aggregatedHealthState,
                description)
        {
            this.NodeName = nodeName;
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets the name of a Service Fabric node.
        /// </summary>
        public NodeName NodeName { get; }

        /// <summary>
        /// Gets list of unhealthy evaluations that led to the current aggregated health state of the node. The types of the
        /// unhealthy evaluations can be EventHealthEvaluation.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
