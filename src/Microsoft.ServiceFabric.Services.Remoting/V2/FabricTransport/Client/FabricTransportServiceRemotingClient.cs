// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport.V2;
    using Microsoft.ServiceFabric.FabricTransport.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    internal class FabricTransportServiceRemotingClient : IServiceRemotingClient
    {
        private readonly ServiceRemotingMessageSerializersManager serializersManager;
        private readonly FabricTransportClient fabricTransportClient;
        private readonly FabricTransportRemotingClientEventHandler remotingHandler;
        private ResolvedServicePartition resolvedServicePartition;
        private string listenerName;
        private ResolvedServiceEndpoint resolvedServiceEndpoint;
        private ExceptionConversionHandler exceptionConversionHandler;

        // we need to pass a cache of the serializers here rather than the known types,
        // the serializer cache should be maintained by the factor
        internal FabricTransportServiceRemotingClient(
            ServiceRemotingMessageSerializersManager serializersManager,
            FabricTransportClient fabricTransportClient,
            FabricTransportRemotingClientEventHandler remotingHandler,
            FabricTransportRemotingSettings remotingSettings,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null)
        {
            this.exceptionConversionHandler = new ExceptionConversionHandler(exceptionConvertors, remotingSettings);
            this.fabricTransportClient = fabricTransportClient;
            this.remotingHandler = remotingHandler;
            this.serializersManager = serializersManager;
            this.IsValid = true;
        }

        public bool IsValid { get; private set; }

        public object ConnectionAddress
        {
            get { return this.fabricTransportClient.ConnectionAddress; }
        }

        public ResolvedServicePartition ResolvedServicePartition
        {
            get
            {
                return this.resolvedServicePartition;
            }

            set
            {
                this.resolvedServicePartition = value;
                if (this.remotingHandler != null)
                {
                    this.remotingHandler.ResolvedServicePartition = value;
                }
            }
        }

        public string ListenerName
        {
            get
            {
                return this.listenerName;
            }

            set
            {
                this.listenerName = value;
                if (this.remotingHandler != null)
                {
                    this.remotingHandler.ListenerName = value;
                }
            }
        }

        public ResolvedServiceEndpoint Endpoint
        {
            get
            {
                return this.resolvedServiceEndpoint;
            }

            set
            {
                this.resolvedServiceEndpoint = value;
                if (this.remotingHandler != null)
                {
                    this.remotingHandler.Endpoint = value;
                }
            }
        }

        public Task OpenAsync(CancellationToken cancellationToken)
        {
            return this.fabricTransportClient.OpenAsync(cancellationToken);
        }

        public async Task<IServiceRemotingResponseMessage> RequestResponseAsync(
            IServiceRemotingRequestMessage remotingRequestRequestMessage)
        {
            var interfaceId = remotingRequestRequestMessage.GetHeader().InterfaceId;
            var serializedHeader = this.serializersManager.GetHeaderSerializer()
                .SerializeRequestHeader(remotingRequestRequestMessage.GetHeader());
            var msgBodySeriaizer = this.serializersManager.GetRequestBodySerializer(interfaceId);
            var serializedMsgBody = msgBodySeriaizer.Serialize(remotingRequestRequestMessage.GetBody());
            var fabricTransportRequestBody = serializedMsgBody != null
                ? new FabricTransportRequestBody(
                    serializedMsgBody.GetSendBuffers(),
                    serializedMsgBody.Dispose)
                : new FabricTransportRequestBody(new List<ArraySegment<byte>>(), null);

            // Send Request
            using (var retval = await this.fabricTransportClient.RequestResponseAsync(
                new FabricTransportMessage(
                    new FabricTransportRequestHeader(serializedHeader.GetSendBuffer(), serializedHeader.Dispose),
                    fabricTransportRequestBody),
                this.fabricTransportClient.Settings.OperationTimeout))
            {
                var incomingHeader = (retval != null && retval.GetHeader() != null)
                    ? new IncomingMessageHeader(retval.GetHeader().GetRecievedStream())
                    : null;

                ////DeSerialize Response
                var header =
                    this.serializersManager.GetHeaderSerializer()
                        .DeserializeResponseHeaders(
                            incomingHeader);

                if (header != null && header.TryGetHeaderValue("HasRemoteException", out var headerValue))
                {
                    await this.exceptionConversionHandler.DeserializeRemoteExceptionAndThrowAsync(retval.GetBody().GetRecievedStream());
                }

                var responseSerializer = this.serializersManager.GetResponseBodySerializer(interfaceId);
                IServiceRemotingResponseMessageBody responseMessageBody = null;
                if (retval != null && retval.GetBody() != null)
                {
                    responseMessageBody =
                        responseSerializer.Deserialize(new IncomingMessageBody(retval.GetBody().GetRecievedStream()));
                }

                return (IServiceRemotingResponseMessage)new ServiceRemotingResponseMessage(header, responseMessageBody);
            }
        }

        public void SendOneWay(IServiceRemotingRequestMessage requestMessage)
        {
            throw new NotImplementedException();
        }

        public void Abort()
        {
            this.IsValid = false;
            this.fabricTransportClient.Abort();
        }
    }
}
