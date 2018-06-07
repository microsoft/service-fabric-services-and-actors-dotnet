// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Client
{
    using Microsoft.ServiceFabric.Services.Common;

    internal class Subscriber
    {
        public Subscriber(ActorId actorId, int eventId, object instance)
        {
            this.ActorId = actorId;
            this.EventId = eventId;
            this.Instance = instance;
        }

        public ActorId ActorId { get; }

        public int EventId { get; }

        public object Instance { get; }

        public override bool Equals(object obj)
        {
            return (
                (obj is Subscriber other) &&
                (this.EventId.Equals(other.EventId)) &&
                (this.ActorId.Equals(other.ActorId)) &&
                (ReferenceEquals(this.Instance, other.Instance)));
        }

        public override int GetHashCode()
        {
            var hash = this.ActorId.GetHashCode();
            hash = IdUtil.HashCombine(hash, this.EventId.GetHashCode());
            return IdUtil.HashCombine(hash, this.Instance.GetHashCode());
        }
    }
}
