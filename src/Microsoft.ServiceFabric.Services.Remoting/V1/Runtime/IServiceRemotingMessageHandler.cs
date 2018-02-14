// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Runtime
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the interface that must be implemented by the ServiceRemotingListener to receive messages from the
    /// remoting transport.
    /// </summary>
    public interface IServiceRemotingMessageHandler
    {
        /// <summary>
        /// Handles a message from the client that requires a response from the service.
        /// </summary>
        /// <param name="requestContext">Contains additional information about the request.</param>
        /// <param name="messageHeaders">The request message headers.</param>
        /// <param name="requestBody">The request message body.</param>
        /// <returns>The response body.</returns>
        Task<byte[]> RequestResponseAsync(
            IServiceRemotingRequestContext requestContext,
            ServiceRemotingMessageHeaders messageHeaders,
            byte[] requestBody);

        /// <summary>
        /// Handles a one way message from the client.
        /// </summary>
        /// <param name="requestContext">Contains additional information about the request.</param>
        /// <param name="messageHeaders">The request message headers.</param>
        /// <param name="requestBody">The request message body.</param>
        void HandleOneWay(
            IServiceRemotingRequestContext requestContext,
            ServiceRemotingMessageHeaders messageHeaders,
            byte[] requestBody);
    }
}
