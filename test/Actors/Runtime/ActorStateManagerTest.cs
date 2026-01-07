// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using System.Threading.Tasks;
using Fuzzy;
using Microsoft.ServiceFabric.Actors.Diagnostics;
using Microsoft.ServiceFabric.Actors.Tests;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.TestFramework;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    public class ActorStateManagerTest : MockedMetricsTest
    {
        readonly static IFuzz fuzzy = new RandomFuzz();

        readonly ActorStateManager sut;

        readonly IDiagnostics diagnosticEvents = Mock.Of<IDiagnostics>();
        readonly IClock clock = Mock.Of<IClock>();

        readonly ActorId actorId = fuzzy.ActorId();
        readonly DateTime startTime = DateTime.Now;
        readonly StatefulServiceContext statefulServiceContext = fuzzy.StatefulServiceContext();

        public ActorStateManagerTest()
        {
            Mock.Get(clock).Setup(clock => clock.UtcNow).Returns(startTime);

            var mockedActorService = new Mock<ActorService>(statefulServiceContext, ActorTypeInformation.Get(typeof(TestActor)), null, null, null, null).Object;
            sut = new ActorStateManager(new TestActor(mockedActorService, actorId), new NullActorStateProvider(), diagnosticEvents, clock);
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

