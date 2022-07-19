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
    using Microsoft.ServiceFabric.Actors.Runtime;

    internal abstract class AmbiguousActorIdHandlerBase : IAmbiguousActorIdHandler
    {
        private ActorStateProviderHelper stateProviderHelper;

        public AmbiguousActorIdHandlerBase(ActorStateProviderHelper stateProviderHelper)
        {
            this.stateProviderHelper = stateProviderHelper;
        }

        public abstract Task<IAmbiguousActorIdHandler.ConditionalValue> TryResolveActorIdAsync(string stateStorageKey, CancellationToken cancellationToken);

        protected List<string> GetActorIdsToResolve(string actorIdWithUnderscores)
        {
            var result = new List<string>();
            var tokens = actorIdWithUnderscores.Split('_');
            var currentSearch = string.Empty;
            foreach (var token in tokens)
            {
                currentSearch = currentSearch == string.Empty ? token : $"{currentSearch}_{token}";
                result.Add(currentSearch);
            }

            return result;
        }

        protected async Task<IAmbiguousActorIdHandler.ConditionalValue> TryResolveActorIdAsync(List<string> search, Func<string, CancellationToken, Task<bool>> resolveFunc, CancellationToken cancellationToken)
        {
            var match = string.Empty;
            foreach (var s in search)
            {
                if (await resolveFunc.Invoke(s, cancellationToken))
                {
                    if (match == string.Empty)
                    {
                        match = s;
                    }
                    else
                    {
                        return new IAmbiguousActorIdHandler.ConditionalValue
                        {
                            HasValue = false,
                        };
                    }
                }
            }

            return new IAmbiguousActorIdHandler.ConditionalValue
            {
                HasValue = match == string.Empty,
                Value = match,
            };
        }

        protected virtual string StripPrefixAndSuffixTokens(string storageKey)
        {
            if (storageKey.StartsWith($"{ActorIdKind.String.ToString()}_"))
            {
                var remaining = storageKey.Substring(storageKey.IndexOf('_') + 1);
                return remaining.Substring(0, remaining.LastIndexOf('_'));
            }
            else
            {
                var remaining = storageKey.Substring(storageKey.IndexOf('_') + 1);
                return remaining.Substring(0, remaining.IndexOf('_'));
            }
        }
    }
}
