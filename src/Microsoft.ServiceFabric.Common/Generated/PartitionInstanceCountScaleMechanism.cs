// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a scaling mechanism for adding or removing instances of stateless service partition.
    /// </summary>
    public partial class PartitionInstanceCountScaleMechanism : ScalingMechanismDescription
    {
        /// <summary>
        /// Initializes a new instance of the PartitionInstanceCountScaleMechanism class.
        /// </summary>
        /// <param name="minInstanceCount">Minimum number of instances of the partition.</param>
        /// <param name="maxInstanceCount">Maximum number of instances of the partition.</param>
        /// <param name="scaleIncrement">The number of instances to add or remove during a scaling operation.</param>
        public PartitionInstanceCountScaleMechanism(
            int? minInstanceCount,
            int? maxInstanceCount,
            int? scaleIncrement)
            : base(
                Common.ScalingMechanismKind.PartitionInstanceCount)
        {
            minInstanceCount.ThrowIfNull(nameof(minInstanceCount));
            maxInstanceCount.ThrowIfNull(nameof(maxInstanceCount));
            scaleIncrement.ThrowIfNull(nameof(scaleIncrement));
            this.MinInstanceCount = minInstanceCount;
            this.MaxInstanceCount = maxInstanceCount;
            this.ScaleIncrement = scaleIncrement;
        }

        /// <summary>
        /// Gets minimum number of instances of the partition.
        /// </summary>
        public int? MinInstanceCount { get; }

        /// <summary>
        /// Gets maximum number of instances of the partition.
        /// </summary>
        public int? MaxInstanceCount { get; }

        /// <summary>
        /// Gets the number of instances to add or remove during a scaling operation.
        /// </summary>
        public int? ScaleIncrement { get; }
    }
}
