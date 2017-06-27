// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Client
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;


    /// <summary>
    /// Provides the base implementation for the proxy to the remoted IService interfaces.
    /// </summary>
    public abstract class ServiceProxy : ProxyBase, IServiceProxy
    {
        private static readonly ServiceProxyFactory DefaultProxyFactory = new ServiceProxyFactory();

        private ServiceProxyGeneratorWith proxyGeneratorWith;
        private ServiceRemotingPartitionClient partitionClient;
        
        /// <summary>
        /// Gets the interface type that is being remoted.
        /// </summary>
        /// <value>The service interface type.</value>
        public Type ServiceInterfaceType
        {
            get { return this.proxyGeneratorWith.ProxyInterfaceType; }
        }

        /// <summary>
        /// Gets the service partition client used to send requests to the service.
        /// </summary>
        /// <value>The ServicePartitionClient used by the ServiceProxy.</value>
        public IServiceRemotingPartitionClient ServicePartitionClient
        {
            get { return this.partitionClient; }
        }

        /// <summary>
        /// Creates a proxy to communicate to the specified service using the remoted interface TServiceInterface that 
        /// the service implements.
        /// <typeparam name="TServiceInterface">The interface that is being remoted.</typeparam>
        /// <param name="serviceUri">The Uri of the Service.</param>
        /// <param name="partitionKey">The Partition key that determines which service partition is responsible for handling requests from this service proxy.</param>
        /// <param name="targetReplicaSelector">Determines which replica or instance of the service partition the client should connect to.</param>
        /// <param name="listenerName">This parameter is Optional if the service has a single communication listener. The endpoints from the service
        /// are of the form {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}. When the service exposes multiple endpoints, this parameter
        /// identifies which of those endpoints to use for the remoting communication.
        /// </param>
        /// <returns>The proxy that implement the interface that is being remoted. The returned object also implement <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.IServiceProxy"/> interface.</returns>
        /// </summary>
        public static TServiceInterface Create<TServiceInterface>(
            Uri serviceUri,
            ServicePartitionKey partitionKey = null,
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default,
            string listenerName = null) where TServiceInterface : IService
        {
            return DefaultProxyFactory.CreateServiceProxy<TServiceInterface>(
                serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName);
        }

        internal void Initialize(ServiceProxyGeneratorWith generatorWith, ServiceRemotingPartitionClient client)
        {
            this.proxyGeneratorWith = generatorWith;
            this.partitionClient = client;
        }

        internal override DataContractSerializer GetRequestMessageBodySerializer(int interfaceId)
        {
            return this.proxyGeneratorWith.GetRequestMessageBodySerializer(interfaceId);
        }

        internal override DataContractSerializer GetResponseMessageBodySerializer(int interfaceId)
        {
            return this.proxyGeneratorWith.GetResponseMessageBodySerializer(interfaceId);
        }

        internal override object GetResponseMessageBodyValue(object responseMessageBody)
        {
            return ((ServiceRemotingMessageBody) responseMessageBody).Value;
        }

        internal override object CreateRequestMessageBody(object requestMessageBodyValue)
        {
            return new ServiceRemotingMessageBody()
            {
                Value = requestMessageBodyValue
            };
        }

        internal override Task<byte[]> InvokeAsync(
            int interfaceId, 
            int methodId, 
            byte[] requestMsgBodyBytes, 
            CancellationToken cancellationToken)
        {
            var headers = new ServiceRemotingMessageHeaders()
            {
                InterfaceId = interfaceId,
                MethodId = methodId
            };

            return this.partitionClient.InvokeAsync(
                headers,
                requestMsgBodyBytes,
                cancellationToken);
        }

        internal override void Invoke(int interfaceId, int methodId, byte[] requestMsgBodyBytes)
        {
            // no - op as events/one way messages are not supported for services
        }
    }
}

