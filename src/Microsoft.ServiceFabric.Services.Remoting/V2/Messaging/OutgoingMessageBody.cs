// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents the outgoing message body to be sent over the wire.
    /// </summary>
    public sealed class OutgoingMessageBody : IDisposable
    {
        private readonly IEnumerable<ArraySegment<byte>> bodyBuffers;
        private readonly IEnumerable<IPooledBuffer> pooledBodyBuffers;
        private bool isDisposed;

        /// <summary>
        /// Creates OutgoingMessageBody with list of pooled Buffers
        /// </summary>
        /// <param name="outgoingPooledBodyBuffers"></param>
        public OutgoingMessageBody(IEnumerable<IPooledBuffer> outgoingPooledBodyBuffers)
        {
            this.pooledBodyBuffers = outgoingPooledBodyBuffers;
            this.bodyBuffers = this.pooledBodyBuffers.Select(
                pooledBuffer => new ArraySegment<byte>(
                    pooledBuffer.Value.Array,
                    pooledBuffer.Value.Offset,
                    pooledBuffer.ContentLength));
        }

        /// <summary>
        /// Creates OutgoingMessageBody with list of segments.
        /// </summary>
        public OutgoingMessageBody(IEnumerable<ArraySegment<byte>> outgoingBodyBuffers)
        {
            this.pooledBodyBuffers = null;
            this.bodyBuffers = outgoingBodyBuffers;
        }

        /// <summary>
        /// Returns the Buffers to be sent over the wire.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ArraySegment<byte>> GetSendBuffers()
        {
            return this.bodyBuffers;
        }

        /// <summary>
        /// Release the pooled Buffers if it has any.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;
                if (this.pooledBodyBuffers != null)
                {
                    foreach (var pooledBuffer in this.pooledBodyBuffers)
                    {
                        pooledBuffer.Release();
                    }
                }
            }
        }
    }
}
