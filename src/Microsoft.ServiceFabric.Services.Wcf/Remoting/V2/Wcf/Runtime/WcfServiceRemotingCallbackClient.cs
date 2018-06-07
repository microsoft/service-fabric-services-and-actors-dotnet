// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime
{
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    internal class WcfServiceRemotingCallbackClient : IServiceRemotingCallbackClient
    {
        private readonly ServiceRemotingMessageSerializersManager serializersManager;
        private readonly IServiceRemotingCallbackContract callbackChannel;
        private IServiceRemotingMessageBodyFactory remotingMessageBodyFactory;

        public WcfServiceRemotingCallbackClient(
            IServiceRemotingCallbackContract callbackChannel,
            ServiceRemotingMessageSerializersManager serializersManager)
        {
            this.callbackChannel = callbackChannel;
            this.serializersManager = serializersManager;
            this.remotingMessageBodyFactory = serializersManager.GetSerializationProvider().CreateMessageBodyFactory();
        }

        public void SendOneWay(IServiceRemotingRequestMessage requestMessage)
        {
            IOutgoingMessageBody outgoingMessageBody = null;
            IMessageHeader outgoingMessageHeader = null;
            try
            {
                var headerSerialzier = this.serializersManager.GetHeaderSerializer();
                outgoingMessageHeader = headerSerialzier.SerializeRequestHeader(requestMessage.GetHeader());
                var requestSerializer =
                    this.serializersManager.GetRequestBodySerializer(requestMessage.GetHeader().InterfaceId);
                outgoingMessageBody = requestSerializer.Serialize(requestMessage.GetBody());
                this.callbackChannel.SendOneWay(
                    outgoingMessageHeader.GetSendBuffer(),
                    outgoingMessageBody.GetSendBuffers());
            }
            finally
            {
                if (outgoingMessageHeader != null)
                {
                    outgoingMessageHeader.Dispose();
                }

                if (outgoingMessageBody != null)
                {
                    outgoingMessageBody.Dispose();
                }
            }
        }

        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
            return this.remotingMessageBodyFactory;
        }
    }
}
