// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Specifies a metric to load balance a service during runtime.
    /// </summary>
    public partial class ServiceLoadMetricDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServiceLoadMetricDescription class.
        /// </summary>
        /// <param name="name">The name of the metric. If the service chooses to report load during runtime, the load metric
        /// name should match the name that is specified in Name exactly. Note that metric names are case-sensitive.</param>
        /// <param name="weight">The service load metric relative weight, compared to other metrics configured for this
        /// service, as a number. Possible values include: 'Zero', 'Low', 'Medium', 'High'
        /// 
        /// Determines the metric weight relative to the other metrics that are configured for this service. During runtime, if
        /// two metrics end up in conflict, the Cluster Resource Manager prefers the metric with the higher weight.
        /// </param>
        /// <param name="primaryDefaultLoad">Used only for Stateful services. The default amount of load, as a number, that
        /// this service creates for this metric when it is a Primary replica.</param>
        /// <param name="secondaryDefaultLoad">Used only for Stateful services. The default amount of load, as a number, that
        /// this service creates for this metric when it is a Secondary replica.</param>
        /// <param name="auxiliaryDefaultLoad">Used only for Stateful services. The default amount of load, as a number, that
        /// this service creates for this metric when it is an Auxiliary replica.</param>
        /// <param name="maximumLoad">Sets the upper bound on the service load to support collocation of multiple max
        /// sensitivity replicas. Loads reported above this value will be ignored.</param>
        /// <param name="defaultLoad">Used only for Stateless services. The default amount of load, as a number, that this
        /// service creates for this metric.</param>
        public ServiceLoadMetricDescription(
            string name,
            ServiceLoadMetricWeight? weight = default(ServiceLoadMetricWeight?),
            int? primaryDefaultLoad = default(int?),
            int? secondaryDefaultLoad = default(int?),
            int? auxiliaryDefaultLoad = default(int?),
            int? maximumLoad = default(int?),
            int? defaultLoad = default(int?))
        {
            name.ThrowIfNull(nameof(name));
            this.Name = name;
            this.Weight = weight;
            this.PrimaryDefaultLoad = primaryDefaultLoad;
            this.SecondaryDefaultLoad = secondaryDefaultLoad;
            this.AuxiliaryDefaultLoad = auxiliaryDefaultLoad;
            this.MaximumLoad = maximumLoad;
            this.DefaultLoad = defaultLoad;
        }

        /// <summary>
        /// Gets the name of the metric. If the service chooses to report load during runtime, the load metric name should
        /// match the name that is specified in Name exactly. Note that metric names are case-sensitive.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the service load metric relative weight, compared to other metrics configured for this service, as a number.
        /// Possible values include: 'Zero', 'Low', 'Medium', 'High'
        /// 
        /// Determines the metric weight relative to the other metrics that are configured for this service. During runtime, if
        /// two metrics end up in conflict, the Cluster Resource Manager prefers the metric with the higher weight.
        /// </summary>
        public ServiceLoadMetricWeight? Weight { get; }

        /// <summary>
        /// Gets used only for Stateful services. The default amount of load, as a number, that this service creates for this
        /// metric when it is a Primary replica.
        /// </summary>
        public int? PrimaryDefaultLoad { get; }

        /// <summary>
        /// Gets used only for Stateful services. The default amount of load, as a number, that this service creates for this
        /// metric when it is a Secondary replica.
        /// </summary>
        public int? SecondaryDefaultLoad { get; }

        /// <summary>
        /// Gets used only for Stateful services. The default amount of load, as a number, that this service creates for this
        /// metric when it is an Auxiliary replica.
        /// </summary>
        public int? AuxiliaryDefaultLoad { get; }

        /// <summary>
        /// Gets sets the upper bound on the service load to support collocation of multiple max sensitivity replicas. Loads
        /// reported above this value will be ignored.
        /// </summary>
        public int? MaximumLoad { get; }

        /// <summary>
        /// Gets used only for Stateless services. The default amount of load, as a number, that this service creates for this
        /// metric.
        /// </summary>
        public int? DefaultLoad { get; }
    }
}
