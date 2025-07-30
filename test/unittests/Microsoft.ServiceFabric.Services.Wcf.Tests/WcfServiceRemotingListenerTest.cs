// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Moq;
using Inspector;
using Xunit;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using System.Fabric;
using System.Fabric.Description;
using System.Collections.ObjectModel;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime.Tests
{
    public abstract class WcfServiceRemotingListenerTest
    {
        public class Constructor
        {
            [Fact]
            public void CreatesWcfRemotingServiceWithGivenExceptionConvertors()
            {
                // Arrange
                var exceptionConvertors = new List<IExceptionConvertor> { new SystemExceptionConvertor() };
                var settings = new FabricTransportRemotingListenerSettings
                {
                    RemotingExceptionDepth = 4
                };

                var mockEndpointsCollection = Mock.Of<KeyedCollection<string, EndpointResourceDescription>>();
                var mockCodePackageActivationContext = Mock.Of<ICodePackageActivationContext>();
                Mock.Get(mockCodePackageActivationContext).Setup(c => c.GetEndpoints()).Returns(mockEndpointsCollection);
                Mock.Get(mockCodePackageActivationContext).Setup(c => c.ApplicationName).Returns("fabric:/DummyApp");
                Mock.Get(mockCodePackageActivationContext).Setup(c => c.ApplicationTypeName).Returns("fabric:/DummyApp");

                var mockServiceContext = new Mock<ServiceContext>(
                    new NodeContext("NodeName", new NodeId(0, 1), 0, "NodeType", "127.0.0.1"),
                    mockCodePackageActivationContext,
                    "DummyServiceType",
                    new Uri("fabric:/DummyApp/DummyService"),
                    null,
                    Guid.NewGuid(),
                    1L
                );

                var mockSerializationProvider = Mock.Of<IServiceRemotingMessageSerializationProvider>();

                // Act
                var listener = new WcfServiceRemotingListener(
                    serviceContext: mockServiceContext.Object,
                    messageHandler: null,
                    serializationProvider: mockSerializationProvider,
                    listenerBinding: null,
                    endpointResourceName: null,
                    useWrappedMessage: false,
                    exceptionConvertors: exceptionConvertors,
                    settings: settings
                );

                // Assert
                WcfRemotingService wcfRemotingService = listener.Field<WcfRemotingService>().Value;
                ExceptionSerializer actualSerializer = wcfRemotingService.Field<ExceptionSerializer>().Value;

                IEnumerable<IExceptionConvertor> actualConvertors = actualSerializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                Assert.NotNull(actualConvertors);

                var convertorList = new List<IExceptionConvertor>(actualConvertors);
                Assert.Equal(4, convertorList.Count);
                Assert.IsType<SystemExceptionConvertor>(convertorList[0]); // passed in
                Assert.IsType<SystemExceptionConvertor>(convertorList[1]); // default
                Assert.IsType<FabricExceptionConvertor>(convertorList[2]); // default
                Assert.IsType<DefaultExceptionConvertor>(convertorList[3]); // default

                var actualSettings = actualSerializer.Field<FabricTransportRemotingListenerSettings>().Value;
                Assert.Same(settings, actualSettings);
            }
        }
    }
}
