// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Security
{
    /// <summary>
    /// Defines how Service Fabric Cluster is secured.
    /// </summary>
    public enum SecurityType
    {
        /// <summary>
        /// Cluster is not secured.
        /// </summary>
        None = 0,

        /// <summary>
        /// Cluster is secured with X509 certificate.
        /// </summary>
        X509,

        /// <summary>
        /// Cluster is secured with claims token acquired from Azure Active Directory.
        /// </summary>
        Claims,

        /// <summary>
        /// Cluster is secured with Windows credentials.
        /// </summary>
        Windows,
    }
}
