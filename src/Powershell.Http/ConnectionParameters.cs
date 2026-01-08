// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    /// <summary>
    /// Represents Connection Parameters for connecting to Local Cluster.
    /// </summary>
    internal class ConnectionParameters
    {
        public string HttpConnectionEndpoint { get; set; }

        public string StoreLocation { get; set; }

        public string FindType { get; set; }

        public bool X509Credential { get; set; }

        public string FindValue { get; set; }

        public string ServerCommonName { get; set; }

        public string StoreName { get; set; }
    }
}
