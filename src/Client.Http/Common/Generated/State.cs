// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for State.
    /// </summary>
    public enum State
    {
        /// <summary>
        /// Indicates that the repair task state is invalid. All Service Fabric enumerations have the invalid value.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the repair task has been created.
        /// </summary>
        Created,

        /// <summary>
        /// Indicates that the repair task has been claimed by a repair executor.
        /// </summary>
        Claimed,

        /// <summary>
        /// Indicates that the Repair Manager is preparing the system to handle the impact of the repair task, usually by
        /// taking resources offline gracefully.
        /// </summary>
        Preparing,

        /// <summary>
        /// Indicates that the repair task has been approved by the Repair Manager and is safe to execute.
        /// </summary>
        Approved,

        /// <summary>
        /// Indicates that execution of the repair task is in progress.
        /// </summary>
        Executing,

        /// <summary>
        /// Indicates that the Repair Manager is restoring the system to its pre-repair state, usually by bringing resources
        /// back online.
        /// </summary>
        Restoring,

        /// <summary>
        /// Indicates that the repair task has completed, and no further state changes will occur.
        /// </summary>
        Completed,
    }
}
