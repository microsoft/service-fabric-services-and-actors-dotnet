// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the horizontal auto scaling mechanism that adds or removes replicas (containers or container groups).
    /// </summary>
    public partial class AddRemoveReplicaScalingMechanism : AutoScalingMechanism
    {
        /// <summary>
        /// Initializes a new instance of the AddRemoveReplicaScalingMechanism class.
        /// </summary>
        /// <param name="minCount">Minimum number of containers (scale down won't be performed below this number).</param>
        /// <param name="maxCount">Maximum number of containers (scale up won't be performed above this number).</param>
        /// <param name="scaleIncrement">Each time auto scaling is performed, this number of containers will be added or
        /// removed.</param>
        public AddRemoveReplicaScalingMechanism(
            int? minCount,
            int? maxCount,
            int? scaleIncrement)
            : base(
                Common.AutoScalingMechanismKind.AddRemoveReplica)
        {
            minCount.ThrowIfNull(nameof(minCount));
            maxCount.ThrowIfNull(nameof(maxCount));
            scaleIncrement.ThrowIfNull(nameof(scaleIncrement));
            this.MinCount = minCount;
            this.MaxCount = maxCount;
            this.ScaleIncrement = scaleIncrement;
        }

        /// <summary>
        /// Gets minimum number of containers (scale down won't be performed below this number).
        /// </summary>
        public int? MinCount { get; }

        /// <summary>
        /// Gets maximum number of containers (scale up won't be performed above this number).
        /// </summary>
        public int? MaxCount { get; }

        /// <summary>
        /// Gets each time auto scaling is performed, this number of containers will be added or removed.
        /// </summary>
        public int? ScaleIncrement { get; }
    }
}
