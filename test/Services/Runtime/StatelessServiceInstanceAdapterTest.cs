// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Moq;
using Moq.Protected;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Runtime
{
    public abstract class StatelessServiceInstanceAdapterTest
    {
        readonly IStatelessServiceInstance sut;

        // Constructor parameters
        readonly StatelessServiceContext context = fuzzy.StatelessServiceContext();
        readonly IStatelessUserServiceInstance userServiceInstance = new Mock<IStatelessUserServiceInstance> { DefaultValue = DefaultValue.Mock }.Object;

        readonly CancellationToken cancellation = TestContext.Current.CancellationToken;

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        protected StatelessServiceInstanceAdapterTest() =>
            sut = new StatelessServiceInstanceAdapter(context, userServiceInstance);

        public sealed class Constructor : StatelessServiceInstanceAdapterTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenContextIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new StatelessServiceInstanceAdapter(null, userServiceInstance));
                Assert.Equal(nameof(context), exception.ParamName);
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenUserServiceInstanceIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new StatelessServiceInstanceAdapter(context, null));
                Assert.Equal(nameof(userServiceInstance), exception.ParamName);
            }

            [Fact]
            public void UsesServiceInstanceListenerInstantiateToCreateCommunicationListeners()
            {
                Assert.Equal(ServiceInstanceListener.Instantiate, sut.Field<Func<ServiceInstanceListener, StatelessServiceContext, CommunicationListenerInfo>>().Value);
            }
        }

        public sealed class OpenAsync : StatelessServiceInstanceAdapterTest
        {
            readonly IStatelessServicePartition partition = Mock.Of<IStatelessServicePartition>();

            [Fact]
            public async Task ToPrimaryCreatesAndOpensCommunicationListeners()
            {
                // Arrange
                IEnumerable<ServiceInstanceListener> instanceListeners = fuzzy.Array(fuzzy.ServiceInstanceListener);
                Mock.Get(userServiceInstance).Setup(_ => _.CreateServiceInstanceListeners()).Returns(instanceListeners);

                var createCommunicationListener = new Mock<Func<ServiceInstanceListener, StatelessServiceContext, CommunicationListenerInfo>>();
                IDictionary<ServiceInstanceListener, CommunicationListenerInfo> communicationListeners = instanceListeners.ToDictionary(_ => _, _ => fuzzy.CommunicationListenerInfo());
                createCommunicationListener.Setup(_ => _.Invoke(It.IsAny<ServiceInstanceListener>(), context))
                    .Returns((ServiceInstanceListener InstanceListener, StatelessServiceContext context) => communicationListeners[InstanceListener]);

                sut.Field<Func<ServiceInstanceListener, StatelessServiceContext, CommunicationListenerInfo>>().Set(createCommunicationListener.Object);

                // Act
                await sut.OpenAsync(partition, cancellation);

                // Assert
                IList<CommunicationListenerInfo> expected = communicationListeners.Values.ToList();
                var actual = sut.Field<IList<CommunicationListenerInfo>>().Value;
                Assert.Equal(expected, actual);
            }
        }

        public sealed class CloseAsync : StatelessServiceInstanceAdapterTest
        {
            [Fact]
            public async Task InvokesOnCloseAsyncOnUserServiceInstance()
            {
                await sut.CloseAsync(cancellation);

                Mock.Get(userServiceInstance).Verify(_ => _.OnCloseAsync(cancellation), Times.Once);
                Mock.Get(userServiceInstance).Verify(_ => _.OnCloseAsync(It.IsAny<CancellationToken>()), Times.Once);
            }

            [Fact]
            public async Task PropagatesExceptionFromUserServiceInstanceOnCloseAsync()
            {
                var expected = new InvalidOperationException();
                Mock.Get(userServiceInstance).Setup(_ => _.OnCloseAsync(cancellation)).ThrowsAsync(expected);

                var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CloseAsync(cancellation));

                Assert.Same(expected, actual);
            }

            [Fact]
            public async Task CallsUserServiceInstanceOnCloseEvenWhenCancelRunAsyncThrows()
            {
                var adapter = new Mock<StatelessServiceInstanceAdapter>(context, userServiceInstance) { CallBase = true };
                adapter.Protected().Setup<Task>("CancelRunAsync").ThrowsAsync(new InvalidOperationException());
                IStatelessServiceInstance ssInstance = adapter.Object;

                await Assert.ThrowsAsync<InvalidOperationException>(() => ssInstance.CloseAsync(cancellation));

                Mock.Get(userServiceInstance).Verify(_ => _.OnCloseAsync(cancellation), Times.Once);
            }

            [Fact]
            public async Task AggregatesExceptionsWhenCancelRunAsyncAndOnCloseAsyncThrow()
            {
                var runEx = new InvalidOperationException();
                var userEx = new InvalidOperationException();
                var adapter = new Mock<StatelessServiceInstanceAdapter>(context, userServiceInstance) { CallBase = true };
                adapter.Protected().Setup<Task>("CancelRunAsync").ThrowsAsync(runEx);
                Mock.Get(userServiceInstance).Setup(_ => _.OnCloseAsync(cancellation)).ThrowsAsync(userEx);
                IStatelessServiceInstance ssInstance = adapter.Object;

                var actual = await Assert.ThrowsAsync<AggregateException>(() => ssInstance.CloseAsync(cancellation));

                Assert.Equal(new Exception[] { runEx, userEx }, actual.InnerExceptions);
            }

            [Fact]
            public async Task PropagatesExceptionFromCancelRunAsync()
            {
                var runEx = new InvalidOperationException();
                var adapter = new Mock<StatelessServiceInstanceAdapter>(context, userServiceInstance) { CallBase = true };
                adapter.Protected().Setup<Task>("CancelRunAsync").ThrowsAsync(runEx);
                IStatelessServiceInstance ssInstance = adapter.Object;

                var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => ssInstance.CloseAsync(cancellation));

                Assert.Same(runEx, actual);
            }
        }
    }
}
