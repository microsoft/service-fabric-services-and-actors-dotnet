// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2
{
    using System;

    /// <summary>
    /// Event arguments for failed requests.
    /// </summary>
    public class ServiceRemotingFailedResponseEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRemotingFailedResponseEventArgs"/> class.
        /// </summary>
        /// <param name="ex">The exception resulting in failure of the service remoting call.</param>
        /// <param name="request">The request against which the response is generated.</param>
        public ServiceRemotingFailedResponseEventArgs(Exception ex, IServiceRemotingRequestMessage request)
        {
            this.Error = ex;
            this.Request = request;
        }

        /// <summary>
        /// Gets the exception resulting in failure of the service remoting call.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets the request against which the response is generated
        /// </summary>
        public IServiceRemotingRequestMessage Request { get; private set; }
    }
}
