// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    /// <summary>
    /// An <see cref="IServiceRemotingClientFactory"/> that uses
    /// Fabric TCP transport to create <see cref="IServiceRemotingClient"/> that communicate with stateless
    /// and stateful services over interfaces that are remoted via
    /// <see cref="FabricTransportServiceRemotingListener"/>.
    /// </summary>
    public class FabricTransportServiceRemotingClientFactory : IServiceRemotingClientFactory
    {
        private FabricTransportServiceRemotingClientFactoryImpl clientFactoryImpl;
        private IServiceRemotingMessageBodyFactory remotingMessageBodyFactory = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingClientFactory"/> class.
        ///     Constructs a fabric transport based service remoting client factory.
        /// </summary>
        /// <param name="remotingSettings">
        ///     The settings for the fabric transport. If the settings are not provided or null, default settings
        ///     with no security.
        /// </param>
        /// <param name="remotingCallbackMessageHandler">
        ///     The callback client that receives the callbacks from the service.
        /// </param>
        /// <param name="servicePartitionResolver">
        ///     Service partition resolver to resolve the service endpoints. If not specified, a default
        ///     service partition resolver returned by <see cref="ServicePartitionResolver.GetDefault"/> is used.
        /// </param>
        /// <param name="exceptionHandlers">
        ///     Exception handlers to handle the exceptions encountered in communicating with the service.
        /// </param>
        /// <param name="traceId">
        ///     Id to use in diagnostics traces from this component.
        /// </param>
        /// <param name="serializationProvider">
        /// Serialization Provider to serialize and deserialize request and response.</param>
        /// <param name="exceptionConvertors">
        ///     Convertors to convert service exception to user exception.
        /// </param>
        /// <remarks>
        ///     This factory uses an internal fabric transport exception handler to handle exceptions at the fabric TCP transport
        ///     level and a <see cref="ServiceRemotingExceptionHandler"/>, in addition to the exception handlers supplied to the
        ///     constructor.
        /// </remarks>
        public FabricTransportServiceRemotingClientFactory(
            FabricTransportRemotingSettings remotingSettings = null,
            IServiceRemotingCallbackMessageHandler remotingCallbackMessageHandler = null,
            IServicePartitionResolver servicePartitionResolver = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            string traceId = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null) // Check existing usage
        {
            this.Initialize(
                remotingSettings,
                remotingCallbackMessageHandler,
                servicePartitionResolver,
                exceptionHandlers,
                exceptionConvertors,
                traceId,
                serializationProvider);
        }

        internal FabricTransportServiceRemotingClientFactory(
            ServiceRemotingMessageSerializersManager serializersManager,
            FabricTransportRemotingSettings remotingSettings = null,
            IServiceRemotingCallbackMessageHandler remotingCallbackMessageHandler = null,
            IServicePartitionResolver servicePartitionResolver = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null,
            string traceId = null)
        {
            this.Initialize(
                remotingSettings,
                remotingCallbackMessageHandler,
                servicePartitionResolver,
                exceptionHandlers,
                exceptionConvertors,
                traceId,
                serializersManager.GetSerializationProvider().CreateMessageBodyFactory(),
                serializersManager);
        }

        /// <summary>
        /// Event handler that is fired when a client is connected to the service endpoint.
        /// </summary>
        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientConnected;

        /// <summary>
        /// Event handler that is fired when a client is disconnected from the service endpoint.
        /// </summary>
        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientDisconnected;

        /// <summary>
        /// Resolves a partition of the specified service containing one or more communication listeners and returns a client to communicate
        /// to the endpoint corresponding to the given listenerName.
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
        public async Task<IServiceRemotingClient> GetClientAsync(
            Uri serviceUri,
            ServicePartitionKey partitionKey,
            TargetReplicaSelector targetReplicaSelector,
            string listenerName,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            return await this.clientFactoryImpl.GetClientAsync(
                serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName,
                retrySettings,
                cancellationToken);
        }

        /// <summary>
        /// Re-resolves a partition of the specified service containing one or more communication listeners and returns a client to communicate
        /// to the endpoint corresponding to the given listenerName.
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
        public async Task<IServiceRemotingClient> GetClientAsync(
            ResolvedServicePartition previousRsp,
            TargetReplicaSelector targetReplicaSelector,
            string listenerName,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            return await this.clientFactoryImpl.GetClientAsync(
                previousRsp,
                targetReplicaSelector,
                listenerName,
                retrySettings,
                cancellationToken);
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
        public Task<OperationRetryControl> ReportOperationExceptionAsync(
            IServiceRemotingClient client,
            ExceptionInformation exceptionInformation,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            return this.clientFactoryImpl.ReportOperationExceptionAsync(
                (FabricTransportServiceRemotingClient)client,
                exceptionInformation,
                retrySettings,
                cancellationToken);
        }

        /// <summary>
        /// Gets a factory for creating the remoting message bodies.
        /// </summary>
        /// <returns>A factory for creating the remoting message bodies</returns>
        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
            return this.remotingMessageBodyFactory;
        }

        /// <summary>
        /// Releases managed/unmanaged resources.
        /// Dispose Method is being added rather than making it IDisposable so that it doesn't change type information and wont be a breaking change.
        /// </summary>
        public void Dispose()
        {
            this.clientFactoryImpl.Dispose();
        }

        private void Initialize(
            FabricTransportRemotingSettings remotingSettings,
            IServiceRemotingCallbackMessageHandler remotingCallbackMessageHandler,
            IServicePartitionResolver servicePartitionResolver,
            IEnumerable<IExceptionHandler> exceptionHandlers,
            IEnumerable<IExceptionConvertor> exceptionConvertors,
            string traceId,
            IServiceRemotingMessageSerializationProvider serializationProvider,
            IServiceRemotingMessageHeaderSerializer headerSerializer = null)
        {
            remotingSettings = remotingSettings ?? FabricTransportRemotingSettings.GetDefault();

            if (headerSerializer == null)
            {
                headerSerializer = new ServiceRemotingMessageHeaderSerializer(new BufferPoolManager(remotingSettings.HeaderBufferSize, remotingSettings.HeaderMaxBufferCount));
            }

            var serializersManager = new ServiceRemotingMessageSerializersManager(
                serializationProvider,
                headerSerializer,
                remotingSettings.UseWrappedMessage);

            this.Initialize(
                remotingSettings,
                remotingCallbackMessageHandler,
                servicePartitionResolver,
                exceptionHandlers,
                exceptionConvertors,
                traceId,
                serializersManager.GetSerializationProvider().CreateMessageBodyFactory(),
                serializersManager);
        }

        private void Initialize(
            FabricTransportRemotingSettings remotingSettings,
            IServiceRemotingCallbackMessageHandler remotingCallbackMessageHandler,
            IServicePartitionResolver servicePartitionResolver,
            IEnumerable<IExceptionHandler> exceptionHandlers,
            IEnumerable<IExceptionConvertor> exceptionConvertors,
            string traceId,
            IServiceRemotingMessageBodyFactory messageBodyFactory,
            ServiceRemotingMessageSerializersManager serializersManager)
        {
            this.remotingMessageBodyFactory = messageBodyFactory;
            this.clientFactoryImpl = new FabricTransportServiceRemotingClientFactoryImpl(
                serializersManager,
                remotingSettings,
                remotingCallbackMessageHandler,
                servicePartitionResolver,
                exceptionHandlers,
                exceptionConvertors,
                traceId);
            this.clientFactoryImpl.ClientConnected += this.OnClientConnected;
            this.clientFactoryImpl.ClientDisconnected += this.OnClientDisconnected;
        }

        private void OnClientConnected(
            object sender,
            CommunicationClientEventArgs<FabricTransportServiceRemotingClient> e)
        {
            var handlers = this.ClientConnected;
            if (handlers != null)
            {
                handlers(
                    this,
                    new CommunicationClientEventArgs<IServiceRemotingClient>()
                    {
                        Client = e.Client,
                    });
            }
        }

        private void OnClientDisconnected(
            object sender,
            CommunicationClientEventArgs<FabricTransportServiceRemotingClient> e)
        {
            var handlers = this.ClientDisconnected;
            if (handlers != null)
            {
                handlers(
                    this,
                    new CommunicationClientEventArgs<IServiceRemotingClient>()
                    {
                        Client = e.Client,
                    });
            }
        }
    }
}
