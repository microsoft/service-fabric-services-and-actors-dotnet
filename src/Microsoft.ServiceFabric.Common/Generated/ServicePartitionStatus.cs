// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ServicePartitionStatus.
    /// </summary>
    public enum ServicePartitionStatus
    {
        /// <summary>
        /// Indicates the partition status is invalid. All Service Fabric enumerations have the invalid type. The value is
        /// zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the partition is ready. This means that for a stateless service partition there is at least one
        /// instance that is up and for a stateful service partition the number of ready replicas is greater than or equal to
        /// the MinReplicaSetSize. The value is 1.
        /// </summary>
        Ready,

        /// <summary>
        /// Indicates that the partition is not ready. This status is returned when none of the other states apply. The value
        /// is 2.
        /// </summary>
        NotReady,

        /// <summary>
        /// Indicates that the partition is in quorum loss. This means that number of replicas that are up and participating in
        /// a replica set is less than MinReplicaSetSize for this partition. The value is 3.
        /// </summary>
        InQuorumLoss,

        /// <summary>
        /// Indicates that the partition is undergoing reconfiguration of its replica sets. This can happen due to failover,
        /// upgrade, load balancing or addition or removal of replicas from the replica set. The value is 4.
        /// </summary>
        Reconfiguring,

        /// <summary>
        /// Indicates that the partition is being deleted. The value is 5.
        /// </summary>
        Deleting,
    }
}
