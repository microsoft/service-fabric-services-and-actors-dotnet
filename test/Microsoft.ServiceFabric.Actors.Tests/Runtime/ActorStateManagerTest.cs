// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Actors.Diagnostics;
using Microsoft.ServiceFabric.Actors.Tests;
using Microsoft.ServiceFabric.Diagnostics;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    public class ActorStateManagerTest
    {
        readonly static IFuzz fuzzy = new RandomFuzz();

        internal readonly ActorId actorId = ActorId.CreateRandom();
        internal readonly ActorService actorService = TestMocksRepository.GetActorService<MockActor>();

        readonly IDiagnosticEvents diagnosticEvents = Mock.Of<IDiagnosticEvents>();
        readonly IClock clock = Mock.Of<IClock>();
        readonly DateTime startTime = DateTime.Now;
        readonly ActorStateManager actorStateManager;

        public ActorStateManagerTest()
        {
            var friendlyNameBuilder = new ActorMethodFriendlyNameBuilder(actorService.ActorTypeInformation);
            actorService.InitializeInternal(friendlyNameBuilder);
            actorService.Field<IClock>().Set(clock);

            Mock.Get(clock).Setup(clock => clock.UtcNow).Returns(startTime);

            actorStateManager = new ActorStateManager(new MockActor(actorService, actorId), new NullActorStateProvider());
            actorStateManager.Field<IDiagnosticEvents>().Set(diagnosticEvents);
        }

        public class State : ActorStateManagerTest
        {
            readonly string stateName = fuzzy.String();

            [Fact]
            public async Task SaveEmitsDiagnostics()
            {
                await actorStateManager.TryAddStateAsync(stateName, fuzzy.String(), TestContext.Current.CancellationToken);

                await actorStateManager.SaveStateAsync(TestContext.Current.CancellationToken);

                Mock.Get(diagnosticEvents).Verify(d => d.SaveActorStateStart(actorId), Times.Once);
                Mock.Get(diagnosticEvents).Verify(d => d.SaveActorStateFinish(actorId, startTime), Times.Once);
            }

            [Fact]
            public async Task TryGetEmitsDiagnostics()
            {
                await actorStateManager.TryAddStateAsync(stateName, fuzzy.String(), TestContext.Current.CancellationToken);
                await actorStateManager.SaveStateAsync(TestContext.Current.CancellationToken);
                await actorStateManager.ClearCacheAsync(TestContext.Current.CancellationToken);

                await actorStateManager.TryGetStateAsync<string>(stateName, TestContext.Current.CancellationToken);

                Mock.Get(diagnosticEvents).Verify(d => d.LoadActorStateStart(), Times.Once);
                Mock.Get(diagnosticEvents).Verify(d => d.LoadActorStateFinish(startTime), Times.Once);
            }
        }
    }
}

