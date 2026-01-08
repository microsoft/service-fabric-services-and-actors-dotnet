// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ComposeDeploymentStatus.
    /// </summary>
    public enum ComposeDeploymentStatus
    {
        /// <summary>
        /// Indicates that the compose deployment status is invalid. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the compose deployment is being provisioned in background. The value is 1.
        /// </summary>
        Provisioning,

        /// <summary>
        /// Indicates that the compose deployment is being created in background. The value is 2.
        /// </summary>
        Creating,

        /// <summary>
        /// Indicates that the compose deployment has been successfully created or upgraded. The value is 3.
        /// </summary>
        Ready,

        /// <summary>
        /// Indicates that the compose deployment is being unprovisioned in background. The value is 4.
        /// </summary>
        Unprovisioning,

        /// <summary>
        /// Indicates that the compose deployment is being deleted in background. The value is 5.
        /// </summary>
        Deleting,

        /// <summary>
        /// Indicates that the compose deployment was terminated due to persistent failures. The value is 6.
        /// </summary>
        Failed,

        /// <summary>
        /// Indicates that the compose deployment is being upgraded in the background. The value is 7.
        /// </summary>
        Upgrading,
    }
}
