// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for DisableServiceFlag.
    /// </summary>
    public enum DisableServiceFlag
    {
        /// <summary>
        /// Indicates that the deactivation flag is invalid. This value is not used.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the service replicas and data should be removed when the service is disabled. Service Fabric will
        /// initiate deletion of replicas for all partitions of the service.
        /// </summary>
        RemoveData,
    }
}
