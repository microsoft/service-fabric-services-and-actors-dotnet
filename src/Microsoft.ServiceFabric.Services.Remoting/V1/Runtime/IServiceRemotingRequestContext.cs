// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Runtime
{
    /// <summary>
    /// Defines the interface that must be implemented to provide the request context for the IServiceRemotingMessageHandler.
    /// </summary>
    public interface IServiceRemotingRequestContext
    {
        /// <summary>
        /// Retrieves the client channel interface to use in cases where service wants to initiate calls to the client.
        /// </summary>
        /// <returns>The remoting callback client.</returns>
        IServiceRemotingCallbackClient GetCallbackClient();
    }
}
