// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric.Interop;
using System.Linq;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport;

public abstract class NativeMessageStreamTest: IDisposable
{
    readonly NativeMessageStream sut;

    // Constructor parameters
    readonly List<Tuple<uint, nint>> bufferList;

    // Test fixture
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);
    readonly PinCollection pins = [];
    readonly List<byte[]> managedBuffers = fuzzy.List(() => fuzzy.Array(fuzzy.Byte));

    protected NativeMessageStreamTest()
    {
        bufferList = [.. managedBuffers.Select(_ => NativeTypes.ToNativeBytes(pins, _))];
        sut = new NativeMessageStream(bufferList);
    }

    public void Dispose() =>
        pins.Dispose();

    public sealed class Length: NativeMessageStreamTest
    {
        [Fact]
        public void ReturnsSumOfBufferLengths() =>
            Assert.Equal(managedBuffers.Sum(_ => _.Length), sut.Length);
    }

    public sealed class Position: NativeMessageStreamTest
    {
        [Fact]
        public void ReturnsZeroBeforeRead() =>
            Assert.Equal(0, sut.Position);

        [Fact]
        public void ReturnsNewPositionAfterRead()
        {
            var output = new byte[fuzzy.Int32().Between(5, 10)];
            int expected = sut.Read(output, 0, output.Length);

            Assert.Equal(expected, sut.Position);
        }
    }

    public sealed class Read: NativeMessageStreamTest
    {
        [Fact]
        public void ReturnsNumberOfBytesReadAndPopulatesGivenArray()
        {
            byte[] expected = [.. managedBuffers.SelectMany(_ => _)];
            var actual = new byte[expected.Length];

            int bytesRead = sut.Read(actual, 0, actual.Length);

            Assert.Equal(expected.Length, bytesRead);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsZeroWhenThereAreNoMoreBytesToRead()
        {
            var output = new byte[managedBuffers.Sum(_ => _.Length)];
            _ = sut.Read(output, 0, output.Length);

            int bytesRead = sut.Read(output, 0, output.Length);

            Assert.Equal(0, bytesRead);
        }

        [Fact]
        public void ThrowsOutOfArgumentExceptionWhenCountExceedsOutputBufferLength()
        {
            byte[] output = fuzzy.Array(fuzzy.Byte, Fuzzy.Length.Between(10, 20));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => sut.Read(output, 0, output.Length + 1));
        }

        [Fact]
        public void ReturnsNumberOfBytesReadWhenOutputBufferLengthExceedsStreamLength()
        {
            var output = new byte[managedBuffers.Sum(_ => _.Length) + fuzzy.Int32().Between(10, 20)];
            int bytesRead = sut.Read(output, 0, output.Length);
            Assert.Equal(sut.Length, bytesRead);
        }
    }
}
