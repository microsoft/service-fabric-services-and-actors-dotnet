// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class SegmentedPoolMemoryStream : Stream
    {
        private readonly IBufferPoolManager bufferPoolManager;
        private List<IPooledBuffer> writeBuffers;
        private bool canRead;
        private bool canSeek;
        private bool canWrite;
        private long position;
        private int bufferSize;
        private int currentBufferOffset;
        private IPooledBuffer currentBuffer;

        public SegmentedPoolMemoryStream(IBufferPoolManager bufferPoolManager)
        {
            this.bufferPoolManager = bufferPoolManager;
            this.Initialize();
        }

        public override bool CanRead
        {
            get { return this.canRead; }
        }

        public override bool CanSeek
        {
            get { return this.canSeek; }
        }

        public override bool CanWrite
        {
            get { return this.canWrite; }
        }

        public override long Length
        {
            get { return this.position; }
        }

        public override long Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public override void Flush()
        {
            // no-op
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            this.position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if ((offset + count) > buffer.Length)
            {
                throw new ArgumentException("buffer too small", "buffer");
            }

            if (offset < 0)
            {
                throw new ArgumentException("offset must be >= 0", "offset");
            }

            if (count < 0)
            {
                throw new ArgumentException("count must be >= 0", "count");
            }

            var i = this.currentBufferOffset + count;

            if (i <= this.bufferSize)
            {
                Buffer.BlockCopy(buffer, offset, this.currentBuffer.Value.Array, this.currentBufferOffset, count);
                this.currentBuffer.ContentLength += count;
                this.currentBufferOffset += count;
                this.position += count;
                return;
            }

            var bytesLeft = count;

            while (bytesLeft > 0)
            {
                // check for buffer full
                if (this.bufferSize <= this.currentBufferOffset)
                {
                    // Create new buffer and Add to buffer
                    this.currentBuffer = this.bufferPoolManager.TakeBuffer();

                    this.writeBuffers.Add(this.currentBuffer);
                    this.currentBufferOffset = 0;
                }

                var bytesToCopy = (this.currentBufferOffset + bytesLeft) <= this.bufferSize
                    ? bytesLeft
                    : this.bufferSize - this.currentBufferOffset;

                Buffer.BlockCopy(buffer, offset, this.currentBuffer.Value.Array, this.currentBufferOffset, bytesToCopy);

                this.currentBuffer.ContentLength += bytesToCopy;

                this.position += bytesToCopy;
                offset += bytesToCopy;
                bytesLeft -= bytesToCopy;
                this.currentBufferOffset += bytesToCopy;
            }
        }

        public override void WriteByte(byte value)
        {
            var i = this.currentBufferOffset + 1;

            if (i > this.bufferSize)
            {
                this.currentBuffer = this.bufferPoolManager.TakeBuffer();
                this.writeBuffers.Add(this.currentBuffer);
                this.currentBufferOffset = 0;
            }

            this.currentBuffer.Value.Array[this.currentBufferOffset] = value;
            this.currentBuffer.ContentLength += 1;
            this.position += 1;
        }

        public IEnumerable<IPooledBuffer> GetBuffers()
        {
            if (!this.CanWrite)
            {
                throw new NotImplementedException();
            }

            return this.writeBuffers;
        }

        private void Initialize()
        {
            this.canWrite = true;
            this.canRead = false;
            this.canSeek = false;
            this.position = 0;
            this.writeBuffers = new List<IPooledBuffer>(1);
            this.currentBuffer = this.bufferPoolManager.TakeBuffer();
            this.writeBuffers.Add(this.currentBuffer);
            this.bufferSize = this.writeBuffers[0].Value.Count;
            this.currentBufferOffset = 0;
        }
    }
}
