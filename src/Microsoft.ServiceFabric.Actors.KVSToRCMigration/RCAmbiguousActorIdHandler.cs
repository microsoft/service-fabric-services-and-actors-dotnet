// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Migration.Exceptions;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime.Migration;

    internal class RCAmbiguousActorIdHandler : AmbiguousActorIdHandlerBase
    {
        private ReliableCollectionsActorStateProvider stateProvider;
        private List<IAmbiguousActorIdResolver> resolvers;

        public RCAmbiguousActorIdHandler(ReliableCollectionsActorStateProvider stateProvider)
            : base(stateProvider.GetActorStateProviderHelper())
        {
            this.stateProvider = stateProvider;
            this.resolvers = new List<IAmbiguousActorIdResolver>();
            foreach (var type in AmbiguousActorIdResolverAttribute.GetTypesWithAttribute())
            {
                this.resolvers.Add((IAmbiguousActorIdResolver)Activator.CreateInstance(type));
            }
        }

        public override Task<IAmbiguousActorIdHandler.ConditionalValue> TryResolveActorIdAsync(string stateStorageKey, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ActorId> ResolveActorIdAsync(string stateStorageKey, Data.ITransaction tx, CancellationToken cancellationToken, bool skipPresenceDictResolve = false)
        {
            var actorIdMatch = this.StripPrefixAndSuffixTokens(stateStorageKey);
            var matchList = this.GetActorIdsToResolve(actorIdMatch);
            if (matchList.Count == 1)
            {
                var kind = GetActorIdKind(stateStorageKey);
                ActorId result;
                switch (kind)
                {
                    case ActorIdKind.Long:
                        result = new ActorId(long.Parse(matchList[0]));
                        break;
                    case ActorIdKind.Guid:
                        result = new ActorId(Guid.Parse(matchList[0]));
                        break;
                    default:
                        result = new ActorId(matchList[0]);
                        break;
                }

                return result;
            }

            if (!skipPresenceDictResolve)
            {
                var presenceDict = this.stateProvider.GetActorPresenceDictionary();
                var cv = await this.TryResolveActorIdAsync(
                   matchList,
                   async (match, token) =>
                   {
                       return await presenceDict.ContainsKeyAsync(tx, $"String_{match}_", MigrationConstants.DefaultRCTimeout, token);
                   },
                   cancellationToken);

                if (cv.HasValue)
                {
                    return new ActorId(cv.Value);
                }
            }

            var toResolve = stateStorageKey.Substring(stateStorageKey.IndexOf("_") + 1);
            foreach (var resolver in this.resolvers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (resolver.TryResolveActorIdAndStateName(toResolve, out var resolvedId))
                {
                    return new ActorId(resolvedId);
                }
            }

            throw new AmbiguousActorIdDetectedException($"Ambiguous actor id detected - <ActorId>_<StateName> : {toResolve}. Implement Microsoft.ServiceFabric.Actors.Runtime.Migration.IAmbiguousActorIdResolver to resolve ambiguity.");
        }

        private static ActorIdKind GetActorIdKind(string key)
        {
            var prefix = key.Substring(0, key.IndexOf('_'));

            if (prefix == ActorIdKind.Guid.ToString())
            {
                return ActorIdKind.Guid;
            }
            else if (prefix == ActorIdKind.Long.ToString())
            {
                return ActorIdKind.Long;
            }
            else
            {
                return ActorIdKind.String;
            }
        }
    }
}
