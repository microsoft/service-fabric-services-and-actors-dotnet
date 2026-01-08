// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes a service resource.
    /// </summary>
    public partial class ServiceResourceDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServiceResourceDescription class.
        /// </summary>
        /// <param name="name">Name of the Service resource.</param>
        /// <param name="properties">This type describes properties of a service resource.</param>
        public ServiceResourceDescription(
            string name,
            ServiceResourceProperties properties)
        {
            name.ThrowIfNull(nameof(name));
            properties.ThrowIfNull(nameof(properties));
            this.Name = name;
            this.Properties = properties;
        }

        /// <summary>
        /// Gets name of the Service resource.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets this type describes properties of a service resource.
        /// </summary>
        public ServiceResourceProperties Properties { get; }
    }
}
