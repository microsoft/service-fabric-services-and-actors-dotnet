// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A base type for the list of health state chunks found in the cluster. It contains the total number of health states
    /// that match the input filters.
    /// </summary>
    public partial class EntityHealthStateChunkList
    {
        /// <summary>
        /// Initializes a new instance of the EntityHealthStateChunkList class.
        /// </summary>
        /// <param name="totalCount">Total number of entity health state objects that match the specified filters from the
        /// cluster health chunk query description.
        /// </param>
        public EntityHealthStateChunkList(
            long? totalCount = default(long?))
        {
            this.TotalCount = totalCount;
        }

        /// <summary>
        /// Gets total number of entity health state objects that match the specified filters from the cluster health chunk
        /// query description.
        /// </summary>
        public long? TotalCount { get; }
    }
}
