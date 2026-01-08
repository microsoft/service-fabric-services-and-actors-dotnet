// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state of an application, which contains the application identifier and the aggregated health
    /// state.
    /// </summary>
    public partial class ApplicationHealthState : EntityHealthState
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationHealthState class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="name">The name of the application, including the 'fabric:' URI scheme.</param>
        public ApplicationHealthState(
            HealthState? aggregatedHealthState = default(HealthState?),
            ApplicationName name = default(ApplicationName))
            : base(
                aggregatedHealthState)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName Name { get; }
    }
}
