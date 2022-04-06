// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
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

        /// <summary>
        /// Starts the Downtime phase on the current partition. In the downtime phase all the actor calls are actively rejected with MigrationException.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on the asynchronous operation</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPut("StartDowntime")]
        public async Task StartDowntimeAsync(CancellationToken cancellationToken)
        {
            await this.migrationOrchestrator.StartDowntimeAsync(CancellationToken.None);
        }

        /// <summary>
        /// Aborts the Actor state migration on the current partition.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on the asynchronous operation</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("AbortMigration")]
        public async Task AbortMigrationAsync(CancellationToken cancellationToken)
        {
            await this.migrationOrchestrator.AbortMigrationAsync(cancellationToken);
        }

        /// <summary>
        /// Starts the Actor state migration on the current partition.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on the asynchronous operation</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("StartMigration")]
        public async Task StartMigrationAsync(CancellationToken cancellationToken)
        {
            await this.migrationOrchestrator.StartMigrationAsync(cancellationToken);
        }
    }
}
