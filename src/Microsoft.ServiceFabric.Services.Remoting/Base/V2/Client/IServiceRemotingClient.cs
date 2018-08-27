// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2.Client
{
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// Defines the interface for service remoting client.
    /// </summary>
    public interface IServiceRemotingClient : ICommunicationClient
    {
        /// <summary>
        /// Send a remoting request to the service and gets a response back.
        /// </summary>
        /// <param name="requestRequestMessage">The request message.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation for remote method call.
        /// The result of the task contains the response for the request.</returns>
        Task<IServiceRemotingResponseMessage> RequestResponseAsync(
            IServiceRemotingRequestMessage requestRequestMessage);

        /// <summary>
        /// Sends a one-way message to the service.
        /// </summary>
        /// <param name="requestMessage">The one-way message.</param>
        void SendOneWay(IServiceRemotingRequestMessage requestMessage);
    }
}
