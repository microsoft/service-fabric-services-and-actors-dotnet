// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2.Client
{
    /// <summary>
    /// Interface for handling the callback messages from the service.
    /// </summary>
    public interface IServiceRemotingCallbackMessageHandler
    {
        /// <summary>
        /// Handles the one-way message sent from the service.
        /// </summary>
        /// <param name="requestMessage">The one-way message.</param>
        void HandleOneWayMessage(IServiceRemotingRequestMessage requestMessage);
    }
}
