// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ApplicationTypeStatus.
    /// </summary>
    public enum ApplicationTypeStatus
    {
        /// <summary>
        /// Indicates the application type status is invalid. All Service Fabric enumerations have the invalid type. The value
        /// is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the application type is being provisioned in the cluster. The value is 1.
        /// </summary>
        Provisioning,

        /// <summary>
        /// Indicates that the application type is fully provisioned and is available for use. An application of this type and
        /// version can be created. The value is 2.
        /// </summary>
        Available,

        /// <summary>
        /// Indicates that the application type is in process of being unprovisioned from the cluster. The value is 3.
        /// </summary>
        Unprovisioning,

        /// <summary>
        /// Indicates that the application type provisioning failed and it is unavailable for use. The failure details can be
        /// obtained from the application type information query. The failed application type information remains in the
        /// cluster until it is unprovisioned or reprovisioned successfully. The value is 4.
        /// </summary>
        Failed,
    }
}
