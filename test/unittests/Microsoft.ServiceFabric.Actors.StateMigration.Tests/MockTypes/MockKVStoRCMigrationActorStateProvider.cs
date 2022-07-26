// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.StateMigration.Tests.MockTypes
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data.Collections;

    internal class MockKVStoRCMigrationActorStateProvider : KVStoRCMigrationActorStateProvider
    {
        private IReliableDictionary2<string, string> metaDict;

        public MockKVStoRCMigrationActorStateProvider(IReliableCollectionsActorStateProviderInternal reliableCollectionsActorStateProvider)
            : base(reliableCollectionsActorStateProvider)
        {
            this.metaDict = new MockReliableDictionary<string, string>();
        }

        public override Task<long> SaveStateAsync(List<KVSToRCMigration.Models.KeyValuePair> kvsData, CancellationToken cancellationToken, bool skipPresenceDictResolve = false)
        {
            return Task.FromResult((long)kvsData.Count);
        }

        internal override Task ValidateDataPostMigrationAsync(List<KVSToRCMigration.Models.KeyValuePair> kvsData, string hashToCompare, bool skipPresenceDictResolve, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        internal override Task<IReliableDictionary2<string, string>> GetMetadataDictionaryAsync()
        {
            return Task.FromResult(this.metaDict);
        }
    }
}
