// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes properties of a network resource.
    /// </summary>
    public partial class NetworkResourceProperties : NetworkResourcePropertiesBase
    {
        /// <summary>
        /// Initializes a new instance of the NetworkResourceProperties class.
        /// </summary>
        /// <param name="kind">The type of a Service Fabric container network.</param>
        /// <param name="description">User readable description of the network.</param>
        public NetworkResourceProperties(
            NetworkKind? kind,
            string description = default(string))
            : base(
                kind)
        {
            this.Description = description;
        }

        /// <summary>
        /// Gets user readable description of the network.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets status of the network. Possible values include: 'Unknown', 'Ready', 'Upgrading', 'Creating', 'Deleting',
        /// 'Failed'
        /// 
        /// Status of the resource.
        /// </summary>
        public ResourceStatus? Status { get; internal set; }

        /// <summary>
        /// Gets additional information about the current status of the network.
        /// </summary>
        public string StatusDetails { get; internal set; }
    }
}
