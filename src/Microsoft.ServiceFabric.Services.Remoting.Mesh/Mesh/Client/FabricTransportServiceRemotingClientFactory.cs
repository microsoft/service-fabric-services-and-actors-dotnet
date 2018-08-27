// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Mesh.Client
{
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport.V2;
    using Microsoft.ServiceFabric.FabricTransport.V2.Client;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.FabricTransport.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Messaging;
    using Microsoft.ServiceFabric.Services.Remoting.Mesh;

    /// <inheritdoc />
    public class FabricTransportServiceRemotingClientFactory : IServiceRemotingClientFactory, IDisposable
    {
        private readonly string endpoint;
        private readonly FabricTransportRemotingMeshClientSettings remotingClientSettings;
        private NativeFabricTransport.IFabricTransportMessageDisposer disposer;
        private IFabricTransportCallbackMessageHandler fabricTransportRemotingCallbackMessageHandler;
        private ServiceRemotingMessageSerializationManager serializersManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingClientFactory"/> class.
        /// </summary>
        /// <param name="endpoint">endpoint</param>
        /// <param name="remotingCallbackMessageHandler">remotingCallbackMessageHandler</param>
        public FabricTransportServiceRemotingClientFactory(string endpoint, IServiceRemotingCallbackMessageHandler remotingCallbackMessageHandler = null)
        {
            this.endpoint = endpoint;
            this.remotingClientSettings = new FabricTransportRemotingMeshClientSettings();
            this.remotingClientSettings.UseWrappedMessage = true;
            this.disposer = new NativeFabricTransportMessageDisposer();
            var headerSerializer = new ServiceRemotingMessageHeaderSerializer(
                new BufferPoolManager());
            this.serializersManager = new ServiceRemotingSerializationManager(
                null,
                headerSerializer,
                true);
            this.fabricTransportRemotingCallbackMessageHandler = new FabricTransportRemotingCallbackMessageHandler(remotingCallbackMessageHandler, this.serializersManager);
        }
#pragma warning disable 67

        /// <inheritdoc/>
        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientConnected;

        /// <inheritdoc />
        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientDisconnected;
#pragma warning restore  67

        /// <inheritdoc/>
        public async Task<IServiceRemotingClient> GetClientAsync(
            Uri serviceUri,
            ServicePartitionKey partitionKey,
            TargetReplicaSelector targetReplicaSelector,
            string listenerName,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            var remotingHandler = new FabricTransportRemotingClientEventHandler();
            var nativeClient = new FabricTransportClient(
                this.remotingClientSettings.GetInternalSettings(),
                this.endpoint,
                remotingHandler,
                this.fabricTransportRemotingCallbackMessageHandler,
                this.disposer);
            var client = new FabricTransportServiceRemotingClient(
                this.serializersManager,
                nativeClient,
                remotingHandler);
            await client.OpenAsync(CancellationToken.None);
            return client;
        }

        /// <inheritdoc/>
        public async Task<IServiceRemotingClient> GetClientAsync(
            ResolvedServicePartition previousRsp,
            TargetReplicaSelector targetReplicaSelector,
            string listenerName,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            var remotingHandler = new FabricTransportRemotingClientEventHandler();
            var nativeClient = new FabricTransportClient(
                this.remotingClientSettings.GetInternalSettings(),
                this.endpoint,
                remotingHandler,
                this.fabricTransportRemotingCallbackMessageHandler,
                this.disposer);
            var client = new FabricTransportServiceRemotingClient(
                this.serializersManager,
                nativeClient,
                remotingHandler);
            await client.OpenAsync(CancellationToken.None);
            return client;
        }

        /// <inheritdoc/>
        public Task<OperationRetryControl> ReportOperationExceptionAsync(
            IServiceRemotingClient client,
            ExceptionInformation exceptionInformation,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
            return new WrappedRequestMessageFactory();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
