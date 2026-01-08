// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Chaos Started event.
    /// </summary>
    public partial class ChaosStartedEvent : ClusterEvent
    {
        /// <summary>
        /// Initializes a new instance of the ChaosStartedEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="maxConcurrentFaults">Maximum number of concurrent faults.</param>
        /// <param name="timeToRunInSeconds">Time to run in seconds.</param>
        /// <param name="maxClusterStabilizationTimeoutInSeconds">Maximum timeout for cluster stabilization in seconds.</param>
        /// <param name="waitTimeBetweenIterationsInSeconds">Wait time between iterations in seconds.</param>
        /// <param name="waitTimeBetweenFaultsInSeconds">Wait time between faults in seconds.</param>
        /// <param name="moveReplicaFaultEnabled">Indicates MoveReplica fault is enabled.</param>
        /// <param name="includedNodeTypeList">List of included Node types.</param>
        /// <param name="includedApplicationList">List of included Applications.</param>
        /// <param name="clusterHealthPolicy">Health policy.</param>
        /// <param name="chaosContext">Chaos Context.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public ChaosStartedEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            long? maxConcurrentFaults,
            double? timeToRunInSeconds,
            double? maxClusterStabilizationTimeoutInSeconds,
            double? waitTimeBetweenIterationsInSeconds,
            double? waitTimeBetweenFaultsInSeconds,
            bool? moveReplicaFaultEnabled,
            string includedNodeTypeList,
            string includedApplicationList,
            string clusterHealthPolicy,
            string chaosContext,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.ChaosStarted,
                category,
                hasCorrelatedEvents)
        {
            maxConcurrentFaults.ThrowIfNull(nameof(maxConcurrentFaults));
            timeToRunInSeconds.ThrowIfNull(nameof(timeToRunInSeconds));
            maxClusterStabilizationTimeoutInSeconds.ThrowIfNull(nameof(maxClusterStabilizationTimeoutInSeconds));
            waitTimeBetweenIterationsInSeconds.ThrowIfNull(nameof(waitTimeBetweenIterationsInSeconds));
            waitTimeBetweenFaultsInSeconds.ThrowIfNull(nameof(waitTimeBetweenFaultsInSeconds));
            moveReplicaFaultEnabled.ThrowIfNull(nameof(moveReplicaFaultEnabled));
            includedNodeTypeList.ThrowIfNull(nameof(includedNodeTypeList));
            includedApplicationList.ThrowIfNull(nameof(includedApplicationList));
            clusterHealthPolicy.ThrowIfNull(nameof(clusterHealthPolicy));
            chaosContext.ThrowIfNull(nameof(chaosContext));
            this.MaxConcurrentFaults = maxConcurrentFaults;
            this.TimeToRunInSeconds = timeToRunInSeconds;
            this.MaxClusterStabilizationTimeoutInSeconds = maxClusterStabilizationTimeoutInSeconds;
            this.WaitTimeBetweenIterationsInSeconds = waitTimeBetweenIterationsInSeconds;
            this.WaitTimeBetweenFaultsInSeconds = waitTimeBetweenFaultsInSeconds;
            this.MoveReplicaFaultEnabled = moveReplicaFaultEnabled;
            this.IncludedNodeTypeList = includedNodeTypeList;
            this.IncludedApplicationList = includedApplicationList;
            this.ClusterHealthPolicy = clusterHealthPolicy;
            this.ChaosContext = chaosContext;
        }

        /// <summary>
        /// Gets maximum number of concurrent faults.
        /// </summary>
        public long? MaxConcurrentFaults { get; }

        /// <summary>
        /// Gets time to run in seconds.
        /// </summary>
        public double? TimeToRunInSeconds { get; }

        /// <summary>
        /// Gets maximum timeout for cluster stabilization in seconds.
        /// </summary>
        public double? MaxClusterStabilizationTimeoutInSeconds { get; }

        /// <summary>
        /// Gets wait time between iterations in seconds.
        /// </summary>
        public double? WaitTimeBetweenIterationsInSeconds { get; }

        /// <summary>
        /// Gets wait time between faults in seconds.
        /// </summary>
        public double? WaitTimeBetweenFaultsInSeconds { get; }

        /// <summary>
        /// Gets indicates MoveReplica fault is enabled.
        /// </summary>
        public bool? MoveReplicaFaultEnabled { get; }

        /// <summary>
        /// Gets list of included Node types.
        /// </summary>
        public string IncludedNodeTypeList { get; }

        /// <summary>
        /// Gets list of included Applications.
        /// </summary>
        public string IncludedApplicationList { get; }

        /// <summary>
        /// Gets health policy.
        /// </summary>
        public string ClusterHealthPolicy { get; }

        /// <summary>
        /// Gets chaos Context.
        /// </summary>
        public string ChaosContext { get; }
    }
}
