// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Wcf.Client
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Wcf.Runtime;

    /// <summary>
    /// An <see cref="IServiceRemotingClientFactory"/> that uses
    /// Windows Communication Foundation to create <see cref="IServiceRemotingClient"/> to communicate with stateless
    /// and stateful services over interfaces that are remoted via
    /// <see cref="WcfServiceRemotingListener"/>.
    /// </summary>
    [Obsolete(DeprecationMessage.RemotingV1)]
    public class WcfServiceRemotingClientFactory : IServiceRemotingClientFactory
    {
        private readonly WcfCommunicationClientFactory<IServiceRemotingContract> wcfFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServiceRemotingClientFactory"/> class.
        /// </summary>
        /// <param name="clientBinding">
        ///     WCF binding to use for the client. If the client binding is not specified or null,
        ///     a default client binding is created using
        ///     <see cref="Microsoft.ServiceFabric.Services.Communication.Wcf.WcfUtility.CreateTcpClientBinding"/> method
        ///     which creates a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="callbackClient">
        ///     The callback client that receives the callbacks from the service.
        /// </param>
        /// <param name="exceptionHandlers">
        ///     Exception handlers to handle the exceptions encountered in communicating with the service.
        /// </param>
        /// <param name="servicePartitionResolver">
        ///     Service partition resolver to resolve the service endpoints. If not specified, a default
        ///     service partition resolver returned by <see cref="ServicePartitionResolver.GetDefault"/> is used.
        /// </param>
        /// <param name="traceId">
        ///     Id to use in diagnostics traces from this component.
        /// </param>
        /// <param name="createWcfClientFactory">
        ///     Delegate function that creates <see cref="Microsoft.ServiceFabric.Services.Communication.Wcf.Client.WcfCommunicationClientFactory{TServiceContract}"/> using the
        ///     <see cref="IServiceRemotingContract"/>.
        /// </param>
        /// <remarks>
        ///     This factory uses <see cref="WcfExceptionHandler"/> and <see cref="ServiceRemotingExceptionHandler"/> in addition to the
        ///     exception handlers supplied to the constructor.
        /// </remarks>
        public WcfServiceRemotingClientFactory(
            Binding clientBinding = null,
            IServiceRemotingCallbackClient callbackClient = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            IServicePartitionResolver servicePartitionResolver = null,
            string traceId = null,
            Func<
                Binding,
                IEnumerable<IExceptionHandler>,
                IServicePartitionResolver,
                string,
                IServiceRemotingCallbackContract,
                WcfCommunicationClientFactory<IServiceRemotingContract>> createWcfClientFactory = null)
        {
            if (traceId == null)
            {
                traceId = Guid.NewGuid().ToString();
            }

            if (createWcfClientFactory == null)
            {
                this.wcfFactory = new WcfCommunicationClientFactory<IServiceRemotingContract>(
                    clientBinding,
                    GetExceptionHandlers(exceptionHandlers, traceId),
                    servicePartitionResolver,
                    traceId,
                    GetCallbackImplementation(callbackClient));
            }
            else
            {
                this.wcfFactory = createWcfClientFactory(
                    clientBinding,
                    GetExceptionHandlers(exceptionHandlers, traceId),
                    servicePartitionResolver,
                    traceId,
                    GetCallbackImplementation(callbackClient));
            }

            this.wcfFactory.ClientConnected += this.OnClientConnected;
            this.wcfFactory.ClientDisconnected += this.OnClientDisconnected;
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
        ///
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
            var wcfClient = await this.wcfFactory.GetClientAsync(
                serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName,
                retrySettings,
                cancellationToken);

            return new WcfServiceRemotingClient(wcfClient);
        }

        /// <summary>
        /// Re-resolves a partition of the specified service containing one or more communication listeners and returns a client to communicate
        /// to the endpoint corresponding to the given listenerName.
        ///
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
            var wcfClient = await this.wcfFactory.GetClientAsync(
                previousRsp,
                targetReplicaSelector,
                listenerName,
                retrySettings,
                cancellationToken);

            return new WcfServiceRemotingClient(wcfClient);
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
            return this.wcfFactory.ReportOperationExceptionAsync(
                ((WcfServiceRemotingClient)client).WcfClient,
                exceptionInformation,
                retrySettings,
                cancellationToken);
        }

        /// <summary>
        /// Releases managed/unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.wcfFactory.Dispose();
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

        private static IServiceRemotingCallbackContract GetCallbackImplementation(
            IServiceRemotingCallbackClient callbackClient)
        {
            if (callbackClient == null)
            {
                return new NoOpCallbackReceiver();
            }
            else
            {
                return new CallbackReceiver(callbackClient);
            }
        }

        private void OnClientDisconnected(
            object sender,
            CommunicationClientEventArgs<WcfCommunicationClient<IServiceRemotingContract>> communicationClientEventArgs)
        {
            var handlers = this.ClientDisconnected;
            if (handlers != null)
            {
                handlers(
                    this,
                    new CommunicationClientEventArgs<IServiceRemotingClient>()
                    {
                        Client = new WcfServiceRemotingClient(communicationClientEventArgs.Client),
                    });
            }
        }

        private void OnClientConnected(
            object sender,
            CommunicationClientEventArgs<WcfCommunicationClient<IServiceRemotingContract>> communicationClientEventArgs)
        {
            var handlers = this.ClientConnected;
            if (handlers != null)
            {
                handlers(
                    this,
                    new CommunicationClientEventArgs<IServiceRemotingClient>()
                    {
                        Client = new WcfServiceRemotingClient(communicationClientEventArgs.Client),
                    });
            }
        }

        [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        private class NoOpCallbackReceiver : IServiceRemotingCallbackContract
        {
            public NoOpCallbackReceiver()
            {
            }

            public Task<byte[]> RequestResponseAsync(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
            {
                return Task.FromResult<byte[]>(null);
            }

            public void SendOneWay(ServiceRemotingMessageHeaders headers, byte[] msgBody)
            {
            }
        }

        [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        private class CallbackReceiver : IServiceRemotingCallbackContract
        {
            private readonly IServiceRemotingCallbackClient callbackHandler;

            public CallbackReceiver(IServiceRemotingCallbackClient callbackHandler)
            {
                this.callbackHandler = callbackHandler;
            }

            public Task<byte[]> RequestResponseAsync(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
            {
                return this.callbackHandler.RequestResponseAsync(messageHeaders, requestBody);
            }

            public void SendOneWay(ServiceRemotingMessageHeaders headers, byte[] msgBody)
            {
                Task.Run(() => this.callbackHandler.OneWayMessage(headers, msgBody));
            }
        }
    }
}
