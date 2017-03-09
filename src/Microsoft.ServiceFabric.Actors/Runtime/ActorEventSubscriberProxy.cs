// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting;

    internal class ActorEventSubscriberProxy : IActorEventSubscriberProxy
    {
        private readonly IServiceRemotingCallbackClient callback;
        private readonly Guid id;

        public ActorEventSubscriberProxy(Guid id, IServiceRemotingCallbackClient callback)
        {
            this.id = id;
            this.callback = callback;
        }

        Guid IActorEventSubscriberProxy.Id
        {
            get { return this.id; }
        }

        void IActorEventSubscriberProxy.RaiseEvent(int eventInterfaceId, int eventMethodId, byte[] eventMsgBody)
        {
            this.callback.OneWayMessage(
                new ActorMessageHeaders()
                {
                    ActorId = new ActorId(this.id),
                    InterfaceId = eventInterfaceId,
                    MethodId = eventMethodId
                }.ToServiceMessageHeaders(),
                eventMsgBody);
        }
    }
}