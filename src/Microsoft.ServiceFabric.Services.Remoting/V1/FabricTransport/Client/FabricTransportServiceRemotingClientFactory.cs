// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Client
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
    using Microsoft.ServiceFabric.Services.Remoting.V1.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Runtime;

    /// <summary>
    /// An <see cref="IServiceRemotingClientFactory"/> that uses
    /// Fabric TCP transport to create <see cref="IServiceRemotingClient"/> that communicate with stateless
    /// and stateful services over interfaces that are remoted via
    /// <see cref="FabricTransportServiceRemotingListener"/>.
    /// </summary>
    public class FabricTransportServiceRemotingClientFactory : IServiceRemotingClientFactory
    {
        private readonly FabricTransportServiceRemotingClientFactoryImpl impl;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingClientFactory"/> class.
        ///     Constructs a fabric transport based service remoting client factory.
        /// </summary>
        /// <param name="fabricTransportRemotingSettings">
        ///     The settings for the fabric transport. If the settings are not provided or null, default settings
        ///     with no security.
        /// </param>
        /// <param name="callbackClient">
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
        /// <remarks>
        ///     This factory uses an internal fabric transport exception handler to handle exceptions at the fabric TCP transport
        ///     level and a <see cref="ServiceRemotingExceptionHandler"/>, in addition to the exception handlers supplied to the
        ///     constructor.
        /// </remarks>
        public FabricTransportServiceRemotingClientFactory(
            FabricTransportRemotingSettings fabricTransportRemotingSettings = null,
            IServiceRemotingCallbackClient callbackClient = null,
            IServicePartitionResolver servicePartitionResolver = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            string traceId = null)
        {
            if (traceId == null)
            {
                traceId = Guid.NewGuid().ToString();
            }

            this.impl = new FabricTransportServiceRemotingClientFactoryImpl(
                fabricTransportRemotingSettings,
                callbackClient,
                servicePartitionResolver,
                GetExceptionHandlers(exceptionHandlers, traceId),
                traceId);
            this.impl.FabricTransportClientConnected += this.OnClientConnected;
            this.impl.FabricTransportClientDisconnected += this.OnClientDisconnected;
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
        /// Dispose Method is being added rather than making it IDisposable so that it doesn't change type information and wont be a breaking change.
        /// </summary>
        public void Dispose()
        {
            this.impl.Dispose();
        }

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
        async Task<IServiceRemotingClient> ICommunicationClientFactory<IServiceRemotingClient>.GetClientAsync(
            Uri serviceUri,
            ServicePartitionKey partitionKey,
            TargetReplicaSelector targetReplicaSelector,
            string listenerName,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            var client = await this.impl.GetClientAsync(
                serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName,
                retrySettings,
                cancellationToken);

            return client;
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
        async Task<IServiceRemotingClient> ICommunicationClientFactory<IServiceRemotingClient>.GetClientAsync(
            ResolvedServicePartition previousRsp,
            TargetReplicaSelector targetReplicaSelector,
            string listenerName,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken)
        {
            var client = await this.impl.GetClientAsync(
                previousRsp,
                targetReplicaSelector,
                listenerName,
                retrySettings,
                cancellationToken);

            return client;
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
            return this.impl.ReportOperationExceptionAsync(
                (FabricTransportServiceRemotingClient)client,
                exceptionInformation,
                retrySettings,
                cancellationToken);
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

        private void OnClientConnected(
            object sender,
            CommunicationClientEventArgs<IServiceRemotingClient> e)
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
            CommunicationClientEventArgs<IServiceRemotingClient> e)
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
