// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Client
{
    using System;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting;

    /// <summary>
    /// Represents a factory class to create a proxy to the remote actor objects.
    /// </summary>
    public class ActorProxyFactory : IActorProxyFactory
    {
       #if !DotNetCoreClr
        private Remoting.V1.Client.ActorProxyFactory proxyFactoryV1;
       #endif
        private Remoting.V2.Client.ActorProxyFactory proxyFactoryV2;
        private bool overrideListenerName = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorProxyFactory"/> class.
        /// </summary>
        /// <param name="retrySettings">Retry settings for the remote object calls  made by proxy.</param>
        public ActorProxyFactory(OperationRetrySettings retrySettings = null)
        {
        }

        #if !DotNetCoreClr
        /// <summary>
        /// Initializes a new instance of the <see cref="ActorProxyFactory"/> class using V1 remoting Client Factory.
        /// </summary>
        /// <param name="createServiceRemotingClientFactory">Factory method to create remoting communication client factory.</param>
        /// <param name="retrySettings">Retry settings for the remote object calls  made by proxy.</param>
        public ActorProxyFactory(
            Func<Services.Remoting.V1.IServiceRemotingCallbackClient,
                    Services.Remoting.V1.Client.IServiceRemotingClientFactory>
                createServiceRemotingClientFactory,
            OperationRetrySettings retrySettings = null)
        {
            this.proxyFactoryV1 =
                new Remoting.V1.Client.ActorProxyFactory(createServiceRemotingClientFactory, retrySettings);
        }
        #endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorProxyFactory"/> class using V2 Remoting Client Factory.
        /// </summary>
        /// <param name="createServiceRemotingClientFactory">Factory method to create remoting communication client factory.</param>
        /// <param name="retrySettings">Retry settings for the remote object calls made by proxy.</param>
        public ActorProxyFactory(
            Func<Services.Remoting.V2.Client.IServiceRemotingCallbackMessageHandler,
                    Services.Remoting.V2.Client.IServiceRemotingClientFactory>
                createServiceRemotingClientFactory,
            OperationRetrySettings retrySettings = null)
        {
            this.proxyFactoryV2 =
                new Remoting.V2.Client.ActorProxyFactory(createServiceRemotingClientFactory, retrySettings);
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
            string listenerName = null) where TActorInterface : IActor
        {
            var actorInterfaceType = typeof(TActorInterface);

            var proxyFactory = this.GetOrSetProxyFactory(actorInterfaceType);
            return proxyFactory.CreateActorProxy<TActorInterface>(actorId,
                applicationName,
                serviceName,
                this.OverrideListenerNameIfConditionMet(listenerName));
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
        public TActorInterface CreateActorProxy<TActorInterface>(Uri serviceUri, ActorId actorId,
            string listenerName = null) where TActorInterface : IActor
        {
            var actorInterfaceType = typeof(TActorInterface);
            var proxyFactory = this.GetOrSetProxyFactory(actorInterfaceType);
            return proxyFactory.CreateActorProxy<TActorInterface>(
                serviceUri,
                actorId,
                this.OverrideListenerNameIfConditionMet(listenerName));
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
        /// <returns>A service proxy object that implements <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.IServiceProxy"/> and TServiceInterface.</returns>
        public TServiceInterface CreateActorServiceProxy<TServiceInterface>(Uri serviceUri, ActorId actorId,
            string listenerName = null) where TServiceInterface : IService
        {
            var serviceInterfaceType = typeof(TServiceInterface);
            var proxyFactory = this.GetOrSetProxyFactory(serviceInterfaceType);

            return proxyFactory.CreateActorServiceProxy<TServiceInterface>(
                serviceUri,
                actorId, this.OverrideListenerNameIfConditionMet(listenerName));
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
        /// <returns>A service proxy object that implements <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.IServiceProxy"/> and TServiceInterface.</returns>
        public TServiceInterface CreateActorServiceProxy<TServiceInterface>(Uri serviceUri, long partitionKey,
            string listenerName = null) where TServiceInterface : IService
        {
            var serviceInterfaceType = typeof(TServiceInterface);
            var proxyFactory = this.GetOrSetProxyFactory(serviceInterfaceType);
            return proxyFactory.CreateActorServiceProxy<TServiceInterface>(
                serviceUri,
                partitionKey,
                this.OverrideListenerNameIfConditionMet(listenerName));
        }

        internal object CreateActorProxy(
            Type actorInterfaceType,
            Uri serviceUri,
            ActorId actorId,
            string listenerName = null)
        {
            this.GetOrSetProxyFactory(actorInterfaceType);
            #if !DotNetCoreClr
            if (this.proxyFactoryV1 != null)
            {
                return this.proxyFactoryV1.CreateActorProxy(
                    actorInterfaceType,
                    serviceUri,
                    actorId,
                    listenerName);
            }
            #endif

            return this.proxyFactoryV2.CreateActorProxy(
                actorInterfaceType,
                serviceUri,
                actorId,
                this.OverrideListenerNameIfConditionMet(listenerName));
        }

        private ActorRemotingProviderAttribute GetProviderAttribute(Type actorInterfaceType)
        {
            return ActorRemotingProviderAttribute.GetProvider(new[] {actorInterfaceType});
        }

        private IActorProxyFactory GetOrSetProxyFactory(Type actorInterfaceType)
        {
#if !DotNetCoreClr
            //Use provider to find the stack
            if (this.proxyFactoryV1 == null && this.proxyFactoryV2 == null)
            {
                var provider = this.GetProviderAttribute(actorInterfaceType);
                if (provider.RemotingClient.Equals(RemotingClient.V2Client))
                {
                    //We are overriding listenerName since using provider service can have multiple listener configured(Compat Mode).
                    this.overrideListenerName = true;
                    this.proxyFactoryV2 =
                        new Remoting.V2.Client.ActorProxyFactory(provider.CreateServiceRemotingClientFactoryV2);
                    return this.proxyFactoryV2;
                }
                this.proxyFactoryV1 =
                    new Remoting.V1.Client.ActorProxyFactory(provider.CreateServiceRemotingClientFactory);
                return this.proxyFactoryV1;
            }

            if (this.proxyFactoryV2 != null)
            {
                return this.proxyFactoryV2;
            }
            return this.proxyFactoryV1;

            
#else
            if (this.proxyFactoryV2 == null)
            {
                var provider = this.GetProviderAttribute(actorInterfaceType);
                this.overrideListenerName = true;
                this.proxyFactoryV2 =
                    new Remoting.V2.Client.ActorProxyFactory(provider.CreateServiceRemotingClientFactoryV2);            
            }

            return this.proxyFactoryV2;
#endif

        }

        private string OverrideListenerNameIfConditionMet(string listenerName)
        {
            if (this.overrideListenerName && listenerName == null)
            {
                return ServiceRemotingProviderAttribute.DefaultV2listenerName;
            }
            return listenerName;
        }
    }
}
