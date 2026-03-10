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
    public abstract class AggregatedDiagnosticsTest
    {
        internal interface ITestDiagnosticEvents : IDiagnostics { }

        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IDiagnostics sut;

        readonly IDiagnostics diagnostics = Mock.Of<IDiagnostics>();
        readonly IDiagnostics anotherDiagnostics = Mock.Of<ITestDiagnosticEvents>();

        readonly ActorId actorId = fuzzy.ActorId();
        readonly long interfaceMethodKey = fuzzy.Int64();
        readonly protected DateTime startTime = fuzzy.DateTime();
        readonly PendingActorMethodDiagnosticData pendingActorMethodDiagnosticData = default;
        readonly ActorMethodDiagnosticData actorMethodDiagnosticData = default;

        public AggregatedDiagnosticsTest() => sut = new AggregatedDiagnostics(new List<IDiagnostics> { diagnostics, anotherDiagnostics });

        public sealed class Constructor : AggregatedDiagnosticsTest
        {
            [Fact]
            public void ThrowsOnNullEventsList()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new AggregatedDiagnostics(null));
                Assert.Equal("diagnosticEvents", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnAnyNullEvents()
            {
                var exception = Assert.Throws<ArgumentException>(() => new AggregatedDiagnostics(new List<IDiagnostics> { diagnostics, null }));
                Assert.Equal("diagnosticEvents", exception.Message);
            }

            [Fact]
            public void AssignsEmptyEvent()
            {
                var newSut = new AggregatedDiagnostics(new List<IDiagnostics>());

                Assert.NotNull(newSut.Field<IEnumerable<IDiagnostics>>());
                Assert.Empty(newSut.Field<IEnumerable<IDiagnostics>>().Value);
            }

            [Fact]
            public void AssignsSingleEvent()
            {
                var newSut = new AggregatedDiagnostics(new List<IDiagnostics>() { diagnostics });

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

        public sealed class ActorRequestProcessing : AggregatedDiagnosticsTest
        {
            [Fact]
            public void StartInvokesAllDiagnostics()
            {
                sut.ActorRequestProcessingStart();

                Mock.Get(diagnostics).Verify(ds => ds.ActorRequestProcessingStart(), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.ActorRequestProcessingStart(), Times.Once);
            }

            [Fact]
            public void FinishInvokesAllDiagnostics()
            {
                sut.ActorRequestProcessingFinish(startTime);

                Mock.Get(diagnostics).Verify(ds => ds.ActorRequestProcessingFinish(startTime), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.ActorRequestProcessingFinish(startTime), Times.Once);
            }
        }

        public sealed class ActorOnActivateAsync : AggregatedDiagnosticsTest
        {
            [Fact]
            public void StartInvokesAllDiagnostics()
            {
                sut.ActorOnActivateAsyncStart();

                Mock.Get(diagnostics).Verify(ds => ds.ActorOnActivateAsyncStart(), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.ActorOnActivateAsyncStart(), Times.Once);
            }

            [Fact]
            public void FinishInvokesAllDiagnostics()
            {
                sut.ActorOnActivateAsyncFinish(startTime);

                Mock.Get(diagnostics).Verify(ds => ds.ActorOnActivateAsyncFinish(startTime), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.ActorOnActivateAsyncFinish(startTime), Times.Once);
            }
        }

        public sealed class ActorMethod : AggregatedDiagnosticsTest
        {
            [Fact]
            public void StartInvokesAllDiagnostics()
            {
                sut.ActorMethodStart(actorId, interfaceMethodKey);

                Mock.Get(diagnostics).Verify(ds => ds.ActorMethodStart(actorId, interfaceMethodKey), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.ActorMethodStart(actorId, interfaceMethodKey), Times.Once);
            }

            [Fact]
            public void FinishInvokesAllDiagnostics()
            {
                var exception = new InvalidOperationException(fuzzy.String());

                sut.ActorMethodFinish(actorMethodDiagnosticData, startTime);

                Mock.Get(diagnostics).Verify(ds => ds.ActorMethodFinish(actorMethodDiagnosticData, startTime), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.ActorMethodFinish(actorMethodDiagnosticData, startTime), Times.Once);
            }
        }

        public sealed class ActorStateLoad : AggregatedDiagnosticsTest
        {
            [Fact]
            public void StartInvokesAllDiagnostics()
            {
                sut.LoadActorStateStart();

                Mock.Get(diagnostics).Verify(ds => ds.LoadActorStateStart(), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.LoadActorStateStart(), Times.Once);
            }

            [Fact]
            public void FinishInvokesAllDiagnostics()
            {
                sut.LoadActorStateFinish(startTime);

                Mock.Get(diagnostics).Verify(ds => ds.LoadActorStateFinish(startTime), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.LoadActorStateFinish(startTime), Times.Once);
            }

            [Fact]
            public void SaveStartInvokesAllDiagnostics()
            {
                sut.SaveActorStateStart(actorId);

                Mock.Get(diagnostics).Verify(ds => ds.SaveActorStateStart(actorId), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.SaveActorStateStart(actorId), Times.Once);
            }

            [Fact]
            public void SaveFinishInvokesAllDiagnostics()
            {
                sut.SaveActorStateFinish(actorId, startTime);

                Mock.Get(diagnostics).Verify(ds => ds.SaveActorStateFinish(actorId, startTime), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.SaveActorStateFinish(actorId, startTime), Times.Once);
            }
        }

        public sealed class ActorLock : AggregatedDiagnosticsTest
        {
            [Fact]
            public void AcquireFinishInvokesAllDiagnostics()
            {
                sut.AcquireActorLockFinish(pendingActorMethodDiagnosticData, startTime);

                Mock.Get(diagnostics).Verify(ds => ds.AcquireActorLockFinish(pendingActorMethodDiagnosticData, startTime), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.AcquireActorLockFinish(pendingActorMethodDiagnosticData, startTime), Times.Once);
            }

            [Fact]
            public void ReleaseInvokesAllDiagnostics()
            {
                sut.ReleaseActorLock(startTime);

                Mock.Get(diagnostics).Verify(ds => ds.ReleaseActorLock(startTime), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.ReleaseActorLock(startTime), Times.Once);
            }
        }

        public sealed class ActorLifecycle : AggregatedDiagnosticsTest
        {
            [Fact]
            public void ChangeRoleInvokesAllDiagnostics()
            {
                const ReplicaRole currentRole = ReplicaRole.Primary;
                const ReplicaRole newRole = ReplicaRole.ActiveSecondary;

                sut.ActorChangeRole(currentRole, newRole);

                Mock.Get(diagnostics).Verify(ds => ds.ActorChangeRole(currentRole, newRole), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.ActorChangeRole(currentRole, newRole), Times.Once);
            }

            [Fact]
            public void ActivatedInvokesAllDiagnostics()
            {
                sut.ActorActivated(actorId);

                Mock.Get(diagnostics).Verify(ds => ds.ActorActivated(actorId), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.ActorActivated(actorId), Times.Once);
            }

            [Fact]
            public void DeactivatedInvokesAllDiagnostics()
            {
                sut.ActorDeactivated(actorId);

                Mock.Get(diagnostics).Verify(ds => ds.ActorDeactivated(actorId), Times.Once);
                Mock.Get(anotherDiagnostics).Verify(ds => ds.ActorDeactivated(actorId), Times.Once);
            }
        }

        public sealed class Dispose : AggregatedDiagnosticsTest
        {
            [Fact]
            public void DisposesAllChildren()
            {
                var child1 = Mock.Of<IDiagnostics>();
                var child2 = Mock.Of<IDiagnostics>();

                var aggregated = new AggregatedDiagnostics(new List<IDiagnostics> { child1, child2 });
                aggregated.Dispose();

                Mock.Get(child1).Verify(d => d.Dispose(), Times.Once);
                Mock.Get(child2).Verify(d => d.Dispose(), Times.Once);
            }

            [Fact]
            public void DoesNotThrowWhenEmpty()
            {
                var aggregated = new AggregatedDiagnostics(new List<IDiagnostics>());
                aggregated.Dispose();
            }
        }
    }
}
