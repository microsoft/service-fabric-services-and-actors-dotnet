// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The list of service health state chunks that respect the input filters in the chunk query. Returned by get cluster
    /// health state chunks query.
    /// </summary>
    public partial class ServiceHealthStateChunkList
    {
        /// <summary>
        /// Initializes a new instance of the ServiceHealthStateChunkList class.
        /// </summary>
        /// <param name="items">The list of service health state chunks that respect the input filters in the chunk query.
        /// </param>
        public ServiceHealthStateChunkList(
            IEnumerable<ServiceHealthStateChunk> items = default(IEnumerable<ServiceHealthStateChunk>))
        {
            this.Items = items;
        }

        /// <summary>
        /// Gets the list of service health state chunks that respect the input filters in the chunk query.
        /// </summary>
        public IEnumerable<ServiceHealthStateChunk> Items { get; }
    }
}
