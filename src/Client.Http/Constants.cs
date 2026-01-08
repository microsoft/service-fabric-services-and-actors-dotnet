// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    /// <summary>
    /// Contains Constant string values used by the library.
    /// </summary>
    internal class Constants
    {
        /// <summary>
        /// Constant string for request id in header.
        /// </summary>
        public const string ServiceFabricHttpRequestIdHeaderName = "X-ServiceFabricRequestId";

        /// <summary>
        /// Constant string for client type in header.
        /// </summary>
        public const string ServiceFabricHttpClientTypeHeaderName = "X-ServiceFabricClientType";

        /// <summary>
        /// Constant string default api version for resources.
        /// </summary>
        public const string DefaultApiVersionForResources = "6.4-preview";

        /// <summary>
        /// Constant string for client type in header.
        /// </summary>
        public const string ClientlibClientTypeHeaderValue = "CSharpClientlib";
    }
}
