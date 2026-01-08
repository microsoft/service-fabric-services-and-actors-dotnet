// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Creates the specified Service Fabric service.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SFService")]
    public partial class NewServiceCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Named flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_Named__Stateless_")]
        public SwitchParameter Named { get; set; }

        /// <summary>
        /// Gets or sets Singleton flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_Singleton__Stateless_")]
        public SwitchParameter Singleton { get; set; }

        /// <summary>
        /// Gets or sets UniformInt64Range flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_UniformInt64Range__Stateful_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_UniformInt64Range__Stateless_")]
        public SwitchParameter UniformInt64Range { get; set; }

        /// <summary>
        /// Gets or sets Stateful flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_UniformInt64Range__Stateful_")]
        public SwitchParameter Stateful { get; set; }

        /// <summary>
        /// Gets or sets Stateless flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_Named__Stateless_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_Singleton__Stateless_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_UniformInt64Range__Stateless_")]
        public SwitchParameter Stateless { get; set; }

        /// <summary>
        /// Gets or sets ApplicationId. The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 2)]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets ServiceName. The full name of the service with 'fabric:' URI scheme.
        /// </summary>
        [Parameter(Mandatory = true, Position = 3)]
        public ServiceName ServiceName { get; set; }

        /// <summary>
        /// Gets or sets ServiceTypeName. Name of the service type as specified in the service manifest.
        /// </summary>
        [Parameter(Mandatory = true, Position = 4)]
        public string ServiceTypeName { get; set; }

        /// <summary>
        /// Gets or sets Count. The number of partitions.
        /// </summary>
        [Parameter(Mandatory = true, Position = 5, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = true, Position = 5, ParameterSetName = "_Named__Stateless_")]
        [Parameter(Mandatory = true, Position = 5, ParameterSetName = "_UniformInt64Range__Stateful_")]
        [Parameter(Mandatory = true, Position = 5, ParameterSetName = "_UniformInt64Range__Stateless_")]
        public int? Count { get; set; }

        /// <summary>
        /// Gets or sets Names. Array of size specified by the ‘Count’ parameter, for the names of the partitions.
        /// </summary>
        [Parameter(Mandatory = true, Position = 6, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = true, Position = 6, ParameterSetName = "_Named__Stateless_")]
        public IEnumerable<string> Names { get; set; }

        /// <summary>
        /// Gets or sets LowKey. String indicating the lower bound of the partition key range that
        /// should be split between the partitions.
        /// </summary>
        [Parameter(Mandatory = true, Position = 7, ParameterSetName = "_UniformInt64Range__Stateful_")]
        [Parameter(Mandatory = true, Position = 7, ParameterSetName = "_UniformInt64Range__Stateless_")]
        public string LowKey { get; set; }

        /// <summary>
        /// Gets or sets HighKey. String indicating the upper bound of the partition key range that
        /// should be split between the partitions.
        /// </summary>
        [Parameter(Mandatory = true, Position = 8, ParameterSetName = "_UniformInt64Range__Stateful_")]
        [Parameter(Mandatory = true, Position = 8, ParameterSetName = "_UniformInt64Range__Stateless_")]
        public string HighKey { get; set; }

        /// <summary>
        /// Gets or sets TargetReplicaSetSize. The target replica set size as a number.
        /// </summary>
        [Parameter(Mandatory = true, Position = 9, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = true, Position = 9, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = true, Position = 9, ParameterSetName = "_UniformInt64Range__Stateful_")]
        public int? TargetReplicaSetSize { get; set; }

        /// <summary>
        /// Gets or sets MinReplicaSetSize. The minimum replica set size as a number.
        /// </summary>
        [Parameter(Mandatory = true, Position = 10, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = true, Position = 10, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = true, Position = 10, ParameterSetName = "_UniformInt64Range__Stateful_")]
        public int? MinReplicaSetSize { get; set; }

        /// <summary>
        /// Gets or sets HasPersistedState. A flag indicating whether this is a persistent service which stores states on the
        /// local disk. If it is then the value of this property is true, if not it is false.
        /// </summary>
        [Parameter(Mandatory = true, Position = 11, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = true, Position = 11, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = true, Position = 11, ParameterSetName = "_UniformInt64Range__Stateful_")]
        public bool? HasPersistedState { get; set; }

        /// <summary>
        /// Gets or sets InstanceCount. The instance count.
        /// </summary>
        [Parameter(Mandatory = true, Position = 12, ParameterSetName = "_Named__Stateless_")]
        [Parameter(Mandatory = true, Position = 12, ParameterSetName = "_Singleton__Stateless_")]
        [Parameter(Mandatory = true, Position = 12, ParameterSetName = "_UniformInt64Range__Stateless_")]
        public int? InstanceCount { get; set; }

        /// <summary>
        /// Gets or sets ApplicationName. The name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        [Parameter(Mandatory = false, Position = 13)]
        public ApplicationName ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets InitializationData. The initialization data as an array of bytes. Initialization data is passed to
        /// service instances or replicas when they are created.
        /// </summary>
        [Parameter(Mandatory = false, Position = 14)]
        public byte[] InitializationData { get; set; }

        /// <summary>
        /// Gets or sets PlacementConstraints. The placement constraints as a string. Placement constraints are boolean
        /// expressions on node properties and allow for restricting a service to particular nodes based on the service
        /// requirements. For example, to place a service on nodes where NodeType is blue specify the following: "NodeColor ==
        /// blue)".
        /// </summary>
        [Parameter(Mandatory = false, Position = 15)]
        public string PlacementConstraints { get; set; }

        /// <summary>
        /// Gets or sets CorrelationScheme. The correlation scheme.
        /// </summary>
        [Parameter(Mandatory = false, Position = 16)]
        public IEnumerable<ServiceCorrelationDescription> CorrelationScheme { get; set; }

        /// <summary>
        /// Gets or sets ServiceLoadMetrics. The service load metrics.
        /// </summary>
        [Parameter(Mandatory = false, Position = 17)]
        public IEnumerable<ServiceLoadMetricDescription> ServiceLoadMetrics { get; set; }

        /// <summary>
        /// Gets or sets ServicePlacementPolicies. The service placement policies.
        /// </summary>
        [Parameter(Mandatory = false, Position = 18)]
        public IEnumerable<ServicePlacementPolicyDescription> ServicePlacementPolicies { get; set; }

        /// <summary>
        /// Gets or sets DefaultMoveCost. The move cost for the service. Possible values include: 'Zero', 'Low', 'Medium',
        /// 'High', 'VeryHigh'
        /// 
        /// Specifies the move cost for the service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 19)]
        public MoveCost? DefaultMoveCost { get; set; }

        /// <summary>
        /// Gets or sets IsDefaultMoveCostSpecified. Indicates if the DefaultMoveCost property is specified.
        /// </summary>
        [Parameter(Mandatory = false, Position = 20)]
        public bool? IsDefaultMoveCostSpecified { get; set; }

        /// <summary>
        /// Gets or sets ServicePackageActivationMode. The activation mode of service package to be used for a service.
        /// Possible values include: 'SharedProcess', 'ExclusiveProcess'
        /// 
        /// The activation mode of service package to be used for a Service Fabric service. This is specified at the time of
        /// creating the Service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 21)]
        public ServicePackageActivationMode? ServicePackageActivationMode { get; set; }

        /// <summary>
        /// Gets or sets ServiceDnsName. The DNS name of the service. It requires the DNS system service to be enabled in
        /// Service Fabric cluster.
        /// </summary>
        [Parameter(Mandatory = false, Position = 22)]
        public string ServiceDnsName { get; set; }

        /// <summary>
        /// Gets or sets ScalingPolicies. Scaling policies for this service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 23)]
        public IEnumerable<ScalingPolicyDescription> ScalingPolicies { get; set; }

        /// <summary>
        /// Gets or sets TagsRequiredToPlace. Tags for placement of this service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 24)]
        public NodeTagsDescription TagsRequiredToPlace { get; set; }

        /// <summary>
        /// Gets or sets TagsRequiredToRun. Tags for running of this service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 25)]
        public NodeTagsDescription TagsRequiredToRun { get; set; }

        /// <summary>
        /// Gets or sets Flags. Flags indicating whether other properties are set. Each of the associated properties
        /// corresponds to a flag, specified below, which, if set, indicate that the property is specified.
        /// This property can be a combination of those flags obtained using bitwise 'OR' operator.
        /// For example, if the provided value is 6 then the flags for QuorumLossWaitDuration (2) and
        /// StandByReplicaKeepDuration(4) are set.
        /// 
        /// - None - Does not indicate any other properties are set. The value is zero.
        /// - ReplicaRestartWaitDuration - Indicates the ReplicaRestartWaitDuration property is set. The value is 1.
        /// - QuorumLossWaitDuration - Indicates the QuorumLossWaitDuration property is set. The value is 2.
        /// - StandByReplicaKeepDuration - Indicates the StandByReplicaKeepDuration property is set. The value is 4.
        /// - ServicePlacementTimeLimit - Indicates the ServicePlacementTimeLimit property is set. The value is 8.
        /// - DropSourceReplicaOnMove - Indicates the DropSourceReplicaOnMove property is set. The value is 16.
        /// </summary>
        [Parameter(Mandatory = false, Position = 26, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = false, Position = 26, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = false, Position = 26, ParameterSetName = "_UniformInt64Range__Stateful_")]
        [Parameter(Mandatory = false, Position = 26, ParameterSetName = "_Named__Stateless_")]
        [Parameter(Mandatory = false, Position = 26, ParameterSetName = "_Singleton__Stateless_")]
        [Parameter(Mandatory = false, Position = 26, ParameterSetName = "_UniformInt64Range__Stateless_")]
        public int? Flags { get; set; }

        /// <summary>
        /// Gets or sets ReplicaRestartWaitDurationSeconds. The duration, in seconds, between when a replica goes down and when
        /// a new replica is created.
        /// </summary>
        [Parameter(Mandatory = false, Position = 27, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = false, Position = 27, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = false, Position = 27, ParameterSetName = "_UniformInt64Range__Stateful_")]
        public long? ReplicaRestartWaitDurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets QuorumLossWaitDurationSeconds. The maximum duration, in seconds, for which a partition is allowed to
        /// be in a state of quorum loss.
        /// </summary>
        [Parameter(Mandatory = false, Position = 28, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = false, Position = 28, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = false, Position = 28, ParameterSetName = "_UniformInt64Range__Stateful_")]
        public long? QuorumLossWaitDurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets StandByReplicaKeepDurationSeconds. Defines how long StandBy replicas should be maintained before being
        /// removed.
        /// </summary>
        [Parameter(Mandatory = false, Position = 29, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = false, Position = 29, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = false, Position = 29, ParameterSetName = "_UniformInt64Range__Stateful_")]
        public long? StandByReplicaKeepDurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets ServicePlacementTimeLimitSeconds. The duration for which replicas can stay InBuild before reporting
        /// that build is stuck.
        /// </summary>
        [Parameter(Mandatory = false, Position = 30, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = false, Position = 30, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = false, Position = 30, ParameterSetName = "_UniformInt64Range__Stateful_")]
        public long? ServicePlacementTimeLimitSeconds { get; set; }

        /// <summary>
        /// Gets or sets DropSourceReplicaOnMove. Indicates whether to drop source Secondary replica even if the target replica
        /// has not finished build. If desired behavior is to drop it as soon as possible the value of this property is true,
        /// if not it is false.
        /// </summary>
        [Parameter(Mandatory = false, Position = 31, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = false, Position = 31, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = false, Position = 31, ParameterSetName = "_UniformInt64Range__Stateful_")]
        public bool? DropSourceReplicaOnMove { get; set; }

        /// <summary>
        /// Gets or sets ReplicaLifecycleDescription. Defines how replicas of this service will behave during ther lifecycle.
        /// </summary>
        [Parameter(Mandatory = false, Position = 32, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = false, Position = 32, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = false, Position = 32, ParameterSetName = "_UniformInt64Range__Stateful_")]
        public ReplicaLifecycleDescription ReplicaLifecycleDescription { get; set; }

        /// <summary>
        /// Gets or sets AuxiliaryReplicaCount. The auxiliary replica count as a number. To use Auxiliary replicas, the
        /// following must be true: AuxiliaryReplicaCount &lt; (TargetReplicaSetSize+1)/2 and TargetReplicaSetSize >=3.
        /// </summary>
        [Parameter(Mandatory = false, Position = 33, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = false, Position = 33, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = false, Position = 33, ParameterSetName = "_UniformInt64Range__Stateful_")]
        public int? AuxiliaryReplicaCount { get; set; }

        /// <summary>
        /// Gets or sets ServiceSensitivityDescription. Defines default levels of replica sensitivity of this service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 34, ParameterSetName = "_Named__Stateful_")]
        [Parameter(Mandatory = false, Position = 34, ParameterSetName = "_Singleton__Stateful_")]
        [Parameter(Mandatory = false, Position = 34, ParameterSetName = "_UniformInt64Range__Stateful_")]
        public ServiceSensitivityDescription ServiceSensitivityDescription { get; set; }

        /// <summary>
        /// Gets or sets MinInstanceCount. MinInstanceCount is the minimum number of instances that must be up to meet the
        /// EnsureAvailability safety check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstanceCount computation -1 is first converted into the number of
        /// nodes on which the instances are allowed to be placed according to the placement constraints on the service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 35, ParameterSetName = "_Named__Stateless_")]
        [Parameter(Mandatory = false, Position = 35, ParameterSetName = "_Singleton__Stateless_")]
        [Parameter(Mandatory = false, Position = 35, ParameterSetName = "_UniformInt64Range__Stateless_")]
        public int? MinInstanceCount { get; set; }

        /// <summary>
        /// Gets or sets MinInstancePercentage. MinInstancePercentage is the minimum percentage of InstanceCount that must be
        /// up to meet the EnsureAvailability safety check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstancePercentage computation, -1 is first converted into the
        /// number of nodes on which the instances are allowed to be placed according to the placement constraints on the
        /// service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 36, ParameterSetName = "_Named__Stateless_")]
        [Parameter(Mandatory = false, Position = 36, ParameterSetName = "_Singleton__Stateless_")]
        [Parameter(Mandatory = false, Position = 36, ParameterSetName = "_UniformInt64Range__Stateless_")]
        public int? MinInstancePercentage { get; set; }

        /// <summary>
        /// Gets or sets InstanceCloseDelayDurationSeconds. Duration in seconds, to wait before a stateless instance is closed,
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
        /// </summary>
        [Parameter(Mandatory = false, Position = 37, ParameterSetName = "_Named__Stateless_")]
        [Parameter(Mandatory = false, Position = 37, ParameterSetName = "_Singleton__Stateless_")]
        [Parameter(Mandatory = false, Position = 37, ParameterSetName = "_UniformInt64Range__Stateless_")]
        public long? InstanceCloseDelayDurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets InstanceLifecycleDescription. Defines how instances of this service will behave during their
        /// lifecycle.
        /// </summary>
        [Parameter(Mandatory = false, Position = 38, ParameterSetName = "_Named__Stateless_")]
        [Parameter(Mandatory = false, Position = 38, ParameterSetName = "_Singleton__Stateless_")]
        [Parameter(Mandatory = false, Position = 38, ParameterSetName = "_UniformInt64Range__Stateless_")]
        public InstanceLifecycleDescription InstanceLifecycleDescription { get; set; }

        /// <summary>
        /// Gets or sets InstanceRestartWaitDurationSeconds. When a stateless instance goes down, this timer starts. When it
        /// expires Service Fabric will create a new instance on any node in the cluster.
        /// This configuration is to reduce unnecessary creation of a new instance in situations where the instance going down
        /// is likely to recover in a short time. For example, during an upgrade.
        /// The default value is 0, which indicates that when stateless instance goes down, Service Fabric will immediately
        /// start building its replacement.
        /// </summary>
        [Parameter(Mandatory = false, Position = 39, ParameterSetName = "_Named__Stateless_")]
        [Parameter(Mandatory = false, Position = 39, ParameterSetName = "_Singleton__Stateless_")]
        [Parameter(Mandatory = false, Position = 39, ParameterSetName = "_UniformInt64Range__Stateless_")]
        public long? InstanceRestartWaitDurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 40)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            PartitionSchemeDescription partitionSchemeDescription = null;
            if (this.Named.IsPresent)
            {
                partitionSchemeDescription = new NamedPartitionSchemeDescription(
                    count: this.Count,
                    names: this.Names);
            }
            else if (this.Singleton.IsPresent)
            {
                partitionSchemeDescription = new SingletonPartitionSchemeDescription();
            }
            else if (this.UniformInt64Range.IsPresent)
            {
                partitionSchemeDescription = new UniformInt64RangePartitionSchemeDescription(
                    count: this.Count,
                    lowKey: this.LowKey,
                    highKey: this.HighKey);
            }

            ServiceDescription serviceDescription = null;
            if (this.Stateful.IsPresent)
            {
                serviceDescription = new StatefulServiceDescription(
                    serviceName: this.ServiceName,
                    serviceTypeName: this.ServiceTypeName,
                    partitionDescription: partitionSchemeDescription,
                    targetReplicaSetSize: this.TargetReplicaSetSize,
                    minReplicaSetSize: this.MinReplicaSetSize,
                    hasPersistedState: this.HasPersistedState,
                    applicationName: this.ApplicationName,
                    initializationData: this.InitializationData,
                    placementConstraints: this.PlacementConstraints,
                    correlationScheme: this.CorrelationScheme,
                    serviceLoadMetrics: this.ServiceLoadMetrics,
                    servicePlacementPolicies: this.ServicePlacementPolicies,
                    defaultMoveCost: this.DefaultMoveCost,
                    isDefaultMoveCostSpecified: this.IsDefaultMoveCostSpecified,
                    servicePackageActivationMode: this.ServicePackageActivationMode,
                    serviceDnsName: this.ServiceDnsName,
                    scalingPolicies: this.ScalingPolicies,
                    tagsRequiredToPlace: this.TagsRequiredToPlace,
                    tagsRequiredToRun: this.TagsRequiredToRun,
                    flags: this.Flags,
                    replicaRestartWaitDurationSeconds: this.ReplicaRestartWaitDurationSeconds,
                    quorumLossWaitDurationSeconds: this.QuorumLossWaitDurationSeconds,
                    standByReplicaKeepDurationSeconds: this.StandByReplicaKeepDurationSeconds,
                    servicePlacementTimeLimitSeconds: this.ServicePlacementTimeLimitSeconds,
                    dropSourceReplicaOnMove: this.DropSourceReplicaOnMove,
                    replicaLifecycleDescription: this.ReplicaLifecycleDescription,
                    auxiliaryReplicaCount: this.AuxiliaryReplicaCount,
                    serviceSensitivityDescription: this.ServiceSensitivityDescription);
            }
            else if (this.Stateless.IsPresent)
            {
                serviceDescription = new StatelessServiceDescription(
                    serviceName: this.ServiceName,
                    serviceTypeName: this.ServiceTypeName,
                    partitionDescription: partitionSchemeDescription,
                    instanceCount: this.InstanceCount,
                    applicationName: this.ApplicationName,
                    initializationData: this.InitializationData,
                    placementConstraints: this.PlacementConstraints,
                    correlationScheme: this.CorrelationScheme,
                    serviceLoadMetrics: this.ServiceLoadMetrics,
                    servicePlacementPolicies: this.ServicePlacementPolicies,
                    defaultMoveCost: this.DefaultMoveCost,
                    isDefaultMoveCostSpecified: this.IsDefaultMoveCostSpecified,
                    servicePackageActivationMode: this.ServicePackageActivationMode,
                    serviceDnsName: this.ServiceDnsName,
                    scalingPolicies: this.ScalingPolicies,
                    tagsRequiredToPlace: this.TagsRequiredToPlace,
                    tagsRequiredToRun: this.TagsRequiredToRun,
                    minInstanceCount: this.MinInstanceCount,
                    minInstancePercentage: this.MinInstancePercentage,
                    flags: this.Flags,
                    instanceCloseDelayDurationSeconds: this.InstanceCloseDelayDurationSeconds,
                    instanceLifecycleDescription: this.InstanceLifecycleDescription,
                    instanceRestartWaitDurationSeconds: this.InstanceRestartWaitDurationSeconds);
            }

            this.ServiceFabricClient.Services.CreateServiceAsync(
                applicationId: this.ApplicationId,
                serviceDescription: serviceDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
