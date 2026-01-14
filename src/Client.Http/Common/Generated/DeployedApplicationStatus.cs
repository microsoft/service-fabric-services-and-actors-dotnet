// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for DeployedApplicationStatus.
    /// </summary>
    public enum DeployedApplicationStatus
    {
        /// <summary>
        /// Indicates that deployment status is not valid. All Service Fabric enumerations have the invalid type. The value is
        /// zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the package is downloading from the ImageStore. The value is 1.
        /// </summary>
        Downloading,

        /// <summary>
        /// Indicates that the package is activating. The value is 2.
        /// </summary>
        Activating,

        /// <summary>
        /// Indicates that the package is active. The value is 3.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the package is upgrading. The value is 4.
        /// </summary>
        Upgrading,

        /// <summary>
        /// Indicates that the package is deactivating. The value is 5.
        /// </summary>
        Deactivating,
    }
}
