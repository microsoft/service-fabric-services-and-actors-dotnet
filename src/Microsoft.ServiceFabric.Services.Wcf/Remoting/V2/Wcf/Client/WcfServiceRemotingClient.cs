// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Client
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Globalization;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    internal class WcfServiceRemotingClient : IServiceRemotingClient
    {
        private ServiceRemotingMessageSerializersManager serializersManager;

        public WcfServiceRemotingClient(
            WcfCommunicationClient<IServiceRemotingContract> wcfClient,
            ServiceRemotingMessageSerializersManager serializersManager)
        {
            this.serializersManager = serializersManager;
            this.WcfClient = wcfClient;
        }

        public WcfCommunicationClient<IServiceRemotingContract> WcfClient { get; }

        /// <summary>
        /// Gets or Sets the Resolved service partition which was used when this client was created.
        /// </summary>
        /// <value><see cref="System.Fabric.ResolvedServicePartition" /> object</value>
        public ResolvedServicePartition ResolvedServicePartition
        {
            get { return this.WcfClient.ResolvedServicePartition; }

            set { this.WcfClient.ResolvedServicePartition = value; }
        }

        /// <summary>
        /// Gets or Sets the name of the listener in the replica or instance to which the client is
        /// connected to.
        /// </summary>
        public string ListenerName
        {
            get { return this.WcfClient.ListenerName; }

            set { this.WcfClient.ListenerName = value; }
        }

        /// <summary>
        /// Gets or Sets the service endpoint to which the client is connected to.
        /// </summary>
        /// <value><see cref="System.Fabric.ResolvedServiceEndpoint" /></value>
        public ResolvedServiceEndpoint Endpoint
        {
            get { return this.WcfClient.Endpoint; }

            set { this.WcfClient.Endpoint = value; }
        }

        public async Task<IServiceRemotingResponseMessage> RequestResponseAsync(IServiceRemotingRequestMessage requestMessage)
        {
            IMessageBody serializedMsgBody = null;
            IMessageHeader serializedHeader = null;

            try
            {
                // Find the Serializer
                var interfaceId = requestMessage.GetHeader().InterfaceId;
                serializedHeader = this.serializersManager.GetHeaderSerializer()
                    .SerializeRequestHeader(requestMessage.GetHeader());
                var msgBodySeriaizer = this.serializersManager.GetRequestBodySerializer(interfaceId);
                serializedMsgBody = msgBodySeriaizer.Serialize(requestMessage.GetBody());

                var responseMessage = await this.WcfClient.Channel.RequestResponseAsync(
                        serializedHeader.GetSendBuffer(),
                        serializedMsgBody == null ? new List<ArraySegment<byte>>() : serializedMsgBody.GetSendBuffers())
                    .ContinueWith(
                        t => t.GetAwaiter().GetResult(),
                        TaskScheduler.Default);

                // the code above (TaskScheduler.Default) for dispatches the responses on different thread
                // so that if the user code blocks, we do not stop the response receive pump in WCF
                var incomingHeader = (responseMessage != null && responseMessage.MessageHeaders != null)
                    ? new IncomingMessageHeader(
                        new SegmentedReadMemoryStream(responseMessage.MessageHeaders))
                    : null;

                ////DeSerialize Response Header
                var header =
                    this.serializersManager.GetHeaderSerializer()
                        .DeserializeResponseHeaders(
                            incomingHeader);

                ////DeSerialize Response Body

                var responseSerializer = this.serializersManager.GetResponseBodySerializer(interfaceId);

                var incomingMsgBody = (responseMessage != null && responseMessage.ResponseBody != null)
                    ? new IncomingMessageBody(new SegmentedReadMemoryStream(responseMessage.ResponseBody))
                    : null;

                var msgBody =
                    responseSerializer.Deserialize(incomingMsgBody);

                // Create Response Message
                return (IServiceRemotingResponseMessage)new ServiceRemotingResponseMessage(
                    header,
                    msgBody);
            }
            catch (FaultException<RemoteException> faultException)
            {
                if (RemoteException.ToException(
                    new SegmentedReadMemoryStream(faultException.Detail.Data),
                    out var remoteException))
                {
                    throw new AggregateException(remoteException);
                }

                throw new ServiceException(remoteException.GetType().FullName, string.Format(
                    CultureInfo.InvariantCulture,
                    Microsoft.ServiceFabric.Services.Wcf.SR.ErrorDeserializationFailure,
                    remoteException.ToString()));
            }
        }

        public void SendOneWay(IServiceRemotingRequestMessage requestMessage)
        {
            throw new NotImplementedException();
        }
    }
}
