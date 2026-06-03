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
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Runtime
{
    public abstract class StatefulServiceReplicaAdapterTest
    {
        readonly IStatefulServiceReplica sut;

        // Constructor parameters
        readonly StatefulServiceContext context = fuzzy.StatefulServiceContext();
        readonly IStatefulUserServiceReplica userServiceReplica = new Mock<IStatefulUserServiceReplica> { DefaultValue = DefaultValue.Mock }.Object;

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        protected StatefulServiceReplicaAdapterTest() =>
            sut = new StatefulServiceReplicaAdapter(context, userServiceReplica);

        public sealed class Constructor : StatefulServiceReplicaAdapterTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenContextIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new StatefulServiceReplicaAdapter(null, userServiceReplica));
                Assert.Equal(nameof(context), exception.ParamName);
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenUserServiceReplicaIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new StatefulServiceReplicaAdapter(context, null));
                Assert.Equal(nameof(userServiceReplica), exception.ParamName);
            }

            [Fact]
            public void UsesServiceReplicaListenerInstantiateToCreateCommunicationListeners()
            {
                Assert.Equal(ServiceReplicaListener.Instantiate, sut.Field<Func<ServiceReplicaListener, StatefulServiceContext, CommunicationListenerInfo>>().Value);
            }
        }

        public sealed class Open : StatefulServiceReplicaAdapterTest
        {
            readonly ReplicaOpenMode openMode = fuzzy.Enum<ReplicaOpenMode>();
            readonly IStatefulServicePartition partition = new Mock<IStatefulServicePartition> { DefaultValue = DefaultValue.Mock }.Object;
            readonly CancellationToken cancellation = new CancellationToken();

            [Fact]
            public async Task ReturnsReplicatorFromStateProviderReplica()
            {
                // Arrange
                IStateProviderReplica stateProviderReplica = sut.Field<IStateProviderReplica>().Value;
                IReplicator expected = Mock.Of<IReplicator>();
                Mock.Get(stateProviderReplica).Setup(_ => _.OpenAsync(openMode, partition, cancellation)).ReturnsAsync(expected);

                // Act
                IReplicator actual = await sut.OpenAsync(openMode, partition, cancellation);

                // Assert
                Assert.Same(expected, actual);
            }

            [Fact]
            public async Task InvokesOnOpenAsyncOnUserServiceReplica()
            {
                // Act
                await sut.OpenAsync(openMode, partition, cancellation);

                // Assert
                Mock.Get(userServiceReplica).Verify(_ => _.OnOpenAsync(openMode, cancellation), Times.Once);
            }

            [Fact]
            public async Task PropagatesExceptionFromUserServiceReplicaOnOpenAsync()
            {
                // Arrange
                var expected = new InvalidOperationException();
                Mock.Get(userServiceReplica).Setup(_ => _.OnOpenAsync(openMode, cancellation)).ThrowsAsync(expected);

                // Act
                var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.OpenAsync(openMode, partition, cancellation));

                // Assert
                Assert.Same(expected, actual);
            }

            [Fact]
            public async Task ClosesStateProviderReplicaWhenUserServiceReplicaOnOpenAsyncThrows()
            {
                // Arrange
                IStateProviderReplica stateProviderReplica = sut.Field<IStateProviderReplica>().Value;
                Mock.Get(userServiceReplica).Setup(_ => _.OnOpenAsync(openMode, cancellation)).ThrowsAsync(new InvalidOperationException());

                // Act
                await Assert.ThrowsAsync<InvalidOperationException>(() => sut.OpenAsync(openMode, partition, cancellation));

                // Assert
                Mock.Get(stateProviderReplica).Verify(_ => _.CloseAsync(cancellation), Times.Once);
            }
        }

        public sealed class ChangeRole : StatefulServiceReplicaAdapterTest
        {
            readonly CancellationToken cancellation = new CancellationToken();

            [Fact]
            public async Task ToPrimaryCreatesAndOpensCommunicationListeners()
            {
                // Arrange
                IEnumerable<ServiceReplicaListener> replicaListeners = fuzzy.Array(fuzzy.ServiceReplicaListener);
                Mock.Get(userServiceReplica).Setup(_ => _.CreateServiceReplicaListeners()).Returns(replicaListeners);

                var createCommunicationListener = new Mock<Func<ServiceReplicaListener, StatefulServiceContext, CommunicationListenerInfo>>();
                IDictionary<ServiceReplicaListener, CommunicationListenerInfo> communicationListeners = replicaListeners.ToDictionary(_ => _, _ => fuzzy.CommunicationListenerInfo());
                createCommunicationListener.Setup(_ => _.Invoke(It.IsAny<ServiceReplicaListener>(), context))
                    .Returns((ServiceReplicaListener replicaListener, StatefulServiceContext context) => communicationListeners[replicaListener]);

                sut.Field<Func<ServiceReplicaListener, StatefulServiceContext, CommunicationListenerInfo>>().Set(createCommunicationListener.Object);

                // Act
                await sut.ChangeRoleAsync(ReplicaRole.Primary, cancellation);

                // Assert
                IList<CommunicationListenerInfo> expected = communicationListeners.Values.ToList();
                var actual = sut.Field<IList<CommunicationListenerInfo>>().Value;
                Assert.Equal(expected, actual);
            }
        }

        public sealed class Close : StatefulServiceReplicaAdapterTest
        {
            readonly CancellationToken cancellation = new CancellationToken();

            [Fact]
            public async Task ClosesStateProviderReplica()
            {
                // Arrange
                IStateProviderReplica stateProviderReplica = sut.Field<IStateProviderReplica>().Value;

                // Act
                await sut.CloseAsync(cancellation);

                // Assert
                Mock.Get(stateProviderReplica).Verify(_ => _.CloseAsync(cancellation), Times.Once);
                Assert.Null(sut.Field<IStateProviderReplica>().Value);
            }

            [Fact]
            public async Task InvokesOnCloseAsyncOnUserServiceReplica()
            {
                // Act
                await sut.CloseAsync(cancellation);

                // Assert
                Mock.Get(userServiceReplica).Verify(_ => _.OnCloseAsync(cancellation), Times.Once);
            }

            [Fact]
            public async Task ClosesCommunicationListeners()
            {
                // Arrange
                CommunicationListenerInfo listenerInfo = fuzzy.CommunicationListenerInfo();
                sut.Field<IList<CommunicationListenerInfo>>().Set(new List<CommunicationListenerInfo> { listenerInfo });

                // Act
                await sut.CloseAsync(cancellation);

                // Assert
                Mock.Get(listenerInfo.Listener).Verify(_ => _.CloseAsync(cancellation), Times.Once);
                Assert.Null(sut.Field<IList<CommunicationListenerInfo>>().Value);
            }

            [Fact]
            public async Task PropagatesExceptionFromUserServiceReplicaOnCloseAsync()
            {
                // Arrange
                var expected = new InvalidOperationException();
                Mock.Get(userServiceReplica).Setup(_ => _.OnCloseAsync(cancellation)).ThrowsAsync(expected);

                // Act
                var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CloseAsync(cancellation));

                // Assert
                Assert.Same(expected, actual);
            }

            [Fact]
            public async Task ClosesStateProviderReplicaEvenWhenUserServiceReplicaOnCloseAsyncThrows()
            {
                // Arrange
                IStateProviderReplica stateProviderReplica = sut.Field<IStateProviderReplica>().Value;
                Mock.Get(userServiceReplica).Setup(_ => _.OnCloseAsync(cancellation)).ThrowsAsync(new InvalidOperationException());

                // Act
                await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CloseAsync(cancellation));

                // Assert
                Mock.Get(stateProviderReplica).Verify(_ => _.CloseAsync(cancellation), Times.Once);
                Assert.Null(sut.Field<IStateProviderReplica>().Value);
            }

            [Fact]
            public async Task CancelsRunAsyncEvenWhenUserServiceReplicaOnCloseAsyncThrows()
            {
                // Arrange
                sut.Field<CancellationTokenSource>().Set(new CancellationTokenSource());
                Mock.Get(userServiceReplica).Setup(_ => _.OnCloseAsync(cancellation)).ThrowsAsync(new InvalidOperationException());

                // Act
                await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CloseAsync(cancellation));

                // Assert
                Assert.Null(sut.Field<CancellationTokenSource>().Value);
            }
        }
    }
}
