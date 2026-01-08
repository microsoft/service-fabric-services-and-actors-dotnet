// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the http configuration for external connectivity for this network.
    /// </summary>
    public partial class HttpConfig
    {
        /// <summary>
        /// Initializes a new instance of the HttpConfig class.
        /// </summary>
        /// <param name="name">http gateway config name.</param>
        /// <param name="port">Specifies the port at which the service endpoint below needs to be exposed.</param>
        /// <param name="hosts">description for routing.</param>
        public HttpConfig(
            string name,
            int? port,
            IEnumerable<HttpHostConfig> hosts)
        {
            name.ThrowIfNull(nameof(name));
            port.ThrowIfNull(nameof(port));
            hosts.ThrowIfNull(nameof(hosts));
            this.Name = name;
            this.Port = port;
            this.Hosts = hosts;
        }

        /// <summary>
        /// Gets http gateway config name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets specifies the port at which the service endpoint below needs to be exposed.
        /// </summary>
        public int? Port { get; }

        /// <summary>
        /// Gets description for routing.
        /// </summary>
        public IEnumerable<HttpHostConfig> Hosts { get; }
    }
}
