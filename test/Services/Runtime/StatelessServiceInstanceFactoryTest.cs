// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Runtime;

public abstract class StatelessServiceInstanceFactoryTest
{
    readonly IStatelessServiceFactory sut;

    // Constructor parameters
    readonly RuntimeContext runtimeContext;
    readonly Func<StatelessServiceContext, StatelessService> serviceFactory;

    // Test fixture
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    protected StatelessServiceInstanceFactoryTest()
    {
        runtimeContext = Mock.Of<RuntimeContext>(ctx =>
            ctx.NodeContext == fuzzy.NodeContext() &&
            ctx.CodePackageContext == fuzzy.ICodePackageActivationContext());

        serviceFactory = Mock.Of<Func<StatelessServiceContext, StatelessService>>();

        sut = new StatelessServiceInstanceFactory(runtimeContext, serviceFactory);
    }

    public sealed class Constructor : StatelessServiceInstanceFactoryTest
    {
        [Fact]
        public void ThrowsArgumentNullExceptionWhenRuntimeContextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new StatelessServiceInstanceFactory(null, serviceFactory));
            Assert.Equal(nameof(runtimeContext), exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenServiceFactoryIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new StatelessServiceInstanceFactory(runtimeContext, null));
            Assert.Equal(nameof(serviceFactory), exception.ParamName);
        }
    }

    public sealed class CreateInstance : StatelessServiceInstanceFactoryTest
    {
        readonly string serviceTypeName = fuzzy.String();
        readonly Uri serviceName = fuzzy.Uri();
        readonly byte[] initializationData = fuzzy.Array(fuzzy.Byte);
        readonly Guid partitionId = Guid.NewGuid();
        readonly long instanceId = fuzzy.Int64();

        readonly Mock<StatelessService> service;
        StatelessServiceContext actualServiceContext;

        public CreateInstance()
        {
            service = new Mock<StatelessService>(fuzzy.StatelessServiceContext()) { DefaultValue = DefaultValue.Mock };

            Mock.Get(serviceFactory)
                .Setup(f => f(It.IsAny<StatelessServiceContext>()))
                .Callback((StatelessServiceContext ctx) => actualServiceContext = ctx)
                .Returns(service.Object);
        }

        [Fact]
        public void InvokesServiceFactory()
        {
            sut.CreateInstance(serviceTypeName, serviceName, initializationData, partitionId, instanceId);

            Assert.Same(runtimeContext.NodeContext, actualServiceContext.NodeContext);
            Assert.Same(runtimeContext.CodePackageContext, actualServiceContext.CodePackageActivationContext);
            Assert.Equal(serviceTypeName, actualServiceContext.ServiceTypeName);
            Assert.Equal(serviceName, actualServiceContext.ServiceName);
            Assert.Same(initializationData, actualServiceContext.InitializationData);
            Assert.Equal(partitionId, actualServiceContext.PartitionId);
            Assert.Equal(instanceId, actualServiceContext.InstanceId);
        }

        [Fact]
        public void ReturnsStatelessServiceInstance()
        {
            IStatelessServiceInstance result = sut.CreateInstance(serviceTypeName, serviceName, initializationData, partitionId, instanceId);

            var adapter = Assert.IsType<StatelessServiceInstanceAdapter>(result);
            Assert.Same(service.Object, adapter.Field<IStatelessUserServiceInstance>().Value);
            Assert.Same(service.Object.Context, adapter.Field<StatelessServiceContext>().Value);
        }

        [Fact]
        public void ThrowsInvalidOperationExceptionWhenServiceFactoryReturnsNull()
        {
            Mock.Get(serviceFactory)
                .Setup(f => f(It.IsAny<StatelessServiceContext>()))
                .Returns((StatelessService)null);

            Assert.Throws<InvalidOperationException>(() =>
                sut.CreateInstance(serviceTypeName, serviceName, initializationData, partitionId, instanceId));
        }
    }

    public sealed class Dispose : StatelessServiceInstanceFactoryTest
    {
        [Fact]
        public void DisposesRuntimeContext()
        {
            var codePackageContext = new Mock<ICodePackageActivationContext>();
            var runtimeContext = new RuntimeContext();
            runtimeContext.Field<ICodePackageActivationContext>().Set(codePackageContext.Object);

            var sut = new StatelessServiceInstanceFactory(runtimeContext, serviceFactory);
            sut.Dispose();

            codePackageContext.Verify(c => c.Dispose(), Times.Once);
        }
    }
}
