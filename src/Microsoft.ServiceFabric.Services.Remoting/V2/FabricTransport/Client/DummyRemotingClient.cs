// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    internal class DummyFabricTransportRemotingClient : FabricTransportServiceRemotingClient
    {
        public new Task<IServiceRemotingRequestMessage> RequestResponseAsync(IServiceRemotingRequestMessage requestRequestMessage)
        {
            throw new ArgumentException(SR.Error_InvalidOperation);
        }

        public new void SendOneWay(IServiceRemotingRequestMessage requestMessage)
        {
            throw new ArgumentException(SR.Error_InvalidOperation);
        }

        public DummyFabricTransportRemotingClient(ServiceRemotingMessageSerializersManager serializersManager, FabricTransportClient fabricTransportClient) : base(serializersManager, fabricTransportClient)
        {
        }
    }
}
