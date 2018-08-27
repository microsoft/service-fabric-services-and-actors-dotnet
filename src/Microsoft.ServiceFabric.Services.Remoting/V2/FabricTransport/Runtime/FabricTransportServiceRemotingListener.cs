// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime
{
    using System.Fabric;
    using System.Fabric.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.Base;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Messaging;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

    /// <summary>
    ///     An <see cref="IServiceRemotingListener"/> that uses
    ///     fabric TCP transport to provide interface remoting for stateless and stateful services.
    /// </summary>
    public class FabricTransportServiceRemotingListener : IServiceRemotingListener
    {
        private static readonly string DefaultV2ListenerEndpointResourceName = "ServiceEndpointV2";
        private static readonly string DefaultWrappedMessageListenerEndpointResourceName = "ServiceEndpointV2_1";
        private Base.V2.FabricTransport.Runtime.FabricTransportServiceRemotingListener fabricTransportlistener;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingListener"/> class.
        ///     Constructs a fabric transport based service remoting listener .
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="serviceImplementation">
        ///     The service implementation object used to construct <see cref="Base.V2.Runtime.ServiceRemotingMessageDispatcher"/>
        ///     for message processing.
        /// </param>
        /// <param name="serializationProvider">It is used to serialize deserialize request and response body </param>
        /// <param name="remotingListenerSettings">The settings for the listener</param>
        public FabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation,
            FabricTransportRemotingListenerSettings remotingListenerSettings = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null)
            : this(
                  serviceContext,
                  new V2.Runtime.ServiceRemotingMessageDispatcher(
                    serviceContext,
                    serviceImplementation,
                    GetMessageBodyFactory(serializationProvider, remotingListenerSettings)),
                  remotingListenerSettings,
                  serializationProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingListener"/> class.
        ///     Constructs a fabric transport based service remoting listener.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="serviceRemotingMessageHandler">serviceRemotingMessageHandler</param>
        /// <param name="serializationProvider">It is used to serialize deserialize request and response body </param>
        /// <param name="remotingListenerSettings">The settings for the listener</param>
        public FabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler serviceRemotingMessageHandler,
            FabricTransportRemotingListenerSettings remotingListenerSettings = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null)
            : this(
                serviceContext,
                serviceRemotingMessageHandler,
                InitializeSerializersManager(serializationProvider, remotingListenerSettings),
                remotingListenerSettings)
        {
        }

        internal FabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler serviceRemotingMessageHandler,
            ServiceRemotingMessageSerializationManager serializersManager,
            FabricTransportRemotingListenerSettings remotingListenerSettings = null)
        {
            Requires.ThrowIfNull(serviceContext, "serviceContext");
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

            this.fabricTransportlistener = new Base.V2.FabricTransport.Runtime.FabricTransportServiceRemotingListener(
                serviceContext.PublishAddress,
                serviceContext.ListenAddress,
                serviceContext.PartitionId,
                serviceContext.ReplicaOrInstanceId,
                serviceRemotingMessageHandler,
                remotingSettings.GetInternalSettings().GetListenerAddress(serviceContext),
                remotingSettings.GetInternalSettings(),
                serializersManager);
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
                   return await this.fabricTransportlistener.OpenAsync(cancellationToken);
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
        }

        /// <summary>
        /// This method causes the communication listener to close. Close is a terminal state and
        /// this method causes the transition to close ungracefully. Any outstanding operations
        /// (including close) should be canceled when this method is called.
        /// </summary>
        public void Abort()
        {
            this.fabricTransportlistener.Abort();
        }

        private static ServiceRemotingSerializationManager InitializeSerializersManager(
            IServiceRemotingMessageSerializationProvider serializationProvider,
            FabricTransportRemotingListenerSettings listenerSettings)
        {
            listenerSettings = listenerSettings ??
                FabricTransportRemotingListenerSettings.GetDefault();

            return new Remoting.V2.ServiceRemotingSerializationManager(
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
    }
}
