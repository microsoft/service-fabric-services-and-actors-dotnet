// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime
{
    using System.Collections.Concurrent;
    using Microsoft.ServiceFabric.FabricTransport.V2.Runtime;

    class FabricTransportRemotingConnectionHandler : IFabricTransportConnectionHandler
    {
        private ConcurrentDictionary<string, FabricTransportCallbackClient> clientCallbackConnection;

        public FabricTransportRemotingConnectionHandler()
        {
            this.clientCallbackConnection = new ConcurrentDictionary<string, FabricTransportCallbackClient>();
        }

        public Task ConnectAsync(FabricTransportCallbackClient fabricTransportServiceRemotingCallback, TimeSpan timeout)
        {
            this.AddCallBackConnection(fabricTransportServiceRemotingCallback);
            return Task.FromResult(true);
        }

        public Task DisconnectAsync(string clientId, TimeSpan timeout)
        {
            this.RemoveCallBackConnection(clientId);
            return Task.FromResult(true);
        }

        FabricTransportCallbackClient IFabricTransportConnectionHandler.GetCallBack(string clientId)
        {
            FabricTransportCallbackClient nativeCallback;
            this.clientCallbackConnection.TryGetValue(clientId, out nativeCallback);
            return nativeCallback;
        }

        private void AddCallBackConnection(FabricTransportCallbackClient fabricTransportServiceRemotingCallback)
        {
            this.clientCallbackConnection.TryAdd(fabricTransportServiceRemotingCallback.GetClientId(), fabricTransportServiceRemotingCallback);
        }

        private void RemoveCallBackConnection(string clientId)
        {
            FabricTransportCallbackClient fabricTransportCallbackClient;
            this.clientCallbackConnection.TryRemove(clientId, out fabricTransportCallbackClient);
            if (fabricTransportCallbackClient != null)
            {
                fabricTransportCallbackClient.Dispose();
            }
        }

        ~FabricTransportRemotingConnectionHandler()
        {
            this.clientCallbackConnection.Clear();
        }
    }
}
