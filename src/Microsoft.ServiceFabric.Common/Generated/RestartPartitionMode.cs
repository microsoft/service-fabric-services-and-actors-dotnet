// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for RestartPartitionMode.
    /// </summary>
    public enum RestartPartitionMode
    {
        /// <summary>
        /// Reserved.  Do not pass into API.
        /// </summary>
        Invalid,

        /// <summary>
        /// All replicas or instances in the partition are restarted at once.
        /// </summary>
        AllReplicasOrInstances,

        /// <summary>
        /// Only the secondary replicas are restarted.
        /// </summary>
        OnlyActiveSecondaries,
    }
}
