// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A ServiceDescription contains all of the information necessary to create a service.
    /// </summary>
    public abstract partial class ServiceDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServiceDescription class.
        /// </summary>
        /// <param name="serviceName">The full name of the service with 'fabric:' URI scheme.</param>
        /// <param name="serviceTypeName">Name of the service type as specified in the service manifest.</param>
        /// <param name="partitionDescription">The partition description as an object.</param>
        /// <param name="serviceKind">The kind of service (Stateless or Stateful).</param>
        /// <param name="applicationName">The name of the application, including the 'fabric:' URI scheme.</param>
        /// <param name="initializationData">The initialization data as an array of bytes. Initialization data is passed to
        /// service instances or replicas when they are created.</param>
        /// <param name="placementConstraints">The placement constraints as a string. Placement constraints are boolean
        /// expressions on node properties and allow for restricting a service to particular nodes based on the service
        /// requirements. For example, to place a service on nodes where NodeType is blue specify the following: "NodeColor ==
        /// blue)".</param>
        /// <param name="correlationScheme">The correlation scheme.</param>
        /// <param name="serviceLoadMetrics">The service load metrics.</param>
        /// <param name="servicePlacementPolicies">The service placement policies.</param>
        /// <param name="defaultMoveCost">The move cost for the service. Possible values include: 'Zero', 'Low', 'Medium',
        /// 'High', 'VeryHigh'
        /// 
        /// Specifies the move cost for the service.
        /// </param>
        /// <param name="isDefaultMoveCostSpecified">Indicates if the DefaultMoveCost property is specified.</param>
        /// <param name="servicePackageActivationMode">The activation mode of service package to be used for a service.
        /// Possible values include: 'SharedProcess', 'ExclusiveProcess'
        /// 
        /// The activation mode of service package to be used for a Service Fabric service. This is specified at the time of
        /// creating the Service.
        /// </param>
        /// <param name="serviceDnsName">The DNS name of the service. It requires the DNS system service to be enabled in
        /// Service Fabric cluster.</param>
        /// <param name="scalingPolicies">Scaling policies for this service.</param>
        /// <param name="tagsRequiredToPlace">Tags for placement of this service.</param>
        /// <param name="tagsRequiredToRun">Tags for running of this service.</param>
        protected ServiceDescription(
            ServiceName serviceName,
            string serviceTypeName,
            PartitionSchemeDescription partitionDescription,
            ServiceKind? serviceKind,
            ApplicationName applicationName = default(ApplicationName),
            byte[] initializationData = default(byte[]),
            string placementConstraints = default(string),
            IEnumerable<ServiceCorrelationDescription> correlationScheme = default(IEnumerable<ServiceCorrelationDescription>),
            IEnumerable<ServiceLoadMetricDescription> serviceLoadMetrics = default(IEnumerable<ServiceLoadMetricDescription>),
            IEnumerable<ServicePlacementPolicyDescription> servicePlacementPolicies = default(IEnumerable<ServicePlacementPolicyDescription>),
            MoveCost? defaultMoveCost = default(MoveCost?),
            bool? isDefaultMoveCostSpecified = default(bool?),
            ServicePackageActivationMode? servicePackageActivationMode = default(ServicePackageActivationMode?),
            string serviceDnsName = default(string),
            IEnumerable<ScalingPolicyDescription> scalingPolicies = default(IEnumerable<ScalingPolicyDescription>),
            NodeTagsDescription tagsRequiredToPlace = default(NodeTagsDescription),
            NodeTagsDescription tagsRequiredToRun = default(NodeTagsDescription))
        {
            serviceName.ThrowIfNull(nameof(serviceName));
            serviceTypeName.ThrowIfNull(nameof(serviceTypeName));
            partitionDescription.ThrowIfNull(nameof(partitionDescription));
            serviceKind.ThrowIfNull(nameof(serviceKind));
            this.ServiceName = serviceName;
            this.ServiceTypeName = serviceTypeName;
            this.PartitionDescription = partitionDescription;
            this.ServiceKind = serviceKind;
            this.ApplicationName = applicationName;
            this.InitializationData = initializationData;
            this.PlacementConstraints = placementConstraints;
            this.CorrelationScheme = correlationScheme;
            this.ServiceLoadMetrics = serviceLoadMetrics;
            this.ServicePlacementPolicies = servicePlacementPolicies;
            this.DefaultMoveCost = defaultMoveCost;
            this.IsDefaultMoveCostSpecified = isDefaultMoveCostSpecified;
            this.ServicePackageActivationMode = servicePackageActivationMode;
            this.ServiceDnsName = serviceDnsName;
            this.ScalingPolicies = scalingPolicies;
            this.TagsRequiredToPlace = tagsRequiredToPlace;
            this.TagsRequiredToRun = tagsRequiredToRun;
        }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName ApplicationName { get; }

        /// <summary>
        /// Gets the full name of the service with 'fabric:' URI scheme.
        /// </summary>
        public ServiceName ServiceName { get; }

        /// <summary>
        /// Gets name of the service type as specified in the service manifest.
        /// </summary>
        public string ServiceTypeName { get; }

        /// <summary>
        /// Gets the initialization data as an array of bytes. Initialization data is passed to service instances or replicas
        /// when they are created.
        /// </summary>
        public byte[] InitializationData { get; }

        /// <summary>
        /// Gets the partition description as an object.
        /// </summary>
        public PartitionSchemeDescription PartitionDescription { get; }

        /// <summary>
        /// Gets the placement constraints as a string. Placement constraints are boolean expressions on node properties and
        /// allow for restricting a service to particular nodes based on the service requirements. For example, to place a
        /// service on nodes where NodeType is blue specify the following: "NodeColor == blue)".
        /// </summary>
        public string PlacementConstraints { get; }

        /// <summary>
        /// Gets the correlation scheme.
        /// </summary>
        public IEnumerable<ServiceCorrelationDescription> CorrelationScheme { get; }

        /// <summary>
        /// Gets the service load metrics.
        /// </summary>
        public IEnumerable<ServiceLoadMetricDescription> ServiceLoadMetrics { get; }

        /// <summary>
        /// Gets the service placement policies.
        /// </summary>
        public IEnumerable<ServicePlacementPolicyDescription> ServicePlacementPolicies { get; }

        /// <summary>
        /// Gets the move cost for the service. Possible values include: 'Zero', 'Low', 'Medium', 'High', 'VeryHigh'
        /// 
        /// Specifies the move cost for the service.
        /// </summary>
        public MoveCost? DefaultMoveCost { get; }

        /// <summary>
        /// Gets indicates if the DefaultMoveCost property is specified.
        /// </summary>
        public bool? IsDefaultMoveCostSpecified { get; }

        /// <summary>
        /// Gets the activation mode of service package to be used for a service. Possible values include: 'SharedProcess',
        /// 'ExclusiveProcess'
        /// 
        /// The activation mode of service package to be used for a Service Fabric service. This is specified at the time of
        /// creating the Service.
        /// </summary>
        public ServicePackageActivationMode? ServicePackageActivationMode { get; }

        /// <summary>
        /// Gets the DNS name of the service. It requires the DNS system service to be enabled in Service Fabric cluster.
        /// </summary>
        public string ServiceDnsName { get; }

        /// <summary>
        /// Gets scaling policies for this service.
        /// </summary>
        public IEnumerable<ScalingPolicyDescription> ScalingPolicies { get; }

        /// <summary>
        /// Gets tags for placement of this service.
        /// </summary>
        public NodeTagsDescription TagsRequiredToPlace { get; }

        /// <summary>
        /// Gets tags for running of this service.
        /// </summary>
        public NodeTagsDescription TagsRequiredToRun { get; }

        /// <summary>
        /// Gets the kind of service (Stateless or Stateful).
        /// </summary>
        public ServiceKind? ServiceKind { get; }
    }
}
