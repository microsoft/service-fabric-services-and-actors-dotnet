// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    using System;

    /// <summary>
    /// Migration Mode.
    /// </summary>
    public enum MigrationMode
    {
        /// <summary>
        /// Automatic Mode.
        /// Migration is started automatically after the Target service is up.
        /// Downtime on the Source service is also invoked automatically when the Downtime threshold is met.
        /// </summary>
        Auto,

        /// <summary>
        /// Manual Mode.
        /// Migration is not started automatically. User needs to use StartMigration Web Api partition wise to start migration.
        /// Similarly Downtime is not invoked automaticaly. User needs to use StartDowntime Web Api partition wise to start downtime.
        /// GetMigrationStatus Web Api can be used to check the current status of the migration and decide whether to invoke downtime or not.
        /// </summary>
        Manual,
    }
}
