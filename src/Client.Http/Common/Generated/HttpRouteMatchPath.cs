// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Path to match for routing.
    /// </summary>
    public partial class HttpRouteMatchPath
    {
        /// <summary>
        /// Initializes a new instance of the HttpRouteMatchPath class.
        /// </summary>
        /// <param name="value">Uri path to match for request.</param>
        /// <param name="rewrite">replacement string for matched part of the Uri.</param>
        public HttpRouteMatchPath(
            string value,
            string rewrite = default(string))
        {
            value.ThrowIfNull(nameof(value));
            this.Value = value;
            this.Rewrite = rewrite;
        }

        /// <summary>
        /// Gets uri path to match for request.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets replacement string for matched part of the Uri.
        /// </summary>
        public string Rewrite { get; }
    }
}
