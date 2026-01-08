// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ServiceEndpointRole.
    /// </summary>
    public enum ServiceEndpointRole
    {
        /// <summary>
        /// Indicates the service endpoint role is invalid. All Service Fabric enumerations have the invalid type. The value is
        /// zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the service endpoint is of a stateless service. The value is 1.
        /// </summary>
        Stateless,

        /// <summary>
        /// Indicates that the service endpoint is of a primary replica of a stateful service. The value is 2.
        /// </summary>
        StatefulPrimary,

        /// <summary>
        /// Indicates that the service endpoint is of a secondary replica of a stateful service. The value is 3.
        /// </summary>
        StatefulSecondary,
    }
}
