// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a stateful service type defined in the service manifest of a provisioned application type.
    /// </summary>
    public partial class StatefulServiceTypeDescription : ServiceTypeDescription
    {
        /// <summary>
        /// Initializes a new instance of the StatefulServiceTypeDescription class.
        /// </summary>
        /// <param name="isStateful">Indicates whether the service type is a stateful service type or a stateless service type.
        /// This property is true if the service type is a stateful service type, false otherwise.</param>
        /// <param name="serviceTypeName">Name of the service type as specified in the service manifest.</param>
        /// <param name="placementConstraints">The placement constraint to be used when instantiating this service in a Service
        /// Fabric cluster.</param>
        /// <param name="loadMetrics">The service load metrics is given as an array of ServiceLoadMetricDescription
        /// objects.</param>
        /// <param name="servicePlacementPolicies">List of service placement policy descriptions.</param>
        /// <param name="extensions">List of service type extensions.</param>
        /// <param name="hasPersistedState">A flag indicating whether this is a persistent service which stores states on the
        /// local disk. If it is then the value of this property is true, if not it is false.</param>
        public StatefulServiceTypeDescription(
            bool? isStateful = default(bool?),
            string serviceTypeName = default(string),
            string placementConstraints = default(string),
            IEnumerable<ServiceLoadMetricDescription> loadMetrics = default(IEnumerable<ServiceLoadMetricDescription>),
            IEnumerable<ServicePlacementPolicyDescription> servicePlacementPolicies = default(IEnumerable<ServicePlacementPolicyDescription>),
            IEnumerable<ServiceTypeExtensionDescription> extensions = default(IEnumerable<ServiceTypeExtensionDescription>),
            bool? hasPersistedState = default(bool?))
            : base(
                Common.ServiceKind.Stateful,
                isStateful,
                serviceTypeName,
                placementConstraints,
                loadMetrics,
                servicePlacementPolicies,
                extensions)
        {
            this.HasPersistedState = hasPersistedState;
        }

        /// <summary>
        /// Gets a flag indicating whether this is a persistent service which stores states on the local disk. If it is then
        /// the value of this property is true, if not it is false.
        /// </summary>
        public bool? HasPersistedState { get; }
    }
}
