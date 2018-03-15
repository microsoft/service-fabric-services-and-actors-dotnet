// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
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
        /// Serializes IServiceRemotingRequestMessageBody to IOutgoingMessageBody
        /// </summary>
        /// <param name="serviceRemotingRequestMessageBody"></param>
        /// <returns>IOutgoingMessageBody</returns>
        IOutgoingMessageBody Serialize(IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody);

        /// <summary>
        /// Deserializes IIncomingMessageBody to IServiceRemotingRequestMessageBody
        /// </summary>
        /// <param name="messageBody"></param>
        /// <returns>IServiceRemotingRequestMessageBody</returns>
        IServiceRemotingRequestMessageBody Deserialize(IIncomingMessageBody messageBody);
    }
}
