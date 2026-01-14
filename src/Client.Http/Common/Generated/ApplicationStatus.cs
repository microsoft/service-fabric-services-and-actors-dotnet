// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ApplicationStatus.
    /// </summary>
    public enum ApplicationStatus
    {
        /// <summary>
        /// Indicates the application status is invalid. All Service Fabric enumerations have the invalid type. The value is
        /// zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates the application status is ready. The value is 1.
        /// </summary>
        Ready,

        /// <summary>
        /// Indicates the application status is upgrading. The value is 2.
        /// </summary>
        Upgrading,

        /// <summary>
        /// Indicates the application status is creating. The value is 3.
        /// </summary>
        Creating,

        /// <summary>
        /// Indicates the application status is deleting. The value is 4.
        /// </summary>
        Deleting,

        /// <summary>
        /// Indicates the creation or deletion of application was terminated due to persistent failures. Another create/delete
        /// request can be accepted to resume a failed application. The value is 5.
        /// </summary>
        Failed,
    }
}
