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
        readonly ActorService actorService = TestMocksRepository.GetActorService<MockActor>();

        public ActorServiceTest()
        {
            actorService.InitializeInternal(new ActorMethodFriendlyNameBuilder(actorService.ActorTypeInformation));
        }

        public class OnRoleChange : ActorServiceTest, IDisposable
        {
            readonly Func<ReplicaRole, CancellationToken, Task> sutMethod;
            readonly IDiagnosticEvents diagnosticEvents = Mock.Of<IDiagnosticEvents>();
            readonly Func<ActorService, IClock, IDiagnosticEvents> createDiagnosticEvents;

            public OnRoleChange()
            {
                var methodInfo = actorService.GetType().GetMethod("OnChangeRoleAsync",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    new[] { typeof(ReplicaRole), typeof(CancellationToken) },
                    null);
                sutMethod = (Func<ReplicaRole, CancellationToken, Task>)Delegate.CreateDelegate(typeof(Func<ReplicaRole, CancellationToken, Task>), actorService, methodInfo);

                createDiagnosticEvents = typeof(ActorManager).Field<Func<ActorService, IClock, IDiagnosticEvents>>().Value;
                typeof(ActorManager).Field<Func<ActorService, IClock, IDiagnosticEvents>>().Set((actorService, clock) => diagnosticEvents);

                var actorManager = new ActorManager(actorService);
                actorService.Field<ActorManagerAdapter>().Value.ActorManager = actorManager;
            }

            public void Dispose()
            {
                typeof(ActorManager).Field<Func<ActorService, IClock, IDiagnosticEvents>>().Set(createDiagnosticEvents);
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
