// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime
{
    /// <summary>
    /// Tracks the per-client callback channels that let a service push one-way messages back to its connected clients.
    /// </summary>
    internal interface IFabricTransportConnectionHandler
    {
        /// <summary>
        /// Asynchronously registers the <paramref name="fabricTransportServiceRemotingCallback"/> channel for a newly
        /// connected client so the service can later push one-way messages back to it.
        /// </summary>
        Task ConnectAsync(FabricTransportCallbackClient fabricTransportServiceRemotingCallback, TimeSpan timeout);

        /// <summary>
        /// Asynchronously removes and disposes the callback channel registered for the client identified by
        /// <paramref name="clientId"/>.
        /// </summary>
        Task DisconnectAsync(string clientId, TimeSpan timeout);

        /// <summary>
        /// Returns the <see cref="FabricTransportCallbackClient"/> registered for the client identified by
        /// <paramref name="clientId"/>, or <see langword="null"/> if no such client is connected.
        /// </summary>
        FabricTransportCallbackClient GetCallBack(string clientId);
    }
}
