// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes a secret resource.
    /// </summary>
    public partial class SecretResourceDescription
    {
        /// <summary>
        /// Initializes a new instance of the SecretResourceDescription class.
        /// </summary>
        /// <param name="properties">Describes the properties of a secret resource.</param>
        /// <param name="name">Name of the Secret resource.</param>
        public SecretResourceDescription(
            SecretResourceProperties properties,
            string name)
        {
            properties.ThrowIfNull(nameof(properties));
            name.ThrowIfNull(nameof(name));
            this.Properties = properties;
            this.Name = name;
        }

        /// <summary>
        /// Gets the properties of a secret resource.
        /// </summary>
        public SecretResourceProperties Properties { get; }

        /// <summary>
        /// Gets name of the Secret resource.
        /// </summary>
        public string Name { get; }
    }
}
