// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Client
{
    using System;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

    /// <summary>
    /// Specifies the factory that creates proxies for remote communication to the specified service.
    /// </summary>
    public class ServiceProxyFactory : IServiceProxyFactory
    {
        private readonly OperationRetrySettings retrySettings;

#if !DotNetCoreClr
        private Remoting.V1.Client.ServiceProxyFactory proxyFactoryV1;
#endif
        private Remoting.V2.Client.ServiceProxyFactory proxyFactoryV2;
        private bool overrideListenerName = false;

        /// <summary>
        /// Instantiates the ServiceProxyFactory with the specified retrysettings and default remotingClientFactory
        /// </summary>
        public ServiceProxyFactory(OperationRetrySettings retrySettings = null)
        {
            this.retrySettings = retrySettings;

#if !DotNetCoreClr
            this.proxyFactoryV1 = null;
#endif
            this.proxyFactoryV2 = null;
        }


#if !DotNetCoreClr

        /// <summary>
        /// Instantiates the ServiceProxyFactory with the specified V1 remoting factory and retrysettings.
        /// </summary>
        /// <param name="createServiceRemotingClientFactory">
        /// Specifies the factory method that creates the remoting client factory. The remoting client factory got from this method
        /// is cached in the ServiceProxyFactory.
        /// </param>
        /// <param name="retrySettings">Specifies the retry policy to use on exceptions seen when using the proxies created by this factory</param>
        public ServiceProxyFactory(
            Func<V1.IServiceRemotingCallbackClient, V1.Client.IServiceRemotingClientFactory> createServiceRemotingClientFactory,
            OperationRetrySettings retrySettings = null)
        {
            this.proxyFactoryV1 = new V1.Client.ServiceProxyFactory(createServiceRemotingClientFactory, retrySettings);
        }

#endif
        /// <summary>
        /// Instantiates the ServiceProxyFactory with the specified V2 remoting factory and retrysettings.
        /// </summary>
        /// <param name="createServiceRemotingClientFactory">
        /// Specifies the factory method that creates the remoting client factory. The remoting client factory got from this method
        /// is cached in the ServiceProxyFactory.
        /// </param>
        /// <param name="retrySettings">Specifies the retry policy to use on exceptions seen when using the proxies created by this factory</param>
        public ServiceProxyFactory(
            Func<IServiceRemotingCallbackMessageHandler, Remoting.V2.Client.IServiceRemotingClientFactory>
                createServiceRemotingClientFactory,
            OperationRetrySettings retrySettings = null)
        {
            this.proxyFactoryV2 = new V2.Client.ServiceProxyFactory(createServiceRemotingClientFactory, retrySettings);
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
        public TServiceInterface CreateServiceProxy<TServiceInterface>(Uri serviceUri,
            ServicePartitionKey partitionKey = null,
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default, string listenerName = null)
            where TServiceInterface : IService
        {
            var serviceInterfaceType = typeof(TServiceInterface);

#if !DotNetCoreClr

            //Use provider to find the stack
            if (this.proxyFactoryV1 == null && this.proxyFactoryV2 == null)
            {
                var provider = this.GetProviderAttribute(serviceInterfaceType);
                if (provider.RemotingClient.Equals(RemotingClient.V2Client))
                {
                    //We are overriding listenerName since using provider we can have multiple listener configured(Compat Mode).
                    this.overrideListenerName = true;
                    this.proxyFactoryV2 =
                        new V2.Client.ServiceProxyFactory(provider.CreateServiceRemotingClientFactoryV2, this.retrySettings);
                }
                else
                {
                    this.proxyFactoryV1 = new V1.Client.ServiceProxyFactory(provider.CreateServiceRemotingClientFactory, this.retrySettings);
                }

            }

            if (this.proxyFactoryV1 != null)
            {
                return this.proxyFactoryV1.CreateServiceProxy<TServiceInterface>(serviceUri,
                    partitionKey,
                    targetReplicaSelector,
                    listenerName);
            }
            if (this.overrideListenerName && listenerName == null)
            {
                return this.proxyFactoryV2.CreateServiceProxy<TServiceInterface>(serviceUri,
                    partitionKey,
                    targetReplicaSelector,
                    ServiceRemotingProviderAttribute.DefaultV2listenerName);
            }
            return this.proxyFactoryV2.CreateServiceProxy<TServiceInterface>(serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName);
#else
            if (proxyFactoryV2 == null)
            {
                var provider = this.GetProviderAttribute(serviceInterfaceType);
                //We are overriding listenerName since using provider we can have multiple listener configured(Compat Mode).
                this.overrideListenerName = true;
                this.proxyFactoryV2 =
                    new V2.Client.ServiceProxyFactory(provider.CreateServiceRemotingClientFactoryV2, this.retrySettings);
            }
            if (this.overrideListenerName && listenerName == null)
            {
                return this.proxyFactoryV2.CreateServiceProxy<TServiceInterface>(serviceUri,
                    partitionKey,
                    targetReplicaSelector,
                    ServiceRemotingProviderAttribute.DefaultV2listenerName);
            }
            return this.proxyFactoryV2.CreateServiceProxy<TServiceInterface>(serviceUri,
              partitionKey,
              targetReplicaSelector,
              listenerName);
#endif
        }


        /// <summary>
        /// Creates a proxy  to communicate to the specified service using the remoted interface TServiceInterface that 
        /// the service implements.
        /// <typeparam name="TServiceInterface">Interface that is being remoted . Service Interface does not need to be inherited from IService.</typeparam>
        /// <param name="serviceUri">Uri of the Service.</param>
        /// <param name="partitionKey">The Partition key that determines which service partition is responsible for handling requests from this service proxy</param>
        /// <param name="targetReplicaSelector">Determines which replica or instance of the service partition the client should connect to.</param>
        /// <param name="listenerName">This parameter is Optional if the service has a single communication listener. The endpoints from the service
        /// are of the form {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}. When the service exposes multiple endpoints, this parameter
        /// identifies which of those endpoints to use for the remoting communication.
        /// </param>
        /// <returns>The proxy that implement the interface that is being remoted. The returned object also implement <see cref="IServiceProxy"/> interface.</returns>
        /// </summary>
        public TServiceInterface CreateNonIServiceProxy<TServiceInterface>(Uri serviceUri,
            ServicePartitionKey partitionKey = null,
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default, string listenerName = null)
        {
            var serviceInterfaceType = typeof(TServiceInterface);

            if (this.proxyFactoryV2 == null)
            {
                var provider = this.GetProviderAttribute(serviceInterfaceType);

                this.proxyFactoryV2 = new V2.Client.ServiceProxyFactory(provider.CreateServiceRemotingClientFactoryV2, this.retrySettings);
            }
            return this.proxyFactoryV2.CreateNonIServiceProxy<TServiceInterface>(serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName);
        }

        private ServiceRemotingProviderAttribute GetProviderAttribute(Type serviceInterfaceType)
        {
            return ServiceRemotingProviderAttribute.GetProvider(new[] { serviceInterfaceType });

        }

    }
}
