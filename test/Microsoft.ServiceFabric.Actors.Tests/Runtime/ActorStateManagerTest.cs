// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Fuzzy;
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

        readonly ActorStateManager sut;

        readonly IDiagnostics diagnosticEvents = Mock.Of<IDiagnostics>();
        readonly IClock clock = Mock.Of<IClock>();

        readonly ActorId actorId = fuzzy.ActorId();
        readonly DateTime startTime = DateTime.Now;
        readonly ActorService actorService = TestMocksRepository.GetActorService<TestActor>();

        public ActorStateManagerTest()
        {
            actorService.InitializeInternal(new ActorMethodFriendlyNameBuilder(actorService.ActorTypeInformation));
            Mock.Get(clock).Setup(clock => clock.UtcNow).Returns(startTime);

            sut = new ActorStateManager(new TestActor(actorService, actorId), new NullActorStateProvider(), diagnosticEvents, clock);
        }

        public class State : ActorStateManagerTest
        {
            readonly string stateName = fuzzy.String();

            [Fact]
            public async Task SaveEmitsDiagnostics()
            {
                await sut.TryAddStateAsync(stateName, fuzzy.String(), TestContext.Current.CancellationToken);

                await sut.SaveStateAsync(TestContext.Current.CancellationToken);

                Mock.Get(diagnosticEvents).Verify(d => d.SaveActorStateStart(actorId), Times.Once);
                Mock.Get(diagnosticEvents).Verify(d => d.SaveActorStateFinish(actorId, startTime), Times.Once);
            }

            [Fact]
            public async Task TryGetEmitsDiagnostics()
            {
                await sut.TryAddStateAsync(stateName, fuzzy.String(), TestContext.Current.CancellationToken);
                await sut.SaveStateAsync(TestContext.Current.CancellationToken);
                await sut.ClearCacheAsync(TestContext.Current.CancellationToken);

                await sut.TryGetStateAsync<string>(stateName, TestContext.Current.CancellationToken);

                Mock.Get(diagnosticEvents).Verify(d => d.LoadActorStateStart(), Times.Once);
                Mock.Get(diagnosticEvents).Verify(d => d.LoadActorStateFinish(startTime), Times.Once);
            }
        }
    }
}

