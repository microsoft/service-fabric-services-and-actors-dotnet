// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport.V2;
    using Microsoft.ServiceFabric.FabricTransport.V2.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    internal class FabricTransportMessageHandler : IFabricTransportMessageHandler
    {
        private readonly IServiceRemotingMessageHandler remotingMessageHandler;
        private readonly ServiceRemotingMessageSerializersManager serializersManager;
        private readonly Guid partitionId;
        private readonly long replicaOrInstanceId;
        private readonly ServiceRemotingPerformanceCounterProvider serviceRemotingPerformanceCounterProvider;
        private IServiceRemotingMessageHeaderSerializer headerSerializer;

        public FabricTransportMessageHandler(
            IServiceRemotingMessageHandler remotingMessageHandler,
            ServiceRemotingMessageSerializersManager serializersManager,
            Guid partitionId,
            long replicaOrInstanceId)
        {
            this.remotingMessageHandler = remotingMessageHandler;
            this.serializersManager = serializersManager;
            this.partitionId = partitionId;
            this.replicaOrInstanceId = replicaOrInstanceId;
            this.serviceRemotingPerformanceCounterProvider = new ServiceRemotingPerformanceCounterProvider(
                this.partitionId,
                this.replicaOrInstanceId);
            this.headerSerializer = this.serializersManager.GetHeaderSerializer();
        }

        public async Task<FabricTransportMessage> RequestResponseAsync(
            FabricTransportRequestContext requestContext,
            FabricTransportMessage fabricTransportMessage)
        {
            if (this.serviceRemotingPerformanceCounterProvider.serviceOutstandingRequestsCounterWriter != null)
            {
                this.serviceRemotingPerformanceCounterProvider.serviceOutstandingRequestsCounterWriter
                    .UpdateCounterValue(1);
            }
            var requestStopWatch = Stopwatch.StartNew();
            var requestResponseSerializationStopwatch = Stopwatch.StartNew();

            try
            {
                var remotingRequestMessage = this.CreateRemotingRequestMessage(fabricTransportMessage, requestResponseSerializationStopwatch
                    );

                var retval = await
                    this.remotingMessageHandler.HandleRequestResponseAsync(
                        new FabricTransportServiceRemotingRequestContext(requestContext, this.serializersManager),
                        remotingRequestMessage);
                return this.CreateFabricTransportMessage(retval, remotingRequestMessage.GetHeader().InterfaceId, requestResponseSerializationStopwatch);
            }
            catch (Exception ex)
            {
                ServiceTrace.Source.WriteInfo("FabricTransportMessageHandler", "Remote Exception occured {0}", ex);
                return this.CreateFabricTransportExceptionMessage(ex);
            }
            finally
            {
                fabricTransportMessage.Dispose();
                if (this.serviceRemotingPerformanceCounterProvider.serviceOutstandingRequestsCounterWriter != null)
                {
                    this.serviceRemotingPerformanceCounterProvider.serviceOutstandingRequestsCounterWriter
                        .UpdateCounterValue(-1);
                }

                if (this.serviceRemotingPerformanceCounterProvider.serviceRequestProcessingTimeCounterWriter != null)
                {
                    this.serviceRemotingPerformanceCounterProvider.serviceRequestProcessingTimeCounterWriter
                        .UpdateCounterValue(
                            requestStopWatch.ElapsedMilliseconds);
                }
            }
        }

        private FabricTransportMessage CreateFabricTransportExceptionMessage(Exception ex)
        {
            var header = new ServiceRemotingResponseMessageHeader();
            header.AddHeader("HasRemoteException", new byte[0]);
            var serializedHeader = this.serializersManager.GetHeaderSerializer().SerializeResponseHeader(header);
            var serializedMsg = RemoteException.FromException(ex);
            var msg = new FabricTransportMessage(
                new FabricTransportRequestHeader(serializedHeader.GetSendBuffer(), serializedHeader.Dispose),
                new FabricTransportRequestBody(serializedMsg.Data, null));
            return msg;
        }

        private FabricTransportMessage CreateFabricTransportMessage(IServiceRemotingResponseMessage retval, int interfaceId, Stopwatch stopwatch)
        {
            if (retval == null)
            {
                return new FabricTransportMessage(null, null);
            }
            var responseHeader = this.headerSerializer.SerializeResponseHeader(retval.GetHeader());
            var fabricTransportRequestHeader = responseHeader != null
                ? new FabricTransportRequestHeader(
                    responseHeader.GetSendBuffer(),
                    responseHeader.Dispose)
                : new FabricTransportRequestHeader(new ArraySegment<byte>(), null);
            var responseSerializer =
                this.serializersManager.GetResponseBodySerializer(interfaceId);
            stopwatch.Restart();
            var responseMsgBody = responseSerializer.Serialize(retval.GetBody());
            if (this.serviceRemotingPerformanceCounterProvider.serviceResponseSerializationTimeCounterWriter != null)
            {
                this.serviceRemotingPerformanceCounterProvider.serviceResponseSerializationTimeCounterWriter
                    .UpdateCounterValue(stopwatch.ElapsedMilliseconds);
            }
            var fabricTransportRequestBody = responseMsgBody != null
                ? new FabricTransportRequestBody(
                    responseMsgBody.GetSendBuffers(),
                    responseMsgBody.Dispose)
                : new FabricTransportRequestBody(new List<ArraySegment<byte>>(), null);

            var message = new FabricTransportMessage(
                fabricTransportRequestHeader,
                fabricTransportRequestBody);
            return message;
        }

        private IServiceRemotingRequestMessage CreateRemotingRequestMessage(
            FabricTransportMessage fabricTransportMessage, Stopwatch stopwatch)
        {
            var deSerializedHeader = this.headerSerializer.DeserializeRequestHeaders(
                new IncomingMessageHeader(fabricTransportMessage.GetHeader().GetRecievedStream()));
            var msgBodySerializer =
                 this.serializersManager.GetRequestBodySerializer(deSerializedHeader.InterfaceId);
            stopwatch.Restart();
            IServiceRemotingRequestMessageBody deserializedMsg;
            if (fabricTransportMessage.GetBody() != null)
            {
                deserializedMsg = msgBodySerializer.Deserialize(
                   new IncomingMessageBody(fabricTransportMessage.GetBody().GetRecievedStream()));
            }
            else
            {
                deserializedMsg = null;
            }

            if (this.serviceRemotingPerformanceCounterProvider.serviceRequestDeserializationTimeCounterWriter != null)
            {
                this.serviceRemotingPerformanceCounterProvider.serviceRequestDeserializationTimeCounterWriter.UpdateCounterValue
                (
                    stopwatch.ElapsedMilliseconds);
            }
            return new ServiceRemotingRequestMessage(deSerializedHeader, deserializedMsg);
        }

        public void HandleOneWay(
            FabricTransportRequestContext requestContext,
            FabricTransportMessage requesTransportMessage)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (this.serviceRemotingPerformanceCounterProvider != null)
            {
                this.serviceRemotingPerformanceCounterProvider.Dispose();
            }
            if (this.remotingMessageHandler is IDisposable disposableItem)
            {
                disposableItem.Dispose();
            }
        }
    }
}
