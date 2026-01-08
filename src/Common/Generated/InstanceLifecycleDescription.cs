// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes how the instance will behave
    /// </summary>
    public partial class InstanceLifecycleDescription
    {
        /// <summary>
        /// Initializes a new instance of the InstanceLifecycleDescription class.
        /// </summary>
        /// <param name="restoreReplicaLocationAfterUpgrade">If set to true, move/swap replica to original location after
        /// upgrade.</param>
        public InstanceLifecycleDescription(
            bool? restoreReplicaLocationAfterUpgrade = default(bool?))
        {
            this.RestoreReplicaLocationAfterUpgrade = restoreReplicaLocationAfterUpgrade;
        }

        /// <summary>
        /// Gets if set to true, move/swap replica to original location after upgrade.
        /// </summary>
        public bool? RestoreReplicaLocationAfterUpgrade { get; }
    }
}
