// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime
{
    /// <summary>
    /// Processes messages received from the native fabric transport.
    /// </summary>
    interface IFabricTransportMessageHandler : IDisposable
    {
        /// <summary>
        /// Asynchronously processes the request in <paramref name="fabricTransportMessage"/> received from the client
        /// identified by <paramref name="requestContext"/> and returns the reply to send back to the client.
        /// </summary>
        Task<FabricTransportMessage> RequestResponseAsync(FabricTransportRequestContext requestContext,
            FabricTransportMessage fabricTransportMessage);

        /// <summary>
        /// Processes the received one-way message in the context described by <paramref name="requestContext"/>.
        /// No reply is sent back to the client.
        /// </summary>
        void HandleOneWay(FabricTransportRequestContext requestContext, FabricTransportMessage requesTransportMessage);
    }
}
