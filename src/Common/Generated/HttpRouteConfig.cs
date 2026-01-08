// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the hostname properties for http routing.
    /// </summary>
    public partial class HttpRouteConfig
    {
        /// <summary>
        /// Initializes a new instance of the HttpRouteConfig class.
        /// </summary>
        /// <param name="name">http route name.</param>
        /// <param name="match">Describes a rule for http route matching.</param>
        /// <param name="destination">Describes destination endpoint for routing traffic.</param>
        public HttpRouteConfig(
            string name,
            HttpRouteMatchRule match,
            GatewayDestination destination)
        {
            name.ThrowIfNull(nameof(name));
            match.ThrowIfNull(nameof(match));
            destination.ThrowIfNull(nameof(destination));
            this.Name = name;
            this.Match = match;
            this.Destination = destination;
        }

        /// <summary>
        /// Gets http route name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a rule for http route matching.
        /// </summary>
        public HttpRouteMatchRule Match { get; }

        /// <summary>
        /// Gets destination endpoint for routing traffic.
        /// </summary>
        public GatewayDestination Destination { get; }
    }
}
