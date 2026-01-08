// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes a gateway resource.
    /// </summary>
    public partial class GatewayResourceDescription
    {
        /// <summary>
        /// Initializes a new instance of the GatewayResourceDescription class.
        /// </summary>
        /// <param name="name">Name of the Gateway resource.</param>
        /// <param name="properties">Describes properties of a gateway resource.</param>
        public GatewayResourceDescription(
            string name,
            GatewayProperties properties)
        {
            name.ThrowIfNull(nameof(name));
            properties.ThrowIfNull(nameof(properties));
            this.Name = name;
            this.Properties = properties;
        }

        /// <summary>
        /// Gets name of the Gateway resource.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets properties of a gateway resource.
        /// </summary>
        public GatewayProperties Properties { get; }
    }
}
