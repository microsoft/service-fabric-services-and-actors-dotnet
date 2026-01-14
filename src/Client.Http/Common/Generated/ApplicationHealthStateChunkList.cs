// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The list of application health state chunks in the cluster that respect the input filters in the chunk query.
    /// Returned by get cluster health state chunks query.
    /// </summary>
    public partial class ApplicationHealthStateChunkList : EntityHealthStateChunkList
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationHealthStateChunkList class.
        /// </summary>
        /// <param name="totalCount">Total number of entity health state objects that match the specified filters from the
        /// cluster health chunk query description.
        /// </param>
        /// <param name="items">The list of application health state chunks that respect the input filters in the chunk query.
        /// </param>
        public ApplicationHealthStateChunkList(
            long? totalCount = default(long?),
            IEnumerable<ApplicationHealthStateChunk> items = default(IEnumerable<ApplicationHealthStateChunk>))
            : base(
                totalCount)
        {
            this.Items = items;
        }

        /// <summary>
        /// Gets the list of application health state chunks that respect the input filters in the chunk query.
        /// </summary>
        public IEnumerable<ApplicationHealthStateChunk> Items { get; }
    }
}
