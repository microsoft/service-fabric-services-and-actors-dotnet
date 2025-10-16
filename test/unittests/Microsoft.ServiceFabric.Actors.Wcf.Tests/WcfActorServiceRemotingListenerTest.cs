// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using Inspector;
using Xunit;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using System.Fabric;
using System.Fabric.Description;
using System.Collections.ObjectModel;
using Microsoft.ServiceFabric.Actors.Remoting.V2.Wcf.Runtime;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.Wcf;
using Microsoft.ServiceFabric.Actors.Remoting;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime.Tests
{
    public abstract class WcfActorServiceRemotingListenerTest
    {
        public class Constructor
        {
            protected Assembly MockAssemblyWithRemotingProviderAttribute =>
                MockAssembly(new WcfActorRemotingProviderAttribute());

            static Assembly MockAssembly(WcfActorRemotingProviderAttribute provider)
            {
                var assembly = new Mock<TestAssembly>();
                Attribute[] attributes = new[] { provider };
                assembly.Setup(_ => _.GetCustomAttributes(typeof(ActorRemotingProviderAttribute), It.IsAny<bool>())).Returns(attributes);
                return assembly.Object;
            }

            [Fact]
            public void CreatesWcfActorServiceRemotingListenerWithGivenExceptionConvertors()
            {
                // Arrange
                typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(this.MockAssemblyWithRemotingProviderAttribute);

                var exceptionConvertors = new List<IExceptionConvertor> { new SystemExceptionConvertor() };
                var settings = new WcfRemotingListenerSettings
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
                ExceptionConversionHandler actualSerializer = wcfRemotingService.Field<ExceptionConversionHandler>().Value;

                IEnumerable<IExceptionConvertor> actualConvertors = actualSerializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                Assert.NotNull(actualConvertors);

                var convertorList = new List<IExceptionConvertor>(actualConvertors);
                Assert.Equal(5, convertorList.Count);
                Assert.IsType<SystemExceptionConvertor>(convertorList[0]); // passed in
                Assert.IsType<FabricActorExceptionConvertor>(convertorList[1]); // default
                Assert.IsType<SystemExceptionConvertor>(convertorList[2]); // default
                Assert.IsType<FabricExceptionConvertor>(convertorList[3]); // default
                Assert.IsType<DefaultExceptionConvertor>(convertorList[4]); // default

                var actualSettings = actualSerializer.Field<IExceptionSerializerSettings>().Value;
                Assert.Same(settings, actualSettings);
            }
            
            // Make Assembly concrete to enable mocking on NetFx
            public class TestAssembly : Assembly { }

            public interface IDummyActor : IActor { }

            // Dummy actor type for proper ActorTypeInformation
            class DummyActor : Actor, IDummyActor
            {
                public DummyActor(ActorService service, ActorId id) : base(service, id) { }
            }
        }
    }
}
