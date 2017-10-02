// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client
{
    using Microsoft.ServiceFabric.FabricTransport.V2;
    using Microsoft.ServiceFabric.FabricTransport.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    class FabricTransportRemotingCallbackMessageHandler : IFabricTransportCallbackMessageHandler
    {
        private IServiceRemotingCallbackMessageHandler remotingCallbackClient;
        private readonly ServiceRemotingMessageSerializersManager manager;

        public FabricTransportRemotingCallbackMessageHandler(IServiceRemotingCallbackMessageHandler remotingCallbackClient,
            ServiceRemotingMessageSerializersManager manager)
        {
            this.remotingCallbackClient = remotingCallbackClient;
            this.manager = manager;
        }

        public void OneWayMessage(FabricTransportMessage message)
        {
            var headerSerializer = this.manager.GetHeaderSerializer();
            var deserializerHeaders = headerSerializer.DeserializeRequestHeaders(new IncomingMessageHeader(message.GetHeader().GetRecievedStream()));
            var msgBodySerializer = this.manager.GetRequestBodySerializer(deserializerHeaders.InterfaceId);
            var deserializedMsgBody = msgBodySerializer.Deserialize(new IncomingMessageBody(message.GetBody().GetRecievedStream()));
            this.remotingCallbackClient.HandleOneWayMessage(new ServiceRemotingRequestMessage(deserializerHeaders,deserializedMsgBody));
        }
    }
}
