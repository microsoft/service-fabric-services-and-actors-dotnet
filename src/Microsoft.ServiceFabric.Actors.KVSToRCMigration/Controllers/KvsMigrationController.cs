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
        /// Sets the flag in KVS to reject all write operations
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPut("RejectWrites")]
        public async Task RejectWritesAsync()
        {
            await this.MigrationOrchestrator.StartDowntimeAsync(CancellationToken.None);
        }

        /// <summary>
        /// Sets the flag in KVS to resume all write opeations
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("ResumeWrites")]
        public async Task ResumeWritesAsync()
        {
            await this.MigrationOrchestrator.AbortMigrationAsync(CancellationToken.None);
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
