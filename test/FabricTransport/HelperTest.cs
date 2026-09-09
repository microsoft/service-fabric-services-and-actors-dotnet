// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Fabric.Interop;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport;

public abstract class HelperTest
{
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class Get_Byte : HelperTest, IDisposable
    {
        // Method parameters
        readonly IntPtr message;

        readonly PinCollection pins = [];
        readonly byte[] bytes = fuzzy.Array(fuzzy.Byte);

        public Get_Byte()
        {
            NativeTypes.FABRIC_MESSAGE_BUFFER buffer = new()
            {
                BufferSize = (uint)bytes.Length,
                Buffer = pins.AddBlittable(bytes),
            };
            message = pins.AddBlittable(buffer);
        }

        void IDisposable.Dispose() => pins.Dispose();

        [Fact]
        public void ReturnsBytesOfNativeMessageBuffer() =>
            Assert.Equal(bytes, Helper.Get_Byte(message));

        [Fact(Explicit = true)] // TODO: SUT bug. Method throws NullReferenceException instead of ArgumentException.
        public void ThrowsArgumentExceptionWhenMessageIsZero()
        {
            // Get_Byte immediately dereferences the native pointer without a zero-check, so passing
            // IntPtr.Zero surfaces the low-level NullReferenceException instead of the expected
            // ArgumentException.
            var exception = Assert.Throws<ArgumentException>(() => Helper.Get_Byte(IntPtr.Zero));
            Assert.Equal(nameof(message), exception.ParamName);
        }
    }

    [WindowsOnly("Can't load libFabricCommon.so on Linux.")]
    public sealed class GetEndpointPort : HelperTest
    {
        // Method parameters
        readonly ICodePackageActivationContext codePackageActivationContext = Mock.Of<ICodePackageActivationContext>();
        // LettersOrDigits avoids characters whose case folding is unstable under the
        // InvariantCultureIgnoreCase comparison exercised by ReturnsPortOfFirstMatchingEndpoint.
        readonly string endpointResourceName = fuzzy.String().LettersOrDigits();

        readonly EndpointResourceDescriptionCollection endpoints = [];

        public GetEndpointPort() =>
            _ = Mock.Get(codePackageActivationContext).Setup(_ => _.GetEndpoints()).Returns(endpoints);

        [Fact]
        public void ReturnsPortOfEndpointWithMatchingName()
        {
            // Non-zero distinguishes a matched port from GetEndpointPort's not-found 0 sentinel.
            int expected = fuzzy.Int32().Minimum(1);
            endpoints.Add(CreateEndpoint(endpointResourceName + fuzzy.String(), fuzzy.Int32()));
            endpoints.Add(CreateEndpoint(endpointResourceName, expected));

            int actual = Helper.GetEndpointPort(codePackageActivationContext, endpointResourceName);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsPortOfFirstMatchingEndpoint()
        {
            string name = endpointResourceName + fuzzy.Char().Between('a', 'z');
            int expected = fuzzy.Int32().Between(1, int.MaxValue - 5);
            int other = expected + fuzzy.SByte().Between(1, 5);
            // Upper- and lower-case names are distinct ordinal keys in the collection but both
            // match the case-insensitive lookup; break must return the first one added.
            endpoints.Add(CreateEndpoint(name.ToUpperInvariant(), expected));
            endpoints.Add(CreateEndpoint(name.ToLowerInvariant(), other));

            int actual = Helper.GetEndpointPort(codePackageActivationContext, name);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsZeroWhenNoEndpointMatches()
        {
            // Non-zero distinguishes a wrongly-matched port from GetEndpointPort's not-found 0 sentinel.
            endpoints.Add(CreateEndpoint(endpointResourceName + fuzzy.String(), fuzzy.Int32().Minimum(1)));

            int actual = Helper.GetEndpointPort(codePackageActivationContext, endpointResourceName);

            Assert.Equal(0, actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Throws NullReferenceException instead of ArgumentNullException.
        public void ThrowsArgumentNullExceptionWhenCodePackageActivationContextIsNull()
        {
            // GetEndpointPort calls GetEndpoints() on the null argument, surfacing the low-level
            // NullReferenceException instead of the expected ArgumentNullException.
            var exception = Assert.Throws<ArgumentNullException>(() => Helper.GetEndpointPort(null, endpointResourceName));
            Assert.Equal(nameof(codePackageActivationContext), exception.ParamName);
        }

        static EndpointResourceDescription CreateEndpoint(string name, int port)
        {
            EndpointResourceDescription endpoint = new() { Name = name };
            endpoint.Property<int>().Set(port);
            return endpoint;
        }

        sealed class EndpointResourceDescriptionCollection : KeyedCollection<string, EndpointResourceDescription>
        {
            protected override string GetKeyForItem(EndpointResourceDescription item) => item.Name;
        }
    }

    public sealed class ThrowIfValueOutOfBounds : HelperTest
    {
        long value;
        readonly string argumentName = fuzzy.String();

        [Fact]
        public void DoesNotThrowWhenValueIsZero()
        {
            value = 0;
            Helper.ThrowIfValueOutOfBounds(value, argumentName);
        }

        [Fact]
        public void DoesNotThrowWhenValueIsIntMaxValue()
        {
            value = int.MaxValue;
            Helper.ThrowIfValueOutOfBounds(value, argumentName);
        }

        [Fact]
        public void ThrowsArgumentOutOfRangeExceptionWhenValueIsNegative()
        {
            value = -1;
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Helper.ThrowIfValueOutOfBounds(value, argumentName));
            Assert.Equal(argumentName, exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentOutOfRangeExceptionWhenValueExceedsIntMaxValue()
        {
            value = (long)int.MaxValue + 1;
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Helper.ThrowIfValueOutOfBounds(value, argumentName));
            Assert.Equal(argumentName, exception.ParamName);
        }
    }
}
