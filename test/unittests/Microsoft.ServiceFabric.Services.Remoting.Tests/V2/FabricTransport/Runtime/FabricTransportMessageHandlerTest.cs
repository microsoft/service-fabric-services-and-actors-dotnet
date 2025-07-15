// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
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
        }
        
    }
}
