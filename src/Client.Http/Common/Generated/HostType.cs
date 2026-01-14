// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for HostType.
    /// </summary>
    public enum HostType
    {
        /// <summary>
        /// Indicates the type of host is not known or invalid. The value is 0.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates the host is an executable. The value is 1.
        /// </summary>
        ExeHost,

        /// <summary>
        /// Indicates the host is a container. The value is 2.
        /// </summary>
        ContainerHost,
    }
}
