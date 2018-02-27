// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Query
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Actors.Remoting;


    /// <summary>
    /// Represents the information about an actor returned by Actor Query method call.
    /// </summary>
    [DataContract(Name = "ActorInformation", Namespace = Constants.Namespace)]
    [Serializable]
    public sealed class ActorInformation
    {
        /// <summary>
        /// Initializes a new instance of the ActorInformation class.
        /// </summary>
        /// <param name="actorId">The ID of the actor.</param>
        /// <param name="isActive">Indicates whether the Actor is active or inactive.</param>
        public ActorInformation(ActorId actorId, bool isActive)
        {
            this.ActorId = actorId;
            this.IsActive = isActive;
        }

        /// <summary>
        /// Gets ActorId for the actor.
        /// </summary>
        /// <value>The <see cref="ServiceFabric.Actors.ActorId"/> for the actor.</value>
        [DataMember(Order = 0, Name = "ActorId", IsRequired = true)]
        public ActorId ActorId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the actor is active.
        /// </summary>
        /// <value><see cref="System.Boolean"/> indicating if the Actor is active.</value>
        [DataMember(Order = 1, Name = "IsActive", IsRequired = true)]
        public bool IsActive { get; private set; }
    }
}
