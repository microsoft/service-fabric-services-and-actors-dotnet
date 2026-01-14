// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes capacity information for services of this application. This description can be used for describing the
    /// following.
    /// - Reserving the capacity for the services on the nodes
    /// - Limiting the total number of nodes that services of this application can run on
    /// - Limiting the custom capacity metrics to limit the total consumption of this metric by the services of this
    /// application
    /// </summary>
    public partial class ApplicationCapacityDescription
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationCapacityDescription class.
        /// </summary>
        /// <param name="minimumNodes">The minimum number of nodes where Service Fabric will reserve capacity for this
        /// application. Note that this does not mean that the services of this application will be placed on all of those
        /// nodes. If this property is set to zero, no capacity will be reserved. The value of this property cannot be more
        /// than the value of the MaximumNodes property.</param>
        /// <param name="maximumNodes">The maximum number of nodes where Service Fabric will reserve capacity for this
        /// application. Note that this does not mean that the services of this application will be placed on all of those
        /// nodes. By default, the value of this property is zero and it means that the services can be placed on any
        /// node.</param>
        /// <param name="applicationMetrics">List of application capacity metric description.</param>
        public ApplicationCapacityDescription(
            long? minimumNodes = default(long?),
            long? maximumNodes = 0,
            IEnumerable<ApplicationMetricDescription> applicationMetrics = default(IEnumerable<ApplicationMetricDescription>))
        {
            minimumNodes?.ThrowIfLessThan("minimumNodes", 0);
            maximumNodes?.ThrowIfLessThan("maximumNodes", 0);
            this.MinimumNodes = minimumNodes;
            this.MaximumNodes = maximumNodes;
            this.ApplicationMetrics = applicationMetrics;
        }

        /// <summary>
        /// Gets the minimum number of nodes where Service Fabric will reserve capacity for this application. Note that this
        /// does not mean that the services of this application will be placed on all of those nodes. If this property is set
        /// to zero, no capacity will be reserved. The value of this property cannot be more than the value of the MaximumNodes
        /// property.
        /// </summary>
        public long? MinimumNodes { get; }

        /// <summary>
        /// Gets the maximum number of nodes where Service Fabric will reserve capacity for this application. Note that this
        /// does not mean that the services of this application will be placed on all of those nodes. By default, the value of
        /// this property is zero and it means that the services can be placed on any node.
        /// </summary>
        public long? MaximumNodes { get; }

        /// <summary>
        /// Gets list of application capacity metric description.
        /// </summary>
        public IEnumerable<ApplicationMetricDescription> ApplicationMetrics { get; }
    }
}
