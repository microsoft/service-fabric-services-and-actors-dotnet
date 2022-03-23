// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Actors.Runtime.Migration;

    /// <summary>
    /// Migration Controller Base class is added to be able to load internal migration Controllers
    /// </summary>
    internal abstract class MigrationControllerBase : ControllerBase
    {
        private IMigrationOrchestrator migrationOrchestrator;

        public MigrationControllerBase(IMigrationOrchestrator migrationOrchestrator)
        {
            this.migrationOrchestrator = migrationOrchestrator;
        }

        internal IMigrationOrchestrator MigrationOrchestrator { get => this.migrationOrchestrator; }
    }
}
