// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Client
{
    using System;
    using System.Fabric;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport;
    using Microsoft.ServiceFabric.FabricTransport.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Client;
    using SR = Microsoft.ServiceFabric.Services.Remoting.SR;

    [Obsolete(DeprecationMessage.RemotingV1)]
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
