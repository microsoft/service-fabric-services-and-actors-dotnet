// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
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

        public class OnRoleChange : ActorServiceTest, IDisposable
        {
            readonly IDiagnostics diagnosticEvents = Mock.Of<IDiagnostics>();

            readonly Func<ReplicaRole, CancellationToken, Task> sutMethod;
            readonly Func<ActorService, IClock, PerformanceCounterProviderV2, IDiagnostics> createDiagnosticEvents;

            public OnRoleChange()
            {
                sutMethod = actorService.DeclaredBy(typeof(ActorService)).Method<Func<ReplicaRole, CancellationToken, Task>>("OnChangeRoleAsync");

                // store createDiagnosticEvents function in order to restore it in Dispose()
                createDiagnosticEvents = typeof(ActorManager).Field<Func<ActorService, IClock, PerformanceCounterProviderV2, IDiagnostics>>().Value;
                typeof(ActorManager).Field<Func<ActorService, IClock, PerformanceCounterProviderV2, IDiagnostics>>().Set((actorService, clock, performanceCounterProvider) => diagnosticEvents);

                var actorManager = new ActorManager(actorService);
                actorService.Field<ActorManagerAdapter>().Value.ActorManager = actorManager;
            }

            public void Dispose()
            {
                // restore createDiagnosticEvents function
                typeof(ActorManager).Field<Func<ActorService, IClock, PerformanceCounterProviderV2, IDiagnostics>>().Set(createDiagnosticEvents);
            }

            [Fact]
            public void RoleChangePrimaryEmitsDiagnostics()
            {
                sutMethod.Invoke(ReplicaRole.Primary, TestContext.Current.CancellationToken);

                Mock.Get(diagnosticEvents).Verify(d => d.ActorChangeRole(It.IsAny<ReplicaRole>(), ReplicaRole.Primary), Times.Once);
            }

            [Fact]
            public void RoleChangeNonPrimaryEmitsDiagnostics()
            {
                sutMethod.Invoke(ReplicaRole.IdleSecondary, TestContext.Current.CancellationToken);

                Mock.Get(diagnosticEvents).Verify(d => d.ActorChangeRole(It.IsAny<ReplicaRole>(), ReplicaRole.IdleSecondary), Times.Once);
            }
        }
    }
}
