// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes a network resource.
    /// </summary>
    public partial class NetworkResourceDescription
    {
        /// <summary>
        /// Initializes a new instance of the NetworkResourceDescription class.
        /// </summary>
        /// <param name="name">Name of the Network resource.</param>
        /// <param name="properties">Describes properties of a network resource.</param>
        public NetworkResourceDescription(
            string name,
            NetworkResourceProperties properties)
        {
            name.ThrowIfNull(nameof(name));
            properties.ThrowIfNull(nameof(properties));
            this.Name = name;
            this.Properties = properties;
        }

        /// <summary>
        /// Gets name of the Network resource.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets properties of a network resource.
        /// </summary>
        public NetworkResourceProperties Properties { get; }
    }
}
