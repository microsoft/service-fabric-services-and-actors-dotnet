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
    public partial class HttpHostConfig
    {
        /// <summary>
        /// Initializes a new instance of the HttpHostConfig class.
        /// </summary>
        /// <param name="name">http hostname config name.</param>
        /// <param name="routes">Route information to use for routing. Routes are processed in the order they are specified.
        /// Specify routes that are more specific before routes that can handle general cases.</param>
        public HttpHostConfig(
            string name,
            IEnumerable<HttpRouteConfig> routes)
        {
            name.ThrowIfNull(nameof(name));
            routes.ThrowIfNull(nameof(routes));
            this.Name = name;
            this.Routes = routes;
        }

        /// <summary>
        /// Gets http hostname config name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets route information to use for routing. Routes are processed in the order they are specified. Specify routes
        /// that are more specific before routes that can handle general cases.
        /// </summary>
        public IEnumerable<HttpRouteConfig> Routes { get; }
    }
}
