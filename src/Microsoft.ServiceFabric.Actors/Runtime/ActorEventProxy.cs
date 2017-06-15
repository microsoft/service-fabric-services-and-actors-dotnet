// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Actors.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;

    /// <summary>
    /// Provides the base implementation for the proxy to invoke methods on actor event subscribers.
    /// </summary>
    public abstract class ActorEventProxy : ProxyBase
    {
        private ActorEventProxyGeneratorWith proxyGeneratorWith;
        private readonly ConcurrentDictionary<Guid, IActorEventSubscriberProxy> subscriberProxies;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorEventProxy"/> class.
        /// </summary>
        protected ActorEventProxy()
        {
            this.subscriberProxies = new ConcurrentDictionary<Guid, IActorEventSubscriberProxy>();
        }

        internal void Initialize(ActorEventProxyGeneratorWith actorEventProxyGeneratorWith)
        {
            this.proxyGeneratorWith = actorEventProxyGeneratorWith;
        }

        internal override DataContractSerializer GetRequestMessageBodySerializer(int interfaceId)
        {
            return this.proxyGeneratorWith.GetRequestMessageBodySerializer(interfaceId);
        }

        internal override DataContractSerializer GetResponseMessageBodySerializer(int interfaceId)
        {
            return this.proxyGeneratorWith.GetResponseMessageBodySerializer(interfaceId);
        }

        internal override object GetResponseMessageBodyValue(object responseMessageBody)
        {
            return ((ActorMessageBody)responseMessageBody).Value;
        }

        internal override object CreateRequestMessageBody(object requestMessageBodyValue)
        {
            return new ActorMessageBody() { Value = requestMessageBodyValue };
        }

        internal void AddSubscriber(IActorEventSubscriberProxy subscriber)
        {
            this.subscriberProxies.AddOrUpdate(subscriber.Id, subscriber, (id, existing) => subscriber);
        }

        internal void RemoveSubscriber(Guid subscriberId)
        {
            IActorEventSubscriberProxy removed;
            this.subscriberProxies.TryRemove(subscriberId, out removed);
        }

        internal override Task<byte[]> InvokeAsync(
            int interfaceId,
            int methodId,
            byte[] requestMsgBodyBytes,
            CancellationToken cancellationToken)
        {
            // Async methods are not supported for actor event interface.
            throw new NotImplementedException();
        }

        internal override void Invoke(
            int interfaceId,
            int methodId,
            byte[] requestMsgBodyBytes)
        {
            this.SendToSubscribers(interfaceId, methodId, requestMsgBodyBytes);
        }

        private void SendToSubscribers(int eventInterfaceId, int eventMethodId, byte[] eventMsgBytes)
        {
            IList<Guid> subscribersToRemove = null;
            foreach (var subscriber in this.subscriberProxies)
            {
                try
                {
                    SendTo(subscriber.Value, eventInterfaceId, eventMethodId, eventMsgBytes);
                }
                catch (Exception e)
                {
                    ActorTrace.Source.WriteWarning(
                        "ActorEventProxy.SendToSubscribers",
                        "Error while Sending Message To Subscribers : {0}",
                        e);

                    if (subscribersToRemove == null)
                    {
                        subscribersToRemove = new List<Guid> { subscriber.Key };
                    }
                    else
                    {
                        subscribersToRemove.Add(subscriber.Key);
                    }
                }
            }

            if (subscribersToRemove != null)
            {
                foreach (var subscriberKey in subscribersToRemove)
                {
                    IActorEventSubscriberProxy eventProxy;
                    this.subscriberProxies.TryRemove(subscriberKey, out eventProxy);
                }
            }
        }

        private static void SendTo(
            IActorEventSubscriberProxy subscriberProxy, int eventInterfaceId, int eventMethodId,
            byte[] eventMsgBytes)
        {
            subscriberProxy.RaiseEvent(eventInterfaceId, eventMethodId, eventMsgBytes);
        }
    }
}
