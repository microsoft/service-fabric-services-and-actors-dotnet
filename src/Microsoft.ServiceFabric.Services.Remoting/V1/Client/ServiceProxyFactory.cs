// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Client
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Builder;

    /// <summary>
    /// Specifies the factory that creates proxies for remote communication to the specified service.
    /// </summary>
    internal class ServiceProxyFactory : IServiceProxyFactory
    {
        private readonly object thisLock;
        private readonly Func<IServiceRemotingCallbackClient, IServiceRemotingClientFactory> createServiceRemotingClientFactory;
        private volatile IServiceRemotingClientFactory remotingClientFactory;
        private readonly OperationRetrySettings retrySettings;

        /// <summary>
        /// Instantiates the ServiceProxyFactory with the specified remoting factory and retrysettings.
        /// </summary>
        /// <param name="createServiceRemotingClientFactory">
        /// Specifies the factory method that creates the remoting client factory. The remoting client factory got from this method
        /// is cached in the ServiceProxyFactory.
        /// </param>
        /// <param name="retrySettings">Specifies the retry policy to use on exceptions seen when using the proxies created by this factory</param>
        public ServiceProxyFactory(
            Func<IServiceRemotingCallbackClient, IServiceRemotingClientFactory> createServiceRemotingClientFactory,
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
            string listenerName = null) where TServiceInterface : IService
        {
            var serviceInterfaceType = typeof(TServiceInterface);
            var proxyGenerator = ServiceCodeBuilder.GetOrCreateProxyGenerator(serviceInterfaceType);
            var serviceRemotingPartitionClient = new ServiceRemotingPartitionClient(
                this.GetOrCreateServiceRemotingClientFactory(serviceInterfaceType),
                serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName,
                this.retrySettings);

            return (TServiceInterface)(object)proxyGenerator.CreateServiceProxy(serviceRemotingPartitionClient);
        }

        private IServiceRemotingClientFactory CreateServiceRemotingClientFactory(Type serviceInterfaceType)
        {
            var callbackClient = new DummyServiceRemotingCallbackClient();

            var factory = this.CreateServiceRemotingClientFactory(callbackClient);
            if (factory == null)
            {
                throw new NotSupportedException("ClientFactory cannot be null");
            }

            return factory;
        }

        /// <summary>
        /// Creates service remoting client factory.
        /// </summary>
        /// <param name="callbackClient">Callback from the remoting listener to the client.</param>
        /// <returns>Created service remoting client factory as <see cref="IServiceRemotingClientFactory"/></returns>
        protected virtual IServiceRemotingClientFactory CreateServiceRemotingClientFactory(
            IServiceRemotingCallbackClient callbackClient)
        {
            if (this.createServiceRemotingClientFactory != null)
            {
                return this.createServiceRemotingClientFactory(callbackClient);
            }

            return null;
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

        private class DummyServiceRemotingCallbackClient : IServiceRemotingCallbackClient
        {
            public Task<byte[]> RequestResponseAsync(
                ServiceRemotingMessageHeaders messageHeaders,
                byte[] requestBody)
            {
                // no op
                return Task.FromResult<byte[]>(null);
            }

            public void OneWayMessage(
                ServiceRemotingMessageHeaders messageHeaders,
                byte[] requestBody)
            {
                // no op
            }
        }
    }
}
