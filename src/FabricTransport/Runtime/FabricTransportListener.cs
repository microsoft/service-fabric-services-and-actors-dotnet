// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Fabric;
using System.Fabric.Interop;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime
{
    /// <summary>
    /// Implements <see cref="IFabricTransportListener"/> over Service Fabric's native transport.
    /// </summary>
    internal class FabricTransportListener : IFabricTransportListener
    {
        private NativeFabricTransport.IFabricTransportListener nativeListner;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportListener"/> class.
        /// </summary>
        /// <param name="transportSettings">The settings that configure the listener.</param>
        /// <param name="listenerAddress">The address on which the listener accepts client connections.</param>
        /// <param name="serviceImplementation">The handler that processes incoming request-response and one-way messages.</param>
        /// <param name="remotingConnectionHandler">The handler that tracks client callback channels for pushing one-way messages back to clients.</param>
        public FabricTransportListener(
            FabricTransportSettings transportSettings,
            FabricTransportListenerAddress listenerAddress,
            IFabricTransportMessageHandler serviceImplementation,
            IFabricTransportConnectionHandler remotingConnectionHandler)
        {
            //TODO: Remove this address update  after Bug :1225032 gets resolved
            //Update the Address Path if Settings is Secure
            var isNotSecureEndpoint = transportSettings.SecurityCredentials.CredentialType.Equals(CredentialType.None);
            listenerAddress.Path = !isNotSecureEndpoint
                ? string.Format(CultureInfo.InvariantCulture, "{0}-{1}", listenerAddress.Path, Helper.Secure)
                : listenerAddress.Path;
            Utility.WrapNativeSyncInvokeInMTA(
                () =>
                    this.CreateNativeListener(serviceImplementation, transportSettings, listenerAddress,
                        remotingConnectionHandler),
                "FabricTransportListener");
        }

        private void CreateNativeListener(
            IFabricTransportMessageHandler contract,
            FabricTransportSettings transportSettings,
            FabricTransportListenerAddress listenerAddress,
            IFabricTransportConnectionHandler connectionHandler)
        {
            var iid = typeof(NativeFabricTransport.IFabricTransportListener).GetTypeInfo().GUID;

            using (var pin = new PinCollection())
            {
                var nativeTransportSettings = transportSettings.ToNativeV2(pin);
                var nativeListenerAddress = listenerAddress.ToNative(pin);
                var nativeConnectionHandler = new FabricTransportConnectionHandlerBroker(connectionHandler);
                var messageHandler = new FabricTransportMessageHandlerBroker(contract, connectionHandler);
                var nativeFabricTransportMessageDisposer = new NativeFabricTransportMessageDisposer();
                this.nativeListner = NativeFabricTransport.CreateFabricTransportListener(
                    ref iid,
                    nativeTransportSettings,
                    nativeListenerAddress,
                    messageHandler,
                    nativeConnectionHandler,
                    nativeFabricTransportMessageDisposer);
            }
        }

        private NativeCommon.IFabricAsyncOperationContext OpenBeginWrapper(
            NativeCommon.IFabricAsyncOperationCallback callback)
        {
            return this.nativeListner.BeginOpen(callback);
        }

        private string OpenEndWrapper(NativeCommon.IFabricAsyncOperationContext context)
        {
            return NativeTypes.FromNativeString(this.nativeListner.EndOpen(context));
        }

        #region API

        /// <inheritdoc/>
        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            return Utility.WrapNativeAsyncInvokeInMTA<string>(
                (callback) => this.OpenBeginWrapper(callback),
                this.OpenEndWrapper,
                cancellationToken,
                "FabricTransportListener.Open");
        }

        /// <inheritdoc/>
        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return Utility.WrapNativeAsyncInvokeInMTA(
                (callback) => this.nativeListner.BeginClose(callback),
                this.nativeListner.EndClose,
                cancellationToken,
                "FabricTransportListener.Close");
        }

        /// <inheritdoc/>
        public void Abort()
        {
            if (this.nativeListner != null)
            {
                Utility.WrapNativeSyncInvokeInMTA(() => this.nativeListner.Abort(), "Listner.Abort");
            }
        }

        #endregion

        /// <inheritdoc/>
        public void Dispose()
        {
            if (nativeListner != null)
            {
                nativeListner.FinalReleaseComObject();
                nativeListner = null;
            }
        }
    }
}
