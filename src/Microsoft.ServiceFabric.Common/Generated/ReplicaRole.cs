// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ReplicaRole.
    /// </summary>
    public enum ReplicaRole
    {
        /// <summary>
        /// Indicates the initial role that a replica is created in. The value is zero.
        /// </summary>
        Unknown,

        /// <summary>
        /// Specifies that the replica has no responsibility in regard to the replica set. The value is 1.
        /// </summary>
        None,

        /// <summary>
        /// Refers to the replica in the set on which all read and write operations are complete in order to enforce strong
        /// consistency semantics. Read operations are handled directly by the Primary replica, while write operations must be
        /// acknowledged by a quorum of the replicas in the replica set. There can only be one Primary replica in a replica set
        /// at a time. The value is 2.
        /// </summary>
        Primary,

        /// <summary>
        /// Refers to a replica in the set that receives a state transfer from the Primary replica to prepare for becoming an
        /// active Secondary replica. There can be multiple Idle Secondary replicas in a replica set at a time. Idle Secondary
        /// replicas do not count as a part of a write quorum. The value is 3.
        /// </summary>
        IdleSecondary,

        /// <summary>
        /// Refers to a replica in the set that receives state updates from the Primary replica, applies them, and sends
        /// acknowledgements back. Secondary replicas must participate in the write quorum for a replica set. There can be
        /// multiple active Secondary replicas in a replica set at a time. The number of active Secondary replicas is
        /// configurable that the reliability subsystem should maintain. The value is 4.
        /// </summary>
        ActiveSecondary,

        /// <summary>
        /// Refers to a replica in the set that receives a state transfer from the Primary replica to prepare for becoming an
        /// ActiveAuxiliary replica. There can be multiple IdleAuxiliary replicas in a replica set at a time. IdleAuxiliary
        /// replicas do not count as a part of a write quorum. The value is 5.
        /// </summary>
        IdleAuxiliary,

        /// <summary>
        /// Refers to a replica in the set that receives state updates from the Primary replica, applies them, and sends
        /// acknowledgements back. ActiveAuxiliary replicas must participate in the write quorum for a replica set. There can
        /// be multiple active ActiveAuxiliary replicas in a replica set at a time. The number of active ActiveAuxiliary
        /// replicas is configurable that the reliability subsystem should maintain. The value is 6.
        /// </summary>
        ActiveAuxiliary,

        /// <summary>
        /// Refers to the replica in the set that is used to rebuild a new Secondary replica to relinquish primary status to.
        /// It cannot field read or write requests. The value is 7.
        /// </summary>
        PrimaryAuxiliary,
    }
}
