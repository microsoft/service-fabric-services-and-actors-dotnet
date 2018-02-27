// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1.Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Actors.Remoting.V1.Builder;
    using Microsoft.ServiceFabric.Services.Common;
    using Microsoft.ServiceFabric.Services.Remoting.V1;

    internal class ActorEventSubscriberManager : IServiceRemotingCallbackClient
    {
        public static readonly ActorEventSubscriberManager Singleton = new ActorEventSubscriberManager();

        private readonly ConcurrentDictionary<Subscriber, SubscriptionInfo> eventKeyToInfoMap;
        private readonly ConcurrentDictionary<Guid, SubscriptionInfo> subscriptionIdToInfoMap;
        private readonly ConcurrentDictionary<int, ActorMethodDispatcherBase> eventIdToDispatchersMap;

        private ActorEventSubscriberManager()
        {
            this.eventIdToDispatchersMap = new ConcurrentDictionary<int, ActorMethodDispatcherBase>();
            this.eventKeyToInfoMap = new ConcurrentDictionary<Subscriber, SubscriptionInfo>();
            this.subscriptionIdToInfoMap = new ConcurrentDictionary<Guid, SubscriptionInfo>();
        }

        public void RegisterEventDispatchers(IEnumerable<ActorMethodDispatcherBase> eventDispatchers)
        {
            if (eventDispatchers != null)
            {
                foreach (var dispatcher in eventDispatchers)
                {
                    this.eventIdToDispatchersMap.GetOrAdd(
                        dispatcher.InterfaceId,
                        dispatcher);
                }
            }
        }

        public Task<byte[]> RequestResponseAsync(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            throw new NotImplementedException();
        }

        public void OneWayMessage(ServiceRemotingMessageHeaders serviceMessageHeaders, byte[] requestBody)
        {
            if (!ActorMessageHeaders.TryFromServiceMessageHeaders(serviceMessageHeaders, out var actorHeaders))
            {
                return;
            }

            if ((this.eventIdToDispatchersMap == null) ||
                (!this.eventIdToDispatchersMap.TryGetValue(actorHeaders.InterfaceId, out var eventDispatcher)))
            {
                return;
            }

            if (!this.subscriptionIdToInfoMap.TryGetValue(actorHeaders.ActorId.GetGuidId(), out var info))
            {
                return;
            }

            if (info.Subscriber.EventId != actorHeaders.InterfaceId)
            {
                return;
            }

            try
            {
                var eventMsgBody = eventDispatcher.DeserializeRequestMessageBody(requestBody);
                eventDispatcher.Dispatch(info.Subscriber.Instance, actorHeaders.MethodId, eventMsgBody);
            }
            catch
            {
                // ignored
            }
        }

        public SubscriptionInfo RegisterSubscriber(ActorId actorId, Type eventInterfaceType, object instance)
        {
            var eventId = this.GetAndEnsureEventId(eventInterfaceType);

            var key = new Subscriber(actorId, eventId, instance);
            var info = this.eventKeyToInfoMap.GetOrAdd(key, k => new SubscriptionInfo(k));
            this.subscriptionIdToInfoMap.GetOrAdd(info.Id, i => info);

            return info;
        }

        public bool TryUnregisterSubscriber(ActorId actorId, Type eventInterfaceType, object instance, out SubscriptionInfo info)
        {
            var eventId = this.GetAndEnsureEventId(eventInterfaceType);

            var key = new Subscriber(actorId, eventId, instance);
            if (this.eventKeyToInfoMap.TryRemove(key, out info))
            {
                info.IsActive = false;

                this.subscriptionIdToInfoMap.TryRemove(info.Id, out var info2);
                return true;
            }

            return false;
        }

        private int GetAndEnsureEventId(Type eventInterfaceType)
        {
            if (this.eventIdToDispatchersMap != null)
            {
                var eventId = IdUtil.ComputeId(eventInterfaceType);
                if (this.eventIdToDispatchersMap.ContainsKey(eventId))
                {
                    return eventId;
                }
            }

            throw new ArgumentException();
        }
    }
}
