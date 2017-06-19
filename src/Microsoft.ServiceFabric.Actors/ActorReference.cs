// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// Encapsulation of a reference to an actor for serialization.
    /// </summary>
    [DataContract(Name = "ActorReference", Namespace = Constants.Namespace)]
    [Serializable]
    public sealed class ActorReference : IActorReference
    {
        /// <summary>
        /// Initializes a new instance of the ActorReference class.
        /// </summary>
        public ActorReference()
        {
        }

        /// <summary>
        /// Gets Uri of the actor service that hosts the actor in service fabric cluster.
        /// </summary>
        /// <value>Service Uri which hosts the actor in service fabric cluster.</value>
        [DataMember(Name = "ServiceUri", Order = 0, IsRequired = true)]
        public Uri ServiceUri { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ServiceFabric.Actors.ActorId"/> of the actor.
        /// </summary>
        /// <value><see cref="ServiceFabric.Actors.ActorId"/> of the actor.</value>
        [DataMember(Name = "ActorId", Order = 1, IsRequired = true)]
        public ActorId ActorId { get; set; }

        /// <summary>
        /// Gets or sets the name of the listener in the actor service to use when communicating with the actor service.
        /// </summary>
        /// <value>The name of the listener</value>
        [DataMember(Name = "ListenerName", Order = 2, IsRequired = false)]
        public string ListenerName { get; set; }

        /// <summary>
        /// Creates an <see cref="ActorProxy"/> that implements an actor interface for the actor using the 
        ///     <see cref="ActorProxyFactory.CreateActorProxy(System.Type,System.Uri, Microsoft.ServiceFabric.Actors.ActorId,System.String)"/>
        /// method.
        /// </summary>
        /// <param name="actorInterfaceType">Actor interface for the created <see cref="ActorProxy"/> to implement.</param>
        /// <returns>An actor proxy object that implements <see cref="IActorProxy"/> and TActorInterface.</returns>
        public object Bind(Type actorInterfaceType)
        {
            return ActorProxy.DefaultProxyFactory.CreateActorProxy(actorInterfaceType, this.ServiceUri, this.ActorId, this.ListenerName);
        }

        /// <summary>
        /// Gets <see cref="ActorReference"/> for the actor.
        /// </summary>
        /// <param name="actor">Actor object to get <see cref="ActorReference"/> for.</param>
        /// <returns><see cref="ActorReference"/> object for the actor.</returns>
        /// <remarks>A null value is returned if actor is passed as null.</remarks>
        public static ActorReference Get(object actor)
        {
            if (actor != null)
            {
                return GetActorReference(actor);
            }

            return null;
        }

        private static ActorReference GetActorReference(object actor)
        {
            if (actor == null)
            {
                throw new ArgumentNullException("actor");
            }

            var actorProxy = actor as IActorProxy;
            if (actorProxy != null)
            {
                return new ActorReference()
                {
                    ActorId = actorProxy.ActorId,
                    ServiceUri = actorProxy.ActorServicePartitionClient.ServiceUri,
                    ListenerName = actorProxy.ActorServicePartitionClient.ListenerName
                };
            }

            var actorBase = actor as ActorBase;
            if (actorBase != null)
            {
                return new ActorReference()
                {
                    ActorId = actorBase.Id,
                    ServiceUri = actorBase.ServiceUri,
                };
            }

            throw new ArgumentOutOfRangeException("actor");
        }
    }
}
