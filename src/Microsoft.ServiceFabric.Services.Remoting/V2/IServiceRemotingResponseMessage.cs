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
        /// Gets the Response Message Header.
        /// </summary>
        /// <returns></returns>
        IServiceRemotingResponseMessageHeader GetHeader();

        /// <summary>
        /// Gets the Response Message Body.
        /// </summary>
        /// <returns></returns>
        IServiceRemotingResponseMessageBody GetBody();
    }
}
