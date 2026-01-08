// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for HostIsolationMode.
    /// </summary>
    public enum HostIsolationMode
    {
        /// <summary>
        /// Indicates the isolation mode is not applicable for given HostType. The value is 0.
        /// </summary>
        None,

        /// <summary>
        /// This is the default isolation mode for a ContainerHost. The value is 1.
        /// </summary>
        Process,

        /// <summary>
        /// Indicates the ContainerHost is a Hyper-V container. This applies to only Windows containers. The value is 2.
        /// </summary>
        HyperV,
    }
}
