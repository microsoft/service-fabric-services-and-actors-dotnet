// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ResultStatus.
    /// </summary>
    public enum ResultStatus
    {
        /// <summary>
        /// Indicates that the repair task result is invalid. All Service Fabric enumerations have the invalid value.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the repair task completed execution successfully.
        /// </summary>
        Succeeded,

        /// <summary>
        /// Indicates that the repair task was cancelled prior to execution.
        /// </summary>
        Cancelled,

        /// <summary>
        /// Indicates that execution of the repair task was interrupted by a cancellation request after some work had already
        /// been performed.
        /// </summary>
        Interrupted,

        /// <summary>
        /// Indicates that there was a failure during execution of the repair task. Some work may have been performed.
        /// </summary>
        Failed,

        /// <summary>
        /// Indicates that the repair task result is not yet available, because the repair task has not finished executing.
        /// </summary>
        Pending,
    }
}
