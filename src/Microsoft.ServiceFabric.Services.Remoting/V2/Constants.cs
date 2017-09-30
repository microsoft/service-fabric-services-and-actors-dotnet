// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    internal static class Constants
    {
        public const string ServiceCommunicationNamespace = "urn:ServiceFabric.Communication";
        public const int DefaultHeaderBufferSize = 1024;
        public const int DefaultHeaderMaxBufferCount = 1000;
        public const int DefaultMessageBufferSize = 2 * 1024;
        public const int DefaultMaxBufferCount = 20000;
    }
}