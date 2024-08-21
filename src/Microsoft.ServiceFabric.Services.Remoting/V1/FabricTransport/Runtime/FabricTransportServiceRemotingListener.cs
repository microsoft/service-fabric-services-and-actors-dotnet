// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Runtime
{
    using System;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Runtime;

    /// <summary>
    ///     An <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.IServiceRemotingListener"/> that uses
    ///     fabric TCP transport to provide interface remoting for stateless and stateful services.
    /// </summary>
    public class FabricTransportServiceRemotingListener : IServiceRemotingListener
    {
        private FabricTransportListener nativeListener;
        private IServiceRemotingMessageHandler messageHandler;
        private string listenAddress;
        private string publishAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingListener"/> class.
        ///     Constructs a fabric transport based service remoting listener with default
        ///     <see cref="FabricTransportRemotingListenerSettings"/>.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="serviceImplementation">
        ///     The service implementation object used to construct <see cref="ServiceRemotingDispatcher"/>
        ///     for message processing.
        /// </param>
        public FabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation)
            : this(
                serviceContext,
                serviceImplementation,
                listenerSettings: FabricTransportRemotingListenerSettings.GetDefault())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingListener"/> class.
        ///     Constructs a fabric transport based service remoting listener with <see cref="FabricTransportRemotingListenerSettings"/>
        ///     loaded from configuration section.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="serviceImplementation">
        ///     The service implementation object used to construct <see cref="ServiceRemotingDispatcher"/>
        ///     for message processing.
        /// </param>
        /// <param name="listenerSettingsConfigSectionName">
        ///    The name of the configuration section in the configuration package named
        ///    "Config" in the service manifest that defines the settings for the listener.
        /// </param>
        public FabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation,
            string listenerSettingsConfigSectionName)
            : this(
                serviceContext,
                new ServiceRemotingDispatcher(serviceContext, serviceImplementation),
                FabricTransportRemotingListenerSettings.LoadFrom(listenerSettingsConfigSectionName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingListener"/> class.
        ///     Constructs a fabric transport based service remoting listener with the specified
        ///     <see cref="FabricTransportRemotingListenerSettings"/>.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="serviceImplementation">
        ///     The service implementation object used to construct <see cref="ServiceRemotingDispatcher"/>
        ///     for message processing.
        /// </param>
        /// <param name="listenerSettings">
        ///     The settings for the listener.
        /// </param>
        public FabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation,
            FabricTransportRemotingListenerSettings listenerSettings)
            : this(
                serviceContext,
                new ServiceRemotingDispatcher(serviceContext, serviceImplementation),
                listenerSettings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingListener"/> class.
        ///     Constructs a fabric transport based service remoting listener with default
        ///     <see cref="FabricTransportRemotingListenerSettings"/>.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="messageHandler">
        ///     The handler for processing remoting messages. As the messages are received,
        ///     the listener delivers them to this handler.
        /// </param>
        public FabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler)
            : this(serviceContext, messageHandler, FabricTransportRemotingListenerSettings.GetDefault())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingListener"/> class.
        ///     Constructs a fabric transport based service remoting listener with <see cref="FabricTransportRemotingListenerSettings"/>
        ///     loaded from configuration section.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="messageHandler">
        ///     The handler for processing remoting messages. As the messages are received,
        ///     the listener delivers them to this handler.
        /// </param>
        /// <param name="listenerSettingsConfigSectionName">
        ///    The name of the configuration section in the configuration package named
        ///    "Config" in the service manifest that defines the settings for the listener.
        /// </param>
        public FabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            string listenerSettingsConfigSectionName)
            : this(
                serviceContext,
                messageHandler,
                FabricTransportRemotingListenerSettings.LoadFrom(listenerSettingsConfigSectionName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingListener"/> class.
        ///     Constructs a fabric transport based service remoting listener with the specified
        ///     <see cref="FabricTransportRemotingListenerSettings"/>.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="messageHandler">
        ///     The handler for processing remoting messages. As the messages are received,
        ///     the listener delivers them to this handler.
        /// </param>
        /// <param name="listenerSettings">
        ///     The settings to use for the listener.
        /// </param>
        public FabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            FabricTransportRemotingListenerSettings listenerSettings)
        {
            this.messageHandler = messageHandler;
            this.nativeListener = this.CreateNativeListener(
                listenerSettings,
                this.messageHandler,
                serviceContext);
            this.listenAddress = serviceContext.ListenAddress;
            this.publishAddress = serviceContext.PublishAddress;

            ServiceTelemetry.FabricTransportServiceRemotingV1Event(
                serviceContext,
                !listenerSettings.SecurityCredentials.CredentialType.Equals(CredentialType.None),
                FabricTransportRemotingListenerSettings.ExceptionSerialization.BinaryFormatter.ToString());
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
            return Task.Run(
                async () =>
            {
                var listenUri = await this.nativeListener.OpenAsync(cancellationToken);
                var publishUri = listenUri.Replace(this.listenAddress, this.publishAddress);

                AppTrace.TraceSource.WriteInfo("FabricTransportServiceRemotingListener.OpenAsync", "ListenURI = {0} PublishURI = {1}", listenUri, publishUri);

                return publishUri;
            }, cancellationToken);
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
        async Task ICommunicationListener.CloseAsync(CancellationToken cancellationToken)
        {
            await this.nativeListener.CloseAsync(cancellationToken);
            this.Dispose();
        }

        /// <summary>
        /// This method causes the communication listener to close. Close is a terminal state and
        /// this method causes the transition to close ungracefully. Any outstanding operations
        /// (including close) should be canceled when this method is called.
        /// </summary>
        void ICommunicationListener.Abort()
        {
            if (this.nativeListener != null)
            {
                this.nativeListener.Abort();
                this.Dispose();
            }
        }

        private FabricTransportListener CreateNativeListener(
            FabricTransportRemotingListenerSettings listenerSettings,
            IServiceRemotingMessageHandler messageHandler,
            ServiceContext serviceContext)
        {
            var connectionHandler = new FabricTransportServiceRemotingConnectionHandler();
            return new FabricTransportListener(
                listenerSettings.GetInternalSettings(),
                listenerSettings.GetInternalSettings().GetListenerAddress(serviceContext),
                new FabricTransportMessagingHandler(messageHandler),
                connectionHandler);
        }

        private void Dispose()
        {
            if (this.nativeListener != null)
            {
                this.nativeListener.Dispose();
                this.nativeListener = null;
            }

            if (this.messageHandler is IDisposable disposableItem)
            {
                disposableItem.Dispose();
            }
        }
    }
}
