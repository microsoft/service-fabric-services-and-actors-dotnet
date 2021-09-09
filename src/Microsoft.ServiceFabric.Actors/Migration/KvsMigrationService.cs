// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System.Fabric;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Microsoft.ServiceFabric.Actors.Migration;

    internal class KvsMigrationService : KvsMigration.KvsMigrationBase
    {
        private KvsActorStateProvider stateProvider;

        public KvsMigrationService(KvsActorStateProvider stateProvider)
        {
            this.stateProvider = stateProvider;
        }

        public override Task<SequenceNumberResponse> GetFirstSequenceNumber(EmptyRequest request, ServerCallContext context)
        {
            return Task.FromResult(new SequenceNumberResponse
            {
                SequenceNumber = this.stateProvider.GetFirstSequeceNumberAsync(context.CancellationToken).ConfigureAwait(false).GetAwaiter().GetResult(),
            });
        }

        public override Task<SequenceNumberResponse> GetLastSequeceNumber(EmptyRequest emptyRequest, ServerCallContext context)
        {
            return Task.FromResult(new SequenceNumberResponse
            {
                SequenceNumber = this.stateProvider.GetLastSequeceNumber(),
            });
        }

        public override Task EnumerateBySequenceNumber(EnumerationRequest request, IServerStreamWriter<KeyValuePair> responseStream, ServerCallContext context)
        {
            return base.EnumerateBySequenceNumber(request, responseStream, context);
        }

        public override Task EnumerateKeysAndTombstones(EnumerationRequest request, IServerStreamWriter<KeyValuePair> responseStream, ServerCallContext context)
        {
            return base.EnumerateKeysAndTombstones(request, responseStream, context);
        }

        public override Task<TryAbortExistingTransactionsAndRejectWritesResponse> TryAbortExistingTransactionAndRejectWrites(EmptyRequest request, ServerCallContext context)
        {
            return base.TryAbortExistingTransactionAndRejectWrites(request, context);
        }

        public override Task<Migration.Status> ResumeWrites(EmptyRequest request, ServerCallContext context)
        {
            return base.ResumeWrites(request, context);
        }
    }
}
