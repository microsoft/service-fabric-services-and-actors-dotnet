// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inspector;
using Microsoft.ServiceFabric.Actors.Diagnostics;
using Microsoft.ServiceFabric.Actors.Tests;
using Microsoft.ServiceFabric.Diagnostics;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    public class ActorServiceTest
    {
        readonly ActorService actorService = TestMocksRepository.GetActorService<TestActor>();

        public ActorServiceTest()
        {
            actorService.InitializeInternal(new ActorMethodFriendlyNameBuilder(actorService.ActorTypeInformation));
        }

        public class Constructor : ActorServiceTest
        {
            [Fact]
            public void DiagnosticsEventsHasAllNeededEventsRegistered()
            {
                AggregatedDiagnosticEvents field = (AggregatedDiagnosticEvents)actorService.Field<IDiagnostics>().Value;
                var registeredDiagnosticEvents = field.Field<IEnumerable<IDiagnostics>>().Value;

                Assert.Equal(2, registeredDiagnosticEvents.Count());
                Assert.IsType<PerformanceCounterDiagnosticEvents>(registeredDiagnosticEvents.ToList()[0]);
                Assert.IsType<EventSourceDiagnosticEvents>(registeredDiagnosticEvents.ToList()[1]);
            }
        }

        public class OnRoleChange : ActorServiceTest
        {
            readonly IDiagnostics diagnostics = Mock.Of<IDiagnostics>();

            readonly Func<ReplicaRole, CancellationToken, Task> sutMethod;

            public OnRoleChange()
            {
                actorService.Field<IDiagnostics>().Set(diagnostics);
                sutMethod = actorService.DeclaredBy(typeof(ActorService)).Method<Func<ReplicaRole, CancellationToken, Task>>("OnChangeRoleAsync");

                var actorManager = new ActorManager(actorService, Mock.Of<IClock>(), diagnostics);
                actorService.Field<ActorManagerAdapter>().Value.ActorManager = actorManager;
            }

            [Fact]
            public void RoleChangePrimaryEmitsDiagnostics()
            {
                sutMethod.Invoke(ReplicaRole.Primary, TestContext.Current.CancellationToken);

                Mock.Get(diagnostics).Verify(d => d.ActorChangeRole(It.IsAny<ReplicaRole>(), ReplicaRole.Primary), Times.Once);
            }

            [Fact]
            public void RoleChangeNonPrimaryEmitsDiagnostics()
            {
                sutMethod.Invoke(ReplicaRole.IdleSecondary, TestContext.Current.CancellationToken);

                Mock.Get(diagnostics).Verify(d => d.ActorChangeRole(It.IsAny<ReplicaRole>(), ReplicaRole.IdleSecondary), Times.Once);
            }
        }
    }
}
