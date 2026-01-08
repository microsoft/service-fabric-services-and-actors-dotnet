// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes how the replica will behave
    /// </summary>
    public partial class ReplicaLifecycleDescription
    {
        /// <summary>
        /// Initializes a new instance of the ReplicaLifecycleDescription class.
        /// </summary>
        /// <param name="isSingletonReplicaMoveAllowedDuringUpgrade">If set to true, replicas with a target replica set size of
        /// 1 will be permitted to move during upgrade.</param>
        /// <param name="restoreReplicaLocationAfterUpgrade">If set to true, move/swap replica to original location after
        /// upgrade.</param>
        public ReplicaLifecycleDescription(
            bool? isSingletonReplicaMoveAllowedDuringUpgrade = default(bool?),
            bool? restoreReplicaLocationAfterUpgrade = default(bool?))
        {
            this.IsSingletonReplicaMoveAllowedDuringUpgrade = isSingletonReplicaMoveAllowedDuringUpgrade;
            this.RestoreReplicaLocationAfterUpgrade = restoreReplicaLocationAfterUpgrade;
        }

        /// <summary>
        /// Gets if set to true, replicas with a target replica set size of 1 will be permitted to move during upgrade.
        /// </summary>
        public bool? IsSingletonReplicaMoveAllowedDuringUpgrade { get; }

        /// <summary>
        /// Gets if set to true, move/swap replica to original location after upgrade.
        /// </summary>
        public bool? RestoreReplicaLocationAfterUpgrade { get; }
    }
}
