// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a partition of a stateful Service Fabric service..
    /// </summary>
    public partial class StatefulServicePartitionInfo : ServicePartitionInfo
    {
        /// <summary>
        /// Initializes a new instance of the StatefulServicePartitionInfo class.
        /// </summary>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="partitionStatus">The status of the service fabric service partition. Possible values include:
        /// 'Invalid', 'Ready', 'NotReady', 'InQuorumLoss', 'Reconfiguring', 'Deleting'</param>
        /// <param name="partitionInformation">Information about the partition identity, partitioning scheme and keys supported
        /// by it.</param>
        /// <param name="targetReplicaSetSize">The target replica set size as a number.</param>
        /// <param name="minReplicaSetSize">The minimum replica set size as a number.</param>
        /// <param name="auxiliaryReplicaCount">The auxiliary replica count as a number. To use Auxiliary replicas the
        /// following must be true, AuxiliaryReplicaCount &lt; (TargetReplicaSetSize+1)/2 and TargetReplicaSetSize >=3.</param>
        /// <param name="lastQuorumLossDuration">The duration for which this partition was in quorum loss. If the partition is
        /// currently in quorum loss, it returns the duration since it has been in that state. This field is using ISO8601
        /// format for specifying the duration.</param>
        /// <param name="primaryEpoch">An Epoch is a configuration number for the partition as a whole. When the configuration
        /// of the replica set changes, for example when the Primary replica changes, the operations that are replicated from
        /// the new Primary replica are said to be a new Epoch from the ones which were sent by the old Primary replica.
        /// </param>
        public StatefulServicePartitionInfo(
            HealthState? healthState = default(HealthState?),
            ServicePartitionStatus? partitionStatus = default(ServicePartitionStatus?),
            PartitionInformation partitionInformation = default(PartitionInformation),
            long? targetReplicaSetSize = default(long?),
            long? minReplicaSetSize = default(long?),
            long? auxiliaryReplicaCount = default(long?),
            TimeSpan? lastQuorumLossDuration = default(TimeSpan?),
            Epoch primaryEpoch = default(Epoch))
            : base(
                Common.ServiceKind.Stateful,
                healthState,
                partitionStatus,
                partitionInformation)
        {
            this.TargetReplicaSetSize = targetReplicaSetSize;
            this.MinReplicaSetSize = minReplicaSetSize;
            this.AuxiliaryReplicaCount = auxiliaryReplicaCount;
            this.LastQuorumLossDuration = lastQuorumLossDuration;
            this.PrimaryEpoch = primaryEpoch;
        }

        /// <summary>
        /// Gets the target replica set size as a number.
        /// </summary>
        public long? TargetReplicaSetSize { get; }

        /// <summary>
        /// Gets the minimum replica set size as a number.
        /// </summary>
        public long? MinReplicaSetSize { get; }

        /// <summary>
        /// Gets the auxiliary replica count as a number. To use Auxiliary replicas the following must be true,
        /// AuxiliaryReplicaCount &amp;lt; (TargetReplicaSetSize+1)/2 and TargetReplicaSetSize &amp;gt;=3.
        /// </summary>
        public long? AuxiliaryReplicaCount { get; }

        /// <summary>
        /// Gets the duration for which this partition was in quorum loss. If the partition is currently in quorum loss, it
        /// returns the duration since it has been in that state. This field is using ISO8601 format for specifying the
        /// duration.
        /// </summary>
        public TimeSpan? LastQuorumLossDuration { get; }

        /// <summary>
        /// Gets an Epoch is a configuration number for the partition as a whole. When the configuration of the replica set
        /// changes, for example when the Primary replica changes, the operations that are replicated from the new Primary
        /// replica are said to be a new Epoch from the ones which were sent by the old Primary replica.
        /// </summary>
        public Epoch PrimaryEpoch { get; }
    }
}
