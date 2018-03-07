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
        /// Serializes IServiceRemotingRequestMessageBody to IMessageBody 
        /// </summary>
        /// <param name="serviceRemotingRequestMessageBody"></param>
        /// <returns>IMessageBody</returns>
        IMessageBody Serialize(IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody);

        /// <summary>
        /// Deserializes IMessageBody to IServiceRemotingRequestMessageBody
        /// </summary>
        /// <param name="messageBody"></param>
        /// <returns>IServiceRemotingRequestMessageBody</returns>
        IServiceRemotingRequestMessageBody Deserialize(IMessageBody messageBody);
    }
}
