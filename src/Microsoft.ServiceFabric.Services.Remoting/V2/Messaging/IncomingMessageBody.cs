// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Serialized Message Body received from an incoming connection.
    /// </summary>
    public sealed class IncomingMessageBody : IMessageBody
    {
        private readonly Stream receivedBufferStream;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncomingMessageBody"/> class.
        /// Creates an incoming Message Body with the received stream .
        /// </summary>
        /// <param name="receivedBufferStream">Recieved Stream </param>
        public IncomingMessageBody(Stream receivedBufferStream)
        {
            this.receivedBufferStream = receivedBufferStream;
        }

        /// <summary>
        /// This is not used for this implementation
        /// </summary>
        /// <returns>List of ArraySegment</returns>
        public IEnumerable<ArraySegment<byte>> GetSendBuffers()
        {
            throw new NotImplementedException("This method is not implemented for incoming messages");
        }

        /// <summary>
        /// Return the Received Buffer Stream
        /// </summary>
        /// <returns>Stream</returns>
        public Stream GetReceivedBuffer()
        {
            return this.receivedBufferStream;
        }

        /// <summary>
        /// Dispose the Received Buffer stream
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;
                this.receivedBufferStream.Dispose();
            }
        }
    }
}
