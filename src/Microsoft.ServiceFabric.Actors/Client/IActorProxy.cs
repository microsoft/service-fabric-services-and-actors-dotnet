// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Actors.Client
{
    /// <summary>
    /// Provides the interface for implementation of proxy access for actor service.
    /// </summary>
    public interface IActorProxy
    {
        /// <summary>
        /// Gets <see cref="ServiceFabric.Actors.ActorId"/> associated with the proxy object.
        /// </summary>
        /// <value><see cref="ServiceFabric.Actors.ActorId"/> associated with the proxy object.</value>
        ActorId ActorId { get; }

#if !DotNetCoreClr
        /// <summary>
        /// Gets <see cref="Remoting.V1.Client.IActorServicePartitionClient"/> that this proxy is using to communicate with the actor.
        /// </summary>
        /// <value><see cref="Remoting.V1.Client.IActorServicePartitionClient"/> that this proxy is using to communicate with the actor.</value>
        [Obsolete(Services.Remoting.DeprecationMessage.RemotingV1)]
        Remoting.V1.Client.IActorServicePartitionClient ActorServicePartitionClient { get; }
#endif

        /// <summary>
        /// Gets <see cref="Remoting.V2.Client.IActorServicePartitionClient"/> that this proxy is using to communicate with the actor.
        /// </summary>
        /// <value><see cref="Remoting.V2.Client.IActorServicePartitionClient"/> that this proxy is using to communicate with the actor.</value>
        Remoting.V2.Client.IActorServicePartitionClient ActorServicePartitionClientV2 { get; }
    }
}
