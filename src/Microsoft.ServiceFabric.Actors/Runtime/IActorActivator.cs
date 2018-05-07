// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    /// <summary>
    /// Contains methods for activiation of actor.
    /// </summary>
    internal interface IActorActivator
    {
        /// <summary>
        /// Method to create actor isntance.
        /// </summary>
        /// <param name="actorService">Actor service.</param>
        /// <param name="actorId">Actor id to create isntance for.</param>
        /// <returns>Instance of the actor created.</returns>
        ActorBase Activate(ActorService actorService, ActorId actorId);
    }
}
