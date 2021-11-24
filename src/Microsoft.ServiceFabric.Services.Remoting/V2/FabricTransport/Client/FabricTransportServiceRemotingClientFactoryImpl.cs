// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport.V2;
    using Microsoft.ServiceFabric.FabricTransport.V2.Client;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using SR = Microsoft.ServiceFabric.Services.Remoting.SR;

    internal class FabricTransportServiceRemotingClientFactoryImpl : CommunicationClientFactoryBase<FabricTransportServiceRemotingClient>
    {
        private readonly IFabricTransportCallbackMessageHandler fabricTransportRemotingCallbackMessageHandler;
        private readonly ServiceRemotingMessageSerializersManager serializersManager;
        private readonly FabricTransportRemotingSettings settings;
        private readonly NativeFabricTransportMessageDisposer disposer;
        private IEnumerable<IExceptionConvertor> exceptionConvertors;

        public FabricTransportServiceRemotingClientFactoryImpl(
            ServiceRemotingMessageSerializersManager serializersManager,
            FabricTransportRemotingSettings remotingSettings,
            IServiceRemotingCallbackMessageHandler remotingCallbackMessageHandler = null,
            IServicePartitionResolver servicePartitionResolver = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null,
            string traceId = null)
            : base(
                servicePartitionResolver,
                GetExceptionHandlers(exceptionHandlers),
                traceId)
        {
            this.exceptionConvertors = GetExceptionConvertors(exceptionConvertors);
            this.settings = remotingSettings ?? FabricTransportRemotingSettings.GetDefault();
            this.serializersManager = serializersManager;
            this.disposer = new NativeFabricTransportMessageDisposer();
            this.fabricTransportRemotingCallbackMessageHandler = new FabricTransportRemotingCallbackMessageHandler(remotingCallbackMessageHandler, this.serializersManager);
        }

        /// <summary>
        /// Aborts the given client
        /// </summary>
        /// <param name="client">Communication client</param>
        protected override void AbortClient(
            FabricTransportServiceRemotingClient client)
        {
            client.Abort();
        }

        protected override Task OpenClient(
            FabricTransportServiceRemotingClient client,
            CancellationToken cancellationToken)
        {
            return client.OpenAsync(cancellationToken);
        }

        /// <summary>
        /// Creates a communication client for the given endpoint address.
        /// </summary>
        /// <param name="endpoint">listener address where the replica is listening</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The communication client that was created</returns>
        protected override Task<FabricTransportServiceRemotingClient> CreateClientAsync(
            string endpoint,
            CancellationToken cancellationToken)
        {
            try
            {
                var remotingHandler = new FabricTransportRemotingClientEventHandler();
                var nativeClient = new FabricTransportClient(
                    this.settings.GetInternalSettings(),
                    endpoint,
                    remotingHandler,
                    this.fabricTransportRemotingCallbackMessageHandler,
                    this.disposer);
                var client = new FabricTransportServiceRemotingClient(
                    this.serializersManager,
                    nativeClient,
                    remotingHandler,
                    this.exceptionConvertors);
                remotingHandler.ClientConnected += this.OnFabricTransportClientConnected;
                remotingHandler.ClientDisconnected += this.OnFabricTransportClientDisconnected;
                return Task.FromResult(client);
            }
            catch (FabricInvalidAddressException)
            {
                throw new FabricInvalidAddressException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.ErrorInvalidAddress,
                        endpoint));
            }
        }

        /// <summary>
        /// Returns true if the client is still valid. Connection oriented transports can use this method to indicate that the client is no longer
        /// connected to the service.
        /// </summary>
        /// <param name="remotingClient">the communication client</param>
        /// <returns>true if the client is valid, false otherwise</returns>
        protected override bool ValidateClient(
            FabricTransportServiceRemotingClient remotingClient)
        {
            return remotingClient.IsValid;
        }

        /// <summary>
        /// Returns true if the client is still valid and connected to the endpoint specified in the parameter.
        /// </summary>
        /// <param name="endpoint">Specifies the expected endpoint to which we think the client is connected to</param>
        /// <param name="remotingClient">the communication client</param>
        /// <returns>true if the client is valid, false otherwise</returns>
        protected override bool ValidateClient(
            string endpoint,
            FabricTransportServiceRemotingClient remotingClient)
        {
            return remotingClient.IsValid && remotingClient.ConnectionAddress.Equals(endpoint);
        }

        private static IEnumerable<IExceptionConvertor> GetExceptionConvertors(
            IEnumerable<IExceptionConvertor> exceptionConvertors)
        {
            var svcExceptionConvetors = new List<IExceptionConvertor>();
            if (exceptionConvertors != null)
            {
                svcExceptionConvetors.AddRange(exceptionConvertors);
            }

            svcExceptionConvetors.Add(new FabricExceptionConvertor());
            svcExceptionConvetors.Add(new SystemExceptionConvertor());

            return svcExceptionConvetors;
        }

        private static IEnumerable<IExceptionHandler> GetExceptionHandlers(
            IEnumerable<IExceptionHandler> exceptionHandlers)
        {
            var handlers = new List<IExceptionHandler>();
            if (exceptionHandlers != null)
            {
                handlers.AddRange(exceptionHandlers);
            }

            handlers.Add(new ExceptionHandler());
            handlers.Add(new ServiceRemotingExceptionHandler());
            return handlers;
        }

        private void OnFabricTransportClientConnected(
            object sender,
            CommunicationClientEventArgs<FabricTransportServiceRemotingClient> e)
        {
            this.OnClientConnected(e.Client);
        }

        private void OnFabricTransportClientDisconnected(
            object sender,
            CommunicationClientEventArgs<FabricTransportServiceRemotingClient> e)
        {
            this.OnClientDisconnected(e.Client);
        }
    }
}
