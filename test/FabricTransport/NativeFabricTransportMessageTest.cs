// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.Generic;
using System.Fabric.Interop;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;
using static Microsoft.ServiceFabric.FabricTransport.NativeFabricTransport;

namespace Microsoft.ServiceFabric.FabricTransport;

public abstract class NativeFabricTransportMessageTest : IDisposable
{
    readonly NativeFabricTransportMessage sut;

    // Constructor parameters
    readonly FabricTransportMessage message;

    readonly byte[] headerBytes;
    readonly List<byte[]> bodyBytes;
    readonly Action headerDispose = Mock.Of<Action>();
    readonly Action bodyDispose = Mock.Of<Action>();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    NativeFabricTransportMessageTest()
    {
        headerBytes = fuzzy.Array(fuzzy.Byte);
        bodyBytes = fuzzy.List(() => fuzzy.Array(fuzzy.Byte));
        FabricTransportRequestHeader header = new(Slice(headerBytes), headerDispose);
        FabricTransportRequestBody body = new([.. bodyBytes.Select(Slice)], bodyDispose);
        message = new FabricTransportMessage(header, body);
        sut = new NativeFabricTransportMessage(message);
    }

    void IDisposable.Dispose() => sut.Dispose();

    public sealed class Constructor : NativeFabricTransportMessageTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Constructor throws NullReferenceException instead of ArgumentNullException.
        public void ThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            // The constructor stores `message` and immediately dereferences it via CreateNativeHeaderBytes
            // and CreateNativeBodyBytesPtr, which call message.GetHeader() / message.GetBody().
            var actual = Assert.Throws<ArgumentNullException>(() => new NativeFabricTransportMessage(null));
            Assert.Equal(nameof(message), actual.ParamName);
        }
    }

    public sealed class CreateNativeBodyBytes : NativeFabricTransportMessageTest
    {
        [Fact]
        public void ReturnsBufferPerBodyBufferContainingGivenBytes()
        {
            FABRIC_TRANSPORT_MESSAGE_BUFFER[] actual = sut.CreateNativeBodyBytes();

            Assert.Equal(bodyBytes.Count, actual.Length);
            for (int i = 0; i < bodyBytes.Count; i++)
            {
                Assert.Equal((uint)bodyBytes[i].Length, actual[i].BufferSize);
                var copy = new byte[actual[i].BufferSize];
                Marshal.Copy(actual[i].Buffer, copy, 0, copy.Length);
                Assert.Equal(bodyBytes[i], copy);
            }
        }
    }

    public sealed class CreateNativeBodyBytesPtr : NativeFabricTransportMessageTest
    {
        [Fact]
        public void ReturnsPointerToBufferPerBodyBufferContainingGivenBytes()
        {
            IntPtr bufferPtr = sut.CreateNativeBodyBytesPtr();

            int size = Marshal.SizeOf<FABRIC_TRANSPORT_MESSAGE_BUFFER>();
            for (int i = 0; i < bodyBytes.Count; i++)
            {
                var element = Marshal.PtrToStructure<FABRIC_TRANSPORT_MESSAGE_BUFFER>(bufferPtr + i * size);
                Assert.Equal((uint)bodyBytes[i].Length, element.BufferSize);
                var copy = new byte[element.BufferSize];
                Marshal.Copy(element.Buffer, copy, 0, copy.Length);
                Assert.Equal(bodyBytes[i], copy);
            }
        }

        [Fact]
        public void ReturnsPointerToEmptyBufferWhenBodyIsNull()
        {
            FabricTransportRequestHeader header = new(Slice(headerBytes), headerDispose);
            NativeFabricTransportMessage sut = new(new FabricTransportMessage(header, null));
            try
            {
                IntPtr bufferPtr = sut.CreateNativeBodyBytesPtr();

                var element = Marshal.PtrToStructure<FABRIC_TRANSPORT_MESSAGE_BUFFER>(bufferPtr);
                Assert.Equal(0u, element.BufferSize);
                Assert.Equal(IntPtr.Zero, element.Buffer);
            }
            finally
            {
                sut.Dispose();
            }
        }

        [Fact]
        public void ReturnsPointerToEmptyBufferWhenBodyBuffersAreEmpty()
        {
            FabricTransportRequestHeader header = new(Slice(headerBytes), headerDispose);
            FabricTransportRequestBody body = new([], bodyDispose);
            NativeFabricTransportMessage sut = new(new FabricTransportMessage(header, body));
            try
            {
                IntPtr bufferPtr = sut.CreateNativeBodyBytesPtr();

                var element = Marshal.PtrToStructure<FABRIC_TRANSPORT_MESSAGE_BUFFER>(bufferPtr);
                Assert.Equal(0u, element.BufferSize);
                Assert.Equal(IntPtr.Zero, element.Buffer);
            }
            finally
            {
                sut.Dispose();
            }
        }
    }

    public sealed class CreateNativeHeaderBytes : NativeFabricTransportMessageTest
    {
        [Fact]
        public void ReturnsBufferContainingHeaderBytesWhenHeaderIsNotNull()
        {
            FABRIC_TRANSPORT_MESSAGE_BUFFER actual = sut.CreateNativeHeaderBytes();

            Assert.Equal((uint)headerBytes.Length, actual.BufferSize);
            var copy = new byte[actual.BufferSize];
            Marshal.Copy(actual.Buffer, copy, 0, copy.Length);
            Assert.Equal(headerBytes, copy);
        }

        [Fact]
        public void ReturnsEmptyBufferWhenHeaderIsNull()
        {
            FabricTransportRequestBody body = new([.. bodyBytes.Select(Slice)], bodyDispose);
            NativeFabricTransportMessage sut = new(new FabricTransportMessage(null, body));
            try
            {
                FABRIC_TRANSPORT_MESSAGE_BUFFER actual = sut.CreateNativeHeaderBytes();

                Assert.Equal(0u, actual.BufferSize);
                Assert.Equal(IntPtr.Zero, actual.Buffer);
            }
            finally
            {
                sut.Dispose();
            }
        }
    }

    public sealed class CreateNativeHeaderBytes_ByteArray : NativeFabricTransportMessageTest
    {
        readonly byte[] bytes = fuzzy.Array(fuzzy.Byte);

        [Fact]
        public void ReturnsBufferContainingGivenBytesWhenBytesIsNotNull()
        {
            FABRIC_TRANSPORT_MESSAGE_BUFFER actual = sut.CreateNativeHeaderBytes(bytes);

            Assert.Equal((uint)bytes.Length, actual.BufferSize);
            var copy = new byte[actual.BufferSize];
            Marshal.Copy(actual.Buffer, copy, 0, copy.Length);
            Assert.Equal(bytes, copy);
        }

        [Fact]
        public void ReturnsEmptyBufferWhenBytesIsNull()
        {
            FABRIC_TRANSPORT_MESSAGE_BUFFER actual = sut.CreateNativeHeaderBytes(null);

            Assert.Equal(0u, actual.BufferSize);
            Assert.Equal(IntPtr.Zero, actual.Buffer);
        }
    }

    public sealed class Dispose : NativeFabricTransportMessageTest
    {
        [Fact]
        public void DisposesWrappedMessage()
        {
            sut.Dispose();

            Mock.Get(headerDispose).Verify(_ => _(), Times.Once);
            Mock.Get(bodyDispose).Verify(_ => _(), Times.Once);
        }

        [Fact]
        public void IsIdempotent()
        {
            sut.Dispose();
            sut.Dispose();

            Mock.Get(headerDispose).Verify(_ => _(), Times.Once);
            Mock.Get(bodyDispose).Verify(_ => _(), Times.Once);
        }

        // Pin release is not verified: the PinCollection allocated in the constructor is private and
        // PinCollection itself exposes no observable state after Dispose. Pin lifetime can only be verified
        // by inspecting native memory, which is unsafe and brittle.
    }

    public sealed class GetBytesFromNative : NativeFabricTransportMessageTest, IDisposable
    {
        // Method parameters
        readonly IntPtr ptr;

        readonly PinCollection pins = [];
        readonly byte[] bytes = fuzzy.Array(fuzzy.Byte);

        public GetBytesFromNative()
        {
            FABRIC_TRANSPORT_MESSAGE_BUFFER buffer = new()
            {
                BufferSize = (uint)bytes.Length,
                Buffer = pins.AddBlittable(bytes),
            };
            ptr = pins.AddBlittable(buffer);
        }

        void IDisposable.Dispose()
        {
            pins.Dispose();
            sut.Dispose();
        }

        [Fact]
        public void ReturnsCopyOfBufferBytesPointedToByGivenPtr()
        {
            byte[] actual = NativeFabricTransportMessage.GetBytesFromNative(ptr);
            Assert.Equal(bytes, actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Method throws NullReferenceException instead of ArgumentException.
        public void ThrowsArgumentExceptionWhenPtrIsZero()
        {
            // GetBytesFromNative immediately dereferences the native pointer without a zero-check, so
            // passing IntPtr.Zero surfaces the low-level NullReferenceException instead of the expected
            // ArgumentException.
            var actual = Assert.Throws<ArgumentException>(() => NativeFabricTransportMessage.GetBytesFromNative(IntPtr.Zero));
            Assert.Equal(nameof(ptr), actual.ParamName);
        }
    }

    public sealed class GetHeaderAndBodyBuffer : NativeFabricTransportMessageTest
    {
        [Fact]
        public void ReturnsNativeHeaderAndBodyBuffers()
        {
            sut.GetHeaderAndBodyBuffer(out IntPtr headerPtr, out uint bufferLength, out IntPtr bufferPtr);

            var header = Marshal.PtrToStructure<FABRIC_TRANSPORT_MESSAGE_BUFFER>(headerPtr);
            Assert.Equal((uint)headerBytes.Length, header.BufferSize);
            Assert.Equal(headerBytes, ReadBytes(header.Buffer, header.BufferSize));

            Assert.Equal((uint)bodyBytes.Count, bufferLength);

            int size = Marshal.SizeOf<FABRIC_TRANSPORT_MESSAGE_BUFFER>();
            for (int i = 0; i < bodyBytes.Count; i++)
            {
                var element = Marshal.PtrToStructure<FABRIC_TRANSPORT_MESSAGE_BUFFER>(bufferPtr + i * size);
                Assert.Equal((uint)bodyBytes[i].Length, element.BufferSize);
                Assert.Equal(bodyBytes[i], ReadBytes(element.Buffer, element.BufferSize));
            }
        }

        static byte[] ReadBytes(IntPtr ptr, uint size)
        {
            var buffer = new byte[size];
            Marshal.Copy(ptr, buffer, 0, buffer.Length);
            return buffer;
        }
    }

    public sealed class ToFabricTransportMessage : NativeFabricTransportMessageTest, IDisposable
    {
        // Method parameters
        new readonly Mock<IFabricTransportMessage> message = new();

        readonly PinCollection pins = [];

        readonly IntPtr headerPtr;
        readonly IntPtr bodyPtr;

        public ToFabricTransportMessage()
        {
            headerPtr = PinHeader(headerBytes);
            bodyPtr = PinBody(bodyBytes);
            SetupGetHeaderAndBodyBuffer(headerPtr, (uint)bodyBytes.Count, bodyPtr);
        }

        void IDisposable.Dispose()
        {
            pins.Dispose();
            sut.Dispose();
        }

        [Fact]
        public void CallsGetHeaderAndBodyBufferOnce()
        {
            using var result = NativeFabricTransportMessage.ToFabricTransportMessage(message.Object);

            message.Verify(_ => _.GetHeaderAndBodyBuffer(out It.Ref<IntPtr>.IsAny, out It.Ref<uint>.IsAny, out It.Ref<IntPtr>.IsAny), Times.Once);
        }

        [Fact]
        public void ReturnsMessageWrappingGivenNativeMessage()
        {
            using var result = NativeFabricTransportMessage.ToFabricTransportMessage(message.Object);

            Assert.Same(message.Object, result.Field<IFabricTransportMessage>().Value);
        }

        [Fact]
        public void ReturnsMessageWithRequestHeaderWhenHeaderBufferIsNonZero()
        {
            using var result = NativeFabricTransportMessage.ToFabricTransportMessage(message.Object);

            Assert.Equal(headerBytes, ReadAllBytes(result.GetHeader().GetRecievedStream()));
        }

        [Fact]
        public void ReturnsMessageWithNullRequestHeaderWhenHeaderBufferIsZero()
        {
            SetupGetHeaderAndBodyBuffer(IntPtr.Zero, (uint)bodyBytes.Count, bodyPtr);

            using var result = NativeFabricTransportMessage.ToFabricTransportMessage(message.Object);

            Assert.Null(result.GetHeader());
        }

        [Fact]
        public void ReturnsMessageWithEmptyRequestHeaderWhenHeaderBufferWrapsZeroSizedBuffer()
        {
            // Documents the round-trip asymmetry between the outbound and inbound branches.
            // GetHeaderAndBodyBuffer wraps a null FabricTransportRequestHeader as a non-zero pointer to a
            // FABRIC_TRANSPORT_MESSAGE_BUFFER with BufferSize=0 and Buffer=IntPtr.Zero (see
            // CreateNativeHeaderBytes.ReturnsEmptyBufferWhenHeaderIsNull). ToFabricTransportMessage
            // only checks headerBuffer != IntPtr.Zero, so feeding the outbound representation back in yields
            // a non-null FabricTransportRequestHeader wrapping a zero-sized stream instead of null.
            FABRIC_TRANSPORT_MESSAGE_BUFFER emptyHeaderBuffer = new() { BufferSize = 0u, Buffer = IntPtr.Zero };
            IntPtr emptyHeaderPtr = pins.AddBlittable(emptyHeaderBuffer);
            SetupGetHeaderAndBodyBuffer(emptyHeaderPtr, (uint)bodyBytes.Count, bodyPtr);

            using var result = NativeFabricTransportMessage.ToFabricTransportMessage(message.Object);

            Assert.Empty(ReadAllBytes(result.GetHeader().GetRecievedStream()));
        }

        [Fact]
        public void ReturnsMessageWithRequestBodyWhenBufferLengthIsNonZero()
        {
            using var result = NativeFabricTransportMessage.ToFabricTransportMessage(message.Object);

            Assert.Equal(bodyBytes.SelectMany(_ => _), ReadAllBytes(result.GetBody().GetRecievedStream()));
        }

        [Fact]
        public void ReturnsMessageWithNullRequestBodyWhenBufferLengthIsZero()
        {
            SetupGetHeaderAndBodyBuffer(headerPtr, 0u, IntPtr.Zero);

            using var result = NativeFabricTransportMessage.ToFabricTransportMessage(message.Object);

            Assert.Null(result.GetBody());
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Method throws NullReferenceException instead of ArgumentNullException.
        public void ThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            // ToFabricTransportMessage dereferences `message` to call message.GetHeaderAndBodyBuffer(...)
            // without a null check.
            var actual = Assert.Throws<ArgumentNullException>(() => NativeFabricTransportMessage.ToFabricTransportMessage(null));
            Assert.Equal(nameof(message), actual.ParamName);
        }

        void SetupGetHeaderAndBodyBuffer(IntPtr headerPtr, uint count, IntPtr bodyPtr) =>
            _ = message.Setup(_ => _.GetHeaderAndBodyBuffer(out headerPtr, out count, out bodyPtr));

        IntPtr PinHeader(byte[] bytes)
        {
            FABRIC_TRANSPORT_MESSAGE_BUFFER buffer = new()
            {
                BufferSize = (uint)bytes.Length,
                Buffer = pins.AddBlittable(bytes),
            };
            return pins.AddBlittable(buffer);
        }

        IntPtr PinBody(IReadOnlyList<byte[]> buffers)
        {
            var array = new FABRIC_TRANSPORT_MESSAGE_BUFFER[buffers.Count];
            for (int i = 0; i < buffers.Count; i++)
            {
                array[i].BufferSize = (uint)buffers[i].Length;
                array[i].Buffer = pins.AddBlittable(buffers[i]);
            }
            return pins.AddBlittable(array);
        }

        static byte[] ReadAllBytes(Stream stream)
        {
            using MemoryStream memory = new();
            stream.CopyTo(memory);
            return memory.ToArray();
        }
    }

    // Wraps `bytes` in an ArraySegment with non-zero Offset and Count < backing array length, so any
    // SUT that substitutes Array.Length for Count or ignores Offset surfaces wrong bytes/sizes.
    static ArraySegment<byte> Slice(byte[] bytes)
    {
        byte[] prefix = fuzzy.Array(fuzzy.Byte, Fuzzy.Length.Min(1));
        byte[] suffix = fuzzy.Array(fuzzy.Byte, Fuzzy.Length.Min(1));
        var backing = new byte[prefix.Length + bytes.Length + suffix.Length];
        Buffer.BlockCopy(prefix, 0, backing, 0, prefix.Length);
        Buffer.BlockCopy(bytes, 0, backing, prefix.Length, bytes.Length);
        Buffer.BlockCopy(suffix, 0, backing, prefix.Length + bytes.Length, suffix.Length);
        return new ArraySegment<byte>(backing, prefix.Length, bytes.Length);
    }
}
