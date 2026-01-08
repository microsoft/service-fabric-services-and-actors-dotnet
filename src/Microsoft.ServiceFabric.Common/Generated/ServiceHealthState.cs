// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health state of a service, which contains the service identifier and its aggregated health state.
    /// </summary>
    public partial class ServiceHealthState : EntityHealthState
    {
        /// <summary>
        /// Initializes a new instance of the ServiceHealthState class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="serviceName">Name of the service whose health state is represented by this object.</param>
        public ServiceHealthState(
            HealthState? aggregatedHealthState = default(HealthState?),
            ServiceName serviceName = default(ServiceName))
            : base(
                aggregatedHealthState)
        {
            this.ServiceName = serviceName;
        }

        /// <summary>
        /// Gets name of the service whose health state is represented by this object.
        /// </summary>
        public ServiceName ServiceName { get; }
    }
}
