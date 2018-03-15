// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Messaging
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an interface that must be implemented to provide message body for the serialized Message.
    /// </summary>
    public interface IOutgoingMessageBody : IDisposable
    {
        /// <summary>
        /// Gets the Send Buffers 
        /// </summary>
        /// <returns></returns>
        IEnumerable<ArraySegment<byte>> GetSendBuffers();
    }
}
