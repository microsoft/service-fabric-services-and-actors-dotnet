// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents data structure that contains load information for a certain metric in a cluster.
    /// </summary>
    public partial class LoadMetricInformation
    {
        /// <summary>
        /// Initializes a new instance of the LoadMetricInformation class.
        /// </summary>
        /// <param name="name">Name of the metric for which this load information is provided.</param>
        /// <param name="isBalancedBefore">Value that indicates whether the metrics is balanced or not before resource balancer
        /// run</param>
        /// <param name="isBalancedAfter">Value that indicates whether the metrics is balanced or not after resource balancer
        /// run.</param>
        /// <param name="deviationBefore">The standard average deviation of the metrics before resource balancer run.</param>
        /// <param name="deviationAfter">The standard average deviation of the metrics after resource balancer run.</param>
        /// <param name="balancingThreshold">The balancing threshold for a certain metric.</param>
        /// <param name="action">The current action being taken with regard to this metric</param>
        /// <param name="activityThreshold">The Activity Threshold specified for this metric in the system Cluster
        /// Manifest.</param>
        /// <param name="clusterCapacity">The total cluster capacity for a given metric</param>
        /// <param name="clusterLoad">The total cluster load. In future releases of Service Fabric this parameter will be
        /// deprecated in favor of CurrentClusterLoad.</param>
        /// <param name="currentClusterLoad">The total cluster load.</param>
        /// <param name="clusterRemainingCapacity">The remaining capacity for the metric in the cluster. In future releases of
        /// Service Fabric this parameter will be deprecated in favor of ClusterCapacityRemaining.</param>
        /// <param name="clusterCapacityRemaining">The remaining capacity for the metric in the cluster.</param>
        /// <param name="isClusterCapacityViolation">Indicates that the metric is currently over capacity in the
        /// cluster.</param>
        /// <param name="nodeBufferPercentage">The reserved percentage of total node capacity for this metric.</param>
        /// <param name="clusterBufferedCapacity">Remaining capacity in the cluster excluding the reserved space. In future
        /// releases of Service Fabric this parameter will be deprecated in favor of BufferedClusterCapacityRemaining.</param>
        /// <param name="bufferedClusterCapacityRemaining">Remaining capacity in the cluster excluding the reserved
        /// space.</param>
        /// <param name="clusterRemainingBufferedCapacity">The remaining percentage of cluster total capacity for this
        /// metric.</param>
        /// <param name="minNodeLoadValue">The minimum load on any node for this metric. In future releases of Service Fabric
        /// this parameter will be deprecated in favor of MinimumNodeLoad.</param>
        /// <param name="minimumNodeLoad">The minimum load on any node for this metric.</param>
        /// <param name="minNodeLoadNodeId">The node id of the node with the minimum load for this metric.</param>
        /// <param name="maxNodeLoadValue">The maximum load on any node for this metric. In future releases of Service Fabric
        /// this parameter will be deprecated in favor of MaximumNodeLoad.</param>
        /// <param name="maximumNodeLoad">The maximum load on any node for this metric.</param>
        /// <param name="maxNodeLoadNodeId">The node id of the node with the maximum load for this metric.</param>
        /// <param name="plannedLoadRemoval">This value represents the load of the replicas that are planned to be removed in
        /// the future within the cluster.
        /// This kind of load is reported for replicas that are currently being moving to other nodes and for replicas that are
        /// currently being dropped but still use the load on the source node.
        /// </param>
        public LoadMetricInformation(
            string name = default(string),
            bool? isBalancedBefore = default(bool?),
            bool? isBalancedAfter = default(bool?),
            double? deviationBefore = default(double?),
            double? deviationAfter = default(double?),
            double? balancingThreshold = default(double?),
            string action = default(string),
            string activityThreshold = default(string),
            string clusterCapacity = default(string),
            string clusterLoad = default(string),
            double? currentClusterLoad = default(double?),
            string clusterRemainingCapacity = default(string),
            double? clusterCapacityRemaining = default(double?),
            bool? isClusterCapacityViolation = default(bool?),
            double? nodeBufferPercentage = default(double?),
            string clusterBufferedCapacity = default(string),
            double? bufferedClusterCapacityRemaining = default(double?),
            string clusterRemainingBufferedCapacity = default(string),
            string minNodeLoadValue = default(string),
            double? minimumNodeLoad = default(double?),
            NodeId minNodeLoadNodeId = default(NodeId),
            string maxNodeLoadValue = default(string),
            double? maximumNodeLoad = default(double?),
            NodeId maxNodeLoadNodeId = default(NodeId),
            double? plannedLoadRemoval = default(double?))
        {
            this.Name = name;
            this.IsBalancedBefore = isBalancedBefore;
            this.IsBalancedAfter = isBalancedAfter;
            this.DeviationBefore = deviationBefore;
            this.DeviationAfter = deviationAfter;
            this.BalancingThreshold = balancingThreshold;
            this.Action = action;
            this.ActivityThreshold = activityThreshold;
            this.ClusterCapacity = clusterCapacity;
            this.ClusterLoad = clusterLoad;
            this.CurrentClusterLoad = currentClusterLoad;
            this.ClusterRemainingCapacity = clusterRemainingCapacity;
            this.ClusterCapacityRemaining = clusterCapacityRemaining;
            this.IsClusterCapacityViolation = isClusterCapacityViolation;
            this.NodeBufferPercentage = nodeBufferPercentage;
            this.ClusterBufferedCapacity = clusterBufferedCapacity;
            this.BufferedClusterCapacityRemaining = bufferedClusterCapacityRemaining;
            this.ClusterRemainingBufferedCapacity = clusterRemainingBufferedCapacity;
            this.MinNodeLoadValue = minNodeLoadValue;
            this.MinimumNodeLoad = minimumNodeLoad;
            this.MinNodeLoadNodeId = minNodeLoadNodeId;
            this.MaxNodeLoadValue = maxNodeLoadValue;
            this.MaximumNodeLoad = maximumNodeLoad;
            this.MaxNodeLoadNodeId = maxNodeLoadNodeId;
            this.PlannedLoadRemoval = plannedLoadRemoval;
        }

        /// <summary>
        /// Gets name of the metric for which this load information is provided.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets value that indicates whether the metrics is balanced or not before resource balancer run
        /// </summary>
        public bool? IsBalancedBefore { get; }

        /// <summary>
        /// Gets value that indicates whether the metrics is balanced or not after resource balancer run.
        /// </summary>
        public bool? IsBalancedAfter { get; }

        /// <summary>
        /// Gets the standard average deviation of the metrics before resource balancer run.
        /// </summary>
        public double? DeviationBefore { get; }

        /// <summary>
        /// Gets the standard average deviation of the metrics after resource balancer run.
        /// </summary>
        public double? DeviationAfter { get; }

        /// <summary>
        /// Gets the balancing threshold for a certain metric.
        /// </summary>
        public double? BalancingThreshold { get; }

        /// <summary>
        /// Gets the current action being taken with regard to this metric
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// Gets the Activity Threshold specified for this metric in the system Cluster Manifest.
        /// </summary>
        public string ActivityThreshold { get; }

        /// <summary>
        /// Gets the total cluster capacity for a given metric
        /// </summary>
        public string ClusterCapacity { get; }

        /// <summary>
        /// Gets the total cluster load. In future releases of Service Fabric this parameter will be deprecated in favor of
        /// CurrentClusterLoad.
        /// </summary>
        public string ClusterLoad { get; }

        /// <summary>
        /// Gets the total cluster load.
        /// </summary>
        public double? CurrentClusterLoad { get; }

        /// <summary>
        /// Gets the remaining capacity for the metric in the cluster. In future releases of Service Fabric this parameter will
        /// be deprecated in favor of ClusterCapacityRemaining.
        /// </summary>
        public string ClusterRemainingCapacity { get; }

        /// <summary>
        /// Gets the remaining capacity for the metric in the cluster.
        /// </summary>
        public double? ClusterCapacityRemaining { get; }

        /// <summary>
        /// Gets indicates that the metric is currently over capacity in the cluster.
        /// </summary>
        public bool? IsClusterCapacityViolation { get; }

        /// <summary>
        /// Gets the reserved percentage of total node capacity for this metric.
        /// </summary>
        public double? NodeBufferPercentage { get; }

        /// <summary>
        /// Gets remaining capacity in the cluster excluding the reserved space. In future releases of Service Fabric this
        /// parameter will be deprecated in favor of BufferedClusterCapacityRemaining.
        /// </summary>
        public string ClusterBufferedCapacity { get; }

        /// <summary>
        /// Gets remaining capacity in the cluster excluding the reserved space.
        /// </summary>
        public double? BufferedClusterCapacityRemaining { get; }

        /// <summary>
        /// Gets the remaining percentage of cluster total capacity for this metric.
        /// </summary>
        public string ClusterRemainingBufferedCapacity { get; }

        /// <summary>
        /// Gets the minimum load on any node for this metric. In future releases of Service Fabric this parameter will be
        /// deprecated in favor of MinimumNodeLoad.
        /// </summary>
        public string MinNodeLoadValue { get; }

        /// <summary>
        /// Gets the minimum load on any node for this metric.
        /// </summary>
        public double? MinimumNodeLoad { get; }

        /// <summary>
        /// Gets the node id of the node with the minimum load for this metric.
        /// </summary>
        public NodeId MinNodeLoadNodeId { get; }

        /// <summary>
        /// Gets the maximum load on any node for this metric. In future releases of Service Fabric this parameter will be
        /// deprecated in favor of MaximumNodeLoad.
        /// </summary>
        public string MaxNodeLoadValue { get; }

        /// <summary>
        /// Gets the maximum load on any node for this metric.
        /// </summary>
        public double? MaximumNodeLoad { get; }

        /// <summary>
        /// Gets the node id of the node with the maximum load for this metric.
        /// </summary>
        public NodeId MaxNodeLoadNodeId { get; }

        /// <summary>
        /// Gets this value represents the load of the replicas that are planned to be removed in the future within the
        /// cluster.
        /// This kind of load is reported for replicas that are currently being moving to other nodes and for replicas that are
        /// currently being dropped but still use the load on the source node.
        /// </summary>
        public double? PlannedLoadRemoval { get; }
    }
}
