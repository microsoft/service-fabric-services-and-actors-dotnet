// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;

    internal class ActorActivator : IActorActivator
    {
        private readonly Func<ActorService, ActorId, ActorBase> factory;

        public ActorActivator(Func<ActorService, ActorId, ActorBase> factory)
        {
            this.factory = factory;
        }

        ActorBase IActorActivator.Activate(ActorService actorService, ActorId actorId)
        {
            return this.factory.Invoke(actorService, actorId);
        }
    }
}
