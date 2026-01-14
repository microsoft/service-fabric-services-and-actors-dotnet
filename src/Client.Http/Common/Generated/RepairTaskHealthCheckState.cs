// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for RepairTaskHealthCheckState.
    /// </summary>
    public enum RepairTaskHealthCheckState
    {
        /// <summary>
        /// Indicates that the health check has not started.
        /// </summary>
        NotStarted,

        /// <summary>
        /// Indicates that the health check is in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// Indicates that the health check succeeded.
        /// </summary>
        Succeeded,

        /// <summary>
        /// Indicates that the health check was skipped.
        /// </summary>
        Skipped,

        /// <summary>
        /// Indicates that the health check timed out.
        /// </summary>
        TimedOut,
    }
}
