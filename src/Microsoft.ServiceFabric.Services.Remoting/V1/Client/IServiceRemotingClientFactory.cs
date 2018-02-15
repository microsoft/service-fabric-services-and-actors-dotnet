// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Client
{
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// Defines the interface that must be implemented for providing the remoting communication client factory.
    /// </summary>
    public interface IServiceRemotingClientFactory : ICommunicationClientFactory<IServiceRemotingClient>
    {
    }
}
