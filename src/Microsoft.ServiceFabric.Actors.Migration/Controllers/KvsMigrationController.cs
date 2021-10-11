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
    using Microsoft.ServiceFabric.Actors.Migration.Operations;
    using Microsoft.ServiceFabric.Actors.Runtime;

    /// <summary>
    /// Represents the controller class for KVS migration REST API.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetFirstSequenceNumber()
        {
            var operation = new GetSequenceNumberOperation(this.kvsActorStateProvider, SequenceNumberType.First, this.Request);
            var sequenceNumber = await operation.ExecuteAsync(CancellationToken.None);

            return this.Ok(sequenceNumber);
        }

        /// <summary>
        /// Gets the Last Sequence number of KVS
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetLastSequeceNumber")]
        public long GetLastSequeceNumber()
        {
            return this.kvsActorStateProvider.GetLastSequeceNumber();
        }

        /// <summary>
        /// Enumerates Key value store data by Sequence Number
        /// </summary>
        /// <param name="request">EnumerationRequest</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("EnumerateBySequenceNumber")]
        public Task EnumerateBySequenceNumber(EnumerationRequest request)
        {
            request.IncludeDeletes = false;
            return this.kvsActorStateProvider.EnumerateAsync(request);
        }

        /// <summary>
        /// Enumerates Key value store data by Sequence Number
        /// </summary>
        /// <param name="request">EnumerationRequest</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("EnumerateKeysAndTombstones")]
        public Task EnumerateKeysAndTombstones(EnumerationRequest request)
        {
            request.IncludeDeletes = true;
            return this.kvsActorStateProvider.EnumerateAsync(request);
        }

        /// <summary>
        /// Gets the Last Sequence number of KVS
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPut("TryAbortExistingTransactionsAndRejectWrites")]
        public async Task<bool> TryAbortExistingTransactionsAndRejectWrites()
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
        public async Task<bool> ResumeWrites()
        {
            await this.kvsActorStateProvider.SaveKvsRejectWriteStatusAsync(false);

            //// TODO: Restart Actor Service Replica
            return true;
        }
    }
#endif
}
