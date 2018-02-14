// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    /// <summary>
    /// Defines the interface that must be implemented for create Remoting Request Message.
    /// </summary>
    public interface IServiceRemotingRequestMessage
    {
        /// <summary>
        /// Gets the Remoting Request Message Header
        /// </summary>
        /// <returns>IServiceRemotingRequestMessageHeader</returns>
        IServiceRemotingRequestMessageHeader GetHeader();


        /// <summary>
        /// Gets the Remoting Request Message Body </summary>
        /// <returns>IServiceRemotingRequestMessageBody</returns>
        IServiceRemotingRequestMessageBody GetBody();
    }
}
