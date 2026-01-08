// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The health statistics of an entity, returned as part of the health query result when the query description is
    /// configured to include statistics.
    /// The statistics include health state counts for all children types of the current entity.
    /// For example, for cluster, the health statistics include health state counts for nodes, applications, services,
    /// partitions, replicas, deployed applications and deployed service packages.
    /// For partition, the health statistics include health counts for replicas.
    /// </summary>
    public partial class HealthStatistics
    {
        /// <summary>
        /// Initializes a new instance of the HealthStatistics class.
        /// </summary>
        /// <param name="healthStateCountList">List of health state counts per entity kind, which keeps track of how many
        /// children of the queried entity are in Ok, Warning and Error state.
        /// </param>
        public HealthStatistics(
            IEnumerable<EntityKindHealthStateCount> healthStateCountList = default(IEnumerable<EntityKindHealthStateCount>))
        {
            this.HealthStateCountList = healthStateCountList;
        }

        /// <summary>
        /// Gets list of health state counts per entity kind, which keeps track of how many children of the queried entity are
        /// in Ok, Warning and Error state.
        /// </summary>
        public IEnumerable<EntityKindHealthStateCount> HealthStateCountList { get; }
    }
}
