// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Data;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Runtime;

public abstract class StatefulServiceReplicaFactoryTest
{
    readonly IStatefulServiceFactory sut;

    // Constructor parameters
    readonly RuntimeContext runtimeContext;
    readonly Func<StatefulServiceContext, StatefulServiceBase> serviceFactory;

    // Test fixture
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    protected StatefulServiceReplicaFactoryTest()
    {
        runtimeContext = Mock.Of<RuntimeContext>(ctx =>
            ctx.NodeContext == fuzzy.NodeContext() &&
            ctx.CodePackageContext == fuzzy.ICodePackageActivationContext());

        serviceFactory = Mock.Of<Func<StatefulServiceContext, StatefulServiceBase>>();

        sut = new StatefulServiceReplicaFactory(runtimeContext, serviceFactory);
    }

    public sealed class Constructor : StatefulServiceReplicaFactoryTest
    {
        [Fact]
        public void ThrowsArgumentNullExceptionWhenRuntimeContextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new StatefulServiceReplicaFactory(null, serviceFactory));
            Assert.Equal(nameof(runtimeContext), exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenServiceFactoryIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new StatefulServiceReplicaFactory(runtimeContext, null));
            Assert.Equal(nameof(serviceFactory), exception.ParamName);
        }
    }

    public sealed class CreateReplica : StatefulServiceReplicaFactoryTest
    {
        readonly string serviceTypeName = fuzzy.String();
        readonly Uri serviceName = fuzzy.Uri();
        readonly byte[] initializationData = fuzzy.Array(fuzzy.Byte);
        readonly Guid partitionId = Guid.NewGuid();
        readonly long replicaId = fuzzy.Int64();

        readonly Mock<StatefulServiceBase> service;
        StatefulServiceContext actualServiceContext;

        public CreateReplica()
        {
            service = new Mock<StatefulServiceBase>(
                fuzzy.StatefulServiceContext(),
                Mock.Of<IStateProviderReplica>())
            {
                DefaultValue = DefaultValue.Mock,
            };

            Mock.Get(serviceFactory)
                .Setup(f => f(It.IsAny<StatefulServiceContext>()))
                .Callback((StatefulServiceContext ctx) => actualServiceContext = ctx)
                .Returns(service.Object);
        }

        [Fact]
        public void InvokesServiceFactory()
        {
            sut.CreateReplica(serviceTypeName, serviceName, initializationData, partitionId, replicaId);

            Assert.Same(runtimeContext.NodeContext, actualServiceContext.NodeContext);
            Assert.Same(runtimeContext.CodePackageContext, actualServiceContext.CodePackageActivationContext);
            Assert.Equal(serviceTypeName, actualServiceContext.ServiceTypeName);
            Assert.Equal(serviceName, actualServiceContext.ServiceName);
            Assert.Same(initializationData, actualServiceContext.InitializationData);
            Assert.Equal(partitionId, actualServiceContext.PartitionId);
            Assert.Equal(replicaId, actualServiceContext.ReplicaId);
        }

        [Fact]
        public void ReturnsStatefulServiceReplica()
        {
            IStatefulServiceReplica result = sut.CreateReplica(serviceTypeName, serviceName, initializationData, partitionId, replicaId);

            var adapter = Assert.IsType<StatefulServiceReplicaAdapter>(result);
            Assert.Same(service.Object, adapter.Field<IStatefulUserServiceReplica>().Value);
            Assert.Same(service.Object.Context, adapter.Field<StatefulServiceContext>().Value);
        }

        [Fact]
        public void ThrowsInvalidOperationExceptionWhenServiceFactoryReturnsNull()
        {
            Mock.Get(serviceFactory)
                .Setup(f => f(It.IsAny<StatefulServiceContext>()))
                .Returns((StatefulServiceBase)null);

            Assert.Throws<InvalidOperationException>(() =>
                sut.CreateReplica(serviceTypeName, serviceName, initializationData, partitionId, replicaId));
        }
    }

    public sealed class Dispose : StatefulServiceReplicaFactoryTest
    {
        [Fact]
        public void DisposesRuntimeContext()
        {
            var codePackageContext = new Mock<ICodePackageActivationContext>();
            var runtimeContext = new RuntimeContext();
            runtimeContext.Field<ICodePackageActivationContext>().Set(codePackageContext.Object);

            var sut = new StatefulServiceReplicaFactory(runtimeContext, serviceFactory);
            sut.Dispose();

            codePackageContext.Verify(c => c.Dispose(), Times.Once);
        }
    }
}
