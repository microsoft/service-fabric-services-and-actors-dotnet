// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    /// <summary>
    /// Interface definition for forwardding actor requests if the current service cannot service the request.
    /// </summary>
    internal interface IRequestForwarder
    {
        /// <summary>
        /// Forwards the actor and actorservice proxy request to the remote service.
        /// </summary>
        /// <param name="request">Request Message.</param>
        /// <param name="token">Cancellation token to signal cancellation on async operation.</param>
        /// <returns>Task to represent the status of the async operation.</returns>
        public Task<IServiceRemotingResponseMessage> ForwardActorRequestAsync(
            IServiceRemotingRequestMessage request,
            CancellationToken token);
    }
}
