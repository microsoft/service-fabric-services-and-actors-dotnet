// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

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
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime
{
    /// <summary>
    /// An <see cref="IServiceRemotingListener"/> that uses
    /// Windows Communication Foundation to provide interface remoting for stateless and stateful services.
    /// </summary>
    public class WcfServiceRemotingListener : IServiceRemotingListener
    {
        const string DefaultEndpointResourceName = "ServiceEndpointV2";

        IServiceRemotingMessageHandler messageHandler;
        ICommunicationListener wcfListener;
        WcfRemotingService remotingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="serviceImplementation">The service implementation object.</param>
        /// <param name="listenerBinding">
        ///     WCF binding to use for the listener. If the listener binding is not specified or null,
        ///     a default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method which creates
        ///     a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="serializationProvider">Serialization Provider.</param>
        /// <param name="endpointResourceName">
        ///     The name of the endpoint resource defined in the service manifest that
        ///     should be used to create the address for the listener. If the endpointResourceName is not specified or null,
        ///     the default value "ServiceEndpointV2" is used.
        /// </param>
        /// <param name="useWrappedMessage">
        ///     It indicates whether the remoting method parameters should be wrapped or not before sending it over the wire.
        ///     When UseWrappedMessage is set to false, parameters will not be wrapped. When this value is set to true, the
        ///     parameters will be wrapped.Default value is false.
        /// </param>
        [Obsolete]
        public WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation,
            Binding listenerBinding,
            IServiceRemotingMessageSerializationProvider serializationProvider,
            string endpointResourceName,
            bool useWrappedMessage) : this(serviceContext,
                                           serviceImplementation,
                                           listenerBinding,
                                           serializationProvider,
                                           endpointResourceName,
                                           useWrappedMessage,
                                           null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="serviceImplementation">The service implementation object.</param>
        /// <param name="listenerBinding">
        ///     WCF binding to use for the listener. If the listener binding is not specified or null,
        ///     a default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method which creates
        ///     a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="serializationProvider">Serialization Provider.</param>
        /// <param name="endpointResourceName">The name of the endpoint resource defined in the service manifest that
        /// should be used to create the address for the listener. If the endpointResourceName is not specified or null,
        /// the default value "ServiceEndpointV2" is used.
        /// </param>
        /// <param name="useWrappedMessage">
        ///     It indicates whether the remoting method parameters should be wrapped or not before sending it over the wire. 
        ///     When UseWrappedMessage is set to false, parameters will not be wrapped. When this value is set to true,
        ///     the parameters will be wrapped.Default value is false.
        /// </param>
        /// <param name="exceptionConvertors">
        ///     Convertors to convert user exception to service exception.
        /// </param>
        /// <param name="settings">
        ///     Settings for the remoting listener.
        /// </param>
        public WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation,
            Binding listenerBinding = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null,
            string endpointResourceName = DefaultEndpointResourceName,
            bool useWrappedMessage = false,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null,
            FabricTransportRemotingListenerSettings settings = null)
        {
            var serializersManager = new ServiceRemotingMessageSerializersManager(
                   GetDefaultSerializationProvider(serializationProvider, useWrappedMessage),
                   new BasicDataContractHeaderSerializer(),
                   useWrappedMessage);

            IServiceRemotingMessageHandler messageHandler = new ServiceRemotingMessageDispatcher(
                   serviceContext,
                   serviceImplementation,
                   serializersManager.GetSerializationProvider().CreateMessageBodyFactory());

            Initialize(
               serviceContext,
               messageHandler,
               serializersManager,
               listenerBinding,
               null,
               endpointResourceName,
               exceptionConvertors,
               settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="messageHandler">
        ///     The handler for receiving and processing remoting messages. As the messages are received
        ///     the listener delivers the messages to the handler.
        /// </param>
        /// <param name="serializationProvider">Serialization Provider.</param>
        /// <param name="listenerBinding">
        ///     WCF binding to use for the listener. If the listener binding is not specified or null,
        ///     a default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method which creates
        ///     a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="endpointResourceName">
        ///     The name of the endpoint resource defined in the service manifest that
        ///     should be used to create the address for the listener. If the endpointResourceName is not specified or it is null,
        ///     the default value "ServiceEndpointV2" is used.
        /// </param>
        /// <param name="useWrappedMessage">
        ///     It indicates whether the remoting method parameters should be wrapped or not before sending it over the wire.
        ///     When UseWrappedMessage is set to false, parameters will not be wrapped. When this value is set to true, the
        ///     parameters will be wrapped.Default value is false.
        /// </param>
        [Obsolete]
        public WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            IServiceRemotingMessageSerializationProvider serializationProvider = null,
            Binding listenerBinding = null,
            string endpointResourceName = DefaultEndpointResourceName,
            bool useWrappedMessage = false)
        {
            var serviceRemotingMessageSerializersManager = new ServiceRemotingMessageSerializersManager(
                GetDefaultSerializationProvider(serializationProvider, useWrappedMessage),
                new BasicDataContractHeaderSerializer());

            Initialize(
                serviceContext,
                messageHandler,
                serviceRemotingMessageSerializersManager,
                listenerBinding,
                null,
                endpointResourceName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="messageHandler">
        ///     The handler for receiving and processing remoting messages. As the messages are received
        ///     the listener delivers the messages to the handler.
        /// </param>
        /// <param name="serializationProvider">Serialization Provider.</param>
        /// <param name="listenerBinding">
        ///     WCF binding to use for the listener. If the listener binding is not specified or null,
        ///     a default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method which creates
        ///     a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="endpointResourceName">
        ///     The name of the endpoint resource defined in the service manifest that
        ///     should be used to create the address for the listener. If the endpointResourceName is not specified or it is null,
        ///     the default value "ServiceEndpointV2" is used.
        /// </param>
        /// <param name="useWrappedMessage">
        ///     It indicates whether the remoting method parameters should be wrapped or not before sending it over the wire.
        ///     When UseWrappedMessage is set to false, parameters will not be wrapped. When this value is set to true, the
        ///     parameters will be wrapped.Default value is false.
        /// </param>
        /// <param name="exceptionConvertors">
        ///     Convertors to convert user exception to service exception.
        /// </param>
        /// <param name="settings">
        ///     Settings for the remoting listener.
        /// </param>
        public WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            IServiceRemotingMessageSerializationProvider serializationProvider = null,
            Binding listenerBinding = null,
            string endpointResourceName = DefaultEndpointResourceName,
            bool useWrappedMessage = false,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null,
            FabricTransportRemotingListenerSettings settings = null)
        {
            var serviceRemotingMessageSerializersManager = new ServiceRemotingMessageSerializersManager(
                GetDefaultSerializationProvider(serializationProvider, useWrappedMessage),
                new BasicDataContractHeaderSerializer());

            Initialize(
                serviceContext,
                messageHandler,
                serviceRemotingMessageSerializersManager,
                listenerBinding,
                null,
                endpointResourceName,
                exceptionConvertors,
                settings);
    }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="messageHandler">
        ///     The handler for receiving and processing remoting messages. As the messages are received
        ///     the listener delivers the messages to the handler.
        /// </param>
        /// <param name="serializationProvider">Serialization Provider.</param>
        /// <param name="listenerBinding">
        ///     WCF binding to use for the listener. If the listener binding is not specified or null,
        ///     a default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method.
        /// </param>
        /// <param name="address">
        ///     The endpoint address to use for the WCF listener. If not specified or null, the endpoint
        ///     address is created using the default endpoint resource named "ServiceEndpointV2" defined in the service manifest.
        /// </param>
        /// <param name="useWrappedMessage">
        ///     It indicates whether the remoting method parameters should be wrapped or not before sending it over the wire.
        ///     When UseWrappedMessage is set to false, parameters will not be wrapped. When this value is set to true, the
        ///     parameters will be wrapped.Default value is false.
        /// </param>
        [Obsolete]
        public WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            IServiceRemotingMessageSerializationProvider serializationProvider = null,
            Binding listenerBinding = null,
            EndpointAddress address = null,
            bool useWrappedMessage = false) : this(
                serviceContext,
                messageHandler,
                serializationProvider,
                listenerBinding,
                address,
                useWrappedMessage,
                null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="messageHandler">
        ///     The handler for receiving and processing remoting messages. As the messages are received
        ///     the listener delivers the messages to the handler.
        /// </param>
        /// <param name="serializationProvider">Serialization Provider.</param>
        /// <param name="listenerBinding">
        ///     WCF binding to use for the listener. If the listener binding is not specified or null,
        ///     a default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method.
        /// </param>
        /// <param name="address">
        ///     The endpoint address to use for the WCF listener. If not specified or null, the endpoint
        ///     address is created using the default endpoint resource named "ServiceEndpointV2" defined in the service manifest.
        /// </param>
        /// <param name="useWrappedMessage">
        ///     It indicates whether the remoting method parameters should be wrapped or not before sending it over the wire.
        ///     When UseWrappedMessage is set to false, parameters will not be wrapped. When this value is set to true, the
        ///     parameters will be wrapped.Default value is false.
        /// </param>
        /// <param name="exceptionConvertors">
        ///     Convertors to convert user exception to service exception.
        /// </param>
        /// <param name="settings">
        ///     Settings for the remoting listener.
        /// </param>
        public WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            IServiceRemotingMessageSerializationProvider serializationProvider = null,
            Binding listenerBinding = null,
            EndpointAddress address = null,
            bool useWrappedMessage = false,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null,
            FabricTransportRemotingListenerSettings settings = null)
        {
            var serviceRemotingMessageSerializersManager = new ServiceRemotingMessageSerializersManager(
                GetDefaultSerializationProvider(serializationProvider, useWrappedMessage),
                new BasicDataContractHeaderSerializer());

            Initialize(
                serviceContext,
                messageHandler,
                serviceRemotingMessageSerializersManager,
                listenerBinding,
                address,
                DefaultEndpointResourceName,
                exceptionConvertors,
                settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="messageHandler">
        ///     The handler for receiving and processing remoting messages. As the messages are received
        ///     the listener delivers the messages to the handler.
        /// </param>
        /// <param name="serializersManager">Service remoting message serializers manager.</param>
        /// <param name="listenerBinding">
        ///     WCF binding to use for the listener. If the listener binding is not specified or null,
        ///     a default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method.
        /// </param>
        /// <param name="address">
        ///     The endpoint address to use for the WCF listener. If not specified or null, the endpoint
        ///     address is created using the default endpoint resource named "ServiceEndpointV2" defined in the service manifest.
        /// </param>
        /// <param name="endpointResourceName">
        ///     The name of the endpoint resource defined in the service manifest that
        ///     should be used to create the address for the listener. If the endpointResourceName is not specified or it is null,
        ///     the default value "ServiceEndpointV2" is used.
        /// </param>
        /// <param name="useWrappedMessage">
        ///     It indicates whether the remoting method parameters should be wrapped or not before sending it over the wire.
        ///     When UseWrappedMessage is set to false, parameters will not be wrapped. When this value is set to true, the
        ///     parameters will be wrapped.Default value is false.
        /// </param>
        /// <param name="exceptionConvertors">
        ///     Convertors to convert user exception to service exception.
        /// </param>
        /// <param name="settings">
        ///     Settings for the remoting listener.
        /// </param>
        private protected WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            ServiceRemotingMessageSerializersManager serializersManager = null,
            Binding listenerBinding = null,
            EndpointAddress address = null,
            string endpointResourceName = DefaultEndpointResourceName,
            bool useWrappedMessage = false,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null,
            FabricTransportRemotingListenerSettings settings = null)
        {
            Initialize(
                serviceContext,
                messageHandler,
                serializersManager,
                listenerBinding,
                address,
                DefaultEndpointResourceName,
                exceptionConvertors,
                settings);
        }

        readonly Func<IEnumerable<IExceptionConvertor>, FabricTransportRemotingListenerSettings, ExceptionSerializer> exceptionSerializerFactory = ExceptionSerializer.CreateDefault;

        private void Initialize(ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            ServiceRemotingMessageSerializersManager serializersManager,
            Binding listenerBinding = null,
            EndpointAddress address = null,
            string endpointResourceName = DefaultEndpointResourceName,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null,
            FabricTransportRemotingListenerSettings settings = null)
        {
            ExceptionSerializer exceptionSerializer = exceptionSerializerFactory(exceptionConvertors, settings);

            remotingService = new WcfRemotingService(
                messageHandler,
                serializersManager,
                exceptionSerializer);

            this.messageHandler = messageHandler;

            if (address != null)
            {
                wcfListener = new WcfCommunicationListener<IServiceRemotingContract>(
                    serviceContext,
                    remotingService,
                    listenerBinding,
                    address);
            }
            else
            {
                wcfListener = new WcfCommunicationListener<IServiceRemotingContract>(
                    serviceContext,
                    remotingService,
                    listenerBinding,
                    endpointResourceName);
            }
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

        private void DisposeIfNeeded()
        {
            if (this.messageHandler is IDisposable disposableItem)
            {
                disposableItem.Dispose();
            }
        }

        private static IServiceRemotingMessageSerializationProvider GetDefaultSerializationProvider(IServiceRemotingMessageSerializationProvider serializationProvider, bool useWrappedMessage)
        {
            if (serializationProvider == null)
            {
                if (useWrappedMessage)
                {
                    return new WrappingServiceRemotingDataContractSerializationProvider(null);
                }

                serializationProvider = new ServiceRemotingDataContractSerializationProvider(null);
            }

            return serializationProvider;
        }
    }
}
