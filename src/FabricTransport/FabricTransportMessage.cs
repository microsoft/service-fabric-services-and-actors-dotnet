// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric.Interop;
using System.IO;
using static Microsoft.ServiceFabric.FabricTransport.NativeFabricTransport;

namespace Microsoft.ServiceFabric.FabricTransport
{
    internal class FabricTransportMessage : IDisposable
    {
        private readonly FabricTransportRequestHeader requestHeader;
        private readonly FabricTransportRequestBody requestBody;
        private readonly IFabricTransportMessage nativeInterfaceRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportMessage"/> class.
        /// </summary>
        public FabricTransportMessage(FabricTransportRequestHeader requestHeader, FabricTransportRequestBody requestBody)
        {
            this.requestHeader = requestHeader;
            this.requestBody = requestBody;
            this.nativeInterfaceRoot = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportMessage"/> class backed by a native message.
        /// </summary>
        // Disposing this message releases the COM object of nativeInterfaceRoot.
        public FabricTransportMessage(FabricTransportRequestHeader requestHeader,
            FabricTransportRequestBody requestBody,
            IFabricTransportMessage nativeInterfaceRoot)
        {
            this.requestHeader = requestHeader;
            this.requestBody = requestBody;
            this.nativeInterfaceRoot = nativeInterfaceRoot;
        }

        /// <summary>
        /// Returns the <see cref="FabricTransportRequestBody"/>, or <see langword="null"/> if the message has no body.
        /// </summary>
        public FabricTransportRequestBody GetBody()
        {
            return this.requestBody;
        }

        /// <summary>
        /// Returns the <see cref="FabricTransportRequestHeader"/>, or <see langword="null"/> if the message has no header.
        /// </summary>
        public FabricTransportRequestHeader GetHeader()
        {
            return this.requestHeader;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.nativeInterfaceRoot != null)
            {
                //To Make it work for Managed CIT's
                this.nativeInterfaceRoot.SafeReleaseComObject();
            }

            if (this.requestBody != null)
            {
                this.requestBody.Dispose();
            }
            if (this.requestHeader != null)
            {
                this.requestHeader.Dispose();
            }
        }
    }

    internal class FabricTransportRequestHeader
    {
        private readonly Stream recievedHeaderStream;
        private readonly ArraySegment<byte> requestHeaderBuffer;
        private readonly Action disposeAction;

        /// <summary>
        /// Returns the serialized header bytes to send.
        /// </summary>
        public ArraySegment<byte> GetSendBuffer()
        {
            return this.requestHeaderBuffer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportRequestHeader"/> class for an outgoing header.
        /// </summary>
        /// <param name="requestHeaderBuffer">The serialized header bytes to send.</param>
        /// <param name="disposeAction">A callback that releases <paramref name="requestHeaderBuffer"/> when the header is disposed.</param>
        public FabricTransportRequestHeader(ArraySegment<byte> requestHeaderBuffer, Action disposeAction)
        {
            this.requestHeaderBuffer = requestHeaderBuffer;
            this.disposeAction = disposeAction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportRequestHeader"/> class for a received header.
        /// </summary>
        public FabricTransportRequestHeader(Stream recievedHeaderStream)
        {
            this.recievedHeaderStream = recievedHeaderStream;
        }

        /// <summary>
        /// Returns the <see cref="Stream"/> containing the received header bytes.
        /// </summary>
        public Stream GetRecievedStream()
        {
            return this.recievedHeaderStream;
        }

        /// <summary>
        /// Releases the resources held by the outgoing header buffer.
        /// </summary>
        public void Dispose()
        {
            if (this.disposeAction != null)
            {
                this.disposeAction();
            }
        }
    }

    internal class FabricTransportRequestBody
    {
        private readonly IEnumerable<ArraySegment<byte>> sendBuffers;
        private readonly Action disposeAction;
        private readonly Stream recievedStream;

        /// <summary>
        /// Returns the serialized body buffers to send.
        /// </summary>
        public IEnumerable<ArraySegment<byte>> GetBodyBuffers()
        {
            return this.sendBuffers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportRequestBody"/> class for an outgoing body.
        /// </summary>
        /// <param name="sendBuffers">The serialized body buffers to send.</param>
        /// <param name="disposeAction">A callback that releases <paramref name="sendBuffers"/> when the body is disposed.</param>
        public FabricTransportRequestBody(IEnumerable<ArraySegment<byte>> sendBuffers, Action disposeAction)
        {
            this.sendBuffers = sendBuffers;
            this.disposeAction = disposeAction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportRequestBody"/> class for a received body.
        /// </summary>
        public FabricTransportRequestBody(Stream recievedStream)
        {
            this.recievedStream = recievedStream;
        }

        /// <summary>
        /// Returns the <see cref="Stream"/> containing the received body bytes.
        /// </summary>
        public Stream GetRecievedStream()
        {
            return this.recievedStream;
        }

        /// <summary>
        /// Releases the resources held by the outgoing body buffers.
        /// </summary>
        public void Dispose()
        {
            if (this.disposeAction != null)
            {
                this.disposeAction();
            }
        }
    }
}
