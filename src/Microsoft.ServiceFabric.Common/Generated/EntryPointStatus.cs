// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for EntryPointStatus.
    /// </summary>
    public enum EntryPointStatus
    {
        /// <summary>
        /// Indicates status of entry point is not known or invalid. The value is 0.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates the entry point is scheduled to be started. The value is 1.
        /// </summary>
        Pending,

        /// <summary>
        /// Indicates the entry point is being started. The value is 2.
        /// </summary>
        Starting,

        /// <summary>
        /// Indicates the entry point was started successfully and is running. The value is 3.
        /// </summary>
        Started,

        /// <summary>
        /// Indicates the entry point is being stopped. The value is 4.
        /// </summary>
        Stopping,

        /// <summary>
        /// Indicates the entry point is not running. The value is 5.
        /// </summary>
        Stopped,
    }
}
