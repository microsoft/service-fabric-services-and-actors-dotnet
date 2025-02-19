// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
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
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    /// <summary>
    /// Provides the base implementation for the proxy to invoke methods on actor event subscribers.
    /// </summary>
    public abstract class ActorEventProxy : Microsoft.ServiceFabric.Services.Remoting.Builder.ProxyBase
    {
        private readonly ConcurrentDictionary<Guid, IActorEventSubscriberProxy> subscriberProxiesV2;
#if !DotNetCoreClr
        [Obsolete("This field is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        private readonly ConcurrentDictionary<Guid, IActorEventSubscriberProxy> subscriberProxiesV1;
        [Obsolete("This field is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        private Remoting.V1.Builder.ActorEventProxyGeneratorWith proxyGeneratorWith;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorEventProxy"/> class.
        /// </summary>
        protected ActorEventProxy()
        {
#if !DotNetCoreClr
#pragma warning disable 618
            this.subscriberProxiesV1 = new ConcurrentDictionary<Guid, IActorEventSubscriberProxy>();
#pragma warning restore 618
#endif
            this.subscriberProxiesV2 = new ConcurrentDictionary<Guid, IActorEventSubscriberProxy>();
        }

        internal void AddSubscriber(IActorEventSubscriberProxy subscriber)
        {
            if (Helper.IsEitherRemotingV2(subscriber.RemotingListener))
            {
                if (this.ServiceRemotingMessageBodyFactory == null)
                {
                    this.ServiceRemotingMessageBodyFactory = subscriber.GetRemotingMessageBodyFactory();
                }

                this.subscriberProxiesV2.AddOrUpdate(subscriber.Id, subscriber, (id, existing) => subscriber);
            }
            else
            {
#if !DotNetCoreClr
#pragma warning disable 618
                this.subscriberProxiesV1.AddOrUpdate(subscriber.Id, subscriber, (id, existing) => subscriber);
#pragma warning restore 618
#endif
            }
        }

        internal void RemoveSubscriber(Guid subscriberId)
        {
#if !DotNetCoreClr
#pragma warning disable 618
            this.subscriberProxiesV1.TryRemove(subscriberId, out var removedV1);
#pragma warning restore 618 
#endif
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

#if !DotNetCoreClr
        [Obsolete("This method is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        internal void Initialize(Remoting.V1.Builder.ActorEventProxyGeneratorWith actorEventProxyGeneratorWith)
        {
            this.proxyGeneratorWith = actorEventProxyGeneratorWith;
        }

        [Obsolete("This method is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        internal override DataContractSerializer GetRequestMessageBodySerializer(int interfaceId)
        {
            return this.proxyGeneratorWith.GetRequestMessageBodySerializer(interfaceId);
        }

        [Obsolete("This method is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        internal override DataContractSerializer GetResponseMessageBodySerializer(int interfaceId)
        {
            return this.proxyGeneratorWith.GetResponseMessageBodySerializer(interfaceId);
        }

        internal override object GetResponseMessageBodyValue(object responseMessageBody)
        {
#pragma warning disable 618
            return ((Remoting.V1.ActorMessageBody)responseMessageBody).Value;
        }
#pragma warning restore 618

        internal override object CreateRequestMessageBody(object requestMessageBodyValue)
        {
#pragma warning disable 618
            return new Remoting.V1.ActorMessageBody() { Value = requestMessageBodyValue };
#pragma warning restore 618
        }

        [Obsolete("This method is part of the deprecated V1 service remoting stack. Use InvokeAsyncImplV2() instead.")]
        internal override Task<byte[]> InvokeAsync(
            int interfaceId,
            int methodId,
            byte[] requestMsgBodyBytes,
            CancellationToken cancellationToken)
        {
            // async methods are not supported for actor event interface
            throw new NotImplementedException();
        }

        [Obsolete("This method is part of the deprecated V1 service remoting stack. Use V2 implementation instead.")]
        internal override void Invoke(
            int interfaceId,
            int methodId,
            byte[] requestMsgBodyBytes)
        {
            this.SendToSubscribers(interfaceId, methodId, requestMsgBodyBytes);
        }

#endif

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

#if !DotNetCoreClr

        private static void SendTo(
            IActorEventSubscriberProxy subscriberProxy,
            int eventInterfaceId,
            int eventMethodId,
            byte[] eventMsgBytes)
        {
            subscriberProxy.RaiseEvent(eventInterfaceId, eventMethodId, eventMsgBytes);
        }

        [Obsolete("This method is part of the deprecated V1 service remoting stack. Use V2 implementation instead.")]
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
