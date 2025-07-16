// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

        readonly internal IServiceRemotingMessageHandler mockRemotingMessageHandler;
        readonly internal IServiceRemotingMessageSerializersManager mockSerializerManager;
        readonly internal IDiagnosticsSource mockDiagnosticsSource;

        readonly internal ExceptionSerializer exceptionSerializer = new ExceptionSerializer(
            new IExceptionConvertor[] { new DefaultExceptionConvertor() },
            new FabricTransportRemotingListenerSettings { RemotingExceptionDepth = 2 }
        );
        readonly internal Guid partitionId = Guid.NewGuid();
        readonly internal long replicaOrInstanceId = 123L;

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
            this.mockRemotingMessageHandler = Mock.Of<IServiceRemotingMessageHandler>();
            Mock.Get(this.mockRemotingMessageHandler).Setup(m => m.HandleRequestResponseAsync(It.IsAny<IServiceRemotingRequestContext>(), It.IsAny<IServiceRemotingRequestMessage>()))
                .Returns(Task.FromResult(Mock.Of<IServiceRemotingResponseMessage>()));

            this.mockDiagnosticsSource = Mock.Of<IDiagnosticsSource>();

            this.mockSerializerManager = Mock.Of<IServiceRemotingMessageSerializersManager>();
            Mock.Get(this.mockSerializerManager).Setup(m => m.GetHeaderSerializer())
                .Returns(mockHeaderSerializer);
            Mock.Get(this.mockSerializerManager).Setup(m => m.GetRequestBodySerializer(It.IsAny<int>()))
                .Returns(mockRequestMessageBodySerializer);
            Mock.Get(this.mockSerializerManager).Setup(m => m.GetResponseBodySerializer(It.IsAny<int>()))
                .Returns(mockResponseMessageBodySerializer);

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
            public void ShouldHave_DiagnosticsSourceField()
            {
                var handlerType = typeof(FabricTransportMessageHandler);
                var diagnosticsSourceField = handlerType
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(f => f.FieldType == typeof(IDiagnosticsSource));

                Assert.NotNull(diagnosticsSourceField);
                Assert.Equal(typeof(IDiagnosticsSource), diagnosticsSourceField.FieldType);
            }

            [Fact]
            public void ShouldInstantiate_DiagnosticsManagerAsSource()
            {
                var handlerType = typeof(FabricTransportMessageHandler);
                var diagnosticsSourceField = handlerType
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(f => f.FieldType == typeof(IDiagnosticsSource));

                Assert.NotNull(diagnosticsSourceField);
                var diagnosticsSourceInstance = diagnosticsSourceField.GetValue(sut);
                Assert.NotNull(diagnosticsSourceInstance);
                Assert.IsType<DiagnosticsManager>(diagnosticsSourceInstance);
            }
        }            
        
        public class RequestReponse : FabricTransportMessageHandlerTest
        {

            FabricTransportMessage fabricTransportMessage = new FabricTransportMessage(new FabricTransportRequestHeader(Mock.Of<Stream>()), new FabricTransportRequestBody(Mock.Of<Stream>()));
            FabricTransportRequestContext requestContext = new FabricTransportRequestContext(null, null);

            public RequestReponse()
            {
                // After creating SUT, we replace DiagnosticsSource with a mock, so we can verify diagnostics calls
                var handlerType = typeof(FabricTransportMessageHandler);
                var diagnosticsField = handlerType
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(f => f.FieldType == typeof(IDiagnosticsSource));
                diagnosticsField?.SetValue(sut, mockDiagnosticsSource);
            }

            [Fact]
            public async Task ShouldCallRequestResponseDiagnostics()
            {
                var methodStartTime = DateTime.Now;
                Mock.Get(this.mockDiagnosticsSource).Setup(d => d.OnRequestResponseBegin()).Returns(methodStartTime);

                try
                {
                    await sut.RequestResponseAsync(requestContext, fabricTransportMessage);
                }
                catch
                {
                    // Expected to fail due to mocked dependencies, but we only care about the diagnostics call
                }

                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnRequestResponseBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnRequestResponseEnd(methodStartTime), Times.Once);
            }            
            
            [Fact]
            public async Task ShouldCallTransportMessageDiagnostics()
            {
                var methodStartTime = DateTime.Now;
                Mock.Get(this.mockDiagnosticsSource).Setup(d => d.OnCreateTransportMessageSerializationBegin()).Returns(methodStartTime);

                try
                {
                    await sut.RequestResponseAsync(requestContext, fabricTransportMessage);
                }
                catch
                {
                    // Expected to fail due to mocked dependencies, but we only care about the diagnostics call
                }

                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnCreateTransportMessageSerializationBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnCreateTransportMessageSerializationEnd(methodStartTime), Times.Once);
            }            
            
            [Fact]
            public async Task ShouldCallRemotingMessageDiagnostics()
            {
                var methodStartTime = DateTime.Now;
                Mock.Get(this.mockDiagnosticsSource).Setup(d => d.OnRemotingRequestDeserializationBegin()).Returns(methodStartTime);

                try
                {
                    await sut.RequestResponseAsync(requestContext, fabricTransportMessage);
                }
                catch
                {
                    // Expected to fail due to mocked dependencies, but we only care about the diagnostics call
                }

                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnRemotingRequestDeserializationBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnRemotingRequestDeserializationEnd(methodStartTime), Times.Once);
            }
        
            [Fact]
            public async Task ShouldCallDiagnostics_InCorrectSequence()
            {
                var sequence = new MockSequence();
                var methodStartTime = DateTime.Now;
                var methodSerializationStartTime = methodStartTime.AddMinutes(1);
                var methodDeserioalizationStartTime = methodStartTime.AddMinutes(2);

                Action onDeserializationBeginCallback = () =>
                {
                    // No need for the last callback.
                };
                Action onSerializationBeginCallback = () =>
                {
                    Mock.Get(this.mockDiagnosticsSource).InSequence(sequence).Setup(d => d.OnCreateTransportMessageSerializationBegin()).Returns(methodDeserioalizationStartTime).Callback(onDeserializationBeginCallback);
                };
                Action onRequestResponseBeginCallback = () =>
                {
                    Mock.Get(this.mockDiagnosticsSource).InSequence(sequence).Setup(d => d.OnRemotingRequestDeserializationBegin()).Returns(methodSerializationStartTime).Callback(onSerializationBeginCallback);
                };

                Mock.Get(this.mockDiagnosticsSource).InSequence(sequence).Setup(d => d.OnRequestResponseBegin()).Returns(methodStartTime).Callback(onRequestResponseBeginCallback);

                try
                {
                    await sut.RequestResponseAsync(requestContext, fabricTransportMessage);
                }
                catch
                {
                    // Expected to fail due to mocked dependencies, but we only care about the diagnostics call
                }

                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnRequestResponseBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnRequestResponseEnd(methodStartTime), Times.Once);
                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnRemotingRequestDeserializationBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnRemotingRequestDeserializationEnd(methodSerializationStartTime), Times.Once);
                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnCreateTransportMessageSerializationBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnCreateTransportMessageSerializationEnd(methodDeserioalizationStartTime), Times.Once);
            }
        }
    }
}
