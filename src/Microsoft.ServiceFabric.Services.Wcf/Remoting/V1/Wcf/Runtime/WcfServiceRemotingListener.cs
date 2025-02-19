// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Wcf.Runtime
{
    using System;
    using System.Fabric;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Wcf;
    using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V1;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Runtime;

    /// <summary>
    /// An <see cref="IServiceRemotingListener"/> that uses
    /// Windows Communication Foundation to provide interface remoting for stateless and stateful services.
    /// </summary>
    [Obsolete("This class is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
    public class WcfServiceRemotingListener : IServiceRemotingListener
    {
        private readonly IServiceRemotingMessageHandler messageHandler;
        private ICommunicationListener wcfListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="serviceImplementation">The service implementation object.</param>
        /// <param name="listenerBinding">WCF binding to use for the listener. If the listener binding is not specified or null,
        /// a default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method which creates
        /// a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="endpointResourceName">The name of the endpoint resource defined in the service manifest that
        /// should be used to create the address for the listener. If the endpointResourceName is not specified or null,
        /// the default value "ServiceEndpoint" is used.
        /// </param>
        public WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation,
            Binding listenerBinding = null,
            string endpointResourceName = "ServiceEndpoint")
        {
            this.messageHandler = new ServiceRemotingDispatcher(serviceContext, serviceImplementation);
            this.wcfListener = new WcfCommunicationListener<IServiceRemotingContract>(
                serviceContext,
                new WcfRemotingService(
                    this.messageHandler),
                listenerBinding,
                endpointResourceName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="messageHandler">The handler for receiving and processing remoting messages. As the messages are received
        /// the listener delivers the messages to the handler.
        /// </param>
        /// <param name="listenerBinding">WCF binding to use for the listener. If the listener binding is not specified or null,
        /// a default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method which creates
        /// a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="endpointResourceName">The name of the endpoint resource defined in the service manifest that
        /// should be used to create the address for the listener. If the endpointResourceName is not specified or it is null,
        /// the default value "ServiceEndpoint" is used.
        /// </param>
        public WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            Binding listenerBinding = null,
            string endpointResourceName = "ServiceEndpoint")
        {
            this.messageHandler = messageHandler;
            this.wcfListener = new WcfCommunicationListener<IServiceRemotingContract>(
                serviceContext,
                new WcfRemotingService(this.messageHandler),
                listenerBinding,
                endpointResourceName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="messageHandler">The handler for receiving and processing remoting messages. As the messages are received
        /// the listener delivers the messages to the handler.
        /// </param>
        /// <param name="listenerBinding">WCF binding to use for the listener. If the listener binding is not specified or null,
        /// a default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method.
        /// </param>
        /// <param name="address">The endpoint address to use for the WCF listener. If not specified or null, the endpoint
        /// address is created using the default endpoint resource named "ServiceEndpoint" defined in the service manifest.
        /// </param>
        public WcfServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            Binding listenerBinding = null,
            EndpointAddress address = null)
        {
            this.messageHandler = messageHandler;
            if (address != null)
            {
                this.wcfListener = new WcfCommunicationListener<IServiceRemotingContract>(
                    serviceContext,
                    new WcfRemotingService(this.messageHandler),
                    listenerBinding,
                    address);
            }
            else
            {
                this.wcfListener = new WcfCommunicationListener<IServiceRemotingContract>(
                    serviceContext,
                    new WcfRemotingService(this.messageHandler),
                    listenerBinding,
                    "ServiceEndpoint");
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

        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        private class WcfRemotingService : IServiceRemotingContract
        {
            private readonly IServiceRemotingMessageHandler messageHandler;

            // The request context need not be generated every time for WCF because for WCF,
            // the actual callback channel is accessed from the current operation context.
            private readonly WcfServiceRemotingRequestContext requestContext;

            public WcfRemotingService(IServiceRemotingMessageHandler messageHandler)
            {
                this.messageHandler = messageHandler;
                this.requestContext = new WcfServiceRemotingRequestContext();
            }

            public async Task<byte[]> RequestResponseAsync(ServiceRemotingMessageHeaders headers, byte[] requestBody)
            {
                try
                {
                    return await this.messageHandler.RequestResponseAsync(
                        this.requestContext,
                        headers,
                        requestBody);
                }
                catch (Exception e)
                {
                    throw new FaultException<RemoteExceptionInformation>(RemoteExceptionInformation.FromException(e));
                }
            }

            public void OneWayMessage(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
            {
                this.messageHandler.HandleOneWay(this.requestContext, messageHeaders, requestBody);
            }
        }
    }
}
