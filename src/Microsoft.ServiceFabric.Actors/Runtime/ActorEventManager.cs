// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Common;
    using System.Globalization;

    internal class ActorEventManager : IActorEventManager
    {
        private readonly IDictionary<int, Type> eventIdToEventTypeMap;
        private readonly ConcurrentDictionary<ActorId, ConcurrentDictionary<Type, ActorEventProxy>> actorIdToEventProxyMap;

        internal ActorEventManager(ActorTypeInformation actorTypeInformation)
        {
            this.eventIdToEventTypeMap = actorTypeInformation.EventInterfaceTypes.ToDictionary(
                t => IdUtil.ComputeId(t),
                t => t);

            this.actorIdToEventProxyMap = new ConcurrentDictionary<ActorId, ConcurrentDictionary<Type, ActorEventProxy>>();
        }

        public Task SubscribeAsync(ActorId actorId, int eventInterfaceId, IActorEventSubscriberProxy subscriber)
        {
            Type eventType;
            if (!this.eventIdToEventTypeMap.TryGetValue(eventInterfaceId, out eventType))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ErrorEventNotSupportedByActor, eventInterfaceId, actorId));
            }

            var eventProxy = this.GetActorEventProxy(actorId, eventType);
            eventProxy.AddSubscriber(subscriber);

            return TaskDone.Done;
        }

        public ActorEventProxy GetActorEventProxy(ActorId actorId, Type eventType)
        {
            var eventProxyMap = this.actorIdToEventProxyMap.GetOrAdd(
                actorId,
                new ConcurrentDictionary<Type, ActorEventProxy>());

            var eventProxy = eventProxyMap.GetOrAdd(
                eventType,
                t =>
                {
                    var eventProxyGenerator = ActorCodeBuilder.GetOrCreateEventProxyGenerator(t);
                    return eventProxyGenerator.CreateActorEventProxy();
                });
            return eventProxy;
        }

        public Task UnsubscribeAsync(ActorId actorId, int eventInterfaceId, Guid subscriberId)
        {
            Type eventType;
            if (this.eventIdToEventTypeMap.TryGetValue(eventInterfaceId, out eventType))
            {
                ConcurrentDictionary<Type, ActorEventProxy> eventProxyMap;
                if (this.actorIdToEventProxyMap.TryGetValue(actorId, out eventProxyMap))
                {
                    ActorEventProxy eventProxy;
                    if (eventProxyMap.TryGetValue(eventType, out eventProxy))
                    {
                        eventProxy.RemoveSubscriber(subscriberId);
                    }
                }
            }

            return TaskDone.Done;
        }

        public Task ClearAllSubscriptions(ActorId actorId)
        {
            ConcurrentDictionary<Type, ActorEventProxy> eventProxyMap;
            this.actorIdToEventProxyMap.TryRemove(actorId, out eventProxyMap);

            return TaskDone.Done;
        }
    }
}