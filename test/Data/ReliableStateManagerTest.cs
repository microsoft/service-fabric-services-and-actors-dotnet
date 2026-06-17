// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Data.Notifications;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Data;

public abstract class ReliableStateManagerTest
{
    readonly ReliableStateManager sut;

    // Constructor parameters
    readonly Mock<IReliableStateManagerReplica2> impl = new() { DefaultValue = DefaultValue.Mock };

    // Test fixture
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ReliableStateManagerTest() 
    {
        sut = Type<ReliableStateManager>.Uninitialized();
        sut.Field<IReliableStateManagerReplica2>().Set(impl.Object);
        sut.Field<object>().Set(new object());
    }

    public sealed class Constructor_StatefulServiceContext_ReliableStateManagerConfiguration : ReliableStateManagerTest
    {
        [Fact(Explicit = true)] // TODO: SUT testability limitation. Constructor's reflection-based assembly loading prevents unit testing in isolation.
        public void DefaultsConfigurationWhenNull() =>
            // The public constructor calls FabricRuntime.LoadAssembly("Microsoft.ServiceFabric.Data.Impl") and invokes
            // EntryPoints.CreateReliableStateManager2 via reflection. Both the assembly resolution and the resulting
            // IReliableStateManagerReplica2 cannot be substituted from a test, so the configuration-defaulting behavior
            // cannot be observed in isolation without modifying the SUT to accept an injectable factory.
            throw new NotImplementedException();

        [Fact(Explicit = true)] // TODO: SUT testability limitation. Constructor's reflection-based assembly loading prevents unit testing in isolation.
        public void ThrowsFileNotFoundExceptionWhenImplAssemblyMissing() =>
            // Triggering the FileNotFoundException branch requires controlling FabricRuntime.LoadAssembly, which is a
            // static method on a non-virtual type. Without an injectable assembly loader the test cannot deterministically
            // produce a missing-assembly condition in isolation from the process's runtime environment.
            throw new NotImplementedException();

        [Fact(Explicit = true)] // TODO: SUT bug. Public constructor does not validate serviceContext. Also a testability limitation due to reflection-based assembly loading.
        public void ThrowsArgumentNullExceptionWhenServiceContextIsNull() =>
            // The constructor forwards serviceContext to CreateReliableStateManager2 via reflection without a null check,
            // so the expected ArgumentNullException is never thrown. Even after fixing the validation bug, exercising the
            // constructor in isolation is blocked by the same reflection-based assembly loading path.
            throw new NotImplementedException();
    }

    public sealed class Abort : ReliableStateManagerTest
    {
        new readonly IStateProviderReplica sut;

        public Abort() => sut = base.sut;

        [Fact]
        public void CleansUpEventHandlersBeforeDelegatingToImpl() =>
            VerifyCleansUpEventHandlersBeforeShutdown(
                callback => _ = impl.Setup(_ => _.Abort()).Callback(callback),
                sut.Abort);

        [Fact]
        public void DelegatesToImpl()
        {
            sut.Abort();
            impl.Verify(_ => _.Abort(), Times.Once);
        }

        [Fact]
        public void UnregistersTransactionEventHandler() =>
            VerifyUnregistersTransactionEventHandler(sut.Abort);

        [Fact]
        public void UnregistersStateManagerEventHandler() =>
            VerifyUnregistersStateManagerEventHandler(sut.Abort);

        [Fact]
        public void DoesNotUnregisterTransactionEventHandlerWhenNoneRegistered() =>
            VerifyDoesNotUnregisterTransactionEventHandlerWhenNoneRegistered(sut.Abort);

        [Fact]
        public void DoesNotUnregisterStateManagerEventHandlerWhenNoneRegistered() =>
            VerifyDoesNotUnregisterStateManagerEventHandlerWhenNoneRegistered(sut.Abort);
    }

    public sealed class BackupAsync_BackupOption_TimeSpan_CancellationToken_FuncOfBackupInfoCancellationTokenTaskOfBoolean : ReliableStateManagerTest
    {
        readonly TimeSpan timeout = fuzzy.TimeSpan();
        readonly CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        readonly Func<BackupInfo, CancellationToken, Task<bool>> backupCallback = (_, _) => Task.FromResult(true);

        [Theory, InlineData(BackupOption.Full), InlineData(BackupOption.Incremental)]
        public void DelegatesToImpl(BackupOption option)
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.BackupAsync(option, timeout, cancellationToken, backupCallback)).Returns(expected);

            Task actual = sut.BackupAsync(option, timeout, cancellationToken, backupCallback);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.BackupAsync(
                It.IsAny<BackupOption>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>(),
                It.IsAny<Func<BackupInfo, CancellationToken, Task<bool>>>()), Times.Once);
        }
    }

    public sealed class BackupAsync_FuncOfBackupInfoCancellationTokenTaskOfBoolean : ReliableStateManagerTest
    {
        readonly Func<BackupInfo, CancellationToken, Task<bool>> backupCallback = (_, _) => Task.FromResult(true);

        [Fact]
        public void DelegatesToImpl()
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.BackupAsync(backupCallback)).Returns(expected);

            Task actual = sut.BackupAsync(backupCallback);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.BackupAsync(It.IsAny<Func<BackupInfo, CancellationToken, Task<bool>>>()), Times.Once);
        }
    }

    public sealed class ChangeRoleAsync : ReliableStateManagerTest
    {
        new readonly IStateProviderReplica sut;

        readonly CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        public ChangeRoleAsync() => sut = base.sut;

        [Theory]
        [InlineData(ReplicaRole.Unknown)]
        [InlineData(ReplicaRole.None)]
        [InlineData(ReplicaRole.Primary)]
        [InlineData(ReplicaRole.IdleSecondary)]
        [InlineData(ReplicaRole.ActiveSecondary)]
        [InlineData(ReplicaRole.IdleAuxiliary)]
        [InlineData(ReplicaRole.ActiveAuxiliary)]
        [InlineData(ReplicaRole.PrimaryAuxiliary)]
        public void DelegatesToImpl(ReplicaRole newRole)
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.ChangeRoleAsync(newRole, cancellationToken)).Returns(expected);

            Task actual = sut.ChangeRoleAsync(newRole, cancellationToken);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.ChangeRoleAsync(It.IsAny<ReplicaRole>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    public sealed class CloseAsync : ReliableStateManagerTest
    {
        new readonly IStateProviderReplica sut;

        readonly CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        public CloseAsync() => sut = base.sut;

        [Fact]
        public void CleansUpEventHandlersBeforeDelegatingToImpl() =>
            VerifyCleansUpEventHandlersBeforeShutdown(
                callback => _ = impl.Setup(_ => _.CloseAsync(cancellationToken)).Callback(callback),
                () => _ = sut.CloseAsync(cancellationToken));

        [Fact]
        public void DelegatesToImpl()
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.CloseAsync(cancellationToken)).Returns(expected);

            Task actual = sut.CloseAsync(cancellationToken);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void UnregistersTransactionEventHandler() =>
            VerifyUnregistersTransactionEventHandler(() => _ = sut.CloseAsync(cancellationToken));

        [Fact]
        public void UnregistersStateManagerEventHandler() =>
            VerifyUnregistersStateManagerEventHandler(() => _ = sut.CloseAsync(cancellationToken));

        [Fact]
        public void DoesNotUnregisterTransactionEventHandlerWhenNoneRegistered() =>
            VerifyDoesNotUnregisterTransactionEventHandlerWhenNoneRegistered(() => _ = sut.CloseAsync(cancellationToken));

        [Fact]
        public void DoesNotUnregisterStateManagerEventHandlerWhenNoneRegistered() =>
            VerifyDoesNotUnregisterStateManagerEventHandlerWhenNoneRegistered(() => _ = sut.CloseAsync(cancellationToken));
    }

    public sealed class CreateTransaction : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        public CreateTransaction() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            var expected = Mock.Of<ITransaction>();
            _ = impl.Setup(_ => _.CreateTransaction()).Returns(expected);

            ITransaction actual = sut.CreateTransaction();

            Assert.Same(expected, actual);
            impl.Verify(_ => _.CreateTransaction(), Times.Once);
        }
    }

    public sealed class GetAsyncEnumerator : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        public GetAsyncEnumerator() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            var expected = Mock.Of<IAsyncEnumerator<IReliableState>>();
            _ = impl.Setup(_ => _.GetAsyncEnumerator()).Returns(expected);

            IAsyncEnumerator<IReliableState> actual = sut.GetAsyncEnumerator();

            Assert.Same(expected, actual);
            impl.Verify(_ => _.GetAsyncEnumerator(), Times.Once);
        }
    }

    public sealed class GetOrAddAsync_ITransaction_String : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly ITransaction tx = Mock.Of<ITransaction>();
        readonly string name = fuzzy.String();

        public GetOrAddAsync_ITransaction_String() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task<IReliableState> expected = Task.FromResult(Mock.Of<IReliableState>());
            _ = impl.Setup(_ => _.GetOrAddAsync<IReliableState>(tx, name)).Returns(expected);

            Task<IReliableState> actual = sut.GetOrAddAsync<IReliableState>(tx, name);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.GetOrAddAsync<IReliableState>(It.IsAny<ITransaction>(), It.IsAny<string>()), Times.Once);
        }
    }

    public sealed class GetOrAddAsync_ITransaction_String_TimeSpan : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly ITransaction tx = Mock.Of<ITransaction>();
        readonly string name = fuzzy.String();
        readonly TimeSpan timeout = fuzzy.TimeSpan();

        public GetOrAddAsync_ITransaction_String_TimeSpan() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task<IReliableState> expected = Task.FromResult(Mock.Of<IReliableState>());
            _ = impl.Setup(_ => _.GetOrAddAsync<IReliableState>(tx, name, timeout)).Returns(expected);

            Task<IReliableState> actual = sut.GetOrAddAsync<IReliableState>(tx, name, timeout);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.GetOrAddAsync<IReliableState>(It.IsAny<ITransaction>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }

    public sealed class GetOrAddAsync_ITransaction_Uri : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly ITransaction tx = Mock.Of<ITransaction>();
        readonly Uri name = fuzzy.Uri();

        public GetOrAddAsync_ITransaction_Uri() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task<IReliableState> expected = Task.FromResult(Mock.Of<IReliableState>());
            _ = impl.Setup(_ => _.GetOrAddAsync<IReliableState>(tx, name)).Returns(expected);

            Task<IReliableState> actual = sut.GetOrAddAsync<IReliableState>(tx, name);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.GetOrAddAsync<IReliableState>(It.IsAny<ITransaction>(), It.IsAny<Uri>()), Times.Once);
        }
    }

    public sealed class GetOrAddAsync_ITransaction_Uri_TimeSpan : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly ITransaction tx = Mock.Of<ITransaction>();
        readonly Uri name = fuzzy.Uri();
        readonly TimeSpan timeout = fuzzy.TimeSpan();

        public GetOrAddAsync_ITransaction_Uri_TimeSpan() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task<IReliableState> expected = Task.FromResult(Mock.Of<IReliableState>());
            _ = impl.Setup(_ => _.GetOrAddAsync<IReliableState>(tx, name, timeout)).Returns(expected);

            Task<IReliableState> actual = sut.GetOrAddAsync<IReliableState>(tx, name, timeout);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.GetOrAddAsync<IReliableState>(It.IsAny<ITransaction>(), It.IsAny<Uri>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }

    public sealed class GetOrAddAsync_String : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly string name = fuzzy.String();

        public GetOrAddAsync_String() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task<IReliableState> expected = Task.FromResult(Mock.Of<IReliableState>());
            _ = impl.Setup(_ => _.GetOrAddAsync<IReliableState>(name)).Returns(expected);

            Task<IReliableState> actual = sut.GetOrAddAsync<IReliableState>(name);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.GetOrAddAsync<IReliableState>(It.IsAny<string>()), Times.Once);
        }
    }

    public sealed class GetOrAddAsync_String_TimeSpan : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly string name = fuzzy.String();
        readonly TimeSpan timeout = fuzzy.TimeSpan();

        public GetOrAddAsync_String_TimeSpan() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task<IReliableState> expected = Task.FromResult(Mock.Of<IReliableState>());
            _ = impl.Setup(_ => _.GetOrAddAsync<IReliableState>(name, timeout)).Returns(expected);

            Task<IReliableState> actual = sut.GetOrAddAsync<IReliableState>(name, timeout);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.GetOrAddAsync<IReliableState>(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }

    public sealed class GetOrAddAsync_Uri : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly Uri name = fuzzy.Uri();

        public GetOrAddAsync_Uri() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task<IReliableState> expected = Task.FromResult(Mock.Of<IReliableState>());
            _ = impl.Setup(_ => _.GetOrAddAsync<IReliableState>(name)).Returns(expected);

            Task<IReliableState> actual = sut.GetOrAddAsync<IReliableState>(name);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.GetOrAddAsync<IReliableState>(It.IsAny<Uri>()), Times.Once);
        }
    }

    public sealed class GetOrAddAsync_Uri_TimeSpan : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly Uri name = fuzzy.Uri();
        readonly TimeSpan timeout = fuzzy.TimeSpan();

        public GetOrAddAsync_Uri_TimeSpan() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task<IReliableState> expected = Task.FromResult(Mock.Of<IReliableState>());
            _ = impl.Setup(_ => _.GetOrAddAsync<IReliableState>(name, timeout)).Returns(expected);

            Task<IReliableState> actual = sut.GetOrAddAsync<IReliableState>(name, timeout);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.GetOrAddAsync<IReliableState>(It.IsAny<Uri>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }

    public sealed class Initialize : ReliableStateManagerTest
    {
        new readonly IStateProviderReplica sut;

        readonly StatefulServiceInitializationParameters initializationParameters = Type<StatefulServiceInitializationParameters>.Uninitialized(); // no public ctor

        public Initialize() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            sut.Initialize(initializationParameters);

            impl.Verify(_ => _.Initialize(initializationParameters), Times.Once);
            impl.Verify(_ => _.Initialize(It.IsAny<StatefulServiceInitializationParameters>()), Times.Once);
        }
    }

    public sealed class OnDataLossAsync : ReliableStateManagerTest
    {
        readonly Func<CancellationToken, Task<bool>> handler = _ => Task.FromResult(true);

        [Fact]
        public void DelegatesToImpl()
        {
            sut.OnDataLossAsync = handler;

            impl.VerifySet(_ => _.OnDataLossAsync = handler, Times.Once);
            impl.VerifySet(_ => _.OnDataLossAsync = It.IsAny<Func<CancellationToken, Task<bool>>>(), Times.Once);
        }
    }

    public sealed class OnRestoreCompletedAsync : ReliableStateManagerTest
    {
        readonly Func<CancellationToken, Task> handler = _ => Task.CompletedTask;

        [Fact]
        public void DelegatesToImpl()
        {
            sut.OnRestoreCompletedAsync = handler;

            impl.VerifySet(_ => _.OnRestoreCompletedAsync = handler, Times.Once);
            impl.VerifySet(_ => _.OnRestoreCompletedAsync = It.IsAny<Func<CancellationToken, Task>>(), Times.Once);
        }
    }

    public sealed class OpenAsync : ReliableStateManagerTest
    {
        new readonly IStateProviderReplica sut;

        readonly IStatefulServicePartition partition = Mock.Of<IStatefulServicePartition>();
        readonly CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        public OpenAsync() => sut = base.sut;

        [Theory]
        [InlineData(ReplicaOpenMode.Invalid)]
        [InlineData(ReplicaOpenMode.New)]
        [InlineData(ReplicaOpenMode.Existing)]
        public void DelegatesToImpl(ReplicaOpenMode openMode)
        {
            Task<IReplicator> expected = Task.FromResult(Mock.Of<IReplicator>());
            _ = impl.Setup(_ => _.OpenAsync(openMode, partition, cancellationToken)).Returns(expected);

            Task<IReplicator> actual = sut.OpenAsync(openMode, partition, cancellationToken);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.OpenAsync(
                It.IsAny<ReplicaOpenMode>(), It.IsAny<IStatefulServicePartition>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    public sealed class RemoveAsync_ITransaction_String : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly ITransaction tx = Mock.Of<ITransaction>();
        readonly string name = fuzzy.String();

        public RemoveAsync_ITransaction_String() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.RemoveAsync(tx, name)).Returns(expected);

            Task actual = sut.RemoveAsync(tx, name);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.RemoveAsync(It.IsAny<ITransaction>(), It.IsAny<string>()), Times.Once);
        }
    }

    public sealed class RemoveAsync_ITransaction_String_TimeSpan : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly ITransaction tx = Mock.Of<ITransaction>();
        readonly string name = fuzzy.String();
        readonly TimeSpan timeout = fuzzy.TimeSpan();

        public RemoveAsync_ITransaction_String_TimeSpan() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.RemoveAsync(tx, name, timeout)).Returns(expected);

            Task actual = sut.RemoveAsync(tx, name, timeout);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.RemoveAsync(It.IsAny<ITransaction>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }

    public sealed class RemoveAsync_ITransaction_Uri : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly ITransaction tx = Mock.Of<ITransaction>();
        readonly Uri name = fuzzy.Uri();

        public RemoveAsync_ITransaction_Uri() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.RemoveAsync(tx, name)).Returns(expected);

            Task actual = sut.RemoveAsync(tx, name);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.RemoveAsync(It.IsAny<ITransaction>(), It.IsAny<Uri>()), Times.Once);
        }
    }

    public sealed class RemoveAsync_ITransaction_Uri_TimeSpan : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly ITransaction tx = Mock.Of<ITransaction>();
        readonly Uri name = fuzzy.Uri();
        readonly TimeSpan timeout = fuzzy.TimeSpan();

        public RemoveAsync_ITransaction_Uri_TimeSpan() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.RemoveAsync(tx, name, timeout)).Returns(expected);

            Task actual = sut.RemoveAsync(tx, name, timeout);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.RemoveAsync(It.IsAny<ITransaction>(), It.IsAny<Uri>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }

    public sealed class RemoveAsync_String : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly string name = fuzzy.String();

        public RemoveAsync_String() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.RemoveAsync(name)).Returns(expected);

            Task actual = sut.RemoveAsync(name);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.RemoveAsync(It.IsAny<string>()), Times.Once);
        }
    }

    public sealed class RemoveAsync_String_TimeSpan : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly string name = fuzzy.String();
        readonly TimeSpan timeout = fuzzy.TimeSpan();

        public RemoveAsync_String_TimeSpan() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.RemoveAsync(name, timeout)).Returns(expected);

            Task actual = sut.RemoveAsync(name, timeout);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.RemoveAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }

    public sealed class RemoveAsync_Uri : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly Uri name = fuzzy.Uri();

        public RemoveAsync_Uri() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.RemoveAsync(name)).Returns(expected);

            Task actual = sut.RemoveAsync(name);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.RemoveAsync(It.IsAny<Uri>()), Times.Once);
        }
    }

    public sealed class RemoveAsync_Uri_TimeSpan : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly Uri name = fuzzy.Uri();
        readonly TimeSpan timeout = fuzzy.TimeSpan();

        public RemoveAsync_Uri_TimeSpan() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.RemoveAsync(name, timeout)).Returns(expected);

            Task actual = sut.RemoveAsync(name, timeout);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.RemoveAsync(It.IsAny<Uri>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }

    public sealed class Replica : ReliableStateManagerTest
    {
        [Fact]
        public void ReturnsUnderlyingReplica() =>
            Assert.Same(impl.Object, sut.Replica);
    }

    public sealed class RestoreAsync_String : ReliableStateManagerTest
    {
        readonly string backupFolderPath = fuzzy.String();

        [Fact]
        public void DelegatesToImpl()
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.RestoreAsync(backupFolderPath)).Returns(expected);

            Task actual = sut.RestoreAsync(backupFolderPath);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.RestoreAsync(It.IsAny<string>()), Times.Once);
        }
    }

    public sealed class RestoreAsync_String_RestorePolicy_CancellationToken : ReliableStateManagerTest
    {
        readonly string backupFolderPath = fuzzy.String();
        readonly CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        [Theory, InlineData(RestorePolicy.Safe), InlineData(RestorePolicy.Force)]
        public void DelegatesToImpl(RestorePolicy restorePolicy)
        {
            Task expected = Task.FromResult(new object());
            _ = impl.Setup(_ => _.RestoreAsync(backupFolderPath, restorePolicy, cancellationToken)).Returns(expected);

            Task actual = sut.RestoreAsync(backupFolderPath, restorePolicy, cancellationToken);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.RestoreAsync(It.IsAny<string>(), It.IsAny<RestorePolicy>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    public sealed class StateManagerChanged : EventForwardingTest<NotifyStateManagerChangedEventArgs>
    {
        protected override void Subscribe(EventHandler<NotifyStateManagerChangedEventArgs> handler) =>
            sut.StateManagerChanged += handler;

        protected override void Unsubscribe(EventHandler<NotifyStateManagerChangedEventArgs> handler) =>
            sut.StateManagerChanged -= handler;

        protected override void SetupImplAddCapture(Action<EventHandler<NotifyStateManagerChangedEventArgs>> capture) =>
            impl.SetupAdd(_ => _.StateManagerChanged += It.IsAny<EventHandler<NotifyStateManagerChangedEventArgs>>())
                .Callback<EventHandler<NotifyStateManagerChangedEventArgs>>(capture);

        protected override void VerifyImplAdd(Func<Times> times) =>
            impl.VerifyAdd(_ => _.StateManagerChanged += It.IsAny<EventHandler<NotifyStateManagerChangedEventArgs>>(), times);

        protected override void VerifyImplRemove(Func<Times> times) =>
            impl.VerifyRemove(_ => _.StateManagerChanged -= It.IsAny<EventHandler<NotifyStateManagerChangedEventArgs>>(), times);

        protected override NotifyStateManagerChangedEventArgs NewArgs() =>
            new NotifyStateManagerRebuildEventArgs(Mock.Of<IAsyncEnumerable<IReliableState>>());
    }

    public sealed class TransactionChanged : EventForwardingTest<NotifyTransactionChangedEventArgs>
    {
        readonly NotifyTransactionChangedAction action = fuzzy.Enum<NotifyTransactionChangedAction>();

        protected override void Subscribe(EventHandler<NotifyTransactionChangedEventArgs> handler) =>
            sut.TransactionChanged += handler;

        protected override void Unsubscribe(EventHandler<NotifyTransactionChangedEventArgs> handler) =>
            sut.TransactionChanged -= handler;

        protected override void SetupImplAddCapture(Action<EventHandler<NotifyTransactionChangedEventArgs>> capture) =>
            impl.SetupAdd(_ => _.TransactionChanged += It.IsAny<EventHandler<NotifyTransactionChangedEventArgs>>())
                .Callback<EventHandler<NotifyTransactionChangedEventArgs>>(capture);

        protected override void VerifyImplAdd(Func<Times> times) =>
            impl.VerifyAdd(_ => _.TransactionChanged += It.IsAny<EventHandler<NotifyTransactionChangedEventArgs>>(), times);

        protected override void VerifyImplRemove(Func<Times> times) =>
            impl.VerifyRemove(_ => _.TransactionChanged -= It.IsAny<EventHandler<NotifyTransactionChangedEventArgs>>(), times);

        protected override NotifyTransactionChangedEventArgs NewArgs() =>
            new(Mock.Of<ITransaction>(), action);
    }

    public sealed class TryAddStateSerializer : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly IStateSerializer<int> stateSerializer = Mock.Of<IStateSerializer<int>>();

        public TryAddStateSerializer() => sut = base.sut;

        [Theory, InlineData(true), InlineData(false)]
        public void DelegatesToImpl(bool expected)
        {
            _ = impl.Setup(_ => _.TryAddStateSerializer(stateSerializer)).Returns(expected);

            bool actual = sut.TryAddStateSerializer(stateSerializer);

            Assert.Equal(expected, actual);
            impl.Verify(_ => _.TryAddStateSerializer(stateSerializer), Times.Once); // required when expected = default(bool)
            impl.Verify(_ => _.TryAddStateSerializer(It.IsAny<IStateSerializer<int>>()), Times.Once);
        }
    }

    public sealed class TryGetAsync_String : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly string name = fuzzy.String();

        public TryGetAsync_String() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task<ConditionalValue<IReliableState>> expected =
                Task.FromResult(new ConditionalValue<IReliableState>(true, Mock.Of<IReliableState>()));
            _ = impl.Setup(_ => _.TryGetAsync<IReliableState>(name)).Returns(expected);

            Task<ConditionalValue<IReliableState>> actual = sut.TryGetAsync<IReliableState>(name);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.TryGetAsync<IReliableState>(It.IsAny<string>()), Times.Once);
        }
    }

    public sealed class TryGetAsync_Uri : ReliableStateManagerTest
    {
        new readonly IReliableStateManager sut;

        readonly Uri name = fuzzy.Uri();

        public TryGetAsync_Uri() => sut = base.sut;

        [Fact]
        public void DelegatesToImpl()
        {
            Task<ConditionalValue<IReliableState>> expected =
                Task.FromResult(new ConditionalValue<IReliableState>(true, Mock.Of<IReliableState>()));
            _ = impl.Setup(_ => _.TryGetAsync<IReliableState>(name)).Returns(expected);

            Task<ConditionalValue<IReliableState>> actual = sut.TryGetAsync<IReliableState>(name);

            Assert.Same(expected, actual);
            impl.Verify(_ => _.TryGetAsync<IReliableState>(It.IsAny<Uri>()), Times.Once);
        }
    }

    public abstract class EventForwardingTest<TArgs> : ReliableStateManagerTest where TArgs : EventArgs
    {
        protected abstract void Subscribe(EventHandler<TArgs> handler);
        protected abstract void Unsubscribe(EventHandler<TArgs> handler);
        protected abstract void SetupImplAddCapture(Action<EventHandler<TArgs>> capture);
        protected abstract void VerifyImplAdd(Func<Times> times);
        protected abstract void VerifyImplRemove(Func<Times> times);
        protected abstract TArgs NewArgs();

        [Fact]
        public void SubscribesOnlyOnceForMultipleHandlers()
        {
            Subscribe((_, _) => { });
            Subscribe((_, _) => { });

            VerifyImplAdd(Times.Once);
        }

        [Fact]
        public void DoesNotSubscribeToImplWhenClosing()
        {
            ((IStateProviderReplica)sut).Abort();

            _ = Assert.Throws<FabricObjectClosedException>(() => Subscribe((_, _) => { }));

            VerifyImplAdd(Times.Never);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Subscribe throws FabricObjectClosedException after appending the user handler to the backing field, leaking it.
        public void DoesNotRetainUserHandlerWhenSubscribeFails()
        {
            ((IStateProviderReplica)sut).Abort();

            _ = Assert.Throws<FabricObjectClosedException>(() => Subscribe((_, _) => { }));

            // The add accessor does `userHandler += value` before registering with the impl, so when registration
            // throws FabricObjectClosedException the user handler has already been stored in the backing field and
            // leaks. Post-fix: the accessor should append the user handler only after a successful registration,
            // leaving the backing field null when Subscribe fails.
            Assert.Null(sut.Field<EventHandler<TArgs>>().Value);
        }

        [Fact]
        public void ForwardsNotificationsToUserHandler()
        {
            EventHandler<TArgs> actualHandler = null;
            SetupImplAddCapture(h => actualHandler = h);

            object actualSender = null;
            TArgs actualArgs = null;
            Subscribe((sender, args) => { actualSender = sender; actualArgs = args; });

            Assert.NotNull(actualHandler);
            TArgs expectedArgs = NewArgs();
            actualHandler(impl.Object, expectedArgs);
            Assert.Same(sut, actualSender);
            Assert.Same(expectedArgs, actualArgs);
        }

        [Fact]
        public void ForwardsNotificationRaisedDuringRegistration()
        {
            TArgs expectedArgs = NewArgs();
            SetupImplAddCapture(h => h(impl.Object, expectedArgs));

            object actualSender = null;
            TArgs actualArgs = null;
            Subscribe((sender, args) => { actualSender = sender; actualArgs = args; });

            Assert.Same(sut, actualSender);
            Assert.Same(expectedArgs, actualArgs);
        }

        [Fact]
        public void ForwardsNotificationsToAllUserHandlers()
        {
            EventHandler<TArgs> actualHandler = null;
            SetupImplAddCapture(h => actualHandler = h);

            bool firstCalled = false, secondCalled = false;
            Subscribe((_, _) => firstCalled = true);
            Subscribe((_, _) => secondCalled = true);

            Assert.NotNull(actualHandler);
            actualHandler(impl.Object, NewArgs());
            Assert.True(firstCalled);
            Assert.True(secondCalled);
        }

        [Fact]
        public void DoesNotThrowWhenAllUserHandlersUnsubscribed()
        {
            EventHandler<TArgs> actualHandler = null;
            SetupImplAddCapture(h => actualHandler = h);

            EventHandler<TArgs> handler = (_, _) => { };
            Subscribe(handler);
            Unsubscribe(handler);

            Assert.NotNull(actualHandler);
            actualHandler(impl.Object, NewArgs());
        }

        [Fact]
        public void UnsubscribesUserHandler()
        {
            EventHandler<TArgs> actualHandler = null;
            SetupImplAddCapture(h => actualHandler = h);

            bool called = false;
            EventHandler<TArgs> handler = (_, _) => called = true;
            Subscribe(handler);
            Unsubscribe(handler);

            Assert.NotNull(actualHandler);
            actualHandler(impl.Object, NewArgs());
            Assert.False(called);
        }

        [Fact]
        public void DoesNotUnregisterFromImplWhenUserUnsubscribes()
        {
            EventHandler<TArgs> handler = (_, _) => { };
            Subscribe(handler);

            Unsubscribe(handler);

            VerifyImplRemove(Times.Never);
        }
    }

    void VerifyCleansUpEventHandlersBeforeShutdown(Action<Action> setupShutdownCallback, Action invokeShutdown)
    {
        sut.TransactionChanged += (_, _) => { };
        sut.StateManagerChanged += (_, _) => { };
        bool txRemoved = false, smRemoved = false;
        _ = impl.SetupRemove(_ => _.TransactionChanged -= It.IsAny<EventHandler<NotifyTransactionChangedEventArgs>>())
            .Callback<EventHandler<NotifyTransactionChangedEventArgs>>(_ => txRemoved = true);
        _ = impl.SetupRemove(_ => _.StateManagerChanged -= It.IsAny<EventHandler<NotifyStateManagerChangedEventArgs>>())
            .Callback<EventHandler<NotifyStateManagerChangedEventArgs>>(_ => smRemoved = true);
        bool txRemovedBefore = false, smRemovedBefore = false;
        setupShutdownCallback(() => { txRemovedBefore = txRemoved; smRemovedBefore = smRemoved; });

        invokeShutdown();

        Assert.True(txRemovedBefore);
        Assert.True(smRemovedBefore);
    }

    void VerifyUnregistersTransactionEventHandler(Action invokeShutdown)
    {
        EventHandler<NotifyTransactionChangedEventArgs> actualAdded = null, actualRemoved = null;
        _ = impl.SetupAdd(_ => _.TransactionChanged += It.IsAny<EventHandler<NotifyTransactionChangedEventArgs>>())
            .Callback<EventHandler<NotifyTransactionChangedEventArgs>>(h => actualAdded = h);
        _ = impl.SetupRemove(_ => _.TransactionChanged -= It.IsAny<EventHandler<NotifyTransactionChangedEventArgs>>())
            .Callback<EventHandler<NotifyTransactionChangedEventArgs>>(h => actualRemoved = h);
        sut.TransactionChanged += (_, _) => { };

        invokeShutdown();

        Assert.NotNull(actualAdded);
        Assert.Equal(actualAdded, actualRemoved);
        impl.VerifyRemove(_ => _.TransactionChanged -= It.IsAny<EventHandler<NotifyTransactionChangedEventArgs>>(), Times.Once);
    }

    void VerifyUnregistersStateManagerEventHandler(Action invokeShutdown)
    {
        EventHandler<NotifyStateManagerChangedEventArgs> actualAdded = null, actualRemoved = null;
        _ = impl.SetupAdd(_ => _.StateManagerChanged += It.IsAny<EventHandler<NotifyStateManagerChangedEventArgs>>())
            .Callback<EventHandler<NotifyStateManagerChangedEventArgs>>(h => actualAdded = h);
        _ = impl.SetupRemove(_ => _.StateManagerChanged -= It.IsAny<EventHandler<NotifyStateManagerChangedEventArgs>>())
            .Callback<EventHandler<NotifyStateManagerChangedEventArgs>>(h => actualRemoved = h);
        sut.StateManagerChanged += (_, _) => { };

        invokeShutdown();

        Assert.NotNull(actualAdded);
        Assert.Equal(actualAdded, actualRemoved);
        impl.VerifyRemove(_ => _.StateManagerChanged -= It.IsAny<EventHandler<NotifyStateManagerChangedEventArgs>>(), Times.Once);
    }

    void VerifyDoesNotUnregisterTransactionEventHandlerWhenNoneRegistered(Action invokeShutdown)
    {
        invokeShutdown();
        impl.VerifyRemove(_ => _.TransactionChanged -= It.IsAny<EventHandler<NotifyTransactionChangedEventArgs>>(), Times.Never);
    }

    void VerifyDoesNotUnregisterStateManagerEventHandlerWhenNoneRegistered(Action invokeShutdown)
    {
        invokeShutdown();
        impl.VerifyRemove(_ => _.StateManagerChanged -= It.IsAny<EventHandler<NotifyStateManagerChangedEventArgs>>(), Times.Never);
    }
}
