// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Common.Security;

    /// <summary>
    /// Represents connection settings for <see cref="ServiceFabricClient"/>
    /// </summary>
    public class ClientSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSettings"/> class.
        /// </summary>
        /// <param name="clientTimeout">Timespan to wait before the request times out for the client.</param>
        public ClientSettings(TimeSpan? clientTimeout = null)
        {
            this.ClientTimeout = clientTimeout;
        }

        /// <summary>
        /// Gets or sets the Timespan to wait before the request times out for the client.
        /// </summary>
        public TimeSpan? ClientTimeout { get; set; }
    }
}
