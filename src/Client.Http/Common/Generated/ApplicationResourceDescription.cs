// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes a application resource.
    /// </summary>
    public partial class ApplicationResourceDescription
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationResourceDescription class.
        /// </summary>
        /// <param name="name">Name of the Application resource.</param>
        /// <param name="properties">Describes properties of a application resource.</param>
        /// <param name="identity">Describes the identity of the application.</param>
        public ApplicationResourceDescription(
            string name,
            ApplicationProperties properties,
            IdentityDescription identity = default(IdentityDescription))
        {
            name.ThrowIfNull(nameof(name));
            properties.ThrowIfNull(nameof(properties));
            this.Name = name;
            this.Properties = properties;
            this.Identity = identity;
        }

        /// <summary>
        /// Gets name of the Application resource.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets properties of a application resource.
        /// </summary>
        public ApplicationProperties Properties { get; }

        /// <summary>
        /// Gets the identity of the application.
        /// </summary>
        public IdentityDescription Identity { get; }
    }
}
