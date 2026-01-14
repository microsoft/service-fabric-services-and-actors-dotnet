// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes properties of a gateway resource.
    /// </summary>
    public partial class GatewayProperties
    {
        /// <summary>
        /// Initializes a new instance of the GatewayProperties class.
        /// </summary>
        /// <param name="sourceNetwork">Network the gateway should listen on for requests.</param>
        /// <param name="destinationNetwork">Network that the Application is using.</param>
        /// <param name="description">User readable description of the gateway.</param>
        /// <param name="tcp">Configuration for tcp connectivity for this gateway.</param>
        /// <param name="http">Configuration for http connectivity for this gateway.</param>
        public GatewayProperties(
            NetworkRef sourceNetwork,
            NetworkRef destinationNetwork,
            string description = default(string),
            IEnumerable<TcpConfig> tcp = default(IEnumerable<TcpConfig>),
            IEnumerable<HttpConfig> http = default(IEnumerable<HttpConfig>))
        {
            sourceNetwork.ThrowIfNull(nameof(sourceNetwork));
            destinationNetwork.ThrowIfNull(nameof(destinationNetwork));
            this.SourceNetwork = sourceNetwork;
            this.DestinationNetwork = destinationNetwork;
            this.Description = description;
            this.Tcp = tcp;
            this.Http = http;
        }

        /// <summary>
        /// Gets user readable description of the gateway.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets network the gateway should listen on for requests.
        /// </summary>
        public NetworkRef SourceNetwork { get; }

        /// <summary>
        /// Gets network that the Application is using.
        /// </summary>
        public NetworkRef DestinationNetwork { get; }

        /// <summary>
        /// Gets configuration for tcp connectivity for this gateway.
        /// </summary>
        public IEnumerable<TcpConfig> Tcp { get; }

        /// <summary>
        /// Gets configuration for http connectivity for this gateway.
        /// </summary>
        public IEnumerable<HttpConfig> Http { get; }

        /// <summary>
        /// Gets status of the resource. Possible values include: 'Unknown', 'Ready', 'Upgrading', 'Creating', 'Deleting',
        /// 'Failed'
        /// </summary>
        public ResourceStatus? Status { get; internal set; }

        /// <summary>
        /// Gets additional information about the current status of the gateway.
        /// </summary>
        public string StatusDetails { get; internal set; }

        /// <summary>
        /// Gets IP address of the gateway. This is populated in the response and is ignored for incoming requests.
        /// </summary>
        public string IpAddress { get; internal set; }
    }
}
