// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport.Client;
    using Microsoft.ServiceFabric.FabricTransport.Runtime;

    internal class FabricTransportServiceRemotingConnectionHandler : IFabricTransportConnectionHandler
    {
        private ConcurrentDictionary<string, FabricTransportCallbackClient> clientCallbackConnection;

        public FabricTransportServiceRemotingConnectionHandler()
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

        ~FabricTransportServiceRemotingConnectionHandler()
        {
            this.clientCallbackConnection.Clear();
        }
    }
}