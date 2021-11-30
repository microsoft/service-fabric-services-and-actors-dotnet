// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration.Controllers
{
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
#pragma warning disable CS3009 // Base type is not CLS-compliant
    public class KvsMigrationController : ControllerBase
#pragma warning restore CS3009 // Base type is not CLS-compliant
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
        public async Task<long> GetFirstSequenceNumber()
        {
            var sequenceNumber = await this.kvsActorStateProvider.GetFirstSequeceNumberAsync(CancellationToken.None);

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
        /// Enumerates Key value store data by Sequence Number
        /// </summary>
        /// <param name="request">EnumerationRequest</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("EnumerateActorIdKindString")]
        public Task EnumerateActorIdKindString([FromBody] EnumerationRequest request)
        {
            return this.kvsActorStateProvider.EnumerateAsync(request, this.Response, "Actor_String", CancellationToken.None);
        }

        /// <summary>
        /// Sets metadata flag for KVS service to reject writes
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPut("RejectWrites")]
        public async Task RejectWritesAsync()
        {
            await this.kvsActorStateProvider.RejectWritesAsync();
        }

        /// <summary>
        /// Sets metadata flag for KVS service to resume writes
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("ResumeWrites")]
        public async Task ResumeWritesAsync()
        {
            await this.kvsActorStateProvider.ResumeWritesAsync();
        }
    }
}
