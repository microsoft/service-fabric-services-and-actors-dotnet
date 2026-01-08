// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for delta unhealthy cluster nodes in an upgrade domain, containing health evaluations
    /// for each unhealthy node that impacted current aggregated health state.
    /// Can be returned during cluster upgrade when cluster aggregated health state is Warning or Error.
    /// </summary>
    public partial class UpgradeDomainDeltaNodesCheckHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the UpgradeDomainDeltaNodesCheckHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="upgradeDomainName">Name of the upgrade domain where nodes health is currently evaluated.</param>
        /// <param name="baselineErrorCount">Number of upgrade domain nodes with aggregated heath state Error in the health
        /// store at the beginning of the cluster upgrade.</param>
        /// <param name="baselineTotalCount">Total number of upgrade domain nodes in the health store at the beginning of the
        /// cluster upgrade.</param>
        /// <param name="maxPercentDeltaUnhealthyNodes">Maximum allowed percentage of upgrade domain delta unhealthy nodes from
        /// the ClusterUpgradeHealthPolicy.</param>
        /// <param name="totalCount">Total number of upgrade domain nodes in the health store.</param>
        /// <param name="unhealthyEvaluations">List of unhealthy evaluations that led to the aggregated health state. Includes
        /// all the unhealthy NodeHealthEvaluation that impacted the aggregated health.</param>
        public UpgradeDomainDeltaNodesCheckHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            string upgradeDomainName = default(string),
            long? baselineErrorCount = default(long?),
            long? baselineTotalCount = default(long?),
            int? maxPercentDeltaUnhealthyNodes = default(int?),
            long? totalCount = default(long?),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.UpgradeDomainDeltaNodesCheck,
                aggregatedHealthState,
                description)
        {
            this.UpgradeDomainName = upgradeDomainName;
            this.BaselineErrorCount = baselineErrorCount;
            this.BaselineTotalCount = baselineTotalCount;
            this.MaxPercentDeltaUnhealthyNodes = maxPercentDeltaUnhealthyNodes;
            this.TotalCount = totalCount;
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets name of the upgrade domain where nodes health is currently evaluated.
        /// </summary>
        public string UpgradeDomainName { get; }

        /// <summary>
        /// Gets number of upgrade domain nodes with aggregated heath state Error in the health store at the beginning of the
        /// cluster upgrade.
        /// </summary>
        public long? BaselineErrorCount { get; }

        /// <summary>
        /// Gets total number of upgrade domain nodes in the health store at the beginning of the cluster upgrade.
        /// </summary>
        public long? BaselineTotalCount { get; }

        /// <summary>
        /// Gets maximum allowed percentage of upgrade domain delta unhealthy nodes from the ClusterUpgradeHealthPolicy.
        /// </summary>
        public int? MaxPercentDeltaUnhealthyNodes { get; }

        /// <summary>
        /// Gets total number of upgrade domain nodes in the health store.
        /// </summary>
        public long? TotalCount { get; }

        /// <summary>
        /// Gets list of unhealthy evaluations that led to the aggregated health state. Includes all the unhealthy
        /// NodeHealthEvaluation that impacted the aggregated health.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
