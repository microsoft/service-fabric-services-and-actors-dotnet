// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using Microsoft.ServiceFabric.Actors.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    internal class ActorEventSubscriberProxy : IActorEventSubscriberProxy
    {
#if !DotNetCoreClr
        private readonly ServiceFabric.Services.Remoting.V1.IServiceRemotingCallbackClient callback;
#endif
        private readonly ServiceFabric.Services.Remoting.V2.Runtime.IServiceRemotingCallbackClient callbackV2;
        private readonly Guid id;
        private readonly RemotingListener remotingListener;

#if !DotNetCoreClr
        public ActorEventSubscriberProxy(Guid id, ServiceFabric.Services.Remoting.V1.IServiceRemotingCallbackClient callback)
        {
            this.id = id;
            this.callback = callback;
            this.remotingListener = RemotingListener.V1Listener;
        }

#endif
        public ActorEventSubscriberProxy(Guid id, ServiceFabric.Services.Remoting.V2.Runtime.IServiceRemotingCallbackClient callback)
        {
            this.id = id;
            this.callbackV2 = callback;
            this.remotingListener = RemotingListener.V2Listener;
        }

        Guid IActorEventSubscriberProxy.Id
        {
            get { return this.id; }
        }

        public RemotingListener RemotingListener
        {
            get { return this.remotingListener; }
        }

#if !DotNetCoreClr
        void IActorEventSubscriberProxy.RaiseEvent(int eventInterfaceId, int eventMethodId, byte[] eventMsgBody)
        {
            this.callback.OneWayMessage(
                new Remoting.V1.ActorMessageHeaders()
                {
                    ActorId = new ActorId(this.id),
                    InterfaceId = eventInterfaceId,
                    MethodId = eventMethodId
                }.ToServiceMessageHeaders(),
                eventMsgBody);
        }
#endif

        public void RaiseEvent(int eventInterfaceId, int methodId, IServiceRemotingRequestMessageBody eventMsgBody)
        {
            var headers = new ActorRemotingMessageHeaders
            {
                ActorId = new ActorId(this.id),
                InterfaceId = eventInterfaceId,
                MethodId = methodId
            };

            this.callbackV2.SendOneWay(
                new ServiceRemotingRequestMessage(headers,
                    eventMsgBody));
        }

        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
            if (this.RemotingListener.Equals(RemotingListener.V2Listener))
            {
                return this.callbackV2.GetRemotingMessageBodyFactory();
            }
            throw new NotSupportedException("MessageFactory is not supported for V1Listener");
        }
    }
}
