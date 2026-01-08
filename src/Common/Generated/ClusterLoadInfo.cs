// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about load in a Service Fabric cluster. It holds a summary of all metrics and their load in a cluster.
    /// </summary>
    public partial class ClusterLoadInfo
    {
        /// <summary>
        /// Initializes a new instance of the ClusterLoadInfo class.
        /// </summary>
        /// <param name="lastBalancingStartTimeUtc">The starting time of last resource balancing run.</param>
        /// <param name="lastBalancingEndTimeUtc">The end time of last resource balancing run.</param>
        /// <param name="loadMetricInformation">List that contains metrics and their load information in this cluster.</param>
        public ClusterLoadInfo(
            DateTime? lastBalancingStartTimeUtc = default(DateTime?),
            DateTime? lastBalancingEndTimeUtc = default(DateTime?),
            IEnumerable<LoadMetricInformation> loadMetricInformation = default(IEnumerable<LoadMetricInformation>))
        {
            this.LastBalancingStartTimeUtc = lastBalancingStartTimeUtc;
            this.LastBalancingEndTimeUtc = lastBalancingEndTimeUtc;
            this.LoadMetricInformation = loadMetricInformation;
        }

        /// <summary>
        /// Gets the starting time of last resource balancing run.
        /// </summary>
        public DateTime? LastBalancingStartTimeUtc { get; }

        /// <summary>
        /// Gets the end time of last resource balancing run.
        /// </summary>
        public DateTime? LastBalancingEndTimeUtc { get; }

        /// <summary>
        /// Gets list that contains metrics and their load information in this cluster.
        /// </summary>
        public IEnumerable<LoadMetricInformation> LoadMetricInformation { get; }
    }
}
