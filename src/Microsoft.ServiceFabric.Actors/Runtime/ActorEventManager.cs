// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Builder;
    using Microsoft.ServiceFabric.Services.Common;

    internal class ActorEventManager : IActorEventManager
    {
        private readonly IDictionary<InterfaceId, Type> eventIdToEventTypeMap;

        private readonly ConcurrentDictionary<ActorId, ConcurrentDictionary<Type, ActorEventProxy>>
            actorIdToEventProxyMap;

        internal ActorEventManager(ActorTypeInformation actorTypeInformation)
        {
            this.eventIdToEventTypeMap = actorTypeInformation.EventInterfaceTypes.ToDictionary(
                t => new InterfaceId(IdUtil.ComputeId(t), IdUtil.ComputeIdWithCRC(t)),
                t => t);

            this.actorIdToEventProxyMap =
                new ConcurrentDictionary<ActorId, ConcurrentDictionary<Type, ActorEventProxy>>();
        }

        public Task SubscribeAsync(ActorId actorId, int eventInterfaceId, IActorEventSubscriberProxy subscriber)
        {
            if (!this.eventIdToEventTypeMap.TryGetValue(new InterfaceId(eventInterfaceId, eventInterfaceId), out var eventType))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ErrorEventNotSupportedByActor,
                    eventInterfaceId, actorId));
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
            if (this.eventIdToEventTypeMap.TryGetValue(new InterfaceId(eventInterfaceId, eventInterfaceId), out var eventType))
            {
                if (this.actorIdToEventProxyMap.TryGetValue(actorId, out var eventProxyMap))
                {
                    if (eventProxyMap.TryGetValue(eventType, out var eventProxy))
                    {
                        eventProxy.RemoveSubscriber(subscriberId);
                    }
                }
            }

            return TaskDone.Done;
        }



        public Task ClearAllSubscriptions(ActorId actorId)
        {
            this.actorIdToEventProxyMap.TryRemove(actorId, out var eventProxyMap);

            return TaskDone.Done;
        }
    }

    internal class InterfaceId
    {
        public InterfaceId(int v1Id, int v2Id)
        {
            this.V2Id = v2Id;
            this.V1Id = v1Id;
        }

        public int V1Id { get; }
        public int V2Id { get; }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var interfaceOther = obj as InterfaceId;
            if (interfaceOther == null)
            {
                return false;
            }

            if (interfaceOther.V1Id != 0 && interfaceOther.V1Id == this.V1Id)
            {
                return true;
            }

            if (interfaceOther.V2Id != 0 && interfaceOther.V2Id == this.V2Id)
            {
                return true;
            }
            return false;
        }
    }


}
