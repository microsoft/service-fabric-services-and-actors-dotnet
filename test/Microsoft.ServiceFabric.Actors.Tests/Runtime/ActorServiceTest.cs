// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Actors.Diagnostics;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.TestFramework;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    public class ActorServiceTest : MockedMetricsTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly ActorService sut;

        readonly StatefulServiceContext serviceContext = fuzzy.StatefulServiceContext();
        readonly ActorTypeInformation typeInformation = ActorTypeInformation.Get(typeof(TestActor));
        readonly Func<ServiceContext, ActorTypeInformation, ActorMethodFriendlyNameBuilder, DiagnosticsFactory> createDiagnosticsFactory;
        readonly Mock<Func<ServiceContext, ActorTypeInformation, ActorMethodFriendlyNameBuilder, DiagnosticsFactory>> mockCreateDiagnosticsFactory = new Mock<Func<ServiceContext, ActorTypeInformation, ActorMethodFriendlyNameBuilder, DiagnosticsFactory>>();
        readonly DiagnosticsFactory diagnosticsFactory;

        public ActorServiceTest()
        {
            diagnosticsFactory = new Mock<DiagnosticsFactory>(serviceContext, typeInformation, new ActorMethodFriendlyNameBuilder(typeInformation)).Object;

            this.createDiagnosticsFactory = typeof(ActorService).Field<Func<ServiceContext, ActorTypeInformation, ActorMethodFriendlyNameBuilder, DiagnosticsFactory>>().Value;
            typeof(ActorService).Field<Func<ServiceContext, ActorTypeInformation, ActorMethodFriendlyNameBuilder, DiagnosticsFactory>>().Set(mockCreateDiagnosticsFactory.Object);
            mockCreateDiagnosticsFactory.Setup(_ => _.Invoke(It.IsAny<ServiceContext>(), It.IsAny<ActorTypeInformation>(), It.IsAny<ActorMethodFriendlyNameBuilder>())).Returns(diagnosticsFactory);

            sut = new ActorService(serviceContext, typeInformation);
            sut.InitializeInternal(new ActorMethodFriendlyNameBuilder(sut.ActorTypeInformation));
        }

        public override void Dispose()
        {
            base.Dispose();
            typeof(ActorService).Field<Func<ServiceContext, ActorTypeInformation, ActorMethodFriendlyNameBuilder, DiagnosticsFactory>>().Set(this.createDiagnosticsFactory);
        }

        public class Diagnostics : ActorServiceTest
        {
            [Fact]
            public void IsCreatedByConstructor()
            {
                mockCreateDiagnosticsFactory.Verify(d => d.Invoke(It.Is<ServiceContext>(c => c == serviceContext), It.Is<ActorTypeInformation>(i => i == typeInformation), It.IsAny<ActorMethodFriendlyNameBuilder>()), Times.Once);
                Mock.Get(diagnosticsFactory).Verify(d => d.CreateDiagnostics(It.IsAny<IClock>()), Times.Once);
            }

            [Fact]
            public void IsDisposedByOnCloseAsync()
            {
                sut.DeclaredBy(typeof(ActorService)).Method<Func<CancellationToken, Task>>("OnCloseAsync").Invoke(TestContext.Current.CancellationToken);

                Mock.Get(diagnosticsFactory).Verify(d => d.Dispose(), Times.Once);
            }
        }

        public class OnRoleChange : ActorServiceTest
        {
            readonly IDiagnostics diagnostics = Mock.Of<IDiagnostics>();

            readonly Func<ReplicaRole, CancellationToken, Task> sutMethod;

            public OnRoleChange()
            {
                sut.Field<IDiagnostics>().Set(diagnostics);
                sutMethod = sut.DeclaredBy(typeof(ActorService)).Method<Func<ReplicaRole, CancellationToken, Task>>("OnChangeRoleAsync");

                var actorManager = new ActorManager(sut, Mock.Of<IClock>(), diagnostics);
                sut.Field<ActorManagerAdapter>().Value.ActorManager = actorManager;
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
