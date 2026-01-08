// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ManagedIdentityType.
    /// </summary>
    public enum ManagedIdentityType
    {
        /// <summary>
        /// Indicates an invalid managed identity type. All Service Fabric enumerations have the invalid type.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates VMSS managed identity should be used to connect to Azure blob store.
        /// </summary>
        VMSS,

        /// <summary>
        /// Indicates cluster managed identity should be used to connect to Azure blob store.
        /// </summary>
        Cluster,
    }
}
