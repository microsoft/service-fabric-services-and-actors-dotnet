// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    static class Constants
    {
        internal const string ServiceFabricHttpRequestIdHeaderName = "X-ServiceFabricRequestId";
        internal const string ServiceFabricHttpClientTypeHeaderName = "X-ServiceFabricClientType";
        internal const string DefaultApiVersionForResources = "6.4-preview";
        internal const string ClientlibClientTypeHeaderValue = "CSharpClientlib";
    }
}
