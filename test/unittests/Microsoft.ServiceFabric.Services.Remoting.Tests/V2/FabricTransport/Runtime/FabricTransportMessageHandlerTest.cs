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
        public class Constructor : FabricTransportMessageHandlerTest
        {
            [Fact]
            public void FabricTransportMessageHandler_ShouldHave_DiagnosticsManagerField()
            {
                var handlerType = typeof(FabricTransportMessageHandler);

                var diagnosticsSourceField = handlerType
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(f => f.FieldType == typeof(IDiagnosticsSource));

                Assert.NotNull(diagnosticsSourceField);
                Assert.Equal(typeof(IDiagnosticsSource), diagnosticsSourceField.FieldType);
            }

            [Fact]
            public void FabricTransportMessageHandler_Constructor_ShouldInstantiate_DiagnosticsManager()
            {
                var mockRemotingMessageHandler = Mock.Of<IServiceRemotingMessageHandler>();
                var mockHeaderSerializer = Mock.Of<IServiceRemotingMessageHeaderSerializer>();
                var mockSerializersManager = Mock.Of<ServiceRemotingMessageSerializersManager>();
                ExceptionSerializer mockExceptionSerializer = new ExceptionSerializer(
                    new IExceptionConvertor[] { new DefaultExceptionConvertor() },
                    new FabricTransportRemotingListenerSettings { RemotingExceptionDepth = 2 }
                );
                var partitionId = Guid.NewGuid();
                var replicaOrInstanceId = 123L;

                var handler = new FabricTransportMessageHandler(
                    mockRemotingMessageHandler,
                    mockSerializersManager,
                    mockExceptionSerializer,
                    partitionId,
                    replicaOrInstanceId);

                var handlerType = typeof(FabricTransportMessageHandler);
                var diagnosticsManagerField = handlerType
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(f => f.FieldType == typeof(IDiagnosticsSource));

                Assert.NotNull(diagnosticsManagerField);
                var diagnosticsManagerInstance = diagnosticsManagerField.GetValue(handler);
                Assert.NotNull(diagnosticsManagerInstance);
                Assert.IsType<DiagnosticsManager>(diagnosticsManagerInstance);
            }
        }            
        
        public class RequestReponse : FabricTransportMessageHandlerTest
        {
            private IServiceRemotingMessageHandler mockRemotingMessageHandler;
            private IServiceRemotingMessageSerializersManager mockSerializerManager;
            private IDiagnosticsSource mockDiagnosticsSource;

            private ExceptionSerializer exceptionSerializer = new ExceptionSerializer(
                    new IExceptionConvertor[] { new DefaultExceptionConvertor() },
                    new FabricTransportRemotingListenerSettings { RemotingExceptionDepth = 2 }
                );
            private Guid partitionId = Guid.NewGuid();
            private long replicaOrInstanceId = 123L;

            public RequestReponse()
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
            }   
    
            [Fact]
            public async Task RequestResponseAsync_ShouldCall_OnRequestResponseBeginEnd()
            {
                var fabricTransportMessage = new FabricTransportMessage(new FabricTransportRequestHeader(Mock.Of<Stream>()), new FabricTransportRequestBody(Mock.Of<Stream>()));
                var requestContext = new FabricTransportRequestContext(null, null);

                var handler = new FabricTransportMessageHandler(
                    mockRemotingMessageHandler,
                    mockSerializerManager,
                    exceptionSerializer,
                    partitionId,
                    replicaOrInstanceId);

                var methodStartTime = DateTime.Now;
                Mock.Get(this.mockDiagnosticsSource).Setup(d => d.OnRequestResponseBegin()).Returns(methodStartTime);
                Mock.Get(this.mockDiagnosticsSource).Setup(d => d.OnRequestResponseEnd(It.IsAny<DateTime>())).Verifiable();

                var handlerType = typeof(FabricTransportMessageHandler);
                var diagnosticsField = handlerType
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(f => f.FieldType == typeof(IDiagnosticsSource));
                diagnosticsField?.SetValue(handler, mockDiagnosticsSource);

                try
                {
                    await handler.RequestResponseAsync(requestContext, fabricTransportMessage);
                }
                catch
                {
                    // Expected to fail due to mocked dependencies, but we only care about the diagnostics call
                }

                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnRequestResponseBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnRequestResponseEnd(methodStartTime), Times.Once);
            }            
            
            [Fact]
            public async Task RequestResponseAsync_ShouldCall_OnTransportMessageBeginEnd()
            {
                var fabricTransportMessage = new FabricTransportMessage(new FabricTransportRequestHeader(Mock.Of<Stream>()), new FabricTransportRequestBody(Mock.Of<Stream>()));
                var requestContext = new FabricTransportRequestContext(null, null);

                var handler = new FabricTransportMessageHandler(
                    mockRemotingMessageHandler,
                    mockSerializerManager,
                    exceptionSerializer,
                    partitionId,
                    replicaOrInstanceId);

                var methodStartTime = DateTime.Now;
                Mock.Get(this.mockDiagnosticsSource).Setup(d => d.OnCreateTransportMessageSerializationBegin()).Returns(methodStartTime);
                Mock.Get(this.mockDiagnosticsSource).Setup(d => d.OnCreateTransportMessageSerializationEnd(It.IsAny<DateTime>())).Verifiable();

                var handlerType = typeof(FabricTransportMessageHandler);
                var diagnosticsField = handlerType
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(f => f.FieldType == typeof(IDiagnosticsSource));
                diagnosticsField?.SetValue(handler, this.mockDiagnosticsSource);

                try
                {
                    await handler.RequestResponseAsync(requestContext, fabricTransportMessage);
                }
                catch
                {
                    // Expected to fail due to mocked dependencies, but we only care about the diagnostics call
                }

                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnCreateTransportMessageSerializationBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnCreateTransportMessageSerializationEnd(methodStartTime), Times.Once);
            }            
            
            [Fact]
            public async Task RequestResponseAsync_ShouldCall_OnRemotingMessageBeginEnd()
            {
                var fabricTransportMessage = new FabricTransportMessage(new FabricTransportRequestHeader(Mock.Of<Stream>()), new FabricTransportRequestBody(Mock.Of<Stream>()));
                var requestContext = new FabricTransportRequestContext(null, null);

                var handler = new FabricTransportMessageHandler(
                    mockRemotingMessageHandler,
                    mockSerializerManager,
                    exceptionSerializer,
                    partitionId,
                    replicaOrInstanceId);

                var methodStartTime = DateTime.Now;
                Mock.Get(this.mockDiagnosticsSource).Setup(d => d.OnRemotingRequestDeserializationBegin()).Returns(methodStartTime);
                Mock.Get(this.mockDiagnosticsSource).Setup(d => d.OnRemotingRequestDeserializationEnd(It.IsAny<DateTime>())).Verifiable();

                var handlerType = typeof(FabricTransportMessageHandler);
                var diagnosticsField = handlerType
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(f => f.FieldType == typeof(IDiagnosticsSource));
                diagnosticsField?.SetValue(handler, this.mockDiagnosticsSource);

                try
                {
                    await handler.RequestResponseAsync(requestContext, fabricTransportMessage);
                }
                catch
                {
                    // Expected to fail due to mocked dependencies, but we only care about the diagnostics call
                }

                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnRemotingRequestDeserializationBegin(), Times.Once);
                Mock.Get(this.mockDiagnosticsSource).Verify(d => d.OnRemotingRequestDeserializationEnd(methodStartTime), Times.Once);
            }
        
            [Fact]
            public async Task RequestResponseAsync_ShouldCall_InCorrectSequence()
            {
                var fabricTransportMessage = new FabricTransportMessage(new FabricTransportRequestHeader(Mock.Of<Stream>()), new FabricTransportRequestBody(Mock.Of<Stream>()));
                var requestContext = new FabricTransportRequestContext(null, null);

                var handler = new FabricTransportMessageHandler(
                    mockRemotingMessageHandler,
                    mockSerializerManager,
                    exceptionSerializer,
                    partitionId,
                    replicaOrInstanceId);

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

                var handlerType = typeof(FabricTransportMessageHandler);
                var diagnosticsField = handlerType
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(f => f.FieldType == typeof(IDiagnosticsSource));
                diagnosticsField?.SetValue(handler, this.mockDiagnosticsSource);

                try
                {
                    await handler.RequestResponseAsync(requestContext, fabricTransportMessage);
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
