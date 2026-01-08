// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a container endpoint.
    /// </summary>
    public partial class EndpointProperties
    {
        /// <summary>
        /// Initializes a new instance of the EndpointProperties class.
        /// </summary>
        /// <param name="name">The name of the endpoint.</param>
        /// <param name="port">Port used by the container.</param>
        public EndpointProperties(
            string name,
            int? port = default(int?))
        {
            name.ThrowIfNull(nameof(name));
            this.Name = name;
            this.Port = port;
        }

        /// <summary>
        /// Gets the name of the endpoint.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets port used by the container.
        /// </summary>
        public int? Port { get; }
    }
}
