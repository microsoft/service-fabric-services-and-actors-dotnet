// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a service type defined in the service manifest of a provisioned application type. The properties the ones
    /// defined in the service manifest.
    /// </summary>
    public abstract partial class ServiceTypeDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServiceTypeDescription class.
        /// </summary>
        /// <param name="kind">The kind of service (Stateless or Stateful).</param>
        /// <param name="isStateful">Indicates whether the service type is a stateful service type or a stateless service type.
        /// This property is true if the service type is a stateful service type, false otherwise.</param>
        /// <param name="serviceTypeName">Name of the service type as specified in the service manifest.</param>
        /// <param name="placementConstraints">The placement constraint to be used when instantiating this service in a Service
        /// Fabric cluster.</param>
        /// <param name="loadMetrics">The service load metrics is given as an array of ServiceLoadMetricDescription
        /// objects.</param>
        /// <param name="servicePlacementPolicies">List of service placement policy descriptions.</param>
        /// <param name="extensions">List of service type extensions.</param>
        protected ServiceTypeDescription(
            ServiceKind? kind,
            bool? isStateful = default(bool?),
            string serviceTypeName = default(string),
            string placementConstraints = default(string),
            IEnumerable<ServiceLoadMetricDescription> loadMetrics = default(IEnumerable<ServiceLoadMetricDescription>),
            IEnumerable<ServicePlacementPolicyDescription> servicePlacementPolicies = default(IEnumerable<ServicePlacementPolicyDescription>),
            IEnumerable<ServiceTypeExtensionDescription> extensions = default(IEnumerable<ServiceTypeExtensionDescription>))
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
            this.IsStateful = isStateful;
            this.ServiceTypeName = serviceTypeName;
            this.PlacementConstraints = placementConstraints;
            this.LoadMetrics = loadMetrics;
            this.ServicePlacementPolicies = servicePlacementPolicies;
            this.Extensions = extensions;
        }

        /// <summary>
        /// Gets indicates whether the service type is a stateful service type or a stateless service type. This property is
        /// true if the service type is a stateful service type, false otherwise.
        /// </summary>
        public bool? IsStateful { get; }

        /// <summary>
        /// Gets name of the service type as specified in the service manifest.
        /// </summary>
        public string ServiceTypeName { get; }

        /// <summary>
        /// Gets the placement constraint to be used when instantiating this service in a Service Fabric cluster.
        /// </summary>
        public string PlacementConstraints { get; }

        /// <summary>
        /// Gets the service load metrics is given as an array of ServiceLoadMetricDescription objects.
        /// </summary>
        public IEnumerable<ServiceLoadMetricDescription> LoadMetrics { get; }

        /// <summary>
        /// Gets list of service placement policy descriptions.
        /// </summary>
        public IEnumerable<ServicePlacementPolicyDescription> ServicePlacementPolicies { get; }

        /// <summary>
        /// Gets list of service type extensions.
        /// </summary>
        public IEnumerable<ServiceTypeExtensionDescription> Extensions { get; }

        /// <summary>
        /// Gets the kind of service (Stateless or Stateful).
        /// </summary>
        public ServiceKind? Kind { get; }
    }
}
