// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Actors.Remoting.Description;
    using Microsoft.ServiceFabric.Services.Remoting;
    using SR = Microsoft.ServiceFabric.Actors.SR;

    /// <summary>
    /// Contains the information about the type implementing an actor.
    /// </summary>
    public sealed class ActorTypeInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActorTypeInformation"/> class.
        /// </summary>
        public ActorTypeInformation()
        {
        }

        /// <summary>
        /// Gets the type of the class implementing the actor.
        /// </summary>
        /// <value>The <see cref="System.Type"/> of the class implementing the actor.</value>
        public Type ImplementationType { get; private set; }

        /// <summary>
        /// Gets the actor interface types which derive from <see cref="IActor"/> and implemented by actor class.
        /// </summary>
        /// <value>An enumerator that can be used to iterate through the actor interface type.</value>
        public IEnumerable<Type> InterfaceTypes { get; private set; }

        /// <summary>
        /// Gets the actor event interface which the actor class implements.
        /// </summary>
        /// <value>An enumerator that can be used to iterate through the actor event interface which the actor class implements.</value>
        public IEnumerable<Type> EventInterfaceTypes { get; private set; }

        /// <summary>
        /// Gets the service name if specified using <see cref="ActorServiceAttribute"/> for actor class.
        /// </summary>
        /// <value>The service name if specified using <see cref="ActorServiceAttribute"/> for actor class, null if attribute is not used.</value>
        public string ServiceName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the class implementing actor is abstract.
        /// </summary>
        /// <value>true if the class implementing actor is abstract, otherwise false.</value>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the actor class implements <see cref="IRemindable"/>.
        /// </summary>
        /// <value>true if the actor class implements <see cref="IRemindable"/>, otherwise false.</value>
        public bool IsRemindable { get; private set; }

        /// <summary>
        /// Gets the <see cref="Microsoft.ServiceFabric.Actors.Runtime.StatePersistence"/> enum representing type of state persistence for the actor.
        /// </summary>
        /// <value>The <see cref="Microsoft.ServiceFabric.Actors.Runtime.StatePersistence"/> representing type of state persistence for the actor.</value>
        public StatePersistence StatePersistence { get; private set; }

        internal RemotingListener RemotingListener { get; private set; }

        /// <summary>
        /// Creates the <see cref="ActorTypeInformation"/> from actorType.
        /// </summary>
        /// <param name="actorType">The type of class implementing the actor to create ActorTypeInforamtion for.</param>
        /// <param name="actorTypeInformation">When this method returns, contains ActorTypeInformation, if the creation of
        /// ActorTypeInformation from actorType succeeded, or null if the creation failed.
        /// The creation fails if the actorType parameter is null or it does not implement an actor.</param>
        /// <returns>true if ActorTypeInformation was successfully created for actorType; otherwise, false.</returns>
        /// <remarks>
        /// <para>Creation of ActorTypeInformation from actorType will fail when </para>
        /// <para>1. <see cref="System.Type.BaseType"/> for actorType is not of type <see cref="Actor"/>.</para>
        /// <para>2. actorType does not implement an interface deriving from <see cref="IActor"/> and is not marked as abstract.</para>
        /// <para>3. actorType implements more than one interface which derives from <see cref="IActor"/>
        /// but doesn't have <see cref="ActorServiceAttribute"/>.</para>
        /// </remarks>
        public static bool TryGet(Type actorType, out ActorTypeInformation actorTypeInformation)
        {
            try
            {
                actorTypeInformation = Get(actorType);
                return true;
            }
            catch (ArgumentException)
            {
                actorTypeInformation = null;
                return false;
            }
        }

        /// <summary>
        /// Creates an <see cref="ActorTypeInformation"/> from actorType.
        /// </summary>
        /// <param name="actorType">The type of class implementing the actor to create ActorTypeInforamtion for.</param>
        /// <returns><see cref="ActorTypeInformation"/> created from actorType.</returns>
        /// <exception cref="System.ArgumentException">
        /// <para>When <see cref="System.Type.BaseType"/> for actorType is not of type <see cref="Actor"/>.</para>
        /// <para>When actorType does not implement an interface deriving from <see cref="IActor"/>
        /// and is not marked as abstract.</para>
        /// <para>When actorType implements more than one interface which derives from <see cref="IActor"/>
        /// but doesn't have <see cref="ActorServiceAttribute"/>.</para>
        /// </exception>
        public static ActorTypeInformation Get(Type actorType)
        {
            if (!actorType.IsActor())
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.ErrorNotAnActor,
                        actorType.FullName,
                        typeof(Actor).FullName),
                    "actorType");
            }

            string actorServiceName = null;
            var actorServiceAttr = ActorServiceAttribute.Get(actorType);
            if (actorServiceAttr != null)
            {
                actorServiceName = actorServiceAttr.Name;
            }

            // get all actor interfaces
            var actorInterfaces = actorType.GetActorInterfaces();

            // ensure that the if the actor type is not abstract it implements at least one actor interface
            if ((actorInterfaces.Length == 0) && (!actorType.GetTypeInfo().IsAbstract))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.ErrorNoActorInterfaceFound,
                        actorType.FullName,
                        typeof(IActor).FullName),
                    "actorType");
            }

            // ensure that all actor interfaces can be remoted
            foreach (var actorInterface in actorInterfaces)
            {
                ActorInterfaceDescription.Create(actorInterface);
            }

            // if the actor implements more than one actor interfaces make sure that it has actorServiceName
            if ((actorInterfaces.Length > 1) && string.IsNullOrEmpty(actorServiceName) && (!actorType.GetTypeInfo().IsAbstract))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.ErrorNoActorServiceNameMultipleInterfaces,
                        actorType.FullName,
                        typeof(ActorServiceAttribute).FullName),
                    "actorType");
            }

            // get actor event interfaces
            var eventInterfaces = actorType.GetActorEventInterfaces();

            // ensure that all of the event interfaces can be remoted
            if (eventInterfaces != null)
            {
                foreach (var eventInterface in eventInterfaces)
                {
                    ActorEventInterfaceDescription.Create(eventInterface);
                }
            }

            var types = new List<Type> { actorType };
            types.AddRange(actorInterfaces);
#if !DotNetCoreClr
            var remotingserver = Services.Remoting.RemotingListener.V1Listener;
#else
            var remotingserver = Services.Remoting.RemotingListener.V2Listener;
#endif
            var remotingserverAttribuite = ActorRemotingProviderAttribute.GetProvider(types);
            if (remotingserverAttribuite != null)
            {
                remotingserver = remotingserverAttribuite.RemotingListener;
            }

            return new ActorTypeInformation()
            {
                InterfaceTypes = actorInterfaces,
                ImplementationType = actorType,
                ServiceName = actorServiceName,
                IsAbstract = actorType.GetTypeInfo().IsAbstract,
                IsRemindable = actorType.IsRemindableActor(),
                EventInterfaceTypes = eventInterfaces,
                StatePersistence = StatePersistenceAttribute.Get(actorType).StatePersistence,
                RemotingListener = remotingserver,
            };
        }
    }
}
