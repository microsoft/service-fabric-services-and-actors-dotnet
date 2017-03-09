// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport;
    using Microsoft.ServiceFabric.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;

    internal class FabricTransportMessagingHandler : IFabricTransportMessageHandler
    {
        private readonly IServiceRemotingMessageHandler messageHandler;
        private readonly DataContractSerializer serializer;

        public FabricTransportMessagingHandler(IServiceRemotingMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
            this.serializer = new DataContractSerializer(typeof(ServiceRemotingMessageHeaders));
        }

        public async Task<FabricTransportReplyMessage> RequestResponseAsync(
            FabricTransportRequestContext requestContext, byte[] headers, byte[] requestBody)
        {
            // We have Cancellation Token for Remoting layer , hence timeout is not used here.
            var messageHeaders = ServiceRemotingMessageHeaders.Deserialize(this.serializer, headers);
            var context = new FabricTransportServiceRemotingRequestContext(requestContext);
            byte[] replybody;
            try
            {
                replybody = await this.messageHandler.RequestResponseAsync(context, messageHeaders, requestBody);
                return new FabricTransportReplyMessage(false, replybody);
            }
            catch (Exception e)
            {
                ServiceTrace.Source.WriteInfo("FabricTransportCommunicationHandler", "Exception While dispatching {0}",
                    e);
                var remoteExceptionInformation = RemoteExceptionInformation.FromException(e);
                replybody = remoteExceptionInformation.Data;
                return new FabricTransportReplyMessage(true, replybody);
            }
        }


        public void HandleOneWay(FabricTransportRequestContext requestContext, byte[] headers, byte[] requestBody)
        {
            var messageHeaders = ServiceRemotingMessageHeaders.Deserialize(this.serializer, headers);
            var context = new FabricTransportServiceRemotingRequestContext(requestContext);
            this.messageHandler.HandleOneWay(context, messageHeaders, requestBody);
        }
    }
}