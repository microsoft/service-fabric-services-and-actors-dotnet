// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A base type for the health state of various entities in the cluster. It contains the aggregated health state.
    /// </summary>
    public partial class EntityHealthState
    {
        /// <summary>
        /// Initializes a new instance of the EntityHealthState class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        public EntityHealthState(
            HealthState? aggregatedHealthState = default(HealthState?))
        {
            this.AggregatedHealthState = aggregatedHealthState;
        }

        /// <summary>
        /// Gets the health state of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica
        /// etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'
        /// </summary>
        public HealthState? AggregatedHealthState { get; }
    }
}
