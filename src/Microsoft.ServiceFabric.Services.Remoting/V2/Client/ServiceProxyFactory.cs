// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Client
{
    using System;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Builder;

    /// <summary>
    /// Specifies the factory that creates proxies for remote communication to the specified service.
    /// </summary>
    internal class ServiceProxyFactory : IServiceProxyFactory
    {
        private readonly object thisLock;

        private readonly Func<IServiceRemotingCallbackMessageHandler, IServiceRemotingClientFactory> createServiceRemotingClientFactory;

        private readonly OperationRetrySettings retrySettings;
        private volatile IServiceRemotingClientFactory remotingClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProxyFactory"/> class with the
        /// specified remoting factory and retrysettings.
        /// </summary>
        /// <param name="createServiceRemotingClientFactory">
        /// Specifies the factory method that creates the remoting client factory
        /// The remoting client factory got from this method is cached in the ServiceProxyFactory.
        /// </param>
        /// <param name="retrySettings">
        /// Specifies the retry policy to use on exceptions seen when using the proxies
        /// created by this factory</param>
        public ServiceProxyFactory(
            Func<IServiceRemotingCallbackMessageHandler, IServiceRemotingClientFactory> createServiceRemotingClientFactory,
            OperationRetrySettings retrySettings = null)
        {
            this.thisLock = new object();
            this.remotingClientFactory = null;
            this.createServiceRemotingClientFactory = createServiceRemotingClientFactory;
            this.retrySettings = retrySettings;
        }

        /// <summary>
        /// Creates a proxy to communicate to the specified service using the remoted interface TServiceInterface that
        /// the service implements.
        /// <typeparam name="TServiceInterface">Interface that is being remoted</typeparam>
        /// <param name="serviceUri">Uri of the Service.</param>
        /// <param name="partitionKey">The Partition key that determines which service partition is responsible for handling requests from this service proxy</param>
        /// <param name="targetReplicaSelector">Determines which replica or instance of the service partition the client should connect to.</param>
        /// <param name="listenerName">This parameter is Optional if the service has a single communication listener. The endpoints from the service
        /// are of the form {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}. When the service exposes multiple endpoints, this parameter
        /// identifies which of those endpoints to use for the remoting communication.
        /// </param>
        /// <returns>The proxy that implement the interface that is being remoted. The returned object also implement <see cref="IServiceProxy"/> interface.</returns>
        /// </summary>
        public TServiceInterface CreateServiceProxy<TServiceInterface>(
            Uri serviceUri,
            ServicePartitionKey partitionKey = null,
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default,
            string listenerName = null)
            where TServiceInterface : IService
        {
            var serviceInterfaceType = typeof(TServiceInterface);
            var proxyGenerator = ServiceCodeBuilder.GetOrCreateProxyGenerator(serviceInterfaceType);
            return this.CreateServiceProxy<TServiceInterface>(serviceUri, partitionKey, targetReplicaSelector, listenerName, serviceInterfaceType, proxyGenerator);
        }

        /// <summary>
        /// Creates a proxy to communicate to the specified service using the remoted interface TServiceInterface that
        /// the service implements.
        /// <typeparam name="TServiceInterface">Interface that is being remoted</typeparam>
        /// <param name="serviceUri">Uri of the Service.</param>
        /// <param name="partitionKey">The Partition key that determines which service partition is responsible for handling requests from this service proxy</param>
        /// <param name="targetReplicaSelector">Determines which replica or instance of the service partition the client should connect to.</param>
        /// <param name="listenerName">This parameter is Optional if the service has a single communication listener. The endpoints from the service
        /// are of the form {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}. When the service exposes multiple endpoints, this parameter
        /// identifies which of those endpoints to use for the remoting communication.
        /// </param>
        /// <returns>The proxy that implement the interface that is being remoted. The returned object also implement <see cref="IServiceProxy"/> interface.</returns>
        /// </summary>
        public TServiceInterface CreateNonIServiceProxy<TServiceInterface>(
            Uri serviceUri,
            ServicePartitionKey partitionKey = null,
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default,
            string listenerName = null)
        {
            var serviceInterfaceType = typeof(TServiceInterface);
            var proxyGenerator = ServiceCodeBuilder.GetOrCreateProxyGeneratorForNonServiceInterface(serviceInterfaceType);
            return this.CreateServiceProxy<TServiceInterface>(serviceUri, partitionKey, targetReplicaSelector, listenerName, serviceInterfaceType, proxyGenerator);
        }

        /// <summary>
        /// Creates service remoting client factory.
        /// </summary>
        /// <param name="callbackClient">Callback from the remoting listener to the client.</param>
        /// <returns>Created service remoting client factory as <see cref="IServiceRemotingClientFactory"/></returns>
        protected virtual IServiceRemotingClientFactory CreateServiceRemotingClientFactory(
            IServiceRemotingCallbackMessageHandler callbackClient)
        {
            if (this.createServiceRemotingClientFactory != null)
            {
                return this.createServiceRemotingClientFactory(callbackClient);
            }

            return null;
        }

        private TServiceInterface CreateServiceProxy<TServiceInterface>(
            Uri serviceUri,
            ServicePartitionKey partitionKey,
            TargetReplicaSelector targetReplicaSelector,
            string listenerName,
            Type serviceInterfaceType,
            ServiceProxyGenerator proxyGenerator)
        {
            var clientFactory = this.GetOrCreateServiceRemotingClientFactory(serviceInterfaceType);
            var serviceRemotingPartitionClient = new ServiceRemotingPartitionClient(
                clientFactory,
                serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName,
                this.retrySettings);

            return (TServiceInterface)(object)proxyGenerator.CreateServiceProxy(
                serviceRemotingPartitionClient,
                clientFactory.GetRemotingMessageBodyFactory());
        }

        private IServiceRemotingClientFactory CreateServiceRemotingClientFactory(Type serviceInterfaceType)
        {
            var callbackClient = new DummyServiceRemotingCallbackClient();

            var factory = this.CreateServiceRemotingClientFactory(callbackClient);
            if (factory == null)
            {
                throw new NotSupportedException("ClientFactory can't be null");
            }

            return factory;
        }

        private IServiceRemotingClientFactory GetOrCreateServiceRemotingClientFactory(Type serviceInterfaceType)
        {
            if (this.remotingClientFactory != null)
            {
                return this.remotingClientFactory;
            }

            lock (this.thisLock)
            {
                if (this.remotingClientFactory == null)
                {
                    this.remotingClientFactory = this.CreateServiceRemotingClientFactory(serviceInterfaceType);
                }
            }

            return this.remotingClientFactory;
        }

        private class DummyServiceRemotingCallbackClient : IServiceRemotingCallbackMessageHandler
        {
            public void HandleOneWayMessage(IServiceRemotingRequestMessage requestMessage)
            {
                // no-op
            }
        }
    }
}
