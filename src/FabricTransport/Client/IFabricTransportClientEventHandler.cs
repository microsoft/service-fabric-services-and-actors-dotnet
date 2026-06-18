// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.FabricTransport.Client
{
    /// <summary>
    /// Receives notifications about the connection state of a <see cref="FabricTransportClient"/>.
    /// </summary>
    internal interface IFabricTransportClientEventHandler
    {
        /// <summary>
        /// Notifies the handler that the client has established a connection to the service endpoint.
        /// </summary>
        void OnConnected();

        /// <summary>
        /// Notifies the handler that the client has lost its connection to the service endpoint.
        /// </summary>
        void OnDisconnected();
    }
}
