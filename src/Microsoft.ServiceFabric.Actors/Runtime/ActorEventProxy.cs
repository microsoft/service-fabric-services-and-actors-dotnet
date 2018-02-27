// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    /// <summary>
    /// Provides the base implementation for the proxy to invoke methods on actor event subscribers.
    /// </summary>
    public abstract class ActorEventProxy : ProxyBase
    {
#if !DotNetCoreClr
        private Remoting.V1.Builder.ActorEventProxyGeneratorWith proxyGeneratorWith;
        private readonly ConcurrentDictionary<Guid, IActorEventSubscriberProxy> subscriberProxiesV1;
#endif
        private readonly ConcurrentDictionary<Guid, IActorEventSubscriberProxy> subscriberProxiesV2;


        /// <summary>
        /// Initializes a new instance of the <see cref="ActorEventProxy"/> class.
        /// </summary>
        protected ActorEventProxy()
        {
#if !DotNetCoreClr
            this.subscriberProxiesV1 = new ConcurrentDictionary<Guid, IActorEventSubscriberProxy>();
#endif
            this.subscriberProxiesV2 = new ConcurrentDictionary<Guid, IActorEventSubscriberProxy>();
        }

        /// <inheritdoc />
        protected override IServiceRemotingRequestMessageBody CreateRequestMessageBodyV2(string interfaceName, string methodName,
            int parameterCount)
        {
            //This cna happen in case someone trries to raiseEvent but no subscribers registered.
            if (this.serviceRemotingMessageBodyFactory == null)
            {
                return new DummyServiceRemoingRequestMessageBody();
            }
            return this.serviceRemotingMessageBodyFactory.CreateRequest(interfaceName, methodName, parameterCount);
        }

        internal void AddSubscriber(IActorEventSubscriberProxy subscriber)
        {
            if (subscriber.RemotingListener.Equals(RemotingListener.V2Listener))
            {
                if (this.serviceRemotingMessageBodyFactory == null)
                {
                    this.serviceRemotingMessageBodyFactory = subscriber.GetRemotingMessageBodyFactory();
                }

                this.subscriberProxiesV2.AddOrUpdate(subscriber.Id, subscriber, (id, existing) => subscriber);
            }
            else
            {
#if !DotNetCoreClr
                this.subscriberProxiesV1.AddOrUpdate(subscriber.Id, subscriber, (id, existing) => subscriber);
#endif
            }
        }

        internal void RemoveSubscriber(Guid subscriberId)
        {
#if !DotNetCoreClr
            this.subscriberProxiesV1.TryRemove(subscriberId, out var removedV1);
#endif
            this.subscriberProxiesV2.TryRemove(subscriberId, out var removedV2);
        }

#if !DotNetCoreClr
        internal void Initialize(Remoting.V1.Builder.ActorEventProxyGeneratorWith actorEventProxyGeneratorWith)
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
            return ((Remoting.V1.ActorMessageBody)responseMessageBody).Value;
        }

        internal override object CreateRequestMessageBody(object requestMessageBodyValue)
        {
            return new Remoting.V1.ActorMessageBody() { Value = requestMessageBodyValue };
        }

        internal override Task<byte[]> InvokeAsync(
            int interfaceId,
            int methodId,
            byte[] requestMsgBodyBytes,
            CancellationToken cancellationToken)
        {
            // async methods are not supported for actor event interface
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
            foreach (var subscriber in this.subscriberProxiesV1)
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
                    this.subscriberProxiesV1.TryRemove(subscriberKey, out var eventProxy);
                }
            }
        }
#endif

        //V2 Stack Api

        internal override Task<IServiceRemotingResponseMessage> InvokeAsyncImplV2(
            int interfaceId,
            int methodId,
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

        private static void SendTo(
            IActorEventSubscriberProxy subscriberProxy, int eventInterfaceId, int eventMethodId,
            IServiceRemotingRequestMessageBody messageBody)
        {
            subscriberProxy.RaiseEvent(eventInterfaceId, eventMethodId, messageBody);
        }

#if !DotNetCoreClr

        private static void SendTo(
            IActorEventSubscriberProxy subscriberProxy, int eventInterfaceId, int eventMethodId,
            byte[] eventMsgBytes)
        {
            subscriberProxy.RaiseEvent(eventInterfaceId, eventMethodId, eventMsgBytes);
        }
#endif
    }
    internal class DummyServiceRemoingRequestMessageBody : IServiceRemotingRequestMessageBody
    {
        public void SetParameter(int position, string parameName, object parameter)
        {
            //no-op
        }

        public object GetParameter(int position, string parameName, Type paramType)
        {
            throw new NotImplementedException();
        }
    }
}
