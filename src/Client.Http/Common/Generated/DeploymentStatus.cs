// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for DeploymentStatus.
    /// </summary>
    public enum DeploymentStatus
    {
        /// <summary>
        /// Indicates status of the application or service package is not known or invalid. The value is 0.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates the application or service package is being downloaded to the node from the ImageStore. The value is 1.
        /// </summary>
        Downloading,

        /// <summary>
        /// Indicates the application or service package is being activated. The value is 2.
        /// </summary>
        Activating,

        /// <summary>
        /// Indicates the application or service package is active the node. The value is 3.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates the application or service package is being upgraded. The value is 4.
        /// </summary>
        Upgrading,

        /// <summary>
        /// Indicates the application or service package is being deactivated. The value is 5.
        /// </summary>
        Deactivating,

        /// <summary>
        /// Indicates the application or service package has ran to completion successfully. The value is 6.
        /// </summary>
        RanToCompletion,

        /// <summary>
        /// Indicates the application or service package has failed to run to completion. The value is 7.
        /// </summary>
        Failed,
    }
}
