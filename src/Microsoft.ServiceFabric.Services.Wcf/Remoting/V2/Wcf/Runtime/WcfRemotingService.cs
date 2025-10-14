// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    class WcfRemotingService : IServiceRemotingContract
    {
        private readonly IServiceRemotingMessageHandler messageHandler;

        private readonly ServiceRemotingMessageSerializersManager serializersManager;

        // The request context need not be generated every time for WCF because for WCF,
        // the actual callback channel is accessed from the current operation context.
        private readonly WcfServiceRemotingRequestContext requestContext;

        readonly ExceptionConversionHandler exceptionConversionHandler;

        readonly WcfRemotingListenerSettings listenerSettings;

        public WcfRemotingService(
            IServiceRemotingMessageHandler messageHandler,
            ServiceRemotingMessageSerializersManager serializersManager,
            ExceptionConversionHandler exceptionConversionHandler,
            WcfRemotingListenerSettings listenerSettings)
        {
            this.exceptionConversionHandler = exceptionConversionHandler;

            this.messageHandler = messageHandler;
            this.serializersManager = serializersManager;
            this.requestContext = new WcfServiceRemotingRequestContext(this.serializersManager);
            this.listenerSettings = listenerSettings;
        }

        public async Task<ResponseMessage> RequestResponseAsync(
            ArraySegment<byte> messageHeaders,
            IEnumerable<ArraySegment<byte>> requestBody)
        {
            IOutgoingMessageBody outgoingMessageBody = null;
            IMessageHeader outgoingMessageHeader = null;
            try
            {
                var headerSerializer = this.serializersManager.GetHeaderSerializer();
                var deSerializedHeader =
                    headerSerializer.DeserializeRequestHeaders(
                        new IncomingMessageHeader(new SegmentedReadMemoryStream(messageHeaders)));

                var msgBodySerializer =
                    this.serializersManager.GetRequestBodySerializer(deSerializedHeader.InterfaceId);
                var deserializedMsg =
                    msgBodySerializer.Deserialize(
                        new IncomingMessageBody(new SegmentedReadMemoryStream(requestBody)));

                var msg = new ServiceRemotingRequestMessage(deSerializedHeader, deserializedMsg);
                var retval = await
                    this.messageHandler.HandleRequestResponseAsync(
                        this.requestContext,
                        msg);

                if (retval == null)
                {
                    return new ResponseMessage();
                }

                outgoingMessageHeader = headerSerializer.SerializeResponseHeader(retval.GetHeader());

                var responseSerializer =
                    this.serializersManager.GetResponseBodySerializer(deSerializedHeader.InterfaceId);

                outgoingMessageBody = responseSerializer.Serialize(retval.GetBody());

                var responseMessage = new ResponseMessage
                {
                    ResponseBody = outgoingMessageBody != null
                    ? outgoingMessageBody.GetSendBuffers()
                    : new List<ArraySegment<byte>>(),
                    MessageHeaders = outgoingMessageHeader != null
                    ? outgoingMessageHeader.GetSendBuffer()
                    : default(ArraySegment<byte>),
                };

                return responseMessage;
            }
            catch (Exception e)
            {
                ServiceTrace.Source.WriteInfo("WcfRemotingService", "Remote Exception occured {0}", e);
#pragma warning disable 618
                if (this.listenerSettings.ExceptionSerializationTechnique == FabricTransportRemotingListenerSettings.ExceptionSerialization.BinaryFormatter)
                {
                    throw new FaultException<RemoteException>(RemoteException.FromException(e), e.Message);
                }
#pragma warning restore 618
                else
                {
                    ServiceException svcException = this.exceptionConversionHandler.ToServiceException(e);

                    throw new FaultException<RemoteException2>(this.exceptionConversionHandler.ToRemoteException(svcException));
                }
            }
        }

        public void OneWayMessage(ArraySegment<byte> messageHeaders, IEnumerable<ArraySegment<byte>> requestBody)
        {
            throw new NotImplementedException();
        }
    }
}