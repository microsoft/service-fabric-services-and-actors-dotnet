// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ReplicaStatus.
    /// </summary>
    public enum ReplicaStatus
    {
        /// <summary>
        /// Indicates the replica status is invalid. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// The replica is being built. This means that a primary replica is seeding this replica. The value is 1.
        /// </summary>
        InBuild,

        /// <summary>
        /// The replica is in standby. The value is 2.
        /// </summary>
        Standby,

        /// <summary>
        /// The replica is ready. The value is 3.
        /// </summary>
        Ready,

        /// <summary>
        /// The replica is down. The value is 4.
        /// </summary>
        Down,

        /// <summary>
        /// Replica is dropped. This means that the replica has been removed from the replica set. If it is persisted, its
        /// state has been deleted. The value is 5.
        /// </summary>
        Dropped,

        /// <summary>
        /// The replica is completed. This means that the replica has been removed from the replica set. The value is 6.
        /// </summary>
        Completed,

        /// <summary>
        /// The replica is to be removed. This means that the replica has been removed from the replica set; however its data
        /// is still preserved on disk temporarily and can be restored if manual actions are taken. The value is 7.
        /// </summary>
        ToBeRemoved,
    }
}
