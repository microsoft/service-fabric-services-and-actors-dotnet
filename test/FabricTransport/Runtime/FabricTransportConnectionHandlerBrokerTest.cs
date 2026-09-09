// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Fuzzy;
using Moq;
using Xunit;
using static System.Fabric.Interop.NativeCommon;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime;

public abstract class FabricTransportConnectionHandlerBrokerTest
{
    readonly NativeFabricTransport.IFabricTransportConnectionHandler sut;

    // Constructor parameters
    readonly Mock<IFabricTransportConnectionHandler> serviceConnectionHandler = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);
    static readonly TimeSpan callbackWait = TimeSpan.FromSeconds(5);

    FabricTransportConnectionHandlerBrokerTest() =>
        sut = new FabricTransportConnectionHandlerBroker(serviceConnectionHandler.Object);

    public sealed class Constructor: FabricTransportConnectionHandlerBrokerTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate serviceConnectionHandler.
        public void ThrowsArgumentNullExceptionWhenServiceConnectionHandlerIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new FabricTransportConnectionHandlerBroker(null));
            Assert.Equal(nameof(serviceConnectionHandler), exception.ParamName);
        }
    }

    [WindowsOnly("Can't load libFabricCommon.so on Linux.")]
    public sealed class BeginProcessConnect: FabricTransportConnectionHandlerBrokerTest, IDisposable
    {
        // Method parameters
        readonly NativeFabricTransport.IFabricTransportClientConnection nativeClientConnection;
        readonly uint timeoutMilliseconds = fuzzy.UInt32();
        readonly IFabricAsyncOperationCallback callback = Mock.Of<IFabricAsyncOperationCallback>();

        readonly IntPtr nativeClientId;
        readonly string clientId = fuzzy.String();

        public BeginProcessConnect()
        {
            nativeClientId = Marshal.StringToHGlobalUni(clientId);
            nativeClientConnection = Mock.Of<NativeFabricTransport.IFabricTransportClientConnection>(_ => _.get_ClientId() == nativeClientId);
        }

        void IDisposable.Dispose() => Marshal.FreeHGlobal(nativeClientId);

        [Fact]
        public void InvokesConnectAsyncOnHandlerWithCallbackClientAndManagedTimeout()
        {
            FabricTransportCallbackClient actualClient = null;
            TimeSpan actualTimeout = default;
            _ = serviceConnectionHandler
                .Setup(_ => _.ConnectAsync(It.IsAny<FabricTransportCallbackClient>(), It.IsAny<TimeSpan>()))
                .Callback<FabricTransportCallbackClient, TimeSpan>((c, t) => { actualClient = c; actualTimeout = t; })
                .Returns(Task.FromResult<object>(null));

            _ = sut.BeginProcessConnect(nativeClientConnection, timeoutMilliseconds, callback);

            Assert.Equal(clientId, actualClient.GetClientId());
            Assert.Equal(TimeSpan.FromMilliseconds(timeoutMilliseconds), actualTimeout);
            serviceConnectionHandler.Verify(
                _ => _.ConnectAsync(It.IsAny<FabricTransportCallbackClient>(), It.IsAny<TimeSpan>()),
                Times.Once);
        }

        [Fact]
        public async Task InvokesCallbackWithReturnedContextWhenTaskCompletes()
        {
            TaskCompletionSource<object> tcs = new();
            _ = serviceConnectionHandler
                .Setup(_ => _.ConnectAsync(It.IsAny<FabricTransportCallbackClient>(), It.IsAny<TimeSpan>()))
                .Returns(tcs.Task);
            TaskCompletionSource<IFabricAsyncOperationContext> callbackInvoked = new();
            _ = Mock.Get(callback)
                .Setup(_ => _.Invoke(It.IsAny<IFabricAsyncOperationContext>()))
                .Callback<IFabricAsyncOperationContext>(c => callbackInvoked.TrySetResult(c));

            IFabricAsyncOperationContext returnedContext = sut.BeginProcessConnect(nativeClientConnection, timeoutMilliseconds, callback);
            Assert.False(callbackInvoked.Task.IsCompleted);

            tcs.SetResult(null);

            Task completed = await Task.WhenAny(callbackInvoked.Task, Task.Delay(callbackWait, TestContext.Current.CancellationToken));
            Assert.Same(callbackInvoked.Task, completed);
            Assert.Same(returnedContext, await callbackInvoked.Task);
            Mock.Get(callback).Verify(_ => _.Invoke(It.IsAny<IFabricAsyncOperationContext>()), Times.Once);
        }
    }

    [WindowsOnly("Can't load libFabricCommon.so on Linux.")]
    public sealed class BeginProcessDisconnect: FabricTransportConnectionHandlerBrokerTest, IDisposable
    {
        // Method parameters
        readonly IntPtr nativeClientId;
        readonly uint timeoutMilliseconds = fuzzy.UInt32();
        readonly IFabricAsyncOperationCallback callback = Mock.Of<IFabricAsyncOperationCallback>();

        readonly string clientId = fuzzy.String();

        public BeginProcessDisconnect() =>
            nativeClientId = Marshal.StringToHGlobalUni(clientId);

        void IDisposable.Dispose() => Marshal.FreeHGlobal(nativeClientId);

        [Fact]
        public void InvokesDisconnectAsyncOnHandlerWithManagedClientIdAndTimeout()
        {
            string actualClientId = null;
            TimeSpan actualTimeout = default;
            _ = serviceConnectionHandler
                .Setup(_ => _.DisconnectAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Callback<string, TimeSpan>((id, t) => { actualClientId = id; actualTimeout = t; })
                .Returns(Task.FromResult<object>(null));

            _ = sut.BeginProcessDisconnect(nativeClientId, timeoutMilliseconds, callback);

            Assert.Equal(clientId, actualClientId);
            Assert.Equal(TimeSpan.FromMilliseconds(timeoutMilliseconds), actualTimeout);
            serviceConnectionHandler.Verify(
                _ => _.DisconnectAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()),
                Times.Once);
        }

        [Fact]
        public async Task InvokesCallbackWithReturnedContextWhenTaskCompletes()
        {
            TaskCompletionSource<object> tcs = new();
            _ = serviceConnectionHandler
                .Setup(_ => _.DisconnectAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Returns(tcs.Task);
            TaskCompletionSource<IFabricAsyncOperationContext> callbackInvoked = new();
            _ = Mock.Get(callback)
                .Setup(_ => _.Invoke(It.IsAny<IFabricAsyncOperationContext>()))
                .Callback<IFabricAsyncOperationContext>(c => callbackInvoked.TrySetResult(c));

            IFabricAsyncOperationContext returnedContext = sut.BeginProcessDisconnect(nativeClientId, timeoutMilliseconds, callback);
            Assert.False(callbackInvoked.Task.IsCompleted);

            tcs.SetResult(null);

            Task completed = await Task.WhenAny(callbackInvoked.Task, Task.Delay(callbackWait, TestContext.Current.CancellationToken));
            Assert.Same(callbackInvoked.Task, completed);
            Assert.Same(returnedContext, await callbackInvoked.Task);
            Mock.Get(callback).Verify(_ => _.Invoke(It.IsAny<IFabricAsyncOperationContext>()), Times.Once);
        }
    }

    [WindowsOnly("Can't load libFabricCommon.so on Linux.")]
    public sealed class EndProcessConnect: FabricTransportConnectionHandlerBrokerTest, IDisposable
    {
        // BeginProcessConnect parameters
        readonly NativeFabricTransport.IFabricTransportClientConnection nativeClientConnection;
        readonly uint timeoutMilliseconds = fuzzy.UInt32();
        readonly IFabricAsyncOperationCallback callback = Mock.Of<IFabricAsyncOperationCallback>();

        readonly IntPtr nativeClientId;

        public EndProcessConnect()
        {
            nativeClientId = Marshal.StringToHGlobalUni(fuzzy.String());
            nativeClientConnection = Mock.Of<NativeFabricTransport.IFabricTransportClientConnection>(_ => _.get_ClientId() == nativeClientId);
        }

        void IDisposable.Dispose() => Marshal.FreeHGlobal(nativeClientId);

        [Fact]
        public void ReturnsWhenWrappedTaskSucceeds()
        {
            _ = serviceConnectionHandler
                .Setup(_ => _.ConnectAsync(It.IsAny<FabricTransportCallbackClient>(), It.IsAny<TimeSpan>()))
                .Returns(Task.FromResult<object>(null));
            IFabricAsyncOperationContext context = sut.BeginProcessConnect(nativeClientConnection, timeoutMilliseconds, callback);

            sut.EndProcessConnect(context);
        }

        [Fact]
        public void PropagatesExceptionFromFaultedWrappedTask()
        {
            TestException expected = new(fuzzy.String());
            _ = serviceConnectionHandler
                .Setup(_ => _.ConnectAsync(It.IsAny<FabricTransportCallbackClient>(), It.IsAny<TimeSpan>()))
                .Returns(Task.FromException(expected));
            IFabricAsyncOperationContext context = sut.BeginProcessConnect(nativeClientConnection, timeoutMilliseconds, callback);

            var actual = Assert.Throws<TestException>(() => sut.EndProcessConnect(context));
            Assert.Same(expected, actual);
        }
    }

    [WindowsOnly("Can't load libFabricCommon.so on Linux.")]
    public sealed class EndProcessDisconnect: FabricTransportConnectionHandlerBrokerTest, IDisposable
    {
        // BeginProcessDisconnect parameters
        readonly IntPtr nativeClientId;
        readonly uint timeoutMilliseconds = fuzzy.UInt32();
        readonly IFabricAsyncOperationCallback callback = Mock.Of<IFabricAsyncOperationCallback>();

        public EndProcessDisconnect() =>
            nativeClientId = Marshal.StringToHGlobalUni(fuzzy.String());

        void IDisposable.Dispose() => Marshal.FreeHGlobal(nativeClientId);

        [Fact]
        public void ReturnsWhenWrappedTaskSucceeds()
        {
            _ = serviceConnectionHandler
                .Setup(_ => _.DisconnectAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Returns(Task.FromResult<object>(null));
            IFabricAsyncOperationContext context = sut.BeginProcessDisconnect(nativeClientId, timeoutMilliseconds, callback);

            sut.EndProcessDisconnect(context);
        }

        [Fact]
        public void PropagatesExceptionFromFaultedWrappedTask()
        {
            TestException expected = new(fuzzy.String());
            _ = serviceConnectionHandler
                .Setup(_ => _.DisconnectAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Returns(Task.FromException(expected));
            IFabricAsyncOperationContext context = sut.BeginProcessDisconnect(nativeClientId, timeoutMilliseconds, callback);

            var actual = Assert.Throws<TestException>(() => sut.EndProcessDisconnect(context));
            Assert.Same(expected, actual);
        }
    }

    sealed class TestException(string message): Exception(message);
}
