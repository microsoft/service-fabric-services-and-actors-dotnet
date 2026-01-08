// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes header information for http route matching.
    /// </summary>
    public partial class HttpRouteMatchHeader
    {
        /// <summary>
        /// Initializes a new instance of the HttpRouteMatchHeader class.
        /// </summary>
        /// <param name="name">Name of header to match in request.</param>
        /// <param name="value">Value of header to match in request.</param>
        /// <param name="type">how to match header value. Possible values include: 'exact'</param>
        public HttpRouteMatchHeader(
            string name,
            string value = default(string),
            HeaderMatchType? type = default(HeaderMatchType?))
        {
            name.ThrowIfNull(nameof(name));
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }

        /// <summary>
        /// Gets name of header to match in request.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets value of header to match in request.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets how to match header value. Possible values include: 'exact'
        /// </summary>
        public HeaderMatchType? Type { get; }
    }
}
