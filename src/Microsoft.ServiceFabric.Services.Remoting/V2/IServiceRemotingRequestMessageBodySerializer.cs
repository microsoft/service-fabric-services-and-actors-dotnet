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
        /// Serializes IServiceRemotingRequestMessageBody to OutgoingMessageBody 
        /// </summary>
        /// <param name="serviceRemotingRequestMessageBody"></param>
        /// <returns></returns>
        OutgoingMessageBody Serialize(IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody);

        /// <summary>
        /// Deserializes IncomingMessageBody to IServiceRemotingRequestMessageBody
        /// </summary>
        /// <param name="messageBody"></param>
        /// <returns></returns>
        IServiceRemotingRequestMessageBody Deserialize(IncomingMessageBody messageBody);
    }
}