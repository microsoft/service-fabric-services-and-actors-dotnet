// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes a value of a secret resource. The name of this resource is the version identifier corresponding
    /// to this secret value.
    /// </summary>
    public partial class SecretValueResourceDescription
    {
        /// <summary>
        /// Initializes a new instance of the SecretValueResourceDescription class.
        /// </summary>
        /// <param name="name">Version identifier of the secret value.</param>
        /// <param name="properties">This type describes properties of a secret value resource.</param>
        public SecretValueResourceDescription(
            string name,
            SecretValueResourceProperties properties)
        {
            name.ThrowIfNull(nameof(name));
            properties.ThrowIfNull(nameof(properties));
            this.Name = name;
            this.Properties = properties;
        }

        /// <summary>
        /// Gets version identifier of the secret value.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets this type describes properties of a secret value resource.
        /// </summary>
        public SecretValueResourceProperties Properties { get; }
    }
}
