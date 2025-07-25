// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Util;
using Microsoft.ServiceFabric.FabricTransport.V2;
using Microsoft.ServiceFabric.FabricTransport.V2.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.FabricTransport.Runtime
{
    public abstract class FabricTransportMessageHandlerTest
    {
        readonly internal FabricTransportMessageHandler sut;

        readonly internal IServiceRemotingMessageHandler mockRemotingMessageHandler = Mock.Of<IServiceRemotingMessageHandler>();
        readonly internal IServiceRemotingMessageSerializersManager mockSerializerManager = Mock.Of<IServiceRemotingMessageSerializersManager>();
        readonly internal IDiagnosticEvents mockDiagnosticEvents = Mock.Of<IDiagnosticEvents>();
        readonly internal IClock mockClock = Mock.Of<IClock>();

        readonly internal ExceptionSerializer exceptionSerializer = new ExceptionSerializer(
            new IExceptionConvertor[] { new DefaultExceptionConvertor() },
            new FabricTransportRemotingListenerSettings { RemotingExceptionDepth = 2 }
        );
        readonly internal Guid partitionId = Guid.NewGuid();
        readonly internal long replicaOrInstanceId = 123L;
        readonly internal DateTime currentTime = new DateTime(2025, 1, 1);

        public FabricTransportMessageHandlerTest()
        {
            // Local variables for mocking dependencies
            var mockRequestMessageBodySerializer = Mock.Of<IServiceRemotingRequestMessageBodySerializer>();
            var mockResponseMessageBodySerializer = Mock.Of<IServiceRemotingResponseMessageBodySerializer>();

            var mockRequestMessageHeaderSerializer = Mock.Of<IServiceRemotingRequestMessageHeader>();
            Mock.Get(mockRequestMessageHeaderSerializer).Setup(m => m.InterfaceId).Returns(0);

            var mockSerializationProvider = Mock.Of<IServiceRemotingMessageSerializationProvider>();
            Mock.Get(mockSerializationProvider).Setup(p => p.CreateResponseMessageSerializer(It.IsAny<Type>(), It.IsAny<Type[]>(), It.IsAny<Type[]>()))
                .Returns(mockResponseMessageBodySerializer);

            var mockHeaderSerializer = Mock.Of<IServiceRemotingMessageHeaderSerializer>();
            Mock.Get(mockHeaderSerializer).Setup(d => d.SerializeResponseHeader(It.IsAny<IServiceRemotingResponseMessageHeader>()))
                .Returns(Mock.Of<IMessageHeader>());
            Mock.Get(mockHeaderSerializer).Setup(d => d.DeserializeRequestHeaders(It.IsAny<IMessageHeader>()))
                .Returns(mockRequestMessageHeaderSerializer);

            // Mock needed dependencies
            Mock.Get(this.mockRemotingMessageHandler).Setup(m => m.HandleRequestResponseAsync(It.IsAny<IServiceRemotingRequestContext>(), It.IsAny<IServiceRemotingRequestMessage>()))
                .Returns(Task.FromResult(Mock.Of<IServiceRemotingResponseMessage>()));

            Mock.Get(this.mockSerializerManager).Setup(m => m.GetHeaderSerializer())
                .Returns(mockHeaderSerializer);
            Mock.Get(this.mockSerializerManager).Setup(m => m.GetRequestBodySerializer(It.IsAny<int>()))
                .Returns(mockRequestMessageBodySerializer);
            Mock.Get(this.mockSerializerManager).Setup(m => m.GetResponseBodySerializer(It.IsAny<int>()))
                .Returns(mockResponseMessageBodySerializer);

            Mock.Get(this.mockClock).Setup(c => c.UtcNow)
                .Returns(currentTime);

            // Initialize System Under Test (SUT)
            this.sut = new FabricTransportMessageHandler(
                this.mockRemotingMessageHandler,
                this.mockSerializerManager,
                exceptionSerializer,
                partitionId,
                replicaOrInstanceId);
        }

        public class Constructor : FabricTransportMessageHandlerTest
        {
            [Fact]
            public void HasDiagnosticsEventsField()
            {
                var field = sut.Field<IDiagnosticEvents>();

                Assert.IsAssignableFrom<IDiagnosticEvents>(field.Value);
                Assert.IsType<AgregatedDiagnosticEvents>(field.Value);
            }

            [Fact]
            public void DiagnosticsEventsHasPerformanceCounterEventsRegistered()
            {
                var field = sut.Field<IDiagnosticEvents>().Value;
                var registeredDiagnosticEvents = field.Field<HashSet<IDiagnosticEvents>>("diagnosticsEventSet").Value;

                Assert.Single(registeredDiagnosticEvents);
                Assert.IsType<PerformanceCounterDiagnosticEvents>(registeredDiagnosticEvents.First());
            }

            [Fact]
            public void HasClockField()
            {
                var field = sut.Field<IClock>();

                Assert.IsAssignableFrom<IClock>(field.Value);
                Assert.IsAssignableFrom<SystemClock>(field.Value);
            }
        }            
        
        public class RequestReponse : FabricTransportMessageHandlerTest
        {

            FabricTransportMessage fabricTransportMessage = new FabricTransportMessage(new FabricTransportRequestHeader(Mock.Of<Stream>()), new FabricTransportRequestBody(Mock.Of<Stream>()));
            FabricTransportRequestContext requestContext = new FabricTransportRequestContext(null, null);

            public RequestReponse()
            {
                // After creating SUT, we replace DiagnosticsSource with a mock, so we can verify diagnostics calls
                sut.Field<IDiagnosticEvents>().Set(mockDiagnosticEvents); 
                sut.Field<IClock>().Set(mockClock);
            }

            [Fact]
            public async Task ShouldCallRequestResponseDiagnostics()
            {
                try
                {
                    await sut.RequestResponseAsync(requestContext, fabricTransportMessage);
                }
                catch
                {
                    // Expected to fail due to mocked dependencies, but we only care about the diagnostics call
                }

                Mock.Get(this.mockDiagnosticEvents).Verify(d => d.OnRequestResponseBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticEvents).Verify(d => d.OnRequestResponseEnd(currentTime), Times.Once);
            }            
            
            [Fact]
            public async Task ShouldCallTransportMessageDiagnostics()
            {
                try
                {
                    await sut.RequestResponseAsync(requestContext, fabricTransportMessage);
                }
                catch
                {
                    // Expected to fail due to mocked dependencies, but we only care about the diagnostics call
                }

                Mock.Get(this.mockDiagnosticEvents).Verify(d => d.OnCreateTransportMessageBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticEvents).Verify(d => d.OnCreateTransportMessageEnd(currentTime), Times.Once);
            }            
            
            [Fact]
            public async Task ShouldCallRemotingMessageDiagnostics()
            {
                try
                {
                    await sut.RequestResponseAsync(requestContext, fabricTransportMessage);
                }
                catch
                {
                    // Expected to fail due to mocked dependencies, but we only care about the diagnostics call
                }

                Mock.Get(this.mockDiagnosticEvents).Verify(d => d.OnRemotingRequestBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticEvents).Verify(d => d.OnRemotingRequestEnd(currentTime), Times.Once);
            }
        
            [Fact]
            public async Task ShouldCallDiagnostics_InCorrectSequence()
            {
                var sequence = new MockSequence();
                var remotingMethodStartTime = currentTime.AddMinutes(1);
                var transportMethodStartTime = currentTime.AddMinutes(2);


                Action onSerializationBeginCallback = () =>
                {
                    Mock.Get(mockClock).Setup(c => c.UtcNow).Returns(transportMethodStartTime);
                };
                Action onRequestBeginCallback = () =>
                {
                    Mock.Get(mockClock).Setup(c => c.UtcNow).Returns(remotingMethodStartTime);
                    Mock.Get(this.mockDiagnosticEvents).InSequence(sequence).Setup(d => d.OnRemotingRequestBegin()).Callback(onSerializationBeginCallback);
                };
                Mock.Get(this.mockDiagnosticEvents).InSequence(sequence).Setup(d => d.OnRequestResponseBegin()).Callback(onRequestBeginCallback);

                try
                {
                    await sut.RequestResponseAsync(requestContext, fabricTransportMessage);
                }
                catch
                {
                    // Expected to fail due to mocked dependencies, but we only care about the diagnostics call
                }

                Mock.Get(this.mockDiagnosticEvents).Verify(d => d.OnRequestResponseBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticEvents).Verify(d => d.OnRequestResponseEnd(currentTime), Times.Once);
                Mock.Get(this.mockDiagnosticEvents).Verify(d => d.OnRemotingRequestBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticEvents).Verify(d => d.OnRemotingRequestEnd(remotingMethodStartTime), Times.Once);
                Mock.Get(this.mockDiagnosticEvents).Verify(d => d.OnCreateTransportMessageBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticEvents).Verify(d => d.OnCreateTransportMessageEnd(transportMethodStartTime), Times.Once);

                Mock.Get(mockClock).Setup(c => c.UtcNow).Returns(currentTime);
            }
        }
    }
}
