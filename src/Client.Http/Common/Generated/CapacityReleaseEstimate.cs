// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An estimate of projected used capacity relative to cluster total capacity for a cluster metric under a capacity
    /// release scenario.
    /// </summary>
    public partial class CapacityReleaseEstimate
    {
        /// <summary>
        /// Initializes a new instance of the CapacityReleaseEstimate class.
        /// </summary>
        /// <param name="level">The capacity release scenario whose projected impact this estimate represents. Possible values
        /// include: 'None', 'Minor', 'Major'
        /// 
        /// The level of capacity release applied to the cluster.
        /// </param>
        /// <param name="metricName">The cluster metric being projected.</param>
        /// <param name="usedCapacity">The projected used capacity for the metric if the capacity release level were
        /// applied.</param>
        /// <param name="totalCapacity">The cluster total capacity for the metric. This value is not projected and does not
        /// depend on the capacity release level.</param>
        public CapacityReleaseEstimate(
            CapacityReleaseLevel? level,
            string metricName,
            long? usedCapacity,
            long? totalCapacity)
        {
            level.ThrowIfNull(nameof(level));
            metricName.ThrowIfNull(nameof(metricName));
            usedCapacity.ThrowIfNull(nameof(usedCapacity));
            totalCapacity.ThrowIfNull(nameof(totalCapacity));
            this.Level = level;
            this.MetricName = metricName;
            this.UsedCapacity = usedCapacity;
            this.TotalCapacity = totalCapacity;
        }

        /// <summary>
        /// Gets the capacity release scenario whose projected impact this estimate represents. Possible values include:
        /// 'None', 'Minor', 'Major'
        /// 
        /// The level of capacity release applied to the cluster.
        /// </summary>
        public CapacityReleaseLevel? Level { get; }

        /// <summary>
        /// Gets the cluster metric being projected.
        /// </summary>
        public string MetricName { get; }

        /// <summary>
        /// Gets the projected used capacity for the metric if the capacity release level were applied.
        /// </summary>
        public long? UsedCapacity { get; }

        /// <summary>
        /// Gets the cluster total capacity for the metric. This value is not projected and does not depend on the capacity
        /// release level.
        /// </summary>
        public long? TotalCapacity { get; }
    }
}
