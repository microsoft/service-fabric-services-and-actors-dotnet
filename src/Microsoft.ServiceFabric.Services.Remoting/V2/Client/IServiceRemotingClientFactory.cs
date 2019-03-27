// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Client
{
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// A factory for creating <see cref="IServiceRemotingClient">service remoting clients.</see>.
    /// </summary>
    public interface IServiceRemotingClientFactory : ICommunicationClientFactory<IServiceRemotingClient>
    {
        /// <summary>
        /// Gets a factory for creating the remoting message bodies.
        /// </summary>
        /// <returns>A factory for creating the remoting message bodies.</returns>
        IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory();
    }
}
