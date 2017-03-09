// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Client
{
    using System;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting;

    /// <summary>
    /// Defines the interface containing methods to create actor proxy factory class.
    /// </summary>
    public interface IActorProxyFactory
    {
        /// <summary>
        /// Creates a proxy to the actor object that implements an actor interface.
        /// </summary>
        /// <typeparam name="TActorInterface">
        /// The actor interface implemented by the remote actor object. 
        /// The returned proxy object will implement this interface.
        /// </typeparam>
        /// <param name="actorId">Actor Id of the proxy actor object. Methods called on this proxy will result in requests 
        /// being sent to the actor with this id.</param>
        /// <param name="applicationName">
        /// Name of the Service Fabric application that contains the actor service hosting the actor objects.
        /// This parameter can be null if the client is running as part of that same Service Fabric application. For more information, see Remarks. 
        /// </param>
        /// <param name="serviceName">
        /// Name of the Service Fabric service as configured by <see cref="ActorServiceAttribute"/> on the actor implementation.
        /// By default, the name of the service is derived from the name of the actor interface. However <see cref="ActorServiceAttribute"/>
        /// is required when an actor implements more than one actor interfaces or an actor interface derives from another actor interface as the determination of the 
        /// serviceName cannot be made automatically.
        /// </param>
        /// <param name="listenerName">
        /// By default an actor service has only one listener for clients to connect to and communicate with.
        /// However it is possible to configure an actor service with more than one listeners, the listenerName parameter specifies the name of the listener to connect to.
        /// </param>
        /// <returns>An actor proxy object that implements <see cref="IActorProxy"/> and TActorInterface.</returns>
        TActorInterface CreateActorProxy<TActorInterface>(
            ActorId actorId,
            string applicationName = null,
            string serviceName = null,
            string listenerName = null) where TActorInterface : IActor;

       

        /// <summary>
        /// Creates a proxy to the actor object that implements an actor interface.
        /// </summary>
        /// <typeparam name="TActorInterface">
        /// The actor interface implemented by the remote actor object. 
        /// The returned proxy object will implement this interface.
        /// </typeparam>
        /// <param name="serviceUri">Uri of the actor service.</param>
        /// <param name="actorId">Actor Id of the proxy actor object. Methods called on this proxy will result in requests 
        /// being sent to the actor with this id.</param>
        /// <param name="listenerName">
        /// By default an actor service has only one listener for clients to connect to and communicate with.
        /// However it is possible to configure an actor service with more than one listeners, the listenerName parameter specifies the name of the listener to connect to.
        /// </param>
        /// <returns>An actor proxy object that implements <see cref="IActorProxy"/> and TActorInterface.</returns>
        TActorInterface CreateActorProxy<TActorInterface>(
            Uri serviceUri,
            ActorId actorId,
            string listenerName = null)
            where TActorInterface : IActor;

        /// <summary>
        /// Create a proxy to the actor service that is hosting the specified actor id and implementing specified type of the service interface.
        /// </summary>
        /// <typeparam name="TServiceInterface">The service interface implemented by the actor service.</typeparam>
        /// <param name="serviceUri">Uri of the actor service to connect to.</param>
        /// <param name="actorId">Id of the actor. The created proxy will be connected to the partition of the actor service hosting actor with this id.</param>
        /// <param name="listenerName">
        /// By default an actor service has only one listener for clients to connect to and communicate with.
        /// However it is possible to configure an actor service with more than one listeners, the listenerName parameter specifies the name of the listener to connect to.
        /// </param>
        /// <returns>A service proxy object that implements <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.IServiceProxy"/> and TServiceInterface.</returns>
        TServiceInterface CreateActorServiceProxy<TServiceInterface>(
            Uri serviceUri,
            ActorId actorId,
            string listenerName = null) where TServiceInterface : IService;


        /// <summary>
        /// Create a proxy to the actor service that is hosting the specified actor id and implementing specified type of the service interface.
        /// </summary>
        /// <typeparam name="TServiceInterface">The service interface implemented by the actor service.</typeparam>
        /// <param name="serviceUri">Uri of the actor service to connect to.</param>
        /// <param name="partitionKey">The key of the actor service partition to connect to.</param>
        /// <param name="listenerName">
        /// By default an actor service has only one listener for clients to connect to and communicate with.
        /// However it is possible to configure an actor service with more than one listeners, the listenerName parameter specifies the name of the listener to connect to.
        /// </param>
        /// <returns>A service proxy object that implements <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.IServiceProxy"/> and TServiceInterface.</returns>
        TServiceInterface CreateActorServiceProxy<TServiceInterface>(
            Uri serviceUri,
            long partitionKey,
            string listenerName = null)
            where TServiceInterface : IService;
    }
}
