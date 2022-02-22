// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    /// <summary>
    /// Indicates actor service migration state.
    /// </summary>
    public enum MigrationState
    {
        /// <summary>
        /// Migration not started.
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        /// Migration InProgress.
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Migration Completed.
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Validating Migration Data.
        /// </summary>
        Validating = 3,

        /// <summary>
        /// Migration Failed.
        /// </summary>
        Failed = 4,
    }
}
