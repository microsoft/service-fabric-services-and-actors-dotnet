// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Builder;
    using Microsoft.ServiceFabric.Services.Common;

    internal class MockActorEventManager : IActorEventManager
    {
        private readonly IDictionary<int, Type> eventIdToEventTypeMap;

        private readonly ConcurrentDictionary<ActorId, ConcurrentDictionary<Type, ActorEventProxy>>
            actorIdToEventProxyMap;

        internal MockActorEventManager(ActorTypeInformation actorTypeInformation)
        {
            this.eventIdToEventTypeMap =
                actorTypeInformation.EventInterfaceTypes.ToDictionary(IdUtil.ComputeId, t => t);
            this.actorIdToEventProxyMap =
                new ConcurrentDictionary<ActorId, ConcurrentDictionary<Type, ActorEventProxy>>();
        }

        public Task SubscribeAsync(ActorId actorId, int eventInterfaceId, IActorEventSubscriberProxy subscriber)
        {
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
            return TaskDone.Done;
        }

        public Task ClearAllSubscriptions(ActorId actorId)
        {
            this.actorIdToEventProxyMap.TryRemove(actorId, out var eventProxyMap);

            return TaskDone.Done;
        }
    }
}
