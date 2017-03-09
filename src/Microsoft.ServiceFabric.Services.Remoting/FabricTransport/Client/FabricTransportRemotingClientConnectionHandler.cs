// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Client
{
    using System;
    using System.Fabric;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.FabricTransport.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using SR = Microsoft.ServiceFabric.Services.Remoting.SR;

    internal class FabricTransportRemotingClientConnectionHandler :IFabricTransportClientConnectionHandler
    {
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

        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientConnected;

        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientDisconnected;

        private IServiceRemotingClient remotingClient;

        public FabricTransportRemotingClientConnectionHandler()
        {
            this.remotingClient = new FabricTransportServiceRemotingClient(new DummyNativeClient(), null);
        }

        void IFabricTransportClientConnectionHandler.OnConnected()
        {
            var handlers = this.ClientConnected;
            if (handlers != null)
            {
                handlers(
                    this,
                    new CommunicationClientEventArgs<IServiceRemotingClient>()
                    {
                        Client = this.remotingClient
                    });
            }
        }

        void IFabricTransportClientConnectionHandler.OnDisconnected()
        {
            var handlers = this.ClientDisconnected;
            if (handlers != null)
            {
                handlers(
                    this,
                    new CommunicationClientEventArgs<IServiceRemotingClient>()
                    {
                        Client = this.remotingClient
                    });
            }
        }
    }

    internal class DummyNativeClient : FabricTransportClient
    {
        public DummyNativeClient()
        {
            this.settings = new FabricTransportSettings();
        }

        public override Task<FabricTransportReplyMessage> RequestResponseAsync(byte[] header, byte[] requestBody, TimeSpan timeout)
        {
            throw new ArgumentException(SR.Error_InvalidOperation);
        }

        public override void SendOneWay(byte[] messageHeaders, byte[] requestBody)
        {
            throw new ArgumentException(SR.Error_InvalidOperation);
        }
    }
}