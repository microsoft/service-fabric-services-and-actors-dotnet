// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for PartitionAccessStatus.
    /// </summary>
    public enum PartitionAccessStatus
    {
        /// <summary>
        /// Indicates that the read or write operation access status is not valid. This value is not returned to the caller.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the read or write operation access is granted and the operation is allowed.
        /// </summary>
        Granted,

        /// <summary>
        /// Indicates that the client should try again later, because a reconfiguration is in progress.
        /// </summary>
        ReconfigurationPending,

        /// <summary>
        /// Indicates that this client request was received by a replica that is not a Primary replica.
        /// </summary>
        NotPrimary,

        /// <summary>
        /// Indicates that no write quorum is available and, therefore, no write operation can be accepted.
        /// </summary>
        NoWriteQuorum,
    }
}
