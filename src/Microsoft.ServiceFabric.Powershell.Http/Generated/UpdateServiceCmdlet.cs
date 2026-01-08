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
    /// Updates a Service Fabric service using the specified update description.
    /// </summary>
    [Cmdlet(VerbsData.Update, "SFService")]
    public partial class UpdateServiceCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Stateful flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_Stateful_")]
        public SwitchParameter Stateful { get; set; }

        /// <summary>
        /// Gets or sets Stateless flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_Stateless_")]
        public SwitchParameter Stateless { get; set; }

        /// <summary>
        /// Gets or sets ServiceId. The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public string ServiceId { get; set; }

        /// <summary>
        /// Gets or sets Flags. Flags indicating whether other properties are set. Each of the associated properties
        /// corresponds to a flag, specified below, which, if set, indicate that the property is specified.
        /// This property can be a combination of those flags obtained using bitwise 'OR' operator.
        /// For example, if the provided value is 6 then the flags for ReplicaRestartWaitDuration (2) and
        /// QuorumLossWaitDuration (4) are set.
        /// 
        /// - None - Does not indicate any other properties are set. The value is zero.
        /// - TargetReplicaSetSize/InstanceCount - Indicates whether the TargetReplicaSetSize property (for Stateful services)
        /// or the InstanceCount property (for Stateless services) is set. The value is 1.
        /// - ReplicaRestartWaitDuration - Indicates the ReplicaRestartWaitDuration property is set. The value is  2.
        /// - QuorumLossWaitDuration - Indicates the QuorumLossWaitDuration property is set. The value is 4.
        /// - StandByReplicaKeepDuration - Indicates the StandByReplicaKeepDuration property is set. The value is 8.
        /// - MinReplicaSetSize - Indicates the MinReplicaSetSize property is set. The value is 16.
        /// - PlacementConstraints - Indicates the PlacementConstraints property is set. The value is 32.
        /// - PlacementPolicyList - Indicates the ServicePlacementPolicies property is set. The value is 64.
        /// - Correlation - Indicates the CorrelationScheme property is set. The value is 128.
        /// - Metrics - Indicates the ServiceLoadMetrics property is set. The value is 256.
        /// - DefaultMoveCost - Indicates the DefaultMoveCost property is set. The value is 512.
        /// - ScalingPolicy - Indicates the ScalingPolicies property is set. The value is 1024.
        /// - ServicePlacementTimeLimit - Indicates the ServicePlacementTimeLimit property is set. The value is 2048.
        /// - MinInstanceCount - Indicates the MinInstanceCount property is set. The value is 4096.
        /// - MinInstancePercentage - Indicates the MinInstancePercentage property is set. The value is 8192.
        /// - InstanceCloseDelayDuration - Indicates the InstanceCloseDelayDuration property is set. The value is 16384.
        /// - InstanceRestartWaitDuration - Indicates the InstanceCloseDelayDuration property is set. The value is 32768.
        /// - DropSourceReplicaOnMove - Indicates the DropSourceReplicaOnMove property is set. The value is 65536.
        /// - ServiceDnsName - Indicates the ServiceDnsName property is set. The value is 131072.
        /// - TagsForPlacement - Indicates the TagsForPlacement property is set. The value is 1048576.
        /// - TagsForRunning - Indicates the TagsForRunning property is set. The value is 2097152.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public string Flags { get; set; }

        /// <summary>
        /// Gets or sets PlacementConstraints. The placement constraints as a string. Placement constraints are boolean
        /// expressions on node properties and allow for restricting a service to particular nodes based on the service
        /// requirements. For example, to place a service on nodes where NodeType is blue specify the following: "NodeColor ==
        /// blue)".
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public string PlacementConstraints { get; set; }

        /// <summary>
        /// Gets or sets CorrelationScheme. The correlation scheme.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public IEnumerable<ServiceCorrelationDescription> CorrelationScheme { get; set; }

        /// <summary>
        /// Gets or sets LoadMetrics. The service load metrics.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public IEnumerable<ServiceLoadMetricDescription> LoadMetrics { get; set; }

        /// <summary>
        /// Gets or sets ServicePlacementPolicies. The service placement policies.
        /// </summary>
        [Parameter(Mandatory = false, Position = 6)]
        public IEnumerable<ServicePlacementPolicyDescription> ServicePlacementPolicies { get; set; }

        /// <summary>
        /// Gets or sets DefaultMoveCost. The move cost for the service. Possible values include: 'Zero', 'Low', 'Medium',
        /// 'High', 'VeryHigh'
        /// 
        /// Specifies the move cost for the service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 7)]
        public MoveCost? DefaultMoveCost { get; set; }

        /// <summary>
        /// Gets or sets ScalingPolicies. Scaling policies for this service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 8)]
        public IEnumerable<ScalingPolicyDescription> ScalingPolicies { get; set; }

        /// <summary>
        /// Gets or sets ServiceDnsName. The DNS name of the service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 9)]
        public string ServiceDnsName { get; set; }

        /// <summary>
        /// Gets or sets TagsForPlacement. Tags for placement of this service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 10)]
        public NodeTagsDescription TagsForPlacement { get; set; }

        /// <summary>
        /// Gets or sets TagsForRunning. Tags for running of this service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 11)]
        public NodeTagsDescription TagsForRunning { get; set; }

        /// <summary>
        /// Gets or sets TargetReplicaSetSize. The target replica set size as a number.
        /// </summary>
        [Parameter(Mandatory = false, Position = 12, ParameterSetName = "_Stateful_")]
        public int? TargetReplicaSetSize { get; set; }

        /// <summary>
        /// Gets or sets MinReplicaSetSize. The minimum replica set size as a number.
        /// </summary>
        [Parameter(Mandatory = false, Position = 13, ParameterSetName = "_Stateful_")]
        public int? MinReplicaSetSize { get; set; }

        /// <summary>
        /// Gets or sets ReplicaRestartWaitDurationSeconds. The duration, in seconds, between when a replica goes down and when
        /// a new replica is created.
        /// </summary>
        [Parameter(Mandatory = false, Position = 14, ParameterSetName = "_Stateful_")]
        public string ReplicaRestartWaitDurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets QuorumLossWaitDurationSeconds. The maximum duration, in seconds, for which a partition is allowed to
        /// be in a state of quorum loss.
        /// </summary>
        [Parameter(Mandatory = false, Position = 15, ParameterSetName = "_Stateful_")]
        public string QuorumLossWaitDurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets StandByReplicaKeepDurationSeconds. The definition on how long StandBy replicas should be maintained
        /// before being removed.
        /// </summary>
        [Parameter(Mandatory = false, Position = 16, ParameterSetName = "_Stateful_")]
        public string StandByReplicaKeepDurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets ServicePlacementTimeLimitSeconds. The duration for which replicas can stay InBuild before reporting
        /// that build is stuck.
        /// </summary>
        [Parameter(Mandatory = false, Position = 17, ParameterSetName = "_Stateful_")]
        public string ServicePlacementTimeLimitSeconds { get; set; }

        /// <summary>
        /// Gets or sets DropSourceReplicaOnMove. Indicates whether to drop source Secondary replica even if the target replica
        /// has not finished build. If desired behavior is to drop it as soon as possible the value of this property is true,
        /// if not it is false.
        /// </summary>
        [Parameter(Mandatory = false, Position = 18, ParameterSetName = "_Stateful_")]
        public bool? DropSourceReplicaOnMove { get; set; }

        /// <summary>
        /// Gets or sets ReplicaLifecycleDescription. Defines how replicas of this service will behave during ther lifecycle.
        /// </summary>
        [Parameter(Mandatory = false, Position = 19, ParameterSetName = "_Stateful_")]
        public ReplicaLifecycleDescription ReplicaLifecycleDescription { get; set; }

        /// <summary>
        /// Gets or sets AuxiliaryReplicaCount. The auxiliary replica count as a number. To use Auxiliary replicas, the
        /// following must be true: AuxiliaryReplicaCount &lt; (TargetReplicaSetSize+1)/2 and TargetReplicaSetSize >=3.
        /// </summary>
        [Parameter(Mandatory = false, Position = 20, ParameterSetName = "_Stateful_")]
        public int? AuxiliaryReplicaCount { get; set; }

        /// <summary>
        /// Gets or sets ServiceSensitivityDescription. Defines default levels of replica sensitivity of this service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 21, ParameterSetName = "_Stateful_")]
        public ServiceSensitivityDescription ServiceSensitivityDescription { get; set; }

        /// <summary>
        /// Gets or sets InstanceCount. The instance count.
        /// </summary>
        [Parameter(Mandatory = false, Position = 22, ParameterSetName = "_Stateless_")]
        public int? InstanceCount { get; set; }

        /// <summary>
        /// Gets or sets MinInstanceCount. MinInstanceCount is the minimum number of instances that must be up to meet the
        /// EnsureAvailability safety check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstanceCount computation -1 is first converted into the number of
        /// nodes on which the instances are allowed to be placed according to the placement constraints on the service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 23, ParameterSetName = "_Stateless_")]
        public int? MinInstanceCount { get; set; }

        /// <summary>
        /// Gets or sets MinInstancePercentage. MinInstancePercentage is the minimum percentage of InstanceCount that must be
        /// up to meet the EnsureAvailability safety check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstancePercentage computation, -1 is first converted into the
        /// number of nodes on which the instances are allowed to be placed according to the placement constraints on the
        /// service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 24, ParameterSetName = "_Stateless_")]
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
        /// </summary>
        [Parameter(Mandatory = false, Position = 25, ParameterSetName = "_Stateless_")]
        public string InstanceCloseDelayDurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets InstanceLifecycleDescription. Defines how instances of this service will behave during their
        /// lifecycle.
        /// </summary>
        [Parameter(Mandatory = false, Position = 26, ParameterSetName = "_Stateless_")]
        public InstanceLifecycleDescription InstanceLifecycleDescription { get; set; }

        /// <summary>
        /// Gets or sets InstanceRestartWaitDurationSeconds. When a stateless instance goes down, this timer starts. When it
        /// expires Service Fabric will create a new instance on any node in the cluster.
        /// This configuration is to reduce unnecessary creation of a new instance in situations where the instance going down
        /// is likely to recover in a short time. For example, during an upgrade.
        /// The default value is 0, which indicates that when stateless instance goes down, Service Fabric will immediately
        /// start building its replacement.
        /// </summary>
        [Parameter(Mandatory = false, Position = 27, ParameterSetName = "_Stateless_")]
        public string InstanceRestartWaitDurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 28)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            ServiceUpdateDescription serviceUpdateDescription = null;
            if (this.Stateful.IsPresent)
            {
                serviceUpdateDescription = new StatefulServiceUpdateDescription(
                    flags: this.Flags,
                    placementConstraints: this.PlacementConstraints,
                    correlationScheme: this.CorrelationScheme,
                    loadMetrics: this.LoadMetrics,
                    servicePlacementPolicies: this.ServicePlacementPolicies,
                    defaultMoveCost: this.DefaultMoveCost,
                    scalingPolicies: this.ScalingPolicies,
                    serviceDnsName: this.ServiceDnsName,
                    tagsForPlacement: this.TagsForPlacement,
                    tagsForRunning: this.TagsForRunning,
                    targetReplicaSetSize: this.TargetReplicaSetSize,
                    minReplicaSetSize: this.MinReplicaSetSize,
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
                serviceUpdateDescription = new StatelessServiceUpdateDescription(
                    flags: this.Flags,
                    placementConstraints: this.PlacementConstraints,
                    correlationScheme: this.CorrelationScheme,
                    loadMetrics: this.LoadMetrics,
                    servicePlacementPolicies: this.ServicePlacementPolicies,
                    defaultMoveCost: this.DefaultMoveCost,
                    scalingPolicies: this.ScalingPolicies,
                    serviceDnsName: this.ServiceDnsName,
                    tagsForPlacement: this.TagsForPlacement,
                    tagsForRunning: this.TagsForRunning,
                    instanceCount: this.InstanceCount,
                    minInstanceCount: this.MinInstanceCount,
                    minInstancePercentage: this.MinInstancePercentage,
                    instanceCloseDelayDurationSeconds: this.InstanceCloseDelayDurationSeconds,
                    instanceLifecycleDescription: this.InstanceLifecycleDescription,
                    instanceRestartWaitDurationSeconds: this.InstanceRestartWaitDurationSeconds);
            }

            this.ServiceFabricClient.Services.UpdateServiceAsync(
                serviceId: this.ServiceId,
                serviceUpdateDescription: serviceUpdateDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
