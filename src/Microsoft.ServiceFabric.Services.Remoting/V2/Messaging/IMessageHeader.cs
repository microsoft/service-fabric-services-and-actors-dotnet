// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Messaging
{
    using System;
    using System.IO;

    /// <summary>
    /// Defines an interface that must be implemented to provide message header for the serialized Message.
    /// </summary>
    internal interface IMessageHeader : IDisposable
    {
        /// <summary>
        /// Returns the Buffer to be sent over the wire.
        /// </summary>
        /// <returns>ArraySegment</returns>
        ArraySegment<byte> GetSendBuffer();

        /// <summary>
        /// Gets the Recieved Stream .
        /// </summary>
        /// <returns>Stream </returns>
        Stream GetReceivedBuffer();
    }
}
