// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The list of partition health state chunks that respect the input filters in the chunk query description.
    /// Returned by get cluster health state chunks query as part of the parent application hierarchy.
    /// </summary>
    public partial class PartitionHealthStateChunkList
    {
        /// <summary>
        /// Initializes a new instance of the PartitionHealthStateChunkList class.
        /// </summary>
        /// <param name="items">The list of partition health state chunks that respect the input filters in the chunk query.
        /// </param>
        public PartitionHealthStateChunkList(
            IEnumerable<PartitionHealthStateChunk> items = default(IEnumerable<PartitionHealthStateChunk>))
        {
            this.Items = items;
        }

        /// <summary>
        /// Gets the list of partition health state chunks that respect the input filters in the chunk query.
        /// </summary>
        public IEnumerable<PartitionHealthStateChunk> Items { get; }
    }
}
