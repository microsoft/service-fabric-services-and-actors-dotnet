// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Fuzzy;
using Microsoft.ServiceFabric.Actors.Tests;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Client;

public abstract class SubscriberTest
{
    readonly Subscriber sut;

    // Constructor parameters
    readonly ActorId actorId = fuzzy.ActorId();
    readonly int eventId = fuzzy.Int32();
    readonly object instance = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    SubscriberTest() =>
        sut = new Subscriber(actorId, eventId, instance);

    public sealed class Constructor : SubscriberTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Same(actorId, sut.ActorId);
            Assert.Equal(eventId, sut.EventId);
            Assert.Same(instance, sut.Instance);
        }
    }

    public new sealed class Equals : SubscriberTest
    {
        // Method parameters
        object obj;

        public Equals() =>
            obj = new Subscriber(actorId, eventId, instance);

        [Fact]
        public void ReturnsTrueWhenActorIdEventIdAndInstanceMatch() =>
            Assert.True(sut.Equals(obj));

        [Fact]
        public void ReturnsTrueWhenActorIdValueMatchesButReferenceDiffers()
        {
            ActorId clone = actorId.Kind switch
            {
                ActorIdKind.Long => new ActorId(actorId.GetLongId()),
                ActorIdKind.Guid => new ActorId(actorId.GetGuidId()),
                ActorIdKind.String => new ActorId(actorId.GetStringId()),
                _ => throw new InvalidOperationException(),
            };
            Assert.NotSame(actorId, clone);
            obj = new Subscriber(clone, eventId, instance);

            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenObjIsNull()
        {
            obj = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenObjIsNotSubscriber()
        {
            obj = new object();
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenActorIdIsDifferent()
        {
            var differentActorId = new ActorId(actorId.ToString() + fuzzy.String());
            obj = new Subscriber(differentActorId, eventId, instance);

            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenEventIdIsDifferent()
        {
            obj = new Subscriber(actorId, eventId + fuzzy.SByte().Between(1, 5), instance);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenInstanceIsDifferentReference()
        {
            obj = new Subscriber(actorId, eventId, new object());
            Assert.False(sut.Equals(obj));
        }
    }

    public new sealed class GetHashCode : SubscriberTest
    {
        [Fact]
        public void ReturnsEqualHashCodesForEqualSubscribers() =>
            Assert.Equal(new Subscriber(actorId, eventId, instance).GetHashCode(), sut.GetHashCode());
    }
}
