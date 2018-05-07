// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Contains extension method for Actor types.
    /// </summary>
    public static class ActorTypeExtensions
    {
        /// <summary>
        /// Gets the actor interfaces implemented by the actor class.
        /// </summary>
        /// <param name="type">The type of class implementing actor.</param>
        /// <returns>An array containing actor interface which the type implements.</returns>
        public static Type[] GetActorInterfaces(this Type type)
        {
            var list = new List<Type>(type.GetInterfaces().Where(t => typeof(IActor).IsAssignableFrom(t)));
            list.RemoveAll(t => (t.GetNonActorParentType() != null));

            return list.ToArray();
        }

        /// <summary>
        /// Gets the actor event interfaces implemented by the actor class.
        /// </summary>
        /// <param name="type">The type of class implementing actor.</param>
        /// <returns>An array containing actor event interface which the type implements.</returns>
        public static Type[] GetActorEventInterfaces(this Type type)
        {
            var eventInterfaces = new HashSet<Type>();
            var actorInterfaces = new List<Type>(GetActorInterfaces(type));
            if (type.IsActorInterface())
            {
                actorInterfaces.Add(type);
            }

            foreach (var ai in actorInterfaces)
            {
                var eventPublishers = ai.GetInterfaces().Where(
                    t => (t.GetTypeInfo().IsGenericType && (t.GetGenericTypeDefinition() == typeof(IActorEventPublisher<>))));

                foreach (var ep in eventPublishers)
                {
                    var ei = ep.GetGenericArguments()[0];
                    if (!eventInterfaces.Contains(ei))
                    {
                        eventInterfaces.Add(ei);
                    }
                }
            }

            return eventInterfaces.ToArray();
        }

        /// <summary>
        /// Indicates whether the interface type is an actor interface.
        /// </summary>
        /// <param name="actorInterfaceType">The interface type of the actor.</param>
        /// <returns>true, if the actorInterfaceType is an interface only implements <see cref="IActor"/>
        /// or (<see cref="IActor"/> and <see cref="IActorEventPublisher"/>); otherwise, false.</returns>
        public static bool IsActorInterface(this Type actorInterfaceType)
        {
            return (actorInterfaceType.GetTypeInfo().IsInterface && (actorInterfaceType.GetNonActorParentType() == null));
        }

        /// <summary>
        /// Indicates a value whether the actorType is an actor.
        /// </summary>
        /// <param name="actorType">The type implementing actor.</param>
        /// <returns>true, if the <see cref="System.Type.BaseType"/> of actorType is an <see cref="Actor"/>; otherwise, false.</returns>
        public static bool IsActor(this Type actorType)
        {
            var actorBaseType = actorType.GetTypeInfo().BaseType;

            while (actorBaseType != null)
            {
                if (actorBaseType == typeof(Actor))
                {
                    return true;
                }

                actorType = actorBaseType;
                actorBaseType = actorType.GetTypeInfo().BaseType;
            }

            return false;
        }

        /// <summary>
        /// Indicates a value whether an actor type implements <see cref="IRemindable"/> interface.
        /// </summary>
        /// <param name="actorType">The type implementing actor.</param>
        /// <returns>true, if the <paramref name="actorType"/> implements an <see cref="IRemindable"/> interface; otherwise, false.</returns>
        public static bool IsRemindableActor(this Type actorType)
        {
            return actorType.IsActor() && actorType.GetInterfaces().Contains(typeof(IRemindable));
        }

        internal static Type GetNonActorParentType(this Type type)
        {
            var list = new List<Type>(type.GetInterfaces());

            // must have IActor as the parent, so removal of it should result in reduction in the count.
            if (list.RemoveAll(t => (t == typeof(IActor))) == 0)
            {
                return type;
            }

            // remove event publisher interfaces
            list.RemoveAll(t => (t.GetTypeInfo().IsGenericType && (t.GetGenericTypeDefinition() == typeof(IActorEventPublisher<>))));
            list.RemoveAll(t => (t == typeof(IActorEventPublisher)));

            foreach (var t in list)
            {
                var nonActorParent = GetNonActorParentType(t);
                if (nonActorParent != null)
                {
                    return nonActorParent;
                }
            }

            return null;
        }
    }
}
