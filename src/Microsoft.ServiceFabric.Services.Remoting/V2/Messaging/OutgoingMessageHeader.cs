// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal sealed class OutgoingMessageHeader : IMessageHeader
    {
        private readonly IPooledBuffer pooledBuffer;
        private readonly ArraySegment<byte> outgoingBuffer;
        private bool isDisposed;

        public OutgoingMessageHeader(IEnumerable<IPooledBuffer> pooledBuffers)
        {
            if (pooledBuffers.Count() > 1)
            {
                ServiceTrace.Source.WriteInfo("OutgoingMessageHeader", "PooledBuffer for header is more than one, which might affect the performance");
                this.outgoingBuffer = this.CreateAndReleaseBuffer(pooledBuffers);
                this.pooledBuffer = null;
            }
            else
            {
                var pooledBuffer = pooledBuffers.ElementAt(0);
                this.outgoingBuffer = new ArraySegment<byte>(
                    pooledBuffer.Value.Array,
                    pooledBuffer.Value.Offset,
                    pooledBuffer.ContentLength);

                this.pooledBuffer = pooledBuffer;
            }
        }

        public OutgoingMessageHeader(ArraySegment<byte> buffer)
        {
            this.outgoingBuffer = buffer;
            this.pooledBuffer = null;
        }

        public Stream GetReceivedBuffer()
        {
            throw new NotImplementedException("This method is not valid on outgoing messages");
        }

        public ArraySegment<byte> GetSendBuffer()
        {
            return this.outgoingBuffer;
        }

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;
                if (this.pooledBuffer != null)
                {
                    this.pooledBuffer.Release();
                }
            }
        }

        private ArraySegment<byte> CreateAndReleaseBuffer(IEnumerable<IPooledBuffer> pooledBuffers)
        {
            ServiceTrace.Source.WriteWarning("OutgoingMessageHeaders", "Header has more than 1 Pooled Buffer");
            var length = 0;
            foreach (var pooledBuffer in pooledBuffers)
            {
                length += pooledBuffer.ContentLength;
            }

            var sourceArr = new byte[length];
            var writtenBytes = 0;
            foreach (var pooledBuffer in pooledBuffers)
            {
                Array.Copy(pooledBuffer.Value.Array, 0, sourceArr, writtenBytes, pooledBuffer.ContentLength);
                writtenBytes += pooledBuffer.ContentLength;
                pooledBuffer.Release();
            }

            return new ArraySegment<byte>(sourceArr);
        }
    }
}
