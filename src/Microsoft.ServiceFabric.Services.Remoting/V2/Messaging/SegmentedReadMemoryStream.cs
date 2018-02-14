// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class SegmentedReadMemoryStream : Stream
    {
        private readonly IEnumerable<ArraySegment<byte>> readbuffers;

        private int bufferNum;

        private int bufferOffset;
        private long length;
        private bool canRead;
        private bool canSeek;
        private bool canWrite;
        private long position;

        public SegmentedReadMemoryStream(IEnumerable<ArraySegment<byte>> readbuffers)
        {
            this.readbuffers = readbuffers;
            this.length = 0;
            this.Initialize();
            this.SetLength();
        }

        public SegmentedReadMemoryStream(ArraySegment<byte> readbuffer)
        {
            var tempBuffers = new List<ArraySegment<byte>>
            {
                readbuffer
            };
            this.length = 0;
            this.readbuffers = tempBuffers;
            this.Initialize();
            this.SetLength();
        }

        private void Initialize()
        {
            this.canWrite = false;
            this.canSeek = false;
            this.canRead = true;
            this.position = 0;
            this.bufferNum = 0;
            this.bufferOffset = 0;
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
            get { return this.length; }
        }

        public override long Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Initialize();
                    return this.Position;

            }
            throw new NotImplementedException();

        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
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

            if (this.Position >= this.Length || count == 0)
            {
                return 0;
            }

            var bytesToRead = Math.Min(count, (int)(this.Length - this.Position));
            var bytesRead = 0;
            var bytesLeft = bytesToRead - bytesRead;

            while (bytesLeft > 0)
            {
                var buf = this.readbuffers.ElementAt((this.bufferNum));
                var bufferSize = buf.Count;
                var bytesToCopy = (this.bufferOffset + bytesLeft) < bufferSize
                    ? bytesLeft
                    : bufferSize - this.bufferOffset;

                Buffer.BlockCopy(buf.Array, buf.Offset + this.bufferOffset, buffer, offset, bytesToCopy);
                this.Position += bytesToCopy;
                offset += bytesToCopy;
                bytesLeft -= bytesToCopy;
                bytesRead += bytesToCopy;
                this.bufferOffset += bytesToCopy;
                if (this.bufferOffset == bufferSize)
                {
                    this.bufferNum++;
                    this.bufferOffset = 0;
                }
            }

            return bytesToRead;
        }

        public override int ReadByte()
        {
            //Number of bytes to read is more than number of unread bytes in buffer

            if (this.length - this.position < 1)
            {
                return -1;
            }
            var currentBuffer = this.readbuffers.ElementAt((this.bufferNum));

            //Read from next buffer
            if (this.bufferOffset == currentBuffer.Count)
            {
                this.bufferNum++;
                this.bufferOffset = 0;
                currentBuffer = this.readbuffers.ElementAt((this.bufferNum));
            }

            var byteread = currentBuffer.Array[this.bufferOffset];
            this.Position++;
            this.bufferOffset++;

            return byteread;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        private void SetLength()
        {
            foreach (var segment in this.readbuffers)
            {
                this.length += segment.Count;
            }
        }
    }
}
