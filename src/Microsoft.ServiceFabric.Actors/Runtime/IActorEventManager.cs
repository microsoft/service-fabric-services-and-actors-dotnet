// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Threading.Tasks;

    internal interface IActorEventManager
    {
        Task SubscribeAsync(ActorId actorId, int eventInterfaceId, IActorEventSubscriberProxy subscriber);

        ActorEventProxy GetActorEventProxy(ActorId actorId, Type eventType);

        Task UnsubscribeAsync(ActorId actorId, int eventInterfaceId, Guid subscriberId);

        Task ClearAllSubscriptions(ActorId actorId);
    }
}
