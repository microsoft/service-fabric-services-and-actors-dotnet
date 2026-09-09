// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;
using static System.Fabric.Interop.NativeCommon;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime;

public abstract class FabricTransportMessageHandlerBrokerTest
{
    readonly NativeFabricTransport.IFabricTransportMessageHandler sut;

    // Constructor parameters
    readonly Mock<IFabricTransportMessageHandler> service = new();
    readonly Mock<IFabricTransportConnectionHandler> nativeConnectionHandler = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    FabricTransportMessageHandlerBrokerTest() =>
        sut = new FabricTransportMessageHandlerBroker(service.Object, nativeConnectionHandler.Object);

    public sealed class Constructor: FabricTransportMessageHandlerBrokerTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate service.
        public void ThrowsArgumentNullExceptionWhenServiceIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new FabricTransportMessageHandlerBroker(null, nativeConnectionHandler.Object));
            Assert.Equal(nameof(service), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate nativeConnectionHandler.
        public void ThrowsArgumentNullExceptionWhenNativeConnectionHandlerIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new FabricTransportMessageHandlerBroker(service.Object, null));
            Assert.Equal(nameof(nativeConnectionHandler), exception.ParamName);
        }
    }

    [WindowsOnly("Can't load libFabricCommon.so on Linux.")]
    public sealed class BeginProcessRequest: FabricTransportMessageHandlerBrokerTest, IDisposable
    {
        // Method parameters
        readonly IntPtr clientId;
        readonly Mock<NativeFabricTransport.IFabricTransportMessage> message = new();
        readonly uint timeoutMilliseconds = fuzzy.UInt32();
        readonly IFabricAsyncOperationCallback callback = Mock.Of<IFabricAsyncOperationCallback>();

        readonly string managedClientId = fuzzy.String();

        public BeginProcessRequest() =>
            clientId = Marshal.StringToHGlobalUni(managedClientId);

        void IDisposable.Dispose() => Marshal.FreeHGlobal(clientId);

        [Fact]
        public void InvokesRequestResponseAsyncOnServiceWithManagedContextAndMessage()
        {
            FabricTransportRequestContext actualContext = null;
            FabricTransportMessage actualMessage = null;
            _ = service
                .Setup(_ => _.RequestResponseAsync(It.IsAny<FabricTransportRequestContext>(), It.IsAny<FabricTransportMessage>()))
                .Callback<FabricTransportRequestContext, FabricTransportMessage>((c, m) => { actualContext = c; actualMessage = m; })
                .Returns(new TaskCompletionSource<FabricTransportMessage>().Task);
            FabricTransportCallbackClient expectedCallbackClient = new(Mock.Of<NativeFabricTransport.IFabricTransportClientConnection>());
            _ = nativeConnectionHandler.Setup(_ => _.GetCallBack(managedClientId)).Returns(expectedCallbackClient);

            IFabricAsyncOperationContext returnedContext = sut.BeginProcessRequest(clientId, message.Object, timeoutMilliseconds, callback);

            Assert.NotNull(returnedContext);
            Assert.Equal(managedClientId, actualContext.ClientId);
            Assert.Same(expectedCallbackClient, actualContext.GetCallbackClient());
            Assert.Same(message.Object, actualMessage.Field<NativeFabricTransport.IFabricTransportMessage>().Value);
            message.Verify(
                _ => _.GetHeaderAndBodyBuffer(out It.Ref<IntPtr>.IsAny, out It.Ref<uint>.IsAny, out It.Ref<IntPtr>.IsAny),
                Times.Once);
            service.Verify(
                _ => _.RequestResponseAsync(It.IsAny<FabricTransportRequestContext>(), It.IsAny<FabricTransportMessage>()),
                Times.Once);
        }
    }

    [WindowsOnly("Can't load libFabricCommon.so on Linux.")]
    public sealed class EndProcessRequest: FabricTransportMessageHandlerBrokerTest, IDisposable
    {
        // BeginProcessRequest parameters
        readonly IntPtr clientId;
        readonly NativeFabricTransport.IFabricTransportMessage message = Mock.Of<NativeFabricTransport.IFabricTransportMessage>();
        readonly uint timeoutMilliseconds = fuzzy.UInt32();
        readonly IFabricAsyncOperationCallback callback = Mock.Of<IFabricAsyncOperationCallback>();

        readonly FabricTransportMessage reply = new(null, null);

        public EndProcessRequest()
        {
            clientId = Marshal.StringToHGlobalUni(fuzzy.String());
            _ = service
                .Setup(_ => _.RequestResponseAsync(It.IsAny<FabricTransportRequestContext>(), It.IsAny<FabricTransportMessage>()))
                .Returns(Task.FromResult(reply));
        }

        void IDisposable.Dispose() => Marshal.FreeHGlobal(clientId);

        [Fact]
        public void ReturnsNativeMessageWrappingReplyWhenRequestResponseAsyncCompletes()
        {
            IFabricAsyncOperationContext context = sut.BeginProcessRequest(clientId, message, timeoutMilliseconds, callback);

            NativeFabricTransport.IFabricTransportMessage result = sut.EndProcessRequest(context);

            try
            {
                Assert.Same(reply, result.Field<FabricTransportMessage>().Value);
            }
            finally
            {
                result.Dispose();
            }
        }
    }

    public sealed class HandleOneWay: FabricTransportMessageHandlerBrokerTest, IDisposable
    {
        // Method parameters
        readonly IntPtr clientId;
        readonly Mock<NativeFabricTransport.IFabricTransportMessage> message = new();

        readonly string managedClientId = fuzzy.String();

        public HandleOneWay() =>
            clientId = Marshal.StringToHGlobalUni(managedClientId);

        void IDisposable.Dispose() => Marshal.FreeHGlobal(clientId);

        [Fact]
        public void InvokesHandleOneWayOnServiceWithManagedContextAndMessage()
        {
            FabricTransportRequestContext actualContext = null;
            FabricTransportMessage actualMessage = null;
            _ = service
                .Setup(_ => _.HandleOneWay(It.IsAny<FabricTransportRequestContext>(), It.IsAny<FabricTransportMessage>()))
                .Callback<FabricTransportRequestContext, FabricTransportMessage>((c, m) => { actualContext = c; actualMessage = m; });
            FabricTransportCallbackClient expectedCallbackClient = new(Mock.Of<NativeFabricTransport.IFabricTransportClientConnection>());
            _ = nativeConnectionHandler.Setup(_ => _.GetCallBack(managedClientId)).Returns(expectedCallbackClient);

            sut.HandleOneWay(clientId, message.Object);

            Assert.Equal(managedClientId, actualContext.ClientId);
            Assert.Same(expectedCallbackClient, actualContext.GetCallbackClient());
            Assert.Same(message.Object, actualMessage.Field<NativeFabricTransport.IFabricTransportMessage>().Value);
            message.Verify(
                _ => _.GetHeaderAndBodyBuffer(out It.Ref<IntPtr>.IsAny, out It.Ref<uint>.IsAny, out It.Ref<IntPtr>.IsAny),
                Times.Once);
            service.Verify(
                _ => _.HandleOneWay(It.IsAny<FabricTransportRequestContext>(), It.IsAny<FabricTransportMessage>()),
                Times.Once);
        }
    }
}
