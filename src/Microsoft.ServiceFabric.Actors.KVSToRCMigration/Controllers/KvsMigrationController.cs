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
    internal class KvsMigrationController : MigrationControllerBase
    {
        private KvsActorStateProvider kvsActorStateProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="KvsMigrationController"/> class.
        /// </summary>
        /// <param name="migrationOrchestrator">Source migraiton orchestrator</param>
        public KvsMigrationController(IMigrationOrchestrator migrationOrchestrator)
            : base(migrationOrchestrator)
        {
            this.kvsActorStateProvider = (KvsActorStateProvider)this.MigrationOrchestrator.GetMigrationActorStateProvider();
        }

        /// <summary>
        /// Gets the First Sequence number of KVS
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetFirstSequenceNumber")]
        public async Task<long> GetFirstSequenceNumber()
        {
            var sequenceNumber = await this.kvsActorStateProvider.GetFirstSequenceNumberAsync(CancellationToken.None);

            return sequenceNumber;
        }

        /// <summary>
        /// Gets the Last Sequence number of KVS
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetLastSequenceNumber")]
        public long GetLastSequenceNumber()
        {
            return this.kvsActorStateProvider.GetLastSequenceNumber();
        }

        /// <summary>
        /// Enumerates Key value store data by Sequence Number
        /// </summary>
        /// <param name="request">EnumerationRequest</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("EnumerateBySequenceNumber")]
        public Task EnumerateBySequenceNumber([FromBody] EnumerationRequest request)
        {
            return this.kvsActorStateProvider.EnumerateAsync(request, this.Response, CancellationToken.None);
        }

        /// <summary>
        /// Starts the Downtime phase on the current partition. In the downtime phase all the actor calls are actively rejected with MigrationException.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on the asynchronous operation</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPut("StartDowntime")]
        public async Task StartDowntimeAsync(CancellationToken cancellationToken)
        {
            await this.MigrationOrchestrator.StartDowntimeAsync(CancellationToken.None);
        }

        /// <summary>
        /// Aborts the Actor state migration on the current partition.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on the asynchronous operation</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("AbortMigration")]
        public async Task AbortMigrationAsync(CancellationToken cancellationToken)
        {
            await this.MigrationOrchestrator.AbortMigrationAsync(cancellationToken);
        }

        /// <summary>
        /// Starts the Actor state migration on the current partition.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on the asynchronous operation</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("StartMigration")]
        public async Task StartMigrationAsync(CancellationToken cancellationToken)
        {
            await this.MigrationOrchestrator.StartMigrationAsync(cancellationToken);
        }

        /// <summary>
        /// Gets DisableTombstoneCleanup Setting value if KVSReplica
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetDisableTombstoneCleanupSetting")]
        public bool GetDisableTombstoneCleanupSetting()
        {
            return this.kvsActorStateProvider.GetDisableTombstoneCleanupSetting();
        }

        /// <summary>
        /// Gets Value for given Keys
        /// </summary>
        /// <param name="keys">Value of Key to fetch</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetValueByKeys")]
        public Task GetValueByKeys([FromBody] List<string> keys)
        {
            return this.kvsActorStateProvider.GetValueByKeysAsync(keys, this.Response, CancellationToken.None);
        }
    }
}
