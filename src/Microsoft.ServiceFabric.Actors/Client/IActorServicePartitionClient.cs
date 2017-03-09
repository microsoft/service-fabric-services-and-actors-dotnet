// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Client
{
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    /// <summary>
    /// Defines the interface for the client that communicate with a partition of an Actor Service.
    /// </summary>
    public interface IActorServicePartitionClient : IServiceRemotingPartitionClient
    {
        /// <summary>
        /// Gets the id of the actor this client communicates with.
        /// </summary>
        ActorId ActorId { get; }
    }
}