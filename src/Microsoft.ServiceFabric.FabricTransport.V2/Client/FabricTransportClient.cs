// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.FabricTransport.V2.Client
{
    using System;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Fabric.Interop;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using static Microsoft.ServiceFabric.FabricTransport.V2.NativeFabricTransport;
    using SR = Microsoft.ServiceFabric.FabricTransport.SR;

    internal class FabricTransportClient : IDisposable
    {
        private NativeFabricTransport.IFabricTransportClient2 nativeClient;
        protected FabricTransportSettings settings;

        public FabricTransportClient(
            FabricTransportSettings transportSettings,
            string connectionAddress,
            IFabricTransportClientEventHandler eventHandler,
            IFabricTransportCallbackMessageHandler contract,
            NativeFabricTransport.IFabricTransportMessageDisposer messageMessageDisposer)
        {
            this.ConnectionAddress = connectionAddress;
            this.settings = transportSettings;
            Utility.WrapNativeSyncInvokeInMTA(
                () => this.CreateNativeClient(transportSettings, connectionAddress, eventHandler, contract, messageMessageDisposer),
                "FabricTransportClient.Create");
        }

        public FabricTransportSettings Settings
        {
            get { return this.settings; }
        }

        public bool IsValid { get; set; }
        public string ConnectionAddress { get; private set; }

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

        public virtual void SendOneWay(FabricTransportMessage message)
        {
            NativeFabricTransport.IFabricTransportMessage nativeMessage =
                new NativeFabricTransportMessage(message);
            this.nativeClient.Send(nativeMessage);
        }


        private void CreateNativeClient(
            FabricTransportSettings transportSettings,
            string connectionAddress,
            IFabricTransportClientEventHandler eventHandler,
            IFabricTransportCallbackMessageHandler contract,
            NativeFabricTransport.IFabricTransportMessageDisposer messageMessageDisposer)
        {
            var iid = typeof(NativeFabricTransport.IFabricTransportClient2).GetTypeInfo().GUID;
            using (var pin = new PinCollection())
            {
                var nativeTransportSettings = transportSettings.ToNativeV2(pin);
                var messageHandler = new FabricTransportCallbackHandlerBroker(contract);
                var nativeConnectionAddress = pin.AddBlittable(connectionAddress);
                var nativeEventHandler = new FabricTransportClientConnectionEventHandlerBroker(eventHandler);
                this.nativeClient =
                    (NativeFabricTransport.IFabricTransportClient2)NativeFabricTransport.CreateFabricTransportClient(
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
            NativeFabricTransport.IFabricTransportMessage nativeFabricTransportMessage =
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

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls


        //Used for Dummy Implemmentation
        protected FabricTransportClient()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // No other managed resources to dispose.
                }

                if (this.nativeClient != null)
                {
                    this.nativeClient.FinalReleaseComObject();
                }

                this.disposedValue = true;
            }
        }

        public void Abort()
        {
            Utility.WrapNativeSyncInvokeInMTA(() => this.internalAbort(), "Client.Abort");
        }

        private void internalAbort()
        {
            this.nativeClient.Abort();
        }

        ~FabricTransportClient()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion



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

