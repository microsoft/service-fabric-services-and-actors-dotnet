// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime
{
    /// <summary>
    /// Defines a transport listener that accepts incoming remoting connections from clients and routes their messages
    /// to an <see cref="IFabricTransportMessageHandler"/>.
    /// </summary>
    interface IFabricTransportListener : IDisposable
    {
        /// <summary>
        /// Asynchronously opens the listener to start accepting client connections and returns the address on which it
        /// listens for requests.
        /// </summary>
        Task<string> OpenAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously closes the listener, stopping it from accepting new connections and gracefully closing open
        /// connections.
        /// </summary>
        Task CloseAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Aborts the listener immediately without gracefully closing open connections, unlike <see cref="CloseAsync"/>.
        /// </summary>
        void Abort();
    }
}
