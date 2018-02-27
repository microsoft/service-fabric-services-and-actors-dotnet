// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
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

    // how does the serialization provider gets the types?
    // ServiceRemotingMessageSerializersManager obtain the known types from the codegen
    // layer and pass to the serialization provider to create the serializer

    // ReSharper disable UnusedParameter.Local
    /// <summary>
    ///
    /// </summary>
    internal class FabricTransportServiceRemotingClientFactoryImpl : CommunicationClientFactoryBase<FabricTransportServiceRemotingClient>
    {
        private readonly IFabricTransportCallbackMessageHandler fabricTransportRemotingCallbackMessageHandler;
        private readonly ServiceRemotingMessageSerializersManager serializersManager;
        private FabricTransportRemotingSettings settings;
        private readonly NativeFabricTransportMessageDisposer disposer;
        /// <summary>
        ///
        /// </summary>
        /// <param name="remotingSettings"></param>
        /// <param name="remotingCallbackMessageHandler"></param>
        /// <param name="servicePartitionResolver"></param>
        /// <param name="exceptionHandlers"></param>
        /// <param name="traceId"></param>
        /// <param name="serializersManager"></param>
        public FabricTransportServiceRemotingClientFactoryImpl(
            ServiceRemotingMessageSerializersManager serializersManager,
            FabricTransportRemotingSettings remotingSettings,
            IServiceRemotingCallbackMessageHandler remotingCallbackMessageHandler = null,
            IServicePartitionResolver servicePartitionResolver = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            string traceId = null)
             : base(
                servicePartitionResolver,
                GetExceptionHandlers(exceptionHandlers),
                traceId)
        {
            this.settings = remotingSettings ?? FabricTransportRemotingSettings.GetDefault();
            this.serializersManager = serializersManager;
            this.disposer = new NativeFabricTransportMessageDisposer();
            this.fabricTransportRemotingCallbackMessageHandler = new FabricTransportRemotingCallbackMessageHandler(remotingCallbackMessageHandler, this.serializersManager);
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


        /// <summary>
        /// Returns true if the client is still valid. Connection oriented transports can use this method to indicate that the client is no longer
        /// connected to the service.
        /// </summary>
        /// <param name="remotingClient">the communication client</param>
        /// <returns>true if the client is valid, false otherwise</returns>
        protected override bool ValidateClient(FabricTransportServiceRemotingClient remotingClient)
        {
            return remotingClient.IsValid;
        }

        /// <summary>
        /// Returns true if the client is still valid and connected to the endpoint specified in the parameter.
        /// </summary>
        /// <param name="endpoint">Specifies the expected endpoint to which we think the client is connected to</param>
        /// <param name="remotingClient">the communication client</param>
        /// <returns>true if the client is valid, false otherwise</returns>
        protected override bool ValidateClient(string endpoint, FabricTransportServiceRemotingClient remotingClient)
        {
            return remotingClient.IsValid && remotingClient.ConnectionAddress.Equals(endpoint);
        }

        /// <summary>
        /// Creates a communication client for the given endpoint address.
        /// </summary>
        /// <param name="endpoint">listener address where the replica is listening</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The communication client that was created</returns>
        protected override Task<FabricTransportServiceRemotingClient> CreateClientAsync(string endpoint, CancellationToken cancellationToken)
        {
            try
            {
                var remotingHandler = new FabricTransportRemotingClientEventHandler();
                var nativeClient = new FabricTransportClient(this.settings.GetInternalSettings(), endpoint,
                    remotingHandler,
                    this.fabricTransportRemotingCallbackMessageHandler,
                    this.disposer);
                var client = new FabricTransportServiceRemotingClient(this.serializersManager, nativeClient);
                remotingHandler.ClientConnected += this.OnFabricTransportClientConnected;
                remotingHandler.ClientDisconnected += this.OnFabricTransportClientDisconnected;
                client.OpenAsync(CancellationToken.None).Wait();

                return Task.FromResult(client);
            }
            catch (FabricInvalidAddressException)
            {
                throw new FabricInvalidAddressException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.ErrorInvalidAddress,
                        endpoint
                        ));
            }
        }

        private void OnFabricTransportClientDisconnected(object sender, CommunicationClientEventArgs<FabricTransportServiceRemotingClient> e)
        {
            this.OnClientDisconnected(e.Client);
        }

        private void OnFabricTransportClientConnected(object sender, CommunicationClientEventArgs<FabricTransportServiceRemotingClient> e)
        {
            this.OnClientConnected(e.Client);
        }

        /// <summary>
        /// Aborts the given client
        /// </summary>
        /// <param name="client">Communication client</param>
        protected override void AbortClient(FabricTransportServiceRemotingClient client)
        {
            client.Abort();
        }
    }
}
