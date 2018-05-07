// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Client
{
    using System;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Builder;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

    /// <summary>
    /// Factory class to create a proxy to the remote actor objects.
    /// </summary>
    internal class ActorProxyFactory : IActorProxyFactory
    {
        private readonly object thisLock;
        private readonly OperationRetrySettings retrySettings;
        private readonly Func<IServiceRemotingCallbackMessageHandler, IServiceRemotingClientFactory>
            createServiceRemotingClientFactory;

        private volatile IServiceRemotingClientFactory remotingClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorProxyFactory"/> class.
        /// </summary>
        /// <param name="createServiceRemotingClientFactory">Factory method to create remoting communication client factory.</param>
        /// <param name="retrySettings">Retry settings for the remote object calls  made by proxy.</param>
        public ActorProxyFactory(
            Func<IServiceRemotingCallbackMessageHandler, IServiceRemotingClientFactory>
                createServiceRemotingClientFactory = null,
            OperationRetrySettings retrySettings = null)
        {
            this.thisLock = new object();
            this.remotingClientFactory = null;
            this.createServiceRemotingClientFactory = createServiceRemotingClientFactory;
            this.retrySettings = retrySettings;
        }

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
        /// Name of the Service Fabric service as configured by <see cref="Microsoft.ServiceFabric.Actors.Runtime.ActorServiceAttribute"/> on the actor implementation.
        /// By default, the name of the service is derived from the name of the actor interface. However <see cref="Microsoft.ServiceFabric.Actors.Runtime.ActorServiceAttribute"/>
        /// is required when an actor implements more than one actor interfaces or an actor interface derives from another actor interface as the determination of the
        /// serviceName cannot be made automatically.
        /// </param>
        /// <param name="listenerName">
        /// By default an actor service has only one listener for clients to connect to and communicate with.
        /// However it is possible to configure an actor service with more than one listeners, the listenerName parameter specifies the name of the listener to connect to.
        /// </param>
        /// <returns>An actor proxy object that implements <see cref="IActorProxy"/> and TActorInterface.</returns>
        public TActorInterface CreateActorProxy<TActorInterface>(
            ActorId actorId,
            string applicationName = null,
            string serviceName = null,
            string listenerName = null)
            where TActorInterface : IActor
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                applicationName = ActorNameFormat.GetCurrentFabricApplicationName();
            }

            var actorInterfaceType = typeof(TActorInterface);
            var serviceUri = ActorNameFormat.GetFabricServiceUri(actorInterfaceType, applicationName, serviceName);
            return this.CreateActorProxy<TActorInterface>(serviceUri, actorId, listenerName);
        }

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
        public TActorInterface CreateActorProxy<TActorInterface>(
            Uri serviceUri,
            ActorId actorId,
            string listenerName = null)
            where TActorInterface : IActor
        {
            var actorInterfaceType = typeof(TActorInterface);

            var factory = this.GetOrCreateServiceRemotingClientFactory(actorInterfaceType);

            var proxyGenerator = ActorCodeBuilder.GetOrCreateProxyGenerator(actorInterfaceType);
            var actorServicePartitionClient = new ActorServicePartitionClient(
                factory,
                serviceUri,
                actorId,
                listenerName,
                this.retrySettings);
            return (TActorInterface)(object)proxyGenerator.CreateActorProxy(
                actorServicePartitionClient,
                factory.GetRemotingMessageBodyFactory());
        }

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
        /// <returns>A service proxy object that implements <see cref="IServiceProxy"/> and TServiceInterface.</returns>
        public TServiceInterface CreateActorServiceProxy<TServiceInterface>(
            Uri serviceUri,
            ActorId actorId,
            string listenerName = null)
            where TServiceInterface : IService
        {
            return this.CreateActorServiceProxy<TServiceInterface>(
                serviceUri,
                actorId.GetPartitionKey(),
                listenerName);
        }

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
        /// <returns>A service proxy object that implements <see cref="IServiceProxy"/> and TServiceInterface.</returns>
        public TServiceInterface CreateActorServiceProxy<TServiceInterface>(
            Uri serviceUri,
            long partitionKey,
            string listenerName = null)
            where TServiceInterface : IService
        {
            var serviceInterfaceType = typeof(TServiceInterface);
            var factory = this.GetOrCreateServiceRemotingClientFactory(serviceInterfaceType);

            var proxyGenerator = ServiceCodeBuilder.GetOrCreateProxyGenerator(serviceInterfaceType);
            var serviceRemotingPartitionClient = new ServiceRemotingPartitionClient(
                factory,
                serviceUri,
                new ServicePartitionKey(partitionKey),
                TargetReplicaSelector.Default,
                listenerName,
                this.retrySettings);

            return (TServiceInterface)(object)proxyGenerator.CreateServiceProxy(
                serviceRemotingPartitionClient,
                factory.GetRemotingMessageBodyFactory());
        }

        internal object CreateActorProxy(
            Type actorInterfaceType,
            Uri serviceUri,
            ActorId actorId,
            string listenerName = null)
        {
            var proxyGenerator = ActorCodeBuilder.GetOrCreateProxyGenerator(actorInterfaceType);
            var factory = this.GetOrCreateServiceRemotingClientFactory(actorInterfaceType);
            var actorServicePartitionClient = new ActorServicePartitionClient(
                factory,
                serviceUri,
                actorId,
                listenerName,
                this.retrySettings);

            return proxyGenerator.CreateActorProxy(
                actorServicePartitionClient,
                factory.GetRemotingMessageBodyFactory());
        }

        private IServiceRemotingClientFactory GetOrCreateServiceRemotingClientFactory(Type actorInterfaceType)
        {
            if (this.remotingClientFactory != null)
            {
                return this.remotingClientFactory;
            }

            lock (this.thisLock)
            {
                if (this.remotingClientFactory == null)
                {
                    this.remotingClientFactory = this.CreateServiceRemotingClientFactory(actorInterfaceType);
                }
            }

            return this.remotingClientFactory;
        }

        private IServiceRemotingClientFactory CreateServiceRemotingClientFactory(Type actorInterfaceType)
        {
            var factory = this.createServiceRemotingClientFactory(ActorEventSubscriberManager.Instance);
            if (factory == null)
            {
                throw new NotSupportedException("ClientFactory can't be null");
            }

            return factory;
        }
    }
}
