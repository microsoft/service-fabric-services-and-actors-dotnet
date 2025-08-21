// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Runtime;

    internal class KVSAmbiguousActorIdHandler : AmbiguousActorIdHandlerBase
    {
        private const string ActorStorageKeyPrefix = "Actor";
        private KvsActorStateProvider stateProvider;

        public KVSAmbiguousActorIdHandler(KvsActorStateProvider stateProvider)
            : base(stateProvider.GetActorStateProviderHelper())
        {
            this.stateProvider = stateProvider;
        }

        public override async Task<IAmbiguousActorIdHandler.ConditionalValue> TryResolveActorIdAsync(string stateStorageKey, CancellationToken cancellationToken)
        {
            if (!stateStorageKey.StartsWith($"{ActorStorageKeyPrefix}_"))
            {
                return new IAmbiguousActorIdHandler.ConditionalValue
                {
                    HasValue = false,
                };
            }

            var actorIdMatch = this.StripPrefixAndSuffixTokens(stateStorageKey);
            var matchList = this.GetActorIdsToResolve(actorIdMatch);
            if (matchList.Count == 1)
            {
                return new IAmbiguousActorIdHandler.ConditionalValue
                {
                    HasValue = true,
                    Value = matchList[0],
                };
            }

            return await this.TryResolveActorIdAsync(
                matchList,
                (match, _) =>
                {
                    using (var txn = this.stateProvider.GetStoreReplica().CreateTransaction())
                    {
                        var contains = this.stateProvider.GetStoreReplica().Contains(txn, $"@@_String_{match}");

                        return Task.FromResult(contains);
                    }
                },
                cancellationToken);
        }

        protected override string StripPrefixAndSuffixTokens(string stateStorageKey)
        {
            var remaining = stateStorageKey.Substring(stateStorageKey.IndexOf('_') + 1);
            return base.StripPrefixAndSuffixTokens(remaining);
        }
    }
}
