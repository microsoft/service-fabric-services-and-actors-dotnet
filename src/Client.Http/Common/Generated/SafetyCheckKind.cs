// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for SafetyCheckKind.
    /// </summary>
    public enum SafetyCheckKind
    {
        /// <summary>
        /// Indicates that the upgrade safety check kind is invalid. All Service Fabric enumerations have the invalid type. The
        /// value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that if we bring down the node then this will result in global seed node quorum loss. The value is 1.
        /// </summary>
        EnsureSeedNodeQuorum,

        /// <summary>
        /// Indicates that there is some partition for which if we bring down the replica on the node, it will result in quorum
        /// loss for that partition. The value is 2.
        /// </summary>
        EnsurePartitionQuorum,

        /// <summary>
        /// Indicates that there is some replica on the node that was moved out of this node due to upgrade. Service Fabric is
        /// now waiting for the primary to be moved back to this node. The value is 3.
        /// </summary>
        WaitForPrimaryPlacement,

        /// <summary>
        /// Indicates that Service Fabric is waiting for a primary replica to be moved out of the node before starting upgrade
        /// on that node. The value is 4.
        /// </summary>
        WaitForPrimarySwap,

        /// <summary>
        /// Indicates that there is some replica on the node that is involved in a reconfiguration. Service Fabric is waiting
        /// for the reconfiguration to be complete before staring upgrade on that node. The value is 5.
        /// </summary>
        WaitForReconfiguration,

        /// <summary>
        /// Indicates that there is either a replica on the node that is going through copy, or there is a primary replica on
        /// the node that is copying data to some other replica. In both cases, bringing down the replica on the node due to
        /// upgrade will abort the copy. The value is 6.
        /// </summary>
        WaitForInbuildReplica,

        /// <summary>
        /// Indicates that there is either a stateless service partition on the node having exactly one instance, or there is a
        /// primary replica on the node for which the partition is quorum loss. In both cases, bringing down the replicas due
        /// to upgrade will result in loss of availability. The value is 7.
        /// </summary>
        EnsureAvailability,
    }
}
