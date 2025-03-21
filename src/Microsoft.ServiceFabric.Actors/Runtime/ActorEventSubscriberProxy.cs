// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Actors.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    internal class ActorEventSubscriberProxy : IActorEventSubscriberProxy
    {
        private readonly IServiceRemotingCallbackClient callback;
        private readonly Guid id;
        private readonly RemotingListenerVersion remotingListener;

        public ActorEventSubscriberProxy(Guid id, IServiceRemotingCallbackClient callback)
        {
            this.id = id;
            this.callback = callback;
            this.remotingListener = RemotingListenerVersion.V2;
        }

        Guid IActorEventSubscriberProxy.Id
        {
            get { return this.id; }
        }

        public RemotingListenerVersion RemotingListener
        {
            get { return this.remotingListener; }
        }

        public void RaiseEvent(int eventInterfaceId, int methodId, IServiceRemotingRequestMessageBody eventMsgBody)
        {
            var headers = new ActorRemotingMessageHeaders
            {
                ActorId = new ActorId(this.id),
                InterfaceId = eventInterfaceId,
                MethodId = methodId,
            };

            this.callback.SendOneWay(
                new ServiceRemotingRequestMessage(
                    headers,
                    eventMsgBody));
        }

        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
            return this.callback.GetRemotingMessageBodyFactory();
        }
    }
}
