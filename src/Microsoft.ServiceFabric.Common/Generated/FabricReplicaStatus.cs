// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for FabricReplicaStatus.
    /// </summary>
    public enum FabricReplicaStatus
    {
        /// <summary>
        /// Indicates that the read or write operation access status is not valid. This value is not returned to the caller.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the replica is down.
        /// </summary>
        Down,

        /// <summary>
        /// Indicates that the replica is up.
        /// </summary>
        Up,
    }
}
