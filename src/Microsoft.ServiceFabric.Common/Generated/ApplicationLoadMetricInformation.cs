// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes load information for a custom resource balancing metric. This can be used to limit the total consumption
    /// of this metric by the services of this application.
    /// </summary>
    public partial class ApplicationLoadMetricInformation
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationLoadMetricInformation class.
        /// </summary>
        /// <param name="name">The name of the metric.</param>
        /// <param name="reservationCapacity">This is the capacity reserved in the cluster for the application.
        /// It's the product of NodeReservationCapacity and MinimumNodes.
        /// If set to zero, no capacity is reserved for this metric.
        /// When setting application capacity or when updating application capacity this value must be smaller than or equal to
        /// MaximumCapacity for each metric.
        /// </param>
        /// <param name="applicationCapacity">Total capacity for this metric in this application instance.
        /// </param>
        /// <param name="applicationLoad">Current load for this metric in this application instance.
        /// </param>
        public ApplicationLoadMetricInformation(
            string name = default(string),
            long? reservationCapacity = default(long?),
            long? applicationCapacity = default(long?),
            long? applicationLoad = default(long?))
        {
            this.Name = name;
            this.ReservationCapacity = reservationCapacity;
            this.ApplicationCapacity = applicationCapacity;
            this.ApplicationLoad = applicationLoad;
        }

        /// <summary>
        /// Gets the name of the metric.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets this is the capacity reserved in the cluster for the application.
        /// It's the product of NodeReservationCapacity and MinimumNodes.
        /// If set to zero, no capacity is reserved for this metric.
        /// When setting application capacity or when updating application capacity this value must be smaller than or equal to
        /// MaximumCapacity for each metric.
        /// </summary>
        public long? ReservationCapacity { get; }

        /// <summary>
        /// Gets total capacity for this metric in this application instance.
        /// </summary>
        public long? ApplicationCapacity { get; }

        /// <summary>
        /// Gets current load for this metric in this application instance.
        /// </summary>
        public long? ApplicationLoad { get; }
    }
}
