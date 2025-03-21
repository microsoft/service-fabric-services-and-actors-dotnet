// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Actors.Remoting.V2.Client;

namespace Microsoft.ServiceFabric.Actors.Client
{
    /// <summary>
    /// Provides the interface for implementation of proxy access for actor service.
    /// </summary>
    public interface IActorProxy
    {
        /// <summary>
        /// Gets <see cref="Actors.ActorId"/> associated with the proxy object.
        /// </summary>
        /// <value><see cref="Actors.ActorId"/> associated with the proxy object.</value>
        ActorId ActorId { get; }

        /// <summary>
        /// Gets <see cref="IActorServicePartitionClient"/> that this proxy is using to communicate with the actor.
        /// </summary>
        /// <value><see cref="IActorServicePartitionClient"/> that this proxy is using to communicate with the actor.</value>
        IActorServicePartitionClient ActorServicePartitionClientV2 { get; }
    }
}
