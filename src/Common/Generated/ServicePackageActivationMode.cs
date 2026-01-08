// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ServicePackageActivationMode.
    /// </summary>
    public enum ServicePackageActivationMode
    {
        /// <summary>
        /// This is the default activation mode. With this activation mode, replicas or instances from different partition(s)
        /// of service, on a given node, will share same activation of service package on a node. The value is zero.
        /// </summary>
        SharedProcess,

        /// <summary>
        /// With this activation mode, each replica or instance of service, on a given node, will have its own dedicated
        /// activation of service package on a node. The value is 1.
        /// </summary>
        ExclusiveProcess,
    }
}
