// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    /// <summary>
    /// Defines the interface that must be implemented to provide a serializer/deserializer for remoting request message body.
    /// </summary>
    public interface IServiceRemotingRequestMessageBodySerializer
    {
        /// <summary>
        /// Serialize the remoting request body object to a message body that can be sent over the wire.
        /// </summary>
        /// <param name="serviceRemotingRequestMessageBody">Remoting request message body object.</param>
        /// <returns>Serialized message body.</returns>
        OutgoingMessageBody Serialize(IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody);

        /// <summary>
        /// Deserializes an incoming message body to remoting request body object.
        /// </summary>
        /// <param name="messageBody">Serialized message body.</param>
        /// <returns>Deserialized remoting request message body object.</returns>
        IServiceRemotingRequestMessageBody Deserialize(IncomingMessageBody messageBody);
    }
}
