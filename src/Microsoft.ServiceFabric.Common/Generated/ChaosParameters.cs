// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines all the parameters to configure a Chaos run.
    /// </summary>
    public partial class ChaosParameters
    {
        /// <summary>
        /// Initializes a new instance of the ChaosParameters class.
        /// </summary>
        /// <param name="timeToRunInSeconds">Total time (in seconds) for which Chaos will run before automatically stopping.
        /// The maximum allowed value is 4,294,967,295 (System.UInt32.MaxValue).
        /// </param>
        /// <param name="maxClusterStabilizationTimeoutInSeconds">The maximum amount of time to wait for all cluster entities
        /// to become stable and healthy. Chaos executes in iterations and at the start of each iteration it validates the
        /// health of cluster entities.
        /// During validation if a cluster entity is not stable and healthy within MaxClusterStabilizationTimeoutInSeconds,
        /// Chaos generates a validation failed event.
        /// </param>
        /// <param name="maxConcurrentFaults">MaxConcurrentFaults is the maximum number of concurrent faults induced per
        /// iteration.
        /// Chaos executes in iterations and two consecutive iterations are separated by a validation phase.
        /// The higher the concurrency, the more aggressive the injection of faults, leading to inducing more complex series of
        /// states to uncover bugs.
        /// The recommendation is to start with a value of 2 or 3 and to exercise caution while moving up.
        /// </param>
        /// <param name="enableMoveReplicaFaults">Enables or disables the move primary and move secondary faults.
        /// </param>
        /// <param name="waitTimeBetweenFaultsInSeconds">Wait time (in seconds) between consecutive faults within a single
        /// iteration.
        /// The larger the value, the lower the overlapping between faults and the simpler the sequence of state transitions
        /// that the cluster goes through.
        /// The recommendation is to start with a value between 1 and 5 and exercise caution while moving up.
        /// </param>
        /// <param name="waitTimeBetweenIterationsInSeconds">Time-separation (in seconds) between two consecutive iterations of
        /// Chaos.
        /// The larger the value, the lower the fault injection rate.
        /// </param>
        /// <param name="clusterHealthPolicy">Passed-in cluster health policy is used to validate health of the cluster in
        /// between Chaos iterations. If the cluster health is in error or if an unexpected exception happens during fault
        /// execution--to provide the cluster with some time to recuperate--Chaos will wait for 30 minutes before the next
        /// health-check.
        /// </param>
        /// <param name="context">Describes a map, which is a collection of (string, string) type key-value pairs. The map can
        /// be used to record information about
        /// the Chaos run. There cannot be more than 100 such pairs and each string (key or value) can be at most 4095
        /// characters long.
        /// This map is set by the starter of the Chaos run to optionally store the context about the specific run.
        /// </param>
        /// <param name="chaosTargetFilter">List of cluster entities to target for Chaos faults.
        /// This filter can be used to target Chaos faults only to certain node types or only to certain application instances.
        /// If ChaosTargetFilter is not used, Chaos faults all cluster entities.
        /// If ChaosTargetFilter is used, Chaos faults only the entities that meet the ChaosTargetFilter specification.
        /// </param>
        public ChaosParameters(
            string timeToRunInSeconds = "4294967295",
            long? maxClusterStabilizationTimeoutInSeconds = 60,
            long? maxConcurrentFaults = 1,
            bool? enableMoveReplicaFaults = true,
            long? waitTimeBetweenFaultsInSeconds = 20,
            long? waitTimeBetweenIterationsInSeconds = 30,
            ClusterHealthPolicy clusterHealthPolicy = default(ClusterHealthPolicy),
            ChaosContext context = default(ChaosContext),
            ChaosTargetFilter chaosTargetFilter = default(ChaosTargetFilter))
        {
            maxClusterStabilizationTimeoutInSeconds?.ThrowIfOutOfInclusiveRange("maxClusterStabilizationTimeoutInSeconds", 0, 4294967295);
            maxConcurrentFaults?.ThrowIfOutOfInclusiveRange("maxConcurrentFaults", 0, 4294967295);
            waitTimeBetweenFaultsInSeconds?.ThrowIfOutOfInclusiveRange("waitTimeBetweenFaultsInSeconds", 0, 4294967295);
            waitTimeBetweenIterationsInSeconds?.ThrowIfOutOfInclusiveRange("waitTimeBetweenIterationsInSeconds", 0, 4294967295);
            this.TimeToRunInSeconds = timeToRunInSeconds;
            this.MaxClusterStabilizationTimeoutInSeconds = maxClusterStabilizationTimeoutInSeconds;
            this.MaxConcurrentFaults = maxConcurrentFaults;
            this.EnableMoveReplicaFaults = enableMoveReplicaFaults;
            this.WaitTimeBetweenFaultsInSeconds = waitTimeBetweenFaultsInSeconds;
            this.WaitTimeBetweenIterationsInSeconds = waitTimeBetweenIterationsInSeconds;
            this.ClusterHealthPolicy = clusterHealthPolicy;
            this.Context = context;
            this.ChaosTargetFilter = chaosTargetFilter;
        }

        /// <summary>
        /// Gets total time (in seconds) for which Chaos will run before automatically stopping. The maximum allowed value is
        /// 4,294,967,295 (System.UInt32.MaxValue).
        /// </summary>
        public string TimeToRunInSeconds { get; }

        /// <summary>
        /// Gets the maximum amount of time to wait for all cluster entities to become stable and healthy. Chaos executes in
        /// iterations and at the start of each iteration it validates the health of cluster entities.
        /// During validation if a cluster entity is not stable and healthy within MaxClusterStabilizationTimeoutInSeconds,
        /// Chaos generates a validation failed event.
        /// </summary>
        public long? MaxClusterStabilizationTimeoutInSeconds { get; }

        /// <summary>
        /// Gets maxConcurrentFaults is the maximum number of concurrent faults induced per iteration.
        /// Chaos executes in iterations and two consecutive iterations are separated by a validation phase.
        /// The higher the concurrency, the more aggressive the injection of faults, leading to inducing more complex series of
        /// states to uncover bugs.
        /// The recommendation is to start with a value of 2 or 3 and to exercise caution while moving up.
        /// </summary>
        public long? MaxConcurrentFaults { get; }

        /// <summary>
        /// Gets enables or disables the move primary and move secondary faults.
        /// </summary>
        public bool? EnableMoveReplicaFaults { get; }

        /// <summary>
        /// Gets wait time (in seconds) between consecutive faults within a single iteration.
        /// The larger the value, the lower the overlapping between faults and the simpler the sequence of state transitions
        /// that the cluster goes through.
        /// The recommendation is to start with a value between 1 and 5 and exercise caution while moving up.
        /// </summary>
        public long? WaitTimeBetweenFaultsInSeconds { get; }

        /// <summary>
        /// Gets time-separation (in seconds) between two consecutive iterations of Chaos.
        /// The larger the value, the lower the fault injection rate.
        /// </summary>
        public long? WaitTimeBetweenIterationsInSeconds { get; }

        /// <summary>
        /// Gets passed-in cluster health policy is used to validate health of the cluster in between Chaos iterations. If the
        /// cluster health is in error or if an unexpected exception happens during fault execution--to provide the cluster
        /// with some time to recuperate--Chaos will wait for 30 minutes before the next health-check.
        /// </summary>
        public ClusterHealthPolicy ClusterHealthPolicy { get; }

        /// <summary>
        /// Gets a map, which is a collection of (string, string) type key-value pairs. The map can be used to record
        /// information about
        /// the Chaos run. There cannot be more than 100 such pairs and each string (key or value) can be at most 4095
        /// characters long.
        /// This map is set by the starter of the Chaos run to optionally store the context about the specific run.
        /// </summary>
        public ChaosContext Context { get; }

        /// <summary>
        /// Gets list of cluster entities to target for Chaos faults.
        /// This filter can be used to target Chaos faults only to certain node types or only to certain application instances.
        /// If ChaosTargetFilter is not used, Chaos faults all cluster entities.
        /// If ChaosTargetFilter is used, Chaos faults only the entities that meet the ChaosTargetFilter specification.
        /// </summary>
        public ChaosTargetFilter ChaosTargetFilter { get; }
    }
}
