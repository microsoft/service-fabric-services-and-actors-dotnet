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
        None = 0,

        /// <summary>
        /// Migration in progress.
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Migration completed.
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Migration aborted.
        /// </summary>
        Aborted = 3,
    }
}
