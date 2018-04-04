// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains methods for Actor Event Manager.
    /// </summary>
    internal interface IActorEventManager
    {
        /// <summary>
        /// Subscribe for actor events.
        /// </summary>
        /// <param name="actorId">Actor id to subscribe events for.</param>
        /// <param name="eventInterfaceId">Id of actor event interface to subscribe to.</param>
        /// <param name="subscriber">Proxy fro subscriber.</param>
        /// <returns>A task representing the asynchronous  operation.</returns>
        Task SubscribeAsync(ActorId actorId, int eventInterfaceId, IActorEventSubscriberProxy subscriber);

        /// <summary>
        /// Gets the proxy to invoke methods on actor event subscribers.
        /// </summary>
        /// <param name="actorId">Actor id.</param>
        /// <param name="eventType">Type of actor event.</param>
        /// <returns>Proxy to invoke methods on actor event subscribers.</returns>
        ActorEventProxy GetActorEventProxy(ActorId actorId, Type eventType);

        /// <summary>
        /// Unsubscribe from actor event.
        /// </summary>
        /// <param name="actorId">Actor id to un-subscribe events for.</param>
        /// <param name="eventInterfaceId">Id of actor event interface to un-subscribe from.</param>
        /// <param name="subscriberId">Subscriber id to unsubscribe.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UnsubscribeAsync(ActorId actorId, int eventInterfaceId, Guid subscriberId);

        /// <summary>
        /// Clears all subscriptions for the actor.
        /// </summary>
        /// <param name="actorId">Actor id to clear event subscriptions for.</param>
        /// <returns>A task representing the asynchronous operation for clearing all actor event subscription.</returns>
        Task ClearAllSubscriptions(ActorId actorId);
    }
}
