// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration.Controllers
{
#if DotNetCoreClr
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Actors.Migration.Models;
    using Microsoft.ServiceFabric.Actors.Runtime;

    /// <summary>
    /// Represents the controller class for KVS migration REST API.
    /// </summary>
    [Route("[controller]")]
    public class KvsMigrationController : ControllerBase
    {
        private StatefulServiceContext serviceContext;
        private ActorTypeInformation actorTypeInformation;
        private KvsActorStateProvider kvsActorStateProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="KvsMigrationController"/> class.
        /// </summary>
        /// <param name="context">StatefulServiceContext</param>
        /// <param name="actorTypeInfo">ActorTypeInformation</param>
        /// <param name="stateProvider">KvsActorStateProvider</param>
        public KvsMigrationController(StatefulServiceContext context, ActorTypeInformation actorTypeInfo, KvsActorStateProvider stateProvider)
        {
            this.serviceContext = context;
            this.actorTypeInformation = actorTypeInfo;
            this.kvsActorStateProvider = stateProvider;
        }

        /// <summary>
        /// Gets the First Sequence number of KVS
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetFirstSequenceNumber")]
        public async Task<ActionResult<long>> GetFirstSequenceNumber()
        {
            var sequenceNumber = await this.kvsActorStateProvider.GetFirstSequeceNumberAsync(CancellationToken.None);

            return this.Ok(sequenceNumber);
        }

        /// <summary>
        /// Gets the Last Sequence number of KVS
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetLastSequenceNumber")]
        public ActionResult<long> GetLastSequenceNumber()
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
            request.IncludeDeletes = false;
            return this.kvsActorStateProvider.EnumerateAsync(request, this.Response, CancellationToken.None);
        }

        /// <summary>
        /// Enumerates Key value store data by Sequence Number
        /// </summary>
        /// <param name="request">EnumerationRequest</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("EnumerateKeysAndTombstones")]
        public Task EnumerateKeysAndTombstones([FromBody] EnumerationRequest request)
        {
            request.IncludeDeletes = true;
            return this.kvsActorStateProvider.EnumerateAsync(request, this.Response, CancellationToken.None);
        }

        /// <summary>
        /// Gets the Last Sequence number of KVS
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPut("TryAbortExistingTransactionsAndRejectWrites")]
        public async Task<ActionResult<bool>> TryAbortExistingTransactionsAndRejectWrites()
        {
            var ready = this.kvsActorStateProvider.TryAbortExistingTransactionsAndRejectWrites();

            await this.kvsActorStateProvider.SaveKvsRejectWriteStatusAsync(ready);

            return ready;
        }

        /// <summary>
        /// Enumerates Key value store data by Sequence Number
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("ResumeWrites")]
        public async Task<ActionResult<bool>> ResumeWrites()
        {
            await this.kvsActorStateProvider.SaveKvsRejectWriteStatusAsync(false);

            //// TOOD: Restart Primary REplica
            await new FabricClient().ServiceManager.RestartReplicaAsync(this.serviceContext.NodeContext.NodeName, this.serviceContext.PartitionId, this.serviceContext.ReplicaId);

            return true;
        }
    }
#endif
}
