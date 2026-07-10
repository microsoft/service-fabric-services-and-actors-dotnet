// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric.Interop;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime
{
    /// <summary>
    /// Sends messages back to a connected client over its callback channel on the service side.
    /// </summary>
    internal class FabricTransportCallbackClient : IDisposable
    {
        private readonly TimeSpan defaultTimeout = TimeSpan.FromMinutes(2);
        private readonly NativeFabricTransport.IFabricTransportClientConnection nativeClientConnection;
        private readonly string clientId;

        bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportCallbackClient"/> class.
        /// </summary>
        /// <param name="nativeClientConnection">The native connection that identifies the client and carries messages back to it.</param>
        public FabricTransportCallbackClient(
            NativeFabricTransport.IFabricTransportClientConnection nativeClientConnection)
        {
            this.nativeClientConnection = nativeClientConnection;
            var clientId = this.nativeClientConnection.get_ClientId();
            this.clientId = NativeTypes.FromNativeString(clientId);
        }

        /// <summary>
        /// Returns the identifier of the client this callback channel sends messages to.
        /// </summary>
        public string GetClientId()
        {
            return this.clientId;
        }

        /// <summary>
        /// Sends <paramref name="requestBody"/> to the client without waiting for a reply.
        /// </summary>
        public void OneWayMessage(FabricTransportMessage requestBody)
        {
            NativeFabricTransport.IFabricTransportMessage message = new NativeFabricTransportMessage(requestBody);
            Utility.WrapNativeSyncInvokeInMTA(() => this.nativeClientConnection.Send(message),
                "NativeFabricClientConnection.SendMessage");
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!disposed)
            {
                nativeClientConnection.FinalReleaseComObject();
                disposed = true;
            }
        }
    }
}
