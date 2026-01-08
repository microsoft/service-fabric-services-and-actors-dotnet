// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An internal ID used by Service Fabric to uniquely identify a node. Node Id is deterministically generated from node
    /// name.
    /// </summary>
    public partial class NodeId
    {
        /// <summary>
        /// Initializes a new instance of the NodeId class.
        /// </summary>
        /// <param name="id">Value of the node Id. This is a 128 bit integer.</param>
        public NodeId(
            string id = default(string))
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets value of the node Id. This is a 128 bit integer.
        /// </summary>
        public string Id { get; }
    }
}
