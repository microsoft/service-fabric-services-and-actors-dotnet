// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    /// <summary>
    /// Defines the interface that must be implemented for providing callback mechanism from the remoting listener to the client.
    /// </summary>
    public interface IServiceRemotingCallbackClient
    {
        /// <summary>
        /// Sends a one way message to the client.
        /// </summary>
        /// <param name="requestMessage">The remoting request message.</param>
        void SendOneWay(IServiceRemotingRequestMessage requestMessage);

        /// <summary>
        /// Gets a factory for creating the remoting request message body to send remoting messages from listener to the client.
        /// </summary>
        /// <returns>A factory for creating the remoting message bodies</returns>
        IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory();
    }
}
