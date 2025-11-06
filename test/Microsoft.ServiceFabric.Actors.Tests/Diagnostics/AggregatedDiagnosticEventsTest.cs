// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Actors.Tests;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    public abstract class AggregatedDiagnosticEventsTest
    {
        internal interface ITestDiagnosticEvents : IDiagnostics { }

        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IDiagnostics sut;

        readonly IDiagnostics diagnosticEvent = Mock.Of<IDiagnostics>();
        readonly IDiagnostics anotherDiagnosticEvents = Mock.Of<ITestDiagnosticEvents>();

        readonly ActorId actorId = fuzzy.ActorId();
        readonly long interfaceMethodKey = fuzzy.Int64();
        readonly protected DateTime startTime = fuzzy.DateTime();
        readonly PendingActorMethodDiagnosticData pendingActorMethodDiagnosticData = default;
        readonly ActorMethodDiagnosticData actorMethodDiagnosticData = default;

        public AggregatedDiagnosticEventsTest() => sut = new AggregatedDiagnosticEvents(new List<IDiagnostics> { diagnosticEvent, anotherDiagnosticEvents });

        public sealed class Constructor : AggregatedDiagnosticEventsTest
        {
            [Fact]
            public void ThrowsOnNullEventsList()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new AggregatedDiagnosticEvents(null));
                Assert.Equal("diagnosticEvents", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnAnyNullEvents()
            {
                var exception = Assert.Throws<ArgumentException>(() => new AggregatedDiagnosticEvents(new List<IDiagnostics> { diagnosticEvent, null }));
                Assert.Equal("diagnosticEvents", exception.Message);
            }

            [Fact]
            public void AssignsEmptyEvent()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnostics>());

                Assert.NotNull(newSut.Field<IEnumerable<IDiagnostics>>());
                Assert.Empty(newSut.Field<IEnumerable<IDiagnostics>>().Value);
            }

            [Fact]
            public void AssignsSingleEvent()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnostics>() { diagnosticEvent });

                Assert.NotNull(newSut.Field<IEnumerable<IDiagnostics>>());
                Assert.Single(newSut.Field<IEnumerable<IDiagnostics>>().Value);
                Assert.IsAssignableFrom<IDiagnostics>(newSut.Field<IEnumerable<IDiagnostics>>().Value.First());
            }

            [Fact]
            public void AssignsMultipleEvent()
            {
                Assert.NotNull(sut.Field<IEnumerable<IDiagnostics>>());
                Assert.Equal(2, sut.Field<IEnumerable<IDiagnostics>>().Value.Count());
                Assert.IsAssignableFrom<IDiagnostics>(sut.Field<IEnumerable<IDiagnostics>>().Value.First());
                Assert.IsAssignableFrom<ITestDiagnosticEvents>(sut.Field<IEnumerable<IDiagnostics>>().Value.Last());
            }
        }

        public sealed class ActorRequestProcessing : AggregatedDiagnosticEventsTest
        {
            [Fact]
            public void StartInvokesAllDiagnostics()
            {
                sut.ActorRequestProcessingStart();

                Mock.Get(diagnosticEvent).Verify(ds => ds.ActorRequestProcessingStart(), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ActorRequestProcessingStart(), Times.Once);
            }

            [Fact]
            public void FinishInvokesAllDiagnostics()
            {
                sut.ActorRequestProcessingFinish(startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.ActorRequestProcessingFinish(startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ActorRequestProcessingFinish(startTime), Times.Once);
            }
        }

        public sealed class ActorOnActivateAsync : AggregatedDiagnosticEventsTest
        {
            [Fact]
            public void StartInvokesAllDiagnostics()
            {
                sut.ActorOnActivateAsyncStart();

                Mock.Get(diagnosticEvent).Verify(ds => ds.ActorOnActivateAsyncStart(), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ActorOnActivateAsyncStart(), Times.Once);
            }

            [Fact]
            public void FinishInvokesAllDiagnostics()
            {
                sut.ActorOnActivateAsyncFinish(startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.ActorOnActivateAsyncFinish(startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ActorOnActivateAsyncFinish(startTime), Times.Once);
            }
        }

        public sealed class ActorMethod : AggregatedDiagnosticEventsTest
        {
            [Fact]
            public void StartInvokesAllDiagnostics()
            {
                sut.ActorMethodStart(actorId, interfaceMethodKey);

                Mock.Get(diagnosticEvent).Verify(ds => ds.ActorMethodStart(actorId, interfaceMethodKey), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ActorMethodStart(actorId, interfaceMethodKey), Times.Once);
            }

            [Fact]
            public void FinishInvokesAllDiagnostics()
            {
                var exception = new InvalidOperationException(fuzzy.String());

                sut.ActorMethodFinish(actorMethodDiagnosticData, startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.ActorMethodFinish(actorMethodDiagnosticData, startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ActorMethodFinish(actorMethodDiagnosticData, startTime), Times.Once);
            }
        }

        public sealed class ActorStateLoad : AggregatedDiagnosticEventsTest
        {
            [Fact]
            public void StartInvokesAllDiagnostics()
            {
                sut.LoadActorStateStart();

                Mock.Get(diagnosticEvent).Verify(ds => ds.LoadActorStateStart(), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.LoadActorStateStart(), Times.Once);
            }

            [Fact]
            public void FinishInvokesAllDiagnostics()
            {
                sut.LoadActorStateFinish(startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.LoadActorStateFinish(startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.LoadActorStateFinish(startTime), Times.Once);
            }

            [Fact]
            public void SaveStartInvokesAllDiagnostics()
            {
                sut.SaveActorStateStart(actorId);

                Mock.Get(diagnosticEvent).Verify(ds => ds.SaveActorStateStart(actorId), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.SaveActorStateStart(actorId), Times.Once);
            }

            [Fact]
            public void SaveFinishInvokesAllDiagnostics()
            {
                sut.SaveActorStateFinish(actorId, startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.SaveActorStateFinish(actorId, startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.SaveActorStateFinish(actorId, startTime), Times.Once);
            }
        }

        public sealed class ActorLock : AggregatedDiagnosticEventsTest
        {
            [Fact]
            public void AcquireFinishInvokesAllDiagnostics()
            {
                sut.AcquireActorLockFinish(pendingActorMethodDiagnosticData, startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.AcquireActorLockFinish(pendingActorMethodDiagnosticData, startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.AcquireActorLockFinish(pendingActorMethodDiagnosticData, startTime), Times.Once);
            }

            [Fact]
            public void ReleaseInvokesAllDiagnostics()
            {
                sut.ReleaseActorLock(startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.ReleaseActorLock(startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ReleaseActorLock(startTime), Times.Once);
            }
        }

        public sealed class ActorLifecycle : AggregatedDiagnosticEventsTest
        {
            [Fact]
            public void ChangeRoleInvokesAllDiagnostics()
            {
                const ReplicaRole currentRole = ReplicaRole.Primary;
                const ReplicaRole newRole = ReplicaRole.ActiveSecondary;

                sut.ActorChangeRole(currentRole, newRole);

                Mock.Get(diagnosticEvent).Verify(ds => ds.ActorChangeRole(currentRole, newRole), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ActorChangeRole(currentRole, newRole), Times.Once);
            }

            [Fact]
            public void ActivatedInvokesAllDiagnostics()
            {
                sut.ActorActivated(actorId);

                Mock.Get(diagnosticEvent).Verify(ds => ds.ActorActivated(actorId), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ActorActivated(actorId), Times.Once);
            }

            [Fact]
            public void DeactivatedInvokesAllDiagnostics()
            {
                sut.ActorDeactivated(actorId);

                Mock.Get(diagnosticEvent).Verify(ds => ds.ActorDeactivated(actorId), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ActorDeactivated(actorId), Times.Once);
            }
        }
    }
}
