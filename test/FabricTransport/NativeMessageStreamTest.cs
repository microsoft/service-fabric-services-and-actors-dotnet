// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.Generic;
using System.Fabric.Interop;
using System.IO;
using System.Linq;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport;

public abstract class NativeMessageStreamTest: IDisposable
{
    readonly Stream sut;

    // Constructor parameters
    readonly List<Tuple<uint, IntPtr>> bufferList;

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);
    readonly PinCollection pins = [];
    readonly List<byte[]> managedBuffers = fuzzy.List(
        static () => fuzzy.Array(fuzzy.Byte, Fuzzy.Length.Min(1)),
        Fuzzy.Count.Min(2));
    readonly byte[] expectedBytes;

    NativeMessageStreamTest()
    {
        bufferList = [.. managedBuffers.Select(_ => NativeTypes.ToNativeBytes(pins, _))];
        expectedBytes = [.. managedBuffers.SelectMany(static _ => _)];
        sut = new NativeMessageStream(bufferList);
    }

    void IDisposable.Dispose() =>
        pins.Dispose();

    public sealed class Constructor: NativeMessageStreamTest
    {
        [Fact]
        public void InitializesLengthToSumOfBufferLengths() =>
            Assert.Equal(managedBuffers.Sum(static _ => _.Length), sut.Length);

        [Fact]
        public void InitializesLengthToZeroWhenMessageIsEmpty()
        {
            using NativeMessageStream emptySut = new([]);
            Assert.Equal(0, emptySut.Length);
        }

        [Fact]
        public void InitializesPositionToZero() =>
            Assert.Equal(0, sut.Position);

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate bufferList; null surfaces as NullReferenceException from the private SetLength().
        public void ThrowsArgumentNullExceptionWhenBufferListIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(static () => new NativeMessageStream(null));
            Assert.Equal(nameof(bufferList), exception.ParamName);
        }
    }

    public sealed class CanRead: NativeMessageStreamTest
    {
        [Fact]
        public void ReturnsTrue() =>
            Assert.True(sut.CanRead);
    }

    public sealed class CanSeek: NativeMessageStreamTest
    {
        [Fact]
        public void ReturnsTrue() =>
            Assert.True(sut.CanSeek);
    }

    public sealed class CanWrite: NativeMessageStreamTest
    {
        [Fact]
        public void ReturnsFalse() =>
            Assert.False(sut.CanWrite);
    }

    public sealed class Dispose: NativeMessageStreamTest
    {
        [Fact]
        public void ClearsBufferList()
        {
            sut.Dispose();
            Assert.Empty(bufferList);
        }

        [Fact]
        public void DoesNotThrowWhenCalledMultipleTimes()
        {
            sut.Dispose();
            sut.Dispose();
        }
    }

    public sealed class Position: NativeMessageStreamTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            int expected = fuzzy.Int32().Minimum(0);
            sut.Position = expected;
            Assert.Equal(expected, sut.Position);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Position setter casts value to int, truncating values outside the int range.
        public void IsSetToGivenValueWhenValueExceedsIntMaxValue()
        {
            long expected = fuzzy.Int64().Minimum((long)int.MaxValue + 1);
            sut.Position = expected;
            Assert.Equal(expected, sut.Position);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Position setter does not validate that value is non-negative.
        public void ThrowsArgumentOutOfRangeExceptionWhenValueIsNegative()
        {
            long value = fuzzy.Int64().Maximum(-1);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => sut.Position = value);
            Assert.Equal(nameof(value), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Position setter updates the reported position without moving the read cursor.
        public void MovesReadCursorToGivenValue()
        {
            sut.Position = 1;
            var actual = new byte[expectedBytes.Length - 1];
            _ = sut.Read(actual, 0, actual.Length);
            Assert.Equal(expectedBytes.Skip(1), actual);
        }
    }

    public sealed class Read: NativeMessageStreamTest
    {
        readonly byte[] buffer;
        readonly int offset = 0;
        readonly int count;

        public Read()
        {
            count = expectedBytes.Length;
            buffer = new byte[count];
        }

        [Fact]
        public void CopiesEntireStreamAcrossNativeBuffers()
        {
            int bytesRead = sut.Read(buffer, offset, count);

            Assert.Equal(expectedBytes.Length, bytesRead);
            Assert.Equal(expectedBytes, buffer);
        }

        [Fact]
        public void AdvancesPositionByCumulativeNumberOfBytesRead()
        {
            int first = sut.Read(buffer, 0, 1);
            int second = sut.Read(buffer, 0, 1);
            Assert.Equal(first + second, sut.Position);
        }

        [Fact]
        public void ReadsSequentiallyAcrossMultipleCalls()
        {
            int firstChunk = expectedBytes.Length / 2;

            int firstRead = sut.Read(buffer, offset, firstChunk);
            int secondRead = sut.Read(buffer, firstRead, count - firstRead);

            Assert.Equal(firstChunk, firstRead);
            Assert.Equal(expectedBytes.Length - firstChunk, secondRead);
            Assert.Equal(expectedBytes, buffer);
        }

        [Fact]
        public void WritesAtGivenOffsetInOutputBuffer()
        {
            int prefix = fuzzy.Int32().Between(1, 5);
            byte[] output = fuzzy.Array(fuzzy.Byte, Fuzzy.Length.Exactly(count + prefix));
            byte[] originalPrefix = [.. output.Take(prefix)];

            int bytesRead = sut.Read(output, prefix, count);

            Assert.Equal(count, bytesRead);
            Assert.Equal(originalPrefix, output.Take(prefix));
            Assert.Equal(expectedBytes, output.Skip(prefix));
        }

        [Fact]
        public void ReturnsRemainingLengthWhenCountExceedsRemaining()
        {
            byte[] larger = fuzzy.Array(fuzzy.Byte, Fuzzy.Length.Exactly(count + fuzzy.Int32().Between(10, 20)));
            byte[] originalSuffix = [.. larger.Skip(count)];

            int bytesRead = sut.Read(larger, offset, larger.Length);

            Assert.Equal(count, bytesRead);
            Assert.Equal(expectedBytes, larger.Take(bytesRead));
            Assert.Equal(originalSuffix, larger.Skip(bytesRead));
        }

        [Fact]
        public void ReturnsZeroWhenThereAreNoMoreBytesToRead()
        {
            _ = sut.Read(buffer, offset, count);
            int bytesRead = sut.Read(buffer, offset, count);
            Assert.Equal(0, bytesRead);
        }

        [Fact]
        public void ReturnsZeroWhenMessageIsEmpty()
        {
            using NativeMessageStream emptySut = new([]);
            Assert.Equal(0, emptySut.Read(buffer, offset, count));
        }

        [Fact]
        public void ReturnsZeroAndDoesNotChangeStateWhenCountIsZero()
        {
            byte[] output = fuzzy.Array(fuzzy.Byte, Fuzzy.Length.Exactly(count));
            byte[] expected = [.. output];
            long position = sut.Position;

            int bytesRead = sut.Read(output, offset, 0);

            Assert.Equal(0, bytesRead);
            Assert.Equal(position, sut.Position);
            Assert.Equal(expected, output);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Read throws ArgumentNullException without ParamName.
        public void ThrowsArgumentNullExceptionWhenBufferIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => sut.Read(null, offset, count));
            Assert.Equal(nameof(buffer), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Read throws ArgumentOutOfRangeException without ParamName.
        public void ThrowsArgumentOutOfRangeExceptionWhenOffsetIsNegative()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => sut.Read(buffer, -1, count));
            Assert.Equal(nameof(offset), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Read throws ArgumentOutOfRangeException without ParamName.
        public void ThrowsArgumentOutOfRangeExceptionWhenCountIsNegative()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => sut.Read(buffer, offset, -1));
            Assert.Equal(nameof(count), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Read throws ArgumentOutOfRangeException instead of ArgumentException.
        public void ThrowsArgumentExceptionWhenOffsetPlusCountExceedsBufferLength() =>
            Assert.Throws<ArgumentException>(() => sut.Read(buffer, 1, count));
    }

    public sealed class ReadByte: NativeMessageStreamTest
    {
        [Fact]
        public void ReturnsBytesSequentiallyAcrossNativeBuffers()
        {
            for (int i = 0; i < expectedBytes.Length; i++)
                Assert.Equal(expectedBytes[i], sut.ReadByte());
        }

        [Fact]
        public void AdvancesPositionByOne()
        {
            _ = sut.ReadByte();
            long before = sut.Position;
            _ = sut.ReadByte();
            Assert.Equal(before + 1, sut.Position);
        }

        [Fact]
        public void ReturnsMinusOneAtEndOfStream()
        {
            for (int i = 0; i < expectedBytes.Length; i++)
                _ = sut.ReadByte();

            Assert.Equal(-1, sut.ReadByte());
        }
    }

    public sealed class Seek: NativeMessageStreamTest
    {
        readonly long offset;
        readonly SeekOrigin origin = SeekOrigin.Begin;

        public Seek() =>
            offset = fuzzy.Int64().Between(1, expectedBytes.Length - 1);

        [Fact]
        public void ResetsPositionToZeroWhenOriginIsBegin()
        {
            _ = sut.Read(new byte[expectedBytes.Length], 0, expectedBytes.Length);

            long result = sut.Seek(0, origin);

            Assert.Equal(0, result);
            Assert.Equal(0, sut.Position);
        }

        [Fact]
        public void RestartsReadingFromBeginningAfterSeekToBegin()
        {
            _ = sut.Read(new byte[expectedBytes.Length], 0, expectedBytes.Length);

            _ = sut.Seek(0, origin);

            var actual = new byte[expectedBytes.Length];
            int bytesRead = sut.Read(actual, 0, actual.Length);
            Assert.Equal(expectedBytes.Length, bytesRead);
            Assert.Equal(expectedBytes, actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Seek ignores offset and always resets to 0.
        public void SetsPositionToOffsetWhenOriginIsBegin()
        {
            long result = sut.Seek(offset, origin);

            Assert.Equal(offset, result);
            Assert.Equal(offset, sut.Position);
            Assert.Equal(expectedBytes[(int)offset], sut.ReadByte());
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Seek throws NotImplementedException for SeekOrigin.Current though CanSeek is true.
        public void SetsPositionRelativeToCurrentWhenOriginIsCurrent()
        {
            _ = sut.ReadByte();

            long result = sut.Seek(offset, SeekOrigin.Current);

            Assert.Equal(1 + offset, result);
            Assert.Equal(1 + offset, sut.Position);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Seek throws NotImplementedException for SeekOrigin.End though CanSeek is true.
        public void SetsPositionRelativeToEndWhenOriginIsEnd()
        {
            long result = sut.Seek(-offset, SeekOrigin.End);

            Assert.Equal(expectedBytes.Length - offset, result);
            Assert.Equal(expectedBytes.Length - offset, sut.Position);
        }
    }

    public sealed class SetLength: NativeMessageStreamTest
    {
        readonly long value = fuzzy.Int64();

        [Fact(Explicit = true)] // TODO: SUT bug. SetLength throws NotImplementedException; Stream requires NotSupportedException when not writable.
        public void ThrowsNotSupportedException() =>
            _ = Assert.Throws<NotSupportedException>(() => sut.SetLength(value));
    }

    public sealed class Write: NativeMessageStreamTest
    {
        readonly byte[] buffer = fuzzy.Array(fuzzy.Byte);
        readonly int offset = 0;
        readonly int count;

        public Write() => count = buffer.Length;

        [Fact(Explicit = true)] // TODO: SUT bug. Write throws NotImplementedException; Stream requires NotSupportedException when not writable.
        public void ThrowsNotSupportedException() =>
            _ = Assert.Throws<NotSupportedException>(() => sut.Write(buffer, offset, count));
    }
}
