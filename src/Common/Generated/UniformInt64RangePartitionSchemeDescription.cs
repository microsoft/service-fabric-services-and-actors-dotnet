// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a partitioning scheme where an integer range is allocated evenly across a number of partitions.
    /// </summary>
    public partial class UniformInt64RangePartitionSchemeDescription : PartitionSchemeDescription
    {
        /// <summary>
        /// Initializes a new instance of the UniformInt64RangePartitionSchemeDescription class.
        /// </summary>
        /// <param name="count">The number of partitions.</param>
        /// <param name="lowKey">String indicating the lower bound of the partition key range that
        /// should be split between the partitions.
        /// </param>
        /// <param name="highKey">String indicating the upper bound of the partition key range that
        /// should be split between the partitions.
        /// </param>
        public UniformInt64RangePartitionSchemeDescription(
            int? count,
            string lowKey,
            string highKey)
            : base(
                Common.PartitionScheme.UniformInt64Range)
        {
            count.ThrowIfNull(nameof(count));
            lowKey.ThrowIfNull(nameof(lowKey));
            highKey.ThrowIfNull(nameof(highKey));
            this.Count = count;
            this.LowKey = lowKey;
            this.HighKey = highKey;
        }

        /// <summary>
        /// Gets the number of partitions.
        /// </summary>
        public int? Count { get; }

        /// <summary>
        /// Gets string indicating the lower bound of the partition key range that
        /// should be split between the partitions.
        /// </summary>
        public string LowKey { get; }

        /// <summary>
        /// Gets string indicating the upper bound of the partition key range that
        /// should be split between the partitions.
        /// </summary>
        public string HighKey { get; }
    }
}
