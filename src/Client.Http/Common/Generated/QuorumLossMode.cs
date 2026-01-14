// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for QuorumLossMode.
    /// </summary>
    public enum QuorumLossMode
    {
        /// <summary>
        /// Reserved.  Do not pass into API.
        /// </summary>
        Invalid,

        /// <summary>
        /// Partial Quorum loss mode : Minimum number of replicas for a partition will be down that will cause a quorum loss.
        /// </summary>
        QuorumReplicas,

        /// <summary>
        /// AllReplicas.
        /// </summary>
        AllReplicas,
    }
}
