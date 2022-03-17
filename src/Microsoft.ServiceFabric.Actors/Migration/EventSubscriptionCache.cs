// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Collections.Concurrent;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Builder;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    internal class EventSubscriptionCache
    {
        private static readonly string TraceType = typeof(EventSubscriptionCache).Name;
        //// TODO: Leak when client crashes or exits without unsubscribing.
        private readonly ConcurrentDictionary<Guid, IServiceRemotingCallbackClient> callbackClientMap;
        private string traceId;

        public EventSubscriptionCache(ActorService actorService, string traceId)
        {
            this.traceId = traceId;

            foreach (var eventType in actorService.ActorTypeInformation.EventInterfaceTypes)
            {
                // Required to generate interface description for IActorEvent,
                //      incase this is the first call to the event type and needs to be forwarded.
                ActorCodeBuilder.GetOrCreateEventProxyGenerator(eventType);
            }

            this.callbackClientMap = new ConcurrentDictionary<Guid, IServiceRemotingCallbackClient>();
        }

        public void AddToCache(Guid subscriptionId, IServiceRemotingCallbackClient callbackClient)
        {
            if (!this.callbackClientMap.TryAdd(subscriptionId, callbackClient))
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.traceId,
                    $"CallbackClient already found for subscription : {subscriptionId}. Ignoring the add callbackclient request.");

                return;
            }
        }

        public void RemoveFromCache(Guid subscriptionId)
        {
            if (!this.callbackClientMap.TryRemove(subscriptionId, out var callbackClient))
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.traceId,
                    $"CallbackClient not found for subscription : {subscriptionId}. Ignoring the remove callbackclient request.");
            }
        }

        public bool GetSubscription(Guid subscriptionId, out IServiceRemotingCallbackClient callbackClient)
        {
            if (!this.callbackClientMap.TryGetValue(subscriptionId, out callbackClient))
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.traceId,
                    $"CallbackClient not found for subscription : {subscriptionId}. Ignoring the actor event.");

                return false;
            }

            return true;
        }
    }
}
