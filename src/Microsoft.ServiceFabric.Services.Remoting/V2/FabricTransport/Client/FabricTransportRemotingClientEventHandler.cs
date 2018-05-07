// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.FabricTransport.V2.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal class FabricTransportRemotingClientEventHandler : IFabricTransportClientEventHandler
    {
        private readonly FabricTransportServiceRemotingClient remotingClient;

        public FabricTransportRemotingClientEventHandler()
        {
            this.remotingClient = new DummyFabricTransportRemotingClient(null, null);
        }

        public event EventHandler<CommunicationClientEventArgs<FabricTransportServiceRemotingClient>> ClientConnected;

        public event EventHandler<CommunicationClientEventArgs<FabricTransportServiceRemotingClient>> ClientDisconnected;

        public string ListenerName
        {
            set { this.remotingClient.ListenerName = value; }
        }

        public ResolvedServicePartition ResolvedServicePartition
        {
            set { this.remotingClient.ResolvedServicePartition = value; }
        }

        public ResolvedServiceEndpoint Endpoint
        {
            set { this.remotingClient.Endpoint = value; }
        }

        void IFabricTransportClientEventHandler.OnConnected()
        {
            var handlers = this.ClientConnected;
            if (handlers != null)
            {
                handlers(
                    this,
                    new CommunicationClientEventArgs<FabricTransportServiceRemotingClient>()
                    {
                        Client = this.remotingClient,
                    });
            }
        }

        void IFabricTransportClientEventHandler.OnDisconnected()
        {
            var handlers = this.ClientDisconnected;
            if (handlers != null)
            {
                handlers(
                    this,
                    new CommunicationClientEventArgs<FabricTransportServiceRemotingClient>()
                    {
                        Client = this.remotingClient,
                    });
            }
        }
    }
}
