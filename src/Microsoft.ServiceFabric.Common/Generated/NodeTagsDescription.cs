// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the tags required for placement or running of the service.
    /// </summary>
    public partial class NodeTagsDescription
    {
        /// <summary>
        /// Initializes a new instance of the NodeTagsDescription class.
        /// </summary>
        /// <param name="count">The number of tags.</param>
        /// <param name="tags">Array of size specified by the ‘Count’ parameter, for the placement tags of the service.</param>
        public NodeTagsDescription(
            int? count,
            IEnumerable<string> tags)
        {
            count.ThrowIfNull(nameof(count));
            tags.ThrowIfNull(nameof(tags));
            this.Count = count;
            this.Tags = tags;
        }

        /// <summary>
        /// Gets the number of tags.
        /// </summary>
        public int? Count { get; }

        /// <summary>
        /// Gets array of size specified by the ‘Count’ parameter, for the placement tags of the service.
        /// </summary>
        public IEnumerable<string> Tags { get; }
    }
}
