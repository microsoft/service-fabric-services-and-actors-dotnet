// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Controllers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration.Models;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime.Migration;

    /// <summary>
    /// Represents the controller class for KVS migration REST API.
    /// </summary>
    [Route("[controller]")]
    internal class KvsMigrationController : ControllerBase
    {
        private KvsActorStateProvider kvsActorStateProvider;
        private IMigrationOrchestrator migrationOrchestrator;
        private KVSAmbiguousActorIdHandler ambiguousActorIdHandler;
        private string traceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="KvsMigrationController"/> class.
        /// </summary>
        /// <param name="migrationOrchestrator">Source migraiton orchestrator</param>
        public KvsMigrationController(IMigrationOrchestrator migrationOrchestrator)
        {
            this.migrationOrchestrator = migrationOrchestrator;
            this.kvsActorStateProvider = (KvsActorStateProvider)this.migrationOrchestrator.GetMigrationActorStateProvider();
            this.traceId = ((MigrationOrchestratorBase)this.migrationOrchestrator).TraceId;
            this.ambiguousActorIdHandler = new KVSAmbiguousActorIdHandler(this.kvsActorStateProvider);
        }

        /// <summary>
        /// Gets the First Sequence number of KVS
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on the asynchronous operation</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetFirstSequenceNumber")]
        public async Task<long> GetFirstSequenceNumberAsync(CancellationToken cancellationToken)
        {
            return await MigrationUtility.ExecuteWithRetriesAsync(
                () => this.kvsActorStateProvider.GetFirstSequenceNumberAsync(this.traceId, cancellationToken),
                this.traceId,
                $"{this.GetType().Name}.GetFirstSequenceNumberAsync");
        }

        /// <summary>
        /// Gets the Last Sequence number of KVS
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on the asynchronous operation</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetLastSequenceNumber")]
        public Task<long> GetLastSequenceNumberAsync(CancellationToken cancellationToken)
        {
            return MigrationUtility.ExecuteWithRetriesAsync(
                () => this.kvsActorStateProvider.GetLastSequenceNumberAsync(this.traceId, cancellationToken),
                this.traceId,
                $"{this.GetType().Name}.GetLastSequenceNumber");
        }

        /// <summary>
        /// Enumerates Key value store data by Sequence Number
        /// </summary>
        /// <param name="request">EnumerationRequest</param>
        /// <param name="cancellationToken">Token to signal cancellation on the asynchronous operation</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("EnumerateBySequenceNumber")]
        public async Task EnumerateBySequenceNumberAsync([FromBody] EnumerationRequest request, CancellationToken cancellationToken)
        {
            await MigrationUtility.ExecuteWithRetriesAsync(
                () => this.kvsActorStateProvider.EnumerateAsync(request, this.Response, this.traceId, cancellationToken),
                this.traceId,
                $"{this.GetType().Name}.EnumerateAsync");
        }

        /// <summary>
        /// Gets DisableTombstoneCleanup Setting value if KVSReplica
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("IsTombstoneCleanupDisabled")]
        public bool IsTombstoneCleanupDisabled()
        {
            return this.kvsActorStateProvider.IsTombstoneCleanupDisabled();
        }

        /// <summary>
        /// Gets Value for given Keys
        /// </summary>
        /// <param name="keys">Value of Key to fetch</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetValueByKeys")]
        public Task GetValueByKeys([FromBody] List<string> keys)
        {
            return MigrationUtility.ExecuteWithRetriesAsync(
                () => this.kvsActorStateProvider.GetValueByKeysAsync(keys, this.Response, CancellationToken.None),
                this.traceId,
                $"{this.GetType().Name}.GetValueByKeysAsync");
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
                () => this.migrationOrchestrator.StartDowntimeAsync(false, cancellationToken),
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
                () => this.migrationOrchestrator.AbortMigrationAsync(false, cancellationToken),
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
                 () => this.migrationOrchestrator.StartMigrationAsync(false, cancellationToken),
                 this.traceId,
                 $"{this.GetType().Name}.StartMigrationAsync");
        }
    }
}
