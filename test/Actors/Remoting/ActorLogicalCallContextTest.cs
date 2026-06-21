// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Remoting;

public abstract class ActorLogicalCallContextTest : IDisposable
{
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ActorLogicalCallContextTest() =>
        ActorLogicalCallContext.Clear();

    void IDisposable.Dispose() =>
        ActorLogicalCallContext.Clear();

    public sealed class Clear : ActorLogicalCallContextTest
    {
        [Fact]
        public void RemovesPreviouslySetValue()
        {
            ActorLogicalCallContext.Set(fuzzy.String());

            ActorLogicalCallContext.Clear();

            Assert.False(ActorLogicalCallContext.TryGet(out string callContextValue));
            Assert.Null(callContextValue);
        }

        [Fact]
        public void IsNoOpWhenValueIsNotSet()
        {
            ActorLogicalCallContext.Clear();
            Assert.False(ActorLogicalCallContext.TryGet(out string callContextValue));
            Assert.Null(callContextValue);
        }

        [Fact]
        public async Task InAwaitedTaskDoesNotAffectCaller()
        {
            string expected = fuzzy.String();
            ActorLogicalCallContext.Set(expected);

            await Task.Run(ActorLogicalCallContext.Clear, TestContext.Current.CancellationToken);

            ActorLogicalCallContext.TryGet(out string actual);
            Assert.Same(expected, actual);
        }
    }

    public sealed class IsPresent : ActorLogicalCallContextTest
    {
        [Fact]
        public void ReturnsFalseWhenValueIsNotSet() =>
            Assert.False(ActorLogicalCallContext.IsPresent());

        [Fact]
        public void ReturnsTrueWhenValueIsSet()
        {
            ActorLogicalCallContext.Set(fuzzy.String());
            Assert.True(ActorLogicalCallContext.IsPresent());
        }
    }

    public sealed class Set : ActorLogicalCallContextTest
    {
        readonly string callContextValue = fuzzy.String();

        [Fact]
        public void StoresValueObservableByTryGet()
        {
            ActorLogicalCallContext.Set(callContextValue);

            ActorLogicalCallContext.TryGet(out string actual);
            Assert.Same(callContextValue, actual);
        }

        [Fact]
        public void OverwritesPreviousValue()
        {
            ActorLogicalCallContext.Set(callContextValue + fuzzy.String());

            ActorLogicalCallContext.Set(callContextValue);

            ActorLogicalCallContext.TryGet(out string actual);
            Assert.Same(callContextValue, actual);
        }

        [Fact]
        public void IsEquivalentToClearWhenCallContextValueIsNull()
        {
            ActorLogicalCallContext.Set(callContextValue);

            ActorLogicalCallContext.Set(null);

            Assert.False(ActorLogicalCallContext.TryGet(out string actual));
            Assert.Null(actual);
        }

        [Fact]
        public async Task StoresValueObservableByAwaitedTask()
        {
            ActorLogicalCallContext.Set(callContextValue);

            string actual = await Task.Run(() =>
            {
                ActorLogicalCallContext.TryGet(out string value);
                return value;
            }, TestContext.Current.CancellationToken);

            Assert.Same(callContextValue, actual);
        }

        [Fact]
        public async Task StoresValueObservableAfterAwait()
        {
            ActorLogicalCallContext.Set(callContextValue);

            await Task.Yield();

            ActorLogicalCallContext.TryGet(out string actual);
            Assert.Same(callContextValue, actual);
        }

        [Fact]
        public async Task InAwaitedTaskDoesNotAffectCaller()
        {
            ActorLogicalCallContext.Set(callContextValue);

            await Task.Run(() => ActorLogicalCallContext.Set(callContextValue + fuzzy.String()), TestContext.Current.CancellationToken);

            ActorLogicalCallContext.TryGet(out string actual);
            Assert.Same(callContextValue, actual);
        }
    }

    public sealed class TryGet : ActorLogicalCallContextTest
    {
        [Fact]
        public void ReturnsFalseAndNullWhenValueIsNotSet()
        {
            bool result = ActorLogicalCallContext.TryGet(out string callContextValue);
            Assert.False(result);
            Assert.Null(callContextValue);
        }

        [Fact]
        public void ReturnsTrueAndValueWhenValueIsSet()
        {
            string expected = fuzzy.String();
            ActorLogicalCallContext.Set(expected);

            bool result = ActorLogicalCallContext.TryGet(out string callContextValue);

            Assert.True(result);
            Assert.Same(expected, callContextValue);
        }
    }
}
