// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    /// <summary>
    /// Indicates actor service migration phase.
    /// </summary>
    public enum MigrationPhase
    {
        /// <summary>
        /// Migration not started.
        /// </summary>
        None = 0,

        /// <summary>
        /// Copy phase of migration.
        /// </summary>
        Copy = 1,

        /// <summary>
        /// Catchup phase of migration.
        /// </summary>
        Catchup = 2,

        /// <summary>
        /// Downtime phase of migration.
        /// </summary>
        Downtime = 3,

        /// <summary>
        /// Data Validation phase of migration.
        /// </summary>
        DataValidation = 4,

        /// <summary>
        /// Migration completed.
        /// </summary>
        Completed = 5,
    }
}
