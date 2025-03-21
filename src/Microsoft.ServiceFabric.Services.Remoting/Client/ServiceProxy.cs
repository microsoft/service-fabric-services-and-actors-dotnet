// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.Builder;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Builder;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

namespace Microsoft.ServiceFabric.Services.Remoting.Client
{
    /// <summary>
    /// Provides the base implementation for the proxy to the remoted IService interfaces.
    /// </summary>
    public abstract class ServiceProxy : ProxyBase, IServiceProxy
    {
        private static readonly ServiceProxyFactory DefaultProxyFactory = new ServiceProxyFactory();
        private ServiceRemotingPartitionClient partitionClientV2;

        /// <summary>
        /// Gets the interface type that is being remoted.
        /// </summary>
        /// <value>Service interface type</value>
        public Type ServiceInterfaceType { get; private set; }

        /// <summary>
        /// Gets the V2 Service partition client used to send requests to the service.
        /// </summary>
        /// <value>ServicePartitionClient used by the ServiceProxy</value>
        public IServiceRemotingPartitionClient ServicePartitionClient2
        {
            get { return this.partitionClientV2; }
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
        public static TServiceInterface Create<TServiceInterface>(
            Uri serviceUri,
            ServicePartitionKey partitionKey = null,
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default,
            string listenerName = null)
            where TServiceInterface : IService
        {
            return DefaultProxyFactory.CreateServiceProxy<TServiceInterface>(
                serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName);
        }

        // V2 APi
        internal void Initialize(
            ServiceProxyGenerator proxyGenerator,
            ServiceRemotingPartitionClient client,
            IServiceRemotingMessageBodyFactory serviceRemotingMessageBodyFactory)
        {
            this.partitionClientV2 = client;
            this.ServiceInterfaceType = proxyGenerator.ProxyInterfaceType;
            this.InitializeV2(serviceRemotingMessageBodyFactory);
        }

        internal override Task<IServiceRemotingResponseMessage> InvokeAsyncImplV2(
            int interfaceId,
            int methodId,
            string methodName,
            IServiceRemotingRequestMessageBody requestMsgBodyValue,
            CancellationToken cancellationToken)
        {
            var requestId = Guid.NewGuid();
            LogContext.Set(new LogContext
            {
                RequestId = requestId,
            });

            var headers = new ServiceRemotingRequestMessageHeader()
            {
                InterfaceId = interfaceId,
                MethodId = methodId,
                MethodName = methodName,
                RequestId = requestId,
            };

            ServiceTrace.Source.WriteInfo(
                "ServiceProxy",
                $"Invoking service proxy - RequestId : {headers.RequestId.ToString()}, MethodName : {headers.MethodName}");

            return this.partitionClientV2.InvokeAsync(
                new ServiceRemotingRequestMessage(headers, requestMsgBodyValue),
                methodName,
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
