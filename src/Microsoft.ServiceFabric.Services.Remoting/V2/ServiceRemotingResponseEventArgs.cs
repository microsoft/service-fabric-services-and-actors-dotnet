// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;

    /// <summary>
    /// Event arguments exposing response
    /// </summary>
    internal class ServiceRemotingResponseEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRemotingResponseEventArgs"/> class.
        /// </summary>
        /// <param name="response">The response object.</param>
        /// <param name="request">The corresponding request object.</param>
        public ServiceRemotingResponseEventArgs(IServiceRemotingResponseMessage response, IServiceRemotingRequestMessage request)
        {
            this.Response = response;
            this.Request = request;
        }

        /// <summary>
        /// Gets the actual response object the the EventArgs provide.
        /// </summary>
        public IServiceRemotingResponseMessage Response { get; private set; }

        /// <summary>
        /// Gets the request against which the response is generated
        /// </summary>
        public IServiceRemotingRequestMessage Request { get; private set; }
    }
}
