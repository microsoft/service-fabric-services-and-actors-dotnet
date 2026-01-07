// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Xunit;

    /// <summary>
    /// Tests for SegmentedPoolMemoryStream.
    /// </summary>
    public class SegmentedMemoryStreamTest
    {
        /// <summary>
        /// Write Array Size Less Than BufferSize.
        /// </summary>
        [Fact]
        public void TestWriteArraySizeLessThanBufferSize()
        {
            var bufferPoolManager = new BufferPoolManager(1000, 10);
            var stream = new SegmentedPoolMemoryStream(bufferPoolManager);
            var input = new byte[3567];
            for (var i = 0; i < input.Length; i++)
            {
                input[i] = (byte)i;
            }

            stream.Write(input, 0, input.Length);
            Assert.True(stream.Length == input.Length, "Buffer Length comparison");
            Assert.True(
                Enumerable.SequenceEqual(this.ToArray(stream.GetBuffers(), (int)stream.Length), input),
                "Buffer Comaparison");

            foreach (var buffers in stream.GetBuffers())
            {
                buffers.Release();
            }

            stream.Dispose();
        }

        /// <summary>
        /// Test headers with more than one buffer.
        /// </summary>
        [Fact]
        public void TestHeadersWithMoreThan1Buffer()
        {
            var bufferPoolManager = new BufferPoolManager(1000, 10);
            var stream = new SegmentedPoolMemoryStream(bufferPoolManager);
            var input = new byte[3567];
            for (var i = 0; i < input.Length; i++)
            {
                input[i] = (byte)i;
            }

            stream.Write(input, 0, input.Length);
            Assert.True(stream.Length == input.Length, "Buffer Length comparison");
            var buffers = stream.GetBuffers();
            Assert.True(buffers.Count() > 1);
            var header = new OutgoingMessageHeader(buffers);
            Assert.True(header.GetSendBuffer().Count == input.Length, "Length should be same");
            Assert.True(header.GetSendBuffer().SequenceEqual(input), "Buffer should be same");
        }

        /// <summary>
        /// Test read from segmented buffer.
        /// </summary>
        [Fact]
        public void TestReadFromSegmentedBuffer()
        {
            var input = new byte[3567];
            for (var i = 0; i < input.Length; i++)
            {
                input[i] = (byte)i;
            }

            var segments = new List<ArraySegment<byte>>();
            segments.Add(new ArraySegment<byte>(input, 0, 1000));
            segments.Add(new ArraySegment<byte>(input, 1000, 1000));
            segments.Add(new ArraySegment<byte>(input, 2000, 1000));
            segments.Add(new ArraySegment<byte>(input, 3000, 567));

            // Read FromStream
            var readStream = new SegmentedReadMemoryStream(segments);
            var input1 = new byte[readStream.Length];
            var bytesRead = readStream.Read(input1, 0, input1.Length);
            Assert.True(bytesRead == input1.Length, "Length Comparison");
            Assert.True(input1.SequenceEqual(input), "Comparing Data");
            readStream.Dispose();
        }

        /// <summary>
        /// Test read byte from segmented buffer.
        /// </summary>
        [Fact]
        public void TestReadByteFromSegmentedBuffer()
        {
            var input = new byte[3567];
            for (var i = 0; i < input.Length; i++)
            {
                input[i] = (byte)(i + 1);
            }

            var segments = new List<ArraySegment<byte>>();
            segments.Add(new ArraySegment<byte>(input, 0, 1000));
            segments.Add(new ArraySegment<byte>(input, 1000, 1000));
            segments.Add(new ArraySegment<byte>(input, 2000, 1000));
            segments.Add(new ArraySegment<byte>(input, 3000, 567));

            // Read FromStream
            var readStream = new SegmentedReadMemoryStream(segments);
            var bytesRead = readStream.ReadByte();
            Assert.True(input[0] == bytesRead, "Comparing Data");
            readStream.Dispose();
        }

        /// <summary>
        /// Test write array size same as BufferSize.
        /// </summary>
        [Fact]
        public void TestWriteArraySizeSameAsBufferSize()
        {
            var bufferPoolManager = new BufferPoolManager(1000, 10);
            var stream = new SegmentedPoolMemoryStream(bufferPoolManager);
            var input = new byte[4000];
            for (var i = 0; i < input.Length; i++)
            {
                input[i] = (byte)i;
            }

            stream.Write(input, 0, input.Length);
            Assert.True(stream.Length == input.Length);
            Assert.True(Enumerable.SequenceEqual(this.ToArray(stream.GetBuffers(), (int)stream.Length), input));
            Assert.True(stream.GetBuffers().Count<IPooledBuffer>() == 4);
            foreach (var buffers in stream.GetBuffers())
            {
                buffers.Release();
            }

            stream.Dispose();
        }

        internal byte[] ToArray(IEnumerable<IPooledBuffer> buffers, int totallength)
        {
            var arr = new byte[totallength];
            var bytesRead = 0;
            var bufNum = 0;
            for (; bufNum < buffers.Count() - 1; bufNum++)
            {
                var arraySegment = buffers.ElementAt(bufNum).Value;
                Buffer.BlockCopy(arraySegment.Array, arraySegment.Offset, arr, bytesRead, arraySegment.Count);
                bytesRead += arraySegment.Count;
            }

            // for the last buffer,Segment might not be fully occupied
            if (bufNum == buffers.Count() - 1)
            {
                var arraySegment = buffers.ElementAt(buffers.Count() - 1).Value;
                Buffer.BlockCopy(
                    arraySegment.Array,
                    arraySegment.Offset,
                    arr,
                    bytesRead,
                    totallength - bytesRead);
            }

            return arr;
        }
    }
}
