// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.FabricTransport.V2;
    using Microsoft.ServiceFabric.FabricTransport.V2.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    internal class FabricTransportServiceRemotingCallbackClient : IServiceRemotingCallbackClient
    {
        private readonly FabricTransportCallbackClient fabricTransportCallbackClient;
        private readonly ServiceRemotingMessageSerializersManager serializersManager;
        private readonly IServiceRemotingMessageBodyFactory remotingMessageBodyFactory;

        public FabricTransportServiceRemotingCallbackClient(
            FabricTransportCallbackClient fabricTransportCallbackClient,
            ServiceRemotingMessageSerializersManager serializersManager)
        {
            this.fabricTransportCallbackClient = fabricTransportCallbackClient;
            this.serializersManager = serializersManager;
            this.remotingMessageBodyFactory = this.serializersManager.GetSerializationProvider().CreateMessageBodyFactory();
        }

        public void SendOneWay(IServiceRemotingRequestMessage requestMessage)
        {
            var headerSerialzier = this.serializersManager.GetHeaderSerializer();
            var serialzedHeader = headerSerialzier.SerializeRequestHeader(requestMessage.GetHeader());
            var requestSerializer = this.serializersManager.GetRequestBodySerializer(requestMessage.GetHeader().InterfaceId);
            var serializedMsgBody = requestSerializer.Serialize(requestMessage.GetBody());
            var fabricTransportRequestBody = serializedMsgBody != null
                ? new FabricTransportRequestBody(
                    serializedMsgBody.GetSendBuffers(),
                    serializedMsgBody.Dispose)
                : new FabricTransportRequestBody(new List<ArraySegment<byte>>(), null);
            this.fabricTransportCallbackClient.OneWayMessage(
                new FabricTransportMessage(
                    new FabricTransportRequestHeader(serialzedHeader.GetSendBuffer(), serialzedHeader.Dispose),
                    fabricTransportRequestBody));
        }

        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
            return this.remotingMessageBodyFactory;
        }
    }
}
