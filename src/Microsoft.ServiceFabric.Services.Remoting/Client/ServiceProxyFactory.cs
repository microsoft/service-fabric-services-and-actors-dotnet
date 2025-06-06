// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

namespace Microsoft.ServiceFabric.Services.Remoting.Client
{
    /// <summary>
    /// Specifies the factory that creates proxies for remote communication to the specified service.
    /// </summary>
    public class ServiceProxyFactory : IServiceProxyFactory
    {
        private readonly object thisLock;
        private readonly OperationRetrySettings retrySettings;
        private V2.Client.ServiceProxyFactory proxyFactoryV2;
        private bool overrideListenerName = false;
        private string defaultListenerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProxyFactory"/> class with the specified retrysettings and default remotingClientFactory.
        /// </summary>
        /// <param name="retrySettings">The settings for retrying the failed operations.</param>
        public ServiceProxyFactory(OperationRetrySettings retrySettings = null)
        {
            this.retrySettings = retrySettings;
            this.thisLock = new object();
            this.proxyFactoryV2 = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProxyFactory"/> class with the specified V2 remoting factory and retrysettings.
        /// </summary>
        /// <param name="createServiceRemotingClientFactory">
        /// Specifies the factory method that creates the remoting client factory. The remoting client factory got from this method
        /// is cached in the ServiceProxyFactory.
        /// </param>
        /// <param name="retrySettings">Specifies the retry policy to use on exceptions seen when using the proxies created by this factory</param>
        /// <param name="disposeFactory">Specifies the method that disposes clientFactory resources.</param>
        public ServiceProxyFactory(
            Func<IServiceRemotingCallbackMessageHandler, IServiceRemotingClientFactory> createServiceRemotingClientFactory,
            OperationRetrySettings retrySettings = null,
            Action<IServiceRemotingClientFactory> disposeFactory = null)
        {
            this.proxyFactoryV2 = new V2.Client.ServiceProxyFactory(createServiceRemotingClientFactory, retrySettings, disposeFactory);
            this.thisLock = new object();
        }

        /// <summary>
        /// Creates a proxy to communicate to the specified service using the remoted interface TServiceInterface that
        /// the service implements.
        /// </summary>
        /// <typeparam name="TServiceInterface">Interface that is being remoted</typeparam>
        /// <param name="serviceUri">Uri of the Service.</param>
        /// <param name="partitionKey">The Partition key that determines which service partition is responsible for handling requests from this service proxy</param>
        /// <param name="targetReplicaSelector">Determines which replica or instance of the service partition the client should connect to.</param>
        /// <param name="listenerName">This parameter is Optional if the service has a single communication listener. The endpoints from the service
        /// are of the form {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}. When the service exposes multiple endpoints, this parameter
        /// identifies which of those endpoints to use for the remoting communication.
        /// </param>
        /// <returns>The proxy that implement the interface that is being remoted. The returned object also implement <see cref="IServiceProxy"/> interface.</returns>
        public TServiceInterface CreateServiceProxy<TServiceInterface>(
            Uri serviceUri,
            ServicePartitionKey partitionKey = null,
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default,
            string listenerName = null)
            where TServiceInterface : IService
        {
            var serviceInterfaceType = typeof(TServiceInterface);

            if (this.proxyFactoryV2 == null)
            {
                lock (this.thisLock)
                {
                    if (this.proxyFactoryV2 == null)
                    {
                        ServiceRemotingProviderAttribute provider = this.GetProviderAttribute(serviceInterfaceType);

                        // We are overriding listenerName since using provider we can have multiple listener configured.
                        this.overrideListenerName = true;
                        this.defaultListenerName = this.GetDefaultListenerName(
                            listenerName,
                            provider.RemotingClientVersion);
                        this.proxyFactoryV2 =
                            new V2.Client.ServiceProxyFactory(provider.CreateServiceRemotingClientFactoryV2, this.retrySettings);
                    }
                }
            }

            return this.proxyFactoryV2.CreateServiceProxy<TServiceInterface>(
                serviceUri,
                partitionKey,
                targetReplicaSelector,
                this.OverrideListenerNameIfConditionMet(listenerName));
        }

        /// <summary>
        /// Creates a proxy  to communicate to the specified service using the remoted interface TServiceInterface that
        /// the service implements.
        /// </summary>
        /// <typeparam name="TServiceInterface">Interface that is being remoted . Service Interface does not need to be inherited from IService.</typeparam>
        /// <param name="serviceUri">Uri of the Service.</param>
        /// <param name="partitionKey">The Partition key that determines which service partition is responsible for handling requests from this service proxy</param>
        /// <param name="targetReplicaSelector">Determines which replica or instance of the service partition the client should connect to.</param>
        /// <param name="listenerName">This parameter is Optional if the service has a single communication listener. The endpoints from the service
        /// are of the form {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}. When the service exposes multiple endpoints, this parameter
        /// identifies which of those endpoints to use for the remoting communication.
        /// </param>
        /// <returns>The proxy that implement the interface that is being remoted. The returned object also implement <see cref="IServiceProxy"/> interface.</returns>
        public TServiceInterface CreateNonIServiceProxy<TServiceInterface>(
            Uri serviceUri,
            ServicePartitionKey partitionKey = null,
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default,
            string listenerName = null)
        {
            var serviceInterfaceType = typeof(TServiceInterface);

            if (this.proxyFactoryV2 == null)
            {
                lock (this.thisLock)
                {
                    if (this.proxyFactoryV2 == null)
                    {
                        var provider = this.GetProviderAttribute(serviceInterfaceType);

                        this.proxyFactoryV2 = new V2.Client.ServiceProxyFactory(provider.CreateServiceRemotingClientFactoryV2, this.retrySettings);
                    }
                }
            }

            return this.proxyFactoryV2.CreateNonIServiceProxy<TServiceInterface>(
                serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName);
        }

        /// <summary>
        /// Releases managed/unmanaged resources.
        /// Dispose Method is being added rather than making it IDisposable so that it doesn't change type information and wont be a breaking change.
        /// </summary>
        public void Dispose()
        {
            if (this.proxyFactoryV2 != null)
            {
                this.proxyFactoryV2.Dispose();
            }
        }

        private ServiceRemotingProviderAttribute GetProviderAttribute(Type serviceInterfaceType)
        {
            return ServiceRemotingProviderAttribute.GetProvider(new[] { serviceInterfaceType });
        }

        private string GetDefaultListenerName(
            string listenerName,
            RemotingClientVersion remotingClientVersion)
        {
            if (string.IsNullOrEmpty(listenerName))
            {
                if (Helper.IsRemotingV2(remotingClientVersion))
                {
                    return ServiceRemotingProviderAttribute.DefaultV2listenerName;
                }

                return ServiceRemotingProviderAttribute.DefaultWrappedMessageStackListenerName;
            }

            return listenerName;
        }

        private string OverrideListenerNameIfConditionMet(string listenerName)
        {
            if (this.overrideListenerName && string.IsNullOrEmpty(listenerName))
            {
                return this.defaultListenerName;
            }

            return listenerName;
        }
    }
}
