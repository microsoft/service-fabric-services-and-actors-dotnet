// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Client
{
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting;

    /// <summary>
    /// Defines the interface that must be implemented to provide a client for Service Remoting communication.
    /// </summary>
    public interface IServiceRemotingClient : ICommunicationClient
    {
        /// <summary>
        /// Sends a message to the service and gets a response back.
        /// </summary>
        /// <param name="messageHeaders">Message headers</param>
        /// <param name="requestBody">Message body</param>
        /// <returns>Response body</returns>
        Task<byte[]> RequestResponseAsync(
            ServiceRemotingMessageHeaders messageHeaders,
            byte[] requestBody);

        /// <summary>
        /// Sends a one-way message to the service
        /// </summary>
        /// <param name="messageHeaders">Message headers</param>
        /// <param name="requestBody">Message body</param>
        void SendOneWay(
            ServiceRemotingMessageHeaders messageHeaders,
            byte[] requestBody);
    }
}