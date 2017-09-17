// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Client
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains extension methods related to actor events.
    /// </summary>
    public static class ActorProxyEventExtensions
    {
        private static readonly TimeSpan DefaultResubscriptionInternal = TimeSpan.FromSeconds(20);

        /// <summary>
        /// Subscribe to a published actor event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event interface.</typeparam>
        /// <param name="actorProxy">The actor that publishes the event.</param>
        /// <param name="subscriber">The subscriber that receives the events.</param>
        /// <returns>A task that represents the asynchronous operation of subscribing to a published actor event.</returns>
        /// <exception cref="System.ArgumentException">
        /// <para>When actorProxy is not of type <see cref="ActorProxy"/></para>.
        /// </exception>
        public static Task SubscribeAsync<TEvent>(
            this IActorEventPublisher actorProxy,
            TEvent subscriber) where TEvent : IActorEvents
        {
            return SubscribeAsync(actorProxy, subscriber, DefaultResubscriptionInternal);
        }

        /// <summary>
        /// Subscribe to a published actor event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event interface.</typeparam>
        /// <param name="actorProxy">The actor that publishes the event.</param>
        /// <param name="subscriber">The subscriber that receives the events.</param>
        /// <param name="resubscriptionInterval">The time between re-subscription attempts.</param>
        /// <returns>A task that represents the asynchronous operation of subscribing to a published actor event.</returns>
        /// <exception cref="System.ArgumentException">
        /// <para>When actorProxy is not of type <see cref="ActorProxy"/></para>.
        /// </exception>
        public static Task SubscribeAsync<TEvent>(
            this IActorEventPublisher actorProxy,
            TEvent subscriber,
            TimeSpan resubscriptionInterval) where TEvent : IActorEvents
        {
            var proxy = actorProxy as ActorProxy;
            if (proxy == null)
            {
                throw new ArgumentException(SR.ActorProxyOnlyMethod, "actorProxy");
            }

            var eventInterfaceType = GetEventInterface(typeof(TEvent));
            if (eventInterfaceType == null)
            {
                throw new ArgumentException(SR.ErrorEventInterface);
            }
            return proxy.SubscribeAsync(eventInterfaceType, subscriber, resubscriptionInterval);
        }

        /// <summary>
        /// Unsubscribe from a published actor event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event interface.</typeparam>
        /// <param name="actorProxy">The actor that publishes the event.</param>
        /// <param name="subscriber">The subscriber that receives the event.</param>
        /// <returns>A task that represents the asynchronous operation of un-subscribing from a published actor event.</returns>
        /// <exception cref="System.ArgumentException">
        /// <para>When actorProxy is not of type <see cref="ActorProxy"/></para>.
        /// <para>When TEvent doesn't implement <see cref="IActorEvents"/></para>
        /// </exception>
        public static Task UnsubscribeAsync<TEvent>(this IActorEventPublisher actorProxy, TEvent subscriber)
        {
            var proxy = actorProxy as ActorProxy;
            if (proxy == null)
            {
                throw new ArgumentException(SR.ActorProxyOnlyMethod, "actorProxy");
            }
            var eventInterfaceType = GetEventInterface(typeof(TEvent));
            if (eventInterfaceType == null)
            {
                throw new ArgumentException(SR.ErrorEventInterface);
            }
            return proxy.UnsubscribeAsync(eventInterfaceType, subscriber);
        }

        private static Type GetEventInterface(Type eventHandlerType)
        {
            return IsEventInterface(eventHandlerType) ? 
                eventHandlerType : 
                eventHandlerType.GetInterfaces().FirstOrDefault(IsEventInterface);
        }

        private static bool IsEventInterface(Type userEventInterfaceType)
        {
            return ((userEventInterfaceType.GetTypeInfo().IsInterface) && 
                   (typeof(IActorEvents).IsAssignableFrom(userEventInterfaceType)));
        }
    }
}
