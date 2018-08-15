// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport.V2;
    using Microsoft.ServiceFabric.FabricTransport.V2.Client;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
    public class FabricTransportServiceRemotingClientPublic : IServiceRemotingClient
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private readonly ServiceRemotingMessageSerializersManager serializersManager;
        private readonly FabricTransportClient fabricTransportClient;
        private readonly FabricTransportRemotingClientEventHandler remotingHandler;
        private ResolvedServicePartition resolvedServicePartition;
        private string listenerName;
        private ResolvedServiceEndpoint resolvedServiceEndpoint;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
        public FabricTransportServiceRemotingClientPublic(string endpoint)
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            var settings = new FabricTransportRemotingSettings();

            var remotingHandler = new FabricTransportRemotingClientEventHandler();
            var nativeClient = new FabricTransportClient(
                settings.GetInternalSettings(),
                endpoint,
                remotingHandler,
                null,
                new NativeFabricTransportMessageDisposer());

            var client = new FabricTransportServiceRemotingClient(
                this.serializersManager,
                nativeClient,
                remotingHandler);
            this.fabricTransportClient = nativeClient;
            this.remotingHandler = remotingHandler;
            this.serializersManager = new ServiceRemotingMessageSerializersManager(
                new ServiceRemotingDataContractSerializationProvider(),
                new ServiceRemotingMessageHeaderSerializer(new BufferPoolManager()),
                true);
            this.IsValid = true;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
        ~FabricTransportServiceRemotingClientPublic()
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            if (this.fabricTransportClient != null)
            {
                this.fabricTransportClient.Dispose();
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
        public bool IsValid { get; private set; }
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
        public object ConnectionAddress
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get { return this.fabricTransportClient.ConnectionAddress; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        /// <inheritdoc/>
        public ResolvedServicePartition ResolvedServicePartition
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        /// <inheritdoc/>
        public string ListenerName
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        /// <inheritdoc/>
        public ResolvedServiceEndpoint Endpoint
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
        public Task OpenAsync(CancellationToken cancellationToken)
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return this.fabricTransportClient.OpenAsync(cancellationToken);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        /// <inheritdoc/>
        public async Task<IServiceRemotingResponseMessage> RequestResponseAsync(
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1114 // Parameter list should follow declaration
            IServiceRemotingRequestMessage remotingRequestRequestMessage)
#pragma warning restore SA1114 // Parameter list should follow declaration
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
                    var isDeserialzied =
                        RemoteException.ToException(
                            retval.GetBody().GetRecievedStream(),
                            out var e);
                    if (isDeserialzied)
                    {
                        throw new AggregateException(e);
                    }
                    else
                    {
                        throw new ServiceException(e.GetType().FullName, string.Format(
                            CultureInfo.InvariantCulture,
                            Remoting.SR.ErrorDeserializationFailure,
                            e.ToString()));
                    }
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        /// <inheritdoc/>
        public void SendOneWay(IServiceRemotingRequestMessage requestMessage)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            throw new NotImplementedException();
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
        public void Abort()
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            this.IsValid = false;
            this.fabricTransportClient.Abort();
        }
    }
}
