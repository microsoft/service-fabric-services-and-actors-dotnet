// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    ////using System.Threading.Tasks;
    ////using Grpc.Core;
    ////using Microsoft.ServiceFabric.Actors.Runtime;

    ////internal class KvsMigrationService : KvsMigration.KvsMigrationBase
    ////{
    ////    private const string TraceType = "KvsMigrationService";

    ////    private KvsActorStateProvider stateProvider;

    ////    public KvsMigrationService(KvsActorStateProvider stateProvider)
    ////    {
    ////        this.stateProvider = stateProvider;
    ////    }

    ////    public override async Task<SequenceNumberResponse> GetFirstSequenceNumber(EmptyRequest request, ServerCallContext context)
    ////    {
    ////        var result = await this.stateProvider.GetFirstSequeceNumberAsync(context.CancellationToken);
    ////        return new SequenceNumberResponse
    ////                {
    ////                    SequenceNumber = result,
    ////                };
    ////    }

    ////    public override Task<SequenceNumberResponse> GetLastSequeceNumber(EmptyRequest emptyRequest, ServerCallContext context)
    ////    {
    ////        return Task.FromResult(new SequenceNumberResponse
    ////        {
    ////            SequenceNumber = this.stateProvider.GetLastSequeceNumber(),
    ////        });
    ////    }

    ////    public override Task EnumerateBySequenceNumber(EnumerationRequest request, IServerStreamWriter<KeyValuePairs> responseStream, ServerCallContext context)
    ////    {
    ////        request.IncludeDeletes = false;
    ////        return this.stateProvider.EnumerateAsync(request, responseStream, context.CancellationToken);
    ////    }

    ////    public override Task EnumerateKeysAndTombstones(EnumerationRequest request, IServerStreamWriter<KeyValuePairs> responseStream, ServerCallContext context)
    ////    {
    ////        request.IncludeDeletes = true;
    ////        return this.stateProvider.EnumerateAsync(request, responseStream, context.CancellationToken);
    ////    }

    ////    public override async Task<RejectOrResumeWritesResponse> TryAbortExistingTransactionsAndRejectWrites(EmptyRequest request, ServerCallContext context)
    ////    {
    ////        var ready = this.stateProvider.TryAbortExistingTransactionsAndRejectWrites();

    ////        await this.stateProvider.SaveKvsRejectWriteStatusAsync(ready);

    ////        return new RejectOrResumeWritesResponse()
    ////                {
    ////                    Ready = ready,
    ////                };
    ////    }

    ////    public override async Task<RejectOrResumeWritesResponse> ResumeWrites(EmptyRequest request, ServerCallContext context)
    ////    {
    ////        await this.stateProvider.SaveKvsRejectWriteStatusAsync(false);

    ////        //// TODO: Restart Actor Service Replica

    ////        return new RejectOrResumeWritesResponse()
    ////        {
    ////            Ready = true,
    ////        };
    ////    }
    ////}
}
