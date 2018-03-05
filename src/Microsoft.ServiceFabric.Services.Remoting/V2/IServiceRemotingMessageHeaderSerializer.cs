// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    /// <summary>
    /// Represents a serializer that can serialize remoting layer message header to messaging layer header.
    /// </summary>
    internal interface IServiceRemotingMessageHeaderSerializer
    {
        /// <summary>
        ///  Serializes the remoting request message header to a message header.
        /// </summary>
        /// <param name="serviceRemotingRequestMessageHeader">Remoting header to serialize.</param>
        /// <returns>An <see cref="IMessageHeader"/> that has the serialized contents of the specified service remoting header.</returns>
        IMessageHeader SerializeRequestHeader(IServiceRemotingRequestMessageHeader serviceRemotingRequestMessageHeader);

        /// <summary>
        /// Deserializes a request message header in to remoting header.
        /// </summary>
        /// <param name="messageHeader">Messaging layer header to be deserialized.</param>
        /// <returns>An <see cref="IServiceRemotingRequestMessageHeader"/> that has the deserialized contents of the specified message header.</returns>
        IServiceRemotingRequestMessageHeader DeserializeRequestHeaders(IMessageHeader messageHeader);

        /// <summary>
        ///  Serializes the remoting response message header to a message header.
        /// </summary>
        /// <param name="serviceRemotingResponseMessageHeader">Remoting header to serialize.</param>
        /// <returns>An <see cref="IMessageHeader"/> that has the serialized contents of the specified service remoting header.</returns>
        IMessageHeader SerializeResponseHeader(IServiceRemotingResponseMessageHeader serviceRemotingResponseMessageHeader);

        /// <summary>
        /// Deserializes a response message header in to remoting header.
        /// </summary>
        /// <param name="messageHeader">Messaging layer header to be deserialized.</param>
        /// <returns>An <see cref="IServiceRemotingRequestMessageHeader"/> that has the deserialized contents of the specified message header.</returns>
        IServiceRemotingResponseMessageHeader DeserializeResponseHeaders(IMessageHeader messageHeader);
    }
}
