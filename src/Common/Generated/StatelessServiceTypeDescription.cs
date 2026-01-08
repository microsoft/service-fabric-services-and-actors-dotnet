// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a stateless service type defined in the service manifest of a provisioned application type.
    /// </summary>
    public partial class StatelessServiceTypeDescription : ServiceTypeDescription
    {
        /// <summary>
        /// Initializes a new instance of the StatelessServiceTypeDescription class.
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
        /// <param name="useImplicitHost">A flag indicating if this type is not implemented and hosted by a user service
        /// process, but is implicitly hosted by a system created process. This value is true for services using the guest
        /// executable services, false otherwise.</param>
        public StatelessServiceTypeDescription(
            bool? isStateful = default(bool?),
            string serviceTypeName = default(string),
            string placementConstraints = default(string),
            IEnumerable<ServiceLoadMetricDescription> loadMetrics = default(IEnumerable<ServiceLoadMetricDescription>),
            IEnumerable<ServicePlacementPolicyDescription> servicePlacementPolicies = default(IEnumerable<ServicePlacementPolicyDescription>),
            IEnumerable<ServiceTypeExtensionDescription> extensions = default(IEnumerable<ServiceTypeExtensionDescription>),
            bool? useImplicitHost = default(bool?))
            : base(
                Common.ServiceKind.Stateless,
                isStateful,
                serviceTypeName,
                placementConstraints,
                loadMetrics,
                servicePlacementPolicies,
                extensions)
        {
            this.UseImplicitHost = useImplicitHost;
        }

        /// <summary>
        /// Gets a flag indicating if this type is not implemented and hosted by a user service process, but is implicitly
        /// hosted by a system created process. This value is true for services using the guest executable services, false
        /// otherwise.
        /// </summary>
        public bool? UseImplicitHost { get; }
    }
}
