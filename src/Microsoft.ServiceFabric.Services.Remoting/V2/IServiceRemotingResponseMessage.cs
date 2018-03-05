// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    /// <summary>
    /// Defines an interface that must be implemented to provide  a remoting response message for remoting Api.
    /// </summary>
    public interface IServiceRemotingResponseMessage
    {
        /// <summary>
        /// Gets the header of the response message.
        /// </summary>
        /// <returns>The header of this response message.</returns>
        IServiceRemotingResponseMessageHeader GetHeader();

        /// <summary>
        /// Gets the body of the response message.
        /// </summary>
        /// <returns>The body of this response message.</returns>
        IServiceRemotingResponseMessageBody GetBody();
    }
}
