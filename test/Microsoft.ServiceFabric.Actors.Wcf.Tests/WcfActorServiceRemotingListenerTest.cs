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
using Microsoft.ServiceFabric.Actors.Remoting.V2.Wcf.Runtime;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime.Tests
{
    public abstract class WcfActorServiceRemotingListenerTest
    {
        public class Constructor
        {
            [Fact]
            public void CreatesWcfActorServiceRemotingListenerWithGivenExceptionConvertors()
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

                var statefulServiceContext = new StatefulServiceContext(
                    new NodeContext("NodeName", new NodeId(0, 1), 0, "NodeType", "127.0.0.1"),
                    mockCodePackageActivationContext,
                    "DummyServiceType",
                    new Uri("fabric:/DummyApp/DummyService"),
                    null,
                    Guid.NewGuid(),
                    1L
                );

                ActorTypeInformation actorTypeInfo = ActorTypeInformation.Get(typeof(DummyActor));

                ActorService dummyActorService = new ActorService(
                    statefulServiceContext,
                    actorTypeInfo,
                    null,
                    null,
                    null,
                    new ActorServiceSettings());

                // Act
                var listener = new WcfActorServiceRemotingListener(
                    actorService: dummyActorService,
                    listenerBinding: null,
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
                Assert.Equal(5, convertorList.Count);
                Assert.IsType<SystemExceptionConvertor>(convertorList[0]); // passed in
                Assert.IsType<FabricActorExceptionConvertor>(convertorList[1]); // default
                Assert.IsType<SystemExceptionConvertor>(convertorList[2]); // default
                Assert.IsType<FabricExceptionConvertor>(convertorList[3]); // default
                Assert.IsType<DefaultExceptionConvertor>(convertorList[4]); // default

                var actualSettings = actualSerializer.Field<FabricTransportRemotingListenerSettings>().Value;
                Assert.Same(settings, actualSettings);
            }
        }
    }

    public interface IDummyActor : IActor { }

    // Dummy actor type for proper ActorTypeInformation
    class DummyActor : Actor, IDummyActor
    {
        public DummyActor(ActorService service, ActorId id) : base(service, id) { }
    }
}
