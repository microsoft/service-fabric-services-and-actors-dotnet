// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Runtime.InteropServices;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime;

public abstract class FabricTransportCallbackClientTest: IDisposable
{
    readonly FabricTransportCallbackClient sut;

    // Constructor parameters
    readonly Mock<NativeFabricTransport.IFabricTransportClientConnection> nativeClientConnection = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    readonly string clientId = fuzzy.String();
    readonly IntPtr clientIdPtr;

    FabricTransportCallbackClientTest()
    {
        clientIdPtr = Marshal.StringToHGlobalUni(clientId);
        _ = nativeClientConnection.Setup(_ => _.get_ClientId()).Returns(clientIdPtr);
        sut = new FabricTransportCallbackClient(nativeClientConnection.Object);
    }

    void IDisposable.Dispose() => Marshal.FreeHGlobal(clientIdPtr);

    public sealed class Constructor: FabricTransportCallbackClientTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate nativeClientConnection.
        public void ThrowsArgumentNullExceptionWhenNativeClientConnectionIsNull()
        {
            // The constructor dereferences nativeClientConnection to call get_ClientId() without a null check,
            // so a null argument produces NullReferenceException instead of ArgumentNullException.
            var exception = Assert.Throws<ArgumentNullException>(() => new FabricTransportCallbackClient(null));
            Assert.Equal(nameof(nativeClientConnection), exception.ParamName);
        }
    }

    public sealed class Dispose: FabricTransportCallbackClientTest
    {
        [Fact(Explicit = true)] // TODO: SUT testability limitation. Utility.FinalReleaseComObject cannot be mocked.
        public void ReleasesNativeClientConnection() =>
            // Dispose() invokes nativeClientConnection.FinalReleaseComObject(), which forwards to
            // System.Fabric.Interop.Utility.FinalReleaseComObject. That helper unconditionally casts its argument
            // to System.Runtime.InteropServices.Marshalling.ComObject, so a Mock<IFabricTransportClientConnection>
            // produces an InvalidCastException. The SUT exposes no seam to substitute the release call.
            throw new NotImplementedException();

        [Fact]
        public void DoesNothingWhenAlreadyDisposed()
        {
            // If Dispose forwarded to nativeClientConnection.FinalReleaseComObject(), the mock would produce an
            // InvalidCastException (see ReleasesNativeClientConnection above). Reaching the end of this test proves
            // the release was suppressed by the disposed flag.
            sut.Field<bool>().Set(true);

            sut.Dispose();
        }
    }

    public sealed class GetClientId: FabricTransportCallbackClientTest
    {
        [Fact]
        public void ReturnsClientIdReadFromNativeClientConnection()
        {
            Assert.Equal(clientId, sut.GetClientId());
            nativeClientConnection.Verify(_ => _.get_ClientId(), Times.Once);
        }
    }

    public sealed class OneWayMessage: FabricTransportCallbackClientTest
    {
        readonly FabricTransportMessage requestBody = fuzzy.FabricTransportMessage();

        [Fact]
        public void WrapsRequestBodyInNativeMessageAndSendsItOnNativeClientConnection()
        {
            NativeFabricTransport.IFabricTransportMessage actualMessage = null;
            _ = nativeClientConnection
                .Setup(_ => _.Send(It.IsAny<NativeFabricTransport.IFabricTransportMessage>()))
                .Callback<NativeFabricTransport.IFabricTransportMessage>(m => actualMessage = m);

            sut.OneWayMessage(requestBody);

            Assert.NotNull(actualMessage);
            Assert.Same(requestBody, actualMessage.Field<FabricTransportMessage>().Value);
            nativeClientConnection.Verify(
                _ => _.Send(It.IsAny<NativeFabricTransport.IFabricTransportMessage>()), Times.Once);
        }
    }
}
