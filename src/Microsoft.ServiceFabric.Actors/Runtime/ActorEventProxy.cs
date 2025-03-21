// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.Builder;
using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    /// <summary>
    /// Provides the base implementation for the proxy to invoke methods on actor event subscribers.
    /// </summary>
    public abstract class ActorEventProxy : ProxyBase
    {
        private readonly ConcurrentDictionary<Guid, IActorEventSubscriberProxy> subscriberProxiesV2;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorEventProxy"/> class.
        /// </summary>
        protected ActorEventProxy()
        {
            this.subscriberProxiesV2 = new ConcurrentDictionary<Guid, IActorEventSubscriberProxy>();
        }

        internal void AddSubscriber(IActorEventSubscriberProxy subscriber)
        {
            if (this.ServiceRemotingMessageBodyFactory == null)
            {
                this.ServiceRemotingMessageBodyFactory = subscriber.GetRemotingMessageBodyFactory();
            }

            this.subscriberProxiesV2.AddOrUpdate(subscriber.Id, subscriber, (id, existing) => subscriber);
        }

        internal void RemoveSubscriber(Guid subscriberId)
        {
            this.subscriberProxiesV2.TryRemove(subscriberId, out var removedV2);
        }

        // V2 Stack Api
        internal override Task<IServiceRemotingResponseMessage> InvokeAsyncImplV2(
            int interfaceId,
            int methodId,
            string methodName,
            IServiceRemotingRequestMessageBody requestMsgBodyValue,
            CancellationToken cancellationToken)
        {
            // async methods are not supported for actor event interface
            throw new NotImplementedException();
        }

        internal override void InvokeImplV2(
            int interfaceId,
            int methodId,
            IServiceRemotingRequestMessageBody requestMsgBodyValue)
        {
            this.SendToSubscribers(interfaceId, methodId, requestMsgBodyValue);
        }

        /// <inheritdoc />
        protected override IServiceRemotingRequestMessageBody CreateRequestMessageBodyV2(
            string interfaceName,
            string methodName,
            int parameterCount,
            object wrappedRequest)
        {
            // This cna happen in case someone trries to raiseEvent but no subscribers registered.
            if (this.ServiceRemotingMessageBodyFactory == null)
            {
                return new DummyServiceRemoingRequestMessageBody();
            }

            return this.ServiceRemotingMessageBodyFactory.CreateRequest(interfaceName, methodName, parameterCount, wrappedRequest);
        }

        private static void SendTo(
            IActorEventSubscriberProxy subscriberProxy,
            int eventInterfaceId,
            int eventMethodId,
            IServiceRemotingRequestMessageBody messageBody)
        {
            subscriberProxy.RaiseEvent(eventInterfaceId, eventMethodId, messageBody);
        }

        private void SendToSubscribers(int eventInterfaceId, int eventMethodId, IServiceRemotingRequestMessageBody messageBody)
        {
            IList<Guid> subscribersToRemove = null;
            foreach (var subscriber in this.subscriberProxiesV2)
            {
                try
                {
                    SendTo(subscriber.Value, eventInterfaceId, eventMethodId, messageBody);
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
                    this.subscriberProxiesV2.TryRemove(subscriberKey, out var eventProxy);
                }
            }
        }
    }
}
