// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Container API result.
    /// </summary>
    public partial class ContainerApiResult
    {
        /// <summary>
        /// Initializes a new instance of the ContainerApiResult class.
        /// </summary>
        /// <param name="status">HTTP status code returned by the target container API</param>
        /// <param name="contentType">HTTP content type</param>
        /// <param name="contentEncoding">HTTP content encoding</param>
        /// <param name="body">container API result body</param>
        public ContainerApiResult(
            int? status,
            string contentType = default(string),
            string contentEncoding = default(string),
            string body = default(string))
        {
            status.ThrowIfNull(nameof(status));
            this.Status = status;
            this.ContentType = contentType;
            this.ContentEncoding = contentEncoding;
            this.Body = body;
        }

        /// <summary>
        /// Gets HTTP status code returned by the target container API
        /// </summary>
        public int? Status { get; }

        /// <summary>
        /// Gets HTTP content type
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Gets HTTP content encoding
        /// </summary>
        public string ContentEncoding { get; }

        /// <summary>
        /// Gets container API result body
        /// </summary>
        public string Body { get; }
    }
}
