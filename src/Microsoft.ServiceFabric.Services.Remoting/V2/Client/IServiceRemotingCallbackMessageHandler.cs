// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Client
{
    /// <summary>
    ///
    /// </summary>
    public interface IServiceRemotingCallbackMessageHandler
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="requestMessage"></param>
        void HandleOneWayMessage(IServiceRemotingRequestMessage requestMessage);
    }
}
