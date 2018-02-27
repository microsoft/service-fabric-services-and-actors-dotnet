// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
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
        /// <param name="requestMessage">The request message.</param>
        /// <returns>The response body.</returns>
        Task<IServiceRemotingResponseMessage> HandleRequestResponseAsync(
            IServiceRemotingRequestContext requestContext,
            IServiceRemotingRequestMessage requestMessage);

        /// <summary>
        /// Handles a one way message from the client.
        /// </summary>
        /// <param name="requestMessage">The request message</param>
        void HandleOneWayMessage(IServiceRemotingRequestMessage requestMessage);

        /// <summary>
        /// Returns the IServiceRemotingMessageBodyFactory . It is used by Dispatcher to create Remoting Response Body
        /// </summary>
        /// <returns></returns>
        IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory();
    }
}
