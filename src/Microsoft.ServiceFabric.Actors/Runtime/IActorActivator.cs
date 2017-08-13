// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    /// <summary>
    /// Represents the interface that an actor activator needs to implement for 
    /// actor runtime to communicate with it. 
    /// </summary>
    public interface IActorActivator
    {
        /// <summary>
        /// The method that is used to active an instance of an actor
        /// using the actor service and actor id
        /// </summary>
        /// <param name="actorService">This parameter is the actor service to be used.</param>
        /// <param name="actorId">This parameter is the actor id to be used.</param>
        /// <returns>The returned value is an instance of the actor.</returns>
        ActorBase Activate(ActorService actorService, ActorId actorId);
    }
}