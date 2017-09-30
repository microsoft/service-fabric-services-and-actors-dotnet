// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Client
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;


    /// <summary>
    /// An <see cref="IServiceRemotingClientFactory"/> that uses
    /// Windows Communication Foundation to create <see cref="IServiceRemotingClient"/> to communicate with stateless
    /// and stateful services over interfaces that are remoted via  WcfServiceRemotingListener
    /// </summary>
    public class WcfServiceRemotingClientFactory : IServiceRemotingClientFactory
    {
        private WcfCommunicationClientFactory<IServiceRemotingContract> wcfFactory;
        private ServiceRemotingMessageSerializersManager serializersManager;
        private IServiceRemotingMessageBodyFactory remotingMessageBodyFactory;
        /// <summary>
        /// Event handler that is fired when a client is connected to the service endpoint.
        /// </summary>
        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientConnected;

        /// <summary>
        /// Event handler that is fired when a client is disconnected from the service endpoint.
        /// </summary>
        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientDisconnected;

        /// <summary>
        ///     Constructs a WCF based service remoting client factory.
        /// </summary>
        /// <param name="clientBinding">
        ///     WCF binding to use for the client. If the client binding is not specified or null,
        ///     a default client binding is created using 
        ///     <see cref="Microsoft.ServiceFabric.Services.Communication.Wcf.WcfUtility.CreateTcpClientBinding"/> method 
        ///     which creates a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="callbackClient">
        ///     The callback client that receives the callbacks from the service.
        /// </param>
        /// <param name="exceptionHandlers">
        ///     Exception handlers to handle the exceptions encountered in communicating with the service.
        /// </param>
        /// <param name="servicePartitionResolver">
        ///     Service partition resolver to resolve the service endpoints. If not specified, a default 
        ///     service partition resolver returned by <see cref="ServicePartitionResolver.GetDefault"/> is used.
        /// </param>
        /// <param name="traceId">
        ///     Id to use in diagnostics traces from this component.
        /// </param>
        /// <param name="createWcfClientFactory">
        ///     Delegate function that creates <see cref="Microsoft.ServiceFabric.Services.Communication.Wcf.Client.WcfCommunicationClientFactory{TServiceContract}"/> using the 
        ///     <see cref="Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.IServiceRemotingContract"/>.
        /// </param>
        /// <param name="serializationProvider"></param>
        /// <remarks>
        ///     This factory uses <see cref="WcfExceptionHandler"/> and <see cref="ServiceRemotingExceptionHandler"/> in addition to the 
        ///     exception handlers supplied to the constructor. 
        /// </remarks>
        public WcfServiceRemotingClientFactory(
            Binding clientBinding = null,
            IServiceRemotingCallbackMessageHandler callbackClient = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            IServicePartitionResolver servicePartitionResolver = null,
            string traceId = null,
            Func<
                Binding,
                IEnumerable<IExceptionHandler>,
                IServicePartitionResolver,
                string,
                IServiceRemotingCallbackContract,
                WcfCommunicationClientFactory<IServiceRemotingContract>> createWcfClientFactory = null,
                IServiceRemotingMessageSerializationProvider serializationProvider = null)
            
        {
            if (serializationProvider == null)
            {
                serializationProvider = new BasicDataContractSerializationProvider();
            }

            var serializersManager = new ServiceRemotingMessageSerializersManager(serializationProvider,
                new BasicDataContractHeaderSerializer());

            this.Initialize(serializersManager,
                clientBinding,
                callbackClient,
                exceptionHandlers,
                servicePartitionResolver,
                traceId,
                createWcfClientFactory);

        }


        internal WcfServiceRemotingClientFactory(
            ServiceRemotingMessageSerializersManager serializersManager,
            Binding clientBinding = null,
            IServiceRemotingCallbackMessageHandler callbackClient = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            IServicePartitionResolver servicePartitionResolver = null,
            string traceId = null,
            Func<
                Binding,
                IEnumerable<IExceptionHandler>,
                IServicePartitionResolver,
                string,
                IServiceRemotingCallbackContract,
                WcfCommunicationClientFactory<IServiceRemotingContract>> createWcfClientFactory = null
            )
        {
            this.Initialize(serializersManager, clientBinding, callbackClient, exceptionHandlers, servicePartitionResolver, traceId, createWcfClientFactory);
        }

        private void Initialize(ServiceRemotingMessageSerializersManager serializersManager, Binding clientBinding,
            IServiceRemotingCallbackMessageHandler callbackClient, IEnumerable<IExceptionHandler> exceptionHandlers,
            IServicePartitionResolver servicePartitionResolver, string traceId, Func<Binding, IEnumerable<IExceptionHandler>, IServicePartitionResolver, string, IServiceRemotingCallbackContract, WcfCommunicationClientFactory<IServiceRemotingContract>> createWcfClientFactory)
        {
            this.serializersManager = serializersManager;
            if (traceId == null)
            {
                traceId = Guid.NewGuid().ToString();
            }

            if (createWcfClientFactory == null)
            {
                this.wcfFactory = new WcfCommunicationClientFactory<IServiceRemotingContract>(
                    clientBinding,
                    GetExceptionHandlers(exceptionHandlers, traceId),
                    servicePartitionResolver,
                    traceId,
                    this.GetCallbackImplementation(callbackClient));
            }
            else
            {
                this.wcfFactory = createWcfClientFactory(
                    clientBinding,
                    GetExceptionHandlers(exceptionHandlers, traceId),
                    servicePartitionResolver,
                    traceId,
                    this.GetCallbackImplementation(callbackClient));
            }


            this.wcfFactory.ClientConnected += this.OnClientConnected;
            this.wcfFactory.ClientDisconnected += this.OnClientDisconnected;

            this.remotingMessageBodyFactory = this.serializersManager.GetSerializationProvider().CreateMessageBodyFactory();
        }

        /// <summary>
        /// Resolves a partition of the specified service containing one or more communication listeners and returns a client to communicate 
        /// to the endpoint corresponding to the given listenerName. 
        /// 
        /// The endpoint of the service is of the form - {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}
        /// </summary>
        /// <param name="serviceUri">Uri of the service to resolve</param>
        /// <param name="partitionKey">Key that identifies the partition to resolve</param>
        /// <param name="targetReplicaSelector">Specifies which replica in the partition identified by the partition key, the client should connect to</param>
        /// <param name="listenerName">Specifies which listener in the endpoint of the chosen replica, to which the client should connect to</param>
        /// <param name="retrySettings">Specifies the retry policy that should be used for exceptions that occur when creating the client.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result of the Task is
        /// the CommunicationClient(<see cref="ICommunicationClient" />) object.
        /// </returns>
        async Task<IServiceRemotingClient> ICommunicationClientFactory<IServiceRemotingClient>.GetClientAsync(
            Uri serviceUri,
            ServicePartitionKey partitionKey,
            TargetReplicaSelector targetReplicaSelector,
            string listenerName,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            var wcfClient = await this.wcfFactory.GetClientAsync(
                serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName,
                retrySettings,
                cancellationToken);

            return new WcfServiceRemotingClient(wcfClient,
                this.serializersManager);
        }

        /// <summary>
        /// Re-resolves a partition of the specified service containing one or more communication listeners and returns a client to communicate 
        /// to the endpoint corresponding to the given listenerName. 
        /// 
        /// The endpoint of the service is of the form - {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}
        /// </summary>
        /// <param name="previousRsp">Previous ResolvedServicePartition value</param>
        /// <param name="targetReplicaSelector">Specifies which replica in the partition identified by the partition key, the client should connect to</param>
        /// <param name="listenerName">Specifies which listener in the endpoint of the chosen replica, to which the client should connect to</param>
        /// <param name="retrySettings">Specifies the retry policy that should be used for exceptions that occur when creating the client.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result of the Task is
        /// the CommunicationClient(<see cref="ICommunicationClient" />) object.
        /// </returns>
        async Task<IServiceRemotingClient> ICommunicationClientFactory<IServiceRemotingClient>.GetClientAsync(
            ResolvedServicePartition previousRsp,
            TargetReplicaSelector targetReplicaSelector,
            string listenerName,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            var wcfClient = await this.wcfFactory.GetClientAsync(
                previousRsp,
                targetReplicaSelector,
                listenerName,
                retrySettings,
                cancellationToken);

            return new WcfServiceRemotingClient(wcfClient,
                this.serializersManager);
        }

        /// <summary>
        /// Handles the exceptions that occur in the CommunicationClient when sending a message to the Service
        /// </summary>
        /// <param name="client">Communication client</param>
        /// <param name="exceptionInformation">Information about exception that happened while communicating with the service.</param>
        /// <param name="retrySettings">Specifies the retry policy that should be used for handling the reported exception.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result of the Task is
        /// a <see cref="OperationRetryControl" /> object that provides information on retry policy for this exception.
        /// </returns>
        Task<OperationRetryControl> ICommunicationClientFactory<IServiceRemotingClient>.ReportOperationExceptionAsync(
            IServiceRemotingClient client,
            ExceptionInformation exceptionInformation,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            return this.wcfFactory.ReportOperationExceptionAsync(
                ((WcfServiceRemotingClient)client).WcfClient,
                exceptionInformation,
                retrySettings,
                cancellationToken);
        }

        /// <summary>
        /// Returns the Message Factory used to create Request and Response Remoting Message Body
        /// </summary>
        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
          return  this.remotingMessageBodyFactory;
        }

        private void OnClientDisconnected(
            object sender,
            CommunicationClientEventArgs<WcfCommunicationClient<IServiceRemotingContract>> communicationClientEventArgs)
        {
            var handlers = this.ClientDisconnected;
            if (handlers != null)
            {
                handlers(
                    this,
                    new CommunicationClientEventArgs<IServiceRemotingClient>()
                    {
                        Client = new WcfServiceRemotingClient(communicationClientEventArgs.Client,this.serializersManager)
                    });
            }
        }

        private void OnClientConnected(
            object sender,
            CommunicationClientEventArgs<WcfCommunicationClient<IServiceRemotingContract>> communicationClientEventArgs)
        {
            var handlers = this.ClientConnected;
            if (handlers != null)
            {
                handlers(
                    this,
                    new CommunicationClientEventArgs<IServiceRemotingClient>()
                    {
                        Client = new WcfServiceRemotingClient(communicationClientEventArgs.Client,this.serializersManager)
                    });
            }
        }

        private static IEnumerable<IExceptionHandler> GetExceptionHandlers(
            IEnumerable<IExceptionHandler> exceptionHandlers,
            string traceId)
        {
            var handlers = new List<IExceptionHandler>();
            if (exceptionHandlers != null)
            {
                handlers.AddRange(exceptionHandlers);
            }
            handlers.Add(new ServiceRemotingExceptionHandler(traceId));

            return handlers;
        }

        private  IServiceRemotingCallbackContract GetCallbackImplementation(
            IServiceRemotingCallbackMessageHandler callbackClient)
        {
            if (callbackClient == null)
            {
                return new NoOpCallbackReceiver();
            }
            else
            {
                return new CallbackReceiver(callbackClient,
                    this.serializersManager);
            }
        }

        [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        private class NoOpCallbackReceiver : IServiceRemotingCallbackContract
        {
            public NoOpCallbackReceiver()
            {
            }
            

            public void SendOneWay(ArraySegment<byte> messageHeaders, IEnumerable<ArraySegment<byte>> requestBody)
            {
            }
        }


        [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        private class CallbackReceiver : IServiceRemotingCallbackContract
        {
            private readonly IServiceRemotingCallbackMessageHandler callbackHandler;
            private readonly ServiceRemotingMessageSerializersManager serializersManager;

            public CallbackReceiver(IServiceRemotingCallbackMessageHandler callbackHandler,
                ServiceRemotingMessageSerializersManager serializersManager)
            {
                this.callbackHandler = callbackHandler;
                this.serializersManager = serializersManager;
            }
          
            
            public void SendOneWay(ArraySegment<byte> messageHeaders, IEnumerable<ArraySegment<byte>> requestBody)
            {
                var headerSerializer = this.serializersManager.GetHeaderSerializer();
                var deserializerHeaders = headerSerializer.DeserializeRequestHeaders(new IncomingMessageHeader(new SegmentedReadMemoryStream(messageHeaders)));
                var msgBodySerializer = this.serializersManager.GetRequestBodySerializer(deserializerHeaders.InterfaceId);
                var deserializedMsgBody = msgBodySerializer.Deserialize(new IncomingMessageBody(new SegmentedReadMemoryStream(requestBody)));
                var msg = new ServiceRemotingRequestMessage(deserializerHeaders,deserializedMsgBody);
                Task.Run(() => this.callbackHandler.HandleOneWayMessage(msg));
            }
        }
    }
}

