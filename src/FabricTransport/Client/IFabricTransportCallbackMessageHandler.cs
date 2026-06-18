// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.FabricTransport.Client
{
    /// <summary>
    /// Handles one-way <see cref="FabricTransportMessage"/> callbacks pushed from a service to its client.
    /// </summary>
    internal interface IFabricTransportCallbackMessageHandler
    {
        /// <summary>
        /// Handles the one-way <paramref name="message"/> received from the service.
        /// </summary>
        void OneWayMessage(FabricTransportMessage message);
    }
}
