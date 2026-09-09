// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Fabric;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;
using static System.Fabric.Interop.NativeCommon;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime;

public abstract class FabricTransportListenerTest
{
    readonly IFabricTransportListener sut = Type<FabricTransportListener>.Uninitialized();

    // Constructor parameters
    readonly FabricTransportSettings transportSettings = new();
    readonly FabricTransportListenerAddress listenerAddress = new(fuzzy.String(), fuzzy.Int32(), fuzzy.String());
    readonly IFabricTransportMessageHandler serviceImplementation = Mock.Of<IFabricTransportMessageHandler>();
    readonly IFabricTransportConnectionHandler remotingConnectionHandler = Mock.Of<IFabricTransportConnectionHandler>();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class Constructor: FabricTransportListenerTest
    {
        [Fact(Explicit = true)] // TODO: SUT testability limitation. Constructor P/Invokes into native runtime via CreateNativeListener.
        public void CreatesNativeListener() =>
            // The public constructor calls CreateNativeListener through Utility.WrapNativeSyncInvokeInMTA,
            // which P/Invokes into the native Service Fabric runtime. The runtime is unavailable
            // in the test process and no injection seam exposes the call for substitution.
            throw new NotImplementedException();

        [Fact]
        public void AppendsSecureSuffixToListenerAddressPathWhenCredentialsAreNotNone()
        {
            transportSettings.SecurityCredentials = new X509Credentials();
            string expectedPath = listenerAddress.Path + "-" + Helper.Secure;

            ConstructListenerSwallowingNativeFailure();

            Assert.Equal(expectedPath, listenerAddress.Path);
        }

        [Fact]
        public void LeavesListenerAddressPathUnchangedWhenCredentialsAreNone()
        {
            transportSettings.SecurityCredentials = new NoneSecurityCredentials();
            string expectedPath = listenerAddress.Path;

            ConstructListenerSwallowingNativeFailure();

            Assert.Equal(expectedPath, listenerAddress.Path);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate transportSettings.
        public void ThrowsArgumentNullExceptionWhenTransportSettingsIsNull()
        {
            // The public constructor accepts FabricTransportSettings without a null check, then
            // dereferences it via transportSettings.SecurityCredentials.CredentialType when
            // computing isNotSecureEndpoint, and later inside CreateNativeListener via
            // transportSettings.ToNativeV2(pin). Today the constructor throws NullReferenceException
            // before reaching CreateNativeListener instead of ArgumentNullException.
            var exception = Assert.Throws<ArgumentNullException>(() => new FabricTransportListener(
                transportSettings: null, listenerAddress, serviceImplementation, remotingConnectionHandler));
            Assert.Equal(nameof(transportSettings), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate listenerAddress.
        public void ThrowsArgumentNullExceptionWhenListenerAddressIsNull()
        {
            // The public constructor accepts FabricTransportListenerAddress without a null check,
            // then dereferences it via listenerAddress.Path. Today the constructor throws
            // NullReferenceException instead of ArgumentNullException.
            var exception = Assert.Throws<ArgumentNullException>(() => new FabricTransportListener(
                transportSettings, listenerAddress: null, serviceImplementation, remotingConnectionHandler));
            Assert.Equal(nameof(listenerAddress), exception.ParamName);
        }

        void ConstructListenerSwallowingNativeFailure()
        {
            // The constructor mutates listenerAddress.Path before invoking the native runtime via
            // CreateNativeListener. That native call's failure mode varies by host (FabricException on
            // Windows with the runtime installed, DllNotFoundException/TypeInitializationException on
            // hosts without it), unrelated to the Path behavior being verified.
            FabricTransportListener listener = null;
            try
            {
                listener = new FabricTransportListener(
                    transportSettings, listenerAddress, serviceImplementation, remotingConnectionHandler);
            }
            catch
            {
            }
            finally
            {
                listener?.Dispose();
            }
        }
    }

    public sealed class Abort: FabricTransportListenerTest
    {
        [Fact]
        public void InvokesAbortOnNativeListener()
        {
            var nativeListener = Mock.Of<NativeFabricTransport.IFabricTransportListener>();
            sut.Field<NativeFabricTransport.IFabricTransportListener>().Set(nativeListener);

            sut.Abort();

            Mock.Get(nativeListener).Verify(_ => _.Abort(), Times.Once);
        }

        [Fact]
        public void DoesNothingWhenNativeListenerIsNull() =>
            sut.Abort();
    }

    [WindowsOnly("Can't load libFabricCommon.so on Linux.")]
    public sealed class CloseAsync: FabricTransportListenerTest
    {
        // Method parameters
        readonly CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        readonly Mock<NativeFabricTransport.IFabricTransportListener> nativeListener = new();

        public CloseAsync() => sut.Field<NativeFabricTransport.IFabricTransportListener>().Set(nativeListener.Object);

        [Fact]
        public async Task InvokesBeginCloseAndEndCloseOnNativeListener()
        {
            IFabricAsyncOperationCallback capturedCallback = null;
            var context = Mock.Of<IFabricAsyncOperationContext>();
            _ = nativeListener
                .Setup(_ => _.BeginClose(It.IsAny<IFabricAsyncOperationCallback>()))
                .Callback<IFabricAsyncOperationCallback>(cb => capturedCallback = cb)
                .Returns(context);

            Task task = sut.CloseAsync(cancellationToken);
            capturedCallback.Invoke(context);
            await task;

            nativeListener.Verify(_ => _.BeginClose(It.IsAny<IFabricAsyncOperationCallback>()), Times.Once);
            nativeListener.Verify(_ => _.EndClose(context), Times.Once);
            nativeListener.Verify(_ => _.EndClose(It.IsAny<IFabricAsyncOperationContext>()), Times.Once);
        }
    }

    public sealed class Dispose: FabricTransportListenerTest
    {
        [Fact(Explicit = true)] // TODO: SUT testability limitation. Utility.FinalReleaseComObject casts to ComObject and throws on mocks.
        public void ReleasesNativeListenerAndSetsItToNull() =>
            // Dispose() invokes nativeListner.FinalReleaseComObject(), which forwards to
            // System.Fabric.Interop.Utility.FinalReleaseComObject. That helper unconditionally
            // casts its argument to System.Runtime.InteropServices.Marshalling.ComObject, so a
            // Mock<NativeFabricTransport.IFabricTransportListener> produces an InvalidCastException
            // before the field can be cleared. The SUT exposes no seam to substitute the release call.
            throw new NotImplementedException();

        [Fact]
        public void DoesNothingWhenNativeListenerIsNull() =>
            sut.Dispose();
    }

    [WindowsOnly("Can't load libFabricCommon.so on Linux.")]
    public sealed class OpenAsync: FabricTransportListenerTest
    {
        // Method parameters
        readonly CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        readonly Mock<NativeFabricTransport.IFabricTransportListener> nativeListener = new();

        public OpenAsync() => sut.Field<NativeFabricTransport.IFabricTransportListener>().Set(nativeListener.Object);

        [Fact]
        public async Task InvokesBeginOpenAndEndOpenOnNativeListener()
        {
            IFabricAsyncOperationCallback capturedCallback = null;
            var context = Mock.Of<IFabricAsyncOperationContext>();
            _ = nativeListener
                .Setup(_ => _.BeginOpen(It.IsAny<IFabricAsyncOperationCallback>()))
                .Callback<IFabricAsyncOperationCallback>(cb => capturedCallback = cb)
                .Returns(context);
            _ = nativeListener.Setup(_ => _.EndOpen(context)).Returns(Mock.Of<IFabricStringResult>());

            Task<string> task = sut.OpenAsync(cancellationToken);
            capturedCallback.Invoke(context);
            _ = await task;

            nativeListener.Verify(_ => _.BeginOpen(It.IsAny<IFabricAsyncOperationCallback>()), Times.Once);
            nativeListener.Verify(_ => _.EndOpen(context), Times.Once);
            nativeListener.Verify(_ => _.EndOpen(It.IsAny<IFabricAsyncOperationContext>()), Times.Once);
        }

        [Fact]
        public async Task ReturnsListenerAddress()
        {
            string expected = fuzzy.String();
            IntPtr nativeAddress = Marshal.StringToHGlobalUni(expected);
            try
            {
                IFabricAsyncOperationCallback capturedCallback = null;
                var context = Mock.Of<IFabricAsyncOperationContext>();
                var address = Mock.Of<IFabricStringResult>();
                _ = Mock.Get(address).Setup(_ => _.get_String()).Returns(nativeAddress);
                _ = nativeListener
                    .Setup(_ => _.BeginOpen(It.IsAny<IFabricAsyncOperationCallback>()))
                    .Callback<IFabricAsyncOperationCallback>(cb => capturedCallback = cb)
                    .Returns(context);
                _ = nativeListener.Setup(_ => _.EndOpen(context)).Returns(address);

                Task<string> task = sut.OpenAsync(cancellationToken);
                capturedCallback.Invoke(context);
                string actual = await task;

                Assert.Equal(expected, actual);
            }
            finally
            {
                Marshal.FreeHGlobal(nativeAddress);
            }
        }
    }
}
