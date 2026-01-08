// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The list of deployed application health state chunks that respect the input filters in the chunk query. Returned by
    /// get cluster health state chunks query.
    /// </summary>
    public partial class DeployedApplicationHealthStateChunkList
    {
        /// <summary>
        /// Initializes a new instance of the DeployedApplicationHealthStateChunkList class.
        /// </summary>
        /// <param name="items">The list of deployed application health state chunks that respect the input filters in the
        /// chunk query.
        /// </param>
        public DeployedApplicationHealthStateChunkList(
            IEnumerable<DeployedApplicationHealthStateChunk> items = default(IEnumerable<DeployedApplicationHealthStateChunk>))
        {
            this.Items = items;
        }

        /// <summary>
        /// Gets the list of deployed application health state chunks that respect the input filters in the chunk query.
        /// </summary>
        public IEnumerable<DeployedApplicationHealthStateChunk> Items { get; }
    }
}
