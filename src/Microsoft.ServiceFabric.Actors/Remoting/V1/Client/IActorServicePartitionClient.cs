// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1.Client
{
    using System;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Client;

    /// <summary>
    /// Defines the interface for the client that communicate with an actor within a service partition.
    /// </summary>
    [Obsolete(Services.Remoting.DeprecationMessage.RemotingV1)]
    public interface IActorServicePartitionClient : IServiceRemotingPartitionClient
    {
        /// <summary>
        /// Gets the id of the actor this client communicates with.
        /// </summary>
        ActorId ActorId { get; }
    }
}
