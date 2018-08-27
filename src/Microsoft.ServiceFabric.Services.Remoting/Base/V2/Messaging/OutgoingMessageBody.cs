// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents the outgoing message body to be sent over the wire.
    /// </summary>
    public sealed class OutgoingMessageBody : IOutgoingMessageBody
    {
        private readonly IEnumerable<ArraySegment<byte>> bodyBuffers;
        private readonly IEnumerable<IPooledBuffer> pooledBodyBuffers;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingMessageBody"/> class.
        /// Creates OutgoingMessageBody with list of pooled Buffers
        /// </summary>
        /// <param name="outgoingPooledBodyBuffers"> List of Pooled Buffers</param>
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
        /// Initializes a new instance of the <see cref="OutgoingMessageBody"/> class.
        /// Creates OutgoingMessageBody with list of segments.
        /// </summary>
        /// <param name="outgoingBodyBuffers">List of ArraySegment </param>
        public OutgoingMessageBody(IEnumerable<ArraySegment<byte>> outgoingBodyBuffers)
        {
            this.pooledBodyBuffers = null;
            this.bodyBuffers = outgoingBodyBuffers;
        }

        /// <summary>
        /// Returns the Buffers to be sent over the wire.
        /// </summary>
        /// <returns>List of ArraySegment</returns>
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
