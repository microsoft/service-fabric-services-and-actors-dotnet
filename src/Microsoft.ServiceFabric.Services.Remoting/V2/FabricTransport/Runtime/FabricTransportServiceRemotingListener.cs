// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport.V2.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    /// <summary>
    ///     An <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.IServiceRemotingListener"/> that uses
    ///     fabric TCP transport to provide interface remoting for stateless and stateful services.
    /// </summary>
    public class FabricTransportServiceRemotingListener : IServiceRemotingListener
    {
        private static readonly string DefaultV2ListenerEndpointResourceName = "ServiceEndpointV2";
        private static readonly string DefaultWrappedMessageListenerEndpointResourceName = "ServiceEndpointV2_1";
        private readonly FabricTransportMessageHandler transportMessageHandler;
        private readonly string listenAddress;
        private readonly string publishAddress;
        private FabricTransportListener fabricTransportlistener;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingListener"/> class.
        ///     Constructs a fabric transport based service remoting listener .
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="serviceImplementation">
        ///     The service implementation object used to construct <see cref="ServiceRemotingMessageDispatcher"/>
        ///     for message processing.
        /// </param>
        /// <param name="serializationProvider">It is used to serialize deserialize request and response body </param>
        /// <param name="remotingListenerSettings">The settings for the listener</param>
        /// <param name="exceptionConvertors">Convertors to convert user exception to service exception.</param>
        public FabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation,
            FabricTransportRemotingListenerSettings remotingListenerSettings = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null)
            : this(
                  serviceContext,
                  new ServiceRemotingMessageDispatcher(
                    serviceContext,
                    serviceImplementation,
                    GetMessageBodyFactory(serializationProvider, remotingListenerSettings)),
                  remotingListenerSettings,
                  serializationProvider,
                  exceptionConvertors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingListener"/> class.
        ///     Constructs a fabric transport based service remoting listener.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="serviceRemotingMessageHandler">
        ///     The handler for processing remoting messages. As the messages are received,
        ///     the listener delivers them to this handler.
        /// </param>
        /// <param name="serializationProvider">It is used to serialize deserialize request and response body </param>
        /// <param name="remotingListenerSettings">The settings for the listener</param>
        /// <param name="exceptionConvertors">Covertors to convert user exception to service exception.</param>
        public FabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler serviceRemotingMessageHandler,
            FabricTransportRemotingListenerSettings remotingListenerSettings = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null) // TODO Check existing usage
            : this(
                serviceContext,
                serviceRemotingMessageHandler,
                InitializeSerializersManager(serializationProvider, remotingListenerSettings),
                remotingListenerSettings,
                exceptionConvertors)
        {
        }

        internal FabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler serviceRemotingMessageHandler,
            ServiceRemotingMessageSerializersManager serializersManager,
            FabricTransportRemotingListenerSettings remotingListenerSettings = null,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null)
        {
            Requires.ThrowIfNull(serviceContext, "serviceContext");

            this.listenAddress = serviceContext.ListenAddress;
            this.publishAddress = serviceContext.PublishAddress;

            var remotingSettings = remotingListenerSettings ?? FabricTransportRemotingListenerSettings.GetDefault();

            if (remotingSettings.EndpointResourceName.Equals(FabricTransportRemotingListenerSettings
                .DefaultEndpointResourceName))
            {
                if (remotingSettings.UseWrappedMessage)
                {
                    remotingSettings.EndpointResourceName = DefaultWrappedMessageListenerEndpointResourceName;
                }
                else
                {
                    remotingSettings.EndpointResourceName = DefaultV2ListenerEndpointResourceName;
                }
            }

            var svcExceptionConvertors = new List<IExceptionConvertor>();
            if (exceptionConvertors != null)
            {
                svcExceptionConvertors.AddRange(exceptionConvertors);
            }

            svcExceptionConvertors.Add(new FabricExceptionConvertor());
            svcExceptionConvertors.Add(new SystemExceptionConvertor());
            svcExceptionConvertors.Add(new ExceptionConvertorHelper.DefaultExceptionConvetor());

            this.transportMessageHandler = new FabricTransportMessageHandler(
                serviceRemotingMessageHandler,
                serializersManager,
                new ExceptionConvertorHelper(svcExceptionConvertors, remotingSettings),
                serviceContext.PartitionId,
                serviceContext.ReplicaOrInstanceId);

            this.fabricTransportlistener = new FabricTransportListener(
                remotingSettings.GetInternalSettings(),
                remotingSettings.GetInternalSettings().GetListenerAddress(serviceContext),
                this.transportMessageHandler,
                new FabricTransportRemotingConnectionHandler());

            ServiceTelemetry.FabricTransportServiceRemotingV2Event(
                serviceContext,
                !remotingSettings.SecurityCredentials.CredentialType.Equals(CredentialType.None));
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
        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            return Task.Run(
                async () =>
                {
                    var listenUri = await this.fabricTransportlistener.OpenAsync(cancellationToken);
                    var publishUri = listenUri.Replace(this.listenAddress, this.publishAddress);

                    System.Fabric.Common.AppTrace.TraceSource.WriteInfo(
                        "FabricTransportServiceRemotingListenerV2.OpenAsync",
                        "ListenURI = {0} PublishURI = {1}",
                        listenUri,
                        publishUri);

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
        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            await this.fabricTransportlistener.CloseAsync(
                cancellationToken);
            this.Dispose();
        }

        /// <summary>
        /// This method causes the communication listener to close. Close is a terminal state and
        /// this method causes the transition to close ungracefully. Any outstanding operations
        /// (including close) should be canceled when this method is called.
        /// </summary>
        public void Abort()
        {
            this.fabricTransportlistener.Abort();
            this.Dispose();
        }

        private static ServiceRemotingMessageSerializersManager InitializeSerializersManager(
            IServiceRemotingMessageSerializationProvider serializationProvider,
            FabricTransportRemotingListenerSettings listenerSettings)
        {
            listenerSettings = listenerSettings ??
                FabricTransportRemotingListenerSettings.GetDefault();

            return new ServiceRemotingMessageSerializersManager(
                serializationProvider,
                new ServiceRemotingMessageHeaderSerializer(
                new BufferPoolManager(
                listenerSettings.HeaderBufferSize,
                listenerSettings.HeaderMaxBufferCount)),
                listenerSettings.UseWrappedMessage);
        }

        private static IServiceRemotingMessageBodyFactory GetMessageBodyFactory(IServiceRemotingMessageSerializationProvider serializationProvider, FabricTransportRemotingListenerSettings remotingListenerSettings)
        {
            if (serializationProvider != null)
            {
                return serializationProvider.CreateMessageBodyFactory();
            }

            if (remotingListenerSettings != null && remotingListenerSettings.UseWrappedMessage)
            {
                return new WrappedRequestMessageFactory();
            }

            return new DataContractRemotingMessageFactory();
        }

        private void Dispose()
        {
            if (this.fabricTransportlistener != null)
            {
                this.fabricTransportlistener.Dispose();
                this.fabricTransportlistener = null;
            }

            this.transportMessageHandler.Dispose();
        }
    }
}
