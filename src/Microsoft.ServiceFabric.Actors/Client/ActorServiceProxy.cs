// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Client
{
    using System;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    /// <summary>
    /// Provides a Proxy used by clients to interact with the actor service running in a Service Fabric cluster
    /// and perform actor service level operations.
    /// </summary>
    public sealed class ActorServiceProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActorServiceProxy"/> class.
        /// </summary>
        public ActorServiceProxy()
        {
        }

        /// <summary>
        /// Creates a proxy to the actor service that is hosting the specified type of actor and implementing the specified type of the service interface.
        /// </summary>
        /// <typeparam name="TServiceInterface">The service interface implemented by the actor service.</typeparam>
        /// <param name="serviceUri">The URI of the actor service to connect to.</param>
        /// <param name="actorId">The ID of the actor. The created proxy will be connected to the partition of the actor service hosting the actor with this ID.</param>
        /// <param name="listenerName">
        /// By default, an actor service has only one listener for clients to connect to and communicate with.
        /// However, it is possible to configure an actor service with more than one listener. This parameter specifies the name of the listener to connect to.
        /// </param>
        /// <returns>A service proxy object that implements <see cref="IServiceProxy"/> and TServiceInterface.</returns>
        public static TServiceInterface Create<TServiceInterface>(
            Uri serviceUri,
            ActorId actorId,
            string listenerName = null)
            where TServiceInterface : IService
        {
            return ActorProxy.DefaultProxyFactory.CreateActorServiceProxy<TServiceInterface>(
                serviceUri,
                actorId,
                listenerName);
        }

        /// <summary>
        /// Creates a proxy to the actor service that is hosting the specified type of actor and implementing the specified type of the service interface.
        /// </summary>
        /// <typeparam name="TServiceInterface">The service interface implemented by the actor service.</typeparam>
        /// <param name="serviceUri">The URI of the actor service to connect to.</param>
        /// <param name="partitionKey">The key of the actor service partition to connect to.</param>
        /// <param name="listenerName">
        /// By default, an actor service has only one listener for clients to connect to and communicate with.
        /// However, it is possible to configure an actor service with more than one listener. This parameter specifies the name of the listener to connect to.
        /// </param>
        /// <returns>A service proxy object that implements <see cref="IServiceProxy"/> and TServiceInterface.</returns>
        public static TServiceInterface Create<TServiceInterface>(
            Uri serviceUri,
            long partitionKey,
            string listenerName = null)
            where TServiceInterface : IService
        {
            return ActorProxy.DefaultProxyFactory.CreateActorServiceProxy<TServiceInterface>(
                serviceUri,
                partitionKey,
                listenerName);
        }

        /// <summary>
        /// Creates a proxy to the actor service that is hosting the specified type of actor and implementing the specified type of the service interface.
        /// </summary>
        /// <param name="serviceUri">The URI of the actor service to connect to.</param>
        /// <param name="actorId">The ID of the actor. The created proxy will be connected to the partition of the actor service hosting the actor with this ID.</param>
        /// <param name="listenerName">
        /// By default, an actor service has only one listener for clients to connect to and communicate with.
        /// However, it is possible to configure an actor service with more than one listener. This parameter specifies the name of the listener to connect to.
        /// </param>
        /// <returns>A service proxy object that implements <see cref="IServiceProxy"/> and <see cref="IActorService"/> interfaces.</returns>
        public static IActorService Create(
            Uri serviceUri,
            ActorId actorId,
            string listenerName = null)
        {
            return ActorProxy.DefaultProxyFactory.CreateActorServiceProxy<IActorService>(
                serviceUri,
                actorId,
                listenerName);
        }

        /// <summary>
        /// Creates a proxy to the actor service that is hosting the specified type of actor and implementing the specified type of the service interface.
        /// </summary>
        /// <param name="serviceUri">The URI of the actor service to connect to.</param>
        /// <param name="partitionKey">The key of the actor service partition to connect to.</param>
        /// <param name="listenerName">
        /// By default, an actor service has only one listener for clients to connect to and communicate with.
        /// However, it is possible to configure an actor service with more than one listener. This parameter specifies the name of the listener to connect to.
        /// </param>
        /// <returns>A service proxy object that implements <see cref="IServiceProxy"/> and <see cref="IActorService"/> interfaces.</returns>
        public static IActorService Create(
            Uri serviceUri,
            long partitionKey,
            string listenerName = null)
        {
            return ActorProxy.DefaultProxyFactory.CreateActorServiceProxy<IActorService>(
                serviceUri,
                partitionKey,
                listenerName);
        }
    }
}
