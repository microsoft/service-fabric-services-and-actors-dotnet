// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Messaging
{
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Messaging;
    using Xunit;

    /// <summary>
    /// Tests for BufferPoolManager.
    /// </summary>
    public class BufferPoolManagerTest
    {
        /// <summary>
        /// Test BufferPoolManager.Release.
        /// </summary>
        [Fact]
        public void TestReleaseBufferFromManager()
        {
            var bufferManager = new BufferPoolManager(100, 5);
            var pooledBuffers = new List<IPooledBuffer>();

            // Use Max Buffer
            for (var i = 0; i < 5; i++)
            {
                var buffer = bufferManager.TakeBuffer();
                Assert.True(buffer != null);
                pooledBuffers.Add(buffer);
            }

            var buffer1 = bufferManager.TakeBuffer();

            // ReleaseBuffer
            foreach (var pooledBuff in pooledBuffers)
            {
                Assert.True(pooledBuff.Release());
            }

            Assert.False(buffer1.Release());
        }

        /// <summary>
        /// Test BufferPoolManager size limit.
        /// </summary>
        [Fact]
        public void TestOutLimitBufferPoolSize()
        {
            var bufferManager = new BufferPoolManager(100, 1);
            var buffer = bufferManager.TakeBuffer();
            var buffer1 = bufferManager.TakeBuffer();
            Assert.True(buffer.Release());
            Assert.False(buffer1.Release(), "Limit is reached , so it return false");
        }

        /// <summary>
        /// Test BufferPoolManager Segment size.
        /// </summary>
        [Fact]
        public void TestBufferSegmentSize()
        {
            var bufferManager = new BufferPoolManager(100, 1);
            var buffer = bufferManager.TakeBuffer();
            Assert.True(buffer.Value.Count == 100);
            buffer.Release();
        }
    }
}
