// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ServiceStatus.
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>
        /// Indicates the service status is unknown. The value is zero.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates the service status is active. The value is 1.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates the service is upgrading. The value is 2.
        /// </summary>
        Upgrading,

        /// <summary>
        /// Indicates the service is being deleted. The value is 3.
        /// </summary>
        Deleting,

        /// <summary>
        /// Indicates the service is being created. The value is 4.
        /// </summary>
        Creating,

        /// <summary>
        /// Indicates creation or deletion was terminated due to persistent failures. Another create/delete request can be
        /// accepted. The value is 5.
        /// </summary>
        Failed,
    }
}
