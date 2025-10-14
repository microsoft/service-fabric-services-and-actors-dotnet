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
using Microsoft.ServiceFabric.Services.Remoting;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    public abstract class AgregateDiagnosticEventsTest
    {
        internal interface ITestDiagnosticEvents : IDiagnosticEvents { }

        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IDiagnosticEvents sut;

        readonly IDiagnosticEvents diagnosticEvent = Mock.Of<IDiagnosticEvents>();
        readonly IDiagnosticEvents anotherDiagnosticEvents = Mock.Of<ITestDiagnosticEvents>();

        readonly DiagnosticsManagerActorContext diagnosticContext = new DiagnosticsManagerActorContext();
        readonly ActorId actorId = new ActorId(Guid.NewGuid());
        readonly long interfaceMethodKey = fuzzy.Int64();
        readonly RemotingListenerVersion remotingListener = RemotingListenerVersion.V2_1;

        public AgregateDiagnosticEventsTest() => sut = new AgregateDiagnosticEvents(new List<IDiagnosticEvents> { diagnosticEvent, anotherDiagnosticEvents });


        public sealed class Constructor : AgregateDiagnosticEventsTest
        {
            [Fact]
            public void ThrowsOnNullEventsList()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new AgregateDiagnosticEvents(null));
                Assert.Equal("diagnosticEvents", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnAnyNullEvents()
            {
                var exception = Assert.Throws<ArgumentException>(() => new AgregateDiagnosticEvents(new List<IDiagnosticEvents> { diagnosticEvent, null }));
                Assert.Equal("diagnosticEvents", exception.Message);
            }

            [Fact]
            public void AssignsEmptyEvent()
            {
                var newSut = new AgregateDiagnosticEvents(new List<IDiagnosticEvents>());

                Assert.NotNull(newSut.Field<IEnumerable<IDiagnosticEvents>>());
                Assert.Empty(newSut.Field<IEnumerable<IDiagnosticEvents>>().Value);
            }

            [Fact]
            public void AssignsSingleEvent()
            {
                var newSut = new AgregateDiagnosticEvents(new List<IDiagnosticEvents>() { diagnosticEvent });

                Assert.NotNull(newSut.Field<IEnumerable<IDiagnosticEvents>>());
                Assert.Single(newSut.Field<IEnumerable<IDiagnosticEvents>>().Value);
                Assert.IsAssignableFrom<IDiagnosticEvents>(newSut.Field<IEnumerable<IDiagnosticEvents>>().Value.First());
            }

            [Fact]
            public void AssignsMultipleEvent()
            {
                Assert.NotNull(sut.Field<IEnumerable<IDiagnosticEvents>>());
                Assert.Equal(2, sut.Field<IEnumerable<IDiagnosticEvents>>().Value.Count());
                Assert.IsAssignableFrom<IDiagnosticEvents>(sut.Field<IEnumerable<IDiagnosticEvents>>().Value.First());
                Assert.IsAssignableFrom<ITestDiagnosticEvents>(sut.Field<IEnumerable<IDiagnosticEvents>>().Value.Last());
            }
        }

        public sealed class ActorRequestProcessing : AgregateDiagnosticEventsTest
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
                var startTime = DateTime.UtcNow;
                sut.ActorRequestProcessingFinish(startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.ActorRequestProcessingFinish(startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ActorRequestProcessingFinish(startTime), Times.Once);
            }
        }

        public sealed class ActorOnActivateAsync : AgregateDiagnosticEventsTest
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
                var startTime = DateTime.UtcNow;
                sut.ActorOnActivateAsyncFinish(startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.ActorOnActivateAsyncFinish(startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ActorOnActivateAsyncFinish(startTime), Times.Once);
            }
        }

        public sealed class ActorMethod : AgregateDiagnosticEventsTest
        {

            [Fact]
            public void StartInvokesAllDiagnostics()
            {
                sut.ActorMethodStart(diagnosticContext, actorId, interfaceMethodKey, remotingListener);

                Mock.Get(diagnosticEvent).Verify(ds => ds.ActorMethodStart(diagnosticContext, actorId, interfaceMethodKey, remotingListener), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ActorMethodStart(diagnosticContext, actorId, interfaceMethodKey, remotingListener), Times.Once);
            }

            [Fact]
            public void FinishInvokesAllDiagnostics()
            {
                var startTime = DateTime.UtcNow;
                var exception = new InvalidOperationException("test exception");

                sut.ActorMethodFinish(diagnosticContext, startTime, actorId, interfaceMethodKey, exception, remotingListener);

                Mock.Get(diagnosticEvent).Verify(ds => ds.ActorMethodFinish(diagnosticContext, startTime, actorId, interfaceMethodKey, exception, remotingListener), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ActorMethodFinish(diagnosticContext, startTime, actorId, interfaceMethodKey, exception, remotingListener), Times.Once);
            }
        }

        public sealed class ActorStateLoad : AgregateDiagnosticEventsTest
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
                var startTime = DateTime.UtcNow;
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
                var startTime = DateTime.UtcNow;
                sut.SaveActorStateFinish(actorId, startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.SaveActorStateFinish(actorId, startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.SaveActorStateFinish(actorId, startTime), Times.Once);
            }
        }

        public sealed class ActorLock : AgregateDiagnosticEventsTest
        {
            [Fact]
            public void AcquireStartInvokesAllDiagnostics()
            {
                sut.AcquireActorLockStart(diagnosticContext);

                Mock.Get(diagnosticEvent).Verify(ds => ds.AcquireActorLockStart(diagnosticContext), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.AcquireActorLockStart(diagnosticContext), Times.Once);
            }

            [Fact]
            public void AcquireFailedInvokesAllDiagnostics()
            {
                sut.AcquireActorLockFailed(diagnosticContext);

                Mock.Get(diagnosticEvent).Verify(ds => ds.AcquireActorLockFailed(diagnosticContext), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.AcquireActorLockFailed(diagnosticContext), Times.Once);
            }

            [Fact]
            public void AcquireFinishInvokesAllDiagnostics()
            {
                var startTime = DateTime.UtcNow;
                sut.AcquireActorLockFinish(diagnosticContext, startTime, actorId);

                Mock.Get(diagnosticEvent).Verify(ds => ds.AcquireActorLockFinish(diagnosticContext, startTime, actorId), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.AcquireActorLockFinish(diagnosticContext, startTime, actorId), Times.Once);
            }

            [Fact]
            public void ReleaseInvokesAllDiagnostics()
            {
                var startTime = DateTime.UtcNow;
                sut.ReleaseActorLock(startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.ReleaseActorLock(startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.ReleaseActorLock(startTime), Times.Once);
            }
        }

        public sealed class ActorLifecycle : AgregateDiagnosticEventsTest
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
