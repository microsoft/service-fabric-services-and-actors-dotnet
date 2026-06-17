// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using System.Fabric.Common;
using System.Fabric.Interop;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.ServiceFabric.FabricTransport.NativeFabricTransport;

namespace Microsoft.ServiceFabric.FabricTransport.Client
{
    /// <summary>
    /// Connects to a service endpoint and exchanges request-response and one-way messages with it over
    /// Service Fabric's native transport.
    /// </summary>
    internal class FabricTransportClient : IDisposable
    {
        private IFabricTransportClient2 nativeClient;

        /// <summary>
        /// Stores the settings that configure this client's connection and timeouts.
        /// </summary>
        protected FabricTransportSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportClient"/> class targeting the service endpoint at <paramref name="connectionAddress"/>.
        /// </summary>
        /// <param name="transportSettings">The settings that configure the connection and its timeouts.</param>
        /// <param name="connectionAddress">The address of the service endpoint to connect to.</param>
        /// <param name="eventHandler">The handler notified when the connection is established or lost.</param>
        /// <param name="contract">The handler that processes one-way callback messages pushed from the service.</param>
        /// <param name="messageMessageDisposer">The disposer that releases native resources of received messages.</param>
        /// <exception cref="FabricInvalidAddressException">
        /// <paramref name="connectionAddress"/> is not a valid FabricTransport endpoint address.
        /// </exception>
        public FabricTransportClient(
            FabricTransportSettings transportSettings,
            string connectionAddress,
            IFabricTransportClientEventHandler eventHandler,
            IFabricTransportCallbackMessageHandler contract,
            IFabricTransportMessageDisposer messageMessageDisposer)
        {
            this.ConnectionAddress = connectionAddress;
            this.settings = transportSettings;
            Utility.WrapNativeSyncInvokeInMTA(
                () => this.CreateNativeClient(transportSettings, connectionAddress, eventHandler, contract,messageMessageDisposer),
                "FabricTransportClient.Create");
        }

        /// <summary>
        /// Gets the settings that configure this client's connection and timeouts.
        /// </summary>
        public FabricTransportSettings Settings
        {
            get { return this.settings; }
        }

        /// <summary>
        /// Gets the address of the service endpoint this client connects to.
        /// </summary>
        public string ConnectionAddress { get; private set; }

        /// <summary>
        /// Asynchronously opens the connection to the service endpoint.
        /// </summary>
        /// <exception cref="FabricCannotConnectException">The client cannot connect to the service endpoint.</exception>
        /// <exception cref="FabricConnectionDeniedException">The client connects without security to a secured service endpoint.</exception>
        /// <exception cref="TimeoutException">The connection is not established within the configured connect timeout.</exception>
        public async Task OpenAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Utility.WrapNativeAsyncInvokeInMTA(
                    (callback) => this.BeginOpen(this.settings.ConnectTimeout, callback),
                    this.EndOpen,
                    cancellationToken,
                    "OpenAsync");
            }
            catch (FabricCannotConnectException)
            {
                //TODO: Remove this check after Bug :1225032 gets resolved
                if (this.IsSecurityMismatch())
                {
                    throw new FabricConnectionDeniedException(SR.Error_ConnectionDenied);
                }
                throw;
            }
            catch (TimeoutException)
            {
                throw new TimeoutException(string.Format(CultureInfo.CurrentCulture, SR.ErrorServiceTooBusy));
            }
        }

        /// <summary>
        /// Asynchronously closes the connection to the service endpoint.
        /// </summary>
        /// <remarks>The <paramref name="cancellationToken"/> is accepted for signature consistency but is not currently observed.</remarks>
        /// <exception cref="FabricCannotConnectException">The client cannot connect to the service endpoint.</exception>
        /// <exception cref="FabricConnectionDeniedException">The client connects without security to a secured service endpoint.</exception>
        /// <exception cref="TimeoutException">The connection is not closed within the configured connect timeout.</exception>
        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Utility.WrapNativeAsyncInvokeInMTA(
                    (callback) => this.BeginClose(this.settings.ConnectTimeout, callback),
                    this.EndClose,
                    CancellationToken.None,
                    "OpenAsync");
            }
            catch (FabricCannotConnectException)
            {
                //TODO: Remove this check after Bug :1225032 gets resolved
                if (this.IsSecurityMismatch())
                {
                    throw new FabricConnectionDeniedException(SR.Error_ConnectionDenied);
                }
                throw;
            }
            catch (TimeoutException)
            {
                throw new TimeoutException(string.Format(CultureInfo.CurrentCulture, SR.ErrorServiceTooBusy));
            }
        }

        /// <summary>
        /// Asynchronously sends <paramref name="requestMessage"/> to the service and returns its reply.
        /// </summary>
        /// <param name="requestMessage">The message to send to the service.</param>
        /// <param name="timeout">The maximum time to wait for the reply.</param>
        /// <param name="requestId">The identifier correlating the request with its reply, or <see cref="Guid.Empty"/> to let the transport assign one.</param>
        /// <exception cref="FabricCannotConnectException">The client cannot connect to the service endpoint.</exception>
        /// <exception cref="FabricConnectionDeniedException">The client connects without security to a secured service endpoint.</exception>
        /// <exception cref="TimeoutException">The reply is not received within <paramref name="timeout"/>.</exception>
        public async Task<FabricTransportMessage> RequestResponseAsync(FabricTransportMessage requestMessage,
            TimeSpan timeout, Guid requestId = default(Guid))
        {
            try
            {
                return
                    await
                        Utility.WrapNativeAsyncInvokeInMTA<FabricTransportMessage>(
                            (callback) => this.BeginRequest(requestId, requestMessage, timeout, callback),
                            (context) => this.EndRequest(requestId, context),
                            CancellationToken.None,
                            "RequestResponseAsync");
            }
            catch (Exception e)
            {
                AppTrace.TraceSource.WriteExceptionAsWarning("FabricTransportClient", e, "RequestResponseAsync failed");

                if (e is FabricCannotConnectException)
                {
                    //TODO: Remove this check after Bug :1225032 gets resolved
                    if (this.IsSecurityMismatch())
                    {
                        throw new FabricConnectionDeniedException(SR.Error_ConnectionDenied);
                    }
                }
                else if (e is TimeoutException)
                {
                    throw new TimeoutException(string.Format(CultureInfo.CurrentCulture, SR.ErrorServiceTooBusy));
                }

                throw;
            }
        }

        /// <summary>
        /// Sends <paramref name="message"/> to the service without waiting for a reply.
        /// </summary>
        public virtual void SendOneWay(FabricTransportMessage message)
        {
            IFabricTransportMessage nativeMessage =
                new NativeFabricTransportMessage(message);
            this.nativeClient.Send(nativeMessage);
        }

    
        private void CreateNativeClient(
            FabricTransportSettings transportSettings,
            string connectionAddress,
            IFabricTransportClientEventHandler eventHandler,
            IFabricTransportCallbackMessageHandler contract,
            IFabricTransportMessageDisposer messageMessageDisposer)
        {
            var iid = typeof(IFabricTransportClient2).GetTypeInfo().GUID;
            using (var pin = new PinCollection())
            {
                var nativeTransportSettings = transportSettings.ToNativeV2(pin);
                var messageHandler = new FabricTransportCallbackHandlerBroker(contract);
                var nativeConnectionAddress = pin.AddBlittable(connectionAddress);
                var nativeEventHandler = new FabricTransportClientConnectionEventHandlerBroker(eventHandler);
                this.nativeClient = CreateFabricTransportClient(
                        ref iid,
                        nativeTransportSettings,
                        nativeConnectionAddress,
                        messageHandler,
                        nativeEventHandler,
                        messageMessageDisposer);
            }
        }

        private NativeCommon.IFabricAsyncOperationContext BeginRequest(
            Guid requestId,
            FabricTransportMessage message,
            TimeSpan timeout,
            NativeCommon.IFabricAsyncOperationCallback callback)
        {
                var timeoutInMilliSeconds = Utility.ToMilliseconds(timeout, "timeout");
                IFabricTransportMessage nativeFabricTransportMessage =
                    new NativeFabricTransportMessage(message);

            if (requestId == default(Guid))
            {
                return this.nativeClient.BeginRequest(nativeFabricTransportMessage, timeoutInMilliSeconds, callback);
            }
            else
            {
                return this.nativeClient.BeginRequestWithId(requestId, nativeFabricTransportMessage, timeoutInMilliSeconds, callback);
            }
        }

        private FabricTransportMessage EndRequest(Guid requestId, NativeCommon.IFabricAsyncOperationContext context)
        {
            IFabricTransportMessage message;
            if (requestId == default(Guid))
            {
                message = this.nativeClient.EndRequest(context);
            }
            else
            {
                message = this.nativeClient.EndRequestWithId(context);
            }

            var reply = NativeFabricTransportMessage.ToFabricTransportMessage(message);
            GC.KeepAlive(message);
            return reply;
        }

        //Used for Dummy Implemmentation
        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportClient"/> class without a native client,
        /// for use by test doubles that override the transport operations.
        /// </summary>
        protected FabricTransportClient()
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (nativeClient != null)
            {
                nativeClient.FinalReleaseComObject();
                nativeClient = null;
            }
        }

        /// <summary>
        /// Aborts the connection to the service endpoint without waiting for pending operations to complete.
        /// </summary>
        public void Abort()
        {
            Utility.WrapNativeSyncInvokeInMTA(() => this.internalAbort(), "Client.Abort");
        }

        private void internalAbort()
        {
            this.nativeClient.Abort();
        }

        private NativeCommon.IFabricAsyncOperationContext BeginOpen(TimeSpan connectTimeout,
            NativeCommon.IFabricAsyncOperationCallback callback)
        {
                var timeoutInMilliSeconds = Utility.ToMilliseconds(connectTimeout, "timeout");
                return this.nativeClient.BeginOpen(timeoutInMilliSeconds, callback);
        }

        private void EndOpen(NativeCommon.IFabricAsyncOperationContext context)
        {
            this.nativeClient.EndOpen(context);
        }

        private NativeCommon.IFabricAsyncOperationContext BeginClose(TimeSpan connectTimeout,
         NativeCommon.IFabricAsyncOperationCallback callback)
        {
            using (var pin = new PinCollection())
            {
                var timeoutInMilliSeconds = Utility.ToMilliseconds(connectTimeout, "timeout");
                return this.nativeClient.BeginClose(timeoutInMilliSeconds, callback);
            }
        }

        private void EndClose(NativeCommon.IFabricAsyncOperationContext context)
        {
            this.nativeClient.EndClose(context);
        }
        private bool IsSecurityMismatch()
        {
            //Cases where Client using unsecure and service using secure connection.
            if (this.ConnectionAddress.Contains(Helper.Secure) &&
                this.settings.SecurityCredentials.CredentialType.Equals(CredentialType.None))
            {
                return true;
            }
            return false;
        }
    }
}
