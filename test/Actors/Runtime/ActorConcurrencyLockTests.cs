// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Tests;
using Microsoft.ServiceFabric.TestFramework;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    public class ActorConcurrencyLockTests
    {
        private static string currentContext = Guid.Empty.ToString();

        private delegate Task<bool> DirtyCallback(Actor actor);

        private interface IDummyActor : IActor
        {
            Task<string> Greetings();
        }

        [Fact]
        public async Task VerifyReentrants()
        {
            var a = new DummyActor();
            var guard = this.CreateAndInitializeReentrancyGuard(a, ActorReentrancyMode.LogicalCallContext);

            var tasks = new Task[1];
            for (var i = 0; i < 1; ++i)
            {
                tasks[i] = Task.Run(() => RunTest(guard), TestContext.Current.CancellationToken);
            }

            await Task.WhenAll(tasks);
        }

        [Fact]
        public async Task VerifyDirtyCallbacks()
        {
            var actor = new DummyActor();
            var guard = this.CreateAndInitializeReentrancyGuard(actor, ActorReentrancyMode.LogicalCallContext);
            actor.IsDirty = true;
            var callContext = Guid.NewGuid().ToString();
            var result = guard.Acquire(callContext, @base => ReplacementHandler(actor), CancellationToken.None);
            try
            {
                await result;
                Assert.False(actor.IsDirty, "ReentrancyGuard IsDirty should be set to false");
            }
            finally
            {
                await guard.ReleaseContext(callContext);
            }

            RunTest(guard);
        }

        [Fact]
        public async Task VerifyInvalidContextRelease()
        {
            var actor = new DummyActor();
            var guard = this.CreateAndInitializeReentrancyGuard(actor, ActorReentrancyMode.LogicalCallContext);
            var context = Guid.NewGuid().ToString();
            await guard.Acquire(context, null, CancellationToken.None);
            Assert.Equal(context, guard.Test_CurrentContext);
            Assert.Equal(1, guard.Test_CurrentCount);

            Action action = () => guard.ReleaseContext(Guid.NewGuid().ToString()).Wait();
            Assert.Throws<AggregateException>(action);

            await guard.ReleaseContext(context);
            Assert.NotEqual(context, guard.Test_CurrentContext);
            Assert.Equal(0, guard.Test_CurrentCount);
        }

        [Fact]
        public async Task ReentrancyDisallowedTest()
        {
            var actor = new DummyActor();
            var guard = this.CreateAndInitializeReentrancyGuard(actor, ActorReentrancyMode.Disallowed);
            var context = Guid.NewGuid().ToString();
            await guard.Acquire(context, null, CancellationToken.None);
            Assert.Equal(context, guard.Test_CurrentContext);
            Assert.Equal(1, guard.Test_CurrentCount);

            Action action = () => guard.Acquire(context, null, CancellationToken.None).Wait();
            Assert.Throws<AggregateException>(action);

            await guard.ReleaseContext(context);
            Assert.NotEqual(context, guard.Test_CurrentContext);
            Assert.Equal(0, guard.Test_CurrentCount);
        }

        private static Task<bool> ReplacementHandler(ActorBase actor)
        {
            Assert.True(actor.IsDirty, "Expect actor to be in dirty state when handler invoked");
            actor.IsDirty = false;
            return Task.FromResult(true);
        }

        private static void RunTest(ActorConcurrencyLock guard)
        {
            var test = Guid.NewGuid().ToString();
            guard.Acquire(test, null, CancellationToken.None).Wait();
            Assert.Equal(1, guard.Test_CurrentCount);
            currentContext = test;
            for (var i = 0; i < 10; i++)
            {
                var testContext = test + ":" + Guid.NewGuid().ToString();
                guard.Acquire(testContext, null, CancellationToken.None).Wait();
                Assert.StartsWith(currentContext, testContext); // Call context Prefix Matching
                guard.ReleaseContext(testContext).Wait();
            }

            Assert.Equal(1, guard.Test_CurrentCount);
            guard.ReleaseContext(test).Wait();
        }

        private ActorConcurrencyLock CreateAndInitializeReentrancyGuard(ActorBase owner, ActorReentrancyMode mode)
        {
            var settings = new ActorConcurrencySettings() { ReentrancyMode = mode };
            var guard = new ActorConcurrencyLock(owner, settings);
            return guard;
        }

        private class DummyActor : Actor, IDummyActor
        {
            public DummyActor()
                : base(GetMockActorService(), null)
            {
            }

            public Task<string> Greetings()
            {
                return Task.FromResult("Hello");
            }

            private static ActorService GetMockActorService()
            {
                var nodeContext = new NodeContext(
                    "MockNodeName",
                    new NodeId(BigInteger.Zero, BigInteger.Zero),
                    BigInteger.Zero,
                    "MockNodeType",
                    "0.0.0.0");

                var serviceContext = new StatefulServiceContext(
                    nodeContext,
                    TestMocksRepository.GetCodePackageActivationContext(),
                    "MockServiceTypeName",
                    new Uri("fabric:/MockServiceName"),
                    null,
                    Guid.Empty,
                    long.MinValue);

                return new ActorService(
                    serviceContext,
                    ActorTypeInformation.Get(typeof(DummyActor)));
            }
        }
    }
}
