// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Collections.Concurrent;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    internal class EventSubscriptionCache
    {
        private readonly ConcurrentDictionary<ActorId, ConcurrentDictionary<int, ConcurrentDictionary<Guid, IServiceRemotingCallbackClient>>> callbackClientMap;

        public EventSubscriptionCache()
        {
            this.callbackClientMap =
                new ConcurrentDictionary<ActorId, ConcurrentDictionary<int, ConcurrentDictionary<Guid, IServiceRemotingCallbackClient>>>();
        }

        public void AddToCache(ActorId actorId, int interfaceId, Guid subscriptionId, IServiceRemotingCallbackClient callbackClient)
        {
            var interfaceMap = this.callbackClientMap.GetOrAdd(actorId, new ConcurrentDictionary<int, ConcurrentDictionary<Guid, IServiceRemotingCallbackClient>>());
            var callbackMap = interfaceMap.GetOrAdd(interfaceId, new ConcurrentDictionary<Guid, IServiceRemotingCallbackClient>());
            callbackMap.AddOrUpdate(subscriptionId, callbackClient, (_, __) => callbackClient);
        }

        public void RemoveFromCache(ActorId actorId, int interfaceId, Guid subscriptionId)
        {
            if (this.callbackClientMap.TryGetValue(actorId, out var interfaceMap))
            {
                if (interfaceMap.TryGetValue(interfaceId, out var callbackMap))
                {
                    callbackMap.TryRemove(subscriptionId, out var callbackClient);
                }
            }
        }

        public ConcurrentDictionary<Guid, IServiceRemotingCallbackClient> GetSubscriptions(ActorId actorId, int interfaceId)
        {
            if (this.callbackClientMap.TryGetValue(actorId, out var interfaceMap))
            {
                if (interfaceMap.TryGetValue(interfaceId, out var callbackMap))
                {
                    return callbackMap;
                }
            }

            return new ConcurrentDictionary<Guid, IServiceRemotingCallbackClient>();
        }
    }
}
