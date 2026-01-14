// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a Service Fabric container network local to a single Service Fabric cluster.
    /// </summary>
    public partial class LocalNetworkResourceProperties : NetworkResourceProperties
    {
        /// <summary>
        /// Initializes a new instance of the LocalNetworkResourceProperties class.
        /// </summary>
        /// <param name="description">User readable description of the network.</param>
        /// <param name="networkAddressPrefix">Address space for the local container network.</param>
        public LocalNetworkResourceProperties(
            string description = default(string),
            string networkAddressPrefix = default(string))
            : base(
                Common.NetworkKind.Local,
                description)
        {
            this.NetworkAddressPrefix = networkAddressPrefix;
        }

        /// <summary>
        /// Gets address space for the local container network.
        /// </summary>
        public string NetworkAddressPrefix { get; }
    }
}
