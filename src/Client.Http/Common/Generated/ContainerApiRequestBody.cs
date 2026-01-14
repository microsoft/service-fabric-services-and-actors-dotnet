// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// parameters for making container API call.
    /// </summary>
    public partial class ContainerApiRequestBody
    {
        /// <summary>
        /// Initializes a new instance of the ContainerApiRequestBody class.
        /// </summary>
        /// <param name="uriPath">URI path of container REST API</param>
        /// <param name="httpVerb">HTTP verb of container REST API, defaults to "GET"</param>
        /// <param name="contentType">Content type of container REST API request, defaults to "application/json"</param>
        /// <param name="body">HTTP request body of container REST API</param>
        public ContainerApiRequestBody(
            string uriPath,
            string httpVerb = default(string),
            string contentType = default(string),
            string body = default(string))
        {
            uriPath.ThrowIfNull(nameof(uriPath));
            this.UriPath = uriPath;
            this.HttpVerb = httpVerb;
            this.ContentType = contentType;
            this.Body = body;
        }

        /// <summary>
        /// Gets HTTP verb of container REST API, defaults to "GET"
        /// </summary>
        public string HttpVerb { get; }

        /// <summary>
        /// Gets URI path of container REST API
        /// </summary>
        public string UriPath { get; }

        /// <summary>
        /// Gets content type of container REST API request, defaults to "application/json"
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Gets HTTP request body of container REST API
        /// </summary>
        public string Body { get; }
    }
}
