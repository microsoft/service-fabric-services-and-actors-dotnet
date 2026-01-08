// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a rule for http route matching.
    /// </summary>
    public partial class HttpRouteMatchRule
    {
        /// <summary>
        /// Initializes a new instance of the HttpRouteMatchRule class.
        /// </summary>
        /// <param name="path">Path to match for routing.</param>
        /// <param name="headers">headers and their values to match in request.</param>
        public HttpRouteMatchRule(
            HttpRouteMatchPath path,
            IEnumerable<HttpRouteMatchHeader> headers = default(IEnumerable<HttpRouteMatchHeader>))
        {
            path.ThrowIfNull(nameof(path));
            this.Path = path;
            this.Headers = headers;
        }

        /// <summary>
        /// Gets path to match for routing.
        /// </summary>
        public HttpRouteMatchPath Path { get; }

        /// <summary>
        /// Gets headers and their values to match in request.
        /// </summary>
        public IEnumerable<HttpRouteMatchHeader> Headers { get; }
    }
}
