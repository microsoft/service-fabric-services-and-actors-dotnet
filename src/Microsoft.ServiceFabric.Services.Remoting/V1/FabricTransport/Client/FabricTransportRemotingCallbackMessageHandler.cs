// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Client
{
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport.Client;

    class FabricTransportRemotingCallbackMessageHandler : IFabricTransportCallbackMessageHandler
    {
        private IServiceRemotingCallbackClient remotingCallbackClient;
        private DataContractSerializer serializer;

        public FabricTransportRemotingCallbackMessageHandler(IServiceRemotingCallbackClient remotingCallbackClient)
        {
            this.remotingCallbackClient = remotingCallbackClient;
            this.serializer = new DataContractSerializer(typeof(ServiceRemotingMessageHeaders));
        }

        public Task<byte[]> RequestResponseAsync(byte[] headers, byte[] requestBody)
        {
            var messageHeaders = ServiceRemotingMessageHeaders.Deserialize(this.serializer, headers);
            return this.remotingCallbackClient.RequestResponseAsync(messageHeaders, requestBody);
        }

        public void OneWayMessage(byte[] headers, byte[] requestBody)
        {
            var messageHeaders = ServiceRemotingMessageHeaders.Deserialize(this.serializer, headers);
            this.remotingCallbackClient.OneWayMessage(messageHeaders, requestBody);
        }
    }
}