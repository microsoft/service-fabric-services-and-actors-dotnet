// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes a volume resource.
    /// </summary>
    public partial class VolumeResourceDescription
    {
        /// <summary>
        /// Initializes a new instance of the VolumeResourceDescription class.
        /// </summary>
        /// <param name="name">Name of the Volume resource.</param>
        /// <param name="properties">Describes properties of a volume resource.</param>
        public VolumeResourceDescription(
            string name,
            VolumeProperties properties)
        {
            name.ThrowIfNull(nameof(name));
            properties.ThrowIfNull(nameof(properties));
            this.Name = name;
            this.Properties = properties;
        }

        /// <summary>
        /// Gets name of the Volume resource.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets properties of a volume resource.
        /// </summary>
        public VolumeProperties Properties { get; }
    }
}
