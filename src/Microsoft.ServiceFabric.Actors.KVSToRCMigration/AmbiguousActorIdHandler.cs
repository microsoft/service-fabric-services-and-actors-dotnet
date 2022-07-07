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

    internal class AmbiguousActorIdHandler
    {
        private static readonly string TraceType = typeof(AmbiguousActorIdHandler).Name;
        private ReliableCollectionsActorStateProvider stateProvider;
        private string traceId;
        private IList<IActorIdResolver> resolvers;

        public AmbiguousActorIdHandler(ReliableCollectionsActorStateProvider stateProvider, string traceId)
        {
            this.stateProvider = stateProvider;
            this.traceId = traceId;

            this.resolvers = new List<IActorIdResolver>();
            foreach (var type in ActorIdResolverAttribute.GetTypesWithAttribute())
            {
                this.resolvers.Add((IActorIdResolver)Activator.CreateInstance(type));
            }
        }

        public async Task<ActorId> GetGetActorIdAsync(string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var presenceDict = this.stateProvider.GetActorPresenceDictionary();
            var kind = GetActorIdKind(key);
            if (kind != ActorIdKind.String)
            {
                // Remove last token
                var match = key.Substring(0, key.LastIndexOf('_'));
                //// Remove prefix
                match = match.Substring(match.IndexOf('_') + 1);
                if (kind == ActorIdKind.Long)
                {
                    return new ActorId(long.Parse(match));
                }
                else
                {
                    return new ActorId(Guid.Parse(match));
                }
            }
            else
            {
                var currentSearch = key.Substring(0, key.LastIndexOf('_'));
                string match = currentSearch;
                var count = 0;
                while (UnderscoreCount(currentSearch) > 1)
                {
                    // TODO: use retry logic
                    if (await presenceDict.ContainsKeyAsync(this.stateProvider.GetStateManager().CreateTransaction(), currentSearch, MigrationConstants.DefaultRCTimeout, cancellationToken))
                    {
                        match = currentSearch;
                        if (++count > 1)
                        {
                            var toResolve = match.Substring(match.IndexOf('_') + 1);
                            foreach (var resolver in this.resolvers)
                            {
                                if (resolver.TryResolveActorIdAndStateName(toResolve, out var resolvedId))
                                {
                                    return new ActorId(resolvedId);
                                }
                            }

                            throw new AmbiguousActorIdDetectedException($"Ambiguous actor id detected - {toResolve}. Implement Microsoft.ServiceFabric.Actors.Runtime.Migration.IActorIdResolver to resolve ambiguity.");
                        }
                    }

                    currentSearch = currentSearch.Substring(0, currentSearch.LastIndexOf('_'));
                }

                // Remove prefix
                match = match.Substring(match.IndexOf('_') + 1);
                return new ActorId(match);
            }
        }

        private static int UnderscoreCount(string key)
        {
            int count = 0;
            foreach (char c in key)
            {
                if (c == '_')
                {
                    ++count;
                }
            }

            return count;
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
