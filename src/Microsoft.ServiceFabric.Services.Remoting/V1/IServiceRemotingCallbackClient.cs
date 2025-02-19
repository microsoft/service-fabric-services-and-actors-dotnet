// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the interface that must be implemented for providing callback mechanism from the remoting listener to the client.
    /// </summary>
    [Obsolete("This class is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
    public interface IServiceRemotingCallbackClient
    {
        /// <summary>
        /// Sends a message to the client and gets the response.
        /// </summary>
        /// <param name="messageHeaders">The message headers.</param>
        /// <param name="requestBody">The message body.</param>
        /// <returns>Response body</returns>
        Task<byte[]> RequestResponseAsync(
            ServiceRemotingMessageHeaders messageHeaders,
            byte[] requestBody);

        /// <summary>
        /// Sends a one way message to the client.
        /// </summary>
        /// <param name="messageHeaders">The message headers.</param>
        /// <param name="requestBody">The message body.</param>
        void OneWayMessage(
            ServiceRemotingMessageHeaders messageHeaders,
            byte[] requestBody);
    }
}
