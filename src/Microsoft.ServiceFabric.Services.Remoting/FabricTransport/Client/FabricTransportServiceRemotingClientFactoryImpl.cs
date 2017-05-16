// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Client
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport.Client;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    internal class FabricTransportServiceRemotingClientFactoryImpl :
        CommunicationClientFactoryBase<FabricTransportServiceRemotingClient>
    {
        private readonly FabricTransportRemotingSettings settings;
        private readonly FabricTransportRemotingCallbackMessageHandler fabricTransportRemotingCallbackMessageHandler;
        //We don't need implementation of ClientConnection handler provided in base class , hence creating new eventHandler here.Using FabricTransport Connectionhandler implementation.
        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> FabricTransportClientConnected;
        //We don't need impl of ClientConnection handler provided in base class , hence creating new eventHandler here.Using FabricTransport Connectionhandler implementation.
        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>>
            FabricTransportClientDisconnected;

        public FabricTransportServiceRemotingClientFactoryImpl(
            FabricTransportRemotingSettings FabricTransportRemotingSettings = null,
            IServiceRemotingCallbackClient callbackHandler = null,
            IServicePartitionResolver servicePartitionResolver = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            string traceId = null)
            : base(
                servicePartitionResolver,
                GetExceptionHandlers(exceptionHandlers),
                traceId)
        {
            this.settings = FabricTransportRemotingSettings ?? FabricTransportRemotingSettings.GetDefault();
            this.fabricTransportRemotingCallbackMessageHandler =
                new FabricTransportRemotingCallbackMessageHandler(callbackHandler);
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
            return handlers;
        }

        protected override Task<FabricTransportServiceRemotingClient> CreateClientAsync(
            string endpoint,
            CancellationToken cancellationToken)
        {
            try
            {
                var remotingHandler = new FabricTransportRemotingClientConnectionHandler();
                var nativeClient = new FabricTransportClient(this.settings.GetInternalSettings(), endpoint,
                    remotingHandler,
                    this.fabricTransportRemotingCallbackMessageHandler);
                var client = new FabricTransportServiceRemotingClient(nativeClient, remotingHandler);
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
                        endpoint
                        ));
            }
        }

        protected override void AbortClient(FabricTransportServiceRemotingClient remotingClient)
        {
            remotingClient.Abort();
        }

        protected override bool ValidateClient(FabricTransportServiceRemotingClient remotingClient)
        {
            return remotingClient.IsValid;
        }

        protected override bool ValidateClient(string endpoint, FabricTransportServiceRemotingClient remotingClient)
        {
            return remotingClient.IsValid && remotingClient.ConnectionAddress.Equals(endpoint);
        }

        private void OnFabricTransportClientConnected(object sender,
            CommunicationClientEventArgs<IServiceRemotingClient> e)
        {
            var handlers = this.FabricTransportClientConnected;
            if (handlers != null)
            {
                handlers(this, new CommunicationClientEventArgs<IServiceRemotingClient>() {Client = e.Client});
            }
        }

        private void OnFabricTransportClientDisconnected(object sender,
            CommunicationClientEventArgs<IServiceRemotingClient> e)
        {
            var handlers = this.FabricTransportClientDisconnected;
            if (handlers != null)
            {
                handlers(this, new CommunicationClientEventArgs<IServiceRemotingClient>() {Client = e.Client});
            }
        }

        private class ExceptionHandler : IExceptionHandler
        {
            bool IExceptionHandler.TryHandleException(
                ExceptionInformation exceptionInformation,
                OperationRetrySettings retrySettings,
                out ExceptionHandlingResult result)
            {
                var fabricException = exceptionInformation.Exception as FabricException;
                if (fabricException != null)
                {
                    return TryHandleFabricException(
                        fabricException,
                        retrySettings,
                        out result);
                }

                result = null;
                return false;
            }

            private static bool TryHandleFabricException(
                FabricException fabricException,
                OperationRetrySettings retrySettings,
                out ExceptionHandlingResult result)
            {
                if ((fabricException is FabricCannotConnectException) ||
                    (fabricException is FabricEndpointNotFoundException)
                    )
                {
                    result = new ExceptionHandlingRetryResult(
                        fabricException,
                        false,
                        retrySettings,
                        int.MaxValue);
                    return true;
                }

                if (fabricException.ErrorCode.Equals(FabricErrorCode.ServiceTooBusy))
                {
                    result = new ExceptionHandlingRetryResult(
                        fabricException,
                        true,
                        retrySettings,
                        int.MaxValue);
                    return true;
                }

                result = null;
                return false;
            }
        }
    }
}