// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Diagnostics.Metrics;
using Microsoft.ServiceFabric.Diagnostics.Tracing;
using Microsoft.ServiceFabric.FabricTransport;
using Microsoft.ServiceFabric.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime
{
    sealed class FabricTransportMessageHandler : IFabricTransportMessageHandler
    {
        private static readonly string TraceType = typeof(FabricTransportMessageHandler).Name;
        private readonly IServiceRemotingMessageHandler remotingMessageHandler;
        private readonly IServiceRemotingMessageSerializersManager serializersManager;
        private readonly Guid partitionId;
        private readonly long replicaOrInstanceId;
        private IServiceRemotingMessageHeaderSerializer headerSerializer;
        private ExceptionSerializer exceptionSerializer;
        private IDiagnosticEvents diagnosticEvents;
        readonly IClock clock;
        readonly ServiceRemotingPerformanceCounterProvider serviceRemotingPerformanceCounterProvider;

        public FabricTransportMessageHandler(
            IServiceRemotingMessageHandler remotingMessageHandler,
            IServiceRemotingMessageSerializersManager serializersManager,
            ExceptionSerializer exceptionConvertorHandler,
            Guid partitionId,
            long replicaOrInstanceId,
            IMeterProvider<TimeSpan> meterProvider)
        {
            this.remotingMessageHandler = remotingMessageHandler;
            this.serializersManager = serializersManager;
            this.partitionId = partitionId;
            this.replicaOrInstanceId = replicaOrInstanceId;
            this.headerSerializer = this.serializersManager.GetHeaderSerializer();
            this.exceptionSerializer = exceptionConvertorHandler;

            this.clock = new SystemClock();

            this.serviceRemotingPerformanceCounterProvider = new ServiceRemotingPerformanceCounterProvider(this.partitionId, this.replicaOrInstanceId);
            var performanceCounterDiagnosticEvents = new PerformanceCounterDiagnosticEvents(serviceRemotingPerformanceCounterProvider, this.clock);
            var telemetryDiagnosticEvents = new TelemetryDiagnosticEvents(meterProvider, this.clock);

            var registeredDiagnosticsEvents = new List<IDiagnosticEvents> { performanceCounterDiagnosticEvents, telemetryDiagnosticEvents };
            this.diagnosticEvents = new AggregatedDiagnosticEvents(registeredDiagnosticsEvents);
        }

        public async Task<FabricTransportMessage> RequestResponseAsync(
            FabricTransportRequestContext requestContext,
            FabricTransportMessage fabricTransportMessage)
        {
            DateTime operationStartTime = clock.UtcNow;
            diagnosticEvents.OnRequestResponseBegin();

            IServiceRemotingRequestMessage remotingRequestMessage = null;
            try
            {
                remotingRequestMessage = this.CreateRemotingRequestMessage(fabricTransportMessage);

                LogContext.Set(new LogContext
                {
                    RequestId = remotingRequestMessage.GetHeader().RequestId,
                });

                var retval = await
                    this.remotingMessageHandler.HandleRequestResponseAsync(
                        new FabricTransportServiceRemotingRequestContext(requestContext, this.serializersManager),
                        remotingRequestMessage);
                return this.CreateFabricTransportMessage(retval, remotingRequestMessage.GetHeader().InterfaceId);
            }
            catch (Exception ex)
            {
                if (remotingRequestMessage != null)
                {
                    ServiceTrace.Source.WriteWarning(TraceType, "[{0}] Remote Exception occured {1}", remotingRequestMessage.GetHeader().RequestId, ex);
                }
                else
                {
                    ServiceTrace.Source.WriteWarning(TraceType, "Remote Exception occured {0}", ex);
                }

                return this.CreateFabricTransportExceptionMessage(ex);
            }
            finally
            {
                fabricTransportMessage.Dispose();
                diagnosticEvents.OnRequestResponseEnd(operationStartTime);
            }
        }

        public void HandleOneWay(
            FabricTransportRequestContext requestContext,
            FabricTransportMessage requesTransportMessage)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (this.remotingMessageHandler is IDisposable disposableItem)
            {
                disposableItem.Dispose();
            }
            if (this.serviceRemotingPerformanceCounterProvider != null)
            {
                serviceRemotingPerformanceCounterProvider.Dispose();
            }
            diagnosticEvents.Dispose();
        }

        private FabricTransportMessage CreateFabricTransportExceptionMessage(Exception ex)
        {
            var header = new ServiceRemotingResponseMessageHeader();
            header.AddHeader("HasRemoteException", new byte[0]);
            var serializedHeader = this.serializersManager.GetHeaderSerializer().SerializeResponseHeader(header);

            var serializedMsg = this.exceptionSerializer.SerializeRemoteException(ex);
            var msg = new FabricTransportMessage(
                new FabricTransportRequestHeader(serializedHeader.GetSendBuffer(), serializedHeader.Dispose),
                new FabricTransportRequestBody(serializedMsg, null));
            return msg;
        }

        private FabricTransportMessage CreateFabricTransportMessage(IServiceRemotingResponseMessage retval, int interfaceId)
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
                : new FabricTransportRequestHeader(default(ArraySegment<byte>), null);
            var responseSerializer =
                this.serializersManager.GetResponseBodySerializer(interfaceId);

            DateTime operationStartTime = clock.UtcNow;
            diagnosticEvents.OnCreateTransportMessageBegin();

            var responseMsgBody = responseSerializer.Serialize(retval.GetBody());
            diagnosticEvents.OnCreateTransportMessageEnd(operationStartTime);

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

        private IServiceRemotingRequestMessage CreateRemotingRequestMessage(FabricTransportMessage fabricTransportMessage)
        {
            var deSerializedHeader = this.headerSerializer.DeserializeRequestHeaders(
                new IncomingMessageHeader(fabricTransportMessage.GetHeader().GetRecievedStream()));
            var msgBodySerializer =
                 this.serializersManager.GetRequestBodySerializer(deSerializedHeader.InterfaceId);

            DateTime operationStartTime = clock.UtcNow;
            diagnosticEvents.OnRemotingRequestBegin();

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

            diagnosticEvents.OnRemotingRequestEnd(operationStartTime);

            return new ServiceRemotingRequestMessage(deSerializedHeader, deserializedMsg);
        }
    }
}
