// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    /// <summary>
    /// Class containing extension methods for Actors.
    /// </summary>
    public static class ActorExtensions
    {
        /// <summary>
        /// Gets <see cref="ActorId"/> for the actor.
        /// </summary>
        /// <typeparam name="TIActor">Actor interface type.</typeparam>
        /// <param name="actor">Actor object to get ActorId for.</param>
        /// <returns><see cref="ActorId"/> for the actor.</returns>
        public static ActorId GetActorId<TIActor>(this TIActor actor)
            where TIActor : IActor
        {
            var r = ActorReference.Get(actor);
            return r.ActorId;
        }

        /// <summary>
        /// Gets <see cref="ActorReference"/> for the actor.
        /// </summary>
        /// <param name="actor">Actor object to get ActorReference for.</param>
        /// <returns><see cref="ActorReference"/> for the actor.</returns>
        public static ActorReference GetActorReference(this IActor actor)
        {
            return ActorReference.Get(actor);
        }
    }
}
