// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Fabric;
using System.Fabric.Interop;
using System.Runtime.InteropServices;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime;

public abstract class FabricTransportListenerAddressTest
{
    readonly FabricTransportListenerAddress sut;

    // Constructor parameters
    readonly string ipAddressOrFQDN = fuzzy.String();
    readonly int port = fuzzy.Int32();
    readonly string path = fuzzy.String();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    FabricTransportListenerAddressTest() =>
        sut = new FabricTransportListenerAddress(ipAddressOrFQDN, port, path);

    public sealed class Constructor : FabricTransportListenerAddressTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Same(ipAddressOrFQDN, sut.IpAddressOrFQDN);
            Assert.Equal(port, sut.Port);
            Assert.Same(path, sut.Path);
        }
    }

    public sealed class IpAddressOrFQDN : FabricTransportListenerAddressTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            string expected = fuzzy.String();
            sut.IpAddressOrFQDN = expected;
            Assert.Same(expected, sut.IpAddressOrFQDN);
        }
    }

    public sealed class Path : FabricTransportListenerAddressTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            string expected = fuzzy.String();
            sut.Path = expected;
            Assert.Same(expected, sut.Path);
        }
    }

    public sealed class Port : FabricTransportListenerAddressTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            int expected = fuzzy.Int32();
            sut.Port = expected;
            Assert.Equal(expected, sut.Port);
        }
    }

    public sealed class ToNative : FabricTransportListenerAddressTest, IDisposable
    {
        readonly PinCollection pin = [];

        public ToNative() => sut.Port = fuzzy.Int32().Minimum(0);

        void IDisposable.Dispose() => pin.Dispose();

        [Fact]
        public void MarshalsPropertiesToNativeStruct()
        {
            var expectedPort = (uint)sut.Port;

            IntPtr ptr = sut.ToNative(pin);

            var native = Marshal.PtrToStructure<NativeTypes.FABRIC_SERVICE_LISTENER_ADDRESS>(ptr);
            Assert.Equal(ipAddressOrFQDN, Marshal.PtrToStringUni(native.IPAddressOrFQDN));
            Assert.Equal(expectedPort, native.Port);
            Assert.Equal(path, Marshal.PtrToStringUni(native.Path));
        }

        [Theory, InlineData(null), InlineData("")]
        public void ThrowsFabricInvalidAddressExceptionWhenIpAddressOrFQDNIsNullOrEmpty(string value)
        {
            sut.IpAddressOrFQDN = value;
            _ = Assert.Throws<FabricInvalidAddressException>(() => sut.ToNative(pin));
        }

        [Fact]
        public void ThrowsArgumentOutOfRangeExceptionWhenPortIsNegative()
        {
            sut.Port = fuzzy.Int32().Maximum(-1);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => sut.ToNative(pin));
            Assert.Equal(nameof(FabricTransportListenerAddress.Port), exception.ParamName);
        }

        [Theory, InlineData(null), InlineData("")]
        public void ThrowsFabricInvalidAddressExceptionWhenPathIsNullOrEmpty(string value)
        {
            sut.Path = value;
            _ = Assert.Throws<FabricInvalidAddressException>(() => sut.ToNative(pin));
        }

        [Fact(Explicit = true)] // TODO: SUT bug. ToNative throws NullReferenceException instead of ArgumentNullException.
        public void ThrowsArgumentNullExceptionWhenPinIsNull()
        {
            // FabricTransportListenerAddress.ToNative dereferences pin without validation, so passing
            // null surfaces the low-level NullReferenceException instead of the expected
            // ArgumentNullException.
            var exception = Assert.Throws<ArgumentNullException>(() => sut.ToNative(null));
            Assert.Equal(nameof(pin), exception.ParamName);
        }
    }
}
