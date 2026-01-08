// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the partition information for the integer range that is based on partition schemes.
    /// </summary>
    public partial class Int64RangePartitionInformation : PartitionInformation
    {
        /// <summary>
        /// Initializes a new instance of the Int64RangePartitionInformation class.
        /// </summary>
        /// <param name="id">An internal ID used by Service Fabric to uniquely identify a partition. This is a randomly
        /// generated GUID when the service was created. The partition ID is unique and does not change for the lifetime of the
        /// service. If the same service was deleted and recreated the IDs of its partitions would be different.</param>
        /// <param name="lowKey">Specifies the minimum key value handled by this partition.</param>
        /// <param name="highKey">Specifies the maximum key value handled by this partition.</param>
        public Int64RangePartitionInformation(
            PartitionId id = default(PartitionId),
            string lowKey = default(string),
            string highKey = default(string))
            : base(
                Common.ServicePartitionKind.Int64Range,
                id)
        {
            this.LowKey = lowKey;
            this.HighKey = highKey;
        }

        /// <summary>
        /// Gets specifies the minimum key value handled by this partition.
        /// </summary>
        public string LowKey { get; }

        /// <summary>
        /// Gets specifies the maximum key value handled by this partition.
        /// </summary>
        public string HighKey { get; }
    }
}
