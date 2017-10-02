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

#if !DotNetCoreClr

    using Microsoft.ServiceFabric.Services.Remoting.V1;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Client;
#endif
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Builder;

    /// <summary>
    /// Provides the base implementation for the proxy to the remoted IService interfaces.
    /// </summary>
    public abstract class ServiceProxy : Remoting.Builder.ProxyBase, IServiceProxy
    {
        private static readonly ServiceProxyFactory DefaultProxyFactory = new ServiceProxyFactory();

#if !DotNetCoreClr
        private ServiceProxyGeneratorWith proxyGeneratorV1;
        private ServiceRemotingPartitionClient partitionClient;
#endif
        private V2.Client.ServiceRemotingPartitionClient partitionClientV2;

        /// <summary>
        /// The interface type that is being remoted.
        /// </summary>
        /// <value>Service interface type</value>

        public Type ServiceInterfaceType { get; private set; }



#if !DotNetCoreClr

        /// <summary>
        /// The V1 Service partition client used to send requests to the service.
        /// </summary>
        /// <value>ServicePartitionClient used by the ServiceProxy</value>
        public IServiceRemotingPartitionClient ServicePartitionClient
        {
            get { return this.partitionClient; }
        }
#endif

        /// <summary>
        /// The V2 Service partition client used to send requests to the service.
        /// </summary>
        /// <value>ServicePartitionClient used by the ServiceProxy</value>

        public V2.Client.IServiceRemotingPartitionClient ServicePartitionClient2
        {
            get { return this.partitionClientV2; }
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

#if !DotNetCoreClr

        internal void Initialize(ServiceProxyGeneratorWith generatorWith, ServiceRemotingPartitionClient client)
        {
            this.proxyGeneratorV1 = generatorWith;
            this.ServiceInterfaceType = this.proxyGeneratorV1.ProxyInterfaceType;
            this.partitionClient = client;
        }

        internal override DataContractSerializer GetRequestMessageBodySerializer(int interfaceId)
        {
            return this.proxyGeneratorV1.GetRequestMessageBodySerializer(interfaceId);
        }

        internal override DataContractSerializer GetResponseMessageBodySerializer(int interfaceId)
        {
            return this.proxyGeneratorV1.GetResponseMessageBodySerializer(interfaceId);
        }

        internal override object GetResponseMessageBodyValue(object responseMessageBody)
        {
            return ((ServiceRemotingMessageBody)responseMessageBody).Value;
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

#endif
        //V2 APi
        internal void Initialize(
            ServiceFabric.Services.Remoting.V2.Builder.ServiceProxyGenerator proxyGenerator,
            ServiceFabric.Services.Remoting.V2.Client.ServiceRemotingPartitionClient client,
            IServiceRemotingMessageBodyFactory serviceRemotingMessageBodyFactory)
        {
            this.partitionClientV2 = client;
            this.ServiceInterfaceType = proxyGenerator.ProxyInterfaceType;
            base.InitializeV2(serviceRemotingMessageBodyFactory);
        }

        internal override Task<IServiceRemotingResponseMessage> InvokeAsyncImplV2(
            int interfaceId,
            int methodId,
            IServiceRemotingRequestMessageBody requestMsgBodyValue,
            CancellationToken cancellationToken)
        {
            var headers = new ServiceRemotingRequestMessageHeader()
            {
                InterfaceId = interfaceId,
                MethodId = methodId
            };
            return this.partitionClientV2.InvokeAsync(
                new ServiceRemotingRequestMessage(headers, requestMsgBodyValue),
                cancellationToken);
        }

        internal override void InvokeImplV2(
            int interfaceId,
            int methodId,
            IServiceRemotingRequestMessageBody requestMsgBodyValue)
        {
            // no - op as events/one way messages are not supported for services
        }
    }
}

