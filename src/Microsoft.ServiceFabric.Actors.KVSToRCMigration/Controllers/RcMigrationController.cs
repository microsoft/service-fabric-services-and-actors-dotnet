// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Runtime.Migration;

    /// <summary>
    /// Represents the controller class for KVS migration REST API.
    /// </summary>
    [Route("[controller]")]
    internal class RcMigrationController : ControllerBase
    {
        private IMigrationOrchestrator migrationOrchestrator;
        private string traceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="RcMigrationController"/> class.
        /// </summary>
        /// <param name="migrationOrchestrator">Target Migration orchestrator</param>
        public RcMigrationController(IMigrationOrchestrator migrationOrchestrator)
        {
            this.migrationOrchestrator = migrationOrchestrator;
            this.traceId = ((MigrationOrchestratorBase)this.migrationOrchestrator).TraceId;
        }

        /// <summary>
        /// Gets migration status
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetMigrationStatus")]
        public async Task<MigrationResult> GetMigrationStatusAsync(CancellationToken cancellationToken)
        {
            return await MigrationUtility.ExecuteWithRetriesAsync(
                () => ((TargetMigrationOrchestrator)this.migrationOrchestrator).GetResultAsync(cancellationToken),
                this.traceId,
                "RcMigrationController.GetMigrationStatusAsync");
        }

        /// <summary>
        /// Starts the Downtime phase on the current partition. In the downtime phase all the actor calls are actively rejected with MigrationException.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on the asynchronous operation</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPut("StartDowntime")]
        public async Task StartDowntimeAsync(CancellationToken cancellationToken)
        {
            await MigrationUtility.ExecuteWithRetriesAsync(
                () => this.migrationOrchestrator.StartDowntimeAsync(true, cancellationToken),
                this.traceId,
                $"{this.GetType().Name}.StartDowntimeAsync");
        }

        /// <summary>
        /// Aborts the Actor state migration on the current partition.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on the asynchronous operation</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("AbortMigration")]
        public async Task AbortMigrationAsync(CancellationToken cancellationToken)
        {
            await MigrationUtility.ExecuteWithRetriesAsync(
                () => this.migrationOrchestrator.AbortMigrationAsync(true, cancellationToken),
                this.traceId,
                $"{this.GetType().Name}.AbortMigrationAsync");
        }

        /// <summary>
        /// Starts the Actor state migration on the current partition.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on the asynchronous operation</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("StartMigration")]
        public async Task StartMigrationAsync(CancellationToken cancellationToken)
        {
            await MigrationUtility.ExecuteWithRetriesAsync(
                 () => this.migrationOrchestrator.StartMigrationAsync(true, cancellationToken),
                 this.traceId,
                 $"{this.GetType().Name}.StartMigrationAsync");
        }
    }
}
