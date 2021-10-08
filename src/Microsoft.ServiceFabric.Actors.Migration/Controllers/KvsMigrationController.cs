// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration.Controllers
{
#if DotNetCoreClr
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Actors.Runtime;

    [Route("[controller]")]
    [ApiController]
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class KvsMigrationController : ControllerBase
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1600 // Elements should be documented
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

        // GET api/kvsmigration
        [HttpGet]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
        public ActionResult<IEnumerable<string>> Get()
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/kvsmigration/5
        [HttpGet("{id}")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
        public ActionResult<string> Get(int id)
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return $"value is {id}";
        }

        /// <summary>
        /// Gets the First Sequence number of KVS
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetFirstSequenceNumber")]
        public async Task<long> GetFirstSequenceNumber()
        {
            return await this.kvsActorStateProvider.GetFirstSequeceNumberAsync(CancellationToken.None);
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

        ///// <summary>
        ///// Enumerates Key value store data by Sequence Number
        ///// </summary>
        ///// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        ////public Task<IActionResult> EnumerateBySequenceNumber()
        ////{
        ////    request.IncludeDeletes = false;
        ////    return this.stateProvider.EnumerateAsync(request, responseStream, context.CancellationToken);
        ////}

        ////public override Task EnumerateKeysAndTombstones(EnumerationRequest request, IServerStreamWriter<KeyValuePairs> responseStream, ServerCallContext context)
        ////{
        ////    request.IncludeDeletes = true;
        ////    return this.stateProvider.EnumerateAsync(request, responseStream, context.CancellationToken);
        ////}

        ////public override async Task<RejectOrResumeWritesResponse> TryAbortExistingTransactionsAndRejectWrites(EmptyRequest request, ServerCallContext context)
        ////{
        ////    var ready = this.stateProvider.TryAbortExistingTransactionsAndRejectWrites();

        ////    await this.stateProvider.SaveKvsRejectWriteStatusAsync(ready);

        ////    return new RejectOrResumeWritesResponse()
        ////    {
        ////        Ready = ready,
        ////    };
        ////}

        ////public override async Task<RejectOrResumeWritesResponse> ResumeWrites(EmptyRequest request, ServerCallContext context)
        ////{
        ////    await this.stateProvider.SaveKvsRejectWriteStatusAsync(false);

        ////    //// TODO: Restart Actor Service Replica

        ////    return new RejectOrResumeWritesResponse()
        ////    {
        ////        Ready = true,
        ////    };
        ////}
    }
#endif
}
