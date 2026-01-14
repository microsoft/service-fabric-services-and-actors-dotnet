// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Http probe for the container.
    /// </summary>
    public partial class ProbeHttpGet
    {
        /// <summary>
        /// Initializes a new instance of the ProbeHttpGet class.
        /// </summary>
        /// <param name="port">Port to access for probe.</param>
        /// <param name="path">Path to access on the HTTP request.</param>
        /// <param name="host">Host IP to connect to.</param>
        /// <param name="httpHeaders">Headers to set in the request.</param>
        /// <param name="scheme">Scheme for the http probe. Can be Http or Https. Possible values include: 'http',
        /// 'https'</param>
        public ProbeHttpGet(
            int? port,
            string path = default(string),
            string host = default(string),
            IEnumerable<ProbeHttpGetHeaders> httpHeaders = default(IEnumerable<ProbeHttpGetHeaders>),
            Scheme? scheme = default(Scheme?))
        {
            port.ThrowIfNull(nameof(port));
            this.Port = port;
            this.Path = path;
            this.Host = host;
            this.HttpHeaders = httpHeaders;
            this.Scheme = scheme;
        }

        /// <summary>
        /// Gets port to access for probe.
        /// </summary>
        public int? Port { get; }

        /// <summary>
        /// Gets path to access on the HTTP request.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets host IP to connect to.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Gets headers to set in the request.
        /// </summary>
        public IEnumerable<ProbeHttpGetHeaders> HttpHeaders { get; }

        /// <summary>
        /// Gets scheme for the http probe. Can be Http or Https. Possible values include: 'http', 'https'
        /// </summary>
        public Scheme? Scheme { get; }
    }
}
