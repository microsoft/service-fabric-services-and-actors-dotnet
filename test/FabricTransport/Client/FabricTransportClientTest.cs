// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;
using static Microsoft.ServiceFabric.FabricTransport.NativeFabricTransport;
using static System.Fabric.Interop.NativeCommon;

namespace Microsoft.ServiceFabric.FabricTransport.Client;

public abstract class FabricTransportClientTest
{
    readonly FabricTransportClient sut = Type<FabricTransportClient>.New();

    // Constructor parameters
    readonly FabricTransportSettings transportSettings = new();
    readonly string connectionAddress = fuzzy.String();
    readonly IFabricTransportClientEventHandler eventHandler = Mock.Of<IFabricTransportClientEventHandler>();
    readonly IFabricTransportCallbackMessageHandler contract = Mock.Of<IFabricTransportCallbackMessageHandler>();
    readonly IFabricTransportMessageDisposer messageMessageDisposer = Mock.Of<IFabricTransportMessageDisposer>(); // Matches SUT parameter name; SUT typo preserved intentionally.

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class Constructor: FabricTransportClientTest
    {
        [Fact(Explicit = true)] // TODO: SUT testability limitation. Constructor P/Invokes into native runtime via CreateNativeClient.
        public void StoresParameterValues() =>
            // The public constructor assigns its connectionAddress parameter to the
            // ConnectionAddress property and its transportSettings parameter to the settings
            // field, but it also calls CreateNativeClient through Utility.WrapNativeSyncInvokeInMTA,
            // which P/Invokes into the native Service Fabric runtime unavailable in the test
            // process. No injection seam bypasses the native call, so the assignments cannot
            // be observed in isolation.
            throw new NotImplementedException();

        [Fact(Explicit = true)] // TODO: SUT testability limitation. Constructor P/Invokes into native runtime via CreateNativeClient.
        public void CreatesNativeClient() =>
            // The public constructor calls CreateNativeClient through Utility.WrapNativeSyncInvokeInMTA,
            // which P/Invokes into the native Service Fabric runtime. The runtime is unavailable
            // in the test process and no injection seam exposes the call for substitution.
            throw new NotImplementedException();

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate transportSettings.
        public void ThrowsArgumentNullExceptionWhenTransportSettingsIsNull()
        {
            // The public constructor accepts FabricTransportSettings without a null check, then
            // dereferences it inside CreateNativeClient via transportSettings.ToNativeV2(pin) and
            // later through settings.ConnectTimeout. Today the constructor throws NullReferenceException
            // from inside Utility.WrapNativeSyncInvokeInMTA instead of ArgumentNullException.
            var exception = Assert.Throws<ArgumentNullException>(() => new FabricTransportClient(
                transportSettings: null, connectionAddress, eventHandler, contract, messageMessageDisposer));
            Assert.Equal(nameof(transportSettings), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate connectionAddress.
        public void ThrowsArgumentNullExceptionWhenConnectionAddressIsNull()
        {
            // The public constructor accepts connectionAddress without a null check, then stores it
            // in the ConnectionAddress property which IsSecurityMismatch dereferences via
            // ConnectionAddress.Contains(...). Today the constructor stores null without validation
            // instead of throwing ArgumentNullException.
            var exception = Assert.Throws<ArgumentNullException>(() => new FabricTransportClient(
                transportSettings, connectionAddress: null, eventHandler, contract, messageMessageDisposer));
            Assert.Equal(nameof(connectionAddress), exception.ParamName);
        }
    }

    public sealed class Abort: FabricTransportClientTest
    {
        [Fact]
        public void InvokesAbortOnNativeClient()
        {
            var nativeClient = Mock.Of<IFabricTransportClient2>();
            sut.Field<IFabricTransportClient2>().Set(nativeClient);

            sut.Abort();

            Mock.Get(nativeClient).Verify(_ => _.Abort(), Times.Once);
        }
    }

    [WindowsOnly("Can't load libFabricCommon.so on Linux.")]
    public sealed class CloseAsync: FabricTransportClientTest
    {
        // Method parameters
        readonly CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        readonly Mock<IFabricTransportClient2> nativeClient = new();
        readonly uint connectTimeoutMs = fuzzy.UInt32();

        public CloseAsync()
        {
            sut.Field<IFabricTransportClient2>().Set(nativeClient.Object);
            sut.Field<FabricTransportSettings>().Set(new FabricTransportSettings { ConnectTimeout = TimeSpan.FromMilliseconds(connectTimeoutMs) });
        }

        [Fact]
        public async Task InvokesBeginCloseAndEndCloseOnNativeClient()
        {
            IFabricAsyncOperationCallback capturedCallback = null;
            var context = Mock.Of<IFabricAsyncOperationContext>();
            _ = nativeClient
                .Setup(_ => _.BeginClose(connectTimeoutMs, It.IsAny<IFabricAsyncOperationCallback>()))
                .Callback<uint, IFabricAsyncOperationCallback>((_, cb) => capturedCallback = cb)
                .Returns(context);

            Task task = sut.CloseAsync(cancellationToken);
            capturedCallback.Invoke(context);
            await task;

            nativeClient.Verify(
                _ => _.BeginClose(It.IsAny<uint>(), It.IsAny<IFabricAsyncOperationCallback>()),
                Times.Once);
            nativeClient.Verify(_ => _.EndClose(context), Times.Once);
            nativeClient.Verify(_ => _.EndClose(It.IsAny<IFabricAsyncOperationContext>()), Times.Once);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. CloseAsync ignores cancellationToken.
        public void UsesCancellationToken()
        {
            var context = Mock.Of<IFabricAsyncOperationContext>();
            _ = nativeClient
                .Setup(_ => _.BeginClose(connectTimeoutMs, It.IsAny<IFabricAsyncOperationCallback>()))
                .Returns(context);
            using CancellationTokenSource cts = new();

            _ = sut.CloseAsync(cts.Token);
            cts.Cancel();

            Mock.Get(context).Verify(_ => _.Cancel(), Times.Once);
        }

        [Fact(Explicit = true)] // TODO: SUT testability limitation. Cannot intercept Utility.WrapNativeAsyncInvokeInMTA to observe operationName.
        public void PassesCloseAsyncAsOperationName() =>
            // The SUT bug: CloseAsync hardcodes "OpenAsync" as the operationName argument to
            // Utility.WrapNativeAsyncInvokeInMTA, so diagnostic output for a close failure would
            // misidentify the operation. The testability limitation: the static
            // Utility.WrapNativeAsyncInvokeInMTA helper is not mockable from the test process and
            // no injection seam exposes the argument, so the operationName cannot be observed.
            throw new NotImplementedException();

        // IsSecurityMismatch's full branch matrix is exercised once through OpenAsync. These two
        // tests prove only that CloseAsync routes through the helper: true wraps, false rethrows.
        [Fact]
        public async Task WrapsFabricCannotConnectExceptionAsConnectionDeniedWhenSecurityMismatch()
        {
            sut.Property<string>().Set(fuzzy.String() + Helper.Secure);
            _ = nativeClient
                .Setup(_ => _.BeginClose(connectTimeoutMs, It.IsAny<IFabricAsyncOperationCallback>()))
                .Throws(new FabricCannotConnectException(fuzzy.String()));

            _ = await Assert.ThrowsAsync<FabricConnectionDeniedException>(() => sut.CloseAsync(cancellationToken));
        }

        [Fact]
        public async Task RethrowsFabricCannotConnectExceptionWhenNotSecurityMismatch()
        {
            sut.Property<string>().Set(fuzzy.Int64().ToString()); // Digits-only so it never contains the "Secure" marker checked by the SUT.
            _ = nativeClient
                .Setup(_ => _.BeginClose(connectTimeoutMs, It.IsAny<IFabricAsyncOperationCallback>()))
                .Throws(new FabricCannotConnectException(fuzzy.String()));

            _ = await Assert.ThrowsAsync<FabricCannotConnectException>(() => sut.CloseAsync(cancellationToken));
        }

        [Fact]
        public async Task PropagatesOtherExceptions()
        {
            TestException expected = new(fuzzy.String());
            _ = nativeClient
                .Setup(_ => _.BeginClose(connectTimeoutMs, It.IsAny<IFabricAsyncOperationCallback>()))
                .Throws(expected);

            var actual = await Assert.ThrowsAsync<TestException>(() => sut.CloseAsync(cancellationToken));
            Assert.Same(expected, actual);
        }

        [Fact(Explicit = true)] // TODO: SUT testability limitation. Utility wraps EndClose exceptions; catch (TimeoutException) is unreachable.
        public void WrapsTimeoutExceptionWithErrorServiceTooBusy() =>
            // When the operation times out, CloseAsync wraps TimeoutException with the
            // ErrorServiceTooBusy message. Empirically, throwing TimeoutException from a mocked
            // EndClose after invoking the captured callback surfaces as System.Fabric.FabricException
            // wrapping a System.Fabric.Interop.Utility+COMWrapperException wrapping the original
            // TimeoutException, because Utility.WrapNativeAsyncInvokeInMTA translates exceptions
            // through COM HResult mapping. The catch (TimeoutException) branch in the SUT is
            // therefore unreachable without substituting the interop helper, which exposes no
            // injection seam.
            throw new NotImplementedException();
    }

    public sealed class ConnectionAddress: FabricTransportClientTest
    {
        [Fact]
        public void ReturnsAssignedAddress()
        {
            string expected = fuzzy.String();
            sut.Property<string>().Set(expected);
            Assert.Same(expected, sut.ConnectionAddress);
        }
    }

    public sealed class Dispose: FabricTransportClientTest
    {
        [Fact]
        public void DoesNothingWhenNativeClientIsNull() =>
            sut.Dispose();

        [Fact(Explicit = true)] // TODO: SUT testability limitation. Utility.FinalReleaseComObject casts to ComObject and throws on mocks.
        public void ReleasesNativeClientAndSetsItToNull() =>
            // Dispose() invokes nativeClient.FinalReleaseComObject(), which forwards to
            // System.Fabric.Interop.Utility.FinalReleaseComObject. That helper unconditionally
            // casts its argument to System.Runtime.InteropServices.Marshalling.ComObject, so a
            // Mock<IFabricTransportClient2> produces an InvalidCastException before the field
            // can be cleared. The SUT exposes no seam to substitute the release call.
            throw new NotImplementedException();
    }

    [WindowsOnly("Can't load libFabricCommon.so on Linux.")]
    public sealed class OpenAsync: FabricTransportClientTest
    {
        // Method parameters
        readonly CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        readonly Mock<IFabricTransportClient2> nativeClient = new();
        readonly uint connectTimeoutMs = fuzzy.UInt32();

        public OpenAsync()
        {
            sut.Field<IFabricTransportClient2>().Set(nativeClient.Object);
            sut.Field<FabricTransportSettings>().Set(new FabricTransportSettings { ConnectTimeout = TimeSpan.FromMilliseconds(connectTimeoutMs) });
        }

        [Fact]
        public async Task InvokesBeginOpenAndEndOpenOnNativeClient()
        {
            IFabricAsyncOperationCallback capturedCallback = null;
            var context = Mock.Of<IFabricAsyncOperationContext>();
            _ = nativeClient
                .Setup(_ => _.BeginOpen(connectTimeoutMs, It.IsAny<IFabricAsyncOperationCallback>()))
                .Callback<uint, IFabricAsyncOperationCallback>((_, cb) => capturedCallback = cb)
                .Returns(context);

            Task task = sut.OpenAsync(cancellationToken);
            capturedCallback.Invoke(context);
            await task;

            nativeClient.Verify(
                _ => _.BeginOpen(It.IsAny<uint>(), It.IsAny<IFabricAsyncOperationCallback>()),
                Times.Once);
            nativeClient.Verify(_ => _.EndOpen(context), Times.Once);
            nativeClient.Verify(_ => _.EndOpen(It.IsAny<IFabricAsyncOperationContext>()), Times.Once);
        }

        [Fact]
        public void UsesCancellationToken()
        {
            var context = Mock.Of<IFabricAsyncOperationContext>();
            _ = nativeClient
                .Setup(_ => _.BeginOpen(connectTimeoutMs, It.IsAny<IFabricAsyncOperationCallback>()))
                .Returns(context);
            using CancellationTokenSource cts = new();

            _ = sut.OpenAsync(cts.Token);
            cts.Cancel();

            Mock.Get(context).Verify(_ => _.Cancel(), Times.Once);
        }

        [Fact]
        public async Task WrapsFabricCannotConnectExceptionAsConnectionDeniedWhenSecurityMismatch()
        {
            sut.Property<string>().Set(fuzzy.String() + Helper.Secure);
            _ = nativeClient
                .Setup(_ => _.BeginOpen(connectTimeoutMs, It.IsAny<IFabricAsyncOperationCallback>()))
                .Throws(new FabricCannotConnectException(fuzzy.String()));

            _ = await Assert.ThrowsAsync<FabricConnectionDeniedException>(() => sut.OpenAsync(cancellationToken));
        }

        [Fact]
        public async Task RethrowsFabricCannotConnectExceptionWhenNotSecurityMismatch()
        {
            sut.Property<string>().Set(fuzzy.Int64().ToString()); // Digits-only so it never contains the "Secure" marker checked by the SUT.
            _ = nativeClient
                .Setup(_ => _.BeginOpen(connectTimeoutMs, It.IsAny<IFabricAsyncOperationCallback>()))
                .Throws(new FabricCannotConnectException(fuzzy.String()));

            _ = await Assert.ThrowsAsync<FabricCannotConnectException>(() => sut.OpenAsync(cancellationToken));
        }

        [Fact]
        public async Task RethrowsFabricCannotConnectExceptionWhenSecureAddressAndNonNoneCredentials()
        {
            sut.Field<FabricTransportSettings>().Set(new FabricTransportSettings { ConnectTimeout = TimeSpan.FromMilliseconds(connectTimeoutMs), SecurityCredentials = new X509Credentials() });
            sut.Property<string>().Set(fuzzy.String() + Helper.Secure);
            _ = nativeClient
                .Setup(_ => _.BeginOpen(connectTimeoutMs, It.IsAny<IFabricAsyncOperationCallback>()))
                .Throws(new FabricCannotConnectException(fuzzy.String()));

            _ = await Assert.ThrowsAsync<FabricCannotConnectException>(() => sut.OpenAsync(cancellationToken));
        }

        [Fact(Explicit = true)] // TODO: SUT bug. IsSecurityMismatch dereferences SecurityCredentials without a null check.
        public async Task WrapsFabricCannotConnectExceptionAsConnectionDeniedWhenSecureAddressAndNullCredentials()
        {
            // Null SecurityCredentials is treated as no credentials everywhere else in the SUT
            // (FabricTransportSettings defaults SecurityCredentials to NoneSecurityCredentials and
            // FabricTransportSettingsExtension converts null as no native credentials), so a secure
            // address combined with null credentials is a security mismatch and should be wrapped
            // as FabricConnectionDeniedException. IsSecurityMismatch, however, dereferences
            // settings.SecurityCredentials.CredentialType without a null check and throws
            // NullReferenceException, so this test cannot pass against the current SUT.
            sut.Field<FabricTransportSettings>().Set(new FabricTransportSettings { ConnectTimeout = TimeSpan.FromMilliseconds(connectTimeoutMs), SecurityCredentials = null });
            sut.Property<string>().Set(fuzzy.String() + Helper.Secure);
            _ = nativeClient
                .Setup(_ => _.BeginOpen(connectTimeoutMs, It.IsAny<IFabricAsyncOperationCallback>()))
                .Throws(new FabricCannotConnectException(fuzzy.String()));

            _ = await Assert.ThrowsAsync<FabricConnectionDeniedException>(() => sut.OpenAsync(cancellationToken));
        }

        [Fact]
        public async Task PropagatesOtherExceptions()
        {
            TestException expected = new(fuzzy.String());
            _ = nativeClient
                .Setup(_ => _.BeginOpen(connectTimeoutMs, It.IsAny<IFabricAsyncOperationCallback>()))
                .Throws(expected);

            var actual = await Assert.ThrowsAsync<TestException>(() => sut.OpenAsync(cancellationToken));
            Assert.Same(expected, actual);
        }

        [Fact(Explicit = true)] // TODO: SUT testability limitation. Utility wraps EndOpen exceptions; catch (TimeoutException) is unreachable.
        public void WrapsTimeoutExceptionWithErrorServiceTooBusy() =>
            // When the operation times out, OpenAsync wraps TimeoutException with the
            // ErrorServiceTooBusy message. Empirically, throwing TimeoutException from a mocked
            // EndOpen after invoking the captured callback surfaces as System.Fabric.FabricException
            // wrapping a System.Fabric.Interop.Utility+COMWrapperException wrapping the original
            // TimeoutException, because Utility.WrapNativeAsyncInvokeInMTA translates exceptions
            // through COM HResult mapping. The catch (TimeoutException) branch in the SUT is
            // therefore unreachable without substituting the interop helper, which exposes no
            // injection seam.
            throw new NotImplementedException();
    }

    [WindowsOnly("Can't load libFabricCommon.so on Linux.")]
    public sealed class RequestResponseAsync: FabricTransportClientTest
    {
        // Method parameters
        readonly FabricTransportMessage requestMessage = fuzzy.FabricTransportMessage();
        readonly TimeSpan timeout = fuzzy.TimeSpan().Milliseconds();
        readonly Guid requestId = Guid.NewGuid();

        readonly Mock<IFabricTransportClient2> nativeClient = new();

        public RequestResponseAsync()
        {
            sut.Field<IFabricTransportClient2>().Set(nativeClient.Object);
            sut.Field<FabricTransportSettings>().Set(new FabricTransportSettings());
        }

        [Fact]
        public async Task InvokesBeginRequestAndEndRequestWhenRequestIdIsDefault()
        {
            IFabricAsyncOperationCallback capturedCallback = null;
            IFabricTransportMessage capturedNativeMessage = null;
            var context = Mock.Of<IFabricAsyncOperationContext>();
            Mock<IFabricTransportMessage> nativeResponse = new();
            _ = nativeClient
                .Setup(_ => _.BeginRequest(
                    It.IsAny<IFabricTransportMessage>(),
                    (uint)timeout.TotalMilliseconds,
                    It.IsAny<IFabricAsyncOperationCallback>()))
                .Callback<IFabricTransportMessage, uint, IFabricAsyncOperationCallback>(
                    (m, _, cb) => { capturedNativeMessage = m; capturedCallback = cb; })
                .Returns(context);
            _ = nativeClient
                .Setup(_ => _.EndRequest(context))
                .Returns(nativeResponse.Object);

            Task<FabricTransportMessage> task = sut.RequestResponseAsync(requestMessage, timeout);
            capturedCallback.Invoke(context);
            FabricTransportMessage result = await task;

            nativeClient.Verify(
                _ => _.BeginRequest(
                    It.IsAny<IFabricTransportMessage>(),
                    It.IsAny<uint>(),
                    It.IsAny<IFabricAsyncOperationCallback>()),
                Times.Once);
            nativeClient.Verify(
                _ => _.BeginRequestWithId(
                    It.IsAny<Guid>(),
                    It.IsAny<IFabricTransportMessage>(),
                    It.IsAny<uint>(),
                    It.IsAny<IFabricAsyncOperationCallback>()),
                Times.Never);
            nativeClient.Verify(_ => _.EndRequest(It.IsAny<IFabricAsyncOperationContext>()), Times.Once);
            nativeClient.Verify(_ => _.EndRequestWithId(It.IsAny<IFabricAsyncOperationContext>()), Times.Never);
            var wrapper = (NativeFabricTransportMessage)capturedNativeMessage;
            Assert.Same(requestMessage, wrapper.Field<FabricTransportMessage>().Value);
            nativeResponse.Verify(_ => _.GetHeaderAndBodyBuffer(out It.Ref<IntPtr>.IsAny, out It.Ref<uint>.IsAny, out It.Ref<IntPtr>.IsAny), Times.Once);
            Assert.Same(nativeResponse.Object, result.Field<IFabricTransportMessage>().Value);
        }

        [Fact]
        public async Task InvokesBeginRequestWithIdAndEndRequestWithIdWhenRequestIdIsNotDefault()
        {
            IFabricAsyncOperationCallback capturedCallback = null;
            IFabricTransportMessage capturedNativeMessage = null;
            var context = Mock.Of<IFabricAsyncOperationContext>();
            Mock<IFabricTransportMessage> nativeResponse = new();
            _ = nativeClient
                .Setup(_ => _.BeginRequestWithId(
                    requestId,
                    It.IsAny<IFabricTransportMessage>(),
                    (uint)timeout.TotalMilliseconds,
                    It.IsAny<IFabricAsyncOperationCallback>()))
                .Callback<Guid, IFabricTransportMessage, uint, IFabricAsyncOperationCallback>(
                    (_, m, _, cb) => { capturedNativeMessage = m; capturedCallback = cb; })
                .Returns(context);
            _ = nativeClient
                .Setup(_ => _.EndRequestWithId(context))
                .Returns(nativeResponse.Object);

            Task<FabricTransportMessage> task = sut.RequestResponseAsync(requestMessage, timeout, requestId);
            capturedCallback.Invoke(context);
            FabricTransportMessage result = await task;

            nativeClient.Verify(
                _ => _.BeginRequestWithId(
                    It.IsAny<Guid>(),
                    It.IsAny<IFabricTransportMessage>(),
                    It.IsAny<uint>(),
                    It.IsAny<IFabricAsyncOperationCallback>()),
                Times.Once);
            nativeClient.Verify(
                _ => _.BeginRequest(
                    It.IsAny<IFabricTransportMessage>(),
                    It.IsAny<uint>(),
                    It.IsAny<IFabricAsyncOperationCallback>()),
                Times.Never);
            nativeClient.Verify(_ => _.EndRequestWithId(It.IsAny<IFabricAsyncOperationContext>()), Times.Once);
            nativeClient.Verify(_ => _.EndRequest(It.IsAny<IFabricAsyncOperationContext>()), Times.Never);
            var wrapper = (NativeFabricTransportMessage)capturedNativeMessage;
            Assert.Same(requestMessage, wrapper.Field<FabricTransportMessage>().Value);
            nativeResponse.Verify(_ => _.GetHeaderAndBodyBuffer(out It.Ref<IntPtr>.IsAny, out It.Ref<uint>.IsAny, out It.Ref<IntPtr>.IsAny), Times.Once);
            Assert.Same(nativeResponse.Object, result.Field<IFabricTransportMessage>().Value);
        }

        // IsSecurityMismatch's full branch matrix is exercised once through OpenAsync. These two
        // tests prove only that RequestResponseAsync routes through the helper: true wraps, false rethrows.
        [Fact]
        public async Task WrapsFabricCannotConnectExceptionAsConnectionDeniedWhenSecurityMismatch()
        {
            sut.Property<string>().Set(fuzzy.String() + Helper.Secure);
            _ = nativeClient
                .Setup(_ => _.BeginRequest(
                    It.IsAny<IFabricTransportMessage>(),
                    (uint)timeout.TotalMilliseconds,
                    It.IsAny<IFabricAsyncOperationCallback>()))
                .Throws(new FabricCannotConnectException(fuzzy.String()));

            _ = await Assert.ThrowsAsync<FabricConnectionDeniedException>(() => sut.RequestResponseAsync(requestMessage, timeout));
        }

        [Fact]
        public async Task RethrowsFabricCannotConnectExceptionWhenNotSecurityMismatch()
        {
            sut.Property<string>().Set(fuzzy.Int64().ToString()); // Digits-only so it never contains the "Secure" marker checked by the SUT.
            _ = nativeClient
                .Setup(_ => _.BeginRequest(
                    It.IsAny<IFabricTransportMessage>(),
                    (uint)timeout.TotalMilliseconds,
                    It.IsAny<IFabricAsyncOperationCallback>()))
                .Throws(new FabricCannotConnectException(fuzzy.String()));

            _ = await Assert.ThrowsAsync<FabricCannotConnectException>(() => sut.RequestResponseAsync(requestMessage, timeout));
        }

        [Fact]
        public async Task RethrowsOtherExceptions()
        {
            TestException expected = new(fuzzy.String());
            _ = nativeClient
                .Setup(_ => _.BeginRequest(
                    It.IsAny<IFabricTransportMessage>(),
                    (uint)timeout.TotalMilliseconds,
                    It.IsAny<IFabricAsyncOperationCallback>()))
                .Throws(expected);

            var actual = await Assert.ThrowsAsync<TestException>(() => sut.RequestResponseAsync(requestMessage, timeout));
            Assert.Same(expected, actual);
        }

        [Fact(Explicit = true)] // TODO: SUT testability limitation. Utility wraps EndRequest exceptions; catch (TimeoutException) is unreachable.
        public void WrapsTimeoutExceptionWithErrorServiceTooBusy() =>
            // When the operation times out, RequestResponseAsync wraps TimeoutException with the
            // ErrorServiceTooBusy message. Empirically, throwing TimeoutException from a mocked
            // EndRequest (or EndRequestWithId) after invoking the captured callback surfaces as
            // System.Fabric.FabricException wrapping a System.Fabric.Interop.Utility+COMWrapperException
            // wrapping the original TimeoutException, because Utility.WrapNativeAsyncInvokeInMTA
            // translates exceptions through COM HResult mapping. The catch (TimeoutException)
            // branch in the SUT is therefore unreachable without substituting the interop helper,
            // which exposes no injection seam.
            throw new NotImplementedException();
    }

    public sealed class SendOneWay: FabricTransportClientTest
    {
        readonly FabricTransportMessage message = fuzzy.FabricTransportMessage();

        [Fact]
        public void InvokesSendOnNativeClient()
        {
            Mock<IFabricTransportClient2> nativeClient = new();
            sut.Field<IFabricTransportClient2>().Set(nativeClient.Object);
            IFabricTransportMessage sent = null;
            _ = nativeClient
                .Setup(_ => _.Send(It.IsAny<IFabricTransportMessage>()))
                .Callback((IFabricTransportMessage actual) => sent = actual);

            sut.SendOneWay(message);

            nativeClient.Verify(_ => _.Send(It.IsAny<IFabricTransportMessage>()), Times.Once);
            var wrapper = (NativeFabricTransportMessage)sent;
            Assert.Same(message, wrapper.Field<FabricTransportMessage>().Value);
        }
    }

    public sealed class Settings: FabricTransportClientTest
    {
        [Fact]
        public void ReturnsAssignedSettings()
        {
            FabricTransportSettings expected = new();
            sut.Field<FabricTransportSettings>().Set(expected);
            Assert.Same(expected, sut.Settings);
        }
    }

    class TestException: Exception
    {
        internal TestException(string message): base(message) { }
    }
}
