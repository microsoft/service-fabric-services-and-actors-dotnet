// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Tcp port to probe inside the container.
    /// </summary>
    public partial class ProbeTcpSocket
    {
        /// <summary>
        /// Initializes a new instance of the ProbeTcpSocket class.
        /// </summary>
        /// <param name="port">Port to access for probe.</param>
        public ProbeTcpSocket(
            int? port)
        {
            port.ThrowIfNull(nameof(port));
            this.Port = port;
        }

        /// <summary>
        /// Gets port to access for probe.
        /// </summary>
        public int? Port { get; }
    }
}
