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
using Moq.Protected;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Runtime
{
    public abstract class StatefulServiceReplicaAdapterTest
    {
        readonly IStatefulServiceReplica sut;
        readonly IStateProviderReplica stateProviderReplica;
        readonly CancellationToken cancellation = TestContext.Current.CancellationToken;

        // Constructor parameters
        readonly StatefulServiceContext context = fuzzy.StatefulServiceContext();
        readonly IStatefulUserServiceReplica userServiceReplica = new Mock<IStatefulUserServiceReplica> { DefaultValue = DefaultValue.Mock }.Object;

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        protected StatefulServiceReplicaAdapterTest()
        {
            sut = new StatefulServiceReplicaAdapter(context, userServiceReplica);
            stateProviderReplica = sut.Field<IStateProviderReplica>().Value;
        }

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

            [Fact]
            public async Task ReturnsReplicatorFromStateProviderReplica()
            {
                IReplicator expected = Mock.Of<IReplicator>();
                Mock.Get(stateProviderReplica).Setup(_ => _.OpenAsync(openMode, partition, cancellation)).ReturnsAsync(expected);

                IReplicator actual = await sut.OpenAsync(openMode, partition, cancellation);

                Assert.Same(expected, actual);
            }

            [Fact]
            public async Task InvokesOnOpenAsyncOnUserServiceReplica()
            {
                await sut.OpenAsync(openMode, partition, cancellation);

                Mock.Get(userServiceReplica).Verify(_ => _.OnOpenAsync(openMode, cancellation), Times.Once);
                Mock.Get(userServiceReplica).Verify(_ => _.OnOpenAsync(It.IsAny<ReplicaOpenMode>(), It.IsAny<CancellationToken>()), Times.Once);
            }

            [Fact]
            public async Task PropagatesExceptionFromUserServiceReplicaOnOpenAsync()
            {
                var expected = new InvalidOperationException();
                Mock.Get(userServiceReplica).Setup(_ => _.OnOpenAsync(openMode, cancellation)).ThrowsAsync(expected);

                var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.OpenAsync(openMode, partition, cancellation));

                Assert.Same(expected, actual);
            }

            [Fact]
            public async Task ClosesStateProviderReplicaWhenUserServiceReplicaOnOpenAsyncThrows()
            {
                Mock.Get(userServiceReplica).Setup(_ => _.OnOpenAsync(openMode, cancellation)).ThrowsAsync(new InvalidOperationException());

                await Assert.ThrowsAsync<InvalidOperationException>(() => sut.OpenAsync(openMode, partition, cancellation));

                Mock.Get(stateProviderReplica).Verify(_ => _.CloseAsync(cancellation), Times.Once);
                Mock.Get(stateProviderReplica).Verify(_ => _.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        public sealed class ChangeRole : StatefulServiceReplicaAdapterTest
        {
            [Fact]
            public async Task ToPrimaryCreatesAndOpensCommunicationListeners()
            {
                IEnumerable<ServiceReplicaListener> replicaListeners = fuzzy.Array(fuzzy.ServiceReplicaListener);
                Mock.Get(userServiceReplica).Setup(_ => _.CreateServiceReplicaListeners()).Returns(replicaListeners);

                var createCommunicationListener = new Mock<Func<ServiceReplicaListener, StatefulServiceContext, CommunicationListenerInfo>>();
                IDictionary<ServiceReplicaListener, CommunicationListenerInfo> communicationListeners = replicaListeners.ToDictionary(_ => _, _ => fuzzy.CommunicationListenerInfo());
                createCommunicationListener.Setup(_ => _.Invoke(It.IsAny<ServiceReplicaListener>(), context))
                    .Returns((ServiceReplicaListener replicaListener, StatefulServiceContext context) => communicationListeners[replicaListener]);

                sut.Field<Func<ServiceReplicaListener, StatefulServiceContext, CommunicationListenerInfo>>().Set(createCommunicationListener.Object);

                await sut.ChangeRoleAsync(ReplicaRole.Primary, cancellation);

                IList<CommunicationListenerInfo> expected = communicationListeners.Values.ToList();
                var actual = sut.Field<IList<CommunicationListenerInfo>>().Value;
                Assert.Equal(expected, actual);
            }
        }

        public sealed class Close : StatefulServiceReplicaAdapterTest
        {
            [Fact]
            public async Task ClosesStateProviderReplica()
            {
                await sut.CloseAsync(cancellation);

                Mock.Get(stateProviderReplica).Verify(_ => _.CloseAsync(cancellation), Times.Once);
                Mock.Get(stateProviderReplica).Verify(_ => _.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
                Assert.Null(sut.Field<IStateProviderReplica>().Value);
            }

            [Fact]
            public async Task InvokesOnCloseAsyncOnUserServiceReplica()
            {
                await sut.CloseAsync(cancellation);

                Mock.Get(userServiceReplica).Verify(_ => _.OnCloseAsync(cancellation), Times.Once);
                Mock.Get(userServiceReplica).Verify(_ => _.OnCloseAsync(It.IsAny<CancellationToken>()), Times.Once);
            }

            [Fact]
            public async Task ClosesCommunicationListeners()
            {
                CommunicationListenerInfo listenerInfo = fuzzy.CommunicationListenerInfo();
                sut.Field<IList<CommunicationListenerInfo>>().Set(new List<CommunicationListenerInfo> { listenerInfo });

                await sut.CloseAsync(cancellation);

                Mock.Get(listenerInfo.Listener).Verify(_ => _.CloseAsync(cancellation), Times.Once);
                Mock.Get(listenerInfo.Listener).Verify(_ => _.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
                Assert.Null(sut.Field<IList<CommunicationListenerInfo>>().Value);
            }

            [Fact]
            public async Task PropagatesExceptionFromUserServiceReplicaOnCloseAsync()
            {
                var expected = new InvalidOperationException();
                Mock.Get(userServiceReplica).Setup(_ => _.OnCloseAsync(cancellation)).ThrowsAsync(expected);

                var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CloseAsync(cancellation));

                Assert.Same(expected, actual);
            }

            [Fact]
            public async Task ClosesStateProviderReplicaEvenWhenUserServiceReplicaOnCloseAsyncThrows()
            {
                Mock.Get(userServiceReplica).Setup(_ => _.OnCloseAsync(cancellation)).ThrowsAsync(new InvalidOperationException());

                await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CloseAsync(cancellation));

                Mock.Get(stateProviderReplica).Verify(_ => _.CloseAsync(cancellation), Times.Once);
                Mock.Get(stateProviderReplica).Verify(_ => _.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
                Assert.Null(sut.Field<IStateProviderReplica>().Value);
            }

            [Fact]
            public async Task CancelsRunAsyncEvenWhenUserServiceReplicaOnCloseAsyncThrows()
            {
                var adapter = new Mock<StatefulServiceReplicaAdapter>(context, userServiceReplica) { CallBase = true };
                Mock.Get(userServiceReplica).Setup(_ => _.OnCloseAsync(cancellation)).ThrowsAsync(new InvalidOperationException());
                IStatefulServiceReplica ssReplica = adapter.Object;

                await Assert.ThrowsAsync<InvalidOperationException>(() => ssReplica.CloseAsync(cancellation));

                adapter.Protected().Verify("CancelRunAsync", Times.Once());
            }

            [Fact]
            public async Task AggregatesExceptionsFromAllStepsWhenAllThrow()
            {
                var userEx = new InvalidOperationException();
                var stateEx = new InvalidOperationException();
                var runEx = new InvalidOperationException();
                var adapter = new Mock<StatefulServiceReplicaAdapter>(context, userServiceReplica) { CallBase = true };
                Mock.Get(userServiceReplica).Setup(_ => _.OnCloseAsync(cancellation)).ThrowsAsync(userEx);
                Mock.Get(stateProviderReplica).Setup(_ => _.CloseAsync(cancellation)).ThrowsAsync(stateEx);
                adapter.Protected().Setup<Task>("CancelRunAsync").ThrowsAsync(runEx);
                IStatefulServiceReplica ssReplica = adapter.Object;

                var actual = await Assert.ThrowsAsync<AggregateException>(() => ssReplica.CloseAsync(cancellation));

                Assert.Equal(new Exception[] { userEx, stateEx, runEx }, actual.InnerExceptions);
            }

            [Fact]
            public async Task PropagatesExceptionFromCancelRunAsync()
            {
                var runEx = new InvalidOperationException();
                var adapter = new Mock<StatefulServiceReplicaAdapter>(context, userServiceReplica) { CallBase = true };
                adapter.Protected().Setup<Task>("CancelRunAsync").ThrowsAsync(runEx);
                IStatefulServiceReplica ssReplica = adapter.Object;

                var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => ssReplica.CloseAsync(cancellation));

                Assert.Same(runEx, actual);
            }
        }
    }
}
