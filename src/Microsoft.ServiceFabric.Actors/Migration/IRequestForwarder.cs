// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    /// <summary>
    /// Interface definition for forwardding actor requests if the current service cannot service the request.
    /// </summary>
    public interface IRequestForwarder
    {
        /// <summary>
        /// Forwards the actor and actorservice proxy request to the remote service.
        /// </summary>
        /// <param name="requestContext">Contains additional information about the request.</param>
        /// <param name="requestMessage">The request message.</param>
        /// <returns>Task to represent the status of the async operation.</returns>
        public Task<IServiceRemotingResponseMessage> ForwardRequestResponseAsync(
            IServiceRemotingRequestContext requestContext,
            IServiceRemotingRequestMessage requestMessage);
    }
}
