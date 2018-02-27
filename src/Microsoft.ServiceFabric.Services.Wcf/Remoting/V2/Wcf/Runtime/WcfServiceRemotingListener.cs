// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Wcf;
    using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    /// <summary>
    /// An <see cref="IServiceRemotingListener"/> that uses
    /// Windows Communication Foundation to provide interface remoting for stateless and stateful services.
    /// </summary>
    public class WcfServiceRemotingListener : IServiceRemotingListener
    {
        private IServiceRemotingMessageHandler messageHandler;
        private ICommunicationListener wcfListener;

        /// <summary>
        /// Constructs a WCF based service remoting listener.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="serviceImplementation">The service implementation object.</param>
        /// <param name="listenerBinding">WCF binding to use for the listener. If the listener binding is not specified or null,
        /// a default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method which creates
        /// a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="serializationProvider"></param>
        /// <param name="endpointResourceName">The name of the endpoint resource defined in the service manifest that
        /// should be used to create the address for the listener. If the endpointResourceName is not specified or null,
        /// the default value "ServiceEndpointV2" is used.
        /// </param>
        public WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation,
            Binding listenerBinding = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null,
            string endpointResourceName = "ServiceEndpointV2")
        {
            if (serializationProvider == null)
            {
                serializationProvider = new BasicDataContractSerializationProvider();
            }
            var serializerManager = new ServiceRemotingMessageSerializersManager(
                serializationProvider,
                new BasicDataContractHeaderSerializer());
            this.messageHandler = new ServiceRemotingMessageDispatcher(serviceContext, serviceImplementation,
                serializerManager.GetSerializationProvider().CreateMessageBodyFactory());
            this.wcfListener = new WcfCommunicationListener<IServiceRemotingContract>(
                serviceContext,
                new WcfRemotingService(
                    this.messageHandler,
                    serializerManager),
                listenerBinding,
                endpointResourceName);
        }

        /// <summary>
        /// Constructs a WCF based service remoting listener.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="messageHandler">The handler for receiving and processing remoting messages. As the messages are received
        /// the listener delivers the messages to the handler.
        /// </param>
        /// <param name="serializationProvider"></param>
        /// <param name="listenerBinding">WCF binding to use for the listener. If the listener binding is not specified or null,
        /// a default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method which creates
        /// a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="endpointResourceName">The name of the endpoint resource defined in the service manifest that
        /// should be used to create the address for the listener. If the endpointResourceName is not specified or it is null,
        /// the default value "ServiceEndpointV2" is used.
        /// </param>
        public WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            IServiceRemotingMessageSerializationProvider serializationProvider = null,
            Binding listenerBinding = null,
            string endpointResourceName = "ServiceEndpointV2")
        {
            if (serializationProvider == null)
            {
                serializationProvider = new BasicDataContractSerializationProvider();
            }
            var serializerManager = new ServiceRemotingMessageSerializersManager(
                serializationProvider,
                new BasicDataContractHeaderSerializer());
            this.Initialize(serviceContext, messageHandler, listenerBinding, endpointResourceName, serializerManager);
        }


        /// <summary>
        /// Constructs a WCF based service remoting listener.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="messageHandler">The handler for receiving and processing remoting messages. As the messages are received
        /// the listener delivers the messages to the handler.
        /// </param>
        /// <param name="serializationProvider"></param>
        /// <param name="listenerBinding">WCF binding to use for the listener. If the listener binding is not specified or null,
        /// a default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method.
        /// </param>
        /// <param name="address">The endpoint address to use for the WCF listener. If not specified or null, the endpoint
        /// address is created using the default endpoint resource named "ServiceEndpointV2" defined in the service manifest.
        /// </param>
        public WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            IServiceRemotingMessageSerializationProvider serializationProvider = null,
            Binding listenerBinding = null,
            EndpointAddress address = null)
        {
            if (serializationProvider == null)
            {
                serializationProvider = new BasicDataContractSerializationProvider();
            }
            var serializerManager = new ServiceRemotingMessageSerializersManager(
                serializationProvider,
                new BasicDataContractHeaderSerializer());
            this.Initialize(serviceContext, listenerBinding, address, serializerManager, messageHandler);
        }


        internal WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            ServiceRemotingMessageSerializersManager serializersManager,
            Binding listenerBinding = null,
            EndpointAddress address = null)
        {
            this.Initialize(
                serviceContext,
                listenerBinding,
                address,
                serializersManager,
                messageHandler);
        }

        internal WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            ServiceRemotingMessageSerializersManager serializerManager,
            Binding listenerBinding = null,
            string endpointResourceName = "ServiceEndpointV2")
        {
            this.Initialize(serviceContext, messageHandler, listenerBinding, endpointResourceName, serializerManager);
        }

        /// <summary>
        ///     Gets the <see cref="System.ServiceModel.ServiceHost"/> used by this listener to host the
        ///     WCF service implementation.
        /// </summary>
        /// <value>
        ///     A <see cref="System.ServiceModel.ServiceHost"/> used by this listener to host the
        ///     WCF service implementation.
        /// </value>
        /// <remarks>
        ///     The service host is created by the listener in it's constructor. Before this communication
        ///     listener is opened by the runtime via <see cref="ICommunicationListener.OpenAsync(CancellationToken)"/> method,
        ///     the service host can be customized by accessing it via this property.
        /// </remarks>
        public ServiceHost ServiceHost
        {
            get { return ((WcfCommunicationListener<IServiceRemotingContract>)this.wcfListener).ServiceHost; }
        }

        /// <summary>
        /// This method causes the communication listener to be opened. Once the Open
        /// completes, the communication listener becomes usable - accepts and sends messages.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result of the Task is
        /// the endpoint string.
        /// </returns>
        Task<string> ICommunicationListener.OpenAsync(CancellationToken cancellationToken)
        {
            return this.wcfListener.OpenAsync(cancellationToken);
        }

        /// <summary>
        /// This method causes the communication listener to close. Close is a terminal state and
        /// this method allows the communication listener to transition to this state in a
        /// graceful manner.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        Task ICommunicationListener.CloseAsync(CancellationToken cancellationToken)
        {
            this.DisposeIfNeeded();
            return this.wcfListener.CloseAsync(cancellationToken);
        }

        /// <summary>
        /// This method causes the communication listener to close. Close is a terminal state and
        /// this method causes the transition to close ungracefully. Any outstanding operations
        /// (including close) should be canceled when this method is called.
        /// </summary>
        void ICommunicationListener.Abort()
        {
            this.DisposeIfNeeded();
            this.wcfListener.Abort();
        }

        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        private class WcfRemotingService : IServiceRemotingContract
        {
            private readonly IServiceRemotingMessageHandler messageHandler;

            private readonly ServiceRemotingMessageSerializersManager serializersManager;

            //
            // The request context need not be generated every time for WCF because for WCF,
            // the actual callback channel is accessed from the current operation context.
            //
            private readonly WcfServiceRemotingRequestContext requestContext;

            public WcfRemotingService(
                IServiceRemotingMessageHandler messageHandler,
                ServiceRemotingMessageSerializersManager serializersManager)
            {
                this.messageHandler = messageHandler;
                this.serializersManager = serializersManager;
                this.requestContext = new WcfServiceRemotingRequestContext(this.serializersManager);
            }

            public async Task<ResponseMessage> RequestResponseAsync(
                ArraySegment<byte> messageHeaders,
                IEnumerable<ArraySegment<byte>> requestBody)
            {
                IMessageBody outgoingMessageBody = null;
                IMessageHeader outgoingMessageHeader = null;
                try
                {
                    var headerSerializer = this.serializersManager.GetHeaderSerializer();
                    var deSerializedHeader =
                        headerSerializer.DeserializeRequestHeaders(
                            new IncomingMessageHeader(new SegmentedReadMemoryStream(messageHeaders)));

                    var msgBodySerializer =
                        this.serializersManager.GetRequestBodySerializer(deSerializedHeader.InterfaceId);
                    var deserializedMsg =
                        msgBodySerializer.Deserialize(
                            new IncomingMessageBody(new SegmentedReadMemoryStream(requestBody)));

                    var msg = new ServiceRemotingRequestMessage(deSerializedHeader, deserializedMsg);
                    var retval = await
                        this.messageHandler.HandleRequestResponseAsync(
                            this.requestContext,
                            msg);

                    if (retval == null)
                    {
                        return new ResponseMessage();
                    }

                    outgoingMessageHeader = headerSerializer.SerializeResponseHeader(retval.GetHeader());

                    var responseSerializer =
                        this.serializersManager.GetResponseBodySerializer(deSerializedHeader.InterfaceId);

                    outgoingMessageBody = responseSerializer.Serialize(retval.GetBody());

                    var responseMessage = new ResponseMessage();
                    responseMessage.ResponseBody = outgoingMessageBody != null
                        ? outgoingMessageBody.GetSendBuffers()
                        : new List<ArraySegment<byte>>();
                    responseMessage.MessageHeaders = outgoingMessageHeader != null
                        ? outgoingMessageHeader.GetSendBuffer()
                        : new ArraySegment<byte>();
                    return responseMessage;
                }
                catch (Exception e)
                {
                    ServiceTrace.Source.WriteInfo("WcfRemotingService", "Remote Exception occured {0}", e);
                    throw new FaultException<RemoteException>(RemoteException.FromException(e), e.Message);
                }
            }

            public void OneWayMessage(ArraySegment<byte> messageHeaders, IEnumerable<ArraySegment<byte>> requestBody)
            {
                throw new NotImplementedException();
            }
        }

        private void DisposeIfNeeded()
        {
            if (this.messageHandler is IDisposable disposableItem)
            {
                disposableItem.Dispose();
            }
        }

        private void Initialize(ServiceContext serviceContext, IServiceRemotingMessageHandler messageHandler,
            Binding listenerBinding, string endpointResourceName,
            ServiceRemotingMessageSerializersManager serializerManager)
        {
            this.messageHandler = messageHandler;
            this.wcfListener = new WcfCommunicationListener<IServiceRemotingContract>(
                serviceContext,
                new WcfRemotingService(this.messageHandler, serializerManager),
                listenerBinding,
                endpointResourceName);
        }

        private void Initialize(ServiceContext serviceContext, Binding listenerBinding, EndpointAddress address,
            ServiceRemotingMessageSerializersManager serializerManager,
            IServiceRemotingMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
            if (address != null)
            {
                this.wcfListener = new WcfCommunicationListener<IServiceRemotingContract>(
                    serviceContext,
                    new WcfRemotingService(
                        this.messageHandler,
                        serializerManager),
                    listenerBinding,
                    address);
            }
            else
            {
                this.wcfListener = new WcfCommunicationListener<IServiceRemotingContract>(
                    serviceContext,
                    new WcfRemotingService(
                        this.messageHandler,
                        serializerManager),
                    listenerBinding,
                    "ServiceEndpointV2");
            }
        }
    }
}
