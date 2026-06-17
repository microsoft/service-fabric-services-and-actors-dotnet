// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime
{
    /// <summary>
    /// Represents the per-request, service-side context that exposes the requesting client's identity and its callback
    /// channel.
    /// </summary>
    internal class FabricTransportRequestContext
    {
        private readonly string clientId;
        private readonly Func<string, FabricTransportCallbackClient> callback;
        private FabricTransportCallbackClient callbackClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportRequestContext"/> class.
        /// </summary>
        /// <param name="clientId">The identifier of the client that sent the request.</param>
        /// <param name="getCallBack">A factory invoked with <paramref name="clientId"/> to resolve the client's <see cref="FabricTransportCallbackClient"/>.</param>
        public FabricTransportRequestContext(string clientId, Func<string, FabricTransportCallbackClient> getCallBack)
        {
            this.clientId = clientId;
            this.callback = getCallBack;
        }

        /// <summary>
        /// Gets the identifier of the client that sent the request.
        /// </summary>
        public string ClientId
        {
            get { return this.clientId; }
        }

        /// <summary>
        /// Returns the <see cref="FabricTransportCallbackClient"/> used to send messages back to the client that sent the request,
        /// or <see langword="null"/> if no callback channel is registered for the client.
        /// </summary>
        public FabricTransportCallbackClient GetCallbackClient()

        {
            if (this.callbackClient == null)
            {
                this.callbackClient = this.callback(this.clientId);
            }

            return this.callbackClient;
        }
    }
}
