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
            return await MigrationUtility.ExecuteWithRetriesAsync(
                () => this.kvsActorStateProvider.GetFirstSequenceNumberAsync(CancellationToken.None),
                ((MigrationOrchestratorBase)this.MigrationOrchestrator).TraceId,
                $"{this.GetType().Name}.GetFirstSequenceNumberAsync");
        }

        /// <summary>
        /// Gets the Last Sequence number of KVS
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetLastSequenceNumber")]
        public long GetLastSequenceNumber()
        {
            return MigrationUtility.ExecuteWithRetries(
                () => this.kvsActorStateProvider.GetLastSequenceNumber(),
                ((MigrationOrchestratorBase)this.MigrationOrchestrator).TraceId,
                $"{this.GetType().Name}.GetLastSequenceNumber");
        }

        /// <summary>
        /// Enumerates Key value store data by Sequence Number
        /// </summary>
        /// <param name="request">EnumerationRequest</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("EnumerateBySequenceNumber")]
        public async Task EnumerateBySequenceNumber([FromBody] EnumerationRequest request)
        {
            await MigrationUtility.ExecuteWithRetriesAsync(
                () => this.kvsActorStateProvider.EnumerateAsync(request, this.Response, CancellationToken.None),
                ((MigrationOrchestratorBase)this.MigrationOrchestrator).TraceId,
                $"{this.GetType().Name}.EnumerateAsync");
        }

        /// <summary>
        /// Gets DisableTombstoneCleanup Setting value if KVSReplica
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetDisableTombstoneCleanupSetting")]
        public bool GetDisableTombstoneCleanupSetting()
        {
            return MigrationUtility.ExecuteWithRetries(
                () => this.kvsActorStateProvider.GetDisableTombstoneCleanupSetting(),
                ((MigrationOrchestratorBase)this.MigrationOrchestrator).TraceId,
                $"{this.GetType().Name}.GetDisableTombstoneCleanupSetting");
        }
    }
}
