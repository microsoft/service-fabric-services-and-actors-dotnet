// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Fabric;
using System.Fabric.Interop;
using System.Runtime.InteropServices;
using Fuzzy;
using Xunit;
using static Microsoft.ServiceFabric.FabricTransport.NativeFabricTransport;

namespace Microsoft.ServiceFabric.FabricTransport;

[WindowsOnly("Can't load libFabricCommon.so on Linux.")]
public abstract class FabricTransportSettingsExtensionTest
{
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class ToNativeV2: FabricTransportSettingsExtensionTest, IDisposable
    {
        readonly FabricTransportSettings transportSettings = new()
        {
            // Suppress credentials marshalling; tests that exercise it reassign explicitly.
            SecurityCredentials = null,
            OperationTimeout = fuzzy.TimeSpan().Seconds(),
            KeepAliveTimeout = fuzzy.TimeSpan().Seconds(),
            ConnectTimeout = fuzzy.TimeSpan().Milliseconds(),
            MaxMessageSize = fuzzy.Int32().Minimum(0),
            MaxConcurrentCalls = fuzzy.Int32().Minimum(0),
            MaxQueueSize = fuzzy.Int32().Minimum(0),
        };

        readonly PinCollection pin = [];

        void IDisposable.Dispose() => pin.Dispose();

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for transportSettings.
        public void ThrowsArgumentNullExceptionWhenTransportSettingsIsNull()
        {
            // ToNativeV2 dereferences transportSettings without validating it, producing NullReferenceException
            // instead of ArgumentNullException with ParamName "transportSettings".
            var ex = Assert.Throws<ArgumentNullException>(() => ((FabricTransportSettings)null).ToNativeV2(pin));
            Assert.Equal(nameof(transportSettings), ex.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for pin.
        public void ThrowsArgumentNullExceptionWhenPinIsNull()
        {
            // ToNativeV2 dereferences pin without validating it, producing NullReferenceException
            // instead of ArgumentNullException with ParamName "pin".
            var ex = Assert.Throws<ArgumentNullException>(() => transportSettings.ToNativeV2(null));
            Assert.Equal(nameof(pin), ex.ParamName);
        }

        [Fact]
        public void SetsSecurityCredentialsToZeroWhenNull()
        {
            transportSettings.SecurityCredentials = null;

            IntPtr ptr = transportSettings.ToNativeV2(pin);

            FABRIC_TRANSPORT_SETTINGS native = Native(ptr);
            Assert.Equal(IntPtr.Zero, native.SecurityCredentials);
        }

        [Fact]
        public void ForwardsSecurityCredentialsToNativeStruct()
        {
            // WindowsCredentials marshals to FABRIC_SECURITY_CREDENTIALS with Kind = WINDOWS (2),
            // distinguishing it from the default NONE (0) kind so the assertion verifies that the
            // pointer actually points to the credentials produced by SecurityCredentials.ToNative.
            transportSettings.SecurityCredentials = new WindowsCredentials();

            IntPtr ptr = transportSettings.ToNativeV2(pin);

            FABRIC_TRANSPORT_SETTINGS native = Native(ptr);
            Assert.NotEqual(IntPtr.Zero, native.SecurityCredentials);
            Assert.Equal((int)CredentialType.Windows, Marshal.ReadInt32(native.SecurityCredentials));
        }

        [Fact]
        public void MarshalsOperationTimeoutToNativeStruct()
        {
            // Drive from a known integer input so the assertion verifies int -> uint marshalling, not the SUT's cast.
            int operationSeconds = fuzzy.Int32().Minimum(0);
            transportSettings.OperationTimeout = TimeSpan.FromSeconds(operationSeconds);

            IntPtr ptr = transportSettings.ToNativeV2(pin);

            FABRIC_TRANSPORT_SETTINGS native = Native(ptr);
            Assert.Equal(Convert.ToUInt32(operationSeconds), native.OperationTimeoutInSeconds);
        }

        [Fact]
        public void ClampsOperationTimeoutToZeroWhenNegative()
        {
            transportSettings.OperationTimeout = -fuzzy.TimeSpan().Seconds();

            IntPtr ptr = transportSettings.ToNativeV2(pin);

            FABRIC_TRANSPORT_SETTINGS native = Native(ptr);
            Assert.Equal(0u, native.OperationTimeoutInSeconds);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. FabricTransportSettingsExtension.ToNativeV2 does not validate OperationTimeout upper bound.
        public void ThrowsArgumentOutOfRangeExceptionWhenOperationTimeoutExceedsUInt32MaxSeconds()
        {
            // ToNativeV2 casts OperationTimeout.TotalSeconds directly to uint without range checking, so values
            // greater than uint.MaxValue silently overflow instead of throwing ArgumentOutOfRangeException with
            // ParamName "OperationTimeout".
            transportSettings.OperationTimeout = TimeSpan.FromSeconds((double)uint.MaxValue + 1);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => transportSettings.ToNativeV2(pin));
            Assert.Equal(nameof(FabricTransportSettings.OperationTimeout), ex.ParamName);
        }

        [Fact]
        public void MarshalsKeepAliveTimeoutToNativeStruct()
        {
            // Drive from a known integer input so the assertion verifies int -> uint marshalling, not the SUT's cast.
            int keepAliveSeconds = fuzzy.Int32().Minimum(0);
            transportSettings.KeepAliveTimeout = TimeSpan.FromSeconds(keepAliveSeconds);

            IntPtr ptr = transportSettings.ToNativeV2(pin);

            FABRIC_TRANSPORT_SETTINGS native = Native(ptr);
            Assert.Equal(Convert.ToUInt32(keepAliveSeconds), native.KeepAliveTimeoutInSeconds);
        }

        [Fact]
        public void ClampsKeepAliveTimeoutToZeroWhenNegative()
        {
            transportSettings.KeepAliveTimeout = -fuzzy.TimeSpan().Seconds();

            IntPtr ptr = transportSettings.ToNativeV2(pin);

            FABRIC_TRANSPORT_SETTINGS native = Native(ptr);
            Assert.Equal(0u, native.KeepAliveTimeoutInSeconds);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. FabricTransportSettingsExtension.ToNativeV2 does not validate KeepAliveTimeout upper bound.
        public void ThrowsArgumentOutOfRangeExceptionWhenKeepAliveTimeoutExceedsUInt32MaxSeconds()
        {
            // ToNativeV2 casts KeepAliveTimeout.TotalSeconds directly to uint without range checking, so values
            // greater than uint.MaxValue silently overflow instead of throwing ArgumentOutOfRangeException with
            // ParamName "KeepAliveTimeout".
            transportSettings.KeepAliveTimeout = TimeSpan.FromSeconds((double)uint.MaxValue + 1);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => transportSettings.ToNativeV2(pin));
            Assert.Equal(nameof(FabricTransportSettings.KeepAliveTimeout), ex.ParamName);
        }

        [Fact]
        public void MarshalsScalarSettingsToNativeStruct()
        {
            // Drive from a known integer input so the assertion verifies int -> uint marshalling, not the SUT's cast.
            int maxMessageSize = fuzzy.Int32().Minimum(0);
            int maxConcurrentCalls = fuzzy.Int32().Minimum(0);
            int maxQueueSize = fuzzy.Int32().Minimum(0);
            transportSettings.MaxMessageSize = maxMessageSize;
            transportSettings.MaxConcurrentCalls = maxConcurrentCalls;
            transportSettings.MaxQueueSize = maxQueueSize;

            IntPtr ptr = transportSettings.ToNativeV2(pin);

            FABRIC_TRANSPORT_SETTINGS native = Native(ptr);
            Assert.Equal(Convert.ToUInt32(maxMessageSize), native.MaxMessageSize);
            Assert.Equal(Convert.ToUInt32(maxConcurrentCalls), native.MaxConcurrentCalls);
            Assert.Equal(Convert.ToUInt32(maxQueueSize), native.MaxQueueSize);
        }

        [Fact]
        public void ThrowsArgumentOutOfRangeExceptionWhenMaxMessageSizeIsNegative()
        {
            transportSettings.MaxMessageSize = fuzzy.Int64().Maximum(-1);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => transportSettings.ToNativeV2(pin));
            Assert.Equal(nameof(FabricTransportSettings.MaxMessageSize), ex.ParamName);
        }

        [Fact]
        public void ThrowsArgumentOutOfRangeExceptionWhenMaxMessageSizeExceedsInt32MaxValue()
        {
            transportSettings.MaxMessageSize = fuzzy.Int64().Minimum((long)int.MaxValue + 1);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => transportSettings.ToNativeV2(pin));
            Assert.Equal(nameof(FabricTransportSettings.MaxMessageSize), ex.ParamName);
        }

        [Fact]
        public void ThrowsArgumentOutOfRangeExceptionWhenMaxConcurrentCallsIsNegative()
        {
            transportSettings.MaxConcurrentCalls = fuzzy.Int64().Maximum(-1);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => transportSettings.ToNativeV2(pin));
            Assert.Equal(nameof(FabricTransportSettings.MaxConcurrentCalls), ex.ParamName);
        }

        [Fact]
        public void ThrowsArgumentOutOfRangeExceptionWhenMaxConcurrentCallsExceedsInt32MaxValue()
        {
            transportSettings.MaxConcurrentCalls = fuzzy.Int64().Minimum((long)int.MaxValue + 1);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => transportSettings.ToNativeV2(pin));
            Assert.Equal(nameof(FabricTransportSettings.MaxConcurrentCalls), ex.ParamName);
        }

        [Fact]
        public void ThrowsArgumentOutOfRangeExceptionWhenMaxQueueSizeIsNegative()
        {
            transportSettings.MaxQueueSize = fuzzy.Int64().Maximum(-1);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => transportSettings.ToNativeV2(pin));
            Assert.Equal(nameof(FabricTransportSettings.MaxQueueSize), ex.ParamName);
        }

        [Fact]
        public void ThrowsArgumentOutOfRangeExceptionWhenMaxQueueSizeExceedsInt32MaxValue()
        {
            transportSettings.MaxQueueSize = fuzzy.Int64().Minimum((long)int.MaxValue + 1);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => transportSettings.ToNativeV2(pin));
            Assert.Equal(nameof(FabricTransportSettings.MaxQueueSize), ex.ParamName);
        }

        [Fact]
        public void MarshalsConnectTimeoutToNativeStruct()
        {
            // Drive from a known integer input so the assertion verifies int -> uint marshalling, not the SUT's cast.
            int connectMilliseconds = fuzzy.Int32().Minimum(0);
            transportSettings.ConnectTimeout = TimeSpan.FromMilliseconds(connectMilliseconds);

            IntPtr ptr = transportSettings.ToNativeV2(pin);

            FABRIC_TRANSPORT_SETTINGS native = Native(ptr);
            FABRIC_TRANSPORT_SETTINGS_EX1 ex1 = Ex1(native);
            Assert.Equal(Convert.ToUInt32(connectMilliseconds), ex1.ConnectTimeoutInMilliseconds);
        }

        [Fact]
        public void UsesDefaultConnectTimeoutWhenNegative()
        {
            transportSettings.ConnectTimeout = -fuzzy.TimeSpan().Milliseconds();

            IntPtr ptr = transportSettings.ToNativeV2(pin);

            FABRIC_TRANSPORT_SETTINGS native = Native(ptr);
            FABRIC_TRANSPORT_SETTINGS_EX1 ex1 = Ex1(native);
            Assert.Equal(Convert.ToUInt32(FabricTransportSettings.DefaultConnectTimeout.TotalMilliseconds), ex1.ConnectTimeoutInMilliseconds);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. FabricTransportSettingsExtension.ToNativeV2 does not validate ConnectTimeout upper bound.
        public void ThrowsArgumentOutOfRangeExceptionWhenConnectTimeoutExceedsUInt32MaxMilliseconds()
        {
            // ToNativeV2 casts ConnectTimeout.TotalMilliseconds directly to uint without range checking, so values
            // greater than uint.MaxValue silently overflow instead of throwing ArgumentOutOfRangeException with
            // ParamName "ConnectTimeout".
            transportSettings.ConnectTimeout = TimeSpan.FromMilliseconds((double)uint.MaxValue + 1);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => transportSettings.ToNativeV2(pin));
            Assert.Equal(nameof(FabricTransportSettings.ConnectTimeout), ex.ParamName);
        }

        [Fact]
        public void EnablesMaxConcurrentCallsWhenGreaterThanZero()
        {
            transportSettings.MaxConcurrentCalls = fuzzy.Int32().Minimum(1);

            IntPtr ptr = transportSettings.ToNativeV2(pin);

            FABRIC_TRANSPORT_SETTINGS native = Native(ptr);
            FABRIC_TRANSPORT_SETTINGS_EX1 ex1 = Ex1(native);
            FABRIC_TRANSPORT_SETTINGS_EX2 ex2 = Ex2(ex1);
            Assert.Equal(NativeTypes.ToBOOLEAN(true), ex2.EnableMaxConcurrentCalls);
        }

        [Fact]
        public void DisablesMaxConcurrentCallsWhenZero()
        {
            transportSettings.MaxConcurrentCalls = 0;

            IntPtr ptr = transportSettings.ToNativeV2(pin);

            FABRIC_TRANSPORT_SETTINGS native = Native(ptr);
            FABRIC_TRANSPORT_SETTINGS_EX1 ex1 = Ex1(native);
            FABRIC_TRANSPORT_SETTINGS_EX2 ex2 = Ex2(ex1);
            Assert.Equal(NativeTypes.ToBOOLEAN(false), ex2.EnableMaxConcurrentCalls);
        }

        static FABRIC_TRANSPORT_SETTINGS Native(IntPtr ptr) =>
            Marshal.PtrToStructure<FABRIC_TRANSPORT_SETTINGS>(ptr);

        static FABRIC_TRANSPORT_SETTINGS_EX1 Ex1(FABRIC_TRANSPORT_SETTINGS native) =>
            Marshal.PtrToStructure<FABRIC_TRANSPORT_SETTINGS_EX1>(native.Reserved);

        static FABRIC_TRANSPORT_SETTINGS_EX2 Ex2(FABRIC_TRANSPORT_SETTINGS_EX1 ex1) =>
            Marshal.PtrToStructure<FABRIC_TRANSPORT_SETTINGS_EX2>(ex1.Reserved);
    }
}
