// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Mesh.Runtime
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.Base;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Messaging;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Mesh.FabricTransport.Runtime;

    /// <inheritdoc />
    public class FabricTransportServiceRemotingListener : IServiceRemotingListener
    {
        private static readonly string DefaultWrappedMessageListenerEndpointResourceName = "ServiceEndpointV2_1";
        private Remoting.Base.V2.FabricTransport.Runtime.FabricTransportServiceRemotingListener remotingListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingListener"/> class.
        ///     Constructs a fabric transport based service remoting listener.
        /// </summary>
        /// <param name="partitionId">Partition Id</param>
        /// <param name="serviceImplementation">serviceImplementation</param>
        /// <param name="serializationProvider">It is used to serialize deserialize request and response body </param>
        /// <param name="remotingListenerClientSettings">The settings for the listener</param>
        public FabricTransportServiceRemotingListener(
            Guid partitionId,
            IService serviceImplementation,
            FabricTransportRemotingMeshListenerSettings remotingListenerClientSettings = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null)
        {
            var remotingSettings = remotingListenerClientSettings ?? new FabricTransportRemotingMeshListenerSettings();
            remotingSettings.EndpointResourceName = DefaultWrappedMessageListenerEndpointResourceName;
            remotingSettings.UseWrappedMessage = true;
            var path = string.Format(CultureInfo.InvariantCulture, "{0}", partitionId);
            var address = new Microsoft.ServiceFabric.FabricTransport.Runtime.FabricTransportListenerAddress(
                "localhost",
                this.GetPublishPort(remotingSettings.EndpointResourceName),
                path);
            var serviceRemotingMessageHandler = new ServiceRemotingMessageDispatcher(
                partitionId,
                0L,
                Guid.NewGuid().ToString(),
                serviceImplementation,
                GetMessageBodyFactory(serializationProvider, remotingListenerClientSettings));

            this.remotingListener = new Base.V2.FabricTransport.Runtime.FabricTransportServiceRemotingListener(
                address.ToString(),
                address.ToString(),
                partitionId,
                0L,
                serviceRemotingMessageHandler,
                address,
                remotingSettings.GetInternalSettings(),
                InitializeSerializersManager(serializationProvider, remotingListenerClientSettings));
        }

        /// <inheritdoc/>
        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            return this.remotingListener.OpenAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return this.remotingListener.CloseAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public void Abort()
        {
           this.remotingListener.Abort();
        }

        private static IServiceRemotingMessageBodyFactory GetMessageBodyFactory(IServiceRemotingMessageSerializationProvider serializationProvider, FabricTransportRemotingMeshListenerSettings remotingListenerClientSettings)
        {
            if (serializationProvider != null)
            {
                return serializationProvider.CreateMessageBodyFactory();
            }

            return new WrappedRequestMessageFactory();
        }

        private static ServiceRemotingMessageSerializationManager InitializeSerializersManager(
            IServiceRemotingMessageSerializationProvider serializationProvider,
            FabricTransportRemotingMeshListenerSettings listenerClientSettings)
        {
            listenerClientSettings = listenerClientSettings ??
                               new FabricTransportRemotingMeshListenerSettings();

            return new ServiceRemotingSerializationManager(
                serializationProvider,
                new ServiceRemotingMessageHeaderSerializer(
                    new BufferPoolManager(
                        listenerClientSettings.HeaderBufferSize,
                        listenerClientSettings.HeaderMaxBufferCount)),
                listenerClientSettings.UseWrappedMessage);
        }

        private int GetPublishPort(string endpointName)
        {
            string portEnv = Environment.GetEnvironmentVariable(endpointName);
            if (!string.IsNullOrEmpty(portEnv))
            {
                Console.WriteLine("{0} Port is ={1}", endpointName, portEnv);
                int port;

                if (int.TryParse(portEnv, out port))
                {
                    return port;
                }
            }

            // Otherwise use the same listening port
            return 0;
        }
    }
}
