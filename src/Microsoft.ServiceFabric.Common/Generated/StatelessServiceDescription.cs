// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a stateless service.
    /// </summary>
    public partial class StatelessServiceDescription : ServiceDescription
    {
        /// <summary>
        /// Initializes a new instance of the StatelessServiceDescription class.
        /// </summary>
        /// <param name="serviceName">The full name of the service with 'fabric:' URI scheme.</param>
        /// <param name="serviceTypeName">Name of the service type as specified in the service manifest.</param>
        /// <param name="partitionDescription">The partition description as an object.</param>
        /// <param name="instanceCount">The instance count.</param>
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
        /// <param name="minInstanceCount">MinInstanceCount is the minimum number of instances that must be up to meet the
        /// EnsureAvailability safety check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstanceCount computation -1 is first converted into the number of
        /// nodes on which the instances are allowed to be placed according to the placement constraints on the service.
        /// </param>
        /// <param name="minInstancePercentage">MinInstancePercentage is the minimum percentage of InstanceCount that must be
        /// up to meet the EnsureAvailability safety check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstancePercentage computation, -1 is first converted into the
        /// number of nodes on which the instances are allowed to be placed according to the placement constraints on the
        /// service.
        /// </param>
        /// <param name="flags">Flags indicating whether other properties are set. Each of the associated properties
        /// corresponds to a flag, specified below, which, if set, indicate that the property is specified.
        /// This property can be a combination of those flags obtained using bitwise 'OR' operator.
        /// For example, if the provided value is 1 then the flags for InstanceCloseDelayDuration is set.
        /// 
        /// - None - Does not indicate any other properties are set. The value is zero.
        /// - InstanceCloseDelayDuration - Indicates the InstanceCloseDelayDuration property is set. The value is 1.
        /// - InstanceRestartWaitDuration - Indicates the InstanceRestartWaitDurationSeconds property is set. The value is 2.
        /// </param>
        /// <param name="instanceCloseDelayDurationSeconds">Duration in seconds, to wait before a stateless instance is closed,
        /// to allow the active requests to drain gracefully. This would be effective when the instance is closing during the
        /// application/cluster upgrade and disabling node.
        /// The endpoint exposed on this instance is removed prior to starting the delay, which prevents new connections to
        /// this instance.
        /// In addition, clients that have subscribed to service endpoint change
        /// events(https://docs.microsoft.com/dotnet/api/system.fabric.fabricclient.servicemanagementclient.registerservicenotificationfilterasync),
        /// can do
        /// the following upon receiving the endpoint removal notification:
        /// - Stop sending new requests to this instance.
        /// - Close existing connections after in-flight requests have completed.
        /// - Connect to a different instance of the service partition for future requests.
        /// Note, the default value of InstanceCloseDelayDuration is 0, which indicates that there won't be any delay or
        /// removal of the endpoint prior to closing the instance.
        /// </param>
        /// <param name="instanceLifecycleDescription">Defines how instances of this service will behave during their
        /// lifecycle.</param>
        /// <param name="instanceRestartWaitDurationSeconds">When a stateless instance goes down, this timer starts. When it
        /// expires Service Fabric will create a new instance on any node in the cluster.
        /// This configuration is to reduce unnecessary creation of a new instance in situations where the instance going down
        /// is likely to recover in a short time. For example, during an upgrade.
        /// The default value is 0, which indicates that when stateless instance goes down, Service Fabric will immediately
        /// start building its replacement.
        /// </param>
        public StatelessServiceDescription(
            ServiceName serviceName,
            string serviceTypeName,
            PartitionSchemeDescription partitionDescription,
            int? instanceCount,
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
            NodeTagsDescription tagsRequiredToRun = default(NodeTagsDescription),
            int? minInstanceCount = default(int?),
            int? minInstancePercentage = default(int?),
            int? flags = default(int?),
            long? instanceCloseDelayDurationSeconds = default(long?),
            InstanceLifecycleDescription instanceLifecycleDescription = default(InstanceLifecycleDescription),
            long? instanceRestartWaitDurationSeconds = default(long?))
            : base(
                serviceName,
                serviceTypeName,
                partitionDescription,
                Common.ServiceKind.Stateless,
                applicationName,
                initializationData,
                placementConstraints,
                correlationScheme,
                serviceLoadMetrics,
                servicePlacementPolicies,
                defaultMoveCost,
                isDefaultMoveCostSpecified,
                servicePackageActivationMode,
                serviceDnsName,
                scalingPolicies,
                tagsRequiredToPlace,
                tagsRequiredToRun)
        {
            instanceCount.ThrowIfNull(nameof(instanceCount));
            instanceCount?.ThrowIfLessThan("instanceCount", -1);
            instanceCloseDelayDurationSeconds?.ThrowIfOutOfInclusiveRange("instanceCloseDelayDurationSeconds", 0, 4294967295);
            instanceRestartWaitDurationSeconds?.ThrowIfOutOfInclusiveRange("instanceRestartWaitDurationSeconds", 0, 4294967295);
            this.InstanceCount = instanceCount;
            this.MinInstanceCount = minInstanceCount;
            this.MinInstancePercentage = minInstancePercentage;
            this.Flags = flags;
            this.InstanceCloseDelayDurationSeconds = instanceCloseDelayDurationSeconds;
            this.InstanceLifecycleDescription = instanceLifecycleDescription;
            this.InstanceRestartWaitDurationSeconds = instanceRestartWaitDurationSeconds;
        }

        /// <summary>
        /// Gets the instance count.
        /// </summary>
        public int? InstanceCount { get; }

        /// <summary>
        /// Gets minInstanceCount is the minimum number of instances that must be up to meet the EnsureAvailability safety
        /// check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstanceCount computation -1 is first converted into the number of
        /// nodes on which the instances are allowed to be placed according to the placement constraints on the service.
        /// </summary>
        public int? MinInstanceCount { get; }

        /// <summary>
        /// Gets minInstancePercentage is the minimum percentage of InstanceCount that must be up to meet the
        /// EnsureAvailability safety check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstancePercentage computation, -1 is first converted into the
        /// number of nodes on which the instances are allowed to be placed according to the placement constraints on the
        /// service.
        /// </summary>
        public int? MinInstancePercentage { get; }

        /// <summary>
        /// Gets flags indicating whether other properties are set. Each of the associated properties corresponds to a flag,
        /// specified below, which, if set, indicate that the property is specified.
        /// This property can be a combination of those flags obtained using bitwise 'OR' operator.
        /// For example, if the provided value is 1 then the flags for InstanceCloseDelayDuration is set.
        /// 
        /// - None - Does not indicate any other properties are set. The value is zero.
        /// - InstanceCloseDelayDuration - Indicates the InstanceCloseDelayDuration property is set. The value is 1.
        /// - InstanceRestartWaitDuration - Indicates the InstanceRestartWaitDurationSeconds property is set. The value is 2.
        /// </summary>
        public int? Flags { get; }

        /// <summary>
        /// Gets duration in seconds, to wait before a stateless instance is closed, to allow the active requests to drain
        /// gracefully. This would be effective when the instance is closing during the application/cluster upgrade and
        /// disabling node.
        /// The endpoint exposed on this instance is removed prior to starting the delay, which prevents new connections to
        /// this instance.
        /// In addition, clients that have subscribed to service endpoint change
        /// events(https://docs.microsoft.com/dotnet/api/system.fabric.fabricclient.servicemanagementclient.registerservicenotificationfilterasync),
        /// can do
        /// the following upon receiving the endpoint removal notification:
        /// - Stop sending new requests to this instance.
        /// - Close existing connections after in-flight requests have completed.
        /// - Connect to a different instance of the service partition for future requests.
        /// Note, the default value of InstanceCloseDelayDuration is 0, which indicates that there won't be any delay or
        /// removal of the endpoint prior to closing the instance.
        /// </summary>
        public long? InstanceCloseDelayDurationSeconds { get; }

        /// <summary>
        /// Gets defines how instances of this service will behave during their lifecycle.
        /// </summary>
        public InstanceLifecycleDescription InstanceLifecycleDescription { get; }

        /// <summary>
        /// Gets when a stateless instance goes down, this timer starts. When it expires Service Fabric will create a new
        /// instance on any node in the cluster.
        /// This configuration is to reduce unnecessary creation of a new instance in situations where the instance going down
        /// is likely to recover in a short time. For example, during an upgrade.
        /// The default value is 0, which indicates that when stateless instance goes down, Service Fabric will immediately
        /// start building its replacement.
        /// </summary>
        public long? InstanceRestartWaitDurationSeconds { get; }
    }
}
