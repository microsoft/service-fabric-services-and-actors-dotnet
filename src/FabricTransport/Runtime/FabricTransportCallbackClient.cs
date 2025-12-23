// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric.Interop;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime
{
    internal class FabricTransportCallbackClient : IDisposable
    {
        private readonly TimeSpan defaultTimeout = TimeSpan.FromMinutes(2);
        private readonly NativeFabricTransport.IFabricTransportClientConnection nativeClientConnection;
        private readonly string clientId;

        bool disposed;

        public FabricTransportCallbackClient(
            NativeFabricTransport.IFabricTransportClientConnection nativeClientConnection)
        {
            this.nativeClientConnection = nativeClientConnection;
            var clientId = this.nativeClientConnection.get_ClientId();
            this.clientId = NativeTypes.FromNativeString(clientId);
        }

        public string GetClientId()
        {
            return this.clientId;
        }

        public void OneWayMessage(FabricTransportMessage requestBody)
        {
            NativeFabricTransport.IFabricTransportMessage message = new NativeFabricTransportMessage(requestBody);
            Utility.WrapNativeSyncInvokeInMTA(() => this.nativeClientConnection.Send(message),
                "NativeFabricClientConnection.SendMessage");
        }

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
