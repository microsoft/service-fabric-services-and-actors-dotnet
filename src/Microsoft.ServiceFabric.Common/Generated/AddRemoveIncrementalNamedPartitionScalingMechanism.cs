// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a scaling mechanism for adding or removing named partitions of a stateless service. Partition names are
    /// in the format '0','1''N-1'
    /// </summary>
    public partial class AddRemoveIncrementalNamedPartitionScalingMechanism : ScalingMechanismDescription
    {
        /// <summary>
        /// Initializes a new instance of the AddRemoveIncrementalNamedPartitionScalingMechanism class.
        /// </summary>
        /// <param name="minPartitionCount">Minimum number of named partitions of the service.</param>
        /// <param name="maxPartitionCount">Maximum number of named partitions of the service.</param>
        /// <param name="scaleIncrement">The number of instances to add or remove during a scaling operation.</param>
        public AddRemoveIncrementalNamedPartitionScalingMechanism(
            int? minPartitionCount,
            int? maxPartitionCount,
            int? scaleIncrement)
            : base(
                Common.ScalingMechanismKind.AddRemoveIncrementalNamedPartition)
        {
            minPartitionCount.ThrowIfNull(nameof(minPartitionCount));
            maxPartitionCount.ThrowIfNull(nameof(maxPartitionCount));
            scaleIncrement.ThrowIfNull(nameof(scaleIncrement));
            this.MinPartitionCount = minPartitionCount;
            this.MaxPartitionCount = maxPartitionCount;
            this.ScaleIncrement = scaleIncrement;
        }

        /// <summary>
        /// Gets minimum number of named partitions of the service.
        /// </summary>
        public int? MinPartitionCount { get; }

        /// <summary>
        /// Gets maximum number of named partitions of the service.
        /// </summary>
        public int? MaxPartitionCount { get; }

        /// <summary>
        /// Gets the number of instances to add or remove during a scaling operation.
        /// </summary>
        public int? ScaleIncrement { get; }
    }
}
