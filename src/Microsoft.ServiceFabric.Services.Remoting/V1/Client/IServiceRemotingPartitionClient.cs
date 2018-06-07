// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Client
{
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// Defines the interface for the client that communicate over remoting to a particular replica of a service partition.
    /// </summary>
    public interface IServiceRemotingPartitionClient : IServicePartitionClient<IServiceRemotingClient>
    {
    }
}
