// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    /// <summary>
    /// Indicates actor service migration phase.
    /// </summary>
    public enum MigrationStatus
    {
        /// <summary>
        /// Migration not started.
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        /// Copy phase of migration.
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Catchup phase of migration.
        /// </summary>
        Completed = 2,
    }
}
