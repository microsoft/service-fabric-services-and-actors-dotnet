// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Diagnostics.Metrics;
using Microsoft.ServiceFabric.FabricTransport.V2;
using Microsoft.ServiceFabric.FabricTransport.V2.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.TestFramework;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime
{
    public abstract class FabricTransportMessageHandlerTest : MockedMetricsTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly FabricTransportMessageHandler sut;

        readonly IDiagnosticEvents diagnosticEvents = Mock.Of<IDiagnosticEvents>();
        readonly DateTime currentTime = fuzzy.DateTime();
        readonly IClock clock = Mock.Of<IClock>();


        public FabricTransportMessageHandlerTest()
        {
            Mock.Get(this.clock).Setup(c => c.UtcNow)
                .Returns(currentTime);

            this.sut = new FabricTransportMessageHandler(
                new Mock<IServiceRemotingMessageHandler>() { DefaultValue = DefaultValue.Mock }.Object,
                new Mock<IServiceRemotingMessageSerializersManager>() { DefaultValue = DefaultValue.Mock }.Object,
                new ExceptionSerializer(
                    new IExceptionConvertor[] { new DefaultExceptionConvertor() },
                    new FabricTransportRemotingListenerSettings { RemotingExceptionDepth = 2 }
                ),
                Guid.NewGuid(),
                fuzzy.Int64(),
                Mock.Of<IMeterProvider<TimeSpan>>());
        }

        public class Constructor : FabricTransportMessageHandlerTest
        {
            [Fact]
            public void HasDiagnosticsEventsField()
            {
                var field = sut.Field<IDiagnosticEvents>();

                Assert.IsType<AggregatedDiagnosticEvents>(field.Value);
            }

            [Fact]
            public void DiagnosticsEventsHasPerformanceCounterEventsRegistered()
            {
                var field = sut.Field<IDiagnosticEvents>().Value;
                var registeredDiagnosticEvents = field.Field<IEnumerable<IDiagnosticEvents>>().Value;

                Assert.Equal(2, registeredDiagnosticEvents.Count());
                Assert.IsType<PerformanceCounterDiagnosticEvents>(registeredDiagnosticEvents.ToList()[0]);
                Assert.IsType<TelemetryDiagnosticEvents>(registeredDiagnosticEvents.ToList()[1]);
            }

            [Fact]
            public void HasClockField()
            {
                var field = sut.Field<IClock>();

                Assert.IsAssignableFrom<SystemClock>(field.Value);
            }
        }

        public class RequestReponse : FabricTransportMessageHandlerTest
        {

            FabricTransportMessage fabricTransportMessage = new FabricTransportMessage(new FabricTransportRequestHeader(Mock.Of<Stream>()), new FabricTransportRequestBody(Mock.Of<Stream>()));
            FabricTransportRequestContext requestContext = new FabricTransportRequestContext(null, null);

            public RequestReponse()
            {
                // After creating SUT, we replace DiagnosticsSource with a mock
                sut.Field<IDiagnosticEvents>().Set(diagnosticEvents);
                sut.Field<IClock>().Set(clock);
            }

            [Fact]
            public async Task ShouldCallRequestResponseDiagnostics()
            {
                await sut.RequestResponseAsync(requestContext, fabricTransportMessage);

                Mock.Get(this.diagnosticEvents).Verify(d => d.OnRequestResponseBegin(), Times.Once);
                Mock.Get(this.diagnosticEvents).Verify(d => d.OnRequestResponseEnd(currentTime), Times.Once);
            }

            [Fact]
            public async Task ShouldCallTransportMessageDiagnostics()
            {
                await sut.RequestResponseAsync(requestContext, fabricTransportMessage);

                Mock.Get(this.diagnosticEvents).Verify(d => d.OnCreateTransportMessageBegin(), Times.Once);
                Mock.Get(this.diagnosticEvents).Verify(d => d.OnCreateTransportMessageEnd(currentTime), Times.Once);
            }

            [Fact]
            public async Task ShouldCallRemotingMessageDiagnostics()
            {
                await sut.RequestResponseAsync(requestContext, fabricTransportMessage);

                Mock.Get(this.diagnosticEvents).Verify(d => d.OnRemotingRequestBegin(), Times.Once);
                Mock.Get(this.diagnosticEvents).Verify(d => d.OnRemotingRequestEnd(currentTime), Times.Once);
            }

            [Fact]
            public async Task ShouldCallDiagnostics_InCorrectSequence()
            {
                var sequence = new MockSequence();
                var remotingMethodStartTime = currentTime.AddMinutes(1);
                var transportMethodStartTime = currentTime.AddMinutes(2);

                Action onSerializationBeginCallback = () =>
                {
                    Mock.Get(clock).Setup(c => c.UtcNow).Returns(transportMethodStartTime);
                };
                Action onRequestBeginCallback = () =>
                {
                    Mock.Get(clock).Setup(c => c.UtcNow).Returns(remotingMethodStartTime);
                    Mock.Get(this.diagnosticEvents).InSequence(sequence).Setup(d => d.OnRemotingRequestBegin()).Callback(onSerializationBeginCallback);
                };
                Mock.Get(this.diagnosticEvents).InSequence(sequence).Setup(d => d.OnRequestResponseBegin()).Callback(onRequestBeginCallback);

                await sut.RequestResponseAsync(requestContext, fabricTransportMessage);

                Mock.Get(this.diagnosticEvents).Verify(d => d.OnRequestResponseBegin(), Times.Once);
                Mock.Get(this.diagnosticEvents).Verify(d => d.OnRequestResponseEnd(currentTime), Times.Once);
                Mock.Get(this.diagnosticEvents).Verify(d => d.OnRemotingRequestBegin(), Times.Once);
                Mock.Get(this.diagnosticEvents).Verify(d => d.OnRemotingRequestEnd(remotingMethodStartTime), Times.Once);
                Mock.Get(this.diagnosticEvents).Verify(d => d.OnCreateTransportMessageBegin(), Times.Once);
                Mock.Get(this.diagnosticEvents).Verify(d => d.OnCreateTransportMessageEnd(transportMethodStartTime), Times.Once);

                // cleanup
                Mock.Get(clock).Setup(c => c.UtcNow).Returns(currentTime);
            }
        }
    }
}
