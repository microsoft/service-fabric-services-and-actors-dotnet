// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Specifies the headers that are sent along with a ServiceRemoting message.
    /// </summary>
    public interface IServiceRemotingRequestMessageHeader
    {
        /// <summary>
        /// Gets or sets the request id.
        /// </summary>
        Guid RequestId { get; set; }

        /// <summary>
        /// Gets or sets the methodId of the remote method.
        /// </summary>
        /// <value>The method id.</value>
        int MethodId { get; set; }

        /// <summary>
        /// Gets or sets the interface id of the remote interface.
        /// </summary>
        /// <value>The interface id.</value>
        int InterfaceId { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the remote method invocation.
        /// </summary>
        string InvocationId { get; set; }

        /// <summary>
        /// Gets or sets the Method Name  of the remoting method.
        /// </summary>
        string MethodName { get; set; }

        /// <summary>
        /// Adds a new header with the specified name and value.
        /// </summary>
        /// <param name="headerName">The header Name.</param>
        /// <param name="headerValue">The header value.</param>
        void AddHeader(string headerName, byte[] headerValue);

        /// <summary>
        /// Gets the header with the specified name.
        /// </summary>
        /// <param name="headerName">The header Name.</param>
        /// <param name="headerValue">The header value.</param>
        /// <returns>true if a header with that name exists; otherwise, false.</returns>
        bool TryGetHeaderValue(string headerName, out byte[] headerValue);
    }
}
