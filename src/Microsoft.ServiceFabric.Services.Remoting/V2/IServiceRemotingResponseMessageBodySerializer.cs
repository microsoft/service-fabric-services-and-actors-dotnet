// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    /// <summary>
    /// Defines an interface that must be implemented to provide a serializer for Remoting Response Body
    /// </summary>
    public interface IServiceRemotingResponseMessageBodySerializer
    {
        /// <summary>
        /// Serialize the Remoting Response Body before sending message over the wire.
        /// </summary>
        /// <param name="serviceRemotingRequestMessageBody"></param>
        /// <returns>OutgoingMessageBody</returns>
        OutgoingMessageBody Serialize(IServiceRemotingResponseMessageBody serviceRemotingRequestMessageBody);

        /// <summary>
        /// Deserialize the incoming Message to a Remoting ResponseMessageBody before sending it to Client Api
        /// </summary>
        /// <param name="messageBody">serialized Message</param>
        /// <returns>IServiceRemotingResponseMessageBody</returns>
        IServiceRemotingResponseMessageBody Deserialize(IncomingMessageBody messageBody);
    }
}
