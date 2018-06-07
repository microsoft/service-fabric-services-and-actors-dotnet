// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;

    /// <summary>
    /// Event arguments exposing request
    /// </summary>
    internal class ServiceRemotingRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRemotingRequestEventArgs"/> class.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="methodName">The method that is being called.</param>
        public ServiceRemotingRequestEventArgs(IServiceRemotingRequestMessage request, string methodName)
        {
            this.Request = request;
            this.MethodName = methodName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRemotingRequestEventArgs"/> class.
        /// Creates a new object of type <see cref="ServiceRemotingRequestEventArgs"/>
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="targetUri">The uri of the target service.</param>
        /// <param name="methodName">The method that is being called.</param>
        public ServiceRemotingRequestEventArgs(IServiceRemotingRequestMessage request, Uri targetUri, string methodName)
        {
            this.Request = request;
            this.ServiceUri = targetUri;
            this.MethodName = methodName;
        }

        /// <summary>
        /// Gets the actual request object the the EventArgs provide.
        /// </summary>
        public IServiceRemotingRequestMessage Request { get; private set; }

        /// <summary>
        /// Gets the target service uri.
        /// </summary>
        public Uri ServiceUri { get; private set; }

        /// <summary>
        /// Gets target method being called.
        /// </summary>
        public string MethodName { get; private set; }
    }
}
