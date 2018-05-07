// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client
{
    using Microsoft.ServiceFabric.FabricTransport.V2;
    using Microsoft.ServiceFabric.FabricTransport.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    internal class FabricTransportRemotingCallbackMessageHandler : IFabricTransportCallbackMessageHandler
    {
        private readonly ServiceRemotingMessageSerializersManager manager;
        private IServiceRemotingCallbackMessageHandler remotingCallbackClient;

        public FabricTransportRemotingCallbackMessageHandler(
            IServiceRemotingCallbackMessageHandler remotingCallbackClient,
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
            IServiceRemotingRequestMessageBody deserializedMsgBody;
            if (message.GetBody() != null)
            {
                deserializedMsgBody =
                    msgBodySerializer.Deserialize(new IncomingMessageBody(message.GetBody().GetRecievedStream()));
            }
            else
            {
                deserializedMsgBody = null;
            }

            this.remotingCallbackClient.HandleOneWayMessage(new ServiceRemotingRequestMessage(deserializerHeaders, deserializedMsgBody));
        }
    }
}
