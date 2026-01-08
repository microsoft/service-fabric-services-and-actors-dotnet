// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the tcp configuration for external connectivity for this network.
    /// </summary>
    public partial class TcpConfig
    {
        /// <summary>
        /// Initializes a new instance of the TcpConfig class.
        /// </summary>
        /// <param name="name">tcp gateway config name.</param>
        /// <param name="port">Specifies the port at which the service endpoint below needs to be exposed.</param>
        /// <param name="destination">Describes destination endpoint for routing traffic.</param>
        public TcpConfig(
            string name,
            int? port,
            GatewayDestination destination)
        {
            name.ThrowIfNull(nameof(name));
            port.ThrowIfNull(nameof(port));
            destination.ThrowIfNull(nameof(destination));
            this.Name = name;
            this.Port = port;
            this.Destination = destination;
        }

        /// <summary>
        /// Gets tcp gateway config name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets specifies the port at which the service endpoint below needs to be exposed.
        /// </summary>
        public int? Port { get; }

        /// <summary>
        /// Gets destination endpoint for routing traffic.
        /// </summary>
        public GatewayDestination Destination { get; }
    }
}
