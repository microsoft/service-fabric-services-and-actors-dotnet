// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ResourceStatus.
    /// </summary>
    public enum ResourceStatus
    {
        /// <summary>
        /// Indicates the resource status is unknown. The value is zero.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates the resource is ready. The value is 1.
        /// </summary>
        Ready,

        /// <summary>
        /// Indicates the resource is upgrading. The value is 2.
        /// </summary>
        Upgrading,

        /// <summary>
        /// Indicates the resource is being created. The value is 3.
        /// </summary>
        Creating,

        /// <summary>
        /// Indicates the resource is being deleted. The value is 4.
        /// </summary>
        Deleting,

        /// <summary>
        /// Indicates the resource is not functional due to persistent failures. See statusDetails property for more details.
        /// The value is 5.
        /// </summary>
        Failed,
    }
}
